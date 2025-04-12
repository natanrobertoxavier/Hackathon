using FluentValidation;
using Doctor.Communication.Request;

namespace Doctor.Application.UseCase.ServiceDay;

public class ServiceDayValidator : AbstractValidator<Communication.Request.ServiceDay>
{
    public ServiceDayValidator()
    {
        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("O horário de início deve ser menor que o horário de fim.");

        RuleFor(x => x.StartTime)
            .GreaterThanOrEqualTo(TimeSpan.FromHours(6))
            .WithMessage("O horário de início deve ser após as 06:00.");

        RuleFor(x => x.EndTime)
            .LessThanOrEqualTo(TimeSpan.FromHours(22))
            .WithMessage("O horário de fim deve ser até as 22:00.");

        RuleFor(x => x)
            .Must(x => (x.EndTime - x.StartTime).TotalMinutes >= 30)
            .WithMessage("O intervalo mínimo de atendimento é de 30 minutos.");
    }
}
