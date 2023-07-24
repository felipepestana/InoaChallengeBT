using System.Text.Json;
using System.Text.Json.Nodes;

namespace Inoa
{
    public class QueryService 
    {
        private string BaseAPIUrl = "https://brapi.dev";
        private string QueryPath = "/api/quote/{0}";
        private HttpClient Client;

        public QueryService()
        {
            Client = new HttpClient()
            {
                BaseAddress = new Uri(BaseAPIUrl),
            };
        }

        public async Task<decimal> QueryAsset(Stock asset)
        {
            string fullQueryUrl = BaseAPIUrl + String.Format(QueryPath, asset.Name);
            HttpResponseMessage response = await Client.GetAsync(fullQueryUrl);
            response.EnsureSuccessStatusCode();

            string body = await response.Content.ReadAsStringAsync();
            if(String.IsNullOrWhiteSpace(body))
                throw new Exception($"Empty response returned when querying asset {asset.Name}");

            return ExtractAmount(body); 
        }

        private decimal ExtractAmount(string body)
        {
            JsonNode jsonBody = JsonNode.Parse(body) ?? throw new JsonException("Invalid response received");

            return jsonBody!["results"]![0]!["regularMarketPrice"]!.GetValue<decimal>();
        }
    }
}