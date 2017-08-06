namespace JPFinancial.Models.Interfaces
{
    public interface IAccount
    {
        int Id { get; set; }
        string Name { get; set; }
        decimal Balance { get; set; }
    }
}