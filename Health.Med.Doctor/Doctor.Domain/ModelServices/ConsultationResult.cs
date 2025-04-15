using Newtonsoft.Json;

namespace Doctor.Domain.ModelServices;

public class ConsultationResult
{
    [JsonProperty("consultationDate")]
    public DateTime ConsultationDate { get; set; }
}
