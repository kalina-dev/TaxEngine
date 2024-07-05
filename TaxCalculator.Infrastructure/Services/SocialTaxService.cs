using TaxCalculator.Infrastructure.Helpers;
using TaxCalculator.Model.Entities;
using TaxCalculator.Model.Interfaces;

namespace TaxCalculator.Infrastructure.Services
{
    public class SocialTaxService : ITaxCalculator
    {
        private readonly IHelperTaxService _helperTaxCalculation;
        private readonly ITaxRepository _taxRepository;
        public SocialTaxService(IHelperTaxService helperTaxCalculation, ITaxRepository taxRepository)
        {
            _helperTaxCalculation = helperTaxCalculation;
            _taxRepository = taxRepository;
        }
        public string TaxType => Model.Enums.TaxType.SocialTax.ToString();

        public decimal CalculateTax(TaxEngine taxPayer)
        {

            var tax = _taxRepository.GetTax();

            decimal taxableIncome = _helperTaxCalculation.TaxableIncome(taxPayer.GrossIncome);

            decimal charityAdjustment = _helperTaxCalculation.CharityAdjustment(taxPayer.GrossIncome, taxPayer.CharitySpent);

            taxableIncome = _helperTaxCalculation.AdjustTaxableIncome(taxableIncome, charityAdjustment);

            decimal socialTaxableIncome = taxPayer.GrossIncome > tax.MinSocialTax ? Math.Min(taxableIncome, tax.MaxSocialTax - tax.MinSocialTax) : 0;

            return socialTaxableIncome * tax.SocialTaxRate;
        }
    }
}
