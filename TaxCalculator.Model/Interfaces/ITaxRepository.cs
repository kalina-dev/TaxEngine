using TaxCalculator.Model.Entities;

namespace TaxCalculator.Model.Interfaces
{
    public interface ITaxRepository
    {
        public Tax GetTax();
    }
}
