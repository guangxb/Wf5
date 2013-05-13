using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.Engine
{
    public class WfTaskImplementException : System.ApplicationException
    {
        public WfTaskImplementException(string message)
            : base(message)
        {
        }

        public WfTaskImplementException(string message, System.Exception ex)
            : base(message, ex)
        {

        }
    }
}
