using Microsoft.EntityFrameworkCore;

using Blogio.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(opts => {
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:MainConnection"]);
});

builder.Services.AddDbContext<IdentityContext>(opts => {
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:IdentityConnection"]);
});

builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<IdentityContext>();

var app = builder.Build();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();

// var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
// Blogio.SeedPosts.Seed(context);

app.Run();
