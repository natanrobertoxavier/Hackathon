namespace Doctor.Application.Services.Doctor;

public interface ILoggedDoctor
{
    Task<Domain.Entities.Doctor> GetLoggedDoctorAsync();
}
