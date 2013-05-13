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
    /// 需要处理的任务类（其子类型有：人工任务或者自动任务）
    /// </summary>
    internal abstract class WorkItem : NodeBase
    {
        internal WorkItem(ActivityEntity activity)
            : base(activity)
        {

        }

        internal long TaskID
        {
            get;
            set;
        }

        internal bool IsAutomanticWorkItem
        {
            get
            {
                return Activity.IsAutomanticWorkItem;
            }
        }

        /// <summary>
        /// 创建任务的虚方法
        /// 1. 对于自动执行的工作项，无需重写该方法
        /// 2. 对于人工执行的工作项，需要重写该方法，插入待办的任务数据
        /// </summary>
        /// <param name="activityResource"></param>
        /// <param name="wfLinqDataContext"></param>
        internal virtual void CreateNewTask(ActivityResource activityResource,
            ISession session)
        {

        }

        /// <summary>
        /// 完成任务的虚方法
        /// 1. 对于自动执行的工作项，无需重写该方法
        /// 2. 对于人工执行的工作项，需要重写该方法，设置任务为完成状态
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="activityResource"></param>
        /// <param name="wfLinqDataContext"></param>
        internal virtual void CompleteTask(long taskID,
            ActivityResource activityResource,
            ISession session)
        {
            //neet to set task completed state
        }

        /// <summary>
        /// 创建工作项及转移数据
        /// </summary>
        /// <param name="processInstance"></param>
        /// <param name="fromToTransition"></param>
        /// <param name="fromActivityInstance"></param>
        /// <param name="activityResource"></param>
        /// <param name="session"></param>
        internal void CreateActivityTaskAndTransitionInstances(ProcessInstanceEntity processInstance,
            ActivityInstanceEntity fromActivityInstance,
            TransitionEntity fromToTransition,
            TransitionTypeEnum transitionType,
            ActivityResource activityResource,
            ISession session)
        {
            //实例化Activity
            var toActivityInstance = base.CreateActivityInstanceObject(processInstance, activityResource.LogonUser);

            //进入运行状态
            toActivityInstance.State = (short)NodeStateEnum.Ready;

            //插入活动实例数据
            base.InsertActivityInstance(toActivityInstance,
                session);

            //插入任务数据
            CreateNewTask(activityResource, session);

            //插入转移数据
            base.InsertTransitionInstance(processInstance,
                fromToTransition,
                fromActivityInstance,
                toActivityInstance,
                transitionType,
                activityResource.LogonUser,
                session);
        }

        //internal void ReverseWorkItemWithTransitonInstance(ProcessInstanceEntity processInstance,
        //    TransitionEntity endToLastTaskTransition,
        //    ActivityInstanceEntity endActivityInstance,
        //    ActivityEntity lastTaskNode,
        //    ActivityResource activityResource,
        //    ISession session)
        //{
        //    var toActivityInstance = base.CreateActivityInstanceObject(processInstance, activityResource.LogonUser);
        //    toActivityInstance.State = (short)NodeStateEnum.Ready;
        //    base.InsertActivityInstance(toActivityInstance,
        //        session);

        //    CreateNewTask(activityResource, session);

        //    base.InsertTransitionInstance(processInstance,
        //        endToLastTaskTransition,
        //        endActivityInstance,
        //        toActivityInstance,
        //        TransitionTypeEnum.Backward,
        //        activityResource.LogonUser,
        //        session);
        //}

        /// <summary>
        /// 完成任务实例
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="activityResource"></param>
        /// <param name="session"></param>
        internal void CompleteWorkItem(long taskID,
            ActivityResource activityResource,
            ISession session)
        {
            //设置任务为完成状态
            CompleteTask(taskID, activityResource, session);
               
            //设置活动节点的状态为完成状态
            base.CompleteActivityInstance(base.ActivityInstance.ActivityInstanceGUID,
                activityResource,
                session);

            SyncActivityInstanceObjectState(NodeStateEnum.Completed);
        }
    }
}
