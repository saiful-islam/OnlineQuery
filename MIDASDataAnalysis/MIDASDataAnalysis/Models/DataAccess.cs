using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace MIDASDataAnalysis.Models
{
    
    public class DataAccess
    {
        string strDBServer = "";
        string strDBName = "";
        
        public DataAccess(string strServer,string StrDatabase)
        {
            this.strDBServer = strServer;
            this.strDBName = StrDatabase;
        }
        public DataAccess()
        {
        }
        public DataAccess(string strServer)
        {
            this.strDBServer = strServer;
        }
        public DataTable MIDASData(string strQuery)
        {
            
            //using (new Impersonation("INTERNAL", "DACASVC-RDPAdmin", "Wh2t3v3r"))
            using (new Impersonation("INTERNAL", "DACASVC-SQLDataViewe", "Password1"))
            {
                SqlConnection dbConn = new SqlConnection(@"Data Source=" + this.strDBServer + ";Initial Catalog=" + this.strDBName + ";Integrated Security=True;");
                if (dbConn.State.ToString() == "Closed")
                {
                    dbConn.Open();
                }
                SqlCommand dbCommand = new SqlCommand(strQuery, dbConn);
                SqlDataAdapter da = new SqlDataAdapter(dbCommand);

                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable GetMasterData(string strQuery)
        {
            
            //using (new Impersonation("INTERNAL", "DACASVC-RDPAdmin", "Wh2t3v3r"))
            using (new Impersonation("INTERNAL", "DACASVC-SQLDataViewe", "Password1"))
            {
                SqlConnection dbConn = new SqlConnection(@"Data Source=" + this.strDBServer + ";Initial Catalog=master;Integrated Security=True;");
                if (dbConn.State.ToString() == "Closed")
                {
                    dbConn.Open();
                }
                SqlCommand dbCommand = new SqlCommand(strQuery, dbConn);
                SqlDataAdapter da = new SqlDataAdapter(dbCommand);

                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
        public DataTable SQLViewerData(string strQuery)
        {
            SqlConnection dbConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLDataViewerAuthenticationConnection"].ConnectionString);
            if (dbConn.State.ToString() == "Closed")
            {
                dbConn.Open();
            }

            SqlCommand dbCommand = new SqlCommand(strQuery, dbConn);
            SqlDataAdapter da = new SqlDataAdapter(dbCommand);

            DataTable dt = new DataTable();

            da.Fill(dt);
            return dt;
        }
        public int InsertDatatoAuthenticationDB(string strQuery)
        {
            SqlConnection dbConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLDataViewerAuthenticationConnection"].ConnectionString);
            if (dbConn.State.ToString() == "Closed")
            {
                dbConn.Open();
            }

            SqlCommand dbCommand = new SqlCommand(strQuery, dbConn);

            return dbCommand.ExecuteNonQuery();
        }
      
    }
}