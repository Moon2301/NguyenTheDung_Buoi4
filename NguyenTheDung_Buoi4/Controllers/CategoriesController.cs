using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NguyenTheDung_Buoi4.Models;
using NguyenTheDung_Buoi4.Repositories;

namespace NguyenTheDung_Buoi4.Controllers { 

    public class CategoriesController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public CategoriesController(IProductRepository productRepository,
       ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index()
        {
            var category = await _categoryRepository.GetAllAsync();
            return View(category);
        }
        public async Task<IActionResult> Display(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
    }

}
