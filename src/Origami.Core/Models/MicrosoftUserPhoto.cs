using System.Text.Json.Serialization;

namespace Origami.Core.Models
{
    public class MicrosoftUserPhoto
    {
        [JsonPropertyName("height")]
        public string Height { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("@odata.context")]
        public string OData_Context { get; set; } = string.Empty;
        
        [JsonPropertyName("@odata.id")]
        public string OData_Id { get; set; } = string.Empty;
        
        [JsonPropertyName("@odata.mediaContentType")]
        public string OData_MediaContentType { get; set; } = string.Empty;

        [JsonPropertyName("@odata.mediaEtag")]
        public string OData_MediaETag { get; set; } = string.Empty;
        
        [JsonPropertyName("width")]
        public string Width { get; set; } = string.Empty;
    }
}
