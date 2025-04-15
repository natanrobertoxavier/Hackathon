using Consultation.Application.Mapping;
using Consultation.Communication.Response;
using Consultation.Domain.Repositories;
using Serilog;

namespace Consultation.Application.UseCase.Consultation.Recover.RecoverByDoctorId;

public class RecoverByDoctorIdUseCase(
    IConsultationReadOnly consultationReadOnlyrepository,
    ILogger logger) : IRecoverByDoctorIdUseCase
{
    private readonly IConsultationReadOnly _consultationReadOnlyrepository = consultationReadOnlyrepository;
    private readonly ILogger _logger = logger;

    public async Task<Result<IEnumerable<ResponseConsultation>>> RecoverByDoctorIdAsync(Guid doctorId)
    {
        var output = new Result<IEnumerable<ResponseConsultation>>();

        try
        {
            _logger.Information($"Início {nameof(RecoverByDoctorIdAsync)}.");

            var entities = await _consultationReadOnlyrepository.GetConsultationByDoctorIdAsync(doctorId);

            if (entities is null || entities.Count() == 0)
            {
                output.Succeeded(null);
                _logger.Information($"Fim {nameof(RecoverByDoctorIdAsync)}. Não foram encontrados dados.");
            }
            else
            {
                output.Succeeded(entities.Select(entity => entity.ToResponse()));
                _logger.Information($"Fim {nameof(RecoverByDoctorIdAsync)}.");
            }
        }
        catch (Exception ex)
        {
            var errorMessage = string.Format("Algo deu errado: {0}", ex.Message);
            _logger.Error(ex, errorMessage);
            output.Failure(new List<string>() { errorMessage });
        }

        return output;
    }
}