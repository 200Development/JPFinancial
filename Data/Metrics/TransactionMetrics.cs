namespace JPFData.Metrics
{
    public class TransactionMetrics
    {
        public TransactionMetrics()
        {
            AccountMetrics = new AccountMetrics();
            CreditCardMetrics = new CreditCardMetrics();
        }

        public AccountMetrics AccountMetrics { get; set; }
        public CreditCardMetrics CreditCardMetrics { get; set; }
    }
}