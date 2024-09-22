using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestSharpSocialMediaPosts.Models.Reddit;
using RestSharpSocialMediaPosts.Services;
using RestSharpSocialMediaPosts.Services.Interfaces;

namespace RestSharpSocialMediaPosts.Controllers
{
    public class RedditController : ControllerBase
    {
        private readonly IRedditService _service;
        public RedditController(IRedditService redditService)
        {
            _service = redditService;
        }

        [AllowAnonymous]
        [HttpGet("reddit_request_permission")]
        public async Task<IActionResult> RequestPermission()
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
        [HttpGet("reddit_get_token")]
        public async Task<IActionResult> GetAccessToken(string code, string state)
        {
            try
            {
                if (code != null && state != null)
                {
                    (string? accessToken, string? refreshToken) = await _service.GetAccessToken(code, state);
                    HttpContext.Session.SetString("redditAccessToken", accessToken);
                    HttpContext.Session.SetString("redditRefreshToken", refreshToken);

                    _service.StartTokenTimer();
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

        //[AllowAnonymous]
        //[HttpPost("reddit_login")]
        //public async Task<IActionResult> Login(RedditLoginModel loginModel)
        //{
        //    int code = 200;

        //    try
        //    {
        //        (string? accessToken, string? refreshToken) = await _service.GetAccessToken(loginModel);
        //        if (accessToken == null || refreshToken == null)
        //        {
        //            return BadRequest("An error occurred");
        //        }
        //        else
        //        {
        //            HttpContext.Session.SetString("redditAccessToken", accessToken);
        //            HttpContext.Session.SetString("redditRefreshToken", refreshToken);
        //            return Ok(accessToken);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [AllowAnonymous]
        [HttpPost("post_thread")]
        public async Task<IActionResult> Post(RedditPostModel postModel)
        {
            int code = 201;
            string? accessToken = HttpContext.Session.GetString("redditAccessToken"); 

            if (accessToken != null)
            {
                try
                {
                    string? result = await _service.SubmitPost(postModel, accessToken);
                    if (result != null)
                    {
                        return StatusCode(code, result);
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
            else
            {
                return Unauthorized();
            }
        }

    }
}
