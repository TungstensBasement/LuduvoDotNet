using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Drawing;
using System.Text;
using System.Text.Json;
using LuduvoDotNet.Records;

namespace LuduvoDotNet.Tests
{
    public class UserApiTests
    {
        [Fact]
        public async Task UpdateMyProfileAsync_Should_SendPutRequestWithExpectedBody()
        {
            var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.NoContent));
            var luduvo = new Luduvo(new HttpClient(handler) { BaseAddress = new Uri("https://api.luduvo.com") });

            var request = new UpdateMyProfileRequest
            {
                Status = "Online",
                Bio = "Hello there",
                DisplayName = "Igor",
                AccentColor = Color.FromArgb(255, 170, 187, 204),
                AllowJoins = true,
                AdditionalProperties = new Dictionary<string, JsonElement>
                {
                    ["custom_field"] = JsonSerializer.SerializeToElement("custom")
                }
            };

            await luduvo.UpdateMyProfileAsync(request);

            Assert.NotNull(handler.LastRequest);
            Assert.Equal(HttpMethod.Put, handler.LastRequest!.Method);
            Assert.Equal("/me/profile", handler.LastRequest.RequestUri!.PathAndQuery);

            var body = await handler.LastRequest.Content!.ReadAsStringAsync();
            Assert.Contains("\"status\":\"Online\"", body);
            Assert.Contains("\"bio\":\"Hello there\"", body);
            Assert.Contains("\"display_name\":\"Igor\"", body);
            Assert.Contains("\"accent_color\":\"#AABBCC\"", body);
            Assert.Contains("\"allow_joins\":true", body);
            Assert.Contains("\"custom_field\":\"custom\"", body);
        }

        [Fact]
        public async Task UpdateMyProfileAsync_Should_ThrowTooManyRequestsException_WhenApiReturns429()
        {
            var httpClient = new HttpClient(new StaticResponseHandler(new HttpResponseMessage(HttpStatusCode.TooManyRequests)))
            {
                BaseAddress = new Uri("https://api.luduvo.com")
            };

            var luduvo = new Luduvo(httpClient);

            await Assert.ThrowsAsync<TooManyRequestsException>(() => luduvo.UpdateMyProfileAsync(new UpdateMyProfileRequest()));
        }

        [Fact]
        public async Task UpdateMyProfileAsync_Should_ThrowArgumentNullException_WhenRequestIsNull()
        {
            var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.NoContent));
            var luduvo = new Luduvo(new HttpClient(handler) { BaseAddress = new Uri("https://api.luduvo.com") });

            await Assert.ThrowsAsync<ArgumentNullException>(() => luduvo.UpdateMyProfileAsync(null!));
        }

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

        [Fact]
        public async Task GetUserInventotoryAsync_Should_HandlePagedObjectResponse()
        {
            var json = """
                       {
                         "items": [
                           {
                             "id": 1,
                             "item_id": 14,
                             "thumbnail_url": "",
                             "price": 0,
                             "category_id": 4,
                             "category_name": "Tops",
                             "category_slug": "tops",
                             "acquired_at": 1776715216
                           }
                         ],
                         "total": 1,
                         "limit": 20,
                         "offset": 0
                       }
                       """;

            var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

            var luduvo = new Luduvo(new HttpClient(handler) { BaseAddress = new Uri("https://api.luduvo.com") });
            var items = await luduvo.GetUserInventotoryAsync(2, limit: 20, offset: 0);

            Assert.Single(items);
            Assert.Equal(14u, items[0].ItemId);
        }

        [Fact]
        public async Task GetUserInventotoryAsync_Should_IncludeLimitAndOffsetInQuery()
        {
            var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"items\":[]}", Encoding.UTF8, "application/json")
            });

            var luduvo = new Luduvo(new HttpClient(handler) { BaseAddress = new Uri("https://api.luduvo.com") });
            await luduvo.GetUserInventotoryAsync(2, limit: 10, offset: 30);

            Assert.NotNull(handler.LastRequest);
            Assert.Equal("/users/2/inventory?limit=10&offset=30", handler.LastRequest!.RequestUri!.PathAndQuery);
        }

        [Fact]
        public async Task GetUserInventotoryAsync_Should_ThrowUserNotFoundException_WhenApiReturns404()
        {
            var httpClient = new HttpClient(new StaticResponseHandler(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = JsonContent.Create(new { error = "user not found" })
            }))
            {
                BaseAddress = new Uri("https://api.luduvo.com")
            };

            var luduvo = new Luduvo(httpClient);

            await Assert.ThrowsAsync<UserNotFoundException>(() => luduvo.GetUserInventotoryAsync(0, 10, 0));
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, 0)]
        [InlineData(10, -1)]
        public async Task GetUserInventotoryAsync_Should_ValidateLimitAndOffset(int limit, int offset)
        {
            var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"items\":[]}", Encoding.UTF8, "application/json")
            });

            var luduvo = new Luduvo(new HttpClient(handler) { BaseAddress = new Uri("https://api.luduvo.com") });

            await Assert.ThrowsAnyAsync<ArgumentOutOfRangeException>(() => luduvo.GetUserInventotoryAsync(2, limit, offset));
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
