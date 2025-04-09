using Consultation.Communication.Request;
using Consultation.Application.Extensions;

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
}
