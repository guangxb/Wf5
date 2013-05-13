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
    internal class NodeManualMediator : NodeMediator
    {
        internal NodeManualMediator(ActivityForwardContext executionObject, ISession session)
            : base(executionObject, session)
        {

        }

        /// <summary>
        /// 执行普通任务节点（next step: 任务执行逻辑考虑用存储过程）
        /// 1. 当设置任务完成时，同时设置活动完成
        /// 2. 当实例化活动数据时，产生新的任务数据
        /// </summary>
        internal override void ExecuteWorkItem()
        {
            try
            {
                var workItem = (WorkItem)CreateNewNode(WfExecutionContext.Activity);
                workItem.ActivityInstance = WfExecutionContext.ActivityInstance;

                //如果是自动类型的任务节点，则要执行自动实现的内部逻辑
                if (workItem.IsAutomanticWorkItem)
                {
                    IDynamicRunable dynamicNode = (IDynamicRunable)workItem;
                    dynamicNode.InvokeMethod(workItem.Activity.TaskImplementDetail,
                        WfExecutionContext.ActivityResource.UserParameters);
                }

                //完成当前的任务节点
                workItem.CompleteWorkItem(WfExecutionContext.TaskView.TaskID,
                    WfExecutionContext.ActivityResource,
                    this.Session);

                //获取下一步节点列表：并继续执行
                ContinueForwardCurrentNode(workItem,
                    WfExecutionContext.ActivityResource);
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        internal override NodeBase CreateNewNode(ActivityEntity activity)
        {
            return new TaskNode(activity);
        }
    }
}
