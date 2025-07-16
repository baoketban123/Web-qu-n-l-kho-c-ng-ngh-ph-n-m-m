using CNPM1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNPM1.Controllers
{
    public class SanPhamController : Controller
    {
        CNPM_WMS_DBEntities db = new CNPM_WMS_DBEntities();

        [HttpGet]
        public ActionResult XemSP()
        {
            var listSP = db.SanPhams.OrderByDescending(x => x.id).ToList();

            // Tính tổng số lượng đã xuất và số lượng còn lại cho mỗi sản phẩm
            foreach (var sanPham in listSP)
            {
                // Tính tổng số lượng đã xuất
                var totalExported = db.Sales
                    .Where(s => s.tenSPSale == sanPham.tenSP)
                    .AsEnumerable() 
                    .Sum(s => Convert.ToInt32(s.SLSale));

                ViewBag.ExportedQuantities = ViewBag.ExportedQuantities ?? new Dictionary<int, int>();
                ((Dictionary<int, int>)ViewBag.ExportedQuantities)[sanPham.id] = totalExported;

                // Tính số lượng còn lại
                var currentQuantity = Convert.ToInt32(sanPham.SLSP);
                var remainingQuantity = currentQuantity - totalExported;

                ViewBag.RemainingQuantities = ViewBag.RemainingQuantities ?? new Dictionary<int, int>();
                ((Dictionary<int, int>)ViewBag.RemainingQuantities)[sanPham.id] = remainingQuantity;
            }

            return View(listSP);
        }

        [HttpGet]
        public ActionResult TaoSP()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TaoSP(SanPham SP)
        {
            // Check xem xem sản phẩm đã tồn tại hay chưa
            bool productExists = db.SanPhams.Any(x => x.tenSP.ToLower() == SP.tenSP.ToLower());

            if (productExists)
            {
                ModelState.AddModelError("tenSP", "Tên sản phẩm đã tồn tại trong hệ thống");
                return View(SP);
            }

            if (ModelState.IsValid)
            {
                db.SanPhams.Add(SP);
                db.SaveChanges();
                TempData["ThongBao"] = "Nhập thành công!";
                return RedirectToAction("XemSP");
            }

            return View(SP);
        }

        [HttpGet]
        public ActionResult SuaSP(int id)
        {
            SanPham SP = db.SanPhams.Where(x => x.id == id).SingleOrDefault();
            return View(SP);
        }

        [HttpPost]
        public ActionResult SuaSP(int id, SanPham SP)
        {
            SanPham sp = db.SanPhams.Where(x => x.id == id).SingleOrDefault();
            sp.tenSP = SP.tenSP;
            sp.SLSP = SP.SLSP;
            db.SaveChanges();
            TempData["ThongBao"] = "Cập nhật thành công!";
            return RedirectToAction("XemSP");
        }

        [HttpGet]
        public ActionResult ChiTietSP(int id)
        {
            SanPham SP = db.SanPhams.Where(x => x.id == id).SingleOrDefault();
            return View(SP);
        }

        [HttpGet]
        public ActionResult XoaSP(int id)
        {
            SanPham SP = db.SanPhams.Where(x => x.id == id).SingleOrDefault();
            return View(SP);
        }

        [HttpPost]
        public ActionResult XoaSP(int id, SanPham SP)
        {
            SanPham sp = db.SanPhams.Where(x => x.id == id).SingleOrDefault();
            db.SanPhams.Remove(sp);
            db.SaveChanges();
            TempData["ThongBao"] = "Xóa thành công!";
            return RedirectToAction("XemSP");
        }

        // Hiển thị tên sản phẩm đã tồn tại
        public JsonResult GetProductSuggestions(string term)
        {
            var suggestions = db.SanPhams
                               .Where(p => p.tenSP.Contains(term))
                               .Select(p => p.tenSP)
                               .Distinct()
                               .ToList();
            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }

        // Tên sản phẩm đã tồn tại hay chưa
        [HttpPost]
        public JsonResult IsProductNameAvailable(string tenSP)
        {
            bool isAvailable = !db.SanPhams.Any(x => x.tenSP.ToLower() == tenSP.ToLower());
            return Json(isAvailable, JsonRequestBehavior.AllowGet);
        }
    }
}