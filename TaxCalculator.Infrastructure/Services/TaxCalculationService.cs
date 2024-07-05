using Newtonsoft.Json;
using TaxCalculator.Model.Entities;
using TaxCalculator.Model.Interfaces;
using TaxCalculator.Model.Interfaces.Application;

namespace TaxCalculator.Infrastructure.Services
{
    public class TaxCalculationService : ITaxCalculationService
    {
        private readonly IEnumerable<ITaxCalculator> _taxCalculator;

        [JsonConstructor]
        public TaxCalculationService(IEnumerable<ITaxCalculator> taxCalculators)
        {
            _taxCalculator = taxCalculators;
        }

        public EmployeeRecord Run(TaxEngine taxPayer)
        {
            var employeeRecord = new Dictionary<string, decimal>();

            foreach (ITaxCalculator taxCalculator in _taxCalculator)
            {
                employeeRecord[taxCalculator.TaxType] = taxCalculator.CalculateTax(taxPayer);
            }

            decimal totalTax = employeeRecord.Values.Sum();

            decimal netIncome = taxPayer.GrossIncome - totalTax;

            return new EmployeeRecord(taxPayer.GrossIncome, taxPayer.CharitySpent, employeeRecord, totalTax, netIncome);
        }
    }
}
