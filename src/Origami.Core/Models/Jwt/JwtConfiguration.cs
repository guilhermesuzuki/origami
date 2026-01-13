namespace Origami.Core.Models.Jwt
{
    /// <summary>
    /// JWT Configuration
    /// </summary>
    public class JwtConfiguration
    {
        public string Issuer { get; set; } = "origami-admin";
        public string Audience { get; set; } = "origami-admin-users";
        public string Key { get; set; } = "origami-admin-key";
    }
}
