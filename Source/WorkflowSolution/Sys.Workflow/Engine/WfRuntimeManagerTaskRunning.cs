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
    /// 任务执行运行时
    /// </summary>
    internal class WfRuntimeManagerTaskRunning : WfRuntimeManager
    {
        /// <summary>
        /// 正常执行的运行时
        /// </summary>
        internal override void ExecuteInstanceImp(ISession session)
        {
            var runningExecutionContext = ActivityForwardContext.CreateTaskContext(base.TaskView,
                base.ProcessModel,
                base.ActivityResource);

            //判断流程是否可以被运行
            if (runningExecutionContext.ProcessInstance.ProcessState != (short)ProcessStateEnum.Running)
            {
                throw new WfRuntimeException(string.Format("当期流程不在运行状态！详细信息：当前流程状态：{0}", 
                    runningExecutionContext.ProcessInstance.ProcessState));
            }

            if (base.TaskView.ActivityState != (short)NodeStateEnum.Running)
            {
                throw new WfRuntimeException(string.Format("当期当前活动节点不在运行状态！详细信息：当前活动状态：{0}", 
                    base.TaskView.ActivityState));
            }

            //执行节点
            base.ExecuteWorkItemIteraly(runningExecutionContext, session);

            //构造回调函数需要的数据
            base.WfExecutedResult = WfExecutedResult.Success();
 
        }

        internal override RuntimeManagerType GetRuntimeManagerType()
        {
            return RuntimeManagerType.RunningRuntime;
        }
    }
}
