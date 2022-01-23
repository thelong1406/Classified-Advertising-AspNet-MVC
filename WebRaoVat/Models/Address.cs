using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRaoVat.Models
{
    public class Address
    {
        public IEnumerable<Ward> Wards { get; set; }
        public IEnumerable<District> Districts { get; set; }
        public IEnumerable<Province> Provinces { get; set; }
    }
}