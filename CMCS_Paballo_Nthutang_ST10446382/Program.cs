// Program.cs - Paballo Nthutang ST10446382 - FINAL WORKING
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CMCS_Paballo_Nthutang_ST10446382.Data;
using CMCS_Paballo_Nthutang_ST10446382.Models;
using CMCS_Paballo_Nthutang_ST10446382.Hubs;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ??
        "Server=(localdb)\\mssqllocaldb;Database=CMCS_Paballo_ST10446382;Trusted_Connection=true;MultipleActiveResultSets=true"));

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    // Simple seed
    string[] roles = { "Lecturer", "Coordinator", "AcademicManager", "HR" };
    foreach (var role in roles)
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));

    if (await userManager.FindByEmailAsync("lecturer@cmcs.ac.za") == null)
    {
        var user = new AppUser { UserName = "lecturer@cmcs.ac.za", Email = "lecturer@cmcs.ac.za", FullName = "Paballo Nthutang" };
        await userManager.CreateAsync(user, "Pass123!");
        await userManager.AddToRoleAsync(user, "Lecturer");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ClaimHub>("/claimHub");

async Task SeedRolesAsync(IHost app)
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Lecturer", "Coordinator", "AcademicManager", "HR" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

await SeedRolesAsync(app);

async Task SeedUsersAsync(IHost app)
{
    using var scope = app.Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    async Task CreateUser(string email, string role)
    {
        if (await userManager.FindByEmailAsync(email) == null)
        {
            var user = new AppUser
            {
                UserName = email,
                Email = email,
                FullName = role + " User"
            };

            await userManager.CreateAsync(user, "Admin!123"); // Default password
            await userManager.AddToRoleAsync(user, role);
        }
    }

    await CreateUser("lecturer@example.com", "Lecturer");
    await CreateUser("coordinator@example.com", "Coordinator");
    await CreateUser("manager@example.com", "AcademicManager");
    await CreateUser("hr@example.com", "HR");
}

await SeedUsersAsync(app);


app.Run();