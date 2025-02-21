using Doctor.Communication.Request;

namespace Doctor.Application.Mapping;

public static class DoctorMapping
{
    public static Domain.Entities.Doctor ToEntity(this RequestRegisterDoctor request, string password)
    {
        return new Domain.Entities.Doctor(
            request.Name,
            request.Email,
            request.CR,
            password
        );
    }
}
