using Etimo.Id.Callback;
using Etimo.Id.Client;
using Etimo.Id.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Etimo.Id.Tests
{
    public class OAuthControllerTests
    {
        private readonly string _code;
        private readonly string _state;

        public OAuthControllerTests()
        {
            _code  = "ABC123";
            _state = "u432hg93h9g32";
        }

        [Fact]
        public async Task CallbackAsync_When_Error_Is_Not_Null_Then_Return_BadRequest()
        {
            // Arrange
            OAuthController controller = CreateOAuthController();

            // Act
            IActionResult result = await controller.CallbackAsync(
                _code,
                _state,
                "an_error_code",
                "Something went bad",
                "http://localhost/error_uri");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CallbackAsync_When_Error_Is_Null_Then_Authorize()
        {
            // Arrange
            var             mock       = new Mock<IEtimoIdClient>();
            OAuthController controller = CreateOAuthController(etimoIdClient: mock);

            // Act
            IActionResult result = await controller.CallbackAsync(
                _code,
                _state,
                null,
                null,
                null);

            // Assert
            mock.Verify(m => m.AuthorizeAsync(It.Is<string>(s => s == _code), It.Is<string>(s => s == _state)), Times.Once);
        }

        [Fact]
        public async Task CallbackAsync_When_Authorize_Fails_Return_Unauthorized()
        {
            // Arrange
            var             mock       = new Mock<IEtimoIdClient>();
            OAuthController controller = CreateOAuthController(etimoIdClient: mock);
            mock.Setup(m => m.AuthorizeAsync(It.Is<string>(s => s == _code), It.Is<string>(s => s == _state))).ReturnsAsync(false);

            // Act
            IActionResult result = await controller.CallbackAsync(
                _code,
                _state,
                null,
                null,
                null);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task CallbackAsync_When_Authorize_Succeeds_Then_Validate_Token()
        {
            // Arrange
            var             mock       = new Mock<IEtimoIdClient>();
            OAuthController controller = CreateOAuthController(etimoIdClient: mock);
            mock.Setup(m => m.AuthorizeAsync(It.Is<string>(s => s == _code), It.Is<string>(s => s == _state))).ReturnsAsync(true);

            // Act
            IActionResult result = await controller.CallbackAsync(
                _code,
                _state,
                null,
                null,
                null);

            // Assert
            mock.Verify(m => m.ValidateAccessTokenAsync(), Times.Once);
        }

        [Fact]
        public async Task CallbackAsync_When_Validation_Succeeds_Then_Return_Access_Token()
        {
            // Arrange
            var             accessToken = new AccessTokenResponseDto { access_token = "bla123" };
            var             mock        = new Mock<IEtimoIdClient>();
            OAuthController controller  = CreateOAuthController(etimoIdClient: mock);
            mock.Setup(m => m.AuthorizeAsync(It.Is<string>(s => s == _code), It.Is<string>(s => s == _state))).ReturnsAsync(true);
            mock.Setup(m => m.ValidateAccessTokenAsync()).ReturnsAsync(true);
            mock.Setup(m => m.GetAccessToken()).Returns(accessToken);

            // Act
            var result = await controller.CallbackAsync(
                _code,
                _state,
                null,
                null,
                null) as OkObjectResult;

            // Assert
            Assert.Equal(accessToken.access_token, (result.Value as AccessTokenResponseDto).access_token);
        }

        private OAuthController CreateOAuthController(Mock<IEtimoIdClient> etimoIdClient = null)
        {
            etimoIdClient ??= new Mock<IEtimoIdClient>();

            var controller = new OAuthController(etimoIdClient.Object);

            return controller;
        }
    }
}
