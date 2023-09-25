using ECommerce_Template_MVC.Data;
using ECommerce_Template_MVC.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce_Template_MVC.Controllers
{
	public class CartController : Controller
	{
		private readonly ApplicationDbContext _context;
		[BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; }
		public int OrderTotal { get; set; }

		public CartController(ApplicationDbContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			return View();
		}
	}
}
