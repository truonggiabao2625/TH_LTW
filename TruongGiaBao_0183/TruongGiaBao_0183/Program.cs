using Microsoft.EntityFrameworkCore;
using TruongGiaBao_0183.DataAccess;
using TruongGiaBao_0183.Repositories;
using TruongGiaBao_0183.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllersWithViews();

// Thêm dịch vụ lưu cache bộ nhớ đệm cho Session
builder.Services.AddDistributedMemoryCache();

// Cấu hình dịch vụ Session
builder.Services.AddSession(options =>
{
    // Thiết lập thời gian hết hạn Session là 30 phút
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    // Chỉ cho phép truy cập Cookie qua giao thức HTTP (tăng bảo mật)
    options.Cookie.HttpOnly = true;
    // Đánh dấu cookie Session là thiết yếu để chạy ứng dụng
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
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

        // Seed roles & admin user
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // Tạo các Roles nếu chưa tồn tại
        var rolesToSeed = new string[] { "Admin", "Employee", "Customer", "Company" };
        foreach (var roleName in rolesToSeed)
        {
            if (!roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
            {
                roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
            }
        }

        // Tạo User Admin nếu chưa tồn tại
        var adminEmail = "admin@techstore.com";
        var adminUser = userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult();
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Administrator",
                EmailConfirmed = true
            };
            var createResult = userManager.CreateAsync(adminUser, "Admin@123").GetAwaiter().GetResult();
            if (createResult.Succeeded)
            {
                userManager.AddToRoleAsync(adminUser, "Admin").GetAwaiter().GetResult();
            }
        }

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

        // Dọn dẹp dữ liệu cũ (bao gồm cả các đơn đặt hàng test cũ để tránh lỗi ràng buộc khóa ngoại)
        try
        {
            if (context.OrderDetails.Any())
            {
                context.OrderDetails.RemoveRange(context.OrderDetails);
            }
            if (context.Orders.Any())
            {
                context.Orders.RemoveRange(context.Orders);
            }
            if (context.ProductImages.Any())
            {
                context.ProductImages.RemoveRange(context.ProductImages);
            }
            if (context.Products.Any())
            {
                context.Products.RemoveRange(context.Products);
            }
            context.SaveChanges();
        }
        catch (Exception)
        {
            // Bỏ qua lỗi ràng buộc nếu có để đảm bảo chạy mượt mà
        }

        var productsToSeed = new List<Product>
        {
            new Product
            {
                Name = "iPhone 15 Pro Max 256GB",
                Price = 29990000,
                Description = "Siêu phẩm điện thoại mới nhất từ Apple sở hữu khung titan siêu nhẹ, nút Action mới và hệ thống camera zoom quang học 5x tiên tiến.",
                CategoryId = phoneCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=600&auto=format&fit=crop&q=60"
            },
            new Product
            {
                Name = "Samsung Galaxy S24 Ultra 256GB",
                Price = 31990000,
                Description = "Điện thoại Android đỉnh cao nhất của Samsung với camera 200MP, bút S-Pen tiện lợi và bộ xử lý Snapdragon 8 Gen 3 cực mạnh mẽ.",
                CategoryId = phoneCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?w=600&auto=format&fit=crop&q=60"
            },
            new Product
            {
                Name = "Xiaomi 14 Ultra 5G",
                Price = 29990000,
                Description = "Flagship cao cấp của Xiaomi hợp tác cùng thương hiệu camera Leica danh tiếng mang lại chất lượng ảnh chụp nghệ thuật đỉnh cao.",
                CategoryId = phoneCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=600&auto=format&fit=crop&q=60"
            },
            new Product
            {
                Name = "MacBook Pro 16 inch M3 Max",
                Price = 79990000,
                Description = "MacBook Pro 16 inch với chip M3 Max mang lại hiệu năng tối thượng cho các tác vụ chuyên nghiệp nặng nhất như lập trình AI, dựng phim 8K.",
                CategoryId = laptopCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=600&auto=format&fit=crop&q=60"
            },
            new Product
            {
                Name = "ASUS ROG Zephyrus G14 OLED",
                Price = 48990000,
                Description = "Laptop gaming mỏng nhẹ cao cấp nhất hiện nay sở hữu màn hình OLED 120Hz siêu đẹp cùng card đồ họa RTX 4060 chiến mọi game AAA.",
                CategoryId = laptopCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=600&auto=format&fit=crop&q=60"
            },
            new Product
            {
                Name = "Dell XPS 13 Plus 9320",
                Price = 45990000,
                Description = "Chiếc ultrabook siêu sang trọng của Dell với thiết kế bàn phím tràn viền vô cực và thanh touchbar cảm ứng hiện đại bậc nhất.",
                CategoryId = laptopCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1588872657578-7efd1f1555ed?w=600&auto=format&fit=crop&q=60"
            },
            new Product
            {
                Name = "iPad Pro M4 11 inch Wifi 256GB",
                Price = 28990000,
                Description = "iPad Pro thế hệ mới đột phá với chip Apple M4 cùng màn hình Ultra Retina XDR siêu sáng, siêu mỏng nhẹ bậc nhất thế giới.",
                CategoryId = tabletCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=600&auto=format&fit=crop&q=60"
            },
            new Product
            {
                Name = "Samsung Galaxy Tab S9 Ultra",
                Price = 26490000,
                Description = "Máy tính bảng màn hình Dynamic AMOLED 2X rộng 14.6 inch siêu khổng lồ, hỗ trợ kháng nước bụi IP68 và kèm bút S-Pen chuyên nghiệp.",
                CategoryId = tabletCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1542751371-adc38448a05e?w=600&auto=format&fit=crop&q=60"
            },
            new Product
            {
                Name = "Apple Watch Ultra 2 GPS + Cellular",
                Price = 21490000,
                Description = "Đồng hồ thể thao chuyên nghiệp với khung vỏ titan siêu bền, độ sáng màn hình 3000 nits và thời lượng pin cực dài lên đến 72 giờ.",
                CategoryId = watchCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1434494878577-86c23bcb06b9?w=600&auto=format&fit=crop&q=60"
            },
            new Product
            {
                Name = "Tai nghe Sony WH-1000XM5",
                Price = 6490000,
                Description = "Tai nghe chụp tai chống ồn chủ động đỉnh cao Sony WH-1000XM5 mang lại không gian âm thanh tuyệt hảo và thời lượng pin 30 giờ liên tục.",
                CategoryId = phukienCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=600&auto=format&fit=crop&q=60"
            },
            new Product
            {
                Name = "Bàn phím cơ Keychron K2 V2 RGB",
                Price = 1990000,
                Description = "Bàn phím cơ không dây nhỏ gọn 84 phím với đèn nền RGB, hỗ trợ kết nối bluetooth đa thiết bị mượt mà giữa macOS và Windows.",
                CategoryId = phukienCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1618384887929-16ec33fab9ef?w=600&auto=format&fit=crop&q=60"
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

// Sử dụng Session trước bước xác thực quyền truy cập
app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
