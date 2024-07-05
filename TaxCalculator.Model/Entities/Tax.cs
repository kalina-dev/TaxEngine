namespace TaxCalculator.Model.Entities
{
    public class Tax
    {
        public decimal MinSocialTax { get; set; }
        public decimal MaxSocialTax { get; set; }
        public decimal SocialTaxRate { get; set; }
        public decimal MinIncomeTax { get; set; }
        public decimal IncomeTaxRate { get; set; }
        public decimal CharitySpentMaxRate { get; set; }
        public string BaseCurrency { get; set; } = "IDR";
    }
}
