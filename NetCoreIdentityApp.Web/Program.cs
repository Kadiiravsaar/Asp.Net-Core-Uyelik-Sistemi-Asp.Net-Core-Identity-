using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using NetCoreIdentityApp.Web.ClaimProvider;
using NetCoreIdentityApp.Web.Extensions;
using NetCoreIdentityApp.Web.Models;
using NetCoreIdentityApp.Web.OptionsModels;
using NetCoreIdentityApp.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

builder.Services.AddIdentityWithExt();


builder.Services.ConfigureApplicationCookie(opt =>
{ // Identity'nin �erez tabanl� kimlik do�rulama hizmetini yap�land�r�r.

    var cookieBuilder = new CookieBuilder(); // CookieBuilder s�n�f�, kimlik do�rulama �erezinin �zelliklerini ayarlamak i�in y�ntemler sa�lar.

    cookieBuilder.Name = "IdentityLearn";
    // Bu sat�r, �erezin ad�n� "IdentityLearn" olarak ayarlar. Varsay�lan olarak, �erez ad� ".AspNetCore.Identity.Application".Daha iyi tan�mlama i�in burada �zelle�tirebilirsiniz.

    opt.LoginPath = new PathString("/Home/SignIn");
    //Kullan�c�lar korunan bir kayna�a eri�meye �al���rken ve kimlik do�rulamas� yapmam��sa, oturum a�mak i�in bu yola y�nlendirilirler.

    opt.AccessDeniedPath = new PathString("/Member/AccessDenied");
    opt.LogoutPath = new PathString("/Member/Logout");


    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan = TimeSpan.FromDays(30);
    //Bu sat�r, kimlik do�rulama �erezinin son kullanma s�resini ayarlar.

    opt.SlidingExpiration = true;
    //Bu, kullan�c�n�n uygulamada aktif olarak kulland��� s�rece oturum a�m�� kalmas�n� sa�lar, bu da ExpireTimeSpan'dan daha uzun olsa bile.

});

builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory())); // benim IFileProvider bunu ge�ti�im yerde �al��aca��m� bil
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();

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

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
