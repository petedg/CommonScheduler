//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CommonScheduler.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Group_L
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Group_L()
        {
            this.Classes_L = new HashSet<Classes_L>();
        }
    
        public long ID { get; set; }
        public string NAME { get; set; }
        public string SHORT_NAME { get; set; }
        public System.DateTime DATE_CREATED { get; set; }
        public Nullable<System.DateTime> DATE_MODIFIED { get; set; }
        public long ID_CREATED { get; set; }
        public long SUBGROUP_ID { get; set; }
    
        public virtual Subgroup_L Subgroup_L { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Classes_L> Classes_L { get; set; }
    }
}