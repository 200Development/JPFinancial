using System.Collections.Generic;
using System.Linq;
using JPFData.Models;


namespace JPFData.Managers
{
    public class TransactionManager
    {
        public TransactionManager()
        {
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }


        private readonly ApplicationDbContext _db = new ApplicationDbContext();


        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public List<Transaction> Get()
        {
            return Get(new Transaction());
        }

        public List<Transaction> Get(Transaction entity)
        {
            List<Transaction> ret = new List<Transaction>();

            return _db.Transactions.ToList();
        }

        public Transaction Get(int transactionId)
        {

            var ret = _db.Transactions.ToList();

            //Find the specific transaction
            var entity = ret.Find(t => t.Id == transactionId) ?? null;

            return entity;
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

        public bool Delete(Transaction entity)
        {
            //todo: add delete code
            return true;
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
