using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSharpSocialMediaPosts.Models.Reddit
{
    public class RedditLoginModel
    {

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Username { get; set; } = "devtestercsharp";
        public string Password { get; set; } = "Come2MyGarden";

    }
}
