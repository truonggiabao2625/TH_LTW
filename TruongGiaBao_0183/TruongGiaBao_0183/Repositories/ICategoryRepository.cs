using TruongGiaBao_0183.Models;

namespace TruongGiaBao_0183.Repositories
{
    public interface ICategoryRepository { 
        Task<IEnumerable<Category>> GetAllAsync(); 
        Task<Category> GetByIdAsync(int id); 
        Task AddAsync(Category category); 
        Task UpdateAsync(Category category); 
        Task DeleteAsync(int id); 
    }
}
