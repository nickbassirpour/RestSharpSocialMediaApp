using RestSharp.Authenticators.OAuth;
using RestSharp.Authenticators;
using RestSharp;
using System.Diagnostics;
using System.Web;
using RestSharpSocialMediaPosts.Models.Tumblr;
using RestSharpSocialMediaPosts.Services.Interfaces;
using System.Formats.Asn1;

namespace RestSharpSocialMediaPosts.Services
{
    public class TumblrService : ITumblrService
    {

        private string _accessToken;

        public async Task<string?> MakeOAuth2Request(TumblrAuthModel authModel)
        {
            RestClient client = new RestClient("https://www.tumblr.com/");
            RestRequest request = new RestRequest("oauth2/authorize");

            request.AddParameter("client_id", authModel.client_id);
            request.AddParameter("response_type", authModel.response_type);
            request.AddParameter("scope", authModel.scope);
            request.AddParameter("state", authModel.state);
            if (authModel.redirect_uri != null)
            {
                request.AddParameter("redirect_uri", authModel.redirect_uri);
            }

            try
            {
                var response = await client.ExecuteAsync(request);
                if (response.IsSuccessful)
                {
                    var parsedString = HttpUtility.ParseQueryString(response.Content);
                    System.Diagnostics.Debug.WriteLine(parsedString);
                    string codeForAccessToken = parsedString["code"];
                    string stateToCompare = parsedString["state"];
                    System.Diagnostics.Debug.WriteLine(codeForAccessToken);
                    System.Diagnostics.Debug.WriteLine(stateToCompare);
                    if (stateToCompare == authModel.state)
                    {
                        return codeForAccessToken;
                    }
                    else
                    {
                        return "State values did not match; error retrieving code for access token";
                    }
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
        //private async Task<(string?, string?)> GetOAuthToken(string consumerKey, string consumerSecret)
        //{
        //    RestClientOptions options = new RestClientOptions("https://www.tumblr.com/")
        //    {
        //        Authenticator = OAuth1Authenticator.ForProtectedResource(consumerKey, consumerSecret, null, null, OAuthSignatureMethod.HmacSha1)
        //    };

        //    var client = new RestClient(options);

        //    // The subpath for the authorization request
        //    RestRequest request = new RestRequest($"oauth/request_token", Method.Post);

        //    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

        //    try
        //    {
        //        var response = await client.ExecuteAsync(request);
        //        if (response.IsSuccessful)
        //        {
        //            var parsedString = HttpUtility.ParseQueryString(response.Content);
        //            string oauthToken = parsedString["oauth_token"];
        //            string oauthTokenSecret = parsedString["oauth_token_secret"];
        //            Console.WriteLine($"OAuth Token: {oauthToken}");
        //            Console.WriteLine($"OAuth Token Secret: {oauthTokenSecret}");
        //            return (oauthToken, oauthTokenSecret);
        //        }
        //        else
        //        {
        //            Console.WriteLine($"Error: {response.StatusCode}");
        //            Console.WriteLine($"Content: {response.Content}");
        //            return (null, null);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Exception occurred: {ex.Message}");
        //        return (null, null);
        //    }
        //}

        //public async Task<(string, string, string)> GetOAuthVerifier(string consumerKey, string consumerSecret)
        //{
        //    (string? oauthToken, string? oauthTokenSecret) = await GetOAuthToken(consumerKey, consumerSecret);

        //    string authURL = $"https://www.tumblr.com/oauth/authorize?oauth_token={oauthToken}";
        //    Console.WriteLine("Please visit the following URL to authorize the application:");
        //    Console.WriteLine(authURL);
        //    System.Diagnostics.Process.Start(new ProcessStartInfo
        //    {
        //        FileName = authURL,
        //        UseShellExecute = true,
        //    });
        //    Console.WriteLine("Once you agree to giving access, grab the oauth_verifier GUID in the URL and paste it here: ");
        //    string oauthVerifier = Console.ReadLine();

        //    return (oauthToken, oauthTokenSecret, oauthVerifier);
        //}

        //public static async Task<string?> GetAccessToken(string consumerKey, string consumerSecret, string oauthToken, string oauthSecret, string oauthVerifier)
        //{
        //    RestClientOptions options = new RestClientOptions("https://www.tumblr.com/")
        //    {
        //        Authenticator = OAuth1Authenticator.ForAccessToken(

        //            consumerKey,
        //            consumerSecret,
        //            oauthToken,
        //            oauthSecret,
        //            oauthVerifier)
        //    };

        //    RestClient client = new RestClient(options);
        //    RestRequest request = new RestRequest("oauth/access_token", Method.Post);

        //    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

        //    try
        //    {
        //        var response = await client.ExecuteAsync(request);
        //        if (response.IsSuccessful)
        //        {
        //            var parsedContent = HttpUtility.ParseQueryString(response.Content);
        //            string accessToken = parsedContent["oauth_token"]?.ToString();
        //            Console.WriteLine($"Your access token: {accessToken}");
        //            return accessToken;
        //        }
        //        else
        //        {
        //            Console.WriteLine($"Error: {response.ErrorMessage}");
        //            Console.WriteLine($"Content: {response.Content}");
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Exception occurred: {ex.Message}");
        //        return null;
        //    }
        //}

        //private RestRequest FilloutDefaultRequest(RestRequest request, TumblrPostModel postModel)
        //{
        //    request.AddHeader("Authorization: ", $"Bearer {_accessToken}");
        //    request.AddHeader("Content-Type", "application/json");

        //    request.AddParameter("type", postModel.Type);

        //    if (!string.IsNullOrEmpty(postModel.State))
        //        request.AddParameter("state", postModel.State);
        //    if (!string.IsNullOrEmpty(postModel.Tags))
        //        request.AddParameter("tags", postModel.Tags);
        //    if (!string.IsNullOrEmpty(postModel.Tweet))
        //        request.AddParameter("tweet", postModel.Tweet);
        //    if (!string.IsNullOrEmpty(postModel.Date))
        //        request.AddParameter("date", postModel.Date);
        //    if (!string.IsNullOrEmpty(postModel.Format))
        //        request.AddParameter("format", postModel.Format);
        //    if (!string.IsNullOrEmpty(postModel.Slug))
        //        request.AddParameter("slug", postModel.Slug);
        //    if (!string.IsNullOrEmpty(postModel.NativeInlineImages))
        //        request.AddParameter("native_inline_images", postModel.NativeInlineImages);

        //    return request;
        //}

        //public async Task<string> PostTextPost(TumblrTextPostModel textPostModel)
        //{
        //    RestClient client = new RestClient($"https://api.tumblr.com/");
        //    RestRequest unfilledRequest = new RestRequest($"v2/blog/{textPostModel.BlogId}/posts", Method.Post);
        //    RestRequest request = FilloutDefaultRequest(unfilledRequest, textPostModel);

        //    request.AddParameter("title", textPostModel.Title);
        //    request.AddParameter("body", textPostModel.Body);

        //    return await Post(client, request);
        //}

        //private async Task<string> Post(RestClient client, RestRequest request)
        //{
        //    try
        //    {
        //        var response = await client.ExecuteAsync(request);

        //        if (response.IsSuccessful)
        //        {
        //            return $"Success: {response.Content}";
        //        }
        //        else
        //        {
        //            return $"Error: {response.StatusCode} - {response.StatusDescription}";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return $"Exception occurred: {ex.Message}";
        //    }
        //}
    }
}
