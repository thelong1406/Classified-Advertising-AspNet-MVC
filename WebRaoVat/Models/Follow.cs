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
    
    public partial class Follow
    {
        public int id { get; set; }
        public string user_id { get; set; }
        public string follower { get; set; }
    
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}