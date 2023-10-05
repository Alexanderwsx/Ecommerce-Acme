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
using ECommerce_Template_MVC.Utility;
using Stripe.BillingPortal;
using Stripe.Checkout;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using SessionService = Stripe.Checkout.SessionService;
using Session = Stripe.Checkout.Session;
using Stripe;
using Microsoft.Extensions.Options;
using Shippo;
using Address = Shippo.Address;
using Product = ECommerce_Template_MVC.Models.Product;
using System.Collections;
using Humanizer.Localisation;
using Elfie.Serialization;

namespace ECommerce_Template_MVC.Controllers
{
    public class ShoppingCartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IOptions<ShippoSettings> _shippoSettings;
        private readonly IOptions<StripeSettings> _stripeSettings;

        public ShoppingCartsController(ApplicationDbContext context, IOptions<StripeSettings> stripeSettings, IOptions<ShippoSettings> shippoSettings)
        {
            _context = context;
            _stripeSettings = stripeSettings;
            _shippoSettings = shippoSettings;
        }

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public Shipment CreateShipment(ShoppingCartVM cartVM)
        {
            APIResource resource = new APIResource(_shippoSettings.Value.ShippoAPIKey);

            // Información del remitente (address_from)
            Address addressFrom = new Address
            {
                // ... (rellena esta parte con los detalles de la dirección de envío de tu empresa o lugar de origen)
                Name = "Nom de l'entreprise",
                Company = "Nom de l'entreprise",
                Street1 = "215 Clayton St.",
                City = "San Francisco",
                State = "CA",
                Zip = "94117",
                Country = "US",
                Phone = "+1 555 555 5555",
                Email = "empresa@gmail.com"
            };

            // Información del destinatario (address_to) usando cartVM.OrderHeader
            Address addressTo = new Address
            {
                // ... (basado en cartVM.OrderHeader, rellena los detalles de la dirección del cliente)
                Name = cartVM.OrderHeader.Nom + " " + cartVM.OrderHeader.Prenom,
                Street1 = "2920 Zoo Drive",
                City = "San Diego",
                State = "CA",
                Zip = "92101",
                Country = "US",
                Phone = cartVM.OrderHeader.PhoneNumber,
                Email = cartVM.OrderHeader.Email
            };

            // Crear una lista de Product basada en los elementos del carrito
            List<Product> products = new List<Product>();
            foreach (var cartItem in cartVM.ListCart)
            {
                for (int i = 0; i < cartItem.Count; i++) // Considerar la cantidad de cada producto
                {
                    products.Add(cartItem.Product);
                }
            }
            var boxes = Box.DetermineSuitableBoxes(products);
            List<Parcel> parcels = new List<Parcel>();

            foreach (var box in boxes)
            {
                Parcel parcel = new Parcel
                {
                    Length = box.Length.ToString(),
                    Width = box.Width.ToString(),
                    Height = box.Height.ToString(),
                    Weight = 5,
                    DistanceUnit = "cm",
                    MassUnit = "g"
                };
                parcels.Add(parcel);
            }

            try
            {
                // Crear el envío usando Shippo
                Shipment shipment = resource.CreateShipment(new Hashtable
                 {
                    { "address_from", addressFrom },
                    { "address_to", addressTo },
                    { "parcels", parcels },
                    { "async", false }
                 });

                return shipment;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el envío con Shippo: " + ex.Message);
            }
        }

        // GET: ShoppingCarts
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = _context.ShoppingCarts.Include(x => x.Product).Where(x => x.ApplicationUserId == claim.Value).ToList(),
                    OrderHeader = new OrderHeader()
                };

                foreach (var cart in ShoppingCartVM.ListCart)
                {
                    cart.Price = cart.Product.Price;
                    ShoppingCartVM.OrderHeader.OrderTotal += (cart.Count * cart.Product.Price);
                    cart.IsTemporary = false;
                }
            }
            else
            {
                string tempCartId = Request.Cookies["TempCartId"];
                List<ShoppingCart> tempCart;

                if (string.IsNullOrEmpty(tempCartId))
                {
                    // No hay cookie, por lo que creamos un nuevo carrito temporal
                    tempCartId = Guid.NewGuid().ToString();
                    Response.Cookies.Append("TempCartId", tempCartId, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) });
                    tempCart = new List<ShoppingCart>();
                }
                else
                {
                    // Recuperamos el carrito temporal usando el valor de la cookie
                    tempCart = _context.ShoppingCarts.Include(x => x.Product).Where(x => x.TempCartId == tempCartId).ToList();
                }

                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = tempCart,
                    OrderHeader = new OrderHeader()
                };

                foreach (var cart in ShoppingCartVM.ListCart)
                {
                    cart.Price = cart.Product.Price;
                    ShoppingCartVM.OrderHeader.OrderTotal += (cart.Count * cart.Product.Price);
                    cart.IsTemporary = true;
                }
            }

            return View(ShoppingCartVM);
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

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _context.OrderHeaders.Include(x => x.ApplicationUser).FirstOrDefault(x => x.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus == "paid")
            {
                orderHeader.PaymentStatus = SD.PaymentStatusApproved;
                orderHeader.OrderStatus = SD.StatusApproved;
            }
            else
            {
                orderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }
            orderHeader.PaymentIntentId = session.PaymentIntentId;

            if (User.Identity.IsAuthenticated)
            {
                List<ShoppingCart> listCart = _context.ShoppingCarts.Include(x => x.Product).Where(x => x.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
                _context.ShoppingCarts.RemoveRange(listCart);
            }
            else
            {
                var tempCartId = Request.Cookies["TempCartId"];
                if (!string.IsNullOrEmpty(tempCartId))
                {
                    var tempCarts = _context.ShoppingCarts.Where(x => x.TempCartId == tempCartId).ToList();
                    _context.ShoppingCarts.RemoveRange(tempCarts);
                    Response.Cookies.Delete("TempCartId");
                }
            }
            _context.SaveChanges();
            return View(id);
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

        public IActionResult Remove(int cartID)
        {
            var cart = _context.ShoppingCarts.FirstOrDefault(x => x.Id == cartID);
            _context.ShoppingCarts.Remove(cart);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        //Pour ajuster le order total avec tax apres le paiement
        [HttpPost]
        [Route("stripe-webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _stripeSettings.Value.WebhookSecret);

                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;

                    // Aquí puedes recuperar el monto total con impuestos de la sesión
                    decimal totalWithTax = (decimal)(session.AmountTotal / 100.0M);  // Stripe maneja montos en centavos, así que lo convertimos a formato decimal.

                    // Busca la orden por session.Id y actualiza el monto total y el estado de la orden
                    var orderToUpdate = _context.OrderHeaders.FirstOrDefault(o => o.SessionId == session.Id);
                    if (orderToUpdate != null)
                    {
                        orderToUpdate.OrderTotal = totalWithTax;
                        orderToUpdate.PaymentStatus = SD.PaymentStatusApproved;
                        orderToUpdate.OrderStatus = SD.StatusApproved;
                        _context.Update(orderToUpdate);
                        _context.SaveChanges();
                    }
                    else
                    {
                        // Aquí puedes manejar el caso en el que no se encuentra una orden correspondiente al ID de la sesión.
                        // Por ejemplo, podrías registrar un error.
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error para futura referencia.
                return BadRequest();
            }
        }

        public IActionResult Summary()
        {
            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = _context.ShoppingCarts.Include(x => x.Product).Where(x => x.ApplicationUserId == claim.Value).ToList(),
                    OrderHeader = new OrderHeader()
                };

                ShoppingCartVM.OrderHeader.ApplicationUser = _context.ApplicationUsers.FirstOrDefault(x => x.Id == claim.Value);

                if (ShoppingCartVM.OrderHeader.ApplicationUser != null)
                {
                    ShoppingCartVM.OrderHeader.Nom = ShoppingCartVM.OrderHeader.ApplicationUser.Nom;
                    ShoppingCartVM.OrderHeader.Prenom = ShoppingCartVM.OrderHeader.ApplicationUser.Prenom;
                    ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.CodePostal;
                    ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
                    ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.Ville;
                    ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.Adresse;
                    ShoppingCartVM.OrderHeader.Country = ShoppingCartVM.OrderHeader.ApplicationUser.Pays;
                    ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.Province;
                }
            }
            else
            {
                var tempCartId = Request.Cookies["TempCartId"];
                if (string.IsNullOrEmpty(tempCartId))
                {
                    // No hay carrito para procesar
                    return RedirectToAction("Index", "Home");
                }

                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = _context.ShoppingCarts.Include(x => x.Product).Where(x => x.TempCartId == tempCartId).ToList(),
                    OrderHeader = new OrderHeader()
                };
            }

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Count * cart.Product.Price);
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            APIResource resource = new APIResource(_shippoSettings.Value.ShippoAPIKey);
            Shipment shipment;

            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                ShoppingCartVM.ListCart = _context.ShoppingCarts.Include(x => x.Product).Where(x => x.ApplicationUserId == claim.Value).ToList();
                ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            }
            else
            {
                var tempCartId = Request.Cookies["TempCartId"];
                if (string.IsNullOrEmpty(tempCartId))
                {
                    // No hay carrito para procesar
                    return RedirectToAction("Index", "Home");
                }
                ShoppingCartVM.ListCart = _context.ShoppingCarts.Include(x => x.Product).Where(x => x.TempCartId == tempCartId).ToList();
            }

            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;

            //prix
            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = cart.Product.Price;
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Count * cart.Product.Price);
            }

            try
            {
                shipment = CreateShipment(ShoppingCartVM);
                Rate rate = shipment.Rates[0]; // Utiliza el primer rate (tarifa) disponible

                decimal shippingCost;

                try
                {
                    shippingCost = Convert.ToDecimal(rate.Amount.ToString());
                    ShoppingCartVM.OrderHeader.OrderTotal += shippingCost;
                    ShoppingCartVM.OrderHeader.ShippingAmount = shippingCost;
                }
                catch (Exception ex)
                {
                    // Añade manejo del error aquí si la conversión falla.
                    ModelState.AddModelError("", "Hubo un error al calcular el costo de envío. Por favor intenta de nuevo.");
                    return View(ShoppingCartVM);
                }
            }
            catch (Exception ex)
            {
                // Maneja el error. Por ejemplo, mostrar un mensaje al usuario sobre el problema con el cálculo del envío.
                ModelState.AddModelError("", "Hubo un error al calcular el costo de envío. Por favor intenta de nuevo.");
                return View(ShoppingCartVM);
            }

            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            _context.OrderHeaders.Add(ShoppingCartVM.OrderHeader);
            _context.SaveChanges();

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                OrderDetail orderDetails = new OrderDetail
                {
                    ProductId = cart.ProductId,
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Product.Price,
                    Count = cart.Count
                };
                _context.OrderDetails.Add(orderDetails);
                _context.SaveChanges();
            }

            //stripe settings/checkout

            var domain = "https://localhost:44316/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>(),

                AutomaticTax = new SessionAutomaticTaxOptions
                {
                    Enabled = true,
                },
                Mode = "payment",
                SuccessUrl = domain + $"ShoppingCarts/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + $"ShoppingCarts/Summary",
            };

            foreach (var item in ShoppingCartVM.ListCart)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100),
                        Currency = "CAD",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }

            //Ajoute couts livraison au stripe
            var shippingCostLineItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(ShoppingCartVM.OrderHeader.ShippingAmount * 100), // convierte el precio a centavos
                    Currency = "CAD",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Livraison",
                    },
                },
                Quantity = 1,
            };
            options.LineItems.Add(shippingCostLineItem);

            var service = new SessionService();
            Session session = service.Create(options);
            var order = _context.OrderHeaders.FirstOrDefault(x => x.Id == ShoppingCartVM.OrderHeader.Id);
            order.PaymentIntentId = session.PaymentIntentId;
            order.SessionId = session.Id;
            order.PaymentDate = DateTime.Now;

            //Shippo transaction
            Transaction transaction = resource.CreateTransaction(new Hashtable
            {
                { "rate", shipment.Rates[0].ObjectId },
                { "label_file_type", "PDF" },
                { "async", false }
            });

            if (transaction.Status.Equals("SUCCESS"))
            {
                order.TrackingNumber = transaction.TrackingNumber.ToString();
                order.Carrier = shipment.Rates[0].Provider.ToString();
            }
            else
            {
                // Aquí puedes manejar el caso en el que la transacción falla.
                // Por ejemplo, podrías registrar un error.
            }

            if (!User.Identity.IsAuthenticated)
            {
                order.Email = ShoppingCartVM.OrderHeader.Email;
            }

            _context.SaveChanges();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        private bool ShoppingCartExists(int id)
        {
            return (_context.ShoppingCarts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}