using System.Data.Entity;
using JPFData.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace JPFData
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("JPFinancial", throwIfV1Schema: false)
        {
        }

        public IDbSet<Account> Accounts { get; set; }
        public IDbSet<Bill> Bills { get; set; }
        public IDbSet<Salary> Salaries { get; set; }
        public IDbSet<Bonus> Bonuses { get; set; }
        public IDbSet<Expense> Expenses { get; set; }
        public IDbSet<Benefit> Benefits { get; set; }
        public IDbSet<Company> Companies { get; set; }
        public IDbSet<Transaction> Transactions { get; set; }
        public IDbSet<Loan> Loans { get; set; }
        public IDbSet<CreditCard> CreditCards { get; set; }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}