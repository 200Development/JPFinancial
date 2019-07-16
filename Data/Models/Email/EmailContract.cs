using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;

namespace JPFData.Models.Email
{
    public class EmailContract
    {
        public EmailContract(IdentityMessage message)
        {
            FromEmailAddress = Properties.Resources.UserRegistrationFromEmailAddress;
            Subject = message.Subject;
            ToEmailAddress = message.Destination;
            HtmlContent = message.Body;
        }

        [Required]
        [Display(Name = "From Email Address")]
        public string FromEmailAddress { get; set; }

        public string Alias { get; set; }

        [Required]
        [Display(Name = "To Email Address")]
        public string ToEmailAddress { get; set; }

        [Display(Name = "Cc Email Address")]
        public string CcEmailAddress { get; set; }

        [Display(Name = "Bcc Email Address")]
        public string BccEmailAddress { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "HTML Content")]
        public string HtmlContent { get; set; }

        [Display(Name = "Plain-text Content")]
        public string PlainTextContent { get; set; }
    }
}
