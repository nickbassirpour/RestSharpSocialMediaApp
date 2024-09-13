using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSharpSocialMediaPosts.Models.Tumblr
{
    public class TumblrAuthModel
    {
        public string client_id { get; set; }
        public string response_type { get; set; } = "code";
        public string scope { get; set; }
        public string state { get; set; }
        public string? redirect_uri { get; set; }
    }
}
