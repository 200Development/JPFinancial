using System;

namespace JPFData.Interfaces
{
    public interface IExpense
    {
        int Id { get; set; }
        string Name { get; set; }
        DateTime Due { get; set; }
        decimal Amount { get; set; }
        int BillId { get; set; }
        int CreditAccountId { get; set; }
        bool IsPaid { get; set; }
        string UserId { get; set; }
    }
}