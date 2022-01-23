using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRaoVat.Models
{
    public class Detail_User
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Image { get; set; }
        public string Email { get; set; }
        public double Rated { get; set; }
        public System.DateTime Date_join { get; set; }
        public User User { get; set; }
        public int be_follow { get; set; }
        public int follow { get; set; }
        public Post Post { get; set; }
        
        public IEnumerable<Follow> Follower_list { get; set; }

        public IEnumerable<Post> Post_User { get; set; }
    }
}