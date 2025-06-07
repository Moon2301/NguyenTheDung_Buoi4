using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NguyenTheDung_Buoi4.Models;

[Authorize(Roles = "Admin")]
[Area("Admin")]
public class AdminOrdersController : Controller
{
    private readonly MyDbContext _context;

    public AdminOrdersController(MyDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders
            .Include(o => o.ApplicationUser)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }


    public async Task<IActionResult> Details(int id)
    {
        var order = await _context.Orders
            .Include(o => o.ApplicationUser)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        order.Status = status;
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = order.Id });
    }

}
