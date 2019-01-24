using System.ComponentModel.DataAnnotations;

namespace JPFData.ViewModels
{
    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}