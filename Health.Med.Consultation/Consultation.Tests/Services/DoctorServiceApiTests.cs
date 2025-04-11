using Consultation.Domain.ModelServices;
using Consultation.Infrastructure.Services;
using Health.Med.Exceptions.ExceptionBase;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Protected;
using Serilog;
using System.Net;

namespace Consultation.Tests.Services;

public class DoctorServiceApiTests
{
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<ILogger> _mockLogger;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly DoctorServiceApi _doctorServiceApi;

    public DoctorServiceApiTests()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockLogger = new Mock<ILogger>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        _doctorServiceApi = new DoctorServiceApi(
            _mockHttpClientFactory.Object,
            _mockLogger.Object,
            _mockHttpContextAccessor.Object
        );
    }

    [Fact]
    public async Task RecoverByIdAsync_ShouldReturnSuccess_WhenApiReturnsValidResponse()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var expectedResponse = new Result<DoctorResult>
        {
            Data = new DoctorResult
            {
                DoctorId = doctorId,
                Name = "Dr. Test",
                Email = "doctor@example.com",
                CR = "12345",
                RegistrationDate = DateTime.UtcNow
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

        _mockHttpClientFactory.Setup(f => f.CreateClient("DoctorApi")).Returns(httpClient);
        _mockHttpContextAccessor.Setup(a => a.HttpContext.Request.Headers["Authorization"]).Returns("Bearer valid_token");

        // Act
        var result = await _doctorServiceApi.RecoverByIdAsync(doctorId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(doctorId, result.Data.DoctorId);
    }

    [Fact]
    public async Task RecoverByIdAsync_ShouldReturnFailure_WhenApiReturnsError()
    {
        // Arrange
        var doctorId = Guid.NewGuid();

        var httpMessageHandler = new Mock<HttpMessageHandler>();
        httpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

        var httpClient = new HttpClient(httpMessageHandler.Object);
        _mockHttpClientFactory.Setup(f => f.CreateClient("DoctorApi")).Returns(httpClient);
        _mockHttpContextAccessor.Setup(a => a.HttpContext.Request.Headers["Authorization"]).Returns("Bearer valid_token");

        // Act
        var result = await _doctorServiceApi.RecoverByIdAsync(doctorId);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Ocorreu um erro ao chamar a API de médicos", result.Error);
    }

    [Fact]
    public async Task RecoverByIdAsync_ShouldThrowException_WhenAuthorizationHeaderIsMissing()
    {
        // Arrange
        var doctorId = Guid.NewGuid();

        _mockHttpContextAccessor.Setup(a => a.HttpContext.Request.Headers["Authorization"]).Returns(string.Empty);

        // Act
        var result = await _doctorServiceApi.RecoverByIdAsync(doctorId);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Ocorreu um erro ao chamar a API de médicos", result.Error);
    }
}
