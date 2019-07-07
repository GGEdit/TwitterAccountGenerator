using System;
using System.Text.RegularExpressions;
class TwitterAPI
{
    static HttpClient client = new HttpClient();
    static HttpClientHeader clientHeader = new HttpClientHeader();

    public static bool CheckEmailAvailable(string _emailAddress)
    {
        client.RequestUri = $"https://api.twitter.com/i/users/email_available.json?email={_emailAddress}";
        clientHeader.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
        var json = client.Get(clientHeader);
        var status = client.GetResponseStatusCode();
        if (status == System.Net.HttpStatusCode.OK)
        {
            var result = Regex.Match(json, "\"taken\":(.*?)}").Groups[1].Value;
            if (result == "false")
                return true;
            else if (result == "true")
                return false;
            else
                return false;
        }
        Console.WriteLine($"NetworkError:{status}");
        return false;
    }
}
