using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRaoVat.Models
{
    public class FollowList
    {
        public IEnumerable<User> Follow { get; set; }
        public IEnumerable<User> Follower { get; set; }
    }
}