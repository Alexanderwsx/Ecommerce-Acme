using ECommerce_Template_MVC.Data;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce_Template_MVC.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CartCountViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            int itemCount = 0;

            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = (System.Security.Claims.ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

                itemCount = _context.ShoppingCarts.Where(x => x.ApplicationUserId == claim.Value).Sum(x => x.Count);
            }
            else
            {
                string tempCartId = Request.Cookies["TempCartId"];

                if (!string.IsNullOrEmpty(tempCartId))
                {
                    itemCount = _context.ShoppingCarts.Where(x => x.TempCartId == tempCartId).Sum(x => x.Count);
                }
            }


            return View(itemCount);
        }
    }
}
