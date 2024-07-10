using dragonchau.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dragonchau.Controllers
{
    public class HomeController : Controller
    {
        private dragonchauEntities db = new dragonchauEntities();
        public ActionResult Index()
        {
            ViewBag.totalOrder = Utils.TotalOrder(db);
            ViewBag.totalMonthly = Utils.MonthlyIncome(db);
            ViewBag.totalValue = Utils.TotalIncome(db);
            ViewBag.totalorder = Utils.CountOrder(db);
            return View();  
        }
        [HttpGet]
        public JsonResult GetMonthlySalesData()
        {
            int currentYear = DateTime.Now.Year;

            // Generate a list of all months for the current year
            var allMonths = Enumerable.Range(1, 12)
                .Select(month => new { Year = currentYear, Month = month })
                .ToList();

            // Get the grouped sales data
            var groupedData = db.Bills
                .Where(b => b.BillDateCreate.HasValue && b.BillDateCreate.Value.Year == currentYear)
                .GroupBy(b => new { Year = b.BillDateCreate.Value.Year, Month = b.BillDateCreate.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Total = g.Sum(b => b.Total) ?? 0,
                    TotalQuantity = g.Sum(b => b.TotalQuantity) ?? 0
                })
                .ToList();

            // Join the allMonths list with the groupedData to ensure all months are included
            var data = from month in allMonths
                       join gd in groupedData on new { month.Year, month.Month } equals new { gd.Year, gd.Month } into gj
                       from subdata in gj.DefaultIfEmpty()
                       select new
                       {
                           Year = month.Year,
                           Month = month.Month,
                           Total = subdata?.Total ?? 0,
                           TotalQuantity = subdata?.TotalQuantity ?? 0
                       };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetWeeklySalesData()
        {
            var bills = db.Bills
                .Where(b => b.BillDateCreate.HasValue)
                .ToList(); // Fetch data 

            var data = bills
                .GroupBy(b => b.BillDateCreate.Value.DayOfWeek)
                .Select(g => new
                {
                    Day = g.Key,
                    Total = g.Sum(b => b.Total) ?? 0,
                    TotalQuantity = g.Sum(b => b.TotalQuantity) ?? 0
                })
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}