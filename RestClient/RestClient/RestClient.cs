using System.Net.Http.Headers;
using System.Text.Json;

namespace RestClient;
public class RestClient
{
    private static readonly HttpClient _client;
    public string EndPoint { get; set; }
    public HttpMethod Method { get; set; }
    public string ContentType { get; set; } = "application/json";
    public string PostData { get; set; }
    public IDictionary<string, string> RequestHeaders { get; set; }
    public IDictionary<string, string> ContentHeaders { get; set; }

    static RestClient()
    {
        _client = new HttpClient();
    }

    public RestClient(string endpoint, HttpMethod method, IDictionary<string, string> requestHeaders = null, IDictionary<string, string> contentHeaders = null)
    {
        EndPoint = endpoint;
        Method = method;
        PostData = "";
        RequestHeaders = requestHeaders;
        ContentHeaders = contentHeaders;
    }

    public HttpResponseMessage SendHttpRequest(string parameters = "")
    {
        HttpResponseMessage response = _client.SendAsync(CreateHttpRequest(EndPoint + parameters)).Result;
        return response;
    }

    public string ReadAsString(HttpResponseMessage message)
    {
        return message.Content.ReadAsStringAsync().Result;
    }

    public async Task<string> ReadAsStringAsync(HttpResponseMessage message)
    {
        return await message.Content.ReadAsStringAsync();
    }

    public async Task<HttpResponseMessage> SendHttpRequestAsync(string parameters = "")
    {
        HttpResponseMessage response = await _client.SendAsync(CreateHttpRequest(EndPoint + parameters)).ConfigureAwait(false);
        return response;
    }

    private HttpRequestMessage CreateHttpRequest(string url)
    {
        HttpRequestMessage request = new HttpRequestMessage();
        request.RequestUri = new Uri(url);
        request.Method = Method;

        if (RequestHeaders != null && RequestHeaders.Any())
        {
            foreach (var header in RequestHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        if (!string.IsNullOrEmpty(PostData))
        {
            request.Content = new StringContent(PostData);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
        }

        if (ContentHeaders != null && ContentHeaders.Any())
        {
            foreach (var header in ContentHeaders)
            {
                request.Content.Headers.Add(header.Key, header.Value);
            }
        }

        return request;

    }

}

public class RestClient<TRequestBody> : RestClient
{
    public RestClient(string endpoint, HttpMethod method, TRequestBody postData, IDictionary<string, string> requestHeaders = null, IDictionary<string, string> contentHeaders = null)
        : base(endpoint, method, requestHeaders: requestHeaders, contentHeaders: contentHeaders)
    {
        PostData = JsonSerializer.Serialize(postData);
    }

    public RestClient(string endpoint, HttpMethod method, TRequestBody postData, string contentType, IDictionary<string, string> requestHeaders = null, IDictionary<string, string> contentHeaders = null)
        : base(endpoint, method, requestHeaders: requestHeaders, contentHeaders: contentHeaders)
    {
        ContentType = contentType;
        PostData = JsonSerializer.Serialize(postData);
    }
}

