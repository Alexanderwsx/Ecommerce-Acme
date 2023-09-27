using ECommerce_Template_MVC.Data;
using ECommerce_Template_MVC.Models;
using ECommerce_Template_MVC.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;

namespace ECommerce_Template_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;

        }

        public async Task<IActionResult> Index(string searchTerm, string[] types, decimal? priceMin, decimal? priceMax, int? page)
        {
            var query = _context.Products.AsQueryable();

            // Filtrar por nombre
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }

            // Filtrar por tipo
            if (types != null && types.Length > 0)
            {
                query = query.Where(p => types.Contains(p.Type));
            }

            // Filtrar por rango de precio
            if (priceMin.HasValue)
            {
                query = query.Where(p => p.Price >= priceMin.Value);
            }
            if (priceMax.HasValue)
            {
                query = query.Where(p => p.Price <= priceMax.Value);
            }

            // Paginación
            int pageSize = 5;
            int pageNumber = (page ?? 1);   
            var pagedProducts = await query.ToPagedListAsync(pageNumber, pageSize);
            var productTypes = await _context.Products.Select(p => p.Type).Distinct().ToListAsync();

            var viewModel = new ProductsViewModel
            {
                Products = pagedProducts,
                ProductTypes = productTypes
            };

            return View(viewModel);
        }


        public IActionResult Details(int productId)
        {
            ShoppingCart cartObj = new()
            {
                Count = 1,
                ProductId = productId,
                Product = _context.Products.Find(productId)
            };
            return View(cartObj);
        }

      


        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int count)
        {
            var product = _context.Products.Find(productId);
            if (product == null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                var userId = (await _userManager.GetUserAsync(User)).Id;

                // Verifica si el producto ya existe en el carrito del usuario.
                var existingCartItem = _context.ShoppingCarts
                    .FirstOrDefault(sc => sc.ProductId == productId && sc.ApplicationUserId == userId);

                if (existingCartItem != null)
                {
                    // Si el producto ya existe en el carrito, simplemente incrementa el conteo.
                    existingCartItem.Count+=count;
                }
                else
                {
                    // Si no existe, crea un nuevo artículo del carrito.
                    var newCartItem = new ShoppingCart
                    {
                        ProductId = productId,
                        Count = count,
                        Price = product.Price,
                        ApplicationUserId = userId,
                    };
                    _context.ShoppingCarts.Add(newCartItem);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                return Json(new { addToLocalStorage = true, product = new { ProductId = productId, Count = count, Price = product.Price, ProductName = product.Name } });
            }
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}