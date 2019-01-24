select a.Name, sum(b.AmountDue)
from Bills b
join Accounts a
on a.Id = b.AccountId
group by a.Name

select a.Name, b.Name, b.AmountDue, b.DueDate
from Accounts a
join Bills b
on b.AccountId = a.Id
