using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using JPFData.DTO;
using JPFData.Enumerations;
using JPFData.Metrics;

namespace JPFData.Managers
{
    public class BillManager
    {
        private readonly ApplicationDbContext _db;


        public BillManager()
        {
            _db = new ApplicationDbContext();
        }


        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public BillDTO Get()
        {
            return Get(new BillDTO());
        }

        public BillDTO Get(BillDTO entity)
        {
            try
            {
                entity.Bills = _db.Bills.ToList();
                entity.Metrics = RefreshBillMetrics(entity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            return entity;
        }

        public BillMetrics RefreshBillMetrics(BillDTO entity)
        {
            BillMetrics metrics = new BillMetrics();

            try
            {
                metrics.LargestBalance = entity.Bills.Max(b => b.AmountDue);
                metrics.SmallestBalance = entity.Bills.Min(b => b.AmountDue);
                metrics.TotalBalance = entity.Bills.Sum(b => b.AmountDue);
                metrics.AverageBalance = entity.Bills.Sum(b => b.AmountDue) / entity.Bills.Count;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return metrics;
        }

        public void UpdateBillDueDates()
        {
            try
            {
                var bills = _db.Bills.ToList();
                var beginDate = DateTime.Today;

                foreach (var bill in bills)
                {
                    if (bill.DueDate.Date > beginDate) continue;

                    var frequency = bill.PaymentFrequency;
                    var dueDate = bill.DueDate;
                    var newDueDate = dueDate;

                    /* Updates bill due date to the current due date
                       while loop handles due date updates, regardless of how out of date they are */
                    while (newDueDate < beginDate)
                    {
                        switch (frequency)
                        {
                            case FrequencyEnum.Daily:
                                newDueDate = newDueDate.AddDays(1);
                                break;
                            case FrequencyEnum.Weekly:
                                newDueDate = newDueDate.AddDays(7);
                                break;
                            case FrequencyEnum.BiWeekly:
                                newDueDate = newDueDate.AddDays(14);
                                break;
                            case FrequencyEnum.Monthly:
                                newDueDate = newDueDate.AddMonths(1);
                                break;
                            case FrequencyEnum.SemiMonthly:
                                newDueDate = newDueDate.AddDays(15);
                                break;
                            case FrequencyEnum.Quarterly:
                                newDueDate = newDueDate.AddMonths(3);
                                break;
                            case FrequencyEnum.SemiAnnually:
                                newDueDate = newDueDate.AddMonths(6);
                                break;
                            case FrequencyEnum.Annually:
                                newDueDate = newDueDate.AddYears(1);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    // Change state of bill to modified for database update
                    bill.DueDate = newDueDate;
                    _db.Entry(bill).State = EntityState.Modified;
                }
                // Update all modified bills
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
