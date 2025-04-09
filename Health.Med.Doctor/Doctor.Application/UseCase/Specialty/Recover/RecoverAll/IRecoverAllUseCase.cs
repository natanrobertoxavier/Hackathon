using Doctor.Communication.Response;

namespace Doctor.Application.UseCase.Specialty.Recover.RecoverAll;

public interface IRecoverAllUseCase
{
    Task<Result<IEnumerable<ResponseSpecialty>>> RecoverAllAsync(int page, int pageSize);
}
