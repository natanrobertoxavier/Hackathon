using Doctor.Communication.Request;
using Doctor.Communication.Response;

namespace Doctor.Application.Mapping;

public static class DoctorMapping
{
    public static Domain.Entities.Doctor ToEntity(this RequestRegisterDoctor request, string password)
    {
        return new Domain.Entities.Doctor(
            request.Name,
            request.Email.ToLower(),
            request.CR.ToUpper(),
            password
        );
    }

    public static ResponseDoctor ToResponse(this Domain.Entities.Doctor doctor)
    {
        return new ResponseDoctor(
            doctor.Id,
            doctor.RegistrationDate,
            doctor.Name,
            doctor.Email,
            doctor.CR
        );
    }

    public static ResponseLogin ToResponseLogin(this Domain.Entities.Doctor doctor)
    {
        return new ResponseLogin(
            doctor.Name,
            doctor.CR,
            doctor.Email
        );
    }
}