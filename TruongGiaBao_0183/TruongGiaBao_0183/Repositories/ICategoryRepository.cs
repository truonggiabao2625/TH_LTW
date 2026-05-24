using TruongGiaBao_0183.Models;

namespace TruongGiaBao_0183.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategories();
    }
}
