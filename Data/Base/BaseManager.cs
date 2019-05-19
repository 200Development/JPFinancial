using System.Collections.Generic;

namespace JPFData.Base
{
    public class BaseManager
    {
        protected BaseManager()
        {
        }

        private static ApplicationDbContext _db;
        private List<KeyValuePair<string, string>> _validationErrors;

        protected static ApplicationDbContext DbContext => _db ?? (_db = new ApplicationDbContext());
        public List<KeyValuePair<string, string>> ValidationErrors => _validationErrors ?? (_validationErrors = new List<KeyValuePair<string, string>>());
    }
}
