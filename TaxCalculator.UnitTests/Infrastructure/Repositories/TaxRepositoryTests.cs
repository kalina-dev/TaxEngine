using Microsoft.Extensions.Options;
using TaxCalculator.Model.Repositories;
using TaxCalculator.Model.Entities;

namespace TaxCalculator.UnitTests.Infrastructure.Repositories
{
    public class TaxRepositoryTests
    {
        private readonly IOptions<Tax> _mockTaxConfigOptions;
        private readonly TaxRepository _repository;

        public TaxRepositoryTests()
        {
            var tax = new Tax
            {
                MinSocialTax = 1000m,
                MaxSocialTax = 5000m,
                SocialTaxRate = 0.15m,
                MinIncomeTax = 2000m,
                IncomeTaxRate = 0.25m,
                CharitySpentMaxRate = 0.1m,
                BaseCurrency = "USD"
            };
            _mockTaxConfigOptions = Options.Create(tax);
            _repository = new TaxRepository(_mockTaxConfigOptions);
        }
    }
}
