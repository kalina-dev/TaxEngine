namespace TaxCalculator.Model.Entities
{
    public class TaxesStoreDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string TaxesCollectionName { get; set; } = null!;
    }
}
