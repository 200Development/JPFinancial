namespace JPFData.Models
{
    public class Benefit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public int SalaryId { get; set; }
        public Bonus Bonus { get; set; }
    }
}