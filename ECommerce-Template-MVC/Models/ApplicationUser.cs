using Microsoft.AspNetCore.Identity;

namespace ECommerce_Template_MVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nom { get; set; }
        public string? Prenom { get; set; }
        public string? Adresse { get; set; }
        public string? Ville { get; set; }
        public string? Province { get; set; }
        public string? CodePostal { get; set; }
        public string? Pays { get; set; }
    }
}
