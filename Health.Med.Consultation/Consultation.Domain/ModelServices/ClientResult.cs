using Newtonsoft.Json;

namespace Consultation.Domain.ModelServices;

public class ClientResult
{
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("registrationDate")]
    public DateTime RegistrationDate { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("cpf")]
    public string CPF { get; set; } = string.Empty;
}
