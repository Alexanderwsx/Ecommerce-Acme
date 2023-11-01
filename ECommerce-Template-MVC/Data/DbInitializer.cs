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

         
            context.SaveChanges();

        }


        //add dummy data to the database for testing purposes
        //add produits


    }
}
