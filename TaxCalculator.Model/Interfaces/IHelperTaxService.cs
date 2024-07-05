namespace TaxCalculator.Model.Interfaces
{
    public interface IHelperTaxService
    {
        public decimal TaxableIncome(decimal grossIncome);
        public decimal CharityAdjustment(decimal grossIncome, decimal charitySpent);
        public decimal AdjustTaxableIncome(decimal taxableIncome, decimal charityAdjustment);
    }
}
