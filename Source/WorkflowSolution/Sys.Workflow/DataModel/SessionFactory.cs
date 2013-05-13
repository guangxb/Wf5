using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Sys.Workflow.DataModel
{
    public class SessionFactory
    {
        public static IDbConnection CreateConnection()
        {
            var strConn = ConfigurationManager.ConnectionStrings["ProductDbContext"].ToString();
            IDbConnection conn = new SqlConnection(strConn);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                Debug.WriteLine("session factory create connecton and opened...");
            }

            Debug.WriteLine(string.Format("crate new database connection:{0}", strConn));
            return conn;
        }

        public static ISession CreateSession()
        {
            try
            {
                IDbConnection conn = CreateConnection();
                ISession session = new Session(conn);
                session.Begin();
                return session;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}
