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
    
    public partial class Subgroup_L
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Subgroup_L()
        {
            this.Group_L = new HashSet<Group_L>();
            this.Subgroup_L1 = new HashSet<Subgroup_L>();
        }
    
        public long ID { get; set; }
        public string NAME { get; set; }
        public string SHORT_NAME { get; set; }
        public Nullable<long> SUBGROUP_TYPE_DV_ID { get; set; }
        public System.DateTime DATE_CREATED { get; set; }
        public Nullable<System.DateTime> DATE_MODIFIED { get; set; }
        public long ID_CREATED { get; set; }
        public long MAJOR_ID { get; set; }
        public long SUBGROUP_ID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Group_L> Group_L { get; set; }
        public virtual Major_L Major_L { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Subgroup_L> Subgroup_L1 { get; set; }
        public virtual Subgroup_L Subgroup_L2 { get; set; }
    }
}
