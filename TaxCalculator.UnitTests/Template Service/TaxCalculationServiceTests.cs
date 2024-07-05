using Moq;
using System;
using System.Collections.Generic;
using TaxCalculator.Infrastructure.Services;
using TaxCalculator.Model.Entities;
using TaxCalculator.Model.Enums;
using TaxCalculator.Model.Interfaces;
using TaxCalculator.Model.Interfaces.Application;
using Xunit;

namespace TaxCalculator.UnitTests.Application.Services
{
    public class TaxCalculationServiceTests
    {
        private readonly Mock<ITaxCalculator> _incomeTaxCalculatorMock;
        private readonly Mock<ITaxCalculator> _socialTaxCalculatorMock;
        private readonly ITaxCalculationService _taxCalculationService;
        public TaxCalculationServiceTests()
        {
            _incomeTaxCalculatorMock = new Mock<ITaxCalculator>();
            _socialTaxCalculatorMock = new Mock<ITaxCalculator>();

            var taxCalculators = new List<ITaxCalculator>
            {
                _incomeTaxCalculatorMock.Object,
                _socialTaxCalculatorMock.Object
            };

            _taxCalculationService = new TaxCalculationService(taxCalculators);
        }

        [Fact]
        public bool TaxCalculationService_CalculateTaxes_With_Valid_TaxPayer_Should_Return_Correct_Taxes()
        {
            var taxPayer = new TaxEngine("Mehmet Aksak", DateTime.Now, 5000, 1234567890, 100);

            _incomeTaxCalculatorMock.Setup(tc => tc.CalculateTax(It.IsAny<TaxEngine>())).Returns(390);
            _incomeTaxCalculatorMock.Setup(tc => tc.TaxType).Returns(TaxType.IncomeTax.ToString());
            _socialTaxCalculatorMock.Setup(tc => tc.CalculateTax(It.IsAny<TaxEngine>())).Returns(300);
            _socialTaxCalculatorMock.Setup(tc => tc.TaxType).Returns(TaxType.SocialTax.ToString());

            var result = _taxCalculationService.Run(taxPayer);

            Assert.Equal(5000, result.GrossIncome);
            Assert.Equal(100, result.CharitySpent);
            Assert.Equal(690, result.TotalTax);
            Assert.Equal(4310, result.NetIncome);
            Assert.Equal(2, result.Taxes.Count);
            Assert.Equal(390, result.Taxes[TaxType.IncomeTax.ToString()]);
            Assert.Equal(300, result.Taxes[TaxType.SocialTax.ToString()]);
            return true;
        }

        [Fact]
        public bool CalculateTaxes_WithZeroGrossIncome_ShouldReturnZeroTaxes()
        {
            var taxPayer = new TaxEngine("Mehmet Aksak", DateTime.Now, 0, 4234355, 50);

            _incomeTaxCalculatorMock.Setup(tc => tc.CalculateTax(It.IsAny<TaxEngine>())).Returns(0);
            _incomeTaxCalculatorMock.Setup(tc => tc.TaxType).Returns(TaxType.IncomeTax.ToString());

            _socialTaxCalculatorMock.Setup(tc => tc.CalculateTax(It.IsAny<TaxEngine>())).Returns(0);
            _socialTaxCalculatorMock.Setup(tc => tc.TaxType).Returns(TaxType.SocialTax.ToString());

            var result = _taxCalculationService.Run(taxPayer);

            Assert.Equal(0, result.GrossIncome);
            Assert.Equal(50, result.CharitySpent);
            Assert.Equal(0, result.TotalTax);
            Assert.Equal(0, result.NetIncome);
            Assert.Equal(2, result.Taxes.Count);
            Assert.Equal(0, result.Taxes[TaxType.IncomeTax.ToString()]);
            Assert.Equal(0, result.Taxes[TaxType.SocialTax.ToString()]);
            return true;
        }
    }
}
