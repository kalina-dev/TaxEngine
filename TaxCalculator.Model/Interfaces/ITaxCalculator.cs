using TaxCalculator.Model.Entities;

namespace TaxCalculator.Model.Interfaces
{
    public interface ITaxCalculator
    {
        public decimal CalculateTax(TaxEngine taxPayer);
        public string TaxType { get; }
    }
}
