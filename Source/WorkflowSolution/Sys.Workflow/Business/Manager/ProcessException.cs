using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.Business
{
    public class ProcessInstanceException : System.ApplicationException
    {
        public ProcessInstanceException(string message)
            : base(message)
        {
        }


        public ProcessInstanceException(string message, Exception ex) : base(message, ex)
        {

        }
    }
}
