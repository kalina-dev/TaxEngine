using Microsoft.Extensions.Options;
using TaxCalculator.Model.Entities;
using TaxCalculator.Model.Interfaces;

namespace TaxCalculator.Model.Repositories
{
    public class TaxRepository : ITaxRepository
    {
        private readonly Tax _taxConfig;
        public TaxRepository(IOptions<Tax> taxConfigOptions)
        {
            _taxConfig = taxConfigOptions.Value;
        }

        public Tax GetTax()
        {
            return new Tax
            {
                MinIncomeTax = 1000m,
                CharitySpentMaxRate = 0.10m,
                IncomeTaxRate = 0.10m,
                SocialTaxRate = 0.15m,
                MinSocialTax = 1000m,
                MaxSocialTax = 3000m
            };
        }
    }
}
