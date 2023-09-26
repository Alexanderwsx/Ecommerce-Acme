using ECommerce_Template_MVC.Data;
using ECommerce_Template_MVC.Models;
using ECommerce_Template_MVC.Models.ViewModel;
using ECommerce_Template_MVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
                user.UserName= user.Email;
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
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            // Si el modelo es válido, procedemos con el intento de inicio de sesión
            if (ModelState.IsValid)
            {
                // Intentamos iniciar sesión con las credenciales proporcionadas
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
               
                if (result.Succeeded)
                {
                    // Si el inicio de sesión es exitoso, redirigimos al usuario a la página principal o a la URL de retorno (si se proporciona)
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                }
                //else if (result.IsLockedOut)
                //{
                //    // Si la cuenta del usuario está bloqueada, puedes redirigirlo a una página específica o mostrar un mensaje
                //    return RedirectToAction(nameof(Lockout));
                //}
                else
                {
                    // Si el inicio de sesión falla, mostramos un mensaje de error y volvemos a mostrar el formulario
                    ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
                    return View(model);
                }
            }

            // Si llegamos hasta aquí, algo falló, volver a mostrar el formulario
            return View(model);
        }
    }
}
