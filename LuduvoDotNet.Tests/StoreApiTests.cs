using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;

namespace LuduvoDotNet.Tests;

public class StoreApiTests
{
    [Fact]
    public async Task SearchStoreAsync_Should_HandlePagedObjectResponse()
    {
        var json = """
                   {
                     "items": [
                       {
                         "id": 14,
                         "creator_id": 6,
                         "creator_username": "mrkbx",
                         "category_id": 4,
                         "category_name": "Tops",
                         "category_slug": "tops",
                         "name": "test",
                         "description": "test",
                         "price": 0,
                         "is_limited": 0,
                         "asset_type": "mesh",
                         "thumbnail_url": "",
                         "sales_count": 1,
                         "thumbs_up": 0,
                         "thumbs_down": 0,
                         "created_at": 1776715216,
                         "owned": false
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
        var items = await luduvo.SearchStoreAsync("test", limit: 20, offset: 0);

        Assert.Single(items);
        Assert.Equal(14u, items[0].Id);
        Assert.Equal("mrkbx", items[0].CreatorUsername);
    }

    [Fact]
    public async Task SearchStoreAsync_Should_IncludeLimitAndOffsetInQuery()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("[]", Encoding.UTF8, "application/json")
        });

        var luduvo = new Luduvo(new HttpClient(handler) { BaseAddress = new Uri("https://api.luduvo.com") });
        await luduvo.SearchStoreAsync("abc", limit: 10, offset: 30);

        Assert.NotNull(handler.LastRequest);
        Assert.Equal("/store?q=abc&limit=10&offset=30", handler.LastRequest!.RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task GetStoreItemByIdAsync_Should_ThrowStoreItemNotFoundException_WhenApiReturns404()
    {
        var httpClient = new HttpClient(new StaticResponseHandler(new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = JsonContent.Create(new { error = "item not found" })
        }))
        {
            BaseAddress = new Uri("https://api.luduvo.com")
        };

        var luduvo = new Luduvo(httpClient);

        await Assert.ThrowsAsync<StoreItemNotFoundException>(() => luduvo.GetStoreItemByIdAsync(1));
    }

    [Fact]
    public async Task GetStoreItemByIdAsync_Should_ReturnItem()
    {
        var json = """
                   {
                     "id": 14,
                     "creator_id": 6,
                     "creator_username": "mrkbx",
                     "category_id": 4,
                     "category_name": "Tops",
                     "category_slug": "tops",
                     "name": "test",
                     "description": "test",
                     "price": 0,
                     "is_limited": 0,
                     "asset_type": "mesh",
                     "thumbnail_url": "",
                     "sales_count": 1,
                     "thumbs_up": 0,
                     "thumbs_down": 0,
                     "created_at": 1776715216,
                     "owned": false,
                     "is_on_sale": 1,
                     "stock": null,
                     "status": "approved",
                     "rig_structural_hash": null,
                     "model_url": "",
                     "updated_at": 1776720619
                   }
                   """;

        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });

        var luduvo = new Luduvo(new HttpClient(handler) { BaseAddress = new Uri("https://api.luduvo.com") });
        var item = await luduvo.GetStoreItemByIdAsync(14);

        Assert.Equal(14u, item.Id);
        Assert.Equal("approved", item.Status);
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

