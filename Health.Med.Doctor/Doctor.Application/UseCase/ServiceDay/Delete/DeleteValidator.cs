using Doctor.Communication.Request;
using FluentValidation;

namespace Doctor.Application.UseCase.ServiceDay.Delete;

public class DeleteValidator : AbstractValidator<RequestDeleteServiceDay>
{
    public DeleteValidator()
    {
    }
}
