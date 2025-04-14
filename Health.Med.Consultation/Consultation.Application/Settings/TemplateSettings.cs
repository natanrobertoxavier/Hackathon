namespace Consultation.Application.Settings;

public class TemplateSettings
{
    public ClientSettings ClientSettings { get; set; }
    public DoctorSettings DoctorSettings { get; set; }
}

public class ClientSettings
{
    public string PathTemplateClient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
}

public class DoctorSettings
{
    public string PathTemplateDoctor { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string BasePath { get; set; } = string.Empty;
}
