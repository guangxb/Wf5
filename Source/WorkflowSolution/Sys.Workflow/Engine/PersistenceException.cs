using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.Engine
{
    /// <summary>
    /// 持久化异常信息
    /// </summary>
    public class PersistenceException : System.ApplicationException
    {
        public PersistenceException(string message)
            : base(message)
        {
        }

        public PersistenceException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}
