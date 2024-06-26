using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using dragonchau.Models;

namespace dragonchau.Controllers
{
    public class BillsController : Controller
    {
        private dragonchauEntities db = new dragonchauEntities();

        // GET: Bills
        public ActionResult Index()
        {
            var bills = db.Bills.Include(b => b.Customer).Include(b => b.Staff);
            return View(bills.ToList());
        }

        // GET: Bills/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bill bill = db.Bills.Find(id);
            if (bill == null)
            {
                return HttpNotFound();
            }
            return View(bill);
        }

        // GET: Bills/Create
        public ActionResult Create()
        {
            ViewBag.UnitID = new SelectList(db.Units, "UnitID", "UnitName");
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName");
            ViewBag.MedicineID = new SelectList(db.Medicines, "MedicineID", "MedicineName"); // Ensure this matches the name in the view
            return View();
        }

        // POST: Bills/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(List<BillDetail> BillDetails, [Bind(Include = "BillID,BillDateCreate,BillStatus,StaffID,CustomerID,Discount,Total,TotalQuantity")] Bill bill)
        {
            if (ModelState.IsValid)
            {
                string id = Guid.NewGuid().ToString();
                // Add BillDetails to database context
                if (BillDetails != null && BillDetails.Count > 0)
                {
                    foreach (var detail in BillDetails)
                    {
                        db.BillDetails.Add(detail);
                    }
                    db.SaveChanges();       
                }
                if (bill.Discount == null) bill.Discount = 0;
                bill.BillID = id;
                bill.BillStatus = true; 
                bill.BillDateCreate = DateTime.Now;
                bill.StaffID = 1; 

                db.Bills.Add(bill);
                db.SaveChanges();

                // Associate BillDetails with the newly created Bill
                if (BillDetails != null && BillDetails.Count > 0)
                {

                    decimal? billtotal = (decimal) 0;
                    int?totalquantity = 0;
                    foreach (var detail in BillDetails)
                    {
                        var payeachunit = price(detail.MedicineID, detail.UnitID);
                        detail.BillDetail_Price = payeachunit;
                        detail.BillDetail_Total = payeachunit * detail.Quantity;
                        //add to bill
                        billtotal = billtotal + detail.BillDetail_Total;
                        totalquantity = totalquantity + detail.Quantity;
                        detail.BillID = bill.BillID; // Assign the BillID to each BillDetail
                    }
                    db.SaveChanges();
                    updatetotal(id, billtotal, totalquantity);
                }

                return RedirectToAction("Index");
            }

            // If ModelState is not valid, repopulate ViewBag and return to view
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", bill.CustomerID);
            ViewBag.MedicineID = new SelectList(db.Medicines, "MedicineID", "MedicineName");
            ViewBag.UnitID = new SelectList(db.Units, "UnitID", "UnitName"); 
            ViewBag.StaffID = new SelectList(db.Staffs, "StaffID", "StaffName", bill.StaffID);
            return View(bill);
        }


        // GET: Bills/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bill bill = db.Bills.Find(id);
            if (bill == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", bill.CustomerID);
            ViewBag.StaffID = new SelectList(db.Staffs, "StaffID", "StaffName", bill.StaffID);
            return View(bill);
        }

        // POST: Bills/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BillID,BillDateCreate,BillStatus,StaffID,CustomerID,Discount,Total,TotalQuantity")] Bill bill)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bill).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", bill.CustomerID);
            ViewBag.StaffID = new SelectList(db.Staffs, "StaffID", "StaffName", bill.StaffID);
            return View(bill);
        }

        // GET: Bills/Delete/5
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //retrive medicine info
        [HttpGet]
        public JsonResult GetMedicineInfo(int?medid, int?unitid)
        {
            var unit = db.MedicineUnits.Find(medid, unitid);
            var med = db.Medicines.Find(medid);
            if (med != null && unit!=null)
            {
                return Json(new { success = true, price = med.MedicinePrice*unit.UnitConvert }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Medicine not found" }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetBillInfo(string billid)
        {
            if (string.IsNullOrEmpty(billid))
            {
                return Json(new { success = false, message = "Invalid information" }, JsonRequestBehavior.AllowGet);
            }

            var detail = db.BillDetails
                .Where(de => de.BillID == billid)
                .Select(de => new BillDetailViewModel
                {
                    MedicineID = de.MedicineID,
                    Quantity = de.Quantity,
                    BillDetail_Price = de.BillDetail_Price,
                    BillDetail_Total = de.BillDetail_Total
                })
                .ToList();

            if (detail == null || !detail.Any())
            {
                return Json(new { success = false, message = "No bill details found" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, detail }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUnitsForMedicine(int? medid)
        {
            if (medid == null)
            {
                return Json(new { success = false, message = "Invalid medicine ID" }, JsonRequestBehavior.AllowGet);
            }

            var units = db.MedicineUnits
                          .Where(mu => mu.MedicineID == medid)
                          .Select(mu => new { UnitID = mu.UnitID, UnitName = mu.Unit.UnitName }) // Assuming MedicineUnit has a navigation property to Unit
                          .ToList();

            return Json(new { success = true, units }, JsonRequestBehavior.AllowGet);
        }
        public decimal? price(int? medid, int? unitid)
        {
            var unit = db.MedicineUnits.Find(medid, unitid);
            var med = db.Medicines.Find(medid);
            if(med != null && unit!=null) return med.MedicinePrice * unit.UnitConvert;
            return null;
        }
        public void updatetotal (string id, decimal? total, int? quantity)
        {
            var bill = db.Bills.Find(id);
            bill.Total = total - bill.Discount;
            bill.TotalQuantity = quantity;
            db.SaveChanges();
        }
        [HttpGet]
        public ActionResult GetMedicineOptions(string searchTerm)
        {
            var medicines = db.Medicines
                .Where(m => m.MedicineName.Contains(searchTerm))
                .Select(m => new { Value = m.MedicineID, Text = m.MedicineName })
                .ToList();

            return Json(new { success = true, medicines }, JsonRequestBehavior.AllowGet);
        }
    }
}
