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
        private string? accessToken = null;
        public RedditController(IRedditService redditService)
        {
            _service = redditService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(RedditLoginModel loginModel)
        {
            int code = 200;
            
            try
            {
                accessToken = await _service.GetAccessToken(loginModel);
                if (accessToken == null)
                {
                    return BadRequest("An error occurred");
                }
                else
                {
                    return Ok(accessToken);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("post_thread")]
        public async Task<IActionResult> Post(RedditPostModel postModel)
        {
            int code = 201;

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
