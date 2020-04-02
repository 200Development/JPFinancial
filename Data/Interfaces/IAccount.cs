namespace JPFData.Interfaces
{
    public interface IAccount
    {
        int Id { get; set; }
        string UserId { get; set; }
        string Name { get; set; }
        decimal Balance { get; set; }
        decimal PaycheckContribution { get; set; }
        decimal SuggestedPaycheckContribution { get; set; }
        decimal RequiredSavings { get; set; }
        decimal BalanceSurplus { get; set; }
        bool ExcludeFromSurplus { get; set; }
        bool IsPoolAccount { get; set; }
        bool IsEmergencyFund { get; set; }
        bool IsMandatory { get; set; }
        decimal BalanceLimit { get; set; }
    }
}