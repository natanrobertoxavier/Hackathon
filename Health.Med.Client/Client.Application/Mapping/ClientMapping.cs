using Client.Communication.Request;
using Client.Communication.Response;

namespace Client.Application.Mapping;

public static class ClientMapping
{
    public static Domain.Entities.Client ToEntity(this RequestRegisterClient request, string password)
    {
        return new Domain.Entities.Client(
            request.Name,
            request.PreferredName,
            request.Email.ToLower(),
            request.CPF,
            password
        );
    }

    public static ResponseClient ToResponse(this Domain.Entities.Client doctor)
    {
        return new ResponseClient(
            doctor.Id,
            doctor.RegistrationDate,
            doctor.Name,
            doctor.Email,
            doctor.CPF
        );
    }

    public static ResponseLogin ToResponseLogin(this Domain.Entities.Client client, string token)
    {
        return new ResponseLogin(
            client.Name,
            client.Email,
            token
        );
    }

    public static ResponseClientBasicInfo ToBasicResponse(this Domain.Entities.Client client)
    {
        return new ResponseClientBasicInfo(
            client.Id,
            client.PreferredName,
            client.Email
        );
    }
}
