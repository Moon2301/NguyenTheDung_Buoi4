using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NguyenTheDung_Buoi4.Models;
using NguyenTheDung_Buoi4.Repositories;

namespace NguyenTheDung_Buoi4.Controllers
{
    [AllowAnonymous]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public ProductController(IProductRepository productRepository,
        ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View("Index", products); 
        }

        // Hiển thị form thêm sản phẩm mới
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Company + "," + SD.Role_Employee)]
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }
        // Xử lý thêm sản phẩm mới
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Company + "," + SD.Role_Employee)]
        public async Task<IActionResult> Add(Product product, IFormFile
        imageUrl)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    // Lưu hình ảnh đại diện tham khảo bài 02 hàm SaveImage
                    product.ImageUrl = await SaveImage(imageUrl);
                }
                await _productRepository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }
        // Viết thêm hàm SaveImage (tham khảo bài 02)
        private async Task<string> SaveImage(IFormFile image)
        {
            //Thay đổi đường dẫn theo cấu hình của bạn
            var savePath = Path.Combine("wwwroot/images", image.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName; // Trả về đường dẫn tương đối
        }
        //Nhớ tạo folder images trong wwwroot

        // Hiển thị thông tin chi tiết sản phẩm
        [AllowAnonymous]
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // Hiển thị sản phẩm theo danh mục
        public async Task<IActionResult> ByCategory(int? categoryId, string keyword)
        {
            IEnumerable<Product> products;

            if (categoryId.HasValue)
            {
                products = await _productRepository.GetByCategoryAsync(categoryId.Value);
            }
            else
            {
                products = await _productRepository.GetAllAsync();
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                products = products.Where(p => p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SearchKeyword = keyword;

            return View("Index", products);
        }

    }
}
