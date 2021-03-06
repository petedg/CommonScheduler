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
    
    public partial class Classes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Classes()
        {
            this.ClassesWeek = new HashSet<ClassesWeek>();
            this.ClassesGroup = new HashSet<ClassesGroup>();
        }
    
        public int ID { get; set; }
        public System.DateTime START_DATE { get; set; }
        public System.DateTime END_DATE { get; set; }
        public int CLASSESS_TYPE_DV_ID { get; set; }
        public string SUBJECT_NAME { get; set; }
        public string SUBJECT_SHORT { get; set; }
        public int Room_ID { get; set; }
        public System.DateTime DATE_CREATED { get; set; }
        public Nullable<System.DateTime> DATE_MODIFIED { get; set; }
        public int ID_CREATED { get; set; }
        public Nullable<int> ID_MODIFIED { get; set; }
        public int TEACHER_ID { get; set; }
        public Nullable<int> SPECIALLOCATION_ID { get; set; }
        public int DAY_OF_WEEK { get; set; }
        public Nullable<int> EXTERNALTEACHER_ID { get; set; }
        public int SCOPE_LEVEL { get; set; }
    
        public virtual Room Room { get; set; }
        public virtual SpecialLocation SpecialLocation { get; set; }
        public virtual Teacher Teacher { get; set; }
        public virtual ExternalTeacher ExternalTeacher { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClassesWeek> ClassesWeek { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClassesGroup> ClassesGroup { get; set; }
    }
}
