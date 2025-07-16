using CNPM1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CNPM1.Controllers
{
    public class XuatController : Controller
    {

        CNPM_WMS_DBEntities db = new CNPM_WMS_DBEntities();
        [HttpGet]
        public ActionResult XemSP()
        {
            List<Sale> list = db.Sales.OrderByDescending(x => x.id).ToList();
            return View(list);
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