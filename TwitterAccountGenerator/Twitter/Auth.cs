using System;
using System.Text.RegularExpressions;

class Auth
{
    private HttpClient client
    {
        get; set;
    }

    private HttpClientHeader clientHeader
    {
        get; set;
    }

    private HttpClientCookie clientCookie
    {
        get; set;
    }

    private string Email
    {
        get; set;
    }

    private string Password
    {
        get; set;
    }

    private string Name
    {
        get; set;
    }

    public string GuestToken
    {
        get; set;
    }

    public string GuestId
    {
        get; set;
    }

    public string PersonalizationId
    {
        get; set;
    }

    public string FlowToken
    {
        get; set;
    }

    public string TwitterSession
    {
        get; set;
    }

    public Auth(string _emailAddress, string _passWord, string _name)
    {
        this.Email = _emailAddress;
        this.Password = _passWord;
        this.Name = _name;

        client = new HttpClient();
        clientHeader = new HttpClientHeader();
        clientCookie = new HttpClientCookie();
    }
    private void Initialize()
    {
        //Header
        clientHeader.Accept = "*/*";
        clientHeader.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";
        clientHeader.ContentType = "application/json";
        clientHeader.Referer = "https://twitter.com/i/flow/signup";
        clientHeader.Connection = "keep-alive";
        clientHeader.AddHeader("Accept-Encoding", "gzip, deflate, br");
        clientHeader.AddHeader("Accept-Language", "ja,en-US;q=0.9,en;q=0.8");
        clientHeader.AddHeader("x-guest-token", this.GuestToken);
        clientHeader.AddHeader("authorization", "Bearer AAAAAAAAAAAAAAAAAAAAANRILgAAAAAAnNwIzUejRCOuH5E6I8xnZz4puTs%3D1Zv7ttfk8LF81IUq16cHjhLTvJu4FA33AGWWjCpTnA");
        clientHeader.AddHeader("x-twitter-active-user", "yes");
        clientHeader.AddHeader("x-twitter-client-language", "ja");
        clientHeader.AddHeader("Origin", "https://twitter.com");

        //Cookie
        clientCookie.AddCookie(new Uri("https://api.twitter.com"), "guest_id", this.GuestId);
        clientCookie.AddCookie(new Uri("https://api.twitter.com"), "personalization_id", this.PersonalizationId);
    }

    public bool GetGuestToken()
    {
        client.RequestUri = Urls.Flow_Signup;
        clientHeader.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
        var html = client.Get(clientHeader);
        var status = client.GetResponseStatusCode();
        if (status == System.Net.HttpStatusCode.OK)
        {
            this.GuestToken = Regex.Match(html, "gt=(.*?);").Groups[1].Value;

            //Setting Cookie
            var cookie = clientCookie.GetResponseCookie(client);
            GuestId = Regex.Match(cookie, "guest_id=(.*?);").Groups[1].Value;
            PersonalizationId = Regex.Match(cookie, "personalization_id=(.*?);").Groups[1].Value;

            return true;
        }
        Console.WriteLine($"Error:{status}");
        return false;
    }

    public bool GetFlowToken()
    {
        Initialize();

        client.RequestUri = Urls.Onboarding_Task + "?flow_name=signup";
        var p_Json = "{\"input_flow_data\":{\"flow_context\":{\"debug_overrides\":{},\"start_location\":{\"location\":\"manual_link\"}}}}";
        var html = client.Post(clientHeader, clientCookie, null, p_Json);
        var status = client.GetResponseStatusCode();
        if (status == System.Net.HttpStatusCode.OK)
        {
            this.FlowToken = Regex.Match(html, "\"flow_token\":\"(.*?)\",").Groups[1].Value;
            return true;
        }
        Console.WriteLine($"Error:{status}");
        return false;
    }

    public bool P13nPreferences()
    {
        client.RequestUri = Urls.Account_Personalization_p13n_preferences;
        client.Get(clientHeader, clientCookie, null);
        var status = client.GetResponseStatusCode();
        if (status == System.Net.HttpStatusCode.OK)
        {
            return true;
        }
        return false;
    }

    public bool SyncOptoutSettings()
    {
        client.RequestUri = Urls.Account_Personalization_sync_optout_settings;
        client.Post(clientHeader, clientCookie, null);
        var status = client.GetResponseStatusCode();
        if (status == System.Net.HttpStatusCode.OK)
        {
            var cookie = clientCookie.GetResponseCookie(client);
            this.TwitterSession = Regex.Match(cookie, "_twitter_sess=(.*?);").Groups[1].Value;
            return true;
        }
        return false;
    }
}
