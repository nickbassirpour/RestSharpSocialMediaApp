using RestSharpSocialMediaPosts.Reddit.Models;
using RestSharpSocialMediaPosts.Validation;

namespace RestSharpSocialMediaPosts.Reddit.Services.Interfaces
{
    public interface IRedditService
    {
        void StartTokenTimer();
        Task<bool> MakeOAuth2Request();
        Task<Result<RedditTokenModel, ValidationFailed>> GetAccessToken(string authToken, string stateToCompare);
        Task<Result<string?, ValidationFailed>> SubmitPost(RedditPostModel postModel, string accessToken);
    }
}