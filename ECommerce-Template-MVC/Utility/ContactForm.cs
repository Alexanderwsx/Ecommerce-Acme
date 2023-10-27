using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce_Template_MVC.Utility
{
    public class ContactForm
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
        [ValidateNever]
        public List<SelectListItem> Subjects { get; } = new List<SelectListItem>
    {
        new SelectListItem { Value = "1", Text = "Question" },
        new SelectListItem { Value = "2", Text = "Problem" },
        new SelectListItem { Value = "3", Text = "Suggestion" },
        new SelectListItem { Value = "4", Text = "Other" }
    };
    }
}
