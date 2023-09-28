using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ECommerce_Template_MVC.Models
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }
        public string ImageUrl { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
