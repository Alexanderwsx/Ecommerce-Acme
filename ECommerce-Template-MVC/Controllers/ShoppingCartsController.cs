using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ECommerce_Template_MVC.Data;
using ECommerce_Template_MVC.Models;
using ECommerce_Template_MVC.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ECommerce_Template_MVC.Controllers
{
    public class ShoppingCartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public ShoppingCartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ShoppingCarts
        public async Task<IActionResult> Index()
        {          
            var cartItems = new List<ShoppingCart>();

            if (User.Identity.IsAuthenticated)
            {
       
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                cartItems = _context.ShoppingCarts
                    .Where(x => x.ApplicationUserId == claim.Value).Include(x => x.Product)
                    .ToList();
            }
            var viewModel = new ShoppingCartVM
            {
                ListCart = cartItems
            };

            return View(viewModel);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            

            return View();
        }

        public IActionResult Plus(int cartID)
        {
            var cart = _context.ShoppingCarts.FirstOrDefault(x => x.Id == cartID);
           
            int stock = _context.Products.FirstOrDefault(x => x.Id == cart.ProductId).QuantiteEnStock;

            if (cart.Count < stock)
            {
                cart.Count += 1;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartID)
        {
            var cart = _context.ShoppingCarts.FirstOrDefault(x => x.Id == cartID);
            if (cart.Count <= 1)
            {
                _context.ShoppingCarts.Remove(cart);
            }
            else
            {
                cart.Count -= 1;
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartID)
        {
            var cart = _context.ShoppingCarts.FirstOrDefault(x => x.Id == cartID);
            _context.ShoppingCarts.Remove(cart);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool ShoppingCartExists(int id)
        {
            return (_context.ShoppingCarts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
