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
        private string? expiresIn = null;

        public TumblrController(ITumblrService tumblrService)
        {
            _service = tumblrService;
        }

        [AllowAnonymous]
        [HttpGet("request_permission")]
        public async Task<IActionResult?> RequestPermission(TumblrAuthModel authModel)
        {
            try
            {
                _service.MakeOAuth2Request();
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("get_token")]
        private async Task<IActionResult> GetAccessToken(string code, string state)
        {
            try
            {
                TumblrAccessTokenModel? tokenModel = await _service.GetAccessToken(code);
                if (tokenModel != null)
                {
                    accessToken = tokenModel.AccessToken;
                    expiresIn = tokenModel.ExpiresIn;
                    return StatusCode(201, accessToken);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
