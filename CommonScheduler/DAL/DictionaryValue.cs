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
    
    public partial class DictionaryValue
    {
        public int DV_ID { get; set; }
        public string VALUE { get; set; }
        public int VALUE_TYPE { get; set; }
        public System.DateTime DATE_CREATED { get; set; }
        public Nullable<System.DateTime> DATE_MODIFIED { get; set; }
        public int ID_CREATED { get; set; }
        public Nullable<int> ID_MODIFIED { get; set; }
        public int DICTIONARY_ID { get; set; }
    
        public virtual Dictionary Dictionary { get; set; }
    }
}
