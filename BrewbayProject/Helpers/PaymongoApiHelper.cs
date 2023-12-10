using System.Text;

namespace BrewbayProject.Helpers;

public class PaymongoApiHelper
{
    public static HttpClient GetHttpClient(IConfiguration config)
    {
        var client = new HttpClient();
        var secretKey = config.GetValue<String>("PaymongoSecretKey");
        var encodedKey = EncodeToBase64(secretKey!);

        client.BaseAddress = new Uri("https://api.paymongo.com/v1/");
        client.DefaultRequestHeaders.Add("Accept", "applicaiton/json");
        client.DefaultRequestHeaders.Add("Authorization", $"Basic {encodedKey}");

        return client;
    }

    private static string EncodeToBase64(string key)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
    }
}