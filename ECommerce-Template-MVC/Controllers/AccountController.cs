using ECommerce_Template_MVC.Data;
using ECommerce_Template_MVC.Models;
using ECommerce_Template_MVC.Models.ViewModel;
using ECommerce_Template_MVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;

namespace ECommerce_Template_MVC.Controllers
{
    //Controlleur separé pour la gestion des comptes pour eviter toucher le controlleur Identity
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly RazorViewToStringRenderer _renderer;

        public AccountController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, 
            SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, RazorViewToStringRenderer renderer)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _renderer = renderer;
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

                    //send email to customer with emailsender
                    // Renderizar el correo
                    var emailHtml = await _renderer.RenderViewToStringAsync("Views/EmailTemplates/RegisterConfirmation.cshtml",
                        user);

                    // Envía el correo
                    await _emailSender.SendEmailAsync(user.Email,"Register Confirmation", emailHtml);


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
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // Leer el carrito de compras de la cookie TempCartId
                    var tempCartId = Request.Cookies["TempCartId"];
                    if (!string.IsNullOrEmpty(tempCartId))
                    {
                        var localCartItems = _context.ShoppingCarts.Where(sc => sc.TempCartId == tempCartId).ToList();

                        // Obtener el usuario actual
                        var user = await _userManager.FindByNameAsync(model.Email);

                        foreach (var item in localCartItems)
                        {
                            if (_context.Products.Find(item.ProductId) != null)
                            {
                                var cartItem = _context.ShoppingCarts.FirstOrDefault(x => x.ProductId == item.ProductId && x.ApplicationUserId == user.Id);
                                if (cartItem != null)
                                {
                                    // Si la cantidad total excede el stock, ajustar al stock máximo
                                    if (cartItem.Count + item.Count > cartItem.Product.QuantiteEnStock)
                                    {
                                        cartItem.Count = cartItem.Product.QuantiteEnStock;
                                    }
                                    else
                                    {
                                        cartItem.Count += item.Count;
                                    }
                                }
                                else
                                {
                                    _context.ShoppingCarts.Add(new ShoppingCart
                                    {
                                        ProductId = item.ProductId,
                                        Count = item.Count,
                                        ApplicationUserId = user.Id,
                                        Price = _context.Products.Find(item.ProductId).Price,
                                        IsTemporary = false
                                    });
                                }

                                // Eliminar el carrito temporal
                                _context.ShoppingCarts.Remove(item);
                            }
                        }

                        // Guardar los cambios en la base de datos
                        await _context.SaveChangesAsync();

                        // Eliminar la cookie después de procesarla
                        Response.Cookies.Delete("TempCartId");
                    }

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
                    return View(model); // Devuelve la vista de inicio de sesión con el modelo y los errores
                }
            }
            return View(model); // Devuelve la vista de inicio de sesión con el modelo y los errores
        }


    }
}
