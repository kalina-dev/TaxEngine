using Moq;
using Xunit;
using TaxCalculator.Infrastructure.Helpers;
using TaxCalculator.Model.Entities;
using TaxCalculator.Model.Interfaces;

namespace TaxCalculator.UnitTests.Infrastructure.Helpers
{
    public class HelperTaxServiceTests
    {
        private readonly Mock<ITaxRepository> _mockTaxConfigRepository;
        private readonly HelperTaxService _helperTaxCalculation;

        public HelperTaxServiceTests()
        {
            _mockTaxConfigRepository = new Mock<ITaxRepository>();
            _helperTaxCalculation = new HelperTaxService(_mockTaxConfigRepository.Object);

            var tax = new Tax
            {
                MinIncomeTax = 1000m,
                CharitySpentMaxRate = 0.10m,
                IncomeTaxRate = 0.10m,
                SocialTaxRate = 0.15m,
                MinSocialTax = 1000m,
                MaxSocialTax = 3000m
            };

            _mockTaxConfigRepository
                .Setup(sq => sq.GetTax())
                .Returns(tax);
        }

        [Fact]
        public bool HelperTaxCalculation_TaxableIncome_With_Income_Higher_Than_Threshold_Should_Return_TaxableIncome()
        {
            decimal grossIncome = 2000;

            var taxableIncome = _helperTaxCalculation.TaxableIncome(grossIncome);

            Assert.Equal(1000, taxableIncome);
            return true;
        }

        [Theory]
        [InlineData(1000, 0)]
        [InlineData(700, 0)]
        public bool HelperTaxCalculation_TaxableIncome_With_Income_Less_Than_Or_Equal_Threshold_Should_Return_Zero(decimal grossIncome, decimal expected)
        {

            var taxableIncome = _helperTaxCalculation.TaxableIncome(grossIncome);

            Assert.Equal(expected, taxableIncome);
            return true;
        }

        [Theory]
        [InlineData(2000, 150, 150)]
        [InlineData(3000, 300, 300)]
        public bool HelperTaxCalculation_CharityAdjustment_With_CharitySpent_Should_Return_Adjustment(decimal grossIncome, decimal charitySpent, decimal expected)
        {

            var charityAdjustment = _helperTaxCalculation.CharityAdjustment(grossIncome, charitySpent);

            Assert.Equal(expected, charityAdjustment);
            return true;
        }

        [Theory]
        [InlineData(2500, 500, 250)]
        [InlineData(2000, 250, 200)]
        public bool HelperTaxCalculation_CharityAdjustment_With_CharitySpent_Higher_than_MaxRate_Should_Return_MaxRate_Adjustment(decimal grossIncome, decimal charitySpent, decimal expected)
        {

            var charityAdjustment = _helperTaxCalculation.CharityAdjustment(grossIncome, charitySpent);

            Assert.Equal(expected, charityAdjustment);
            return true;
        }

        [Fact]
        public bool HelperTaxCalculation_AdjustTaxableIncome_With_Positive_Income_And_Charity_Should_Return_Adjusted_Income()
        {
            decimal taxableIncome = 1000;
            decimal charityAdjustment = 200;

            var adjustedIncome = _helperTaxCalculation.AdjustTaxableIncome(taxableIncome, charityAdjustment);

            Assert.Equal(800, adjustedIncome);
            return true;
        }

        [Fact]
        public bool HelperTaxCalculation_AdjustTaxableIncome_With_Negative_Result_Should_Return_Zero()
        {
            decimal taxableIncome = 100;
            decimal charityAdjustment = 200;

            var adjustedIncome = _helperTaxCalculation.AdjustTaxableIncome(taxableIncome, charityAdjustment);

            Assert.Equal(0, adjustedIncome);
            return true;
        }
    }
}
