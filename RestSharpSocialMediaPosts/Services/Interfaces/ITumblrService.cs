using RestSharpSocialMediaPosts.Models.Tumblr;

namespace RestSharpSocialMediaPosts.Services.Interfaces
{
    public interface ITumblrService
    {
        Task<string?> MakeOAuth2Request(TumblrAuthModel authModel);
    }
}