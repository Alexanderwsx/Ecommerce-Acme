using X.PagedList;

namespace ECommerce_Template_MVC.Models.ViewModel
{
    public class OrderVM
    {
        public IPagedList<OrderHeader> OrderHeader { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string OrderStatus { get; set; }
    }
}
