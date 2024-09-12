using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestSharpSocialMediaPosts.Models.Reddit;
using RestSharpSocialMediaPosts.Services;
using RestSharpSocialMediaPosts.Services.Interfaces;

namespace RestSharpSocialMediaPosts.Controllers
{
    public class RedditController : ControllerBase
    {
        private readonly IRedditService _redditService;
        private string? accessToken = null;
        public RedditController(IRedditService redditService)
        {
            _redditService = redditService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(RedditLoginModel loginModel)
        {
            int code = 200;
            
            try
            {
                accessToken = await _redditService.GetAccessToken(loginModel);
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


    }
}
