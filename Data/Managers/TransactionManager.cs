using System;
using System.Collections.Generic;
using System.Linq;
using JPFData.DTO;
using JPFData.Metrics;
using JPFData.Models;


namespace JPFData.Managers
{
    public class TransactionManager
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();


        public TransactionManager()
        {
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }


        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public TransactionDTO Get()
        {
            return Get(new TransactionDTO());
        }

        public TransactionDTO Get(TransactionDTO entity)
        {
            try
            {
                entity.Transactions = _db.Transactions.ToList();
                entity.Metrics = RefreshTransactionMetrics(entity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return entity;
        }

        private TransactionMetrics RefreshTransactionMetrics(TransactionDTO entity)
        {
           TransactionMetrics metrics = new TransactionMetrics();

           metrics.AccountMetrics = new AccountMetrics();
           metrics.CreditCardMetrics = new CreditCardMetrics();


           return metrics;
        }

        public bool Update(Transaction entity)
        {
            bool ret = false;

            ret = Validate(entity);

            if (ret)
            {
                //todo: add update code
            }

            return ret;
        }

        public bool Delete(int? id)
        {
            throw new NotImplementedException();
        }

        public bool Validate(Transaction entity)
        {
            ValidationErrors.Clear();

            if (!string.IsNullOrEmpty(entity.Payee))
            {
                if (entity.Payee.ToLower() == entity.Payee)
                {
                    ValidationErrors.Add(new KeyValuePair<string, string>("Payee", "Payee must not be all lower case."));
                }
            }

            return ValidationErrors.Count == 0;
        }

        public bool Insert(Transaction entity)
        {
            bool ret = false;

            ret = Validate(entity);

            if (ret)
            {
                //todo: add insert code
            }

            return ret;
        }
    }
}
