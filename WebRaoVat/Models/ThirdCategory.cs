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
    
    public partial class ThirdCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ThirdCategory()
        {
            this.Posts = new HashSet<Post>();
        }
    
        public int third_cate_id { get; set; }
        public int seccond_cate_id_1 { get; set; }
        public string third_cate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Post> Posts { get; set; }
        public virtual SeccondCategory SeccondCategory { get; set; }
    }
}
