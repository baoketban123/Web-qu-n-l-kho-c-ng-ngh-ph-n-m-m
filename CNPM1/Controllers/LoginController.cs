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
        public ActionResult Login(TaiKhoan cus)
        {
            if(ModelState.IsValid)
            {
                //Tìm khách hàng có tên đăng nhập và pass hợp lệ trong csdl
                var nguoidung= db.TaiKhoans.FirstOrDefault(x=>x.userName==cus.userName&&x.passName==cus.passName);
                if(nguoidung!=null)
                {
                    Session["user"]=cus.userName;
                    Session["idrole"]=cus.idRole;
                    if(nguoidung.idRole==1)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                        return RedirectToAction("About", "Home");
                }
                else
                {
                    TempData["Error"] = "Tài khoản đăng nhập không đúng";
                    return View();
                }
            }
            return RedirectToAction("Contact", "Home");
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(TaiKhoan cus)
        {
            //kt xem có người nào đã đăng kí với tên đăng nhập này hay chưa
            var nguoidung = db.TaiKhoans.FirstOrDefault(x => x.userName == cus.userName);
            if (ModelState.IsValid)
            {
                if (nguoidung != null)
                    ModelState.AddModelError(string.Empty, "Tên đăng kí đã được sử dụng");
                if (ModelState.IsValid)
                {
                    db.TaiKhoans.Add(cus);
                    db.SaveChanges();
                }
                else { return View(); }
            }
            return RedirectToAction("Login");
        }
        public ActionResult DangXuat()
        {
            //xóa session
            Session.Remove("user");
            //xóa session from authenticator
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}