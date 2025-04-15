namespace Doctor.Application.Services.Doctor;

public interface ILoggedDoctor
{
    Guid GetLoggedDoctorId();
    Domain.Entities.Doctor GetLoggedDoctor();
}
