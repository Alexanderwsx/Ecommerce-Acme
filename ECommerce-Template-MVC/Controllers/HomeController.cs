﻿using ECommerce_Template_MVC.Data;
using ECommerce_Template_MVC.Models;
using ECommerce_Template_MVC.Models.ViewModel;
using ECommerce_Template_MVC.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks.Dataflow;
using X.PagedList;

namespace ECommerce_Template_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IEmailSender emailSender)
        {
            _logger = logger;
            _context = context;
            _emailSender = emailSender;

        }

        public async Task<IActionResult> Index()
        {
            if (_context != null)
            {
                var products = await _context.Products.Include(_context => _context.Images).Take(3).ToListAsync();
                return View(products);
            }
            else
            {
                return View();
            }

        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            var model = new ContactForm();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactForm contactForm)
        {
            if (!ModelState.IsValid)
            {
                return View(contactForm);
            }
            var selectedSubjectText = contactForm.Subjects.FirstOrDefault(s => s.Value == contactForm.Subject)?.Text;


            var message = $"Name: {contactForm.Name} <br/> " +
                $"Email: {contactForm.Email} <br/> " +
                $"Subject: {selectedSubjectText} <br/> " +
                $"Message: {contactForm.Message}";

            await _emailSender.SendEmailAsync("roberto.au.vera@gmail.com", selectedSubjectText, message);
            ViewData["MessageSent"] = true;

            var model = new ContactForm();
            return View(model);
        }

        public async Task<IActionResult> Shop(string searchTerm, string[] types, decimal? priceMin, decimal? priceMax, int? page)
        {

            var query = _context.Products.AsQueryable();
            query = query.Include(p => p.Images);


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
            int pageSize = 3;
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



        //public async Task<IActionResult> Index(string searchTerm, string[] types, decimal? priceMin, decimal? priceMax, int? page)
        //{
        //    var query = _context.Products.AsQueryable();

        //    // Filtrar por nombre
        //    if (!string.IsNullOrWhiteSpace(searchTerm))
        //    {
        //        query = query.Where(p => p.Name.Contains(searchTerm));
        //    }

        //    // Filtrar por tipo
        //    if (types != null && types.Length > 0)
        //    {
        //        query = query.Where(p => types.Contains(p.Type));
        //    }

        //    // Filtrar por rango de precio
        //    if (priceMin.HasValue)
        //    {
        //        query = query.Where(p => p.Price >= priceMin.Value);
        //    }
        //    if (priceMax.HasValue)
        //    {
        //        query = query.Where(p => p.Price <= priceMax.Value);
        //    }

        //    // Paginación
        //    int pageSize = 5;
        //    int pageNumber = (page ?? 1);
        //    var pagedProducts = await query.ToPagedListAsync(pageNumber, pageSize);
        //    var productTypes = await _context.Products.Select(p => p.Type).Distinct().ToListAsync();

        //    var viewModel = new ProductsViewModel
        //    {
        //        Products = pagedProducts,
        //        ProductTypes = productTypes
        //    };

        //    return View(viewModel);
        //}


        public IActionResult Details(int productId)
        {
            ShoppingCart cartObj = new()
            {
                Count = 1,
                ProductId = productId,
                Product = _context.Products.Include(p =>p.Images).Where(p => p.Id == productId).FirstOrDefault()
            };
            return View(cartObj);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int count)
        {
            var product = _context.Products.Find(productId);
            if (product == null) return NotFound();

            // Verificar si la cantidad solicitada no excede el stock disponible
            if (count > product.QuantiteEnStock)
            {
                // Maneja el error: la cantidad solicitada excede el stock disponible.
                return View("Error", "La cantidad solicitada excede el stock disponible.");
            }

            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                var existingCartItem = _context.ShoppingCarts
                    .FirstOrDefault(sc => sc.ProductId == productId && sc.ApplicationUserId == claim.Value);

                if (existingCartItem != null)
                {
                    // Verifica si la cantidad total no excede el stock disponible
                    if (existingCartItem.Count + count > product.QuantiteEnStock)
                    {
                        var errorModel = new ErrorViewModel { Message = "La cantidad solicitada excede el stock disponible." };
                        return View("Error", errorModel);
                    }
                    existingCartItem.Count += count;
                }
                else
                {
                    var newCartItem = new ShoppingCart
                    {
                        ProductId = productId,
                        Count = count,
                        Price = product.Price,
                        ApplicationUserId = claim.Value,
                        IsTemporary = false
                    };
                    _context.ShoppingCarts.Add(newCartItem);
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                string tempCartId = Request.Cookies["TempCartId"];
                ShoppingCart existingCartItem;

                if (string.IsNullOrEmpty(tempCartId))
                {
                    // No hay cookie, por lo que creamos un nuevo carrito temporal
                    tempCartId = Guid.NewGuid().ToString();
                    Response.Cookies.Append("TempCartId", tempCartId, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) });
                    existingCartItem = null;
                }
                else
                {
                    // Recuperamos el carrito temporal usando el valor de la cookie
                    existingCartItem = _context.ShoppingCarts.FirstOrDefault(sc => sc.ProductId == productId && sc.TempCartId == tempCartId);
                }

                if (existingCartItem != null)
                {
                    // Verifica si la cantidad total no excede el stock disponible
                    if (existingCartItem.Count + count > product.QuantiteEnStock)
                    {
                        return View("Error", "La cantidad total excede el stock disponible.");
                    }
                    existingCartItem.Count += count;
                }
                else
                {
                    var newCartItem = new ShoppingCart
                    {
                        ProductId = productId,
                        Count = count,
                        Price = product.Price,
                        TempCartId = tempCartId
                    };
                    _context.ShoppingCarts.Add(newCartItem);
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");

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



        public IActionResult NotFound(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                if (statusCode == 404)
                {
                    var viewName = "Error404";
                    // Aquí puedes agregar lógica adicional si lo necesitas
                    return View(viewName);
                }
            }
            return View("Error");
        }
    }

}
