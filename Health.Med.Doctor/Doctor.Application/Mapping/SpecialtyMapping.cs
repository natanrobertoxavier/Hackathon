using Doctor.Communication.Request;
using Doctor.Communication.Response;
using System.Globalization;

namespace Doctor.Application.Mapping;

public static class SpecialtyMapping
{
    public static Domain.Entities.Specialty ToEntity(this RequestRegisterSpecialty request, string standardDescription, Guid userId)
    {
        TextInfo stringHelper = CultureInfo.CurrentCulture.TextInfo;

        return new Domain.Entities.Specialty(
            userId,
            stringHelper.ToTitleCase(request.Description.ToLower()),
            standardDescription
        );
    }

    public static ResponseSpecialty ToResponse(this Domain.Entities.Specialty specialty)
    {
        return new ResponseSpecialty(
            specialty.Id,
            specialty.RegistrationDate,
            specialty.Description,
            specialty.StandardDescription
        );
    }

    public static ResponseSpecialtyDoctor ToSpecialDoctorResponse(this Domain.Entities.Specialty specialty)
    {
        return new ResponseSpecialtyDoctor(
            specialty.Id,
            specialty.Description,
            specialty.StandardDescription
        );
    }
}