using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharpSocialMediaPosts.Models.Reddit;
using RestSharpSocialMediaPosts.Services.Interfaces;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace RestSharpSocialMediaPosts.Services
{
    public class RedditService : IRedditService
    {
        string _state = Guid.NewGuid().ToString();
        private (RestClient, RestRequest) FillOutLoginRequest(RedditLoginModel loginModel)
        {
            // Base URL for Reddit
            RestClient client = new RestClient("https://www.reddit.com");

            // The subpath for the access token request
            RestRequest request = new RestRequest("/api/v1/access_token", Method.Post);

            // Add headers
            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{loginModel.ClientId}:{loginModel.ClientSecret}")));
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            // Add Parameters
            request.AddParameter("grant_type", "password");
            request.AddParameter("duration", "permanent");
            request.AddParameter("username", loginModel.Username);
            request.AddParameter("password", loginModel.Password);

            return (client, request);
        }

        public async Task<(string?, string?)> GetAccessToken(RedditLoginModel loginModel)
        {
            try
            {
                var (client, request) = FillOutLoginRequest(loginModel);
                var response = await client.ExecuteAsync(request);

                if (response != null && response.IsSuccessful)
                {
                    var json = JObject.Parse(response.Content);
                    if (json != null)
                    {
                        string? access_token = json["access_token"]?.ToString();
                        string? refresh_token = json["refresh_token"]?.ToString();
                        return (access_token, refresh_token);
                    }
                    else
                    {
                        return (null, null);
                    }
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                    Console.WriteLine($"Content: {response.Content}");
                    return (null, null);
                }
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

            // Add Parameters
            request.AddParameter("client_id", authModel.ClientId);
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
    }
}
