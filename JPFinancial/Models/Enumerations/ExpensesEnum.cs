using Newtonsoft.Json;

namespace JPFinancial.Models.Enumerations
{
    public enum ExpensesEnum
    {
        [JsonProperty(PropertyName = "Auto Insurance")]
        AutoInsurance,
        [JsonProperty(PropertyName = "Auto Payment")]
        AutoPayment,
        [JsonProperty(PropertyName = "Car Wash")]
        CarWash,
        [JsonProperty(PropertyName = "Gas & Fuel")]
        Gas,
        Parking,
        [JsonProperty(PropertyName = "Public Transportation")]
        PublicTransportation,
        [JsonProperty(PropertyName = "Vehicle Registration")]
        VehicleRegistration,
        [JsonProperty(PropertyName = "Vehicle Service & Parts")]
        ServiceAndParts,
        [JsonProperty(PropertyName = "Tolls")]
        Toll,
        [JsonProperty(PropertyName = "Credit Card Payment")]
        CreditCardPayment,
        [JsonProperty(PropertyName = "Home Phone")]
        HomePhone,
        Internet,
        [JsonProperty(PropertyName = "Cell Phone")]
        CellPhone,
        Television,
        Utilities,
        [JsonProperty(PropertyName = "ATM")]
        Atm,
        Education,
        [JsonProperty(PropertyName = "School Supplies")]
        SchoolSupplies,
        [JsonProperty(PropertyName = "Student Loans")]
        StudentLoan,
        Tuition,
        Entertainment,
        Arts,
        Movies,
        Music,
        [JsonProperty(PropertyName = "Books & Magazines")]
        ReadingMaterial,
        Fees,
        [JsonProperty(PropertyName = "ATM Fee")]
        AtmFee,
        [JsonProperty(PropertyName = "Bank Fee")]
        BankFee,
        [JsonProperty(PropertyName = "Late Fee")]
        LateFee,
        [JsonProperty(PropertyName = "Service Fee")]
        ServiceFee,
        [JsonProperty(PropertyName = "Trade Commission")]
        TradeCommission,
        Financial,
        [JsonProperty(PropertyName = "Financial Advisor")]
        FinancialAdvisor,
        [JsonProperty(PropertyName = "Life Insurance")]
        LifeInsurance,
        Alcohol,
        Bar,
        Food,
        Dining,
        [JsonProperty(PropertyName = "Coffee Shop")]
        CoffeeShop,
        [JsonProperty(PropertyName = "Fast Food")]
        FastFood,
        Groceries,
        Lunch,
        Restaurants,
        Charity,
        Gift,
        Donation,
        Health,
        Fitness,
        Dentist,
        Doctor,
        Eyecare,
        Gym,
        [JsonProperty(PropertyName = "Health Insurance")]
        HealthInsurance,
        Pharmacy,
        Sports,
        Home,
        Furnishings,
        [JsonProperty(PropertyName = "HOA Dues")]
        HoaDues,
        [JsonProperty(PropertyName = "Home Improvement")]
        HomeImprovement,
        [JsonProperty(PropertyName = "Home Insurance")]
        HomeInsurance,
        [JsonProperty(PropertyName = "Home Services")]
        HomeServices,
        [JsonProperty(PropertyName = "Home Supplies")]
        HomeSupplies,
        [JsonProperty(PropertyName = "Lawn & Garden")]
        LawnAndGarden,
        Mortgage,
        Rent,
        Interest,
        [JsonProperty(PropertyName = "IRA Contribution")]
        IraContribution,
        Kids,
        Allowance,
        [JsonProperty(PropertyName = "Baby Supplies")]
        BabySupplies,
        Babysitter,
        Daycare,
        [JsonProperty(PropertyName = "Child Support")]
        ChildSupport,
        [JsonProperty(PropertyName = "Kids Activities")]
        KidsActivities,
        Toys,
        [JsonProperty(PropertyName = "Loan Payment")]
        LoanPayment,
        [JsonProperty(PropertyName = "Loan Interest")]
        LoanInterest,
        [JsonProperty(PropertyName = "Loan Principal")]
        LoanPrincipal,
        [JsonProperty(PropertyName = "Misc.")]
        Misc,
        [JsonProperty(PropertyName = "Not Sure")]
        NotSure,
        [JsonProperty(PropertyName = "Personal Care")]
        PersonalCare,
        Hair,
        Laundry,
        Spa,
        Massage,
        Pets,
        [JsonProperty(PropertyName = "Pet Food")]
        PetFood,
        [JsonProperty(PropertyName = "Pet Supplies")]
        PetSupplies,
        [JsonProperty(PropertyName = "Pet Grooming")]
        PetGrooming,
        Vet,
        Shopping,
        Books,
        Clothing,
        Electronics,
        Software,
        Hobby,
        [JsonProperty(PropertyName = "Sporting Goods")]
        SportingGoods,
        Tax,
        [JsonProperty(PropertyName = "Federal Tax")]
        FederalTax,
        Medicare,
        [JsonProperty(PropertyName = "Other Tax")]
        OtherTax,
        [JsonProperty(PropertyName = "Property Tax")]
        PropertyTax,
        [JsonProperty(PropertyName = "SDI Tax")]
        SdiTax,
        [JsonProperty(PropertyName = "Social Security")]
        SocialSecurity,
        [JsonProperty(PropertyName = "State Tax")]
        StateTax,
        [JsonProperty(PropertyName = "City Tax")]
        CityTax,
        Travel,
        [JsonProperty(PropertyName = "Air Travel")]
        AirTravel,
        Hotel,
        [JsonProperty(PropertyName = "Rental Car")]
        RentalCar,
        Taxi,
        Uber,
        Lyft,
        Vacation
    }
}