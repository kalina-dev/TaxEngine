using TaxCalculator.Model.Entities;

namespace TaxCalculator.Model.Interfaces.Application
{
    public interface ITaxCalculationService
    {
        EmployeeRecord Run(TaxEngine taxPayer);
    }
}
