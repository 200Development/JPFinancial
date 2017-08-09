namespace JPFinancial.Models.Interfaces
{
    public interface ISalaryLineItem
    {
        int Id { get; set; }
        string Name { get; set; }
        decimal Amount { get; set; }
        int SalaryId { get; set; }
    }
}