
namespace TruongGiaBao_0183.Repositories
{
    using System.Collections.Generic;
    using TruongGiaBao_0183.Models;
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        Product GetById(int id);
        void Add(Product product);
        void Update(Product product);
        void Delete(int id);
    }
}
