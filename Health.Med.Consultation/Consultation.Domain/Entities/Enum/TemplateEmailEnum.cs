using System.ComponentModel;

namespace Consultation.Domain.Entities.Enum;

public enum TemplateEmailEnum
{
    [Description("ConsultationScheduleClient")]
    ConsultationSchedulingClientEmail = 1,
    [Description("ConsultationScheduleDoctor")]
    ConsultationSchedulingDoctorEmail = 2,
}
