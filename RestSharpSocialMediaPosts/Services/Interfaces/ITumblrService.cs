using RestSharpSocialMediaPosts.Models.Tumblr;

namespace RestSharpSocialMediaPosts.Services.Interfaces
{
    public interface ITumblrService
    {
        Task<(string, string, string)> GetOAuthVerifier(string consumerKey, string consumerSecret);
        Task<string> PostTextPost(TumblrTextPostModel textPostModel);
    }
}