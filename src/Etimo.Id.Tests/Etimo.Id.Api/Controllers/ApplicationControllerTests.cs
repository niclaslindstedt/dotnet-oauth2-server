using System;
using System.Threading.Tasks;
using Shouldly;
using Moq;
using Xunit;
using System.Net;
using Etimo.Id.Abstractions;
using Etimo.Id.Tests.TestHelpers;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Etimo.Id.Entities;

namespace Etimo.Id.Tests.Etimo.Id.Api.Controllers
{
    public class ApplicationControllerTests
    {
        private readonly Mock<IGetApplicationsService> _getApplicationServiceMock;
        public ApplicationControllerTests()
        {
            _getApplicationServiceMock = new();
        }

        [Fact]
        public async Task GetApplications_WithReadPermissions_Returns_Ok()
        {
            // Arrange
            _getApplicationServiceMock
                .Setup(service => service
                .GetByUserIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(new List<Application>()));

            var server = TestHelper.CreateTestServer(services =>
            {
                services.AddTransient<IGetApplicationsService>(_ => _getApplicationServiceMock.Object);
            });

            var client = server.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await TestHelper.GetReadJwt}");

            // Act
            var response = await client.GetAsync("/applications");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetApplications_With_No_Provided_Auth_Header_Returns_Unauthorized() {
            // Act
            var server = TestHelper.CreateTestServer();
            var client = server.CreateClient();
            var response = await client.GetAsync("/applications");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetApplications_With_Invalid_Auth_Header_Returns_Unauthorized()
        {
            // Arrange
            var server = TestHelper.CreateTestServer();
            var client = server.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer dummyKey");

            // Act
            var response = await client.GetAsync("/applications");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        }
    }
}
