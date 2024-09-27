namespace RestSharpSocialMediaPosts.Token
{
    public interface ITokenRefreshService
    {
        void SetRedditRefreshToken(string refreshToken);
    }
}