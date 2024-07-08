using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TaxCalculator.Model.Entities
{
    public class Tax
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("MinSocialTax")]
        [JsonPropertyName("MinSocialTax")]
        public decimal MinSocialTax { get; set; }

        [BsonElement("MaxSocialTax")]
        [JsonPropertyName("MaxSocialTax")]
        public decimal MaxSocialTax { get; set; }

        [BsonElement("SocialTaxRate")]
        [JsonPropertyName("SocialTaxRate")]
        public decimal SocialTaxRate { get; set; }

        [BsonElement("MinIncomeTax")]
        [JsonPropertyName("MinIncomeTax")]
        public decimal MinIncomeTax { get; set; }

        [BsonElement("IncomeTaxRate")]
        [JsonPropertyName("IncomeTaxRate")]
        public decimal IncomeTaxRate { get; set; }

        [BsonElement("CharitySpentMaxRate")]
        [JsonPropertyName("CharitySpentMaxRate")]
        public decimal CharitySpentMaxRate { get; set; }

        [BsonElement("BaseCurrency")]
        [JsonPropertyName("BaseCurrency")]
        public string BaseCurrency { get; set; } = "IDR";
    }
}
