using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NetCoreIdentityApp.Web.Extensions;
using NetCoreIdentityApp.Web.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

builder.Services.AddIdentityWithExt();


builder.Services.ConfigureApplicationCookie(opt =>
{ // Identity'nin çerez tabanlý kimlik doðrulama hizmetini yapýlandýrýr.

    var cookieBuilder = new CookieBuilder(); // CookieBuilder sýnýfý, kimlik doðrulama çerezinin özelliklerini ayarlamak için yöntemler saðlar.

    cookieBuilder.Name = "IdentityLearn";
    // Bu satýr, çerezin adýný "IdentityLearn" olarak ayarlar. Varsayýlan olarak, çerez adý ".AspNetCore.Identity.Application".Daha iyi tanýmlama için burada özelleþtirebilirsiniz.

    opt.LoginPath = new PathString("/Home/SignIn");
    //Kullanýcýlar korunan bir kaynaða eriþmeye çalýþýrken ve kimlik doðrulamasý yapmamýþsa, oturum açmak için bu yola yönlendirilirler.

    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan = TimeSpan.FromDays(30);
    //Bu satýr, kimlik doðrulama çerezinin son kullanma süresini ayarlar.

    opt.SlidingExpiration = true;
    //Bu, kullanýcýnýn uygulamada aktif olarak kullandýðý sürece oturum açmýþ kalmasýný saðlar, bu da ExpireTimeSpan'dan daha uzun olsa bile.

});


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
