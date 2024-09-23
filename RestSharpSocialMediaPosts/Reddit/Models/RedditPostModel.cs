using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSharpSocialMediaPosts.Reddit.Models
{
    public class RedditPostModel
    {
        public string UserName { get; set; } = "devtestercsharp";
        public string AppName { get; set; } = "devtestercsharp";
        [Required]
        [StringLength(300)]
        public string Title { get; set; }
        public string? Text { get; set; }
        public string? Url { get; set; }
        [Required]
        public string SubReddit { get; set; }
        [StringLength(36)]
        public string? FlairId { get; set; }
        [StringLength(64)]
        public string? FlairText { get; set; }
        [RegularExpression("link|self|image|video|videogif", ErrorMessage = "Invalid Post Type")]
        [Required]
        public string Kind { get; set; }
        public Uri? Link { get; set; }

    }
}
