using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow
{
    /// <summary>
    /// 工作流对外提供的数据异常类
    /// </summary>
    public class WfDataException : System.ApplicationException
    {
        public WfDataException(string message)
            : base(message)
        {
        }

        public WfDataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
