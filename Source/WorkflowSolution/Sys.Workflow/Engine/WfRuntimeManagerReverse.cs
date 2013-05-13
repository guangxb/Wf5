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
    /// 流程返签时的运行时
    /// </summary>
    internal class WfRuntimeManagerReverse : WfRuntimeManager
    {
        internal override void ExecuteInstanceImp(ISession session)
        {
            //修改流程实例为返签状态
            var pim = new ProcessInstanceManager();
            pim.Reverse(base.BackwardContext.ProcessInstance.ProcessInstanceGUID, base.ActivityResource.LogonUser, session);

            //创建新任务节点
            var workItem = (WorkItem)WorkItemNodeFactory.CreateNewNode(base.BackwardContext.BackwardToTaskActivity);
            workItem.CreateActivityTaskAndTransitionInstances(base.BackwardContext.ProcessInstance,
                base.BackwardContext.FromActivityInstance,
                base.BackwardContext.BackwardToTargetTransition,
                TransitionTypeEnum.Backward,
                base.ActivityResource,
                session);

            //构造回调函数需要的数据
            base.WfExecutedResult = WfExecutedResult.Success();
        }

        internal override RuntimeManagerType GetRuntimeManagerType()
        {
            return RuntimeManagerType.ReverseRuntime;
        }
    }
}
