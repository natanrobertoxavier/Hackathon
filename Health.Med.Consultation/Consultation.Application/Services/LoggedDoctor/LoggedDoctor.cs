using Consultation.Application.Services.LoggedClientService;
using Health.Med.Exceptions.ExceptionBase;
using Microsoft.AspNetCore.Http;

namespace Consultation.Application.Services.LoggedDoctor;

public class LoggedDoctor(IHttpContextAccessor httpContextAccessor) : ILoggedDoctor
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid GetLoggedDoctorId()
    {
        if (_httpContextAccessor.HttpContext?.Items["AuthenticatedDoctor"]
            is not Domain.ModelServices.DoctorResult doctor)
        {
            throw new HealthMedException("Médico não localizado");
        }

        return doctor.DoctorId;
    }
}
