using System;
using System.Threading;
using System.Data.Linq;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Common;
using Sys.Workflow.DataModel;
using Sys.Workflow.Business;
using Sys.Workflow.Utility;

namespace Sys.Workflow.Engine
{
    /// <summary>
    /// 流程回退处理时的上下文对象
    /// </summary>
    internal class BackwardContext
    {
        internal ActivityEntity BackwardToTaskActivity { get; set; }
        internal ActivityInstanceEntity BackwardToTaskActivityInstance { get; set; }
        internal ActivityEntity FromActivity { get; set; }
        internal ActivityInstanceEntity FromActivityInstance { get; set; }
        internal ProcessInstanceEntity ProcessInstance { get; set; }
        internal TransitionEntity BackwardToTargetTransition { get; set; }
    }
}
