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
    
    public partial class Account
    {
        public int StaffID { get; set; }
        public string StaffPassword { get; set; }
        public string StaffEmail { get; set; }
    
        public virtual Staff Staff { get; set; }
    }
}
