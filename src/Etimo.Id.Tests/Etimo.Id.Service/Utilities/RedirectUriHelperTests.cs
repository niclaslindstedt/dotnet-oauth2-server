using Etimo.Id.Service.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Etimo.Id.Tests
{
    public class RedirectUriHelperTests
    {
        [Fact]
        public void UriMatches_IdenticalUris_ShouldReturnTrue()
        {
            // Arrange
            var          uri            = "http://localhost/callback";
            List<string> registeredUris = new() { "http://localhost/callback" };

            // Act
            bool result = RedirectUriHelper.UriMatches(uri, registeredUris, false);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void UriMatches_NoUri_ShouldReturnTrue()
        {
            // Arrange
            string       uri            = null;
            List<string> registeredUris = new() { "http://localhost/callback" };

            // Act
            bool result = RedirectUriHelper.UriMatches(uri, registeredUris, true);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void UriMatches_MatchingInList_ShouldReturnTrue()
        {
            // Arrange
            var uri = "http://localhost/callback";
            List<string> registeredUris = new()
            {
                "http://localhost/callback",
                "http://localhost/callback2",
            };

            // Act
            bool result = RedirectUriHelper.UriMatches(uri, registeredUris, false);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void UriMatches_NotMatchingInList_ShouldReturnFalse()
        {
            // Arrange
            var uri = "http://localhost/callback3";
            List<string> registeredUris = new()
            {
                "http://localhost/callback",
                "http://localhost/callback2",
            };

            // Act
            bool result = RedirectUriHelper.UriMatches(uri, registeredUris, false);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UriMatches_QueryAdded_ShouldReturnFalse()
        {
            // Arrange
            var          uri            = "http://localhost/callback?test=hello";
            List<string> registeredUris = new() { "http://localhost/callback" };

            // Act
            bool result = RedirectUriHelper.UriMatches(uri, registeredUris, false);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UriMatches_QueryMatching_ShouldReturnTrue()
        {
            // Arrange
            var          uri            = "http://localhost/callback?test=hello";
            List<string> registeredUris = new() { "http://localhost/callback?test=hello" };

            // Act
            bool result = RedirectUriHelper.UriMatches(uri, registeredUris, false);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void UriMatches_QueryAllowed_ShouldReturnTrue()
        {
            // Arrange
            var          uri            = "http://localhost/callback?test=hello";
            List<string> registeredUris = new() { "http://localhost/callback?" };

            // Act
            bool result = RedirectUriHelper.UriMatches(uri, registeredUris, true);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void UriMatches_QueryAllowedWithLongQuery_ShouldReturnTrue()
        {
            // Arrange
            var          uri            = "http://localhost/callback?test=hello&test2=hello2";
            List<string> registeredUris = new() { "http://localhost/callback?" };

            // Act
            bool result = RedirectUriHelper.UriMatches(uri, registeredUris, true);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void UriMatches_QueryAllowedWithNoQuery_ShouldReturnTrue()
        {
            // Arrange
            var          uri            = "http://localhost/callback?";
            List<string> registeredUris = new() { "http://localhost/callback?" };

            // Act
            bool result = RedirectUriHelper.UriMatches(uri, registeredUris, true);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void UriMatches_QueryAllowedWithNoQueryOrQuestionMark_ShouldReturnTrue()
        {
            // Arrange
            var          uri            = "http://localhost/callback";
            List<string> registeredUris = new() { "http://localhost/callback?" };

            // Act
            bool result = RedirectUriHelper.UriMatches(uri, registeredUris, true);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void UriMatches_QueryAllowedWithAddedParameters_ShouldReturnTrue()
        {
            // Arrange
            var          uri            = "http://localhost/callback?test=hello&test2=hello2";
            List<string> registeredUris = new() { "http://localhost/callback?test=hello&" };

            // Act
            bool result = RedirectUriHelper.UriMatches(uri, registeredUris, true);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void UriMatches_QueryAllowedWithNoAddedParameters_ShouldReturnTrue()
        {
            // Arrange
            var          uri            = "http://localhost/callback?test=hello&";
            List<string> registeredUris = new() { "http://localhost/callback?test=hello&" };

            // Act
            bool result = RedirectUriHelper.UriMatches(uri, registeredUris, true);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void UriMatches_QueryAllowedWithNoAddedParametersOrAmpersand_ShouldReturnTrue()
        {
            // Arrange
            var          uri            = "http://localhost/callback?test=hello";
            List<string> registeredUris = new() { "http://localhost/callback?test=hello&" };

            // Act
            bool result = RedirectUriHelper.UriMatches(uri, registeredUris, true);

            // Assert
            Assert.True(result);
        }
    }
}
