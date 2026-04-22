using System.Net;
using System.Net.Http;
using System.Text;

namespace LuduvoDotNet.Tests;

public class PlacesApiTests
{
    [Fact]
    public async Task SearchPlacesAsync_Should_HandlePagedObjectResponse()
    {
        var json = """
                   {
                     "places": [
                       {
                         "id": 2,
                         "owner_id": 3,
                         "owner_username": "Isaac",
                         "title": "Test2",
                         "description": "Welcome to my place!",
                         "access": "public",
                         "max_players": 24,
                         "visit_count": 0,
                         "thumbs_up": 0,
                         "thumbs_down": 0,
                         "active_players": 0,
                         "created_at": 1775619390,
                         "updated_at": 1775619390,
                         "thumbnail_url": ""
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
        var places = await luduvo.SearchPlacesAsync("test", limit: 20, offset: 0);

        Assert.Single(places);
        Assert.Equal("Test2", places[0].Title);
    }

    [Fact]
    public async Task SearchPlacesAsync_Should_IncludeLimitAndOffsetInQuery()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("[]", Encoding.UTF8, "application/json")
        });

        var luduvo = new Luduvo(new HttpClient(handler) { BaseAddress = new Uri("https://api.luduvo.com") });
        await luduvo.SearchPlacesAsync("abc", limit: 10, offset: 30);

        Assert.NotNull(handler.LastRequest);
        Assert.Equal("/places?q=abc&limit=10&offset=30", handler.LastRequest!.RequestUri!.PathAndQuery);
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

