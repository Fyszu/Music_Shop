using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Music_Shop.Data;
using Music_Shop.Repositories;
using Music_Shop.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MyDbContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate();
builder.Services.AddHttpContextAccessor();

//User register password conditions
builder.Services.AddIdentity<User, IdentityRole>(
    options => {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 8;
        options.Password.RequiredUniqueChars = 1;
    }
    ).AddEntityFrameworkStores<MyDbContext>();

//Cookies and redirection options
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

//Services and repositories injection
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IAlbumService, AlbumService>();
builder.Services.AddTransient<IArtistService, ArtistService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddScoped<AlbumRepository, AlbumRepository>();
builder.Services.AddScoped<ArtistRepository, ArtistRepository>();
builder.Services.AddScoped<UserRepository, UserRepository>();
builder.Services.AddScoped<OrderRepository, OrderRepository>();

builder.Services.AddControllersWithViews().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseExceptionHandler("/Error"); // to removal
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Main}/{id?}" );

app.MapRazorPages();

app.Run();
