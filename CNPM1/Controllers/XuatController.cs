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
        public ActionResult TaoSp()
        {
            var sanPhamList = db.SanPhams.Select(sp => sp.tenSP).ToList();
            ViewBag.SanPhamList = new SelectList(sanPhamList);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoSP([Bind(Include = "tenSPSale,SLSale,DateSale")] Sale sale)
        {
            if (ModelState.IsValid)
            {
                db.Sales.Add(sale);
                db.SaveChanges();
                return RedirectToAction("XemSP");
            }
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