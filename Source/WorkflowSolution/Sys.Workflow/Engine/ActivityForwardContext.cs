using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Business;

namespace Sys.Workflow.Engine
{
    /// <summary>
    /// 活动节点执行上下文对象
    /// </summary>
    internal class ActivityForwardContext
    {
        #region WfExecutionContenxt 属性列表

        internal ProcessModel ProcessModel { get; set; }
        internal ProcessInstanceEntity ProcessInstance { get; set; }
        internal ActivityEntity Activity { get; set; }
        internal ActivityResource ActivityResource { get; set; }
        internal ActivityInstanceEntity ActivityInstance { get; set; }
        internal TaskViewEntity TaskView { get; set; }

        #endregion

        #region WfExecutionContenxt 构造函数
        /// <summary>
        /// 开始节点的构造执行上下文对象
        /// </summary>
        /// <param name="processModel"></param>
        /// <param name="processInstance"></param>
        /// <param name="activity"></param>
        /// <param name="activityResource"></param>
        private ActivityForwardContext(ProcessModel processModel,
            ProcessInstanceEntity processInstance,
            ActivityEntity activity,
            ActivityResource activityResource)
        {
            ProcessModel = processModel;
            ProcessInstance = processInstance;
            Activity = activity;
            ActivityResource = activityResource;
        }

        /// <summary>
        /// 任务执行的上下文对象
        /// </summary>
        /// <param name="task"></param>
        /// <param name="processModel"></param>
        /// <param name="activityResource"></param>
        private ActivityForwardContext(TaskViewEntity task,
            ProcessModel processModel,
            ActivityResource activityResource)
        {
            this.TaskView = task;

            //check task condition has load activity instance
            this.ActivityInstance = (new ActivityInstanceManager()).GetById(task.ActivityInstanceGUID);
            this.ProcessInstance = (new ProcessInstanceManager()).GetById(task.ProcessInstanceGUID);
            this.Activity = processModel.GetActivity(task.ActivityGUID);
            this.ProcessModel = processModel;
            this.ActivityResource = activityResource;
        }

        /// <summary>
        /// 启动流程的上下文对象
        /// </summary>
        /// <param name="processModel"></param>
        /// <param name="processInstance"></param>
        /// <param name="activity"></param>
        /// <param name="activityResource"></param>
        /// <returns></returns>
        internal static ActivityForwardContext CreateStartupContext(ProcessModel processModel,
            ProcessInstanceEntity processInstance,
            ActivityEntity activity,
            ActivityResource activityResource)
        {
            return new ActivityForwardContext(processModel, processInstance, activity, activityResource);
        }

        /// <summary>
        /// 创建任务执行上下文对象
        /// </summary>
        /// <param name="task"></param>
        /// <param name="processModel"></param>
        /// <param name="activityResource"></param>
        /// <returns></returns>
        internal static ActivityForwardContext CreateTaskContext(TaskViewEntity task,
            ProcessModel processModel,
            ActivityResource activityResource)
        {
            return new ActivityForwardContext(task, processModel, activityResource);
        }
        #endregion
    }
}
