using System;
using System.Text.RegularExpressions;

class Twitter
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

    private Gmail Gmail
    {
        get; set;
    }

    private Auth Auth
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

    public Twitter(string _emailAddress, string _password, string _name)
    {
        client = new HttpClient();
        clientHeader = new HttpClientHeader();
        clientCookie = new HttpClientCookie();
        this.Email = _emailAddress;
        this.Password = _password;
        this.Name = _name;

        Initialize();
    }

    public Twitter(string _emailAddress, string _password, string _name, Gmail _gmail)
    {
        client = new HttpClient();
        clientHeader = new HttpClientHeader();
        clientCookie = new HttpClientCookie();
        this.Email = _emailAddress;
        this.Password = _password;
        this.Name = _name;

        if (_gmail != null)
            Gmail = _gmail;

        Initialize();
    }

    private bool Initialize()
    {
        Auth = new Auth(this.Email, this.Password, this.Name);
        if (this.Auth.GetGuestToken())
        {
            if (this.Auth.GetFlowToken())
            {
                if (this.Auth.P13nPreferences())
                {
                    if (this.Auth.SyncOptoutSettings())
                    {
                        clientHeader.Accept = "*/*";
                        clientHeader.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";
                        clientHeader.ContentType = "application/json";
                        clientHeader.Referer = "https://twitter.com/i/flow/signup";
                        clientHeader.Connection = "keep-alive";
                        clientHeader.AddHeader("Accept-Encoding", "gzip, deflate, br");
                        clientHeader.AddHeader("Accept-Language", "ja,en-US;q=0.9,en;q=0.8");
                        clientHeader.AddHeader("x-guest-token", this.Auth.GuestToken);
                        clientHeader.AddHeader("authorization", "Bearer AAAAAAAAAAAAAAAAAAAAANRILgAAAAAAnNwIzUejRCOuH5E6I8xnZz4puTs%3D1Zv7ttfk8LF81IUq16cHjhLTvJu4FA33AGWWjCpTnA");
                        clientHeader.AddHeader("x-twitter-active-user", "yes");
                        clientHeader.AddHeader("x-twitter-client-language", "ja");
                        clientHeader.AddHeader("Origin", "https://twitter.com");

                        clientCookie.AddCookie(new Uri("https://api.twitter.com"), "guest_id", this.Auth.GuestId);
                        clientCookie.AddCookie(new Uri("https://api.twitter.com"), "personalization_id", this.Auth.PersonalizationId);
                        clientCookie.AddCookie(new Uri("https://api.twitter.com"), "_twitter_sess", this.Auth.TwitterSession);

                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void EmailVerification()
    {
        client.RequestUri = Urls.Onboarding_Begin_Verification;
        var p_Json = $"{{\"email\":\"{this.Email}\",\"display_name\":\"{this.Name}\",\"flow_token\":\"{this.Auth.FlowToken}\"}}";
        var html = client.Post(clientHeader, clientCookie, null, p_Json);
    }

    public string GetPinCode()
    {
        if (this.Gmail == null)
            return "[Gmail]インスタンスが初期化されていません。";

        string mail = this.Gmail.ReceiveTopUnseenMessage("verify@twitter.com");
        if (mail == "NotFound")
            return GetPinCode();

        return mail.Split('\n')[8];
    }

    public bool EmailTask(string _code)
    {
        client.RequestUri = Urls.Onboarding_Task;
        var p_Json = $"{{\"flow_token\":\"{this.Auth.FlowToken}\",\"subtask_inputs\":[{{\"subtask_id\":\"GenerateTemporaryPassword\",\"fetch_temporary_password\":{{\"password\":\"{this.Password}\",\"link\":\"next_link\"}}}},{{\"subtask_id\":\"Signup\",\"sign_up\":{{\"email\":\"{this.Email}\",\"link\":\"next_link\",\"name\":\"{this.Name}\"}}}},{{\"subtask_id\":\"SignupReview\",\"sign_up_review\":{{\"link\":\"signup_with_email_next_link\"}}}},{{\"subtask_id\":\"EmailVerification\",\"email_verification\":{{\"code\":\"{_code}\",\"email\":\"{this.Email}\",\"link\":\"next_link\"}}}}]}}";
        var html = client.Post(clientHeader, clientCookie, null, p_Json);
        var status = client.GetResponseStatusCode();
        if (status == System.Net.HttpStatusCode.OK)
        {
            var result = Regex.Match(html, "\"status\":\"(.*?)\",").Groups[1].Value;
            if (result == "success")
            {
                //Renew FlowToken
                this.Auth.FlowToken = Regex.Match(html, "\"flow_token\":\"(.*?)\",").Groups[1].Value;
                //Add Cookie
                var authToken = Regex.Match(html, "\"auth_token\":\"(.*?)\",").Groups[1].Value;
                clientCookie.AddCookie(new Uri("https://api.twitter.com"), "auth_token", authToken);
                return true;
            }
            else
                Console.WriteLine($"アカウント作成に失敗しました:{result}");
        }
        else
            Console.WriteLine($"NetworkError:{status}");

        return false;
    }

    public TwitterUser VerifyCredentials()
    {
        TwitterUser user = new TwitterUser();
        client.RequestUri = Urls.Account_Verify_Credentials + "?skip_status=1";
        var html = client.Get(clientHeader, clientCookie, null);
        var status = client.GetResponseStatusCode();
        if (status == System.Net.HttpStatusCode.OK)
        {
            //Return TwitterUser
            user.Email = Regex.Match(html, "\"address\":\"(.*?)\",").Groups[1].Value;
            user.Name = Regex.Match(html, "\"name\":\"(.*?)\",").Groups[1].Value;
            user.ScreenName = Regex.Match(html, "\"screen_name\":\"(.*?)\",").Groups[1].Value;
            user.Password = this.Password;
            user.UserId = Regex.Match(html, "\"id_str\":\"(.*?)\",").Groups[1].Value;
            user.CreatedTime = DateTime.Now;
            user.Suspended = Regex.Match(html, "\"suspended\":(.*?),").Groups[1].Value;
        }
        else
            Console.WriteLine($"NetworkError:{status}");

        return user;
    }
}