namespace RestSharpSocialMediaPosts.Models.Tumblr
{
    public class TumblrAccessTokenModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiresIn { get; set; }
    }
}
