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
        public string BlogId { get; set; }
        [RegularExpression("text|photo|quote|link|chat|audio|video", ErrorMessage = "Invalid Post Type")]
        public string Type { get; set; }
        [RegularExpression("published|draft|queue|private", ErrorMessage = "Invalid Post State")]
        public string? State { get; set; }
        public string? Tags { get; set; }
        public string? Tweet { get; set; }

        public string? Date { get; set; }
        public string? Format { get; set; }
        public string? Slug { get; set; }
        [RegularExpression("true|false", ErrorMessage = "Must be bool value (true/false).")]
        public string? NativeInlineImages { get; set; } = "false";



    }
}
