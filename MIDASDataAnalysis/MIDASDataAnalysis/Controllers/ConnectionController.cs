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
    public class ConnectionController : Controller
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
        // GET: /Connection/

        public ActionResult Index()
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            var tblconnections = db.tblConnections.Include(t => t.tblDBName).Include(t => t.tblDBSchema).Include(t => t.tblDBServer).Include(t => t.tblDBTable);
            return View(tblconnections.ToList());

        }
         //
        // GET: /Connection/

        public ActionResult UserSaveStates()
        {
            DataAccess dbAccess = new DataAccess();
            String strQuery = @"SELECT us.StateName,c.ConnectionName,us.TableName
                                FROM tblUserSaveStates us
                                INNER JOIN tblConnection c
                                on us.ConnectionID=c.ConnId
                                INNER JOIN MapUserConnection mcu
                                on c.ConnId=mcu.ConnId
                                INNER JOIN UserProfile u
                                on mcu.UserID=u.UserId
                                WHERE u.UserName='" + User.Identity.Name + "'";
            DataTable dt = dbAccess.SQLViewerData(strQuery);
            ViewBag.listofStates = dt;

            strQuery = @"SELECT us.StateName,c.ConnectionName
                                FROM tblUserSaveScript us
                                INNER JOIN tblConnection c
                                on us.ConnectionID=c.ConnId
                                INNER JOIN MapUserConnection mcu
                                on c.ConnId=mcu.ConnId
                                INNER JOIN UserProfile u
                                on mcu.UserID=u.UserId
                                WHERE u.UserName='" + User.Identity.Name + "'";
            dt = dbAccess.SQLViewerData(strQuery);
            ViewBag.listofScript = dt;

            return View();

        }
        

        //
        // GET: /Connection/Details/5

        public ActionResult Details(int id = 0)
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            tblConnection tblconnection = db.tblConnections.Find(id);
            if (tblconnection == null)
            {
                return HttpNotFound();
            }
            return View(tblconnection);
        }


        public JsonResult getDatabaseName(List<string> serverName)
        {
            try
            {
                DataAccess dbAccess = new DataAccess(serverName[0]);
                String strQuery = @"SELECT ' ' name
                                    UNION ALL
                                    SELECT name
                                    FROM
	                                    sys.sysdatabases";
                DataTable dt = dbAccess.GetMasterData(strQuery);
                strQuery = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strQuery += "," + dt.Rows[i][0].ToString();
                }
                strQuery = strQuery.Substring(1, strQuery.Length - 1);
                return Json(strQuery);
            }
            catch (Exception ex)
            {
                return Json("~," + ex.Message);
            }
        }

        public JsonResult getSchemaName(List<string> DBName)
        {
            try
            {
                DataAccess dbAccess = new DataAccess(DBName[0], DBName[1]);
                String strQuery = @"SELECT ' ' name
                                    UNION ALL
                                    SELECT '[ALL]' name
                                    UNION ALL
                                    SELECT name
                                    from sys.schemas";
                DataTable dt = dbAccess.MIDASData(strQuery);
                strQuery = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strQuery += "," + dt.Rows[i][0].ToString();
                }
                strQuery = strQuery.Substring(1, strQuery.Length - 1);
                return Json(strQuery);
            }
            catch (Exception ex)
            {
                return Json("~," + ex.Message);
            }
        }

        public JsonResult getTableName(List<string> Schema)
        {
            try
            {
                DataAccess dbAccess = new DataAccess(Schema[0], Schema[1]);
                String strQuery = "";
                if (Schema[2] == "[ALL]")
                {
                    strQuery = @" SELECT '[ALL]' name
                                UNION ALL
                                SELECT t.name
                                FROM
	                                sys.tables AS t
	                                INNER JOIN sys.schemas AS s
		                                ON t.[schema_id] = s.[schema_id]

                                UNION ALL
                                SELECT t.name
                                FROM
	                                sys.views AS t
	                                INNER JOIN sys.schemas AS s
		                                ON t.[schema_id] = s.[schema_id]";
                }
                else
                {
                    strQuery = @"SELECT '[ALL]' name
                                UNION ALL
                                SELECT t.name
                                FROM
	                                sys.tables AS t
	                                INNER JOIN sys.schemas AS s
		                                ON t.[schema_id] = s.[schema_id]
                                WHERE
	                                s.name = N'" + Schema[2] + "'";
                    strQuery += @"UNION ALL
                                SELECT t.name
                                FROM
	                                sys.views AS t
	                                INNER JOIN sys.schemas AS s
		                                ON t.[schema_id] = s.[schema_id]
                                WHERE
	                                s.name = N'" + Schema[2] + "'";
                }
                DataTable dt = dbAccess.MIDASData(strQuery);
                strQuery = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strQuery += "," + dt.Rows[i][0].ToString();
                }
                strQuery = strQuery.Substring(1, strQuery.Length - 1);
                return Json(strQuery);
            }
            catch (Exception ex)
            {
                return Json("~," + ex.Message);
            }
        }

        //
        // GET: /Connection/Create

        public ActionResult Create()
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            ViewBag.DBId = new SelectList(db.tblDBNames, "Id", "DBName");
            ViewBag.DBSchemaId = new SelectList(db.tblDBSchemas, "Id", "DBSchema");
            ViewBag.DBServerId = new SelectList(db.tblDBServers, "Id", "DBServer");
            ViewBag.DBTableId = new SelectList(db.tblDBTables, "Id", "DBTableOrView");
            return View();
        }


        //
        // POST: /Connection/Create

        [HttpPost]
        public JsonResult Create(List<string> connection)
        {
            try
            {
               
                tblConnection tblconnection = new tblConnection();

                DataAccess dbAccess = new DataAccess();
                String strQuery = @"SELECT Id
                                    FROM tblDBServer
                                    WHERE DBServer='" + connection[0] + "'";

                DataTable dt = dbAccess.SQLViewerData(strQuery);

                tblconnection.DBServerId = (dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0][0]) : -1;
                strQuery = @"SELECT Id
                                    FROM tblDBName
                                    WHERE DBName='" + connection[1] + "'";

                dt = dbAccess.SQLViewerData(strQuery);
                tblconnection.DBId = (dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0][0]) : -1;

                strQuery = @"SELECT Id
                                    FROM tblDBSchema
                                    WHERE DBSchema='" + connection[2] + "'";

                dt = dbAccess.SQLViewerData(strQuery);
                tblconnection.DBSchemaId = (dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0][0]) : -1;

                strQuery = @"SELECT Id
                                    FROM tblDBTable
                                    WHERE DBTableOrView='" + connection[3] + "'";

                dt = dbAccess.SQLViewerData(strQuery);
                tblconnection.DBTableId = (dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0][0]) : -1;

                tblconnection.ConnectionName = connection[4];
                //tblconnection.DBServerId = db.tblDBServers.Where(t => t.DBServer.ToString().Contains(connection[0])).Select(id => id.Id).FirstOrDefault();
                //tblconnection.DBId = db.tblDBNames.Where(t => t.DBName == connection[1]).Select(id => id.Id).FirstOrDefault();
                //tblconnection.DBSchemaId = db.tblDBSchemas.Where(t => t.DBSchema == connection[0]).Select(id => id.Id).FirstOrDefault();
                //tblconnection.DBTableId = db.tblDBTables.Where(t => t.DBTableOrView == connection[0]).Select(id => id.Id).FirstOrDefault();

                var count_id = db.tblConnections.Where(t => t.DBServerId == tblconnection.DBServerId
                                                            && t.DBId == tblconnection.DBId
                                                            && t.DBSchemaId == tblconnection.DBSchemaId
                                                            && t.DBTableId == tblconnection.DBTableId
                                                            ).Select(id => id.ConnId).Count();
                var name_count = db.tblConnections.Where(t => t.ConnectionName == tblconnection.ConnectionName).Select(id => id.ConnectionName).Count();
                if (count_id > 0 || name_count > 0)
                {
                    return Json("~,This Connection already exists");
                }
                tblDBServer tbldbserver = new tblDBServer();
                tbldbserver.DBServer = connection[0];

                name_count = db.tblDBServers.Where(t => t.DBServer == tbldbserver.DBServer).Select(id => id.DBServer).Count();
                if (name_count == 0)
                {
                    var max_id = db.tblDBServers.Where(t => t.Id == db.tblDBServers.Max(x => x.Id)).Select(id => id.Id).FirstOrDefault();

                    tbldbserver.Id = max_id + 1;
                    db.tblDBServers.Add(tbldbserver);
                    db.SaveChanges();

                    tblconnection.DBServerId = max_id + 1;
                }

                tblDBName tbldbname = new tblDBName();
                tbldbname.DBName = connection[1];

                name_count = db.tblDBNames.Where(t => t.DBName == tbldbname.DBName).Select(id => id.DBName).Count();
                if (name_count == 0)
                {
                    //var context = new SQLDataViewerAuthenticationEntities();
                    //context.Entry(tbldbname).State = EntityState.Added;

                    var max_id = db.tblDBNames.Where(t => t.Id == db.tblDBNames.Max(x => x.Id)).Select(id => id.Id).FirstOrDefault();

                    tbldbname.Id = max_id + 1;
                    db.tblDBNames.Add(tbldbname);
                    db.SaveChanges();

                    tblconnection.DBId = max_id + 1;
                }

                tblDBSchema tbldbschema = new tblDBSchema();
                tbldbschema.DBSchema = connection[2];

                name_count = db.tblDBSchemas.Where(t => t.DBSchema == tbldbschema.DBSchema).Select(id => id.DBSchema).Count();
                if (name_count == 0)
                {
                    var max_id = db.tblDBSchemas.Where(t => t.Id == db.tblDBSchemas.Max(x => x.Id)).Select(id => id.Id).FirstOrDefault();

                    tbldbschema.Id = max_id + 1;

                    db.tblDBSchemas.Add(tbldbschema);
                    db.SaveChanges();

                    tblconnection.DBSchemaId = max_id + 1;
                }

                tblDBTable tbldbtable = new tblDBTable();
                tbldbtable.DBTableOrView = connection[3];

                name_count = db.tblDBTables.Where(t => t.DBTableOrView == tbldbtable.DBTableOrView).Select(id => id.DBTableOrView).Count();
                if (name_count == 0)
                {
                    var max_id = db.tblDBTables.Where(t => t.Id == db.tblDBTables.Max(x => x.Id)).Select(id => id.Id).FirstOrDefault();

                    tbldbtable.Id = max_id + 1;
                    db.tblDBTables.Add(tbldbtable);
                    db.SaveChanges();
                    tblconnection.DBTableId = max_id + 1;
                }

                {
                    var max_id = db.tblConnections.Where(t => t.ConnId == db.tblConnections.Max(x => x.ConnId)).Select(id => id.ConnId).FirstOrDefault();

                    tblconnection.ConnId = max_id + 1;
                    db.tblConnections.Add(tblconnection);
                    db.SaveChanges();
                    
                }

                return Json("Successfully Saved");
            }
            catch (Exception ex)
            {
                return Json("~," + ex.Message);
            }
        }
        // GET: /Connection/Rename/5//

        public ActionResult Rename(int id = 0)
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }
            tblConnection tblconnection = db.tblConnections.Find(id);
            if (tblconnection == null)
            {
                return HttpNotFound();
            }
            ViewBag.DBId = new SelectList(db.tblDBNames, "Id", "DBName", tblconnection.DBId);
            ViewBag.DBSchemaId = new SelectList(db.tblDBSchemas, "Id", "DBSchema", tblconnection.DBSchemaId);
            ViewBag.DBServerId = new SelectList(db.tblDBServers, "Id", "DBServer", tblconnection.DBServerId);
            ViewBag.DBTableId = new SelectList(db.tblDBTables, "Id", "DBTableOrView", tblconnection.DBTableId);
            

            
            

            return View(tblconnection);
        }

        //
        // POST: /Connection/Rename/5

        [HttpPost]
        public ActionResult Rename(tblConnection tblconnection)
        {

            var name_count = db.tblConnections.Where(t => t.ConnectionName == tblconnection.ConnectionName).Select(id => id.ConnectionName).Count();
            if (name_count > 0)
            {
                ViewBag.DBId = new SelectList(db.tblDBNames, "Id", "DBName", tblconnection.DBId);
                ViewBag.DBSchemaId = new SelectList(db.tblDBSchemas, "Id", "DBSchema", tblconnection.DBSchemaId);
                ViewBag.DBServerId = new SelectList(db.tblDBServers, "Id", "DBServer", tblconnection.DBServerId);
                ViewBag.DBTableId = new SelectList(db.tblDBTables, "Id", "DBTableOrView", tblconnection.DBTableId);
                return View(tblconnection);
            }
            else if (ModelState.IsValid)
            {
                db.Entry(tblconnection).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DBId = new SelectList(db.tblDBNames, "Id", "DBName", tblconnection.DBId);
            ViewBag.DBSchemaId = new SelectList(db.tblDBSchemas, "Id", "DBSchema", tblconnection.DBSchemaId);
            ViewBag.DBServerId = new SelectList(db.tblDBServers, "Id", "DBServer", tblconnection.DBServerId);
            ViewBag.DBTableId = new SelectList(db.tblDBTables, "Id", "DBTableOrView", tblconnection.DBTableId);
            return View(tblconnection);
        }

        // GET: /Connection/Edit/5//

        public ActionResult Error()
        {
            return View();
        }

        //
        // GET: /Connection/Edit/5//

        public ActionResult Edit(int id = 0)
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }

            DataAccess dbAccess = new DataAccess();
            String strQuery = @"select *
                                from
	                                (
	                                select ConnectionID,count(*) as NoOfRows
	                                from dbo.tblUserSaveStates
	                                where ConnectionID=" + id;
            strQuery += @"  group by ConnectionID

	                                union all

	                                select ConnectionID,count(*) as NoOfRows 
	                                from dbo.tblUserSaveScript
	                                where ConnectionID=" + id + "  group by ConnectionID) as tableTemp";

            DataTable dt = dbAccess.SQLViewerData(strQuery);

            if (dt.Rows.Count > 0)
            {
                return RedirectToActionPermanent("Error", "Connection");
            }


            tblConnection tblconnection = db.tblConnections.Find(id);
            if (tblconnection == null)
            {
                return HttpNotFound();
            }
            ViewBag.DBId = new SelectList(db.tblDBNames, "Id", "DBName", tblconnection.DBId);
            ViewBag.DBSchemaId = new SelectList(db.tblDBSchemas, "Id", "DBSchema", tblconnection.DBSchemaId);
            ViewBag.DBServerId = new SelectList(db.tblDBServers, "Id", "DBServer", tblconnection.DBServerId);
            ViewBag.DBTableId = new SelectList(db.tblDBTables, "Id", "DBTableOrView", tblconnection.DBTableId);
            return View(tblconnection);
        }

        //
        // POST: /Connection/Edit/5

        [HttpPost]
        public JsonResult Edit(List<string> connection)
        {
            try
            {

                tblConnection tblconnection = new tblConnection();

                DataAccess dbAccess = new DataAccess();
                String strQuery = @"SELECT Id
                                    FROM tblDBServer
                                    WHERE DBServer='" + connection[0] + "'";

                DataTable dt = dbAccess.SQLViewerData(strQuery);

                tblconnection.DBServerId = (dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0][0]) : -1;
                strQuery = @"SELECT Id
                                    FROM tblDBName
                                    WHERE DBName='" + connection[1] + "'";

                dt = dbAccess.SQLViewerData(strQuery);
                tblconnection.DBId = (dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0][0]) : -1;


                strQuery = @"SELECT ConnId
                            FROM tblConnection
                            where ConnectionName='" + connection[4] + "'";

                dt = dbAccess.SQLViewerData(strQuery);
                tblconnection.ConnId = (dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0][0]) : -1;

                strQuery = @"SELECT Id
                                    FROM tblDBSchema
                                    WHERE DBSchema='" + connection[2] + "'";

                dt = dbAccess.SQLViewerData(strQuery);
                tblconnection.DBSchemaId = (dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0][0]) : -1;

                strQuery = @"SELECT Id
                                    FROM tblDBTable
                                    WHERE DBTableOrView='" + connection[3] + "'";

                dt = dbAccess.SQLViewerData(strQuery);
                tblconnection.DBTableId = (dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0][0]) : -1;

                tblconnection.ConnectionName = connection[4];

                tblDBServer tbldbserver = new tblDBServer();
                tbldbserver.DBServer = connection[0];

                var name_count = db.tblDBServers.Where(t => t.DBServer == tbldbserver.DBServer).Select(id => id.DBServer).Count();
                if (name_count == 0)
                {
                    var max_id = db.tblDBServers.Where(t => t.Id == db.tblDBServers.Max(x => x.Id)).Select(id => id.Id).FirstOrDefault();

                    tbldbserver.Id = max_id + 1;
                    db.tblDBServers.Add(tbldbserver);
                    db.SaveChanges();

                    tblconnection.DBServerId = max_id + 1;
                }

                tblDBName tbldbname = new tblDBName();
                tbldbname.DBName = connection[1];

                name_count = db.tblDBNames.Where(t => t.DBName == tbldbname.DBName).Select(id => id.DBName).Count();
                if (name_count == 0)
                {
                    var max_id = db.tblDBNames.Where(t => t.Id == db.tblDBNames.Max(x => x.Id)).Select(id => id.Id).FirstOrDefault();

                    tbldbname.Id = max_id + 1;
                    db.tblDBNames.Add(tbldbname);
                    db.SaveChanges();

                    tblconnection.DBId = max_id + 1;
                }

                tblDBSchema tbldbschema = new tblDBSchema();
                tbldbschema.DBSchema = connection[2];

                name_count = db.tblDBSchemas.Where(t => t.DBSchema == tbldbschema.DBSchema).Select(id => id.DBSchema).Count();
                if (name_count == 0)
                {
                    var max_id = db.tblDBSchemas.Where(t => t.Id == db.tblDBSchemas.Max(x => x.Id)).Select(id => id.Id).FirstOrDefault();

                    tbldbschema.Id = max_id + 1;

                    db.tblDBSchemas.Add(tbldbschema);
                    db.SaveChanges();

                    tblconnection.DBSchemaId = max_id + 1;
                }

                tblDBTable tbldbtable = new tblDBTable();
                tbldbtable.DBTableOrView = connection[3];

                name_count = db.tblDBTables.Where(t => t.DBTableOrView == tbldbtable.DBTableOrView).Select(id => id.DBTableOrView).Count();
                if (name_count == 0)
                {
                    var max_id = db.tblDBTables.Where(t => t.Id == db.tblDBTables.Max(x => x.Id)).Select(id => id.Id).FirstOrDefault();

                    tbldbtable.Id = max_id + 1;
                    db.tblDBTables.Add(tbldbtable);
                    db.SaveChanges();
                    tblconnection.DBTableId = max_id + 1;
                }

                {
                    db.Entry(tblconnection).State = EntityState.Modified;
                    //db.tblConnections.Add(tblconnection);
                    db.SaveChanges();

                }

                return Json("Successfully Saved");
            }
            catch (Exception ex)
            {
                return Json("~," + ex.Message);
            }
        }

        //
        // GET: /Connection/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (!validate())
            {
                return RedirectToActionPermanent("UserConnection", "Home");
            }

            DataAccess dbAccess = new DataAccess();
            String strQuery = @"select *
                                from
	                                (
	                                select ConnectionID,count(*) as NoOfRows
	                                from dbo.tblUserSaveStates
	                                where ConnectionID=" + id;
            strQuery += @"  group by ConnectionID

	                                union all

	                                select ConnectionID,count(*) as NoOfRows 
	                                from dbo.tblUserSaveScript
	                                where ConnectionID=" + id + "  group by ConnectionID) as tableTemp";

            DataTable dt = dbAccess.SQLViewerData(strQuery);

            if (dt.Rows.Count > 0)
            {
                return RedirectToActionPermanent("Error", "Connection");
            }

            tblConnection tblconnection = db.tblConnections.Find(id);
            if (tblconnection == null)
            {
                return HttpNotFound();
            }
            return View(tblconnection);
        }

        //
        // POST: /Connection/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            tblConnection tblconnection = db.tblConnections.Find(id);
            db.tblConnections.Remove(tblconnection);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Connection/RenameState/5

        public ActionResult RenameState(string stateName, string connName)
        {
            ViewBag.StateName = stateName;
            ViewBag.ConnName = connName;

            
            return View();
        }

        //
        // POST: /Connection/RenameState/5
        [HttpPost]
        public JsonResult RenameState(List<string> inputUser)
        {
            try
            {
                DataAccess dbAccess = new DataAccess();
                String strQuery = @"UPDATE tblUserSaveStates
SET StateName='" + inputUser[0] + "' WHERE StateName='" + inputUser[2] + "'  AND ConnectionID=(SELECT ConnId FROM tblConnection WHERE ConnectionName='" + inputUser[1] + "')";

                int result = dbAccess.InsertDatatoAuthenticationDB(strQuery);

                if (result == 0)
                {
                    strQuery = @"UPDATE tblUserSaveScript
SET StateName='" + inputUser[0] + "' WHERE StateName='" + inputUser[2] + "'  AND ConnectionID=(SELECT ConnId FROM tblConnection WHERE ConnectionName='" + inputUser[1] + "')";

                    result = dbAccess.InsertDatatoAuthenticationDB(strQuery);
                }


                string s1 = Session["SaveStateName"].ToString();
                string s2=Session["ConnectionName"].ToString();

                if (s1 == inputUser[2] && s2 == inputUser[1])
                {
                    Session["SaveStateName"] = inputUser[0];
                }

                return Json("Correct");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        //
        // GET: /Connection/DeleteState/5

        public ActionResult DeleteState(string stateName, string connName)
        {
            ViewBag.StateName = stateName;
            ViewBag.ConnName = connName;
            return View();
        }
        //
        // POST: /Connection/DeleteState/5
        [HttpPost]
        public JsonResult DeleteState(List<string> inputUser)
        {
            try
            {
                DataAccess dbAccess = new DataAccess();
                String strQuery = @"Delete from tblUserSaveStates
                 WHERE StateName='" + inputUser[0] + "' AND ConnectionID=(SELECT ConnId FROM tblConnection WHERE ConnectionName='" + inputUser[1] + "')";

                int result = dbAccess.InsertDatatoAuthenticationDB(strQuery);

                if (result == 0)
                {
                    strQuery = @"Delete from tblUserSaveScript
                 WHERE StateName='" + inputUser[0] + "' AND ConnectionID=(SELECT ConnId FROM tblConnection WHERE ConnectionName='" + inputUser[1] + "')";

                    result = dbAccess.InsertDatatoAuthenticationDB(strQuery);
                }

                string s1 = Session["SaveStateName"].ToString();
                string s2 = Session["ConnectionName"].ToString();

                if (s1 == inputUser[0] && s2 == inputUser[1])
                {
                    Session["SaveStateName"] = "";
                }
                return Json("Correct");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}