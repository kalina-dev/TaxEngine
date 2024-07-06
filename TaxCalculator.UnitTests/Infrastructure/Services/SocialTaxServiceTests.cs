using Moq;
using System;
using TaxCalculator.Model.Entities;
using TaxCalculator.Model.Interfaces;
using TaxCalculator.Infrastructure.Services;
using Xunit;

namespace TaxCalculator.UnitTests.Infrastructure.Services
{
    public class SocialTaxServiceTests
    {
        private readonly Mock<IHelperTaxService> _mockHelperTaxCalculation;
        private readonly Mock<ITaxRepository> _mockTaxConfigRepository;
        private readonly SocialTaxService _socialTaxCalculator;
        public SocialTaxServiceTests()
        {
            _mockHelperTaxCalculation = new Mock<IHelperTaxService>();
            _mockTaxConfigRepository = new Mock<ITaxRepository>();
            _socialTaxCalculator = new SocialTaxService(_mockHelperTaxCalculation.Object, _mockTaxConfigRepository.Object);

            Setup();
        }

        private void Setup()
        {
            var tax = new Tax
            {
                SocialTaxRate = 0.15m,
                MinSocialTax = 1000,
                MaxSocialTax = 3000,
                CharitySpentMaxRate = 0.10m,
            };

            _mockTaxConfigRepository.Setup(h => h.GetTax())
                .Returns(tax);

            _mockHelperTaxCalculation.Setup(h => h.TaxableIncome(It.IsAny<decimal>()))
                .Returns((decimal grossIncome) => grossIncome > tax.MinSocialTax ? grossIncome - tax.MinSocialTax : 0);

            _mockHelperTaxCalculation.Setup(h => h.CharityAdjustment(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns((decimal grossIncome, decimal charitySpent) =>
                    Math.Min(charitySpent, grossIncome * tax.CharitySpentMaxRate));

            _mockHelperTaxCalculation.Setup(h => h.AdjustTaxableIncome(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns((decimal taxableIncome, decimal charityAdjustment) =>
                    Math.Max(taxableIncome - charityAdjustment, 0));
        }

        [Fact]
        public void SocialTaxCalculator_CalculateTax_With_Less_Than_Or_Equal_1000_Income_And_No_Charity_Should_Return_SocialTax_As_Zero()
        {
            var taxPayer = new TaxEngine("Mehmet Aksak", DateTime.Now, 500, 1234567890, 0);

            var tax = _socialTaxCalculator.CalculateTax(taxPayer);

            Assert.Equal(0, tax);
        }

        [Fact]
        public void SocialTaxCalculator_CalculateTax_With_Higher_Than_1000_Income_And_No_Charity_Should_Return_SocialTax()
        {
            var taxPayer = new TaxEngine("Mehmet Aksak", DateTime.Now, 1800, 1234567890, 0);

            var tax = _socialTaxCalculator.CalculateTax(taxPayer);

            Assert.Equal(120, tax);
        }

        [Fact]
        public void SocialTaxCalculator_CalculateTax_With_Income_1000_And_No_Charity_Should_Return_SocialTax_As_Zero()

        {
            var taxPayer = new TaxEngine("Mehmet Aksak", DateTime.Now, 1000, 1234567890, 0);

            var tax = _socialTaxCalculator.CalculateTax(taxPayer);

            Assert.Equal(0, tax);
        }


        [Theory]
        [InlineData(1000, 50, 0)]
        [InlineData(800, 100, 0)]
        [InlineData(0, 100, 0)]
        public void SocialTaxCalculator_CalculateTax_With_Less_Than_Or_Equal_1000_Income_And_With_Charity_Should_Return_SocialTax_As_Zero(decimal grossIncome, decimal charitySpent, decimal expectedTax)
        {
            var taxPayer = new TaxEngine("Mehmet Aksak", DateTime.Now, grossIncome, 1234567890, charitySpent);

            var tax = _socialTaxCalculator.CalculateTax(taxPayer);

            Assert.Equal(expectedTax, tax);
        }

        [Theory]
        [InlineData(1100, 100, 0)]
        [InlineData(1100, 110, 0)]
        [InlineData(1050, 120, 0)]
        [InlineData(1050, 300, 0)]
        [InlineData(1100, 300, 0)]
        public void SocialTaxCalculator_CalculateTax_If_Taxable_Income_Less_Than_Or_Equal_1000_And_With_Higher_Than_1000_Income_And_With_Charity_Should_Return_SocialTax_As_Zero(decimal grossIncome, decimal charitySpent, decimal expectedTax)
        {
            var taxPayer = new TaxEngine("Mehmet Aksak", DateTime.Now, grossIncome, 1234567890, charitySpent);

            var tax = _socialTaxCalculator.CalculateTax(taxPayer);

            Assert.Equal(expectedTax, tax);
        }

        [Theory]
        [InlineData(1200, 100, 15)]
        [InlineData(1200, 200, 12)]
        public void SocialTaxCalculator_CalculateTax_If_Taxable_Income_Higher_Than_1000_And_With_Higher_Than_1000_Income_And_With_Charity_Should_Return_SocialTax(decimal grossIncome, decimal charitySpent, decimal expectedTax)
        {
            var taxPayer = new TaxEngine("Mehmet Aksak", DateTime.Now, grossIncome, 1234567890, charitySpent);

            var tax = _socialTaxCalculator.CalculateTax(taxPayer);

            Assert.Equal(expectedTax, tax);
        }

        [Fact]
        public void SocialTaxCalculator_CalculateTax_If_Income_Higher_Than_3000_Should_Adjust_Taxable_Income_As_3000_And_Should_Return_SocialTax()
        {
            var taxPayer = new TaxEngine("Mehmet Aksak", DateTime.Now, 3400, 1234567890, 0);

            var tax = _socialTaxCalculator.CalculateTax(taxPayer);

            Assert.Equal(300, tax);
        }

        [Theory]
        [InlineData("Ricardo Meloni", 2000, 12347, 100, 135)]
        [InlineData("Cahit Yigit", 1000, 12348, 0, 0)]
        [InlineData("Ihsan Yoldas", 1250, 12349, 250, 18.75)]
        [InlineData("Didem Zorba", 1100, 12349, 100, 0)]
        [InlineData("Dorota Wyrobek", 3500, 12346, 50, 300)]
        public void SocialTaxCalculator_CalculateTax_Should_Return_SocialTax(string fullName, decimal grossIncome, ulong ssn, decimal charitySpent, decimal expectedTax)
        {
            var taxPayer = new TaxEngine(fullName, DateTime.Now, grossIncome, ssn, charitySpent);

            var tax = _socialTaxCalculator.CalculateTax(taxPayer);

            Assert.Equal(expectedTax, tax);
        }
    }
}
