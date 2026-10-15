using Microsoft.EntityFrameworkCore;
using VirtualWallet.Data.Context;
using VirtualWallet.Data.Repositories;
using VirtualWallet.Data.Repositories.Contracts;
using VirtualWallet.Business.Services.Contracts;
using VirtualWallet.Business.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<VirtualWalletDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransferRepository, TransferRepository>();

// Business Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddHttpClient<ITransferService, TransferService>();

var app = builder.Build();

// Seed initial test data if missing
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<VirtualWalletDbContext>();
    DbInitializer.SeedTestData(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();