using Newtonsoft.Json;

namespace Client.Domain.ModelServices;

public class DoctorResult
{
    [JsonProperty("doctorId")]
    public Guid DoctorId { get; set; }

    [JsonProperty("registrationDate")]
    public DateTime RegistrationDate { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("preferredName")]
    public string PreferredName { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("cr")]
    public string CR { get; set; } = string.Empty;

    [JsonProperty("specialtyDoctor")]
    public ResponseSpecialtyDoctor SpecialtyDoctor { get; set; } = new ResponseSpecialtyDoctor();

    [JsonProperty("serviceDays")]
    public IEnumerable<ResponseServiceDay> ServiceDays { get; set; } = [];
}

public class ResponseSpecialtyDoctor
{
    [JsonProperty("specialtyId")]
    public Guid SpecialtyId { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("standardDescription")]
    public string StandardDescription { get; set; } = string.Empty;
}

public class ResponseServiceDay
{
    [JsonProperty("day")]
    public string Day { get; set; }

    [JsonProperty("startTime")]
    public TimeSpan StartTime { get; set; }

    [JsonProperty("endTime")]
    public TimeSpan EndTime { get; set; }
}
