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
    
    public partial class Major
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Major()
        {
            this.Subgroup = new HashSet<Subgroup>();
        }
    
        public int ID { get; set; }
        public string NAME { get; set; }
        public string SHORT_NAME { get; set; }
        public string WWW_HOME_PAGE { get; set; }
        public int MAJOR_DEGREE_DV_ID { get; set; }
        public Nullable<int> MAJOR_TYPE_DV_ID { get; set; }
        public System.DateTime DATE_CREATED { get; set; }
        public Nullable<System.DateTime> DATE_MODIFIED { get; set; }
        public int ID_CREATED { get; set; }
        public int DEPARTMENT_ID { get; set; }
    
        public virtual Department Department { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Subgroup> Subgroup { get; set; }
    }
}