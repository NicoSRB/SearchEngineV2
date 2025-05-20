using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace LoadBalancerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoadBalancerController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly List<string> _backendServers;
        private static int _currentServerIndex = 0;
        private readonly ILogger<LoadBalancerController> _logger;



        public LoadBalancerController(
            IHttpClientFactory httpClientFactory,
            IOptions<LoadBalancerConfig> config, 
            ILogger<LoadBalancerController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _backendServers = config.Value.BackendServers;
            _logger = logger;
        }

        private string GetNextServer()
        {
            var server = _backendServers[_currentServerIndex];
            _currentServerIndex = (_currentServerIndex + 1) % _backendServers.Count;
            return server;
        }

        [HttpGet("search")]
        public async Task<IActionResult> ForwardRequest(
            [FromQuery] string query,
            [FromQuery] int maxAmount = 10,
            [FromQuery] bool caseSensitive = false,
            [FromQuery] bool includeTimeStamps = true,
            [FromQuery] string[]? domains = null)
        {
            var server = GetNextServer();

            var queryParams = new List<string>
        {
                $"query={Uri.EscapeDataString(query)}",
                $"maxAmount={maxAmount}",
                $"caseSensitive={caseSensitive}",
                $"includeTimeStamps={includeTimeStamps}"
        };

            if (domains != null && domains.Any())
            {
                foreach (var domain in domains)
                    queryParams.Add($"domains={Uri.EscapeDataString(domain)}");
            }

            var forwardUrl = $"{server}/api/search?" + string.Join("&", queryParams);
            _logger.LogInformation($"Forwarding request to: {server}");
            _logger.LogInformation($"Forwarding URL: {forwardUrl}");
            _logger.LogInformation($"Query: {query}, MaxAmount: {maxAmount}");

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(forwardUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(content);
                    return Ok(jsonDoc.RootElement);
                }

                return StatusCode((int)response.StatusCode, "Backend error.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to forward request");
                return StatusCode(500, $"Forwarding failed: {ex.Message}");
            }
        }
    }
}
