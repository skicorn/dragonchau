//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace dragonchau.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Staff
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Staff()
        {
            this.Bills = new HashSet<Bill>();
        }
    
        public int StaffID { get; set; }
        public string StaffName { get; set; }
        public string StaffPhone { get; set; }
        public string StaffEmail { get; set; }
        public Nullable<int> StaffSalary { get; set; }
        public string StaffIDnum { get; set; }
        public Nullable<System.DateTime> StaffDateCreate { get; set; }
        public Nullable<int> StaffRole { get; set; }
    
        public virtual Account Account { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bill> Bills { get; set; }
        public virtual Role Role { get; set; }
    }
}