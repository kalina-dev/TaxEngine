using TaxCalculator.Model.Interfaces;

namespace TaxCalculator.Infrastructure.Helpers
{
    public class HelperTaxService : IHelperTaxService
    {
        private readonly ITaxRepository _taxRepository;
        public HelperTaxService(ITaxRepository taxRepository)
        {
            _taxRepository = taxRepository;
        }

        public decimal TaxableIncome(decimal grossIncome)
        {
            var tax = _taxRepository.GetTax();

            return grossIncome > tax.MinIncomeTax ? grossIncome - tax.MinIncomeTax : 0;
        }

        public decimal CharityAdjustment(decimal grossIncome, decimal charitySpent)
        {
            var tax = _taxRepository.GetTax();

            return Math.Min(charitySpent, grossIncome * tax.CharitySpentMaxRate);
        }

        public decimal AdjustTaxableIncome(decimal taxableIncome, decimal charityAdjustment)
        {
            var adjustedIncome = taxableIncome - charityAdjustment;

            return Math.Max(adjustedIncome, 0);
        }
    }
}
