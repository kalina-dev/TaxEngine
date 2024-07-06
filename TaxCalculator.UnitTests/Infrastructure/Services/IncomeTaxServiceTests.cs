using Moq;
using System;
using TaxCalculator.Model.Entities;
using TaxCalculator.Model.Interfaces;
using TaxCalculator.Infrastructure.Services;
using Xunit;

namespace TaxCalculator.UnitTests.Infrastructure.Services
{
    public class IncomeTaxServiceTests
    {
        private readonly Mock<IHelperTaxService> _mockHelperTaxCalculation;
        private readonly Mock<ITaxRepository> _mockTaxConfigRepository;
        private readonly IncomeTaxService _incomeTaxCalculator;
        public IncomeTaxServiceTests()
        {
            _mockHelperTaxCalculation = new Mock<IHelperTaxService>();
            _mockTaxConfigRepository = new Mock<ITaxRepository>();
            _incomeTaxCalculator = new IncomeTaxService(_mockHelperTaxCalculation.Object, _mockTaxConfigRepository.Object);

            Setup();
        }

        private void Setup()
        {
            var tax = new Tax
            {
                IncomeTaxRate = 0.10m,
                MinIncomeTax = 1000m,
                CharitySpentMaxRate = 0.10m,
            };

            _mockTaxConfigRepository
                .Setup(h => h.GetTax())
                .Returns(tax);

            _mockHelperTaxCalculation
                .Setup(h => h.TaxableIncome(It.IsAny<decimal>()))
                .Returns((decimal grossIncome) => grossIncome > tax.MinIncomeTax ? grossIncome - tax.MinIncomeTax : 0);

            _mockHelperTaxCalculation
                .Setup(h => h.CharityAdjustment(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns((decimal grossIncome, decimal charitySpent) =>
                    Math.Min(charitySpent, grossIncome * tax.CharitySpentMaxRate));

            _mockHelperTaxCalculation
                .Setup(h => h.AdjustTaxableIncome(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns((decimal taxableIncome, decimal charityAdjustment) =>
                    Math.Max(taxableIncome - charityAdjustment, 0));
        }

        [Fact]
        public void IncomeTaxCalculator_CalculateTax_With_Less_Than_Or_Equal_1000_Income_And_No_Charity_Should_Return_IncomeTax_As_Zero()
        {
            var taxPayer = new TaxEngine("Mehmet Aksak", DateTime.Now, 500, 1234567890, 0);

            var tax = _incomeTaxCalculator.CalculateTax(taxPayer);

            Assert.Equal(0, tax);
        }

        [Fact]
        public void IncomeTaxCalculator_CalculateTax_With_Higher_Than_1000_Income_And_No_Charity_Should_Return_IncomeTax()
        {
            var taxPayer = new TaxEngine("Mehmet Aksak", DateTime.Now, 1800, 1234567890, 0);

            var tax = _incomeTaxCalculator.CalculateTax(taxPayer);

            Assert.Equal(80, tax);
        }

        [Fact]
        public void IncomeTaxCalculator_CalculateTax_With_Income_1000_And_No_Charity_Should_Return_IncomeTax_As_Zero()

        {
            var taxPayer = new TaxEngine("Mehmet Aksak", DateTime.Now, 1000, 1234567890, 0);

            var tax = _incomeTaxCalculator.CalculateTax(taxPayer);

            Assert.Equal(0, tax);
        }


        [Theory]
        [InlineData(1000, 50, 0)]
        [InlineData(800, 100, 0)]
        [InlineData(0, 100, 0)]
        public void IncomeTaxCalculator_CalculateTax_With_Less_Than_Or_Equal_1000_Income_And_With_Charity_Should_Return_IncomeTax_As_Zero(decimal grossIncome, decimal charitySpent, decimal expectedTax)
        {
            var taxPayer = new TaxEngine("Mehmet Aksak", DateTime.Now, grossIncome, 1234567890, charitySpent);

            var tax = _incomeTaxCalculator.CalculateTax(taxPayer);

            Assert.Equal(expectedTax, tax);
        }

        [Theory]
        [InlineData(1100, 100, 0)]
        [InlineData(1100, 110, 0)]
        [InlineData(1050, 120, 0)]
        [InlineData(1050, 300, 0)]
        [InlineData(1100, 300, 0)]
        public void IncomeTaxCalculator_CalculateTax_If_Taxable_Income_Less_Than_Or_Equal_1000_And_With_Higher_Than_1000_Income_And_With_Charity_Should_Return_IncomeTax_As_Zero(decimal grossIncome, decimal charitySpent, decimal expectedTax)
        {
            var taxPayer = new TaxEngine("Mehmet Aksak", DateTime.Now, grossIncome, 1234567890, charitySpent);

            var tax = _incomeTaxCalculator.CalculateTax(taxPayer);

            Assert.Equal(expectedTax, tax);
        }

        [Theory]
        [InlineData(1200, 100, 10)]
        [InlineData(1200, 200, 8)]
        public void IncomeTaxCalculator_CalculateTax_If_Taxable_Income_Higher_Than_1000_And_With_Higher_Than_1000_Income_And_With_Charity_Should_Return_IncomeTax(decimal grossIncome, decimal charitySpent, decimal expectedTax)
        {
            var taxPayer = new TaxEngine("Mehmet Aksak", DateTime.Now, grossIncome, 1234567890, charitySpent);

            var tax = _incomeTaxCalculator.CalculateTax(taxPayer);

            Assert.Equal(expectedTax, tax);
        }

        [Theory]
        [InlineData("Ricardo Meloni", 2000, 12347, 100,90)]
        [InlineData("Cahit Yigit", 1000, 12348, 0, 0)]
        [InlineData("Ihsan Yoldas", 1250, 12349, 250,12.5)]
        [InlineData("Didem Zorba", 1100, 12349, 100, 0)]
        [InlineData("Dorota Wyrobek", 1500, 12346, 50, 45)]
        public void IncomeTaxCalculator_CalculateTax_Should_Return_IncomeTax(string fullName, decimal grossIncome, ulong ssn, decimal charitySpent, decimal expectedTax)
        {
            var taxPayer = new TaxEngine(fullName, DateTime.Now, grossIncome, ssn, charitySpent);

            var tax = _incomeTaxCalculator.CalculateTax(taxPayer);

            Assert.Equal(expectedTax, tax);
        }
    }
}
