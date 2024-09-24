using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharpSocialMediaPosts.Reddit.Models;
using RestSharpSocialMediaPosts.Reddit.Services.Interfaces;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace RestSharpSocialMediaPosts.Reddit.Services
{
    public class RedditService : IRedditService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static string _state = Guid.NewGuid().ToString();
        string clientId = Environment.GetEnvironmentVariable("reddit_client_id");
        string clientSecret = Environment.GetEnvironmentVariable("reddit_client_secret");
        private Timer _refreshTokenTimer;

        public RedditService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = _httpContextAccessor;
        }

        private (RestClient, RestRequest) FillOutLoginRequest(string authToken)
        {
            // Base URL for Reddit
            RestClient client = new RestClient("https://www.reddit.com");

            // The subpath for the access token request
            RestRequest request = new RestRequest("/api/v1/access_token", Method.Post);

            string redirectURI = Environment.GetEnvironmentVariable("reddit_redirect_uri");

            // Add headers
            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")));
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");


            // Add Parameters
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("duration", "permanent");
            request.AddParameter("code", authToken);
            request.AddParameter("redirect_uri", redirectURI);

            return (client, request);
        }

        public void StartTokenTimer()
        {
            _refreshTokenTimer = new Timer(async state =>
            {
                await RefreshToken();
            }, null, TimeSpan.FromHours(1), Timeout.InfiniteTimeSpan);
        }

        public async Task<(string?, string?)> GetAccessToken(string authToken, string stateToCompare)
        {
            if (stateToCompare != _state)
            {
                return ("Potential CSRF Attack.", null);
            }

            try
            {
                var (client, request) = FillOutLoginRequest(authToken);
                var response = await client.ExecuteAsync(request);

                if (response == null)
                {
                    return (null, null);
                }

                var json = JObject.Parse(response.Content);
                if (json == null)
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                    Console.WriteLine($"Content: {response.Content}");
                    return (null, null);
                }

                string? access_token = json["access_token"]?.ToString();
                string? refresh_token = json["refresh_token"]?.ToString();
                return (access_token, refresh_token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return (null, null);
            }
        }

        public async Task<bool> MakeOAuth2Request()
        {
            // Base URL for Reddit
            RestClient client = new RestClient("https://www.reddit.com");

            // The subpath for the access token request
            RestRequest request = new RestRequest("/api/v1/authorize", Method.Get);

            RedditAuthModel authModel = new RedditAuthModel();

            string clientId = Environment.GetEnvironmentVariable("reddit_client_id");

            // Add Parameters
            request.AddParameter("client_id", clientId);
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

        public async Task<string?> SubmitPost(RedditPostModel postModel, string accessToken)
        {
            var (client, request) = FillOutPostRequest(postModel, accessToken);
            var submitResponse = await client.ExecuteAsync(request);

            if (submitResponse.IsSuccessful)
            {
                return "Post submitted successfully!";
            }
            else
            {
                Console.WriteLine(submitResponse.Content);
                return $"Error: {submitResponse.StatusCode}";
            }
        }

        private async Task<(string?, string?)> RefreshToken()
        {
            string? refreshToken = _httpContextAccessor.HttpContext.Session.GetString("redditRefreshToken");

            if (refreshToken == null)
            {
                return (null, null);
            }
            
            RestClient client = new RestClient("https://reddit.com/");
            RestRequest request = new RestRequest("/api/v1/access_token");

            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")));

            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("refresh_token", refreshToken);

            try
            {
                var response = await client.ExecuteAsync(request);
                if (!response.IsSuccessful)
                {
                    return (null, null);
                }

                var json = JObject.Parse(response.Content);
                if (json == null)
                {
                    return (null, null);
                }

                string newAccessToken = json["access_token"].ToString();
                string newRefreshToken = json["refresh_token"].ToString();
                return (newAccessToken, newRefreshToken);
               
                }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
           
        }
    }
}
