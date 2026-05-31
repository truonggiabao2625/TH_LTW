
namespace TruongGiaBao_0183.Repositories
{
    using System.Collections.Generic;
    using TruongGiaBao_0183.Models;
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync(); 
        Task<Product> GetByIdAsync(int id); 
        Task AddAsync(Product product); 
        Task UpdateAsync(Product product); 
        Task DeleteAsync(int id);
    }
}
