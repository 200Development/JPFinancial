using System.Collections.Generic;
using JPFData.Metrics;
using JPFData.Models;

namespace JPFData.DTO
{
    public class CreditCardDTO
    {
        public CreditCardDTO()
        {
            CreditCards = new List<CreditCard>();
            Metrics = new CreditCardMetrics();
        }

        public List<CreditCard> CreditCards { get; set; }
        public CreditCardMetrics Metrics { get; set; }
    }
}
