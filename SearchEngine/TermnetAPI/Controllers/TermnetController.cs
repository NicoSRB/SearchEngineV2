using Microsoft.AspNetCore.Mvc;
using Shared.Model;
using TermnetAPI.Interfaces;


namespace TermnetAPI.Controllers
{
    [ApiController]
    [Route("api/termnet")]
    public class TermnetController : ControllerBase
    {
        private readonly ITermnetService _termnetService;

        public TermnetController(ITermnetService termnetService)
        {
            _termnetService = termnetService;
        }

        [HttpPost("expand")]
        public async Task<IActionResult> Expand([FromBody] TermnetRequest request)
        {
            var terms = request.Query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var response = new TermnetResponse();

            foreach (var term in terms)
            {
                response.ExpandedTerms[term] = await _termnetService.GetSynonymsAsync(term, request.Domains);
            }

            return Ok(response);
        }
    }
}
