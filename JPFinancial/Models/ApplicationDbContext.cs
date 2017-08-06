using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace JPFinancial.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("ApplicationConnection", throwIfV1Schema: false)
        {
        }

        public IDbSet<Account> Accounts { get; set; }
        public IDbSet<Bill> Bills { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}