using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using dragonchau.Models;
using PagedList.Mvc;
using PagedList;

using System.Drawing.Printing;
using Google.Protobuf.Compiler;
using IronPdf;
using Grpc.Core;

namespace dragonchau.Controllers
{
    public class StaffsController : Controller
    {
        private dragonchauEntities db = new dragonchauEntities();


        // GET: Staffs
        public ActionResult Index(string SearchString, int? i,string CateString)
        {	// SearchString
            
            var staffs = db.Staffs.Include(p => p.Account);

            if (!String.IsNullOrEmpty(CateString))
            {

                if (int.TryParse(CateString, out int cateInt))
                {
                    staffs = staffs.Where(s => s.StaffRole == cateInt);
                }
                else
                {
                    // Xử lý khi CateString không thể chuyển đổi sang int
                    TempData["WarningMessage"] = "CateString không hợp lệ.";
                }
            }
          

            return View(staffs.ToList().ToPagedList(i ?? 1, 10));

        }


        // GET: Staffs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // GET: Staffs/Create
        public ActionResult Create()
        {
            ViewBag.StaffID = new SelectList(db.Accounts, "StaffID", "StaffPassword");
            ViewBag.StaffRole = new SelectList(db.Roles, "RoleID", "RoleName");
            return View();
        } 

        // POST: Staffs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StaffID,StaffName,StaffPhone,StaffEmail,StaffSalary,StaffIDnum,StaffDateCreate,StaffRole")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                db.Staffs.Add(staff);
                db.SaveChanges();
                TempData["SuccessMessage"] = "";
                return RedirectToAction("Index");
            }

            ViewBag.StaffID = new SelectList(db.Accounts, "StaffID", "StaffPassword", staff.StaffID);
            ViewBag.StaffRole = new SelectList(db.Roles, "RoleID", "RoleName", staff.StaffRole);
            return View(staff);
        }

        // GET: Staffs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            ViewBag.StaffID = new SelectList(db.Accounts, "StaffID", "StaffPassword", staff.StaffID);
            ViewBag.StaffRole = new SelectList(db.Roles, "RoleID", "RoleName", staff.StaffRole);

            return View(staff);
        }

        // POST: Staffs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StaffID,StaffName,StaffPhone,StaffEmail,StaffSalary,StaffIDnum,StaffDateCreate,StaffRole")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                db.Entry(staff).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "";
                return RedirectToAction("Index");
            }
            ViewBag.StaffID = new SelectList(db.Accounts, "StaffID", "StaffPassword", staff.StaffID);
            ViewBag.StaffRole = new SelectList(db.Roles, "RoleID", "RoleName", staff.StaffRole);
            return View(staff);
        }

        // GET: Staffs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Staff staff = db.Staffs.Find(id);
            db.Staffs.Remove(staff);
            db.SaveChanges();
            TempData["SuccessMessage"] = "";
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
       
    }
}
