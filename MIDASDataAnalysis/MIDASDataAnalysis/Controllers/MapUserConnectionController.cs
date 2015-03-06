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
    public class MapUserConnectionController : Controller
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
        // GET: /MapUserConnection/

        public ActionResult Index()
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            var mapuserconnections = db.MapUserConnections.Include(m => m.tblConnection).Include(m => m.UserProfile);
            return View(mapuserconnections.ToList());
        }
        //
        // GET: /MapUsersConnections/Details/5

        public ActionResult Details(int id = 0)
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "MIDAS");
            }
            MapUserConnection mapuserconnection = db.MapUserConnections.Find(id);
            if (mapuserconnection == null)
            {
                return HttpNotFound();
            }
            return View(mapuserconnection);
        }

        //
        // GET: /MapUsersConnections/Create

        public ActionResult Create()
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "MIDAS");
            }
            ViewBag.ConnID = new SelectList(db.tblConnections, "ConnId", "ConnectionName");
            ViewBag.UserID = new SelectList(db.UserProfiles, "UserId", "UserName");
            return View();
        }

        //
        // POST: /MapUsersConnections/Create

        [HttpPost]
        public ActionResult Create(MapUserConnection mapuserconnection)
        {
            var count_id = db.MapUserConnections.Where(t => t.ConnID == mapuserconnection.ConnID
                                                        && t.UserID == mapuserconnection.UserID
                                                        ).Select(id => id.Id).Count();
            if (count_id > 0)
            {
                ViewBag.ConnID = new SelectList(db.tblConnections, "ConnId", "ConnectionName", mapuserconnection.ConnID);
                ViewBag.UserID = new SelectList(db.UserProfiles, "UserId", "UserName", mapuserconnection.UserID);
                return View(mapuserconnection);
            }
            else if (ModelState.IsValid)
            {
                var max_id = db.MapUserConnections.Where(t => t.Id == db.MapUserConnections.Max(x => x.Id)).Select(id => id.Id).FirstOrDefault();

                mapuserconnection.Id = max_id + 1;
                db.MapUserConnections.Add(mapuserconnection);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ConnID = new SelectList(db.tblConnections, "ConnId", "ConnectionName", mapuserconnection.ConnID);
            ViewBag.UserID = new SelectList(db.UserProfiles, "UserId", "UserName", mapuserconnection.UserID);
            return View(mapuserconnection);
        }

        //
        // GET: /MapUsersConnections/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "MIDAS");
            }
            MapUserConnection mapuserconnection = db.MapUserConnections.Find(id);
            if (mapuserconnection == null)
            {
                return HttpNotFound();
            }
            ViewBag.ConnID = new SelectList(db.tblConnections, "ConnId", "ConnectionName", mapuserconnection.ConnID);
            ViewBag.UserID = new SelectList(db.UserProfiles, "UserId", "UserName", mapuserconnection.UserID);
            return View(mapuserconnection);
        }

        //
        // POST: /MapUsersConnections/Edit/5

        [HttpPost]
        public ActionResult Edit(MapUserConnection mapuserconnection)
        {
            var count_id = db.MapUserConnections.Where(t => t.ConnID == mapuserconnection.ConnID
                                                        && t.UserID == mapuserconnection.UserID
                                                        ).Select(id => id.Id).Count();
            if (count_id > 0)
            {
                ViewBag.ConnID = new SelectList(db.tblConnections, "ConnId", "ConnectionName", mapuserconnection.ConnID);
                ViewBag.UserID = new SelectList(db.UserProfiles, "UserId", "UserName", mapuserconnection.UserID);
                return View(mapuserconnection);
            }
            else if (ModelState.IsValid)
            {
                db.Entry(mapuserconnection).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ConnID = new SelectList(db.tblConnections, "ConnId", "ConnectionName", mapuserconnection.ConnID);
            ViewBag.UserID = new SelectList(db.UserProfiles, "UserId", "UserName", mapuserconnection.UserID);
            return View(mapuserconnection);
        }

        //
        // GET: /MapUsersConnections/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "MIDAS");
            }
            MapUserConnection mapuserconnection = db.MapUserConnections.Find(id);
            if (mapuserconnection == null)
            {
                return HttpNotFound();
            }
            return View(mapuserconnection);
        }

        //
        // POST: /MapUsersConnections/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            MapUserConnection mapuserconnection = db.MapUserConnections.Find(id);
            db.MapUserConnections.Remove(mapuserconnection);
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
