namespace TruongGiaBao_0183.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using TruongGiaBao_0183.Models;

    public class MockProductRepository : IProductRepository
    {
        private readonly List<Product> _products;
        public MockProductRepository()
        {
            _products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Laptop Dell Inspiron",
                    Price = 15000000,
                    Description = "Laptop học tập và làm việc với hiệu năng ổn định.",
                    CategoryId = 1,
                    ImageUrl = "/images/aebd708c-1f7b-4852-800e-97e342268249_laptop.jpg"
                },
                new Product
                {
                    Id = 2,
                    Name = "Laptop Asus Vivobook",
                    Price = 18500000,
                    Description = "Thiết kế mỏng nhẹ, màn hình sắc nét.",
                    CategoryId = 1,
                    ImageUrl = "/images/d4f6b138-3a3d-4290-8646-6af3710f777c_laptop.jpg"
                },
                new Product
                {
                    Id = 3,
                    Name = "Tai nghe Bluetooth",
                    Price = 1200000,
                    Description = "Âm thanh rõ, pin lâu, kết nối nhanh.",
                    CategoryId = 2,
                    ImageUrl = "/images/5a2132ef-5bda-499c-b538-9d73541144fd_tải xuống.jpg"
                }
            };
        }
        public IEnumerable<Product> GetAll()
        {
            return _products;
        }
        public Product GetById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }
        public void Add(Product product)
        {
            product.Id = _products.Any() ? _products.Max(p => p.Id) + 1 : 1;
            _products.Add(product);
        }
        public void Update(Product product)
        {
            var index = _products.FindIndex(p => p.Id == product.Id);
            if (index != -1)
            {
                _products[index] = product;
            }
        }
        public void Delete(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
            }
        }
    }
}
