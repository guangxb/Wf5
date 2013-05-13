using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Utility;
using Sys.Workflow.DataModel;

namespace Sys.Workflow.Business
{
    internal class DataContextFactory
    {
        private static readonly string PROCESS_DATABASE_CONN_STRING = "SSIP-SYSTEM-ADMINConnectionString";

        internal static ProcessModelDataContext CreateWfLinqDataContext()
        {
            string conn = ConfigHelper.GetConnectionString(PROCESS_DATABASE_CONN_STRING);
            return new ProcessModelDataContext(conn);
        }
    }
}
