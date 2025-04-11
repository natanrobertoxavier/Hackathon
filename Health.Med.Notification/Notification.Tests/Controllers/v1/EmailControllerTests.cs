using Microsoft.AspNetCore.Mvc;
using Moq;
using Notification.Api.Controllers.v1;
using Notification.Application.UseCase.SendMail;
using Notification.Communication.Request;
using Notification.Communication.Response;
using System.Net;

namespace Notification.Tests.Controllers.v1;

public class EmailControllerTests
{
    private readonly Mock<ISendMailUseCase> _mockSendMailUseCase;
    private readonly EmailController _controller;

    public EmailControllerTests()
    {
        _mockSendMailUseCase = new Mock<ISendMailUseCase>();
        _controller = new EmailController();
    }

    [Fact]
    public async Task SendMail_ShouldReturnOk_WhenUseCaseSucceeds()
    {
        // Arrange
        var request = new RequestSendMail(
            recipients: new List<string> { "test@example.com" },
            copyRecipients: new List<string>(),
            hiddenRecipients: new List<string>(),
            subject: "Test Subject",
            body: "Test Body"
        );

        var result = new Result<MessageResult>();
        result.Succeeded(new MessageResult("Email sent successfully"));

        _mockSendMailUseCase
            .Setup(x => x.SendAsync(request))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.SendMail(_mockSendMailUseCase.Object, request) as ObjectResult;

        // Assert
        var responseData = Assert.IsType<Result<MessageResult>>(response.Value);

        Assert.True(responseData.Success);
        Assert.Equal("Email sent successfully", responseData.Data.Message);
    }

    [Fact]
    public async Task SendMail_ShouldReturnUnprocessableEntity_WhenUseCaseFails()
    {
        // Arrange
        var request = new RequestSendMail(
            recipients: new List<string> { "test@example.com" },
            copyRecipients: new List<string>(),
            hiddenRecipients: new List<string>(),
            subject: "Test Subject",
            body: "Test Body"
        );

        var result = new Result<MessageResult>();
        result.Failure(new List<string> { "Error sending email" });

        _mockSendMailUseCase
            .Setup(x => x.SendAsync(request))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.SendMail(_mockSendMailUseCase.Object, request) as ObjectResult;

        // Assert
        var responseData = Assert.IsType<Result<MessageResult>>(response.Value);

        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.False(responseData.Success);
        Assert.Contains("Error sending email", responseData.Errors);
    }
}
