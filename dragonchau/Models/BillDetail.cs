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
    
    public partial class BillDetail
    {
        public int DetailID { get; set; }
        public string BillID { get; set; }
        public Nullable<int> MedicineID { get; set; }
        public Nullable<int> UnitID { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> BillDetail_Total { get; set; }
        public Nullable<decimal> BillDetail_Price { get; set; }
    
        public virtual Bill Bill { get; set; }
        public virtual Medicine Medicine { get; set; }
        public virtual Unit Unit { get; set; }
    }
}