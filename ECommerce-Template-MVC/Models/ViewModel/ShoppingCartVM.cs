namespace ECommerce_Template_MVC.Models.ViewModel
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart>? ListCart { get; set; }
        public OrderHeader? OrderHeader { get; set; }
    }
}
