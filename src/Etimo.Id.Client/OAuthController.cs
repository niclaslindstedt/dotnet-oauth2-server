using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Etimo.Id.Client
{
    [ApiController]
    [Route("oauth2")]
    public class OAuthController : ControllerBase
    {
        private readonly IEtimoIdClient _idClient;

        public OAuthController(IEtimoIdClient idClient)
        {
            _idClient = idClient;
        }

        [HttpGet]
        [Route("callback")]
        public async Task<IActionResult> CallbackAsync(
            [FromQuery] string code,
            [FromQuery] string state,
            [FromQuery] string error,
            [FromQuery] string error_description,
            [FromQuery] string error_uri)
        {
            if (error != null) { return BadRequest(error_description); }

            if (await _idClient.AuthorizeAsync(code, state))
            {
                if (await _idClient.ValidateAccessTokenAsync()) { return Ok(_idClient.GetAccessToken()); }
            }

            return Unauthorized();
        }
    }
}
