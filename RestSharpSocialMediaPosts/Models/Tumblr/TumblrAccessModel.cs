namespace RestSharpSocialMediaPosts.Models.Tumblr
{
    public class TumblrAccessModel
    {
        public string grant_type { get; set; } = "authorization_code";
        public string code { get; set; }
        public string client_id { get; set; } = "YdJusuzOdtRNCZfXpS6K4Mh0Ov4L4o9vpTh6SCVTiYqX5qdcsS";
        public string client_secret { get; set; } = "RcpU0wNK7TBfkBNjSGXkXhbpV6fmVIYg8HrbUH4ynYMnNDcC5L";
        public string? redirect_uri { get; set; }

    }
}
