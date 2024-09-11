using RestSharpSocialMediaPosts.Models.Reddit;

namespace RestSharpSocialMediaPosts.Services.Interfaces
{
    public interface IRedditService
    {
        Task<string?> GetAccessToken(RedditLoginModel loginModel);
        void SubmitPost(RedditPostModel postModel, string accessToken);
    }
}