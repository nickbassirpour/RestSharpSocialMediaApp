using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Principal;

namespace RestSharpSocialMediaPosts.Reddit.Models
{
    public class RedditAuthModel
    {
        [Required]
        public string ClientId { get; set; } = "";
        public string? RedirectUri { get; set; } = "https://localhost:7091/reddit_get_token";
        [Required]
        [RegularExpression("identity|edit|flair|history|modconfig|modflair|modlog|modposts|modwiki|mysubreddits|privatemessages|read|report|save|submit|subscribe|vote|wikiedit|wikiread", ErrorMessage = "Incorrect Scope")]
        public string Scope { get; set; } = "submit";
    }
}
