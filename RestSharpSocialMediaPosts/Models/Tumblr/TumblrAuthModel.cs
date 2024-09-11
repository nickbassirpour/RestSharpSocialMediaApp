using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSharpSocialMediaPosts.Models.Tumblr
{
    public class TumblrAuthModel
    {
        public string oauthConsumerKey { get; set; }
        public string oauthConsumerSecret { get; set; }
        public string oauthNonce { get; set; }
        public string oauthSignatureMethod { get; set; }
        public string oauthTimeStamp { get; set; }
        public string version { get; set; }
        public string oauthSignature { get; set; }



    }
}
