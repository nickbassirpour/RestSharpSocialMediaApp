using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSharpSocialMediaPosts.Models.Tumblr
{
    public class TumblrAuthModel
    {
        public string client_id { get; set; } = "YdJusuzOdtRNCZfXpS6K4Mh0Ov4L4o9vpTh6SCVTiYqX5qdcsS";
        public string response_type { get; set; } = "code";
        public string scope { get; set; } = "basic write";
        public string state { get; set; } = Guid.NewGuid().ToString();
        public string? redirect_uri { get; set; }
    }
}
