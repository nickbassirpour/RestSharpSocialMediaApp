using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSharpSocialMediaPosts.Models.Tumblr
{
    public class TumblrTextPostModel : TumblrPostModel
    {
        public string? Title { get; set; }
        public string Body { get; set; }
    }
}
