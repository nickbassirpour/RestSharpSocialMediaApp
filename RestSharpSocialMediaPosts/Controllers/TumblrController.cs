using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using RestSharpSocialMediaPosts.Models.Reddit;
using RestSharpSocialMediaPosts.Models.Tumblr;
using RestSharpSocialMediaPosts.Services;
using RestSharpSocialMediaPosts.Services.Interfaces;

namespace RestSharpSocialMediaPosts.Controllers
{
    public class TumblrController : ControllerBase
    {
        private readonly ITumblrService _service;

        public TumblrController(ITumblrService tumblrService)
        {
            _service = tumblrService;
        }

        [AllowAnonymous]
        [HttpGet("request_permission")]
        public async Task<IActionResult?> RequestPermission()
        {
            try
            {
                bool success = await _service.MakeOAuth2Request();
                if (success)
                {
                    return StatusCode(201);
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

        [AllowAnonymous]
        [HttpGet("get_token")]
        public async Task<IActionResult> GetAccessToken(string code, string state)
        {
            try
            {
                TumblrAccessTokenModel? tokenModel = await _service.GetAccessToken(code);
                if (tokenModel != null)
                {
                    HttpContext.Session.SetString("AccessToken", tokenModel.AccessToken);
                    HttpContext.Session.SetString("ExpiresIn", tokenModel.ExpiresIn);
                    return StatusCode(201, tokenModel.AccessToken);
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

        [AllowAnonymous]
        [HttpPost("make_post")]
        public async Task<IActionResult> MakeTextPost(TumblrTextPostModel postModel)
        {
            try
            {
                string? accessToken = HttpContext.Session.GetString("AccessToken");

                if (accessToken == null)
                {
                    return Unauthorized("Access token not found");
                }

                string response = await _service.PostTextPost(postModel, accessToken);
                return StatusCode(201, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
