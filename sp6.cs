
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        string tenantId = "YOUR_TENANT_ID";
        string clientId = "YOUR_CLIENT_ID";
        string clientSecret = "YOUR_CLIENT_SECRET";

        // Step 1: Get Access Token
        string tokenEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";

        using (HttpClient client = new HttpClient())
        {
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);

            tokenRequest.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("scope", "https://graph.microsoft.com/.default"),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var tokenResponse = await client.SendAsync(tokenRequest);
            var tokenResult = await tokenResponse.Content.ReadAsStringAsync();

            var json = JsonDocument.Parse(tokenResult);
            string accessToken = json.RootElement.GetProperty("access_token").GetString();

            Console.WriteLine("Access Token Acquired\n");

            // Step 2: Call Graph API for SharePoint List

            string graphEndpoint =
                "https://graph.microsoft.com/v1.0/sites/inspirewellness.sharepoint.com:/sites/FRNewConcepts:/lists/FR New Concept Request/items?$top=5";

            var graphRequest = new HttpRequestMessage(HttpMethod.Get, graphEndpoint);
            graphRequest.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var graphResponse = await client.SendAsync(graphRequest);

            Console.WriteLine("Status: " + graphResponse.StatusCode);

            string result = await graphResponse.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }
    }
}
