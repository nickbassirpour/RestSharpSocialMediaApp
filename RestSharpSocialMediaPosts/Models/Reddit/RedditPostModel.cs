using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSharpSocialMediaPosts.Models.Reddit
{
    public class RedditPostModel
    {
        public string redditUserName { get; set; }
        public string redditAppName { get; set; }
        [StringLength(300)]
        public string title { get; set; }
        public string text { get; set; }
        public string subReddit { get; set; }
        [StringLength(36)]
        public string? flairId { get; set; }
        [StringLength(64)]
        public string? flairText { get; set; }
        [RegularExpression("link|self|image|video|videogif", ErrorMessage = "Invalid Post Type")]
        public string kind { get; set; }
        public Uri? link { get; set; }

    }
}
