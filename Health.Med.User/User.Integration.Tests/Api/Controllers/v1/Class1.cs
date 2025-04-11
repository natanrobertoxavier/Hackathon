using System.Net;
using System.Text;
using System.Text.Json;

namespace User.Integration.Tests.Api.Controllers.v1;

public class LoginControllerTests(
    CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task RecoverByCRPasswordAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new
        {
            Email = "testuser@example.com",
            Password = "123456"
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/login", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("Test User", responseString);
    }

    [Fact]
    public async Task RecoverByCRPasswordAsync_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var request = new
        {
            Email = "testuser@example.com",
            Password = "wrongPassword"
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/login", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
