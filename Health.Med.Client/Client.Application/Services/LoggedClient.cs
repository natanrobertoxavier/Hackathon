using Client.Domain.Repositories.Contracts;
using Microsoft.AspNetCore.Http;
using TokenService.Manager.Controller;

namespace Client.Application.Services;

public class LoggedClient(
    IHttpContextAccessor httpContextAccessor,
    TokenController tokenController,
    IClientReadOnly repository) : ILoggedClient
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly TokenController _tokenController = tokenController;
    private readonly IClientReadOnly _repository = repository;

    public async Task<Domain.Entities.Client> GetLoggedClientAsync()
    {
        var authorization = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

        var token = authorization["Bearer".Length..].Trim();

        var email = _tokenController.RecoverEmail(token);

        return await _repository.RecoverByEmailAsync(email);
    }
}
