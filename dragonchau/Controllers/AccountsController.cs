using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using System.Web.Security;
using dragonchau.Models;

namespace dragonchau.Controllers
{
    public class AccountController : Controller
    {

        private dragonchauEntities db = new dragonchauEntities();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string staffName, string password)
        {
            if (ModelState.IsValid)
            {
                var account = db.Accounts.SingleOrDefault(a => a.Staff.StaffName == staffName && a.StaffPassword == password);
                if (account != null)
                {
                    // Tạo session hoặc cookie ở đây
                    Session["StaffID"] = account.StaffID;
                    return RedirectToAction("Index", "Staffs");
                }
                else
                {
                    TempData["WarningMessage"] = "sai thong tin";
                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }
    }
}