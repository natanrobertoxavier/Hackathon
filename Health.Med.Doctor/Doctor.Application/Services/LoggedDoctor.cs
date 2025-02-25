
using Doctor.Domain.Repositories.Contracts;
using Microsoft.AspNetCore.Http;
using TokenService.Manager.Controller;

namespace Doctor.Application.Services;

public class LoggedDoctor(
    IHttpContextAccessor httpContextAccessor,
    TokenController tokenController,
    IDoctorReadOnly repository) : ILoggedDoctor
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly TokenController _tokenController = tokenController;
    private readonly IDoctorReadOnly _repository = repository; 

    public async Task<Domain.Entities.Doctor> GetLoggedDoctorAsync()
    {
        var authorization = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

        var token = authorization["Bearer".Length..].Trim();

        var email = _tokenController.RecoverEmail(token);

        var doctor = await _repository.RecoverByEmailAsync(email);

        return doctor;
    }
}
