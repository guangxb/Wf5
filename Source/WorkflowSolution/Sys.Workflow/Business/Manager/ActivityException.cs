using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.Business
{
    public class ActivityInstanceException : System.ApplicationException
    {
        public ActivityInstanceException(string message)
            : base(message)
        {
        }

        public ActivityInstanceException(string message, Exception ex)
            : base(message, ex)
        {

        }
    }
}
