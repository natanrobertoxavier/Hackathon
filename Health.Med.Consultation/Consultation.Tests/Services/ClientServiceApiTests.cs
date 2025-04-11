using Consultation.Domain.ModelServices;
using Consultation.Infrastructure.Services;
using Health.Med.Exceptions.ExceptionBase;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Protected;
using Serilog;
using System.Net;

namespace Consultation.Tests.Services;

public class ClientServiceApiTests
{
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<ILogger> _mockLogger;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly ClientServiceApi _clientServiceApi;

    public ClientServiceApiTests()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockLogger = new Mock<ILogger>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        _clientServiceApi = new ClientServiceApi(
            _mockHttpClientFactory.Object,
            _mockLogger.Object,
            _mockHttpContextAccessor.Object
        );
    }

    [Fact]
    public async Task RecoverBasicInformationByEmailAsync_ShouldReturnSuccess_WhenApiReturnsValidResponse()
    {
        // Arrange
        var email = "test@example.com";
        var expectedResponse = new Result<ClientBasicInformationResult>
        {
            Data = new ClientBasicInformationResult
            {
                Id = Guid.NewGuid(),
                PreferredName = "Test User",
                Email = email
            },
            Success = true
        };

        var httpMessageHandler = new Mock<HttpMessageHandler>();
        httpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(expectedResponse))
            });

        var httpClient = new HttpClient(httpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://any-url.com")
        };

        _mockHttpClientFactory.Setup(f => f.CreateClient("ClientApi")).Returns(httpClient);

        _mockHttpContextAccessor.Setup(a => a.HttpContext.Request.Headers["Authorization"]).Returns("Bearer valid_token");

        // Act
        var result = await _clientServiceApi.RecoverBasicInformationByEmailAsync(email);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(email, result.Data.Email);
    }

    [Fact]
    public async Task RecoverBasicInformationByEmailAsync_ShouldReturnFailure_WhenApiReturnsError()
    {
        // Arrange
        var email = "test@example.com";

        var httpMessageHandler = new Mock<HttpMessageHandler>();
        httpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

        var httpClient = new HttpClient(httpMessageHandler.Object);
        _mockHttpClientFactory.Setup(f => f.CreateClient("ClientApi")).Returns(httpClient);

        _mockHttpContextAccessor.Setup(a => a.HttpContext.Request.Headers["Authorization"]).Returns("Bearer valid_token");

        // Act
        var result = await _clientServiceApi.RecoverBasicInformationByEmailAsync(email);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Ocorreu um erro ao chamar a API de clientes", result.Error);
    }

    [Fact]
    public async Task RecoverBasicInformationByEmailAsync_ShouldThrowException_WhenAuthorizationHeaderIsMissing()
    {
        // Arrange
        var email = "test@example.com";

        _mockHttpContextAccessor.Setup(a => a.HttpContext.Request.Headers["Authorization"]).Returns(string.Empty);

        // Act
        var result = await _clientServiceApi.RecoverBasicInformationByEmailAsync(email);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Ocorreu um erro ao chamar a API de clientes", result.Error);
    }
}
