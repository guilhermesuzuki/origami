using System.Text.Json.Serialization;

namespace Origami.Core.Models
{
    public class GoogleUserTokenInfoRefresh
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; } = string.Empty;

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;

        /// <summary>
        /// Or new/refreshed token
        /// </summary>
        [JsonPropertyName("id_token")]
        public string IdToken { get; set; } = string.Empty;
    }
}
