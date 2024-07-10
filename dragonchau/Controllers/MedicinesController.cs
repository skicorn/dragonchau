using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using dragonchau.Models;
using Microsoft.Ajax.Utilities;

namespace dragonchau.Controllers
{
    public class MedicinesController : Controller
    {
        private dragonchauEntities db = new dragonchauEntities();

        // GET: Medicines
        public ActionResult Index(string searchString)
        {
            var medicines = db.Medicines.Include(m => m.Brand);

            if (!String.IsNullOrEmpty(searchString))
            {
                medicines = medicines.Where(m => m.MedicineName.Contains(searchString) ||
                                                 m.Brand.BrandName.Contains(searchString));
            }

            return View(medicines.ToList());
        }


        // GET: Medicines/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medicine medicine = db.Medicines.Find(id);
            if (medicine == null)
            {
                return HttpNotFound();
            }
            return PartialView("Detail", medicine);
        }


        // GET: Medicines/Create
        public ActionResult Create()
        {
            ViewBag.MedicineCate = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.UnitID = new SelectList(db.Units, "UnitID", "UnitName");
            ViewBag.MedicineBrand = new SelectList(db.Brands, "BrandID", "BrandName");
            return View();
        }

        // POST: Medicines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MedicineID,MedicineName,MedicinePrice,MedicineBrand,MedicineExp,MedicineDescription,MedicineIngredients,Available,UnitID,MedicinePrice_Sell,MedicineCate")] Medicine medicine, List<MedicineUnit> ConversionUnits, HttpPostedFileBase MedicineImg)
        {
            if (ModelState.IsValid)
            {
                if (MedicineImg != null && MedicineImg.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(MedicineImg.InputStream))
                    {
                        medicine.MedicineImg = binaryReader.ReadBytes(MedicineImg.ContentLength);
                    }
                }

                db.Medicines.Add(medicine);
                db.SaveChanges();

                if (ConversionUnits != null)
                {
                    foreach (var unit in ConversionUnits)
                    {
                        unit.UnitName = unitname(unit.UnitID);
                        unit.MedicineID = medicine.MedicineID;
                        db.MedicineUnits.Add(unit);
                    }
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            ViewBag.MedicineCate = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.UnitID = new SelectList(db.Units, "UnitID", "UnitName");
            ViewBag.MedicineBrand = new SelectList(db.Brands, "BrandID", "BrandName");
            return View(medicine);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medicine medicine = db.Medicines.Find(id);
            if (medicine == null)
            {
                return HttpNotFound();
            }
            ViewBag.MedicineBrand = new SelectList(db.Brands, "BrandID", "BrandName", medicine.MedicineBrand);
            return PartialView("EditPar", medicine); 
        }

        // POST: Medicines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MedicineID,MedicineName,MedicinePrice,MedicineBrand,MedicineExp,MedicineDescription,MedicineIngredients,Available,MedicinePrice_Sell,MedicineCate")] Medicine medicine, HttpPostedFileBase MedicineImg)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload if MedicineImg is provided
                if (MedicineImg != null && MedicineImg.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(MedicineImg.InputStream))
                    {
                        medicine.MedicineImg = binaryReader.ReadBytes(MedicineImg.ContentLength);
                    }
                }

                db.Entry(medicine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Index");
        }

        // GET: Medicines/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medicine medicine = db.Medicines.Find(id);
            if (medicine == null)
            {
                return HttpNotFound();
            }
            return View(medicine);
        }

        // POST: Medicines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Medicine medicine = db.Medicines.Find(id);
            var medicineunit = db.MedicineUnits.Where(u => u.MedicineID == id).ToList();
            var billDetails = db.BillDetails.Where(bd => bd.MedicineID == id).ToList();
            foreach (var billDetail in billDetails)
            {
                billDetail.MedicineID = null; // Set MedicineID to null or another placeholder value
                db.Entry(billDetail).State = EntityState.Modified;
            }

            foreach (var unit in medicineunit)
            {
                db.MedicineUnits.Remove(unit);
                db.SaveChanges();
            }
            db.Medicines.Remove(medicine);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public string unitname(int id)
        {
            return db.Units.Find(id).UnitName;
        }

        [HttpGet]
        public JsonResult SearchMedicines(string name)
        {
            if (name == null)
            {
                return Json(new { success = false, message = "Invalid medicine" }, JsonRequestBehavior.AllowGet);
            }

            var medicines = db.Medicines
                              .Where(cu => cu.MedicineName.Contains(name))
                              .Select(me => new MedicineViewModel
                              {
                                  MedicineID = me.MedicineID,
                                  MedicineName = me.MedicineName,
                                  MedicineDescription = me.MedicineDescription
                              })
                              .ToList();

            return Json(medicines, JsonRequestBehavior.AllowGet);
        }
    }
}
