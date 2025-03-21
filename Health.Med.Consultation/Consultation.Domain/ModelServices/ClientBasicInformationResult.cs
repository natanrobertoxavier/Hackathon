using Newtonsoft.Json;

namespace Consultation.Domain.ModelServices;

public class ClientBasicInformationResult
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
}
