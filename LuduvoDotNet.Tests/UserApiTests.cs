using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace LuduvoDotNet.Tests
{
    public class UserApiTests
    {
        [Fact]
        public async Task GetUserByIdAsync_Should_ReturnCorrectData()
        {
            var luduvo = new Luduvo();
            var result = await luduvo.GetUserByIdAsync(2);
            Assert.NotNull(result);
            Assert.Equal(2u, result.UserId);
            Assert.Equal("IgorAlexey", result.Username);
            Assert.Equal(1768055400ul, result.MemberSince);
        }

        [Fact]
        public async Task GetUserByIdAsync_Should_ThrowUserNotFoundException_WhenApiReturns404()
        {
            var httpClient = new HttpClient(new StaticResponseHandler(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = JsonContent.Create(new { error = "profile not found" })
            }))
            {
                BaseAddress = new Uri("https://api.luduvo.com")
            };

            var luduvo = new Luduvo(httpClient);

            await Assert.ThrowsAsync<UserNotFoundException>(() => luduvo.GetUserByIdAsync(0));
        }

        private sealed class StaticResponseHandler(HttpResponseMessage response) : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => Task.FromResult(response);
        }
    }
}
