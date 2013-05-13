using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.DataModel.SQL;

namespace Sys.Workflow.DataModel
{
    public static class DataAccessFactory
    {
        private readonly static object _lock = new object();
        private static DataAccessManager _instance;
        public static DataAccessManager Instance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new DataAccessManager();
                    }
                }
            }
            return _instance;
        }

        public static SqlGenerator GetSqlGenerator()
        {
            var instance = Instance();
            return instance.GetSqlGenerator();
        }

        public static MapExtension GetMapExtension()
        {
            var instance = Instance();
            return instance.GetMapExtension();
        }
    }
}
