using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;

class HttpClient
{
    public HttpWebRequest request;
    public HttpWebResponse response;
    private Stream stream;
    private StreamReader reader;
    private Image img;
    public string RequestUri;

    public HttpClient()
    {
    }

    private HttpWebRequest CreateInstance(string _requestUri, string _method, HttpClientHeader _clientHeader = null, HttpClientCookie _clientCookie = null, HttpClientProxy _clientProxy = null)
    {
        HttpWebRequest httpWebRequest;
        httpWebRequest = (HttpWebRequest)WebRequest.Create(_requestUri);
        httpWebRequest.Method = _method;
        httpWebRequest.Timeout = 5000;

        if (_clientHeader != null)
        {
            httpWebRequest.Accept = _clientHeader.Accept;
            httpWebRequest.ContentType = _clientHeader.ContentType;
            httpWebRequest.Referer = _clientHeader.Referer;
            httpWebRequest.UserAgent = _clientHeader.UserAgent;
            httpWebRequest.Expect = _clientHeader.Expect;
        }

        if (_clientProxy != null)
            httpWebRequest.Proxy = _clientProxy.wProxy;

        if (_clientCookie != null && _clientCookie.cookieContainer != null)
            httpWebRequest.CookieContainer = _clientCookie.cookieContainer;

        if (_clientHeader.headersKeyValuePairs != null)
            foreach (var pair in _clientHeader.headersKeyValuePairs)
                httpWebRequest.Headers.Add(pair.Key, pair.Value);

        return httpWebRequest;
    }

    private byte[] GetParamByte(Dictionary<string, string> _KeyValuePairs)
    {
        byte[] pByte;
        string param = "";
        if (_KeyValuePairs != null)
            foreach (string key in _KeyValuePairs.Keys)
                param += $"{key}={_KeyValuePairs[key]}&";
        pByte = Encoding.ASCII.GetBytes(param);

        return pByte;
    }

    private byte[] GetParamByte(string _json)
    {
        byte[] pByte;
        pByte = Encoding.ASCII.GetBytes(_json);

        return pByte;
    }

    public HttpStatusCode GetResponseStatusCode()
    {
        if (response == null)
            return 0;
        return response.StatusCode;
    }

    public string GetResponseStatusDescription()
    {
        if (response == null)
            return null;

        return response.StatusDescription;
    }

    public string Get(HttpClientHeader _clientHeader = null, HttpClientCookie _clientCookie = null, HttpClientProxy _clientProxy = null)
    {
        if (RequestUri == null)
            return null;

        try
        {
            request = CreateInstance(RequestUri, "GET", _clientHeader, _clientCookie, _clientProxy);
            response = (HttpWebResponse)request.GetResponse();
            stream = response.GetResponseStream();
            reader = new StreamReader(stream, Encoding.GetEncoding("Shift_JIS"));
            return reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"NetworkError:{ex.Message}");
            if (ex.Message.Contains("タイムアウト"))
            {
                if (request != null)
                    request.Abort();

                Console.WriteLine("再試行します..");
                return Get(_clientHeader, _clientCookie, _clientProxy);
            }
            return null;
        }
        finally
        {
            if (request != null)
                request.Abort();
        }
    }

    public Image GetImage(HttpClientHeader _clientHeader = null, HttpClientCookie _clientCookie = null, HttpClientProxy _clientProxy = null)
    {
        if (RequestUri == null)
            return null;

        try
        {
            request = CreateInstance(RequestUri, "GET", _clientHeader, _clientCookie, _clientProxy);
            response = (HttpWebResponse)request.GetResponse();
            img = Image.FromStream(response.GetResponseStream());
            return img;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"NetworkError:{ex.Message}");
            if (ex.Message.Contains("タイムアウト"))
            {
                if (request != null)
                    request.Abort();

                Console.WriteLine("再試行します..");
                return GetImage(_clientHeader, _clientCookie, _clientProxy);
            }
            return null;
        }
        finally
        {
            if (request != null)
                request.Abort();
        }
    }

    public string Post(HttpClientHeader _clientHeader = null, HttpClientCookie _clientCookie = null, HttpClientProxy _clientProxy = null)
    {
        if (RequestUri == null)
            return null;

        try
        {
            request = CreateInstance(RequestUri, "POST", _clientHeader, _clientCookie, _clientProxy);
            if (_clientHeader != null)
            {
                byte[] pByte = GetParamByte(_clientHeader.postKeyValuePairs);
                request.ContentLength = pByte.Length;
                stream = request.GetRequestStream();
                stream.Write(pByte, 0, pByte.Length);
            }
            response = (HttpWebResponse)request.GetResponse();
            stream = response.GetResponseStream();
            reader = new StreamReader(stream, Encoding.GetEncoding("Shift_JIS"));

            return reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"NetworkError:{ex.Message}");
            if (ex.Message.Contains("タイムアウト"))
            {
                if (request != null)
                    request.Abort();

                Console.WriteLine("再試行します..");
                return Post(_clientHeader, _clientCookie, _clientProxy);
            }
            return null;
        }
        finally
        {
            if (request != null)
                request.Abort();
        }
    }

    public string Post(HttpClientHeader _clientHeader = null, HttpClientCookie _clientCookie = null, HttpClientProxy _clientProxy = null, string _json = null)
    {
        if (RequestUri == null)
            return null;

        try
        {
            request = CreateInstance(RequestUri, "POST", _clientHeader, _clientCookie, _clientProxy);
            if (_clientHeader != null)
            {
                byte[] pByte = GetParamByte(_json);
                request.ContentLength = pByte.Length;
                stream = request.GetRequestStream();
                stream.Write(pByte, 0, pByte.Length);
            }
            response = (HttpWebResponse)request.GetResponse();
            stream = response.GetResponseStream();
            reader = new StreamReader(stream, Encoding.GetEncoding("Shift_JIS"));

            return reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"NetworkError:{ex.Message}");
            if (ex.Message.Contains("タイムアウト"))
            {
                if (request != null)
                    request.Abort();

                Console.WriteLine("再試行します..");
                return Post(_clientHeader, _clientCookie, _clientProxy, _json);
            }
            return null;
        }
        finally
        {
            if (request != null)
                request.Abort();
        }
    }
}