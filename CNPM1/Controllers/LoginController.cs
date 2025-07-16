using CNPM1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace cnpm_ck.Controllers
{
    public class LoginController : Controller
    {
        CNPM_WMS_DBEntities db = new CNPM_WMS_DBEntities();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string userName, string passName)
        {
            if (ModelState.IsValid)
            {
                var nguoidung = db.TaiKhoans.FirstOrDefault(x => x.userName == userName && x.passName == passName);
                if (nguoidung != null)
                {
                    Session["user"] = userName;
                    Session["UserName"] = userName; 
                    Session["idrole"] = nguoidung.idRole;

                    if (nguoidung.idRole == 1)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    TempData["Error"] = "Tài khoản đăng nhập không đúng";
                    return View();
                }
            }
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(TaiKhoan cus)
        {
            //Check xem đã tồn tại tài khoản chưa
            var nguoidung = db.TaiKhoans.FirstOrDefault(x => x.userName == cus.userName);
            if (ModelState.IsValid)
            {
                if (nguoidung != null)
                {
                    ModelState.AddModelError(string.Empty, "Tên đăng kí đã được sử dụng");
                }
                if (ModelState.IsValid)
                {
                    //Role default khi chưa phân
                    if (cus.idRole == null)
                    {
                        cus.idRole = 2; // Default role
                    }

                    db.TaiKhoans.Add(cus);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    return View();
                }
            }
            return View();
        }

        public ActionResult DangXuat()
        {
            // Logout session ra
            Session.Remove("user");
            Session.Remove("UserName");
            Session.Remove("idrole");
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Login");
        }
    }
}