using System.Text.Json.Serialization;

namespace Origami.Core.Models
{
    public class MicrosoftUser
    {
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonPropertyName("givenName")]
        public string GivenName { get; set; } = string.Empty;

        [JsonPropertyName("jobTitle")]
        public string JobTitle { get; set; } = string.Empty;

        [JsonPropertyName("mail")]
        public string Mail { get; set; } = string.Empty;

        [JsonPropertyName("mobilePhone")]
        public string MobilePhone { get; set; } = string.Empty;

        [JsonPropertyName("officeLocation")]
        public string OfficeLocation { get; set; } = string.Empty;

        [JsonPropertyName("preferredLanguage")]
        public string PreferredLanguage { get; set; } = string.Empty;

        [JsonPropertyName("surName")]
        public string SurName { get; set; } = string.Empty;

        [JsonPropertyName("userPrincipalName")]
        public string UserPrincipalName { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }
}
