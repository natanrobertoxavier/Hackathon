using Newtonsoft.Json;

namespace Consultation.Application.DTO;

public class LoggedUserDto(
    Guid id,
    string preferredName,
    string email)
{
    public Guid Id { get; set; } = id;
    public string PreferredName { get; set; } = preferredName;
    public string Email { get; set; } = email;
}
