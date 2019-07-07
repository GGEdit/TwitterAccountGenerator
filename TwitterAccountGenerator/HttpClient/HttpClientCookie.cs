using System;
using System.Collections.Generic;
using System.Net;

class HttpClientCookie
{
    private Cookie cookie;
    public CookieContainer cookieContainer;

    public HttpClientCookie()
    {
        cookieContainer = new CookieContainer();
    }

    public HttpClientCookie(Uri _uri, string _key, string _value)
    {
        cookieContainer = new CookieContainer();
        cookie = new Cookie(_key, _value);
        cookieContainer.Add(_uri, cookie);
    }

    public void AddCookie(Uri _uri, string _key, string _value)
    {
        if (_uri == null)
            return;

        cookie = new Cookie(_key, _value);
        cookieContainer.Add(_uri, cookie);
    }

    public void SetCookie(Uri _uri, string _key, string _value)
    {
        if (_uri == null)
            return;

        cookieContainer = new CookieContainer();
        cookie = new Cookie(_key, _value);
        cookieContainer.Add(_uri, cookie);
    }

    public void SetCookie(Uri _uri, Dictionary<string, string> _cookieKeyValuePairs)
    {
        if (_uri == null || _cookieKeyValuePairs == null)
            return;

        cookieContainer = new CookieContainer();
        foreach (var pair in _cookieKeyValuePairs)
        {
            cookie = new Cookie(pair.Key, pair.Value);
            cookieContainer.Add(_uri, cookie);
        }
    }

    public CookieCollection GetCookies(HttpClient _client, Uri _uri)
    {
        if (_client.request == null || _client.request.CookieContainer == null)
            return null;

        return _client.request.CookieContainer.GetCookies(_uri);
    }

    public string GetResponseCookie(HttpClient _client)
    {
        if (_client.response == null || _client.response.Headers == null)
            return null;

        return _client.response.Headers.Get("Set-Cookie");
    }
}
