using ECommerce_Template_MVC.Models;
using ECommerce_Template_MVC.Utility;
using Microsoft.AspNetCore.Identity;

namespace ECommerce_Template_MVC.Data
{
    //Created admin user and user test in the database
    public static class DbInitializer
    {
        public static async Task Initialize (ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if(!userManager.Users.Any())
            {

                ApplicationUser admin = new ApplicationUser
                {
                    UserName = "admin@ecommerce.com",
                    Email = "admin@ecommerce.com"
                };

                await userManager.CreateAsync(admin, "Pa$$w0rd");
                await userManager.AddToRoleAsync(admin, SD.Role_Admin);

                ApplicationUser user = new ApplicationUser
                {
                    UserName = "bob@test.com",
                    Email = "bob@test.com"
                };

                await userManager.CreateAsync (user, "Pa$$w0rd");
                await userManager.AddToRoleAsync (user, SD.Role_User_Individuel);

            }

            if(context.Products.Any())
            {
                return;
            }
            //make another 10 product more
            for (int i = 0; i < 10; i++)
            {
                Product produit = new Product
                {
                    QuantiteEnStock = 10,
                    Name = "Produit " + i,
                    Description = "Description du produit " + i,
                    Type = "Type " + i,
                    Brand = "Brand " + i,
                    Price = 10+i,
                };

                context.Products.Add(produit);
            }
            context.SaveChanges();

        }


        //add dummy data to the database for testing purposes
        //add produits


    }
}
