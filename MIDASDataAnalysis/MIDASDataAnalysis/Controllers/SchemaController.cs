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
    public class SchemaController : Controller
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
        // GET: /Schema/

        public ActionResult Index()
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            return View(db.tblDBSchemas.ToList());
        }

        //
        // GET: /Schema/Details/5

        public ActionResult Details(int id = 0)
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            tblDBSchema tbldbschema = db.tblDBSchemas.Find(id);
            if (tbldbschema == null)
            {
                return HttpNotFound();
            }
            return View(tbldbschema);
        }

        //
        // GET: /Schema/Create

        public ActionResult Create()
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            return View();
        }

        //
        // POST: /Schema/Create

        [HttpPost]
        public ActionResult Create(tblDBSchema tbldbschema)
        {
            var name_count = db.tblDBSchemas.Where(t => t.DBSchema == tbldbschema.DBSchema).Select(id => id.DBSchema).Count();
            if (name_count > 0)
            {
                return View(tbldbschema);
            }
            else if (ModelState.IsValid)
            {
                var max_id = db.tblDBSchemas.Where(t => t.Id == db.tblDBSchemas.Max(x => x.Id)).Select(id => id.Id).FirstOrDefault();

                tbldbschema.Id = max_id + 1;

                db.tblDBSchemas.Add(tbldbschema);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(tbldbschema);
        }

        //
        // GET: /Schema/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            tblDBSchema tbldbschema = db.tblDBSchemas.Find(id);
            if (tbldbschema == null)
            {
                return HttpNotFound();
            }
            return View(tbldbschema);
        }

        //
        // POST: /Schema/Edit/5

        [HttpPost]
        public ActionResult Edit(tblDBSchema tbldbschema)
        {
            var name_count = db.tblDBSchemas.Where(t => t.DBSchema == tbldbschema.DBSchema).Select(id => id.DBSchema).Count();
            if (name_count > 0)
            {
                return View(tbldbschema);
            }
            else if (ModelState.IsValid)
            {
                db.Entry(tbldbschema).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbldbschema);
        }

        //
        // GET: /Schema/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            tblDBSchema tbldbschema = db.tblDBSchemas.Find(id);
            if (tbldbschema == null)
            {
                return HttpNotFound();
            }
            return View(tbldbschema);
        }

        //
        // POST: /Schema/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            tblDBSchema tbldbschema = db.tblDBSchemas.Find(id);
            db.tblDBSchemas.Remove(tbldbschema);
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