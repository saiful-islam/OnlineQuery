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
    public class ServerController : Controller
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
        // GET: /Server/

        public ActionResult Index()
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            return View(db.tblDBServers.ToList());
        }

        //
        // GET: /Server/Details/5

        public ActionResult Details(int id = 0)
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            tblDBServer tbldbserver = db.tblDBServers.Find(id);
            if (tbldbserver == null)
            {
                return HttpNotFound();
            }
            return View(tbldbserver);
        }

        //
        // GET: /Server/Create

        public ActionResult Create()
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            return View();
        }

        //
        // POST: /Server/Create

        [HttpPost]
        public ActionResult Create(tblDBServer tbldbserver)
        {
            try
            {
                DataAccess dbconn = new DataAccess(tbldbserver.DBServer);
                String strQuery = @"SELECT ' ' name
                                    UNION ALL
                                    SELECT name
                                    FROM
	                                    sys.sysdatabases";
                dbconn.MIDASData(strQuery);
                ViewBag.exMessage = "";
            }
            catch (Exception ex)
            {
                ViewBag.exMessage = "Need permission to add this server";
                return View(tbldbserver);
            }
            var name_count = db.tblDBServers.Where(t => t.DBServer == tbldbserver.DBServer).Select(id => id.DBServer).Count();
            if (name_count > 0)
            {
                return View(tbldbserver);
            }
            else if (ModelState.IsValid)
            {
                var max_id = db.tblDBServers.Where(t => t.Id == db.tblDBServers.Max(x => x.Id)).Select(id => id.Id).FirstOrDefault();

                tbldbserver.Id = max_id + 1;
                db.tblDBServers.Add(tbldbserver);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbldbserver);
        }

        

        //
        // GET: /Server/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            tblDBServer tbldbserver = db.tblDBServers.Find(id);
            if (tbldbserver == null)
            {
                return HttpNotFound();
            }
            return View(tbldbserver);
        }

        //
        // POST: /Server/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            tblDBServer tbldbserver = db.tblDBServers.Find(id);
            db.tblDBServers.Remove(tbldbserver);
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