using CNPM1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace CNPM1.Controllers
{
    public class XuatController : Controller
    {

        CNPM_WMS_DBEntities db = new CNPM_WMS_DBEntities();
        [HttpGet]
        public ActionResult XemSP(string fromDate, string toDate)
        {
            var sales = db.Sales.AsQueryable();

            // filter date ra
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime from = DateTime.Parse(fromDate);
                sales = sales.Where(s => s.DateSale >= from);
                ViewBag.FromDate = fromDate;
            }

            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime to = DateTime.Parse(toDate).AddDays(1);
                sales = sales.Where(s => s.DateSale < to);
                ViewBag.ToDate = toDate;
            }

            var listSales = sales.OrderByDescending(x => x.DateSale).ToList();

            return View(listSales);
        }

        [HttpGet]
        public ActionResult TaoSP()
        {
            //Tạo model với ngày hiện tại
            var model = new Sale
            {
                DateSale = DateTime.Now
            };

            var products = db.SanPhams.ToList();

            // Dropdown
            ViewBag.SanPhamList = new SelectList(products, "tenSP", "tenSP");

            // tính tồn kho
            var productStock = new Dictionary<string, int>();

            foreach (var product in products)
            {
                // tính tổng xuất
                var totalExported = db.Sales
                    .Where(s => s.tenSPSale == product.tenSP)
                    .AsEnumerable() //bug LINQ 
                    .Sum(s => Convert.ToInt32(s.SLSale ?? "0"));

                // tính tổng còn lại
                var currentQuantity = Convert.ToInt32(product.SLSP ?? "0");
                var remainingQuantity = currentQuantity - totalExported;

                productStock[product.tenSP] = Math.Max(0, remainingQuantity); // check không phải số âm
            }

            ViewBag.ProductStock = productStock;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoSP(Sale sale)
        {
            if (ModelState.IsValid)
            {
                sale.DateSale = DateTime.Now;

                var product = db.SanPhams.FirstOrDefault(p => p.tenSP == sale.tenSPSale);
                if (product != null)
                {
                    var totalExported = db.Sales
                        .Where(s => s.tenSPSale == sale.tenSPSale)
                        .AsEnumerable()
                        .Sum(s => Convert.ToInt32(s.SLSale ?? "0"));

                    var currentQuantity = Convert.ToInt32(product.SLSP ?? "0");
                    var remainingQuantity = currentQuantity - totalExported;
                    var requestedQuantity = Convert.ToInt32(sale.SLSale);

                    if (requestedQuantity > remainingQuantity)
                    {
                        ModelState.AddModelError("SLSale", $"Số lượng xuất ({requestedQuantity}) vượt quá số lượng tồn kho ({remainingQuantity})");

                        var products = db.SanPhams.ToList();
                        ViewBag.SanPhamList = new SelectList(products, "tenSP", "tenSP");

                        var productStock = new Dictionary<string, int>();
                        foreach (var p in products)
                        {
                            var exported = db.Sales
                                .Where(s => s.tenSPSale == p.tenSP)
                                .AsEnumerable()
                                .Sum(s => Convert.ToInt32(s.SLSale ?? "0"));
                            var current = Convert.ToInt32(p.SLSP ?? "0");
                            productStock[p.tenSP] = Math.Max(0, current - exported);
                        }
                        ViewBag.ProductStock = productStock;

                        return View(sale);
                    }
                }

                db.Sales.Add(sale);
                db.SaveChanges();
                TempData["ThongBao"] = "Xuất hàng thành công!";
                return RedirectToAction("XemSP");
            }

            var allProducts = db.SanPhams.ToList();
            ViewBag.SanPhamList = new SelectList(allProducts, "tenSP", "tenSP");

            var stockData = new Dictionary<string, int>();
            foreach (var product in allProducts)
            {
                var totalExported = db.Sales
                    .Where(s => s.tenSPSale == product.tenSP)
                    .AsEnumerable()
                    .Sum(s => Convert.ToInt32(s.SLSale ?? "0"));
                var currentQuantity = Convert.ToInt32(product.SLSP ?? "0");
                stockData[product.tenSP] = Math.Max(0, currentQuantity - totalExported);
            }
            ViewBag.ProductStock = stockData;

            return View(sale);
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Sale sale = db.Sales.Find(id);
            if (sale == null)
                return HttpNotFound();

            return View(sale);
        }

        [HttpGet]
        public ActionResult DetailsBaoCao(string fromDate, string toDate)
        {
            var sales = db.Sales.AsQueryable();

            // filter date ra
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime from = DateTime.Parse(fromDate);
                sales = sales.Where(s => s.DateSale >= from);
                ViewBag.FromDate = fromDate;
            }

            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime to = DateTime.Parse(toDate).AddDays(1);
                sales = sales.Where(s => s.DateSale < to);
                ViewBag.ToDate = toDate;
            }

            var listSales = sales.OrderByDescending(x => x.DateSale).ToList();

            // tính tổng
            ViewBag.TotalQuantity = listSales.Sum(s => Convert.ToInt32(s.SLSale));
            ViewBag.TotalRecords = listSales.Count();

            return View(listSales);
        }

        [HttpGet]

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Sale sale = db.Sales.Find(id);
            if (sale == null)
                return HttpNotFound();

            return View(sale);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,tenSPSale,SLSale,DateSale")] Sale sale)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sale).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("XemSP");
            }
            return View(sale);
        }
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Sale sale = db.Sales.Find(id);
            if (sale == null)
                return HttpNotFound();

            return View(sale);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sale sale = db.Sales.Find(id);
            db.Sales.Remove(sale);
            db.SaveChanges();
            return RedirectToAction("XemSP");
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