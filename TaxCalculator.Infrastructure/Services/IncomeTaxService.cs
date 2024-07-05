using TaxCalculator.Model.Entities;
using TaxCalculator.Model.Interfaces;

namespace TaxCalculator.Infrastructure.Services
{
    public class IncomeTaxService : ITaxCalculator
    {
        private readonly IHelperTaxService _helperTaxCalculation;
        private readonly ITaxRepository _taxConfigRepository;
        public IncomeTaxService(IHelperTaxService helperTaxCalculation, ITaxRepository taxConfigRepository)
        {
            _helperTaxCalculation = helperTaxCalculation;
            _taxConfigRepository = taxConfigRepository;
        }
        public string TaxType => Model.Enums.TaxType.IncomeTax.ToString();

        public decimal CalculateTax(TaxEngine taxPayer)
        {

            var tax = _taxConfigRepository.GetTax();

            decimal taxableIncome = _helperTaxCalculation.TaxableIncome(taxPayer.GrossIncome);

            decimal charityAdjustment = _helperTaxCalculation.CharityAdjustment(taxPayer.GrossIncome, taxPayer.CharitySpent);

            taxableIncome = _helperTaxCalculation.AdjustTaxableIncome(taxableIncome, charityAdjustment);

            return taxableIncome * tax.IncomeTaxRate;
        }
    }
}
