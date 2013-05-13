using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Sys.Workflow.DataModel
{
    public class WfDatabase : Database, IDatabase
    {
        public WfDatabase(IDbConnection conn)
            : base(conn)
        {
        }
    }
}
