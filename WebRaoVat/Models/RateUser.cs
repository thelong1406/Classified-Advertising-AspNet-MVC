//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebRaoVat.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class RateUser
    {
        public int id { get; set; }
        public string who_rate_id { get; set; }
        public string who_be_rated_id { get; set; }
        public double rate { get; set; }
        public string comment { get; set; }
    
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}