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
    /// 任务节点
    /// </summary>
    internal class TaskNode : WorkItem
    {
        internal TaskNode(ActivityEntity activity)
            : base(activity)
        {

        }

        internal override void CreateNewTask(ActivityResource activityResource,
            ISession session)
        {
            if (activityResource.NextActivityPerformers == null)
            {
                throw new WorkflowException("无法创建任务，流程流转下一步的办理人员不能为空！");
            }

            TaskManager taskManager = new TaskManager();
            taskManager.Insert(base.ActivityInstance,
                activityResource.NextActivityPerformers[base.Activity.ActivityGUID], 
                activityResource.LogonUser,
                session);
        }

        internal override void CompleteTask(long taskID,
            ActivityResource activityResource,
            ISession session)
        {
            TaskManager tm = new TaskManager();
            tm.Complete(taskID,
                activityResource.LogonUser,
                session);
        }
    }
}
