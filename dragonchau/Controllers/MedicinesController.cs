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
            var model = db.Medicines.AsQueryable();

            // Lọc theo điều kiện
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(m =>
                    m.MedicineName.Contains(searchString) ||
                    m.Category.CategoryName.Contains(searchString) ||
                    m.Brand.BrandName.Contains(searchString));
            }

            return View(model.ToList());
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
        // POST: Medicines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MedicineID,MedicineName,MedicinePrice,MedicineBrand,MedicineExp,MedicineDescription,MedicineIngredients,Available,MedicinePrice_Sell,MedicineCate")] Medicine medicine, HttpPostedFileBase MedicineImg)
        {
            if (ModelState.IsValid)
            {
                // Fetch the existing entity from the database
                var existingMedicine = db.Medicines.Find(medicine.MedicineID);

                if (existingMedicine == null)
                {
                    return HttpNotFound();
                }

                // Update properties of the existing entity
                existingMedicine.MedicineName = medicine.MedicineName;
                existingMedicine.MedicinePrice = medicine.MedicinePrice;
                existingMedicine.MedicineBrand = medicine.MedicineBrand;
                existingMedicine.MedicineDescription = medicine.MedicineDescription;
                existingMedicine.MedicineIngredients = medicine.MedicineIngredients;
                existingMedicine.Available = medicine.Available;
                existingMedicine.MedicinePrice_Sell = medicine.MedicinePrice_Sell;
                existingMedicine.MedicineCate = medicine.MedicineCate;

                // Handle file upload if MedicineImg is provided
                if (MedicineImg != null && MedicineImg.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(MedicineImg.InputStream))
                    {
                        existingMedicine.MedicineImg = binaryReader.ReadBytes(MedicineImg.ContentLength);
                    }
                }

                // Restore MedicineExp from the existing entity
                existingMedicine.MedicineExp = medicine.MedicineExp;

                db.Entry(existingMedicine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // If ModelState is not valid, return to the edit view with validation errors
            ViewBag.MedicineBrand = new SelectList(db.Brands, "BrandID", "BrandName", medicine.MedicineBrand);
            return PartialView("EditPar", medicine);
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
            var today = DateTime.Today;
            var expiryThreshold = today.AddDays(7);
            if (name == null)
            {
                return Json(new { success = false, message = "Invalid medicine" }, JsonRequestBehavior.AllowGet);
            }

            var medicines = db.Medicines
                              .Where(cu => cu.MedicineName.Contains(name) && cu.MedicineExp > today && cu.MedicineExp > expiryThreshold)
                              .Select(me => new MedicineViewModel
                              {
                                  MedicineID = me.MedicineID,
                                  MedicineName = me.MedicineName,
                                  MedicineDescription = me.MedicineDescription
                              })
                              .ToList();

            return Json(medicines, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExpiredMedicines()
        {
            try
            {
                var today = DateTime.Today;
                var expiryThreshold = today.AddDays(7); // Ngày hết hạn trong vòng 7 ngày từ ngày hiện tại
                var expiredMedicines = db.Medicines.Where(m => m.MedicineExp <= today || m.MedicineExp <= expiryThreshold).ToList();
                return PartialView("_MedicineTablePartial", expiredMedicines);
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                return Content("Error: " + ex.Message); // For debugging purposes
            }
        }
        //them hang
        [HttpPost]
        public JsonResult CreateBrand(string BrandName, string BrandCountry)
        {
            // Kiểm tra xem brand với BrandName đã tồn tại chưa
            var existingBrand = db.Brands.FirstOrDefault(b => b.BrandName == BrandName);

            if (existingBrand != null)
            {
                return Json(new { success = false, message = "Thương hiệu đã tồn tại" }, JsonRequestBehavior.AllowGet);
            }

            // Nếu chưa tồn tại, tạo mới thương hiệu
            Brand brand = new Brand
            {
                BrandName = BrandName,
                BrandCountry = BrandCountry
            };

            db.Brands.Add(brand);
            db.SaveChanges();

            return Json(new { success = true, message = "Tạo thành công" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CreateCate(string CateName)
        {
            var existingCate= db.Categories.FirstOrDefault(b => b.CategoryName == CateName);

            if (existingCate != null)
            {
                return Json(new { success = false, message = "Danh mục đã tồn tại" }, JsonRequestBehavior.AllowGet);
            }

            // Nếu chưa tồn tại, tạo mới thương hiệu
            Category cate = new Category
            {
                CategoryName = CateName
            };

            db.Categories.Add(cate);
            db.SaveChanges();

            return Json(new { success = true, message = "Tạo thành công" }, JsonRequestBehavior.AllowGet);
        }

        //them hang
    }
}
