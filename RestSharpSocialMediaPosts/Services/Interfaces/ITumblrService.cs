using RestSharp;
using RestSharpSocialMediaPosts.Models.Tumblr;

namespace RestSharpSocialMediaPosts.Services.Interfaces
{
    public interface ITumblrService
    {
        Task<bool> MakeOAuth2Request();

        Task<TumblrAccessTokenModel?> GetAccessToken(string authToken);
        Task<string> PostTextPost(TumblrTextPostModel textPostModel, string accessToken);

    }
}