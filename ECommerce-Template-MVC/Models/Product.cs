using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce_Template_MVC.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Nom")]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        [DisplayName("Prix")]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(1, (double)decimal.MaxValue, ErrorMessage ="Le prix doit etre minumum 1")]
        public decimal Price { get; set; }

        [Required]
        [DisplayName("Quantite en Stock")]
        public int QuantiteEnStock { get; set; }

        [ValidateNever]
        public List<ProductImage> Images { get; set; }
        [ValidateNever]

        [NotMapped]
        public List<IFormFile> ImageFiles { get; set; }


    }
}
