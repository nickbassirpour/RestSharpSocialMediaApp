using RestSharp.Authenticators.OAuth;
using RestSharp.Authenticators;
using RestSharp;
using System.Diagnostics;
using System.Web;
using System.Formats.Asn1;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharpSocialMediaPosts.Tumblr.Services.Interfaces;
using RestSharpSocialMediaPosts.Tumblr.Models;
using RestSharpSocialMediaPosts.Validation;
using Microsoft.AspNetCore.Http;

namespace RestSharpSocialMediaPosts.Tumblr.Services
{
    public class TumblrService : ITumblrService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static string _state = Guid.NewGuid().ToString();
        private readonly string? _clientId;
        private readonly string? _clientSecret;
        private readonly string? _redirectUri;

        public TumblrService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientId = configuration["Tumblr:ConsumerKey"];
            _clientSecret = configuration["Tumblr:ConsumerSecret"];
            _redirectUri = configuration["Tumblr:RedirectUri"];
        }
        public async Task<bool> MakeOAuth2Request()
        {
            RestClient client = new RestClient("https://www.tumblr.com/");
            RestRequest request = new RestRequest("oauth2/authorize", Method.Get);

            //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            TumblrAuthModel authModel = new TumblrAuthModel();

            request.AddParameter("client_id", _clientId);
            request.AddParameter("response_type", authModel.response_type);
            request.AddParameter("scope", authModel.scope);
            request.AddParameter("state", _state);
            if (authModel.redirect_uri != null)
            {
                request.AddParameter("redirect_uri", authModel.redirect_uri);
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

        public async Task<Result<TumblrAccessTokenModel?, ValidationFailed>> GetAccessToken(string authToken, string stateToCompare)
        {
            if (stateToCompare != _state) return new ValidationFailed("Potential CSRF attack", 403);

            RestClient client = new RestClient("https://api.tumblr.com/");
            RestRequest request = new RestRequest("v2/oauth2/token", Method.Post);

            TumblrAccessModel accessModel = new TumblrAccessModel();
            accessModel.code = authToken;

            request.AddParameter("grant_type", accessModel.grant_type);
            request.AddParameter("code", accessModel.code);
            request.AddParameter("client_id", _clientId);
            request.AddParameter("client_secret", _clientSecret);
            request.AddParameter("redirect_uri", _redirectUri);

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

                string accessToken = json["access_token"].ToString();
                string refreshToken = json["refresh_token"].ToString();

                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
                {
                    return new ValidationFailed("Access token or expiration is missing", 400);
                }

                TumblrAccessTokenModel tokenModel = new TumblrAccessTokenModel()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
                return tokenModel;
            }
            catch (Exception ex)
            {
                return new ValidationFailed(ex.Message, 500);
            }
        }

        private RestRequest FilloutDefaultRequest(string blogId, TumblrPostModel postModel, string accessToken)
        {
            RestRequest request = new RestRequest($"v2/blog/{blogId}/post", Method.Post);
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            request.AddParameter("type", postModel.type);

            if (!string.IsNullOrEmpty(postModel.state))
                request.AddParameter("state", postModel.state);
            if (!string.IsNullOrEmpty(postModel.tags))
                request.AddParameter("tags", postModel.tags);
            if (!string.IsNullOrEmpty(postModel.tweet))
                request.AddParameter("tweet", postModel.tweet);
            if (!string.IsNullOrEmpty(postModel.date))
                request.AddParameter("date", postModel.date);
            if (!string.IsNullOrEmpty(postModel.format))
                request.AddParameter("format", postModel.format);
            if (!string.IsNullOrEmpty(postModel.slug))
                request.AddParameter("slug", postModel.slug);
            if (!string.IsNullOrEmpty(postModel.nativeInlineImages))
                request.AddParameter("native_inline_images", postModel.nativeInlineImages);

            return request;
        }

        public async Task<string> PostTextPost(TumblrTextPostModel textPostModel, string accessToken)
        {
            RestClient client = new RestClient($"https://api.tumblr.com/");
            RestRequest request = FilloutDefaultRequest(textPostModel.blogId, textPostModel, accessToken);

            if (textPostModel.title != null)
            {
                request.AddParameter("title", textPostModel.title);
            }
            request.AddParameter("body", textPostModel.body);

            string response = await Post(client, request);

            return response;
        }

        private async Task<string?> Post(RestClient client, RestRequest request)
        {
            try
            {
                var response = await client.ExecuteAsync(request);

                if (!response.IsSuccessful)
                {
                    return $"Error: {response.StatusCode} - {response.StatusDescription} - {response.Content}";
                }

                return $"Success: {response.Content}";
            }
            catch (Exception ex)
            {
                return $"Exception occurred: {ex.Message}";
            }
        }

        public async Task<Result<(string?, string?), ValidationFailed>> RefreshToken()
        {
            string? accessToken = _httpContextAccessor.HttpContext.Session.GetString("tumblrAccessToken");
            string? refreshToken = _httpContextAccessor.HttpContext.Session.GetString("tumblrRefreshToken");

            if (refreshToken == null)
            {
                return new ValidationFailed("Refresh Token is null", 500);
            }

            RestClient client = new RestClient("https://api.tumblr.com");
            RestRequest request = new RestRequest("/v2/oauth2/token", Method.Post);

            request.AddHeader("Authorization", "Bearer " + accessToken);

            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("client_id", _clientId);
            request.AddParameter("client_secret", _clientSecret);
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
