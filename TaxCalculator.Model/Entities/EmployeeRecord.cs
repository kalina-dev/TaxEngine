
using System.Text.Json.Serialization;

namespace TaxCalculator.Model.Entities
{
    public class EmployeeRecord
    {
        [JsonInclude]
        [JsonPropertyName("grossIncome")]
        public decimal GrossIncome { get; private set; }

        [JsonInclude]
        [JsonPropertyName("charitySpent")]
        public decimal CharitySpent { get; private set; }

        [JsonInclude]
        [JsonPropertyName("taxes")]
        public Dictionary<string, decimal> Taxes { get; private set; }

        [JsonInclude]
        [JsonPropertyName("totalTax")]
        public decimal TotalTax { get; private set; }

        [JsonPropertyName("netIncome")]
        [JsonInclude]
        public decimal NetIncome { get; private set; }

        public EmployeeRecord(decimal grossIncome, decimal charitySpent, Dictionary<string, decimal> appliedTaxes, decimal totalTax, decimal netIncome)
        {
            GrossIncome = grossIncome;
            CharitySpent = charitySpent;
            Taxes = appliedTaxes;
            TotalTax = totalTax;
            NetIncome = netIncome;
        }
    }
}
