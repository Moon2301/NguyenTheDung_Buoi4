using Microsoft.EntityFrameworkCore;
using NguyenTheDung_Buoi4.Models;

namespace NguyenTheDung_Buoi4.Repositories
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private readonly MyDbContext _context;
        public EFCategoryRepository(MyDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            // return await _context.Categorys.ToListAsync();
            return await _context.Categories.ToListAsync();
        }
        public async Task<Category> GetByIdAsync(int id)
        {
            // return await _context.Categorys.FindAsync(id);
            // lấy thông tin kèm theo category
            return await _context.Categories.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task AddAsync(Category Category)
        {
            _context.Categories.Add(Category);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Category Category)
        {
            _context.Categories.Update(Category);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var Category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(Category);
            await _context.SaveChangesAsync();
        }
    }
}
