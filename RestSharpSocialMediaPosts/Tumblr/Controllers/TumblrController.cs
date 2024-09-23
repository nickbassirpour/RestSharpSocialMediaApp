using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using RestSharpSocialMediaPosts.Tumblr.Models;
using RestSharpSocialMediaPosts.Tumblr.Services.Interfaces;

namespace RestSharpSocialMediaPosts.Tumblr.Controllers
{
    public class TumblrController : ControllerBase
    {
        private readonly ITumblrService _service;

        public TumblrController(ITumblrService tumblrService)
        {
            _service = tumblrService;
        }

        [AllowAnonymous]
        [HttpGet("tumblr_request_permission")]
        public async Task<IActionResult?> RequestPermission()
        {
            try
            {
                bool success = await _service.MakeOAuth2Request();
                if (!success)
                {
                    return BadRequest();
                }
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("tumblr_get_token")]
        public async Task<IActionResult> GetAccessToken(string code, string state)
        {
            try
            {
                TumblrAccessTokenModel? tokenModel = await _service.GetAccessToken(code);
                if (tokenModel == null)
                {
                    return BadRequest();
                }
                
                HttpContext.Session.SetString("TumblrAccessToken", tokenModel.AccessToken);
                HttpContext.Session.SetString("TumblrExpiresIn", tokenModel.ExpiresIn);
                return StatusCode(201, tokenModel.AccessToken);
                
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
                string? accessToken = HttpContext.Session.GetString("TumblrAccessToken");

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
