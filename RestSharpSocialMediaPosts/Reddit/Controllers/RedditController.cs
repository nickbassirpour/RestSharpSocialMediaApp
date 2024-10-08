﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestSharpSocialMediaPosts.Reddit.Models;
using RestSharpSocialMediaPosts.Reddit.Services.Interfaces;
using RestSharpSocialMediaPosts.Token;

namespace RestSharpSocialMediaPosts.Reddit.Controllers
{
    public class RedditController : ControllerBase
    {
        private readonly IRedditService _service;
        private readonly ITokenRefreshService _tokenRefreshService;
        public RedditController(IRedditService redditService, ITokenRefreshService tokenRefreshService)
        {
            _service = redditService;
            _tokenRefreshService = tokenRefreshService;
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
                if (code == null || state == null)
                {
                    return BadRequest();
                }
                var response = await _service.GetAccessToken(code, state);
                return response.Match<IActionResult>(
                    success =>
                    {
                        HttpContext.Session.SetString("redditAccessToken", success.accessToken);
                        HttpContext.Session.SetString("redditRefreshToken", success.refreshToken);

                        _tokenRefreshService.SetRedditRefreshToken(success.refreshToken);
                        
                        return StatusCode(201, success.accessToken);
                    },
                    error =>
                    {
                        return StatusCode((int)error.StatusCode.GetValueOrDefault(500), error.ErrorMessage);
                    }
                );
            }
            catch (Exception ex)
            {
                // coment for the gram
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("post_thread")]
        public async Task<IActionResult> Post(RedditPostModel postModel)
        {
            string? accessToken = HttpContext.Session.GetString("redditAccessToken");

            if (accessToken == null)
            {
                return Unauthorized();
            }
            
            try
            {
                var response = await _service.SubmitPost(postModel, accessToken);
                return response.Match<IActionResult>(
                    success =>
                    {
                        return StatusCode(201, success);
                    },
                    error =>
                    {
                        return StatusCode((int)error.StatusCode.GetValueOrDefault(500), error.ErrorMessage);
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
