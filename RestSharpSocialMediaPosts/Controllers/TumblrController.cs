using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestSharpSocialMediaPosts.Models.Reddit;
using RestSharpSocialMediaPosts.Models.Tumblr;
using RestSharpSocialMediaPosts.Services;
using RestSharpSocialMediaPosts.Services.Interfaces;

namespace RestSharpSocialMediaPosts.Controllers
{
    public class TumblrController : ControllerBase
    {
        private readonly ITumblrService _service;
        private string? accessRequestCode = null;
        private string? accessToken = null;

        public TumblrController(ITumblrService tumblrService)
        {
            _service = tumblrService;
        }

        [AllowAnonymous]
        [HttpPost("request_permission")]
        public async Task<IActionResult> RequestPermission(TumblrAuthModel authModel)
        {
            try
            {
                accessRequestCode = await _service.MakeOAuth2Request(authModel);
                if (accessRequestCode != null)
                {
                    return StatusCode(201, accessRequestCode);
                }
                else
                {
                    return StatusCode(400, "no access code");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
