using Microsoft.EntityFrameworkCore;
using TruongGiaBao_0183.DataAccess;
using TruongGiaBao_0183.Repositories;
using TruongGiaBao_0183.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new Category { Name = "Điện thoại" },
                new Category { Name = "Laptop" },
                new Category { Name = "Phụ kiện" },
                new Category { Name = "Smartwatch" },
                new Category { Name = "Tablet" }
            );
            context.SaveChanges();
        }

        var phoneCat = context.Categories.FirstOrDefault(c => c.Name == "Điện thoại") ?? context.Categories.First();
        var laptopCat = context.Categories.FirstOrDefault(c => c.Name == "Laptop") ?? context.Categories.First();
        var phukienCat = context.Categories.FirstOrDefault(c => c.Name == "Phụ kiện") ?? context.Categories.First();
        var watchCat = context.Categories.FirstOrDefault(c => c.Name == "Smartwatch") ?? context.Categories.First();
        var tabletCat = context.Categories.FirstOrDefault(c => c.Name == "Tablet") ?? context.Categories.First();

        var productsToSeed = new List<Product>
        {
            new Product
            {
                Name = "iPhone 15 Pro Max 256GB",
                Price = 29990000,
                Description = "iPhone 15 Pro Max là siêu phẩm điện thoại mới nhất từ Apple sở hữu khung titan siêu nhẹ, nút Action mới và hệ thống camera zoom quang học 5x tiên tiến nhất hiện nay.",
                CategoryId = phoneCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=600&auto=format&fit=crop&q=60",
                ProductImages = new List<ProductImage>
                {
                    new ProductImage { Url = "https://images.unsplash.com/photo-1695048132924-f7b5264b3ef8?w=600&auto=format&fit=crop&q=60" },
                    new ProductImage { Url = "https://images.unsplash.com/photo-1695048133044-66ee34091924?w=600&auto=format&fit=crop&q=60" }
                }
            },
            new Product
            {
                Name = "MacBook Pro 16 inch M3 Max",
                Price = 79990000,
                Description = "MacBook Pro 16 inch với chip M3 Max mang lại hiệu năng tối thượng cho các tác vụ chuyên nghiệp nặng nhất như render 3D, lập trình AI, edit video 8K.",
                CategoryId = laptopCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=600&auto=format&fit=crop&q=60",
                ProductImages = new List<ProductImage>
                {
                    new ProductImage { Url = "https://images.unsplash.com/photo-1611186871348-b1ce696e52c9?w=600&auto=format&fit=crop&q=60" }
                }
            },
            new Product
            {
                Name = "Tai nghe Sony WH-1000XM5",
                Price = 6490000,
                Description = "Tai nghe chụp tai chống ồn chủ động Sony WH-1000XM5 với bộ xử lý chống ồn V1 và HD QN1 chuyên biệt mang đến không gian âm nhạc hoàn hảo, tĩnh lặng hoàn toàn.",
                CategoryId = phukienCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=600&auto=format&fit=crop&q=60",
                ProductImages = new List<ProductImage>
                {
                    new ProductImage { Url = "https://images.unsplash.com/photo-1583394838336-acd977736f90?w=600&auto=format&fit=crop&q=60" },
                    new ProductImage { Url = "https://images.unsplash.com/photo-1487215078519-e21cc028cb29?w=600&auto=format&fit=crop&q=60" }
                }
            },
            new Product
            {
                Name = "Apple Watch Ultra 2 GPS + Cellular",
                Price = 21490000,
                Description = "Đồng hồ thể thao chuyên nghiệp Apple Watch Ultra 2 với thiết kế vỏ titan siêu bền, độ sáng màn hình lên đến 3000 nits và thời lượng pin vượt trội lên tới 72 giờ ở chế độ tiết kiệm pin.",
                CategoryId = watchCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1434494878577-86c23bcb06b9?w=600&auto=format&fit=crop&q=60",
                ProductImages = new List<ProductImage>
                {
                    new ProductImage { Url = "https://images.unsplash.com/photo-1508685096489-7aacd43bd3b1?w=600&auto=format&fit=crop&q=60" }
                }
            },
            new Product
            {
                Name = "iPad Pro M4 11 inch Wifi 256GB",
                Price = 28990000,
                Description = "iPad Pro thế hệ mới với chip M4 đột phá cùng màn hình OLED Ultra Retina XDR siêu sáng, siêu mỏng nhẹ mang đến trải nghiệm hiển thị và hiệu năng chưa từng có.",
                CategoryId = tabletCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=600&auto=format&fit=crop&q=60",
                ProductImages = new List<ProductImage>
                {
                    new ProductImage { Url = "https://images.unsplash.com/photo-1589739900243-4b52cd9b104e?w=600&auto=format&fit=crop&q=60" }
                }
            },
            new Product
            {
                Name = "Máy chơi game ASUS ROG Ally Z1 Extreme",
                Price = 17990000,
                Description = "ASUS ROG Ally là máy chơi game cầm tay chạy Windows 11 mạnh mẽ sở hữu vi xử lý AMD Ryzen Z1 Extreme, màn hình 120Hz mượt mà chơi tốt mọi tựa game AAA PC.",
                CategoryId = laptopCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1605901309584-818e25960a8f?w=600&auto=format&fit=crop&q=60",
                ProductImages = new List<ProductImage>
                {
                    new ProductImage { Url = "https://images.unsplash.com/photo-1531525645387-7f14be1bdbbd?w=600&auto=format&fit=crop&q=60" }
                }
            }
        };

        foreach (var p in productsToSeed)
        {
            if (!context.Products.Any(prod => prod.Name == p.Name))
            {
                context.Products.Add(p);
            }
        }
        context.SaveChanges();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Có lỗi xảy ra khi seed dữ liệu.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
