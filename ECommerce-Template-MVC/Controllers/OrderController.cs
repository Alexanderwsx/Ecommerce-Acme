using ECommerce_Template_MVC.Data;
using ECommerce_Template_MVC.Models;
using ECommerce_Template_MVC.Models.ViewModel;
using ECommerce_Template_MVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace ECommerce_Template_MVC.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? page, DateTime? startDate, DateTime? endDate, string orderStatus)
        {
            var query = _context.OrderHeaders.AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= startDate.Value); // Suponiendo que tu OrderHeader tiene una propiedad 'Date'
            }

            if (endDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= endDate.Value);
            }
            // Filtrar por OrderStatus si está presente
            if (!string.IsNullOrEmpty(orderStatus))
            {
                query = query.Where(o => o.OrderStatus == orderStatus);
            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            var pagedOrders = await query.ToPagedListAsync(pageNumber, pageSize);
            
            var viewModel = new OrderVM
            {
                OrderHeader = pagedOrders,
                StartDate = startDate,
                EndDate = endDate,
                OrderStatus = orderStatus

            }; 

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int orderId)
        {          
            OrderDetailsVM orderDetailsVM = new OrderDetailsVM()
            {
                OrderHeader = await _context.OrderHeaders.FirstOrDefaultAsync(m => m.Id == orderId),
                OrderDetail = await _context.OrderDetails.Include(p => p.Product).Where(m => m.OrderId == orderId).ToListAsync()
            };

            return View(orderDetailsVM);
        }

        public async Task<IActionResult> StartProcessing(int id)
        {
            OrderHeader orderHeader = await _context.OrderHeaders.FindAsync(id);
            orderHeader.OrderStatus = SD.StatusInProcess;
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Order", new { orderId = orderHeader.Id });
        }

        public async Task<IActionResult> ShipOrder(int id)
        {
            OrderHeader orderHeader = await _context.OrderHeaders.FindAsync(id);
            orderHeader.OrderStatus = SD.StatusShipped;
            await _context.SaveChangesAsync();

            //ajouter notificaiton email au client

            return RedirectToAction("Details", "Order", new { orderId = orderHeader.Id });
        }

        public async Task<IActionResult> CancelOrder(int id)
        {
            OrderHeader orderHeader = await _context.OrderHeaders.FindAsync(id);
            orderHeader.OrderStatus = SD.StatusCancelled;
            await _context.SaveChangesAsync();

            //ajouter notificaiton email au client
            return RedirectToAction("Details", "Order", new { orderId = orderHeader.Id });
        }

        public async  Task<IActionResult> UpdateOrderDetail(OrderDetailsVM order)
        {
            var orderHeader = _context.OrderHeaders.FirstOrDefault(o => o.Id == order.OrderHeader.Id);
            orderHeader.Nom = order.OrderHeader.Nom;
            orderHeader.Prenom = order.OrderHeader.Prenom;
            orderHeader.Email = order.OrderHeader.Email;
            orderHeader.PhoneNumber = order.OrderHeader.PhoneNumber;
            orderHeader.StreetAddress = order.OrderHeader.StreetAddress;
            orderHeader.City = order.OrderHeader.City;
            orderHeader.State = order.OrderHeader.State;
            orderHeader.PostalCode = order.OrderHeader.PostalCode;
            orderHeader.OrderDate = order.OrderHeader.OrderDate;
            orderHeader.Carrier = order.OrderHeader.Carrier;
            orderHeader.TrackingNumber = order.OrderHeader.TrackingNumber;
            orderHeader.OrderStatus = order.OrderHeader.OrderStatus;
            orderHeader.PaymentStatus = order.OrderHeader.PaymentStatus;

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Order", new { orderId = orderHeader.Id });

        }
    }
}
