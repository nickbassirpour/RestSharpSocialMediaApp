using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSharpSocialMediaPosts.Models.Tumblr
{
    public class TumblrPostModel
    {
        public string blogId { get; set; } = "devtestercsharp";
        [Required]
        [RegularExpression("text|photo|quote|link|chat|audio|video", ErrorMessage = "Invalid Post Type")]

        public string type { get; set; }
        [Required]
        [RegularExpression("published|draft|queue|private", ErrorMessage = "Invalid Post State")]
        public string? state { get; set; }
        public string? tags { get; set; }
        public string? tweet { get; set; }
        public string? date { get; set; }
        public string? format { get; set; }
        public string? slug { get; set; }
        [RegularExpression("true|false", ErrorMessage = "Must be bool value (true/false).")]
        public string? nativeInlineImages { get; set; } = "false";



    }
}
