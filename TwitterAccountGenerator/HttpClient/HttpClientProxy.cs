using System.Net;

class HttpClientProxy
{
    public WebProxy wProxy;

    public HttpClientProxy()
    {
        wProxy = new WebProxy();
    }

    public HttpClientProxy(string _ip, int _port)
    {
        wProxy = new WebProxy(_ip, _port);
    }
    
    public void SetProxy(string _ip, int _port)
    {
        wProxy = new WebProxy(_ip, _port);
    }

    public void ClearProxy()
    {
        wProxy = null;
    }
}
