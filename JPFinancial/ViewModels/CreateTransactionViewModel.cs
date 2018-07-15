﻿using JPFinancial.Models;
using JPFinancial.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JPFinancial.Models.Interfaces;

namespace JPFinancial.ViewModels
{
    public class CreateTransactionViewModel : ITransaction
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public CreateTransactionViewModel()
        {
            Accounts = _db.Accounts.ToList();
            CreditCards = _db.CreditCards.ToList();
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Payee { get; set; }
        public string Memo { get; set; }
        public TransactionTypesEnum Type { get; set; }
        public CategoriesEnum Category { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public int? SelectedCreditAccount { get; set; }
        public int? SelectedDebitAccount { get; set; }
        public decimal Amount { get; set; }
        public bool UsedCreditCard { get; set; }
        public IEnumerable<CreditCard> CreditCards { get; set; }
        public int? SelectedCreditCardAccount { get; set; }
    }
}