using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Model;
using TermnetAPI.Interfaces;


namespace TermnetAPI.Controllers
{
    [ApiController]
    [Route("api/termnet")]
    public class TermnetController : ControllerBase
    {
        private readonly ITermnetService _termnetService;
        private readonly bool _termnetEnabled;
        private readonly ILogger<TermnetController> _logger;


        public TermnetController(ITermnetService termnetService, ILogger<TermnetController> logger, IConfiguration config)
        {
            _termnetService = termnetService;
            _termnetEnabled = config.GetValue<bool>("TermnetAPI:Enabled", true);
            _logger = logger;
        }

        [HttpPost("expand")]
        public async Task<IActionResult> Expand([FromBody] TermnetRequest request)
        {
            if (!_termnetEnabled)
            {
                _logger.LogInformation("Termnet API is disabled - returning empty response.");
                return Ok(new TermnetResponse());
            }

            _logger.LogInformation("Termnet expand request received for query: {Query}", request.Query);

            var terms = request.Query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var response = new TermnetResponse();

            foreach (var term in terms)
            {
                response.ExpandedTerms[term] = await _termnetService.GetSynonymsAsync(term, request.Domains);
            }

            _logger.LogInformation("Termnet expand request completed for query: {Query}", request.Query);
            return Ok(response);

        }
    }
}
