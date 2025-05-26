using NguyenTheDung_Buoi4.Models;

namespace NguyenTheDung_Buoi4.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);

        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
