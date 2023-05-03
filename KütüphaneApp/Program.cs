using Businnes.Abstract;
using Businnes.Concrete;
using Data.Abstract;
using Data.Concrete;
using KütüphaneApp.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer("Server=.;Database=Library.Db;User Id=sa;Password=123WsX.456;Encrypt=False"));
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options =>
{
    //password
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;

    //Locout
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(5);
    options.Lockout.AllowedForNewUsers = true;

    //UserName
    //options.User.AllowedUserNameCharacters = "";

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.LogoutPath = "/account/logout";
    options.AccessDeniedPath = "/account/accessdenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.Cookie = new CookieBuilder
    {
        HttpOnly = false,
        Name = "LibraryApp.Security.Cookie",
    };
});

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IBookRepository, EfCorerBookRepository>();
builder.Services.AddScoped<IBookService, BookManager>();

builder.Services.AddScoped<ICategoryRepository, EfCoreCategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();

builder.Services.AddScoped<ICartRepository, EfCoreCartRepository>();
builder.Services.AddScoped<ICartService, CartManager>();


var app = builder.Build();




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
         name: "books",
         pattern: "books/{category?}",
         defaults: new { controller = "Home", action = "BookList" }
       );

    //roles
    endpoints.MapControllerRoute(
      name: "adminroles",
      pattern: "admin/role/list",
      defaults: new { controller = "Admin", action = "RoleList" }
   );
    endpoints.MapControllerRoute(
      name: "adminroleedit",
      pattern: "admin/role/{id?}",
      defaults: new { controller = "Admin", action = "RoleEdit" }
   );
    endpoints.MapControllerRoute(
      name: "adminrolecreate",
      pattern: "admin/role/create",
      defaults: new { controller = "Admin", action = "RoleCreate" }
   );

    //users
    endpoints.MapControllerRoute(
     name: "adminusers",
     pattern: "admin/user/list",
     defaults: new { controller = "Admin", action = "UserList" }
     );
    endpoints.MapControllerRoute(
     name: "adminuseredit",
     pattern: "admin/user/{id}",
     defaults: new { controller = "Admin", action = "UserEdit" }
     );

});

app.Run();
