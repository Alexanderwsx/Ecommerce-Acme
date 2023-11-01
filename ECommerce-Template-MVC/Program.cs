using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ECommerce_Template_MVC.Data;
using ECommerce_Template_MVC.Models;
using Stripe;
using ECommerce_Template_MVC.Utility;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DbContextConnection") ?? throw new InvalidOperationException("Connection string 'DbContextConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders().AddDefaultUI();

/*Toutes les propri�t�s de l'objet StripeSettings
 * seront initialis�es � partir des valeurs de configuration fournies
 * dans la section "Stripe" du fichier de configuration de l'application.
 * De cette fa�on, l'objet StripeSettings peut �tre facilement inject� dans
 * d'autres classes qui en ont besoin, sans avoir � sp�cifier manuellement les
 * valeurs de configuration chaque fois que l'objet est utilis�.*/
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

//change language to en-us
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("en-US") };
    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

//email services
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddScoped<RazorViewToStringRenderer>();


// Add services to the container.
builder.Services.AddControllersWithViews();

//activer les sessions
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//Ajouter les services de l'API Shippo
builder.Services.Configure<ShippoSettings>(builder.Configuration.GetSection("Shippo"));


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

/*Cela permet � la biblioth�que Stripe d'utiliser la cl� d'API secr�te
 * correcte pour toutes les op�rations d'API, en fonction de la configuration
 * de l'application. Cela permet �galement de s�parer la configuration de l'application
 * de la logique de l'API Stripe, ce qui facilite la maintenance et
 * la mise � jour de l'application en cas de changement de cl� d'API.*/
StripeConfiguration.ApiKey = app.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseAuthorization();
app.MapRazorPages(); //permet l'utilisation de razorpage
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.UseStatusCodePagesWithRedirects("/Home/NotFound?statusCode={0}"); // Agrega esta l�nea

//Execute dbInitializer
var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
try
{
    await context.Database.MigrateAsync();
    await DbInitializer.Initialize(context, userManager);
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred creating the DB.");
}

//change language to en-us
var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

app.Run();