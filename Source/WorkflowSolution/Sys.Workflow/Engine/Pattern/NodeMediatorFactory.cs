using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Sys.Workflow.Common;
using Sys.Workflow.Business;
using Sys.Workflow.DataModel;
using Sys.Workflow.Utility;

namespace Sys.Workflow.Engine
{
    /// <summary>
    /// 节点执行器的工厂类
    /// </summary>
    internal class NodeMediatorFactory
    {
        /// <summary>
        /// 创建节点执行器的抽象类
        /// </summary>
        /// <param name="processInstance"></param>
        /// <param name="activity"></param>
        /// <param name="activityResource"></param>
        /// <param name="dataContext"></param>
        /// <returns></returns>
        internal static NodeMediator CreateWorkItemMediator(ActivityForwardContext executionObject,
            ISession session)
        {
            if (executionObject.Activity.IsStartNode)
                return new NodeMediatorStart(executionObject, session);
            else if (executionObject.Activity.IsAutomanticWorkItem)
                return new NodeMediatorAutomantic(executionObject, session);
            else
                return new NodeManualMediator(executionObject, session);
        }
    }
}
