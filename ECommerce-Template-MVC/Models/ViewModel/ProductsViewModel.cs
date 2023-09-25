using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace ECommerce_Template_MVC.Models.ViewModel
{
    //View model pour la page de produits dans ShoppingCart
    public class ProductsViewModel
    {
        public IPagedList<Product> Products { get; set; }
        public IEnumerable<string> ProductTypes { get; set; }
    }

}
