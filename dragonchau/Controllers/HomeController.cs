using dragonchau.Models;
using System;
using System.Collections.Generic;
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
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
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
                   
                    Session["StaffID"] = account.StaffID;
                    return RedirectToAction("Index", "Staffs");
                }
                else
                {
                    TempData["WarningMessage"] = "Sai thông tin đăng nhập";
                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }


    }
}