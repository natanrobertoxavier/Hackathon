using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Doctor.Domain.Entities;

namespace Doctor.Application.Mapping;

public static class DoctorMapping
{
    public static Domain.Entities.Doctor ToEntity(this RequestRegisterDoctor request, string password, Guid userId)
    {
        return new Domain.Entities.Doctor(
            request.Name,
            request.PreferredName,
            request.Email.ToLower(),
            request.CR.ToUpper(),
            password,
            request.SpecialtyId,
            userId,
            request.ConsultationPrice
        );
    }

    public static ResponseDoctor ToResponse(this Domain.Entities.Doctor doctor)
    {
        return new ResponseDoctor(
            doctor.Id,
            doctor.RegistrationDate,
            doctor.Name,
            doctor.PreferredName,
            doctor.Email,
            doctor.CR,
            doctor.Specialty.ToSpecialDoctorResponse(),
            doctor.ServiceDays.Select(serviceDay => serviceDay.ToRespose()).ToList(),
            doctor.ConsultationPrice
        );
    }

    public static ResponseLogin ToResponseLogin(this Domain.Entities.Doctor doctor, string token)
    {
        return new ResponseLogin(
            doctor.Name,
            doctor.PreferredName,
            doctor.CR,
            doctor.Email,
            token
        );
    }

    public static ResponseScheduleDoctor ToResponseSchedule(this Domain.Entities.Doctor doctor)
    {
        return new ResponseScheduleDoctor(
            doctor.Name,
            doctor.ConsultationPrice,
            doctor.ServiceDays.Select(serviceDay => serviceDay.ToRespose()).ToList());
    }
}