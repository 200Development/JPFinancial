using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPFData.Enumerations;
using JPFData.Models;

namespace JPFData
{
    public class DatabaseEditor
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly Calculations _calculations = new Calculations();

        public DatabaseEditor()
        {
            
        }


        public void UpdateAccountBalances(Transaction transaction, string type)
        {
            if (type == "create")
            {
                var newTransaction = new Transaction();
                newTransaction.Date = transaction.Date;
                newTransaction.Payee = transaction.Payee;
                newTransaction.Category = transaction.Category;
                newTransaction.Memo = transaction.Memo;
                newTransaction.Type = transaction.Type;
                newTransaction.DebitAccount = transaction.DebitAccount;
                newTransaction.CreditAccount = transaction.CreditAccount;
                newTransaction.Amount = transaction.Amount;

                if (transaction.DebitAccount != null)
                {
                    transaction.DebitAccount.Balance += transaction.Amount;
                    _db.Entry(transaction.DebitAccount).State = EntityState.Modified;
                }

                if (transaction.CreditAccount != null)
                {
                    transaction.CreditAccount.Balance -= transaction.Amount;
                    _db.Entry(transaction.CreditAccount).State = EntityState.Modified;
                }
            }
            else if (type == "delete" || type == "edit")
            {
                var originalTransaction = _db.Transactions
                    .AsNoTracking()
                    .Where(t => t.Id == transaction.Id)
                    .Cast<Transaction>()
                    .FirstOrDefault();
                if (originalTransaction == null) return;
                var originalCreditAccount = _db.Accounts.FirstOrDefault(a => a.Id == originalTransaction.CreditAccountId);
                var originalDebitAccount = _db.Accounts.FirstOrDefault(a => a.Id == originalTransaction.DebitAccountId);
                var originalAmount = originalTransaction.Amount;

                // Reassign the Debit/Credit Account Id's to Transaction Model
                transaction.CreditAccountId = originalTransaction.CreditAccountId;
                transaction.DebitAccountId = originalTransaction.DebitAccountId;

                if (type == "delete")
                {
                    if (originalDebitAccount != null)
                    {
                        originalDebitAccount.Balance -= transaction.Amount;
                        _db.Entry(originalDebitAccount).State = EntityState.Modified;
                    }

                    if (originalCreditAccount != null)
                    {
                        originalCreditAccount.Balance += transaction.Amount;
                        _db.Entry(originalCreditAccount).State = EntityState.Modified;
                    }
                }
                else if (type == "edit")
                {
                    var amountDifference = transaction.Amount - originalAmount;
                    if (originalDebitAccount != null)
                    {
                        originalDebitAccount.Balance += amountDifference;
                        _db.Entry(originalDebitAccount).State = EntityState.Modified;
                    }

                    if (originalCreditAccount != null)
                    {
                        originalCreditAccount.Balance -= amountDifference;
                        _db.Entry(originalCreditAccount).State = EntityState.Modified;
                    }
                }
            }
        }

        public void UpdateCreditCardBalances(Transaction transaction, string type)
        {
            var creditCardId = transaction?.SelectedCreditCardAccount;
            if (creditCardId == null) return;
            var creditCards = _db.CreditCards.ToList();
            var creditCard = creditCards.FirstOrDefault(c => c.Id == creditCardId);

            if (type == "create")
            {
                if (creditCard != null) creditCard.Balance += transaction.Amount;
                _db.Entry(creditCard).State = EntityState.Modified;
            }
            else if (type == "delete" || type == "edit")
            {
                var originalTransaction = _db.Transactions
                    .AsNoTracking()
                    .Where(t => t.Id == transaction.Id)
                    .Cast<Transaction>()
                    .FirstOrDefault();
                if (originalTransaction == null) return;
                var originalCreditCard = _db.CreditCards.AsNoTracking().FirstOrDefault(a => a.Id == originalTransaction.SelectedCreditCardAccount);
                var originalAmount = originalTransaction.Amount;

                // Reassign the credit card Id to Transaction Model
                transaction.SelectedCreditCardAccount = originalTransaction.SelectedCreditCardAccount;

                if (type == "delete")
                {
                    if (originalCreditCard == null) return;
                    originalCreditCard.Balance -= transaction.Amount;
                    _db.Entry(originalCreditCard).State = EntityState.Modified;
                }
                else if (type == "edit")
                {
                    var amountDifference = transaction.Amount - originalAmount;
                    if (creditCard == null) return;
                    creditCard.Balance += amountDifference;
                    _db.Entry(creditCard).State = EntityState.Modified;
                }
            }
        }

        public void UpdateRequiredBalance()
        {
            try
            {
                var accounts = _db.Accounts.ToList();
                var bills = _db.Bills.ToList();
                var savingsAccountBalances = new List<KeyValuePair<string, decimal>>();

                foreach (var bill in bills)
                {
                    var billTotal = bill.AmountDue;
                    var dueDate = bill.DueDate;
                    var payPeriodsLeft = _calculations.PayPeriodsTilDue(dueDate);
                    decimal savePerPaycheck = 0;

                    switch (bill.PaymentFrequency)
                    {
                        case FrequencyEnum.Annually:
                            savePerPaycheck = billTotal / 24;
                            break;
                        case FrequencyEnum.SemiAnnually:
                            savePerPaycheck = billTotal / 12;
                            break;
                        case FrequencyEnum.Quarterly:
                            savePerPaycheck = billTotal / 6;
                            break;
                        case FrequencyEnum.SemiMonthly:
                            savePerPaycheck = billTotal / 4;
                            break;
                        case FrequencyEnum.Monthly:
                            savePerPaycheck = billTotal / 2;
                            break;
                        case FrequencyEnum.Weekly:
                            savePerPaycheck = billTotal * 2;
                            break;
                        default:
                            savePerPaycheck = billTotal / 2;
                            break;
                    }
                    var save = billTotal - payPeriodsLeft * savePerPaycheck;
                    savingsAccountBalances.Add(new KeyValuePair<string, decimal>(bill.Account.Name, save));
                }


                foreach (var account in accounts)
                {
                    var valuesFound = false;
                    decimal totalSavings = 0;

                    foreach (var savings in savingsAccountBalances)
                    {
                        if (savings.Key != account.Name) continue;
                        totalSavings += savings.Value;
                        valuesFound = true;
                    }
                    if (!valuesFound) continue;
                    account.RequiredSavings = totalSavings;
                    _db.Entry(account).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateRequiredBalanceSurplus()
        {
            try
            {
                var accounts = _db.Accounts.ToList();

                foreach (var account in accounts)
                {
                    var acctBalance = account.Balance;
                    var reqbalance = account.RequiredSavings;
                    account.BalanceSurplus = acctBalance - reqbalance;
                    _db.Entry(account).State = EntityState.Modified;
                    _db.SaveChanges();

                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
