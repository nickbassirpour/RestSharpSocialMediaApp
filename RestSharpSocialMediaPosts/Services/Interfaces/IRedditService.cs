using RestSharpSocialMediaPosts.Models.Reddit;

namespace RestSharpSocialMediaPosts.Services.Interfaces
{
    public interface IRedditService
    {
        Task<bool> MakeOAuth2Request();
        Task<(string?, string?)> GetAccessToken(string authToken, string state);
        Task<string?> SubmitPost(RedditPostModel postModel, string accessToken);
    }
}