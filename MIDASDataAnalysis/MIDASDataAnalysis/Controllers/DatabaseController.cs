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
    public class DatabaseController : Controller
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
        // GET: /Database/

        public ActionResult Index()
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            return View(db.tblDBNames.ToList());
        }

        //
        // GET: /Database/Details/5

        public ActionResult Details(int id = 0)
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            tblDBName tbldbname = db.tblDBNames.Find(id);
            if (tbldbname == null)
            {
                return HttpNotFound();
            }
            return View(tbldbname);
        }

        //
        // GET: /Database/Create

        public ActionResult Create()
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            return View();
        }

        //
        // POST: /Database/Create

        [HttpPost]
        public ActionResult Create(tblDBName tbldbname)
        {
            var name_count = db.tblDBNames.Where(t => t.DBName == tbldbname.DBName).Select(id => id.DBName).Count();
            if (name_count > 0)
            {
                return View(tbldbname);
            }
            else if (ModelState.IsValid)
            {
                var max_id = db.tblDBNames.Where(t => t.Id == db.tblDBNames.Max(x => x.Id)).Select(id => id.Id).FirstOrDefault();
                
                tbldbname.Id = max_id + 1;
                db.tblDBNames.Add(tbldbname);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbldbname);
        }

        //
        // GET: /Database/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            tblDBName tbldbname = db.tblDBNames.Find(id);
            if (tbldbname == null)
            {
                return HttpNotFound();
            }
            return View(tbldbname);
        }

        //
        // POST: /Database/Edit/5

        [HttpPost]
        public ActionResult Edit(tblDBName tbldbname)
        {
            var name_count = db.tblDBNames.Where(t => t.DBName == tbldbname.DBName).Select(id => id.DBName).Count();
            if (name_count > 0)
            {
                return View(tbldbname);
            }
            else if (ModelState.IsValid)
            {
                db.Entry(tbldbname).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbldbname);
        }

        //
        // GET: /Database/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            tblDBName tbldbname = db.tblDBNames.Find(id);
            if (tbldbname == null)
            {
                return HttpNotFound();
            }
            return View(tbldbname);
        }

        //
        // POST: /Database/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            tblDBName tbldbname = db.tblDBNames.Find(id);
            db.tblDBNames.Remove(tbldbname);
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