select a.Name as "Account Name", b.Name as "Bill Name", b.AmountDue as "Amount Due", b.DueDate as "Due Date", t.Payee as "Transaction Payee", t.Date as "Transaction Date"
from Accounts a
join Bills b
on b.AccountId = a.Id
join Transactions t
on t.CreditAccountId = b.AccountId


