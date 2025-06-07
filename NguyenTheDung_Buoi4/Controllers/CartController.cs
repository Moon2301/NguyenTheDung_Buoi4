using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NguyenTheDung_Buoi4.Models;
using NguyenTheDung_Buoi4.Repositories;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class CartController : Controller
{
    private const string CARTKEY = "CART";
    private readonly IProductRepository _productRepo;
    private readonly MyDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartController(IProductRepository productRepo, MyDbContext context, UserManager<ApplicationUser> userManager)
    {
        _productRepo = productRepo;
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int id)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product == null) return NotFound();

        // Lấy cart từ session
        var cart = HttpContext.Session.Get<ShoppingCart>(CARTKEY) ?? new ShoppingCart();

        // Thêm vào cart
        cart.AddItem(new CartItem
        {
            ProductId = product.Id,
            Name = product.Name,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            Quantity = 1
        });

        HttpContext.Session.Set(CARTKEY, cart);
        return Ok();
    }

    public IActionResult Index()
    {
        var cart = HttpContext.Session.Get<ShoppingCart>(CARTKEY) ?? new ShoppingCart();
        return View(cart);
    }

    [HttpPost]
    public IActionResult UpdateQuantity(int productId, int quantity)
    {
        var cart = HttpContext.Session.Get<ShoppingCart>("CART") ?? new ShoppingCart();

        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            item.Quantity = quantity > 0 ? quantity : 1;
            HttpContext.Session.Set("CART", cart);
        }

        return RedirectToAction("Index");
    }

    public IActionResult Remove(int id)
    {
        var cart = HttpContext.Session.Get<ShoppingCart>(CARTKEY) ?? new ShoppingCart();
        cart.RemoveItem(id);
        HttpContext.Session.Set(CARTKEY, cart);
        return RedirectToAction("Index");
    }

    public IActionResult Checkout()
    {
        return View(new Order());
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(Order order)
    {
        var cart = HttpContext.Session.Get<ShoppingCart>(CARTKEY);
        if (cart == null || !cart.Items.Any())
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index");
        }

        var user = await _userManager.GetUserAsync(User);

        order.UserId = user.Id;
        order.OrderDate = DateTime.UtcNow;
        order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
        order.OrderDetails = cart.Items.Select(i => new OrderDetail
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            Price = i.Price
        }).ToList();

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        HttpContext.Session.Remove(CARTKEY); // clear cart

        return RedirectToAction("OrderCompleted", new { id = order.Id });
    }

    public IActionResult OrderCompleted(int id)
    {
        return View(id);
    }

    [Authorize]
    public async Task<IActionResult> MyOrders()
    {
        var user = await _userManager.GetUserAsync(User);

        var orders = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .Where(o => o.UserId == user.Id)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }
    [Authorize]
    public async Task<IActionResult> OrderDetails(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }


}
