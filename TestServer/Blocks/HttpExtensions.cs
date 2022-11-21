using System.Text;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;

namespace TestServer.Blocks
{
    public static class HttpExtensions
    {
        public static async Task<T?> ReadFromJsonAsync<T>(this HttpContent content, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = await content.ReadAsStringAsync(cancellationToken);

                if (typeof(T) == typeof(JToken))
                {
                    return (T)(object)JToken.Parse(json);
                }
                else
                {
                    return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings()
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });
                }
            }
            catch (JsonSerializationException)
            {
                return default;
            }
            catch (JsonReaderException)
            {
                return default;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public static AndConstraint<HttpResponseMessageAssertions> BeValidSchema(this HttpResponseMessageAssertions httpAssertions, JSchema jschema)
        {
            var httpResponse = httpAssertions.Subject;

            var content = httpResponse.Content.ReadFromJsonAsync<JToken>().Result;

            var valid = content.IsValid(jschema, out IList<string> errors);

            Execute.Assertion
                .ForCondition(valid).BecauseOf("", null)
                .FailWith("Schema validation failed with errors: {0}", string.Join(", ", errors));

            return new AndConstraint<HttpResponseMessageAssertions>(httpAssertions);
        }


        public static Task<HttpResponseMessage> DeleteAsync(this HttpClient httpClient, string requestUri, Dictionary<string, string> headers)
        {
            return httpClient.SendAsync(HttpMethod.Delete, requestUri, null, headers);
        }

        public static Task<HttpResponseMessage> PutAsync(this HttpClient httpClient, string requestUri, object content, Dictionary<string, string> headers)
        {
            return httpClient.SendAsync(HttpMethod.Put, requestUri, content, headers);
        }

        public static Task<HttpResponseMessage> PostAsync(this HttpClient httpClient, string requestUri, object content, Dictionary<string, string> headers)
        {
            return httpClient.SendAsync(HttpMethod.Post, requestUri, content, headers);
        }

        public static Task<HttpResponseMessage> GetAsync(this HttpClient httpClient, string requestUri, Dictionary<string, string?>? parameters, Dictionary<string, string> headers)
        {
            if (parameters != null)
            {
                var separator = requestUri.Contains("?") ? "&" : "?";
                var queryString = parameters.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}");

                requestUri = $"{requestUri}{separator}{string.Join("&", queryString)}";
            }

            return httpClient.SendAsync(HttpMethod.Get, requestUri, null, headers);
        }

        private static async Task<HttpResponseMessage> SendAsync(this HttpClient httpClient, HttpMethod method,
            string requestUri, object? content = null, Dictionary<string, string>? headers = null)
        {
            using (var request = new HttpRequestMessage(method, requestUri))
            {
                if (content != null)
                {
                    var json = JsonConvert.SerializeObject(content);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                return await httpClient.SendAsync(request);
            }
        }
    }
}
