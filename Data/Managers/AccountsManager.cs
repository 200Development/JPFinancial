using System;
using System.Collections.Generic;
using System.Linq;
using JPFData.Base;
using JPFData.Models;

namespace JPFData.Managers
{
    public class AccountsManager : BaseManager
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        public AccountsManager()
        {
            
        }


        public List<Account> Get()
        {
            return Get(new Account());
        }

        public List<Account> Get(Account entity)
        {
            var ret = _db.Accounts.ToList();

            // Do any searching
            if (!string.IsNullOrEmpty(entity.Name))
            {
                ret = ret.FindAll(
                    a => a.Name.ToLower()
                        .StartsWith(entity.Name, StringComparison.CurrentCultureIgnoreCase));
            }

            return ret;
        }

        public Account Get(int accountId)
        {
            var ret = _db.Accounts.ToList();

            //Find the specific Account
            var entity = ret.Find(a => a.Id == accountId) ?? null;

            return entity;
        }
    }
}
