using System.Text.Json.Serialization;

namespace Origami.Core.Models
{
    /// <summary>
    /// class for the token info endpoint JSON representation
    /// </summary>
    public class GoogleUserTokenInfo :
        IHas
    {
        #region Error

        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;

        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; } = string.Empty;

        #endregion

        #region These six fields are included in all Google ID Tokens

        /// <summary>
        /// Issuer
        /// </summary>
        [JsonPropertyName("iss")]
        public string Iss { get; set; } = string.Empty;

        /// <summary>
        /// Subject
        /// </summary>
        [JsonPropertyName("sub")]
        public string Sub { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("azp")]
        public string Azp { get; set; } = string.Empty;

        /// <summary>
        /// Audience
        /// </summary>
        [JsonPropertyName("aud")]
        public string Aud { get; set; } = string.Empty;

        /// <summary>
        /// Issued At
        /// </summary>
        [JsonPropertyName("iat")]
        public string Iat { get; set; } = string.Empty;

        [JsonPropertyName("exp")]
        public string Exp { get; set; } = string.Empty;

        /// <summary>
        /// Unique identifier for the token
        /// </summary>
        [JsonPropertyName("jti")]
        public string Jti { get; set; } = string.Empty;

        #endregion

        #region User has granted the "profile" and "email" scopes

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("email_verified")]
        public string EmailVerified { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("picture")]
        public string Picture { get; set; } = string.Empty;

        [JsonPropertyName("given_name")]
        public string GivenName { get; set; } = string.Empty;

        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; } = string.Empty;

        [JsonPropertyName("locale")]
        public string Locale { get; set; } = string.Empty;

        #endregion

        /// <summary>
        /// Has Profile?
        /// </summary>
        /// <returns></returns>
        public bool HasProfile() => GivenName.Has() && FamilyName.Has() && Picture.Has();

        /// <summary>
        /// Does it have an Email and Profile information
        /// </summary>
        /// <returns></returns>
        public bool Has()
        {
            return Email.Has() && HasProfile();
        }

        /// <summary>
        /// Expiration time from the Exp property
        /// </summary>
        public long ExpirationTime
        {
            get => long.TryParse(Exp, out long value) ? value : 0;
        }
    }
}
