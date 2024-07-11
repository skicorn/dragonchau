using System.Linq;
using System.Web.Mvc;
using System.Web;
using System;
using System.Web.Security;
using dragonchau.Models;

namespace dragonchau.Controllers
{
    public class LoginController : Controller
    {
        dragonchauEntities db = new dragonchauEntities();

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AuthenLogin(string userName, string userPass, bool? rememberMe)
        {
            // Kiểm tra nếu rememberMe không có giá trị thì gán mặc định là false
            bool rememberUser = rememberMe ?? false;

            var userStore = db.Staffs.FirstOrDefault(u => u.StaffPhone == userName && u.Account.StaffPassword == userPass);

            if (userStore == null)
            {
                ViewBag.ErrorLog = "Bạn đã nhập sai UserName hoặc PassWord";
                return View("Login");
            }
            else
            {
                // Chuyển đổi StaffRole từ int? sang string
                string userRole = userStore.StaffRole.HasValue ? userStore.StaffRole.Value.ToString() : string.Empty;

                // Tạo cookie
                var authTicket = new FormsAuthenticationTicket(
                    1, // phiên bản
                    userName, // tên người dùng
                    DateTime.Now, // thời gian phát hành
                    DateTime.Now.AddMinutes(30), // thời gian hết hạn
                    rememberUser, // lưu thông tin đăng nhập?
                    userRole // vai trò người dùng
                );

                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                if (rememberUser)
                {
                    cookie.Expires = authTicket.Expiration;
                }
                Response.Cookies.Add(cookie);

                // Lưu thông tin vào session
                Session["Email"] = userStore.StaffEmail;
                Session["StaffID"] = userStore.StaffID;
                Session["LastName"] = userStore.StaffName;
                Session["Role"] = userRole;
                Session["Phone"] = userStore.StaffPhone;

                return RedirectToAction("Index", "Home");
            }
        }


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login", "Login");
        }
    }
}
