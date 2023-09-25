using ECommerce_Template_MVC.Models;
using ECommerce_Template_MVC.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce_Template_MVC.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {

    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<OrderHeader> OrderHeaders { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {

                Name = SD.Role_Admin,
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {

                Name = SD.Role_User_Individuel,
                NormalizedName = "USER"
            },
            new IdentityRole
            {

                Name = SD.Role_Employe,
                NormalizedName = "EMPLOYE"
            }

      );
    }

}
