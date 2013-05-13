using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.Engine
{
    public class DLRuntimeInteroperationException : System.ApplicationException
    {
        public DLRuntimeInteroperationException(string message)
            : base(message)
        {
        }

        public DLRuntimeInteroperationException(string message, System.Exception ex)
            : base(message, ex)
        {

        }
    }
}
