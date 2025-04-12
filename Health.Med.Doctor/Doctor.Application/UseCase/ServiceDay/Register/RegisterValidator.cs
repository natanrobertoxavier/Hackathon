using Doctor.Communication.Request;
using FluentValidation;

namespace Doctor.Application.UseCase.ServiceDay.Register;

public class RegisterValidator : AbstractValidator<RequestServiceDay>
{
    public RegisterValidator()
    {
        RuleFor(x => x.ServiceDays)
            .NotEmpty().WithMessage("É necessário informar ao menos um dia de atendimento.");

        RuleFor(x => x.ServiceDays)
            .Must(serviceDays => serviceDays.Select(d => d.Day).Distinct().Count() == serviceDays.Count)
            .WithMessage("Não é permitido repetir dias da semana.");

        RuleForEach(x => x.ServiceDays)
            .SetValidator(new ServiceDayValidator());
    }
}
