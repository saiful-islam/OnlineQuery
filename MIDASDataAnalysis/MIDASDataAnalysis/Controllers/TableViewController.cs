using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MIDASDataAnalysis.Models;

namespace MIDASDataAnalysis.Controllers
{
    public class TableViewController : Controller
    {
        private SQLDataViewerAuthenticationEntities db = new SQLDataViewerAuthenticationEntities();
        private bool validate()
        {
            try
            {
                if (Session["isAdmin"].ToString() == "")
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
        //
        // GET: /TableView/

        public ActionResult Index()
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            return View(db.tblDBTables.ToList());
        }

        //
        // GET: /TableView/Details/5

        public ActionResult Details(int id = 0)
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            tblDBTable tbldbtable = db.tblDBTables.Find(id);
            if (tbldbtable == null)
            {
                return HttpNotFound();
            }
            return View(tbldbtable);
        }

        //
        // GET: /TableView/Create

        public ActionResult Create()
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            return View();
        }

        //
        // POST: /TableView/Create

        [HttpPost]
        public ActionResult Create(tblDBTable tbldbtable)
        {
            var name_count = db.tblDBTables.Where(t => t.DBTableOrView == tbldbtable.DBTableOrView).Select(id => id.DBTableOrView).Count();
            if (name_count > 0)
            {
                return View(tbldbtable);
            }
            else if (ModelState.IsValid)
            {
                var max_id = db.tblDBTables.Where(t => t.Id == db.tblDBTables.Max(x => x.Id)).Select(id => id.Id).FirstOrDefault();

                tbldbtable.Id = max_id + 1;
                db.tblDBTables.Add(tbldbtable);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbldbtable);
        }

        //
        // GET: /TableView/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            tblDBTable tbldbtable = db.tblDBTables.Find(id);
            if (tbldbtable == null)
            {
                return HttpNotFound();
            }
            return View(tbldbtable);
        }

        //
        // POST: /TableView/Edit/5

        [HttpPost]
        public ActionResult Edit(tblDBTable tbldbtable)
        {
            var name_count = db.tblDBTables.Where(t => t.DBTableOrView == tbldbtable.DBTableOrView).Select(id => id.DBTableOrView).Count();
            if (name_count > 0)
            {
                return View(tbldbtable);
            }
            else if (ModelState.IsValid)
            {
                db.Entry(tbldbtable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbldbtable);
        }

        //
        // GET: /TableView/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            tblDBTable tbldbtable = db.tblDBTables.Find(id);
            if (tbldbtable == null)
            {
                return HttpNotFound();
            }
            return View(tbldbtable);
        }

        //
        // POST: /TableView/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            tblDBTable tbldbtable = db.tblDBTables.Find(id);
            db.tblDBTables.Remove(tbldbtable);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}