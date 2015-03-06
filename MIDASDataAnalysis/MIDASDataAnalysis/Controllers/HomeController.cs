using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MIDASDataAnalysis.Models;
using System.Data;
using System.Text;

namespace MIDASDataAnalysis.Controllers
{
    public class HomeController : Controller
    {

        public void LoadSession()
        {
            int flag = 0;
            try
            {
                if (Session["isAdmin"].ToString() == "")
                {
                    DataAccess dbAccess = new DataAccess();
                    string strQuery = @"select distinct RoleName
                                        from dbo.UserProfile u
                                        inner join dbo.webpages_UsersInRoles map
                                        on u.UserId = map.UserId
                                        inner join dbo.webpages_Roles r
                                        on map.RoleId = r.RoleId
                                        where u.UserName='" +User.Identity.Name+"'";
                    DataTable dt = dbAccess.SQLViewerData(strQuery);
                    if (dt.Rows.Count>0)
                    {
                        if (dt.Rows[0][0].ToString() == "Admin")
                        {
                            Session["isAdmin"] = "TRUE";
                        }
                    }
                    flag = 1;
                }

            }
            catch (Exception ex)
            {
                if (flag == 0)
                {
                    Session["isAdmin"] = "";
                    LoadSession();
                }
               
            }
            try
            {
                if (Session["SaveStateName"].ToString() == "" && Session["SaveScriptStateName"].ToString() == "" && Session["StrTableName"].ToString() == "")
                {
                    Session["StrDBServer"] = "";
                    Session["StrDBName"] = "";
                    Session["StrSchema"] = "";
                    Session["StrTableName"] = "";
                    Session["ConnectionName"] = "";
                }
            }
            catch (Exception ex)
            {
                    Session["SaveScriptStateName"]="";
                    Session["SaveStateName"]="";
                    Session["StrDBServer"] = "";
                    Session["StrDBName"] = "";
                    Session["StrSchema"] = "";
                    Session["StrTableName"] = "";
                    Session["ConnectionName"] = "";
            }
        }

        public ActionResult Index()
        {
            try
            {
                if (Session["SaveScriptStateName"].ToString() != "")
                {
                    return View("ScriptView");
                }
            }
            catch (Exception ex)
            {

            }

            LoadSession();
            if (Session["StrDBServer"].ToString() == "" && Session["StrDBName"].ToString() == "")
            {

                DataAccess dbAccess = new DataAccess();
                String strQuery = @"SELECT ' ' ConnectionName
                                    UNION ALL
                                    SELECT DISTINCT c.ConnectionName
                                    FROM
	                                tblConnection c
	                                INNER JOIN MapUserConnection m
		                                ON c.ConnId = m.ConnID
	                                INNER JOIN UserProfile u
		                                ON m.UserID = u.UserID
                                    WHERE
	                                    u.UserName = '"+User.Identity.Name+"'";

                ViewBag.dtDBConnections = dbAccess.SQLViewerData(strQuery);
              
                return View("UserConnection");
            }
            else
            {
                DataAccess dbAccess = new DataAccess(Session["StrDBServer"].ToString(), Session["StrDBName"].ToString());
                String strQuery = @"select Column_Name
                                from information_schema.columns
                                where Table_name='" + Session["StrTableName"] + "' and Table_Schema='" + Session["StrSchema"] + "'  order by Ordinal_position";
                DataTable dt = dbAccess.MIDASData(strQuery);

                ViewBag.dtColumns = dt;
                strQuery = @"SELECT DISTINCT " + dt.Rows[0][0].ToString() + " from " + Session["StrTableName"] + " order by " + dt.Rows[0][0].ToString();

                ViewBag.dtCTY = dbAccess.MIDASData(strQuery);

                string strConnectionName = Session["ConnectionName"].ToString();
                string strUserName = User.Identity.Name.ToString();

                return View();
            }
        }

        public ActionResult UserConnection()
        {
            DataAccess dbAccess = new DataAccess();
            String strQuery = @"SELECT ' ' ConnectionName
                                    UNION ALL
                                    SELECT DISTINCT c.ConnectionName
                                                                    FROM
	                                                                    tblConnection c
	                                                                    INNER JOIN MapUserConnection m
		                                                                    ON c.ConnId = m.ConnID
	                                                                    INNER JOIN UserProfile u
		                                                                    ON m.UserID = u.UserID
                                    WHERE
	                                    u.UserName = '" + User.Identity.Name + "'";
            
            ViewBag.dtDBConnections = dbAccess.SQLViewerData(strQuery);

            try
            {
                if (Session["isAdmin"].ToString() == "")
                {
 
                }
            }
            catch (Exception ex)
            {
                Session["isAdmin"] = "";
            }
            
            LoadSession();
            
            return View();
        }
       

        public JsonResult GetTableName(List<String> selectedColumns)
        {
            DataAccess dbAccess = new DataAccess();
            string strQuery = @"SELECT DISTINCT ltrim(rtrim(s.DBSchema)) + '.' + ltrim(rtrim(t.DBTableOrView)) tblName
			                                  , ser.DBServer
			                                  , dbname.DBName
                                FROM
	                                tblConnection c
	                                INNER JOIN tblDBTable t
		                                ON c.DBTableId = t.Id
	                                INNER JOIN tblDBSchema s
		                                ON c.DBSchemaId = s.Id
	                                INNER JOIN tblDBServer ser
		                                ON c.DBServerId = ser.Id
	                                INNER JOIN tblDBName dbname
		                                ON c.DBId = dbname.Id
                                    WHERE
            	                         c.ConnectionName='" + selectedColumns[0] + "'";
            DataTable dt = dbAccess.SQLViewerData(strQuery);
            strQuery = "";

            dbAccess = new DataAccess(dt.Rows[0][1].ToString(), dt.Rows[0][2].ToString());

            if (dt.Rows[0][0].ToString().Split('.')[0] == "[ALL]")
            {
                string strSubQuery = @" Select '' tblName
                                        union all
                                        SELECT s.name + '.' + o.name tblName
                                        FROM
	                                        sys.objects o
	                                        INNER JOIN sys.schemas s
		                                        ON o.schema_id = s.schema_id
                                        WHERE
	                                        type IN ('V', 'U')
	                                        AND s.name LIKE '%'
	                                        AND o.name LIKE '%'";
                DataTable dtsub = dbAccess.MIDASData(strSubQuery);
                strQuery = "";
                for (int j = 0; j < dtsub.Rows.Count; j++)
                {
                    strQuery += "," + dtsub.Rows[j][0].ToString();
                }

            }
            else if (dt.Rows[0][0].ToString().Split('.')[1] == "[ALL]")
            {
                string strSubQuery = @" Select '' tblName
                                        union all
                                        SELECT s.name + '.' + o.name tblName
                                        FROM
	                                        sys.objects o
	                                        INNER JOIN sys.schemas s
		                                        ON o.schema_id = s.schema_id
                                        WHERE
	                                        type IN ('V', 'U')
	                                        AND s.name ='" + dt.Rows[0][0].ToString().Split('.')[0] + "'   AND o.name LIKE '%'";
                DataTable dtsub = dbAccess.MIDASData(strSubQuery);
                for (int j = 0; j < dtsub.Rows.Count; j++)
                {
                    strQuery += "," + dtsub.Rows[j][0].ToString();
                }

            }
            else
            {
                strQuery += ",," + dt.Rows[0][0].ToString();
            }

            strQuery = strQuery.Substring(1, strQuery.Length - 1);

            //States 


            string strQuery_States = @"Select '' StateName
                                union all
                                Select us.StateName
                                FROM tblUserSaveStates us
                                INNER JOIN tblConnection c
                                on us.ConnectionID=c.ConnId
                                WHERE c.ConnectionName='" + selectedColumns[0] + "' ";
            strQuery_States += @" union all
                                Select us.StateName
                                FROM tblUserSaveScript us
                                INNER JOIN tblConnection c
                                on us.ConnectionID=c.ConnId
                                WHERE c.ConnectionName='" + selectedColumns[0] + "' ";
            dt = dbAccess.SQLViewerData(strQuery_States);
            strQuery_States = "";
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                strQuery_States += "," + dt.Rows[j][0].ToString();
            }
            strQuery_States = strQuery_States.Substring(1, strQuery_States.Length - 1);

            strQuery = strQuery +"~"+ strQuery_States;

            //END Save States

            return Json(strQuery);
        }

        public ActionResult ScriptView(List<String> selectedColumns)
        {
            return View();
        }

        public JsonResult PostConnection(List<String> selectedColumns)
        {

            DataAccess dbAccess = new DataAccess();
            String strQuery = @"SELECT DISTINCT s.DBServer
			                                  , db.DBName
			                                  , sc.DBSchema
			                                  , t.DBTableOrView
                                FROM
	                                tblDBServer s
	                                INNER JOIN tblConnection a
		                                ON s.Id = a.DBServerId
	                                INNER JOIN tblDBName db
		                                ON a.DBId = db.Id
	                                INNER JOIN tblDBSchema sc
		                                ON a.DBSchemaId = sc.Id
	                                INNER JOIN tblDBTable t
		                                ON a.DBTableId = t.Id
                                WHERE
	                                a.ConnectionName = '" + selectedColumns[0]+"'";
            DataTable dt = dbAccess.SQLViewerData(strQuery);
            


            Session["StrDBServer"] = dt.Rows[0][0].ToString();
            Session["StrDBName"] = dt.Rows[0][1].ToString();
            Session["ConnectionName"] = selectedColumns[0].ToString();

            DataTable dtWhichState = new DataTable();
            strQuery = @"Select script 
                        FROM tblUserSaveScript
                        where stateName ='" + selectedColumns[2].ToString() + "'";
            dtWhichState = dbAccess.SQLViewerData(strQuery);

            string returnStr = "View";

            if (dtWhichState.Rows.Count <= 0)
            {
                Session["SaveStateName"] = selectedColumns[2].ToString();
                Session["SaveScriptStateName"] = "";
            }
            else
            {
                Session["SaveScriptStateName"] = selectedColumns[2].ToString();
                returnStr = dtWhichState.Rows[0][0].ToString();
                Session["SaveStateName"] = "";
            }

            if (selectedColumns[2].ToString().Trim() == string.Empty)
            {
                Session["StrSchema"] = selectedColumns[1].Split('.')[0];
                Session["StrTableName"] = selectedColumns[1].Split('.')[1];
               
            }
            else
            {
                strQuery = @"select TableName
                            from  dbo.tblUserSaveStates    
                            where stateName ='" + selectedColumns[2].ToString() + "'";
                DataTable dtState = dbAccess.SQLViewerData(strQuery);
                if (dtState != null)
                {
                    if (dtState.Rows.Count > 0)
                    {
                        Session["StrSchema"] = dtState.Rows[0][0].ToString().Split('.')[0];
                        Session["StrTableName"] = dtState.Rows[0][0].ToString().Split('.')[1];
                    }
                }
            }

            return Json(returnStr);
        }


        public JsonResult GetScriptResult(List<String> selectedColumns)
        {
            DataAccess dbAccess = new DataAccess(Session["StrDBServer"].ToString(), Session["StrDBName"].ToString());
            String strQuery="";
            try
            {
                strQuery = selectedColumns[0];

                DataTable dt = dbAccess.MIDASData(strQuery);

                strQuery = ConvertDT2HTMLString(dt);
            }
            catch (Exception ex)
            {
                strQuery = ex.Message;
            }
            return Json(strQuery);
        }

        [HttpPost]
        public JsonResult PostSelectedColumns(List<String> selectedColumns)
        {
            DataAccess dbAccess = new DataAccess(Session["StrDBServer"].ToString(), Session["StrDBName"].ToString());
            string selectCol = "";
            string whereCluse = "";
            string previousColumns = "";
            int flag = 0, flag2 = 0, nullFlag = 0, previousColumnsFlag = 0;

            for (int i = 0; i < selectedColumns.Count; i++)
            {
                if (selectedColumns[i] == "---###---")
                {
                    flag2 = i+1;
                    break;
                }
                selectCol = selectCol + " " + selectedColumns[i] + ",";
            }
            selectCol = selectCol.Substring(0, selectCol.Length - 1);

            for (int i = flag2; i < selectedColumns.Count; i++)
            {
                if (flag == 0)
                {
                    whereCluse = whereCluse + " " + selectedColumns[i] + " " + selectedColumns[i+2] + " (";
                    previousColumns = " " + selectedColumns[i] + " ";
                    flag = 1;
                    i=i+3 ;
                }

                else
                {
                    if (selectedColumns[i] == "~")
                    {
                        flag = 0;
                        whereCluse = whereCluse.Substring(0, whereCluse.Length - 1);
                        if (i != selectedColumns.Count-1)
                        {
                            i++;
                            if (nullFlag == 1)
                            {
                                whereCluse = whereCluse + ")) " + selectedColumns[i];
                                nullFlag = 0;
                            }
                            else if (previousColumnsFlag == 1)
                            {
                                whereCluse = whereCluse + ")) " + selectedColumns[i];
                                previousColumnsFlag = 0;
                            }
                            else
                            {
                                whereCluse = whereCluse + ")  " + selectedColumns[i];
                            }
                            i++;

                            if (selectedColumns[i + 1] == "isSubFilter")
                            {
                                whereCluse = whereCluse.Replace(previousColumns, "(" + previousColumns);
                                previousColumnsFlag = 1;
                                i=i+2;
                            }
                        }
                        else
                        {
                            if (nullFlag == 1)
                            {
                                whereCluse = whereCluse + ")) and ";
                                nullFlag = 0;
                            }
                            else if (previousColumnsFlag == 1)
                            {
                                whereCluse = whereCluse + ")) and ";
                                previousColumnsFlag = 0;
                            }
                            else
                            {
                                whereCluse = whereCluse + ")  and ";
                            }
                        }
                       
                    }
                    else
                    {
                        if (selectedColumns[i] == "<<NULL>>")
                        {
                            string[] arrSplit = whereCluse.Split(' ');
                            string nullCheck = "";
                            if (selectedColumns[i + 1] == "~")
                            {
                                nullCheck = " " + arrSplit[arrSplit.Length - 3] + " is null ";
                            }
                            else
                            {
                                nullCheck = " ( " + arrSplit[arrSplit.Length - 3] + " is null or " + arrSplit[arrSplit.Length - 3] + "";
                                nullFlag = 1;
                            }
                            arrSplit[arrSplit.Length - 3] = nullCheck;
                            whereCluse = string.Join(" ", arrSplit);
                        }
                        else if (selectedColumns[i] == "<<BLANK>>")
                        {
                            string[] arrSplit = whereCluse.Split(' ');
                            arrSplit[arrSplit.Length - 3] = "ltrim(rtrim(" + arrSplit[arrSplit.Length - 3] + "))";
                            whereCluse = string.Join(" ", arrSplit);
                            whereCluse = whereCluse + "'',";
                        }
                        else
                        {
                            whereCluse = whereCluse + "'" + selectedColumns[i] + "',";
                        }
                    }
                }
            }

            whereCluse = whereCluse.Substring(0, whereCluse.Length - 4);
            String strQuery = @"SELECT distinct " + selectCol + " from " + Session["StrTableName"] + "  where " + whereCluse;
            
            DataTable dt = dbAccess.MIDASData(strQuery);
            strQuery = ConvertDT2HTMLString(dt);

            return Json(strQuery);
        }

        [HttpPost]
        public JsonResult SaveScript(List<String> selectedColumns)
        {
            try
            {
                string strConnectionName = Session["ConnectionName"].ToString();
                string strUserName = User.Identity.Name.ToString();
                String strQuery = "";
                string strAttributes = "";
                for (int i = 3; i < selectedColumns.Count; i++)
                {
                    strAttributes += "," + selectedColumns[i];
                }
                strAttributes = strAttributes.Substring(1);

                string[] arr = strAttributes.Split('~');
                strAttributes = arr[0].Substring(0, arr[0].Length - 1);
                string strColumns = arr[1].Substring(1, arr[1].Length - 1);
                if ((selectedColumns[2].Trim() == "") || (selectedColumns[2] != selectedColumns[1]))
                {
                    strQuery = @"insert into tblUserSaveStates 
                            values('" + selectedColumns[1] + "',(SELECT ConnId from tblConnection WHERE ConnectionName='" + strConnectionName + "'),'" + selectedColumns[0] + "','" + strAttributes + "','" + strColumns + "','" + Session["StrSchema"] + "." + Session["StrTableName"] + "')";
                }
                else
                {
                    strQuery = @"Update tblUserSaveStates 
                            set StateName='" + selectedColumns[1] + "',HTMLDOC='" + selectedColumns[0] + "',Attributes='" + strAttributes + "',Columns='" + strColumns + "' ";
                    strQuery += @" where ConnectionID=(SELECT ConnId from tblConnection WHERE ConnectionName='" + strConnectionName + "') and TableName='" + Session["StrSchema"] + "." + Session["StrTableName"] + "' and StateName='" + selectedColumns[2] + "' ";
                }
                
                DataAccess objconn = new DataAccess();
                objconn.InsertDatatoAuthenticationDB(strQuery);
                Session["SaveStateName"] = selectedColumns[1];
                return Json("Success");
            }
            catch (Exception ex)
            {
                return Json("~,"+ex.Message);
            }
            
        }

        [HttpPost]
        public JsonResult SaveOnlyScript(List<String> selectedColumns)
        {
            try
            {
                string strScriptText = selectedColumns[0];
                string strConnectionName = Session["ConnectionName"].ToString();
                string strQuery = "";

                strScriptText=strScriptText.Replace("'", "''");

                

                if ((selectedColumns[2].Trim() == "") || (selectedColumns[2] != selectedColumns[1]))
                {
                    strQuery = @"insert into tblUserSaveScript 
                            values('" + selectedColumns[1] + "',(SELECT ConnId from tblConnection WHERE ConnectionName='" + strConnectionName + "'),'" + strScriptText + "','True')";
                }
                else
                {
                    strQuery = @"Update tblUserSaveScript 
                            set Script='" + strScriptText + "'";
                    strQuery += @" where ConnectionID=(SELECT ConnId from tblConnection WHERE ConnectionName='" + strConnectionName + "') and StateName='" + selectedColumns[2] + "' ";
                }
                Session["SaveScriptStateName"] = selectedColumns[1];
                DataAccess objconn = new DataAccess();
                objconn.InsertDatatoAuthenticationDB(strQuery);



                return Json(selectedColumns[0]);
            }
            catch (Exception ex)
            {
                return Json("~," + ex.Message);
            }

        }

        [HttpPost]
        public JsonResult LoadState(List<String> selectedColumns)
        {

            string strConnectionName = Session["ConnectionName"].ToString();
            string strUserName = User.Identity.Name.ToString();
            String strQuery = @"SELECT HTMLDOC,Attributes,Columns,TableName
                                FROM tblUserSaveStates
                                WHERE  ConnectionID=(SELECT ConnId from tblConnection WHERE ConnectionName='" + strConnectionName + "') AND StateName='" + selectedColumns[0] + "'";
            DataAccess objconn = new DataAccess();
            DataTable dt = objconn.SQLViewerData(strQuery);
            Session["StrSchema"] = dt.Rows[0][3].ToString().Split('.')[0];
            Session["StrTableName"] = dt.Rows[0][3].ToString().Split('.')[1];
            Session["SaveScriptStateName"] = "";
            return Json(dt.Rows[0][0].ToString() + "~" + dt.Rows[0][1].ToString() + "~" + dt.Rows[0][2].ToString());
        }

        [HttpPost]
        public JsonResult ViewScript(List<String> selectedColumns)
        {

            string selectCol = "";
            string whereCluse = "";
            string previousColumns = "";
            int flag = 0, flag2 = 0, nullFlag = 0, previousColumnsFlag = 0;

            for (int i = 0; i < selectedColumns.Count; i++)
            {
                if (selectedColumns[i] == "---###---")
                {
                    flag2 = i + 1;
                    break;
                }
                selectCol = selectCol + " " + selectedColumns[i] + ",";
            }
            selectCol = selectCol.Substring(0, selectCol.Length - 1);

            for (int i = flag2; i < selectedColumns.Count; i++)
            {
                if (flag == 0)
                {
                    whereCluse = whereCluse + " " + selectedColumns[i] + " " + selectedColumns[i + 2] + " (";
                    previousColumns = " " + selectedColumns[i] + " ";
                    flag = 1;
                    i = i + 3;
                }

                else
                {
                    if (selectedColumns[i] == "~")
                    {
                        flag = 0;
                        whereCluse = whereCluse.Substring(0, whereCluse.Length - 1);
                        if (i != selectedColumns.Count - 1)
                        {
                            i++;
                            if (nullFlag == 1)
                            {
                                whereCluse = whereCluse + ")) " + selectedColumns[i];
                                nullFlag = 0;
                            }
                            else if (previousColumnsFlag == 1)
                            {
                                whereCluse = whereCluse + ")) " + selectedColumns[i];
                                previousColumnsFlag = 0;
                            }
                            else
                            {
                                whereCluse = whereCluse + ")  " + selectedColumns[i];
                            }
                            i++;

                            if (selectedColumns[i + 1] == "isSubFilter")
                            {
                                whereCluse = whereCluse.Replace(previousColumns, "(" + previousColumns);
                                previousColumnsFlag = 1;
                                i = i + 2;
                            }
                        }
                        else
                        {
                            if (nullFlag == 1)
                            {
                                whereCluse = whereCluse + ")) and ";
                                nullFlag = 0;
                            }
                            else if (previousColumnsFlag == 1)
                            {
                                whereCluse = whereCluse + ")) and ";
                                previousColumnsFlag = 0;
                            }
                            else
                            {
                                whereCluse = whereCluse + ")  and ";
                            }
                        }

                    }
                    else
                    {
                        if (selectedColumns[i] == "<<NULL>>")
                        {
                            string[] arrSplit = whereCluse.Split(' ');
                            string nullCheck = "";
                            if (selectedColumns[i + 1] == "~")
                            {
                                nullCheck = " " + arrSplit[arrSplit.Length - 3] + " is null ";
                            }
                            else
                            {
                                nullCheck = " ( " + arrSplit[arrSplit.Length - 3] + " is null or " + arrSplit[arrSplit.Length - 3] + "";
                                nullFlag = 1;
                            }
                            arrSplit[arrSplit.Length - 3] = nullCheck;
                            whereCluse = string.Join(" ", arrSplit);
                        }
                        else if (selectedColumns[i] == "<<BLANK>>")
                        {
                            string[] arrSplit = whereCluse.Split(' ');
                            arrSplit[arrSplit.Length - 3] = "ltrim(rtrim(" + arrSplit[arrSplit.Length - 3] + "))";
                            whereCluse = string.Join(" ", arrSplit);
                            whereCluse = whereCluse + "'',";
                        }
                        else
                        {
                            whereCluse = whereCluse + "'" + selectedColumns[i] + "',";
                        }
                    }
                }
            }

            whereCluse = whereCluse.Substring(0, whereCluse.Length - 4);
            String strQuery = @"SELECT distinct " + selectCol + " from " + Session["StrTableName"] + "  where " + whereCluse;

            return Json(strQuery);
        }

        [HttpPost]
        public JsonResult SelectedColumn(List<String> selectedColumns)
        {
            DataAccess dbAccess = new DataAccess(Session["StrDBServer"].ToString(), Session["StrDBName"].ToString());
            String strQuery = @"SELECT DISTINCT  " + selectedColumns[0] + " from " + Session["StrTableName"] + " order by " + selectedColumns[0];


            DataTable dt = dbAccess.MIDASData(strQuery);
            strQuery = "";
            foreach (DataRow row in dt.Rows)
            {
                if (row[0] == DBNull.Value)
                {
                    strQuery += "|<<NULL>>";
                }
                else if (row[0].ToString().Trim() == string.Empty)
                {
                    strQuery += "|<<BLANK>>";
                }
                else
                {
                    strQuery += "|" + row[0].ToString();
                }
            }
            strQuery = strQuery.Substring(1, strQuery.Length-1);

            return Json(strQuery);
        }

        [HttpPost]
        public JsonResult SelectedColumnAndValue(List<String> selectedColumns)
        {
            DataAccess dbAccess = new DataAccess(Session["StrDBServer"].ToString(), Session["StrDBName"].ToString());
            string whereCluse = "";
            int flag = 0,nullFlag=0;
            for (int i = 0; i < selectedColumns.Count; i++)
            {
                if (selectedColumns[i] == "---###---")
                {
                    flag = i;
                    break;
                }
                if (flag == 0)
                {
                    whereCluse = whereCluse + " " + selectedColumns[i] + " " + selectedColumns[i + 2] + " (";
                    flag = 1;
                    i = i + 3;
                }
                
                else
                {


                    if (selectedColumns[i] == "~")
                    {
                        flag = 0;
                        whereCluse = whereCluse.Substring(0, whereCluse.Length - 1);
                        if (nullFlag == 1)
                        {
                            whereCluse = whereCluse + ")) and ";
                            nullFlag = 0;
                        }
                        else
                        {
                            whereCluse = whereCluse + ") and ";
                        }
                    }
                    else
                    {
                        if (selectedColumns[i] == "<<NULL>>")
                        {
                            string[] arrSplit = whereCluse.Split(' ');
                            string nullCheck = "";
                            if (selectedColumns[i + 1] == "~")
                            {
                                nullCheck = " " + arrSplit[arrSplit.Length - 3] + " is null ";
                            }
                            else
                            {
                                nullCheck = " ( " + arrSplit[arrSplit.Length - 3] + " is null or " + arrSplit[arrSplit.Length - 3] + "";
                                nullFlag = 1;
                            }
                            arrSplit[arrSplit.Length - 3] = nullCheck;
                            whereCluse = string.Join(" ", arrSplit);
                        }
                        else if (selectedColumns[i] == "<<BLANK>>")
                        {
                            string[] arrSplit = whereCluse.Split(' ');
                            arrSplit[arrSplit.Length - 3] = "ltrim(rtrim(" + arrSplit[arrSplit.Length - 3] + "))";
                            whereCluse = string.Join(" ", arrSplit);
                            whereCluse = whereCluse + "'',";
                        }
                        else
                        {
                            whereCluse = whereCluse + "'" + selectedColumns[i] + "',";
                        }
                    }
                }

               
            }

            whereCluse = whereCluse.Substring(0, whereCluse.Length-4);

            String strQuery = @"SELECT DISTINCT  " + selectedColumns[flag + 1] + " from " + Session["StrTableName"] + "  where " + whereCluse + " order by " + selectedColumns[flag + 1];

            DataTable dt = dbAccess.MIDASData(strQuery);
            strQuery = "";
            foreach (DataRow row in dt.Rows)
            {
                if (row[0] == DBNull.Value)
                {
                    strQuery += "|<<NULL>>";
                }
                else if (row[0].ToString().Trim() == string.Empty)
                {
                    strQuery += "|<<BLANK>>";
                }
                else
                {
                    strQuery += "|" + row[0].ToString();
                }
            }

            ///Edited for new error
            try
            {
                strQuery = strQuery.Substring(1, strQuery.Length - 1);
            }
            catch (Exception ex)
            {
 
            }
            return Json(strQuery);
        }

        public string ConvertDT2HTMLString(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append("<table><thead><tr>");
            sb.Append("<thead><tr>");
            foreach (DataColumn c in dt.Columns)
            {
                sb.AppendFormat("<th>{0}</th>", c.ColumnName);
            }
            sb.AppendLine("</tr></thead><tbody>");
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append("<tr>");
                foreach (object o in dr.ItemArray)
                {
                    if (o == DBNull.Value)
                    {
                        sb.AppendFormat("<td>{0}</td>", System.Web.HttpUtility.HtmlEncode("<<NULL>>"));
                    }
                    else if (o.ToString().Trim() == "")
                    {
                        sb.AppendFormat("<td>{0}</td>", System.Web.HttpUtility.HtmlEncode("<<BLANK>>"));
                    }
                    else
                    {
                        sb.AppendFormat("<td>{0}</td>", System.Web.HttpUtility.HtmlEncode(o.ToString()));
                    }
                }
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</tbody>");
            return sb.ToString();
        }


    }
  
}
