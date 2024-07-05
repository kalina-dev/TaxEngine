using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaxCalculator.Model.Entities
{
    public class TaxEngine 
    {
        [JsonPropertyName("fullName")]
        [JsonInclude]
        [Required(ErrorMessage = "Full name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name can only contain letters and spaces")]
        [CustomValidation(typeof(TaxEngine), nameof(ValidateFullName))]
        public string FullName { get; private set; }

        [JsonPropertyName("dateOfBirth")]
        [JsonInclude]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; private set; }

        [JsonPropertyName("grossIncome")]
        [JsonInclude]
        [Required(ErrorMessage = "Gross income is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Gross income must be a valid positive number.")]
        public decimal GrossIncome { get; private set; }

        [JsonPropertyName("sSN")]
        [JsonInclude] 
        [Required(ErrorMessage = "SSN is required")]
        [Range(10000, 9999999999, ErrorMessage = "SSN is in the range between 5 and 10 digits.")]
        [RegularExpression(@"^\d{5,10}$", ErrorMessage = "SSN must be a valid number between 5 to 10 digits.")]
        public ulong SSN { get; private set; }

        [JsonInclude]
        [JsonPropertyName("charitySpent")]
        [Range(0, double.MaxValue, ErrorMessage = "Charity spent must be a valid positive number.")]
        public decimal CharitySpent { get; private set; }
        
        public TaxEngine(string fullName, DateTime dateOfBirth, decimal grossIncome, ulong sSN, decimal charitySpent)
        {
            FullName = fullName;
            DateOfBirth = dateOfBirth;
            GrossIncome = grossIncome;
            SSN = sSN;
            CharitySpent = charitySpent;
        }
        public static ValidationResult ValidateFullName(string fullName)
        {
            if (!string.IsNullOrWhiteSpace(fullName) && fullName.Trim().Split(' ').Length >= 2)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("Full name contains at least two words");
        }
    }
}
