using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharpSocialMediaPosts.Reddit.Models;
using RestSharpSocialMediaPosts.Reddit.Services.Interfaces;
using RestSharpSocialMediaPosts.Validation;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace RestSharpSocialMediaPosts.Reddit.Services
{
    public class RedditService : IRedditService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static string _state = Guid.NewGuid().ToString();
        private readonly string? _clientId;
        private readonly string? _clientSecret;
        private readonly string? _redirectUri;
        private Timer? _refreshTokenTimer;

        public RedditService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientId = configuration["Reddit:ClientId"];
            _clientSecret = configuration["Reddit:ClientSecret"];
            _redirectUri = configuration["Reddit:RedirectUri"];
        }

        private (RestClient, RestRequest) FillOutLoginRequest(string authToken)
        {
            // Base URL for Reddit
            RestClient client = new RestClient("https://www.reddit.com");

            // The subpath for the access token request
            RestRequest request = new RestRequest("/api/v1/access_token", Method.Post);

            // Add headers
            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}")));
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");


            // Add Parameters
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("duration", "permanent");
            request.AddParameter("code", authToken);
            request.AddParameter("redirect_uri", _redirectUri);

            return (client, request);
        }

        public void StartTokenTimer()
        {
            _refreshTokenTimer = new Timer(async state =>
            {
                await RefreshToken();
            }, null, TimeSpan.FromHours(1), Timeout.InfiniteTimeSpan);
        }

        public async Task<Result<RedditTokenModel, ValidationFailed>> GetAccessToken(string authToken, string stateToCompare)
        {
            if (stateToCompare != _state)
            {
                return new ValidationFailed("Potential CSRF Attack.", 403);
            }

            try
            {
                var (client, request) = FillOutLoginRequest(authToken);
                RestResponse response = await client.ExecuteAsync(request);

                if (!response.IsSuccessful)
                {
                    return new ValidationFailed(response);
                }

                var json = JObject.Parse(response.Content);
                if (json == null)
                {
                    return new ValidationFailed("Response content is null", 500);
                }

                if (string.IsNullOrEmpty(json["access_token"].ToString()) || string.IsNullOrEmpty(json["refresh_token"].ToString()))
                {
                    return new ValidationFailed("Access or refresh token is missing", 400);
                }

                RedditTokenModel tokens = new RedditTokenModel()
                {
                    accessToken = json["access_token"]?.ToString(),
                    refreshToken = json["refresh_token"]?.ToString()
                };


                return tokens;
            }
            catch (Exception ex)
            {
                return new ValidationFailed(ex.Message, 500);
            }
        }

        public async Task<bool> MakeOAuth2Request()
        {
            // Base URL for Reddit
            RestClient client = new RestClient("https://www.reddit.com");

            // The subpath for the access token request
            RestRequest request = new RestRequest("/api/v1/authorize", Method.Get);

            RedditAuthModel authModel = new RedditAuthModel();

            // Add Parameters
            request.AddParameter("client_id", _clientId);
            request.AddParameter("response_type", "code");
            request.AddParameter("duration", "permanent");
            request.AddParameter("state", _state);
            request.AddParameter("scope", authModel.Scope);
            if (authModel.RedirectUri != null)
            {
                request.AddParameter("redirect_uri", authModel.RedirectUri);
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = client.BuildUri(request).ToString(),
                    UseShellExecute = true,
                });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return false;
            }
        }

        private (RestClient, RestRequest) FillOutPostRequest(RedditPostModel postModel, string accessToken)
        {

            // Create new client for oauth
            RestClient client = new RestClient("https://oauth.reddit.com");

            // Rest Request for the submit
            RestRequest request = new RestRequest($"/r/{postModel.SubReddit}/api/submit", Method.Post);

            // Add headers
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            request.AddHeader("User-Agent", $"{postModel.AppName}/1.0 by {postModel.UserName}");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            // add parameters
            request.AddParameter("kind", postModel.Kind);
            request.AddParameter("sr", postModel.SubReddit);
            request.AddParameter("title", postModel.Title);
            if (postModel.Text != null)
            {
                request.AddParameter("text", postModel.Text);
            }
            else if (postModel.Url != null)
            {
                request.AddParameter("url", postModel.Url);
            }
            if (postModel.FlairText != null && postModel.FlairId != null)
            {
                request.AddParameter("flair_id", postModel.FlairId);
                request.AddParameter("flair_text", postModel.FlairText);
            }

            return (client, request);
        }

        public async Task<Result<string?, ValidationFailed>> SubmitPost(RedditPostModel postModel, string accessToken)
        {
            var (client, request) = FillOutPostRequest(postModel, accessToken);
            RestResponse response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                return new ValidationFailed(response);
            }

            return "Post submitted successfully";
            
        }

        public async Task<Result<(string?, string?), ValidationFailed>> RefreshToken()
        {
            string? refreshToken = _httpContextAccessor.HttpContext.Session.GetString("redditRefreshToken");

            if (refreshToken == null)
            {
                return new ValidationFailed("Refresh Token is null", 500);
            }
            
            RestClient client = new RestClient("https://reddit.com/");
            RestRequest request = new RestRequest("/api/v1/access_token");

            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}")));

            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("refresh_token", refreshToken);

            try
            {
                RestResponse response = await client.ExecuteAsync(request);
                if (!response.IsSuccessful)
                {
                    return new ValidationFailed(response);
                }

                var json = JObject.Parse(response.Content);
                if (json == null)
                {
                    return new ValidationFailed("Response content is null", 500);
                }

                string newAccessToken = json["access_token"].ToString();
                string newRefreshToken = json["refresh_token"].ToString();

                if (string.IsNullOrEmpty(newAccessToken) || string.IsNullOrEmpty(newRefreshToken))
                {
                    return new ValidationFailed("Access or refresh token is missing", 400);
                }

                return (newAccessToken, newRefreshToken);
               
                }
            catch (Exception ex)
            {
                return new ValidationFailed(ex.Message, 500);
            }
           
        }
    }
}
