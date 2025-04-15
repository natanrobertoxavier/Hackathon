using Consultation.Application.Extensions;
using Consultation.Communication.Request;
using Consultation.Communication.Response;

namespace Consultation.Application.Mapping;

public static class ConsultationMapping
{
    public static Domain.Entities.Consultation ToEntity(this RequestRegisterConsultation request, Guid clientId)
    {
        return new Domain.Entities.Consultation(
            clientId,
            request.DoctorId,
            request.ConsultationDate.TrimMilliseconds()
        );
    }

    public static ResponseConsultation ToResponse(this Domain.Entities.Consultation entity) =>
        new ResponseConsultation(entity.ConsultationDate);
}
