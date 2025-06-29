﻿using Client.Application.Mapping;
using Client.Communication.Request;
using Client.Communication.Response;
using Client.Domain.Repositories.Contracts;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;
using TokenService.Manager.Controller;

namespace Client.Application.UseCase.Login
{
    public class LoginUseCase(
    IClientReadOnly clientReadOnlyrepository,
    PasswordEncryptor passwordEncryptor,
    TokenController tokenController,
    ILogger logger) : ILoginUseCase
    {
        private readonly IClientReadOnly _clientReadOnlyrepository = clientReadOnlyrepository;
        private readonly PasswordEncryptor _passwordEncryptor = passwordEncryptor;
        private readonly TokenController _tokenController = tokenController;
        private readonly ILogger _logger = logger;

        public async Task<Result<ResponseLogin>> LoginAsync(RequestLoginClient request)
        {
            var output = new Result<ResponseLogin>();

            try
            {
                _logger.Information($"Início {nameof(LoginAsync)}.");

                var encryptedPassword = _passwordEncryptor.Encrypt(request.Password);

                var entity = await _clientReadOnlyrepository.RecoverByEmailPasswordAsync(request.Email.ToLower(), encryptedPassword);

                if (entity?.Id == Guid.Empty)
                {
                    _logger.Information($"Fim {nameof(LoginAsync)}. Não foram encontrados dados.");

                    throw new InvalidLoginException();
                }
                else
                {
                    var token = _tokenController.GenerateToken(entity.Email);

                    output.Succeeded(entity.ToResponseLogin(token));

                    _logger.Information($"Fim {nameof(LoginAsync)}.");
                }
            }
            catch (InvalidLoginException ex)
            {
                output.Failure(new List<string>() { ex.Message });
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("Algo deu errado: {0}", ex.Message);

                output.Failure(new List<string>() { errorMessage });

                _logger.Error(ex, errorMessage);
            }

            return output;
        }
    }
}
