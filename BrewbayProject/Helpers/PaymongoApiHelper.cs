namespace BrewbayProject.Helpers;

public class PaymongoApiHelper
{
    public static HttpClient GetHttpClient(IConfiguration config)
    {
        var client = new HttpClient();
        var secretKey = config.GetValue<String>("PaymongoSecretKey");

        client.BaseAddress = new Uri("https://api.paymongo.com/v1/");
        client.DefaultRequestHeaders.Add("Accept", "applicaiton/json");
        client.DefaultRequestHeaders.Add("Authorization", $"Basic c2tfdGVzdF9Ma2IxWGdxZVdKVXFDbzNCeVdONzM1a2k=");

        return client;
    }
}