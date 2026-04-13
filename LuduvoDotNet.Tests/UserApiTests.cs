using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;

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
            Assert.NotNull(result.DisplayName);
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

        [Fact]
        public async Task SearchUsersAsync_Should_HandleArrayResponse()
        {
            var json = """
                       [
                         {
                           "id": 2,
                           "username": "IgorAlexey",
                           "head_color": "#C8C8C8",
                           "torso_color": "#C8C8C8",
                           "display_name": "IgorAlexey",
                           "role": "player",
                           "created_at": 1768055400
                         }
                       ]
                       """;

            var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

            var luduvo = new Luduvo(new HttpClient(handler) { BaseAddress = new Uri("https://api.luduvo.com") });
            var users = await luduvo.SearchUsersAsync("Igor");

            Assert.Single(users);
            Assert.Equal("IgorAlexey", users[0].username);
        }

        [Fact]
        public async Task SearchUsersAsync_Should_HandlePagedObjectResponse()
        {
            var json = """
                       {
                         "users": [
                           {
                             "id": 2,
                             "username": "IgorAlexey",
                             "head_color": "#C8C8C8",
                             "torso_color": "#C8C8C8",
                             "display_name": "IgorAlexey",
                             "role": "player",
                             "created_at": 1768055400
                           }
                         ],
                         "total": 100,
                         "limit": 20,
                         "offset": 0
                       }
                       """;

            var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

            var luduvo = new Luduvo(new HttpClient(handler) { BaseAddress = new Uri("https://api.luduvo.com") });
            var users = await luduvo.SearchUsersAsync(string.Empty, limit: 20, offset: 0);

            Assert.Single(users);
            Assert.Equal("IgorAlexey", users[0].username);
        }

        [Fact]
        public async Task SearchUsersAsync_Should_IncludeLimitAndOffsetInQuery()
        {
            var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]", Encoding.UTF8, "application/json")
            });

            var luduvo = new Luduvo(new HttpClient(handler) { BaseAddress = new Uri("https://api.luduvo.com") });
            await luduvo.SearchUsersAsync("abc", limit: 10, offset: 30);

            Assert.NotNull(handler.LastRequest);
            Assert.Equal("/users?q=abc&limit=10&offset=30", handler.LastRequest!.RequestUri!.PathAndQuery);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, 0)]
        [InlineData(10, -1)]
        public async Task SearchUsersAsync_Should_ValidateLimitAndOffset(int limit, int offset)
        {
            var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]", Encoding.UTF8, "application/json")
            });

            var luduvo = new Luduvo(new HttpClient(handler) { BaseAddress = new Uri("https://api.luduvo.com") });

            await Assert.ThrowsAnyAsync<ArgumentOutOfRangeException>(() => luduvo.SearchUsersAsync("abc", limit, offset));
        }

        private sealed class StaticResponseHandler(HttpResponseMessage response) : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => Task.FromResult(response);
        }

        private sealed class RecordingHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
        {
            public HttpRequestMessage? LastRequest { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                return Task.FromResult(responder(request));
            }
        }
    }
}
