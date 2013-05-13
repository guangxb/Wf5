using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Sys.Workflow.Common;
using Sys.Workflow.Business;

namespace Sys.Workflow.Engine
{
    internal class ScriptNode : WorkItem, IDynamicRunable
    {
        internal ScriptNode(ActivityEntity activity)
            : base(activity)
        {

        }

        #region IDynamicRunable Members
        public object InvokeMethod(TaskImplementDetail implementation, object[] userParameters)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
