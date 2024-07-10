using dragonchau.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dragonchau.Controllers
{
    public class BillDetailViewModel
    {
        public string MedicineName { get; set; }
        public int?Quantity { get; set; }
        public decimal?BillDetail_Price { get; set; }
        public decimal?BillDetail_Total { get; set; }
    }

}