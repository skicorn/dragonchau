using dragonchau.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dragonchau.Controllers
{
    public static class Utils
    {
        public static int CountOrder(dragonchauEntities db)
        {
            var today = DateTime.Today;
            var bills = db.Bills.Where(b => DbFunctions.TruncateTime(b.BillDateCreate) == today).ToList();
            return bills.Count;
        }

        public static decimal? TotalIncome(dragonchauEntities db)
        {
            var today = DateTime.Today;
            var totalValue = db.Bills
                             .Where(b => DbFunctions.TruncateTime(b.BillDateCreate) == today)
                             .Sum(b => (decimal?)b.Total) ?? 0;
            return totalValue;
        }

         public static decimal? MonthlyIncome(dragonchauEntities db)
        {
            var today = DateTime.Today;
            var currentMonth = today.Month;
            var currentYear = today.Year;
            var totalValue = db.Bills
                             .Where(b => b.BillDateCreate.HasValue && b.BillDateCreate.Value.Month == currentMonth && b.BillDateCreate.Value.Year == currentYear)
                             .Sum(b => (decimal?)b.Total) ?? 0;
            return totalValue;
        }
        public static int TotalOrder(dragonchauEntities db)
        {
            var today = DateTime.Today;
            var totalValue = db.Bills
                               .Where(b => DbFunctions.TruncateTime(b.BillDateCreate) == today)
                               .Count();
            return totalValue;
        }
        
        public static int TotalExpire(dragonchauEntities db)
        {
            var today = DateTime.Today;
            return db.Medicines.Where(m => m.MedicineExp <  today).Count();   
        }
    }
}