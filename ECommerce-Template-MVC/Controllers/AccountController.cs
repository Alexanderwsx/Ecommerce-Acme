using ECommerce_Template_MVC.Data;
using ECommerce_Template_MVC.Models;
using ECommerce_Template_MVC.Models.ViewModel;
using ECommerce_Template_MVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ECommerce_Template_MVC.Controllers
{
    //Controlleur separé pour la gestion des comptes pour eviter toucher le controlleur Identity
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([Bind("UserName,Email,Nom,Prenom,Adresse,Ville,Province,CodePostal,Pays")] ApplicationUser user, string Password)
        {
            if (ModelState.IsValid)
            {
                user.UserName = user.Email;
                var result = await _userManager.CreateAsync(user, Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, SD.Role_User_Individuel);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(user);
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null, string localCart = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(localCart) && localCart != "null")
                    {
                        // Deserializar el carrito de compras del localStorage
                        var localCartItems = JsonConvert.DeserializeObject<List<ShoppingCart>>(localCart);

                        // Obtener el usuario actual
                        var user = await _userManager.FindByNameAsync(model.Email);

                        foreach (var item in localCartItems)
                        {
                            //make a if to check if the product exist in db
                            if(_context.Products.Find(item.ProductId) != null) { 

                            // Verificar si el producto ya existe en el carrito del usuario
                            var cartItem = _context.ShoppingCarts.FirstOrDefault(x => x.ProductId == item.ProductId && x.ApplicationUserId == user.Id);
                            if (cartItem != null)
                            {
                                // Si el producto ya está en el carrito del usuario, suma la cantidad
                                cartItem.Count += item.Count;
                            }
                            else
                            {
                                // Si el producto no está en el carrito del usuario, agrégalo
                                _context.ShoppingCarts.Add(new ShoppingCart
                                {
                                    ProductId = item.ProductId,
                                    Count = item.Count,
                                    ApplicationUserId = user.Id,
                                    Price = _context.Products.Find(item.ProductId).Price
                                });
                            }
                            }
                        }

                        // Guardar los cambios en la base de datos
                        await _context.SaveChangesAsync();
                    }

                    // Redireccionar al usuario
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Json(new { success = true, redirectUrl = returnUrl });
                    }
                    else
                    {
                        return Json(new { success = true, redirectUrl = Url.Action(nameof(HomeController.Index), "Home") });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Intento de inicio de sesión no válido." });
                }
            }
            // Si el modelo no es válido, puedes decidir qué hacer. Por ejemplo, podrías devolver un mensaje de error.
            return Json(new { success = false, message = "Datos de inicio de sesión no válidos." });
        }

    }
}
