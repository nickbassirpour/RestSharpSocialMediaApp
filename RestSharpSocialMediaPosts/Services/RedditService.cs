using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharpSocialMediaPosts.Models.Reddit;
using RestSharpSocialMediaPosts.Services.Interfaces;
using System.Text;

namespace RestSharpSocialMediaPosts.Services
{
    public class RedditService : IRedditService
    {
        private (RestClient, RestRequest) FillOutLoginRequest(RedditLoginModel loginModel)
        {
            // Base URL for Reddit
            RestClient client = new RestClient("https://www.reddit.com");

            // The subpath for the access token request
            RestRequest request = new RestRequest("/api/v1/access_token", Method.Post);

            // Add headers
            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{loginModel.client_id}:{loginModel.client_secret}")));
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            // Add Parameters
            request.AddParameter("grant_type", "password");
            request.AddParameter("username", loginModel.username);
            request.AddParameter("password", loginModel.password);

            return (client, request);
        }

        public async Task<string?> GetAccessToken(RedditLoginModel loginModel)
        {
            try
            {
                var (client, request) = FillOutLoginRequest(loginModel);
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    var json = JObject.Parse(response.Content);
                    string access_token = json["access_token"]?.ToString();
                    Console.WriteLine(access_token);
                    return access_token;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                    Console.WriteLine($"Content: {response.Content}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return null;
            }
        }

        private (RestClient, RestRequest) FillOutPostRequest(RedditPostModel postModel, string accessToken)
        {

            // Create new client for oauth
            RestClient client = new RestClient("https://oauth.reddit.com");

            // Rest Request for the submit
            RestRequest request = new RestRequest($"/r/{postModel.subReddit}/api/submit", Method.Post);

            // Add headers
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            request.AddHeader("User-Agent", $"{postModel.redditAppName}/1.0 by {postModel.redditUserName}");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            // add parameters
            request.AddParameter("kind", postModel.kind);
            request.AddParameter("sr", postModel.subReddit);
            request.AddParameter("title", postModel.title);
            request.AddParameter("text", postModel.text);
            request.AddParameter("flair_id", postModel.flairId);
            request.AddParameter("flair_text", postModel.flairText);

            return (client, request);
        }

        public async void SubmitPost(RedditPostModel postModel, string accessToken)
        {
            var (client, request) = FillOutPostRequest(postModel, accessToken);
            var submitResponse = await client.ExecuteAsync(request);

            if (submitResponse.IsSuccessful)
            {
                Console.WriteLine("Post submitted successfully!");
                Console.WriteLine(submitResponse.Content);
            }
            else
            {
                Console.WriteLine($"Error: {submitResponse.StatusCode}");
                Console.WriteLine(submitResponse.Content);
            }
        }
    }
}
