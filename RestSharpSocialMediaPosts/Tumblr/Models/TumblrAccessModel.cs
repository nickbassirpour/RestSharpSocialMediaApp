﻿namespace RestSharpSocialMediaPosts.Tumblr.Models
{
    public class TumblrAccessModel
    {
        public string grant_type { get; set; } = "authorization_code";
        public string code { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string? redirect_uri { get; set; }

    }
}
