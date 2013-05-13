using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.Engine
{
    /// <summary>
    /// 流程运行时异常类
    /// </summary>
    public class WfRuntimeException : System.ApplicationException
    {
        public WfRuntimeException(string message)
            : base(message)
        {
        }

        public WfRuntimeException(string message, System.Exception ex)
            : base(message, ex)
        {

        }
    }
}
