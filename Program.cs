using Microsoft.EntityFrameworkCore;
using MyProject.Data;
using MyProject.Services;

var builder = WebApplication.CreateBuilder(args);

// ── EF Core ───────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── MVC + Session ─────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
});

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ClaimService>();
builder.Services.AddScoped<ClaimDocumentService>();
builder.Services.AddScoped<AssessmentService>();
builder.Services.AddScoped<FraudService>();
builder.Services.AddScoped<SettlementService>();

var app = builder.Build();

// ── Migrate + Seed ────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await UserService.SeedAdminAsync(db);
}

// ── Pipeline ──────────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
