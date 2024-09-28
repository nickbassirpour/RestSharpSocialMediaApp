using RestSharp;
using RestSharpSocialMediaPosts.Tumblr.Models;
using RestSharpSocialMediaPosts.Validation;

namespace RestSharpSocialMediaPosts.Tumblr.Services.Interfaces
{
    public interface ITumblrService
    {
        Task<bool> MakeOAuth2Request();
        Task<Result<TumblrAccessTokenModel?, ValidationFailed>> GetAccessToken(string authToken, string stateToCompare);
        Task<string> PostTextPost(TumblrTextPostModel textPostModel, string accessToken);
        Task<Result<(string?, string?), ValidationFailed>> RefreshToken();
    }
}