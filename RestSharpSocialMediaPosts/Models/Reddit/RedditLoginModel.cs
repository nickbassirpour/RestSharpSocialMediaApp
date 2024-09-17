using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSharpSocialMediaPosts.Models.Reddit
{
    public class RedditLoginModel
    {
        [Required] 
        public string ClientId { get; set; }
        [Required]
        public string ClientSecret { get; set; }
        public string Username { get; set; } = "devtestercsharp";
        public string Password { get; set; } = "Come2MyGarden";

    }
}
