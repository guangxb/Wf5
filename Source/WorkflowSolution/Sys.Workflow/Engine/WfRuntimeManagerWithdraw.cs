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
    /// 任务撤销运行时
    /// </summary>
    internal class WfRuntimeManagerWithdraw : WfRuntimeManager
    {
        internal override void ExecuteInstanceImp(ISession session)
        {
            //创建新任务节点
            var workItem = (WorkItem)WorkItemNodeFactory.CreateNewNode(base.BackwardContext.BackwardToTaskActivity);
            workItem.CreateActivityTaskAndTransitionInstances(base.BackwardContext.ProcessInstance,
                base.BackwardContext.FromActivityInstance,
                base.BackwardContext.BackwardToTargetTransition,
                TransitionTypeEnum.Withdrawed,
                base.ActivityResource,
                session);

            //更新撤销节点的状态（从准备状态更新为撤销状态）
            var aim = new ActivityInstanceManager();
            aim.Withdraw(base.BackwardContext.FromActivityInstance.ActivityInstanceGUID, 
                base.ActivityResource.LogonUser, session);

            //构造回调函数需要的数据
            base.WfExecutedResult = WfExecutedResult.Success();
        }

        internal override RuntimeManagerType GetRuntimeManagerType()
        {
            return RuntimeManagerType.WithdrawRuntime;
        }
    }
}
