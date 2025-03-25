using Doctor.Communication.Request;

namespace Doctor.Application.Mapping;

public static class SpecialtyMapping
{
    public static Domain.Entities.Specialty ToEntity(this RequestRegisterSpecialty request, string standardDescription, Guid userId)
    {
        return new Domain.Entities.Specialty(
            userId,
            request.Description,
            standardDescription
        );
    }

    //public static ResponseDoctor ToResponse(this Domain.Entities.Doctor doctor)
    //{
    //    return new ResponseDoctor(
    //        doctor.Id,
    //        doctor.RegistrationDate,
    //        doctor.Name,
    //        doctor.Email,
    //        doctor.CR
    //    );
    //}
}