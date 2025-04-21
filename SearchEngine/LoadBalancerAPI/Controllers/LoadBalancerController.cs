using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LoadBalancerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoadBalancerController : ControllerBase
    {
        private static List<string> backendServers = new List<string>
        {
            "http://localhost:5267", // Example backend server 1
            "http://localhost:5268", // Example backend server 2
        };

        private static int currentServerIndex = 0;

        // Round-robin load balancing method
        private static string GetNextServer()
        {
            // stay within the bounds of available servers
            var server = backendServers[currentServerIndex];

            // Move to the next server, wrapping around if necessary
            currentServerIndex = (currentServerIndex + 1) % backendServers.Count;

            return server;
        }

        // Forward request to backend server and perform a redirect
        [HttpGet("search/{query}/{maxAmount}")]
        public async Task<IActionResult> ForwardRequest(string query, int maxAmount)
        {
            // Get the next backend server using round-robin
            var server = GetNextServer();

            // Construct the URL to forward the request to the backend server
            var forwardUrl = $"{server}/api/search/{query}/{maxAmount}";

            using (var client = new HttpClient())
            {
                try
                {
                    // Make HTTP GET request to the backend server
                    var response = await client.GetAsync(forwardUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        return Ok(content);
                    }
                    else
                    {
                        // If the request fails, return the status code of the backend response
                        return StatusCode((int)response.StatusCode, "Error in backend service.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    return StatusCode(500, $"Error forwarding the request: {ex.Message}");
                }
            }
        }
    }
}
