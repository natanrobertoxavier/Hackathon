using Doctor.Integration.Tests.Fixture;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Doctor.Integration.Tests.Controllers.v1;
public class LoginControllerTests : IClassFixture<CustomWebApplicationFactory<Microsoft.VisualStudio.TestPlatform.TestHost.Program>>
{
    private readonly HttpClient _client;

    public LoginControllerTests(CustomWebApplicationFactory<Microsoft.VisualStudio.TestPlatform.TestHost.Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RecoverByCRPasswordAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new
        {
            CR = "12345",
            Password = "encryptedPassword"
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/Login/RecoverByCRPasswordAsync", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("Test Doctor", responseString);
    }

    [Fact]
    public async Task RecoverByCRPasswordAsync_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var request = new
        {
            CR = "12345",
            Password = "wrongPassword"
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/Login/RecoverByCRPasswordAsync", content);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
