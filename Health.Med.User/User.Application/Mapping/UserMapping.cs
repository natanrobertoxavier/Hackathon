using User.Communication.Request;
using User.Communication.Response;

namespace User.Application.Mapping;

public static class UserMapping
{
    public static Domain.Entities.User ToEntity(this RequestRegisterUser request, string password)
    {
        return new Domain.Entities.User(
            request.Name,
            request.Email.ToLower(),
            password
        );
    }

    public static ResponseUser ToResponse(this Domain.Entities.User doctor)
    {
        return new ResponseUser(
            doctor.Id,
            doctor.RegistrationDate,
            doctor.Name,
            doctor.Email
        );
    }

    public static ResponseLogin ToResponseLogin(this Domain.Entities.User doctor, string token)
    {
        return new ResponseLogin(
            doctor.Name,
            doctor.Email,
            token
        );
    }
}
