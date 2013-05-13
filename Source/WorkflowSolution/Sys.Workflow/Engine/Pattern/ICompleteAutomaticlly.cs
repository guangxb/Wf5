using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Business;
using Sys.Workflow.Common;
using Sys.Workflow.DataModel;

namespace Sys.Workflow.Engine
{
    /// <summary>
    /// 路由接口
    /// </summary>
    internal interface ICompleteAutomaticlly
    {
        GatewayExecutedResult CompleteAutomaticlly(ProcessInstanceEntity processInstance,
            TransitionEntity fromTransition,
            ActivityInstanceEntity fromActivityInstance,
            ActivityResource activityResource,
            ISession session);
    }
}
