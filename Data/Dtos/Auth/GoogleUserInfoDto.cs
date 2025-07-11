using System.Text.Json.Serialization;

namespace NHT_Marine_BE.Data.Dtos.Auth
{
    public class GoogleUserInfoDto
    {
        [JsonPropertyName("sub")]
        public string Sub { get; set; } = "Unknown";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "Unknown";

        [JsonPropertyName("given_name")]
        public string FirstName { get; set; } = "Unknown";

        [JsonPropertyName("family_name")]
        public string LastName { get; set; } = "Unknown";

        [JsonPropertyName("email")]
        public string Email { get; set; } = "Unknown";

        [JsonPropertyName("picture")]
        public string Picture { get; set; } = string.Empty;

        [JsonPropertyName("email_verified")]
        public bool EmailVerified { get; set; }
    }
}
