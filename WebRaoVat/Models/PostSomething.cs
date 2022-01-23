using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebRaoVat.Models;

namespace WebRaoVat.Models
{
    public class PostSomething
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<SeccondCategory> SeccondCategories { get; set; }
        public IEnumerable<ThirdCategory> ThirdCategories { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
        public IEnumerable<BrandSelect> BrandSelects { get; set; }
        public IEnumerable<Image> Images { get; set; }
        public IEnumerable<Atribute> Atributes { get; set; }
        public IEnumerable<AbtributeAndValue> AbtributeAndValues { get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public int count { get; set; }
    }
}