using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRaoVat.Models
{
    public class AttributeVal
    {
        public int id { get; set; }
        public string value { get; set; }
        public int post_id { get; set; }
        public int atribute_id { get; set; }
        public string atribute_name { get; set; }
    }
}