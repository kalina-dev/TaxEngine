using TaxCalculator.Model.Interfaces;

namespace TaxCalculator.Infrastructure.Helpers
{
    public class HelperTaxService : IHelperTaxService
    {
        private readonly ITaxRepository _taxConfigRepository;
        public HelperTaxService(ITaxRepository taxConfigRepository)
        {
            _taxConfigRepository = taxConfigRepository;
        }

        public decimal TaxableIncome(decimal grossIncome)
        {
            var tax = _taxConfigRepository.GetTax();

            return grossIncome > tax.MinIncomeTax ? grossIncome - tax.MinIncomeTax : 0;
        }

        public decimal CharityAdjustment(decimal grossIncome, decimal charitySpent)
        {
            var tax = _taxConfigRepository.GetTax();

            return Math.Min(charitySpent, grossIncome * tax.CharitySpentMaxRate);
        }

        public decimal AdjustTaxableIncome(decimal taxableIncome, decimal charityAdjustment)
        {
            var adjustedIncome = taxableIncome - charityAdjustment;

            return Math.Max(adjustedIncome, 0);
        }
    }
}
