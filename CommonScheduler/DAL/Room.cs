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
    
    public partial class Room
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Room()
        {
            this.Classes = new HashSet<Classes>();
        }
    
        public int ID { get; set; }
        public string NUMBER { get; set; }
        public Nullable<int> NUMBER_OF_PLACES { get; set; }
        public int Location_ID { get; set; }
        public Nullable<System.DateTime> DATE_MODIFIED { get; set; }
        public System.DateTime DATE_CREATED { get; set; }
        public int ID_CREATED { get; set; }
        public Nullable<int> ID_MODIFIED { get; set; }
        public string NUMBER_SHORT { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Classes> Classes { get; set; }
        public virtual Location Location { get; set; }
    }
}
