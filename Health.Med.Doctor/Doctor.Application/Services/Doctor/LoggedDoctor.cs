using Doctor.Domain.Repositories.Contracts.Doctor;
using Health.Med.Exceptions.ExceptionBase;
using Microsoft.AspNetCore.Http;
using TokenService.Manager.Controller;

namespace Doctor.Application.Services.Doctor;

public class LoggedDoctor(IHttpContextAccessor httpContextAccessor) : ILoggedDoctor
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid GetLoggedDoctorId()
    {
        if (_httpContextAccessor.HttpContext?.Items["AuthenticatedDoctor"]
            is not Domain.Entities.Doctor doctor)
        {
            throw new HealthMedException("Médico não localizado");
        }

        return doctor.Id;
    }

    public Domain.Entities.Doctor GetLoggedDoctor()
    {
        if (_httpContextAccessor.HttpContext?.Items["AuthenticatedDoctor"]
            is not Domain.Entities.Doctor doctor)
        {
            throw new HealthMedException("Médico não localizado");
        }

        return doctor;
    }
}