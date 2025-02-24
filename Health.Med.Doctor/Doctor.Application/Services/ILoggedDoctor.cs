namespace Doctor.Application.Services;

public interface ILoggedDoctor
{
    Task<Domain.Entities.Doctor> GetLoggedDoctorAsync();
}
