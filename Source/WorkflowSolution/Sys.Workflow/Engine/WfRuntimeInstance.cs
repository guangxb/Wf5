//using System;
//using System.Threading;
//using System.Data.Linq;
//using System.Transactions;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Sys.Workflow.Common;
//using Sys.Workflow.DataModel;
//using Sys.Workflow.Business;
//using Sys.Workflow.Utility;

//namespace Sys.Workflow.Engine
//{
//    /// <summary>
//    /// 流程运行类
//    /// </summary>
//    public class WfRuntimeInstance
//    {
//        #region 属性列表
//        private string _appName;
//        private string _appInstanceID;
//        private long _taskID;
//        private ActivityResource _activityResource;
//        private Guid _processGUID;
//        private ProcessModel _processModel;
//        private ActivityEntity _lastTaskActivity;
//        private ActivityInstanceEntity _lastTaskActivityInstance;
//        private ProcessInstanceEntity _processInstance;
//        private TransitionEntity _lastToEndTransiton;

//        public WfRuntimeManager WfRuntimeManager { get; set; }

//        /// <summary>
//        /// 工作项执行结果
//        /// </summary>
//        internal WorkItemExecutedResult WorkItemExecutedResult { get; set; }

//        /// <summary>
//        /// 下一步活动列表的匹配结果
//        /// </summary>
//        internal NextActivityMatchedResult NextActivityMatchedResult { get; set; }
//        #endregion

//        #region 流程事件定义
//        /// <summary>
//        /// 流程创建事件
//        /// </summary>
//        private event EventHandler<WfProcessCreatedEventArgs> _wfProcessCreated;
//        public event EventHandler<WfProcessCreatedEventArgs> WfProcessCreated
//        {
//            add
//            {
//                _wfProcessCreated += value;
//            }
//            remove
//            {
//                _wfProcessCreated -= value;
//            }
//        }

//        /// <summary>
//        /// 流程继续流转事件
//        /// </summary>
//        private event EventHandler<WfProcessContinuedEventArgs> _wfProcessContinued;
//        public event EventHandler<WfProcessContinuedEventArgs> WfProcessContinued
//        {
//            add
//            {
//                _wfProcessContinued += value;
//            }
//            remove
//            {
//                _wfProcessContinued -= value;
//            }
//        }
//        #endregion

//        #region 构造函数
//        /// <summary>
//        /// 启动流程的构造方法
//        /// </summary>
//        /// <param name="appName"></param>
//        /// <param name="appInstanceID"></param>
//        /// <param name="processGUID"></param>
//        /// <param name="processModel"></param>
//        /// <param name="activityResource"></param>
//        public WfRuntimeInstance(string appName,
//            string appInstanceID, 
//            Guid processGUID,
//            ProcessModel processModel,
//            ActivityResource activityResource)
//        {
//            _appName = appName;
//            _appInstanceID = appInstanceID;
//            _processGUID = processGUID;
//            _processModel = processModel;
//            _activityResource = activityResource;
//        }

//        /// <summary>
//        /// 已经运行流程的构造方法
//        /// </summary>
//        /// <param name="taskID"></param>
//        /// <param name="processModel"></param>
//        /// <param name="activityResource"></param>
//        public WfRuntimeInstance(long taskID,
//            ProcessModel processModel,
//            ActivityResource activityResource)
//        {
//            _taskID = taskID;
//            _processGUID = processModel.ProcessGUID;
//            _processModel = processModel;
//            _activityResource = activityResource;
//        }


//        /// <summary>
//        /// 已经结束流程返签的构造方法
//        /// </summary>
//        /// <param name="appName"></param>
//        /// <param name="appInstanceID"></param>
//        /// <param name="processGUID"></param>
//        /// <param name="processModel"></param>
//        /// <param name="processInstance"></param>
//        /// <param name="activityResource"></param>
//        public WfRuntimeInstance(string appName,
//            string appInstanceID,
//            Guid processGUID,
//            ProcessModel processModel,
//            ProcessInstanceEntity processInstance,
//            ActivityEntity lastTaskActivity,
//            ActivityInstanceEntity lastTaskActivityInstance,
//            TransitionEntity lastToEndTransition,
//            ActivityResource activityResource)
//        {
//            _appName = appName;
//            _appInstanceID = appInstanceID;
//            _processGUID = processGUID;
//            _processModel = processModel;
//            _lastTaskActivity = lastTaskActivity;
//            _lastTaskActivityInstance = lastTaskActivityInstance;
//            _lastToEndTransiton = lastToEndTransition;
//            _processInstance = processInstance;
//            _activityResource = activityResource;
//        }
//        #endregion

//        #region 流程启动
//        /// <summary>
//        /// 流程启动执行
//        /// </summary>
//        /// <returns></returns>
//        public bool StartProcess()
//        {
//            try
//            {
//                Thread thread = new Thread(StartupProcessInstance);
//                thread.Start();

//                return true;
//            }
//            catch (System.Exception ex)
//            {
//                throw;
//            }
//        }

//        /// <summary>
//        /// 启动流程
//        /// </summary>
//        internal void StartupProcessInstance()
//        {
//            ISession session = SessionFactory.CreateSession();
//            try
//            {
//                //构造流程实例
//                var processInstance = new ProcessInstanceManager()
//                    .CreateNewProcessInstanceObject(_appName, _appInstanceID, 
//                    _processModel.ProcessEntity, 
//                    _activityResource.LogonUser.UserID,
//                    _activityResource.LogonUser.UserName);

//                //构造活动实例
//                //1. 获取开始节点活动
//                var startActivity = _processModel.GetStartActivity();

//                var startExecutionContext = new ActivityExecutionContextStart(_processModel,
//                    processInstance,
//                    startActivity,
//                    _activityResource);

//                ExecuteWorkItemIteraly(startExecutionContext, session);

//                if (_wfProcessCreated != null)
//                {
//                    var args = new WfProcessCreatedEventArgs();
//                    args.ProcessInstanceGUID = processInstance.ProcessInstanceGUID;
//                    args.WorkItemExecutedResult = WorkItemExecutedResult;
//                    args.NextActivityMatchedResult = NextActivityMatchedResult;
//                    _wfProcessCreated(this, args);
//                }

//                session.Commit();
//            }
//            catch (System.Exception e)
//            {
//                session.Rollback();
//                throw new WfRuntimeException("流程未能成功启动，请查看内部异常！", e);
//            }
//            finally
//            {
//                session.Dispose();
//            }
//        }
//        #endregion

//        #region 流程运行
//        /// <summary>
//        /// 流程执行
//        /// </summary>
//        /// <returns></returns>
//        public bool RunProcess()
//        {
//            try
//            {
//                var thread = new Thread(RunProcessInstance);
//                thread.Start();

//                return true;
//            }
//            catch (WfRuntimeException)
//            {
//                throw;
//            }
//            catch (System.Exception)
//            {
//                throw;
//            }
//        }

//        /// <summary>
//        /// 流程运行
//        /// </summary>
//        internal void RunProcessInstance()
//        {
//            ISession session = SessionFactory.CreateSession();

//            try
//            {
//                var runningExecutionContext = new ActivityExecutionContexttRunning(_taskID,
//                    _processModel,
//                    _activityResource);

//                //判断流程是否可以被运行
//                if (runningExecutionContext.ProcessInstance.RunningState != (short)ProcessStateEnum.Running)
//                {
//                    throw new WfRuntimeException(string.Format("当期流程不在运行状态！详细信息：当前流程状态：{0}", runningExecutionContext.ProcessInstance.RunningState));
//                }

//                var tm = new TaskManager();
//                var taskView = tm.GetById(_taskID);
//                if (taskView.ActivityState != (short)NodeStateEnum.Running)
//                {
//                    throw new WfRuntimeException(string.Format("当期当前活动节点不在运行状态！详细信息：当前活动状态：{0}", taskView.ActivityState));
//                }

//                //执行节点
//                ExecuteWorkItemIteraly(runningExecutionContext, session);

//                if (_wfProcessContinued != null)
//                {
//                    var args = new WfProcessContinuedEventArgs();
//                    args.ProcessInstanceGUID = runningExecutionContext.ProcessInstance.ProcessInstanceGUID;
//                    args.WorkItemExecutedResult = WorkItemExecutedResult;
//                    args.NextActivityMatchedResult = NextActivityMatchedResult;
//                    _wfProcessContinued(this, args);
//                }

//                session.Commit();
//            }
//            catch (System.Exception e)
//            {
//                session.Rollback();
//                throw new WfRuntimeException("流程未能成功执行，请查看内部异常！", e);
//            }
//            finally
//            {
//                session.Dispose();
//            }
//        }

//        /// <summary>
//        /// 执行要办理的工作项
//        /// </summary>
//        /// <param name="ProcessInstanceEntity"></param>
//        internal void ExecuteWorkItemIteraly(ActivityExecutionContext activityExecutionContext, 
//            ISession session)
//        {
//            NodeMediator mediator = NodeMediatorFactory.CreateWorkItemMediator(activityExecutionContext, session);
//            mediator.ExecuteWorkItem();

//            //继续执行队列中的节点
//            Queue<WorkItem> toDoQueue = mediator.ToDoAutoWorkItemQueue;
//            while (toDoQueue.Count > 0)
//            {
//                //更新执行上下文数据
//                WorkItem workItem = toDoQueue.Dequeue();
//                activityExecutionContext.Activity = workItem.Activity;
//                activityExecutionContext.ActivityInstance = workItem.ActivityInstance;

//                //继续向前执行...
//                NodeMediator nextMediator = NodeMediatorFactory.CreateWorkItemMediator(activityExecutionContext, session);
//                nextMediator.ExecuteWorkItem();

//                toDoQueue = nextMediator.ToDoAutoWorkItemQueue;
//            }
//        }
//        #endregion
//        /// <summary>
//        /// 流程返签
//        /// </summary>
//        /// <returns></returns>
//        public bool ReverseProcess()
//        {
//            try
//            {
//                Thread thread = new Thread(ReverseProcessInstance);
//                thread.Start();
//                return true;
//            }
//            catch (System.Exception ex)
//            {
//                throw;
//            }
//        }


//        /// <summary>
//        /// 流程返签执行逻辑
//        /// </summary>
//        internal void ReverseProcessInstance()
//        {
//            ISession session = SessionFactory.CreateSession();

//            try
//            {
//                //修改流程实例为返签状态
//                var pim = new ProcessInstanceManager();
//                pim.Reverse(_processInstance.ProcessInstanceGUID, _activityResource.LogonUser, session);

//                var endExecutionContext = new ActivityExecutionContextEnd(_processModel, _processInstance, 
//                    _lastTaskActivity, _activityResource);

//                //创建新任务节点
//                var workItem = (WorkItem)WorkItemNodeFactory.CreateNewNode(_lastTaskActivity);
//                workItem.CreateNewWorkItemWithTransitionInstance(_processInstance,
//                    _lastTaskActivityInstance,
//                    _lastToEndTransiton,
//                    TransitionTypeEnum.Backward,
//                    _activityResource,
//                    session);
//            }
//            catch (System.Exception e)
//            {
//                session.Rollback();
//                throw new WfRuntimeException("流程未能成功返签，请查看内部异常！", e);
//            }
//            finally
//            {
//                session.Dispose();
//            }
//        }
//    }
//}