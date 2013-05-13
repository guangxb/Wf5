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
    /// 流程运行时管理
    /// </summary>
    internal abstract class WfRuntimeManager
    {
        #region 抽象方法
        internal abstract void ExecuteInstanceImp(ISession session);
        internal abstract RuntimeManagerType GetRuntimeManagerType();
        #endregion

        #region 流转属性
        internal WfAppRunner AppRunner { get; set; }
        internal ProcessModel ProcessModel { get; set; }
        internal ActivityResource ActivityResource { get; set; }
        internal TaskViewEntity TaskView { get; set; }
        
        //流程返签时的属性
        internal BackwardContext BackwardContext { get; set; }

        /// <summary>
        /// 流程执行结果对象
        /// </summary>
        internal WfExecutedResult WfExecutedResult { get; set; }
        
        #endregion

        #region 流程事件定义
        /// <summary>
        /// 流程被创建事件
        /// </summary>
        private event EventHandler<WfEventArgs> _onWfProcessCreated;
        internal event EventHandler<WfEventArgs> OnWfProcessCreated
        {
            add
            {
                _onWfProcessCreated += value;
            }
            remove
            {
                _onWfProcessCreated -= value;
            }
        }

        /// <summary>
        /// 流程执行事件
        /// </summary>
        private event EventHandler<WfEventArgs> _onWfProcessContinued;
        internal event EventHandler<WfEventArgs> OnWfProcessContinued
        {
            add
            {
                _onWfProcessContinued += value;
            }
            remove
            {
                _onWfProcessContinued -= value;
            }
        }

        /// <summary>
        /// 流程撤销回上一步事件
        /// </summary>
        private event EventHandler<WfEventArgs> _onWfProcessWithdrawed;
        internal event EventHandler<WfEventArgs> OnWfProcessWithdrawed
        {
            add
            {
                _onWfProcessWithdrawed += value;
            }
            remove
            {
                _onWfProcessWithdrawed -= value;
            }
        }

        /// <summary>
        /// 流程退回上一步事件
        /// </summary>
        private event EventHandler<WfEventArgs> _onWfProcessRejected;
        internal event EventHandler<WfEventArgs> OnWfProcessRejected
        {
            add
            {
                _onWfProcessRejected += value;
            }
            remove
            {
                _onWfProcessRejected -= value;
            }
        }

        /// <summary>
        /// 流程返签，重新运行事件
        /// </summary>
        private event EventHandler<WfEventArgs> _onWfProcessReversed;
        internal event EventHandler<WfEventArgs> OnWfProcessReversed
        {
            add
            {
                _onWfProcessReversed += value;
            }
            remove
            {
                _onWfProcessReversed -= value;
            }
        }
        #endregion

        #region 构造方法
        internal WfRuntimeManager()
        {
            AppRunner = new WfAppRunner();
            BackwardContext = new BackwardContext();
        }
        #endregion

        #region 运行方法
        /// <summary>
        /// 线程执行方法
        /// </summary>
        /// <returns></returns>
        internal bool Execute()
        {
            try
            {
                Thread thread = new Thread(ExecuteInstance);
                thread.Start();

                return true;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 流程执行方法
        /// </summary>
        internal void ExecuteInstance()
        {
            ISession session = SessionFactory.CreateSession();
            try
            {
                ExecuteInstanceImp(session);
                session.Commit();
            }
            catch (WfRuntimeException rx)
            {
                session.Rollback();
                WfExecutedResult = WfExecutedResult.Failed(rx.Message);
                LogManager.RecordLog(WfDefine.WF_PROCESS_ERROR, LogEventType.Error, LogPriority.High, AppRunner, rx);
            }
            catch (System.Exception e)
            {
                session.Rollback();
                WfExecutedResult = WfExecutedResult.Failed(e.Message);
                LogManager.RecordLog(WfDefine.WF_PROCESS_ERROR, LogEventType.Error, LogPriority.High, AppRunner, e);
            }
            finally
            {
                session.Dispose();
                Callback(GetRuntimeManagerType(), WfExecutedResult);
            }
        }

        /// <summary>
        /// 事件回调
        /// </summary>
        /// <param name="runtimeType"></param>
        /// <param name="result"></param>
        internal void Callback(RuntimeManagerType runtimeType, WfExecutedResult result)
        {
            WfEventArgs args = new WfEventArgs(result);
            
            if (runtimeType == RuntimeManagerType.StartupRuntime && _onWfProcessCreated != null)
            {
               _onWfProcessCreated(this, args);
            }
            else if (runtimeType == RuntimeManagerType.RunningRuntime && _onWfProcessContinued != null)
            {
                _onWfProcessContinued(this, args);
            }
            else if (runtimeType == RuntimeManagerType.WithdrawRuntime && _onWfProcessWithdrawed != null)
            {
                _onWfProcessWithdrawed(this, args);
            }
            else if (runtimeType == RuntimeManagerType.RejectRuntime && _onWfProcessRejected != null)
            {
                _onWfProcessRejected(this, args);
            }
            else if (runtimeType == RuntimeManagerType.ReverseRuntime && _onWfProcessReversed != null)
            {
                _onWfProcessReversed(this, args);
            }
        }

        /// <summary>
        /// 执行要办理的工作项
        /// </summary>
        /// <param name="ProcessInstanceEntity"></param>
        internal void ExecuteWorkItemIteraly(ActivityForwardContext executionContext,
            ISession session)
        {
            NodeMediator mediator = NodeMediatorFactory.CreateWorkItemMediator(executionContext, session);
            mediator.ExecuteWorkItem();

            //继续执行队列中的节点
            Queue<WorkItem> toDoQueue = mediator.ToDoAutoWorkItemQueue;
            while (toDoQueue.Count > 0)
            {
                //更新执行上下文数据
                WorkItem workItem = toDoQueue.Dequeue();
                executionContext.Activity = workItem.Activity;
                executionContext.ActivityInstance = workItem.ActivityInstance;

                //继续向前执行...
                NodeMediator nextMediator = NodeMediatorFactory.CreateWorkItemMediator(executionContext, session);
                nextMediator.ExecuteWorkItem();

                toDoQueue = nextMediator.ToDoAutoWorkItemQueue;
            }
        }
        #endregion

        #region WfRuntimeManager 创建执行实例的运行者对象
        /// <summary>
        /// 启动流程
        /// </summary>
        /// <param name="user"></param>
        /// <param name="appInstanceID"></param>
        /// <param name="processGUID"></param>
        /// <param name="nextActivityGUID"></param>
        /// <returns></returns>
        public WfRuntimeManager GetRuntimeInstanceStartup(WfAppRunner runner)
        {
            //检查流程是否可以被启动
            var pim = new ProcessInstanceManager();
            var processInstance = pim.GetRunningProcess(runner.AppName, runner.AppInstanceID, runner.ProcessGUID);
            if (processInstance != null)
            {
                throw new WfRuntimeException("当前应用已经有流程实例在运行中，除非终止或取消流程，否则流程不能被再次启动。");
            }

            this.AppRunner = runner;

            //获取流程第一个可办理节点
            this.ProcessModel = new ProcessModel(runner.ProcessGUID);
            var firstActivity = this.ProcessModel.GetFirstActivity();

            this.AppRunner.NextActivityPerformers = ActivityResource.CreateNextActivityPerformers(firstActivity.ActivityGUID,
                runner.UserID,
                runner.UserName);

            this.ActivityResource = new ActivityResource(runner.UserID, runner.UserName, this.AppRunner.NextActivityPerformers);

            return this;
        }

        /// <summary>
        /// 根据办理的业务数据，获取流程信息
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="appInstanceID"></param>
        /// <param name="processGUID"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="nextActivityPerformers"></param>
        /// <returns></returns>
        public WfRuntimeManager GetRuntimeInstanceAppRunning(WfAppRunner runner)
        {
            //检查传人参数是否有效
            if (string.IsNullOrEmpty(runner.AppName) || runner.AppInstanceID == 0 || runner.ProcessGUID == null)
            {
                throw new WfRuntimeException("方法参数错误，无法运行流程！");
            }

            //传递runner变量
            this.AppRunner = runner;
            
            //获取待办或者已经办理的任务
            var myTasks = (new TaskManager()).GetTaskOfMine(this.AppRunner).ToList();
            if (myTasks == null)
            {
                throw new WfRuntimeException("当前没有要办理的任务，无法运行流程!");
            }
            else if (myTasks.Count == 0)
            {
                throw new WfRuntimeException("当前没有要办理的任务，无法运行流程！");
            }
            else if (myTasks.Count > 1)
            {
                throw new WfRuntimeException("要办理的任务数目大于1，不是有效流程数据，无法运行流程！");
            }

            var task = myTasks[0];
            var processModel = new ProcessModel(task.ProcessGUID);
            var activityResource = new ActivityResource(runner.UserID, runner.UserName, runner.NextActivityPerformers, runner.Conditions);

            this.TaskView = task;
            this.ProcessModel = processModel;
            this.ActivityResource = activityResource;

            return this;
        }


        /// <summary>
        /// 根据任务ID, 创建流程执行时实例
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="currentLogonUser">当前登录用户</param>
        /// <param name="nextActivityPerformers">下一步活动的执行者</param>
        /// <param name="conditionKeyValuePair">条件key-value对</param>
        /// <returns></returns>
        public WfRuntimeManager GetRuntimeInstanceTaskRunning(long taskID,
            int userID,
            string userName,
            IDictionary<Guid, PerformerList> nextActivityPerformers)
        {
            var task = (new TaskManager()).GetTaskView(taskID);
            var processModel = new ProcessModel(task.ProcessGUID);
            var activityResource = new ActivityResource(userID, userName, nextActivityPerformers, null);

            this.TaskView = task;
            this.AppRunner.UserID = userID;
            this.AppRunner.UserName = userName;
            this.AppRunner.NextActivityPerformers = nextActivityPerformers;
            this.ProcessModel = processModel;
            this.ActivityResource = activityResource;

            return this;
        }

        public WfRuntimeManager GetRuntimeInstanceWithdraw(string appName,
            int appInstanceID,
            Guid processGUID,
            int userID,
            string userName)
        {
            //是否可撤销的条件检查
            var lastTaskTransitionInstance = (new TransitionInstanceManager()).GetLastTaskTransition(appName, appInstanceID, processGUID);
            if (lastTaskTransitionInstance.FromActivityType != (short)NodeTypeEnum.TaskNode)
            {
                throw new WfRuntimeException("当前撤销位置节点不是任务节点，无法撤销回到上一步！");
            }
            else if (lastTaskTransitionInstance.TransitionType == (short)TransitionTypeEnum.Loop)
            {
                throw new WfRuntimeException("当前流转是自循环，无需撤销！");
            }

            var aim = new ActivityInstanceManager();
            var withdrawActivityInstance = aim.GetById(lastTaskTransitionInstance.FromActivityInstanceGUID);
            if (withdrawActivityInstance.EndedByUserID.Value != userID)
            {
                throw new WfRuntimeException(string.Format("上一步节点的任务办理人跟当前登录用户不一致，无法撤销回上一步！节点办理人：{0}",
                    withdrawActivityInstance.EndedByUserName));
            }
            else if (withdrawActivityInstance.State != (short)NodeStateEnum.Completed)
            {
                throw new WfRuntimeException(string.Format("上一步节点的任务办理状态不是完成状态，无法撤销回来！上一步节点最终状态：{0}",
                    withdrawActivityInstance.State));
            }

            var acceptedActivityInstance = aim.GetById(lastTaskTransitionInstance.ToActivityInstanceGUID);
            if (acceptedActivityInstance.State != (short)NodeStateEnum.Ready)
            {
                throw new WfRuntimeException(string.Format("接收节点的状态不在准备状态，无法撤销到上一步，接收节点状态：{0}",
                   acceptedActivityInstance.State));
            }

            //准备撤销节点的相关信息
            var processModel = (new ProcessModel(processGUID));
            this.ProcessModel = processModel;
            this.AppRunner.ProcessGUID = processGUID;
            this.BackwardContext.ProcessInstance = (new ProcessInstanceManager()).GetById(lastTaskTransitionInstance.ProcessInstanceGUID);
            this.BackwardContext.BackwardToTargetTransition = processModel.GetTransition(lastTaskTransitionInstance.TransitionGUID);
            this.BackwardContext.BackwardToTaskActivity = processModel.GetActivity(lastTaskTransitionInstance.FromActivityGUID);
            this.BackwardContext.BackwardToTaskActivityInstance = withdrawActivityInstance;
            this.BackwardContext.FromActivity = processModel.GetActivity(acceptedActivityInstance.ActivityGUID);
            this.BackwardContext.FromActivityInstance = acceptedActivityInstance; //准备状态的接收节点

            //封装AppUser对象
            this.AppRunner.AppName = appName;
            this.AppRunner.AppInstanceID = appInstanceID;
            this.AppRunner.UserID = userID;
            this.AppRunner.UserName = userName;
            this.AppRunner.NextActivityPerformers = ActivityResource.CreateNextActivityPerformers(lastTaskTransitionInstance.FromActivityGUID,
                userID,
                userName);
            this.ActivityResource = new ActivityResource(userID, userName, this.AppRunner.NextActivityPerformers);


            return this;
        }

        public WfRuntimeManager GetRuntimeInstanceReject(string appName,
            int appInstanceID,
            Guid processGUID,
            int userID,
            string userName)
        {
            //是否可退回的条件检查
            var lastTaskTransitionInstance = (new TransitionInstanceManager()).GetLastTaskTransition(appName, appInstanceID, processGUID);
            if (lastTaskTransitionInstance.ToActivityType != (short)NodeTypeEnum.TaskNode)
            {
                throw new WfRuntimeException("最后流转记录的接收节点不是任务节点，无法退回上一步节点！");
            }
            else if (lastTaskTransitionInstance.TransitionType == (short)TransitionTypeEnum.Loop)
            {
                throw new WfRuntimeException("当前流转是自循环，无需退回！");
            }

            //读取当前办理节点及要进行退回操作的节点
            var currentRunningActivityInstance = (new ActivityInstanceManager()).GetById(lastTaskTransitionInstance.ToActivityInstanceGUID);
            if (!(currentRunningActivityInstance.State == (short)NodeStateEnum.Ready 
                || currentRunningActivityInstance.State == (short)NodeStateEnum.Running))
            {
                throw new WfRuntimeException(string.Format("当前节点的状态不在运行状态，无法退回上一步节点！当前节点状态：{0}",
                    currentRunningActivityInstance.State));
            }

            //从任务表里查看退回操作人，是否是AssignedToUser列表中的一项，如果有，则表示有权退回。
            var mineNodes = (new TaskManager()).GetTaskOfMine(appInstanceID, processGUID, userID).ToList();
            if (mineNodes == null || mineNodes.Count != 1)
            {
                throw new WfRuntimeException("任务记录不存在，请查看是否是您有权办理此项任务，无法退回上一步节点！");
            }
           
            //设置退回节点的相关信息
            var rejectToActivityInstance = (new ActivityInstanceManager()).GetById(lastTaskTransitionInstance.FromActivityInstanceGUID);
            var processModel = (new ProcessModel(processGUID));
            this.ProcessModel = processModel;
            this.BackwardContext.ProcessInstance = (new ProcessInstanceManager()).GetById(lastTaskTransitionInstance.ProcessInstanceGUID);
            this.BackwardContext.BackwardToTaskActivity = processModel.GetActivity(rejectToActivityInstance.ActivityGUID);
            this.BackwardContext.BackwardToTaskActivityInstance = rejectToActivityInstance;
            this.BackwardContext.BackwardToTargetTransition = processModel.GetTransition(lastTaskTransitionInstance.TransitionGUID);
            this.BackwardContext.FromActivity = processModel.GetActivity(currentRunningActivityInstance.ActivityGUID);
            this.BackwardContext.FromActivityInstance = currentRunningActivityInstance;

            //封装AppUser对象
            this.AppRunner.AppName = appName;
            this.AppRunner.AppInstanceID = appInstanceID;
            this.AppRunner.ProcessGUID = processGUID;
            this.AppRunner.UserID = userID;
            this.AppRunner.UserName = userName;
            this.AppRunner.NextActivityPerformers = ActivityResource.CreateNextActivityPerformers(lastTaskTransitionInstance.FromActivityGUID,
                rejectToActivityInstance.EndedByUserID.Value,
                rejectToActivityInstance.EndedByUserName);
            this.ActivityResource = new ActivityResource(userID, userName, this.AppRunner.NextActivityPerformers);

            return this;
        }

        /// <summary>
        /// 流程返签，先检查约束条件，然后调用wfruntimeinstance执行
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="appInstanceID"></param>
        /// <param name="processGUID"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public WfRuntimeManager GetRuntimeInstanceReverse(string appName,
            int appInstanceID,
            Guid processGUID,
            int userID,
            string userName)
        {
            var pim = new ProcessInstanceManager();
            var processInstance = pim.GetCompletedProcess(appName, appInstanceID, processGUID);
            if (processInstance == null)
            {
                throw new WfRuntimeException(string.Format("当前应用:{0}，实例ID：{0}, 没有办结的流程实例，无法让流程重新运行！",
                    appName, appInstanceID));
            }

            var processModel = new ProcessModel(processGUID);
            var endTransitionInstance = (new TransitionInstanceManager()).GetEndTransition(appName, appInstanceID, processGUID);
            var lastTaskActivity = processModel.GetActivity(endTransitionInstance.FromActivityGUID);
            var lastTaskActivityInstace = (new ActivityInstanceManager()).GetById(endTransitionInstance.FromActivityInstanceGUID);
            var lastToEndTransition = processModel.GetForwardTransition(lastTaskActivity.ActivityGUID);

            if (!lastTaskActivityInstace.EndedByUserID.HasValue)
            {
                throw new WfRuntimeException("流程最后一步的办理人员为空，不正常的流程数据，无法使结束的流程回退！");
            }

            var endActivity = processModel.GetActivity(endTransitionInstance.ToActivityGUID);
            var endActivityInstance = (new ActivityInstanceManager()).GetById(endTransitionInstance.ToActivityInstanceGUID);

            this.AppRunner.NextActivityPerformers = ActivityResource.CreateNextActivityPerformers(endTransitionInstance.FromActivityGUID,
                lastTaskActivityInstace.EndedByUserID.Value,
                lastTaskActivityInstace.EndedByUserName);

            this.ActivityResource = new ActivityResource(userID, userName, this.AppRunner.NextActivityPerformers);
            this.AppRunner.AppName = appName;
            this.AppRunner.AppInstanceID = appInstanceID;
            this.AppRunner.ProcessGUID = processGUID;
            this.AppRunner.UserID = userID;
            this.AppRunner.UserName = userName;

            this.BackwardContext.ProcessInstance = processInstance;
            this.BackwardContext.BackwardToTaskActivity = lastTaskActivity;
            this.BackwardContext.BackwardToTaskActivityInstance = lastTaskActivityInstace;
            this.BackwardContext.BackwardToTargetTransition = lastToEndTransition;
            this.BackwardContext.FromActivity = endActivity;
            this.BackwardContext.FromActivityInstance = endActivityInstance;
            return this;
        }

        #endregion
    }
}

            