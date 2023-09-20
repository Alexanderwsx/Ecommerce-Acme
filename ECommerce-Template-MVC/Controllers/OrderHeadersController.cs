using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ECommerce_Template_MVC.Data;
using ECommerce_Template_MVC.Models;

namespace ECommerce_Template_MVC.Controllers
{
    public class OrderHeadersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderHeadersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OrderHeaders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.OrderHeaders.Include(o => o.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: OrderHeaders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderHeaders == null)
            {
                return NotFound();
            }

            var orderHeader = await _context.OrderHeaders
                .Include(o => o.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderHeader == null)
            {
                return NotFound();
            }

            return View(orderHeader);
        }

        // GET: OrderHeaders/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: OrderHeaders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,OrderDate,ShippingDate,OrderTotal,OrderStatus,PaymentStatus,TrackingNumber,Carrier,PaymentDate,PaymentDueDate,SessionId,PaymentIntentId,PhoneNumber,StreetAddress,City,PostalCode,State,Country,Nom,Prenom")] OrderHeader orderHeader)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderHeader);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", orderHeader.ApplicationUserId);
            return View(orderHeader);
        }

        // GET: OrderHeaders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrderHeaders == null)
            {
                return NotFound();
            }

            var orderHeader = await _context.OrderHeaders.FindAsync(id);
            if (orderHeader == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", orderHeader.ApplicationUserId);
            return View(orderHeader);
        }

        // POST: OrderHeaders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,OrderDate,ShippingDate,OrderTotal,OrderStatus,PaymentStatus,TrackingNumber,Carrier,PaymentDate,PaymentDueDate,SessionId,PaymentIntentId,PhoneNumber,StreetAddress,City,PostalCode,State,Country,Nom,Prenom")] OrderHeader orderHeader)
        {
            if (id != orderHeader.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderHeader);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderHeaderExists(orderHeader.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", orderHeader.ApplicationUserId);
            return View(orderHeader);
        }

        // GET: OrderHeaders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrderHeaders == null)
            {
                return NotFound();
            }

            var orderHeader = await _context.OrderHeaders
                .Include(o => o.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderHeader == null)
            {
                return NotFound();
            }

            return View(orderHeader);
        }

        // POST: OrderHeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrderHeaders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.OrderHeaders'  is null.");
            }
            var orderHeader = await _context.OrderHeaders.FindAsync(id);
            if (orderHeader != null)
            {
                _context.OrderHeaders.Remove(orderHeader);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderHeaderExists(int id)
        {
          return (_context.OrderHeaders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
