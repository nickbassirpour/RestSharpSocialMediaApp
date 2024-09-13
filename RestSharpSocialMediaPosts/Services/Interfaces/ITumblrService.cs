using RestSharpSocialMediaPosts.Models.Tumblr;

namespace RestSharpSocialMediaPosts.Services.Interfaces
{
    public interface ITumblrService
    {
        void MakeOAuth2Request();

        Task<TumblrAccessTokenModel?> GetAccessToken(string authToken);
    }
}