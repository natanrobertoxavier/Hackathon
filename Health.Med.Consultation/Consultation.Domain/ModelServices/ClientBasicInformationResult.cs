using Newtonsoft.Json;

namespace Consultation.Domain.ModelServices;

public class ClientBasicInformationResult
{
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("preferredName")]
    public string PreferredName { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }
}
