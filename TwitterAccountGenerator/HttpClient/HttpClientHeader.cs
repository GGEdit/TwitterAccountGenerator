using System.Collections.Generic;

class HttpClientHeader
{
    public Dictionary<string, string> headersKeyValuePairs;
    public Dictionary<string, string> postKeyValuePairs;
    
    public string MediaType = "";
    public string TransferEncoding = "";
    public string Connection = "";
    public string ContentType = "";
    public string Accept = "";
    public string Referer = "";
    public string UserAgent = "";
    public string Expect = "";

    public HttpClientHeader()
    {
        headersKeyValuePairs = new Dictionary<string, string>();
        postKeyValuePairs = new Dictionary<string, string>();
    }

    //Header
    public void AddHeader(string _key, string _value)
    {
        headersKeyValuePairs.Add(_key, _value);
    }

    public void AddHeader(Dictionary<string, string> _headersKeyValuePairs)
    {
        if (_headersKeyValuePairs != null)
            foreach (var data in _headersKeyValuePairs)
                headersKeyValuePairs.Add(data.Key, data.Value);
    }

    public void SetHeader(Dictionary<string, string> _headersKeyValuePairs)
    {
        if (_headersKeyValuePairs != null)
            headersKeyValuePairs = new Dictionary<string, string>(_headersKeyValuePairs);
    }

    //Parameter
    public void AddParam(Dictionary<string, string> _postKeyValuePairs)
    {
        foreach (var data in _postKeyValuePairs)
            postKeyValuePairs.Add(data.Key, data.Value);
    }

    public void AddParam(string _key, string _value)
    {
        postKeyValuePairs.Add(_key, _value);
    }

    public void SetParam(Dictionary<string, string> _postKeyValuePairs)
    {
        if (_postKeyValuePairs == null)
            return;

        postKeyValuePairs = new Dictionary<string, string>(_postKeyValuePairs);
    }
}
