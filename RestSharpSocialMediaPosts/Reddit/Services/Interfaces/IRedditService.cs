using RestSharpSocialMediaPosts.Reddit.Models;

namespace RestSharpSocialMediaPosts.Reddit.Services.Interfaces
{
    public interface IRedditService
    {
        void StartTokenTimer();
        Task<bool> MakeOAuth2Request();
        Task<(string?, string?)> GetAccessToken(string authToken, string state);
        Task<string?> SubmitPost(RedditPostModel postModel, string accessToken);
    }
}