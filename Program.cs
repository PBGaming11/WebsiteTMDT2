using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using WebsiteTMDT.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.Password.RequireUppercase = false)
    .AddDefaultTokenProviders().AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddViewOptions(options =>
                {
                    options.HtmlHelperOptions.ClientValidationEnabled = true;
                });
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("SellerOnly", policy => policy.RequireRole("Seller"));
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseEndpoints(endpoints =>
{
    // Routing cho các area
    endpoints.MapControllerRoute(
        name: "admin_default",
        pattern: "Admin/{controller=Admin}/{action=Index}/{id?}",
        defaults: new { area = "Admin" });
    endpoints.MapControllerRoute(
        name: "seller_default",
        pattern: "Seller/{controller=Home}/{action=Index}/{id?}",
        defaults: new { area = "Seller" });

    endpoints.MapControllerRoute(
        name: "user_default",
        pattern: "{controller=Home}/{action=Index}/{id?}",
        defaults: new { area = "User" });


    endpoints.MapControllerRoute(
        name: "category",
        pattern: "danh-muc/{categoryAlias}",
        defaults: new { area = "User", controller = "Shop", action = "Index" });

    endpoints.MapControllerRoute(
        name: "productDetail",
        pattern: "{alias}-p{id}",
        defaults: new { area = "User", controller = "ProductDetails", action = "Index" });

    endpoints.MapControllerRoute(
        name: "gioHang",
        pattern: "gio-hang",
        defaults: new { area = "User", controller = "Cart", action = "Index" });

    endpoints.MapControllerRoute(
        name: "thanhtoan",
        pattern: "thanh-toan",
        defaults: new { area = "User", controller = "CheckOut", action = "Index" });

    endpoints.MapRazorPages();
});


app.MapRazorPages();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.Run();