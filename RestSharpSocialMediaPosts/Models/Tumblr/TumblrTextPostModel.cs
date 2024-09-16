using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSharpSocialMediaPosts.Models.Tumblr
{
    public class TumblrTextPostModel : TumblrPostModel
    {
        public string? title { get; set; }
        public string body { get; set; }
    }
}
