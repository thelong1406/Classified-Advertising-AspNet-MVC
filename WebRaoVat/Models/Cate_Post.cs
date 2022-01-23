using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRaoVat.Models
{
    public class Cate_Post
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public IEnumerable<Post> Ad_Posts { get; set; }
        public IEnumerable<Image> Images { get; set; }
    }
}