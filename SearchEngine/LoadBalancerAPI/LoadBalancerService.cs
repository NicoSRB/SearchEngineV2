using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public class LoadBalancerService
{
    private readonly List<string> _servers;
    private int _currentServerIndex;

    public LoadBalancerService(List<string> servers)
    {
        _servers = servers;
        _currentServerIndex = 0;
    }

    // This method selects the next server using round-robin
    public string GetNextServer()
    {
        var server = _servers[_currentServerIndex];
        _currentServerIndex = (_currentServerIndex + 1) % _servers.Count;
        return server;
    }

    // This method forwards the request to the selected server
    public async Task<HttpResponseMessage> ForwardRequestAsync(HttpRequestMessage request)
    {
        var server = GetNextServer();
        var client = new HttpClient();
        request.RequestUri = new Uri($"http://{server}{request.RequestUri.PathAndQuery}");
        return await client.SendAsync(request);
    }
}
