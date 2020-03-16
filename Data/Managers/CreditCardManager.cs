using System;
using System.Collections.Generic;
using System.Linq;
using JPFData.DTO;
using JPFData.Metrics;

namespace JPFData.Managers
{
    /// <summary>
    /// Handles all Credit Card interactions with the database
    /// </summary>
    public class CreditCardManager
    {
        private readonly ApplicationDbContext _db;


        public CreditCardManager()
        {
            _db = new ApplicationDbContext();
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }


        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public CreditCardDTO Get()
        {
            return Get(new CreditCardDTO());
        }

        public CreditCardDTO Get(CreditCardDTO entity)
        {
            try
            {
                entity.CreditCards = _db.CreditCards.ToList();
                entity.Metrics = RefreshCreditCardMetrics(entity);
            }
            catch (Exception)
            {
                //ignore
            }


            return entity;
        }

        private CreditCardMetrics RefreshCreditCardMetrics(CreditCardDTO entity)
        {
            CreditCardMetrics metrics = new CreditCardMetrics();


            return metrics;
        }
    }
}
