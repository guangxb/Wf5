using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Sys.Workflow.Engine;
using Sys.Workflow.Common;
using Sys.Workflow.Utility;
using Sys.Workflow.DataModel;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 工作流服务(执行部分)
    /// </summary>
    public partial class WorkflowService : IWorkflowService
    {
        #region 流程启动
        private AutoResetEvent waitHandler = new AutoResetEvent(false);
        private WfExecutedResult _wfExecutedResult;
        /// <summary>
        /// 启动流程
        /// </summary>
        /// <param name="starter"></param>
        /// <returns></returns>
        public WfExecutedResult StartProcess(WfAppRunner starter)
        {
            try
            {
                WfRuntimeManager runtimeManager = new WfRuntimeManagerStartup();
                var runtimeInstance = runtimeManager.GetRuntimeInstanceStartup(starter);

                runtimeInstance.OnWfProcessCreated += runtimeInstance_OnWfProcessCreated;
                runtimeInstance.Execute();

                //do something else here...

                waitHandler.WaitOne();
            }
            catch (WfRuntimeException e)
            {
                throw new WorkflowException(string.Format("流程启动发生错误，内部异常:{0}", e.Message), e); 
            }
            
            //查看是否生成新流程实例
            if (_wfExecutedResult.ProcessInstanceGUID == null)
            {
                throw new WorkflowException("流程启动失败，获取不到运行的流程实例!"); 
            }
            return _wfExecutedResult;
        }

        private void runtimeInstance_OnWfProcessCreated(object sender, WfEventArgs args)
        {
            _wfExecutedResult = args.WfExecutedResult;
            waitHandler.Set();
        }

        ///// <summary>
        ///// 流程重置：流程先取消，后重新开始启动流程
        ///// </summary>
        ///// <param name="reseter"></param>
        ///// <returns></returns>
        //public WfExecutedResult ResetProcess(WfAppRunner reseter)
        //{
        //    //检查是否有运行中的流程，如果有运行中的流程，则必须先取消
        //    var pim = new ProcessInstanceManager();
        //    var entity = pim.GetRunningProcess(reseter.AppName, reseter.AppInstanceID, reseter.ProcessGUID);
        //    if (entity != null)
        //    {
        //        throw new WorkflowException("当前单据有流程实例处于运行状态，必须先取消流程，才可以重置！"); 
        //    }

        //    //当前无运行中的流程，可以再次启动流程
        //    StartProcess(reseter);

        //    return _wfExecutedResult;
        //}
        #endregion

        #region 运行流程
        /// <summary>
        /// 根据应用获取流程下一步节点列表
        /// </summary>
        /// <param name="runner">应用执行人</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public IList<NodeView> GetNextActivityTree(WfAppRunner runner, IDictionary<string, string> condition = null)
        {
            var tm = new TaskManager();
            return tm.GetNextActivityTree(runner, condition);
        }

        /// <summary>
        /// 获取下一步活动列表树
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public IList<NodeView> GetNextActivityTree(long taskID, IDictionary<string, string> condition = null)
        {
            var task = (new TaskManager()).GetTaskView(taskID);
            var processModel = new ProcessModel(task.ProcessGUID);
            var nextSteps = processModel.GetNextActivityTree(task.ActivityGUID, condition);
            return nextSteps;
        }

       
        /// <summary>
        /// 运行流程(业务处理)
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        public WfExecutedResult RunProcessApp(WfAppRunner runner)
        {
            try
            {
                WfRuntimeManager runtimeManager = new WfRuntimeManagerAppRunning();
                var runtimeInstance = runtimeManager.GetRuntimeInstanceAppRunning(runner);

                runtimeInstance.OnWfProcessContinued += runtimeInstance_OnWfProcessContinued;
                bool isRunning = runtimeInstance.Execute();

                waitHandler.WaitOne();

                return _wfExecutedResult;
            }
            catch (WfRuntimeException e)
            {
                throw new WorkflowException(string.Format("流程运行时发生异常！，详细错误：{0}", e.Message), e);
            }
        }

        /// <summary>
        /// 运行流程(待办任务办理)
        /// </summary>
        /// <param name="continuer"></param>
        public WfExecutedResult RunProcessTask(WfTaskRunner runner)
        {
            try
            {
                WfRuntimeManager runtimeManager = new WfRuntimeManagerTaskRunning();
                var runtimeInstance = runtimeManager.GetRuntimeInstanceTaskRunning(runner.TaskID, runner.UserID, runner.UserName, runner.NextActivityPerformers);

                runtimeInstance.OnWfProcessContinued += runtimeInstance_OnWfProcessContinued;
                bool isRunning = runtimeInstance.Execute();

                waitHandler.WaitOne();

                return _wfExecutedResult;
            }
            catch (WfRuntimeException e)
            {
                throw new WorkflowException(string.Format("流程运行时发生异常！，详细错误：{0}", e.Message), e);
            }
        }

        private void runtimeInstance_OnWfProcessContinued(object sender, WfEventArgs args)
        {
            _wfExecutedResult = args.WfExecutedResult;
            waitHandler.Set();
        }
        #endregion

        #region 流程撤销、回退和返签（已经结束的流程可以被复活）
        /// <summary>
        /// 流程撤销
        /// </summary>
        /// <param name="recaller"></param>
        /// <returns></returns>
        public WfExecutedResult WithdrawProcess(WfAppRunner recaller)
        {
            try
            {
                WfRuntimeManager runtimeManager = new WfRuntimeManagerWithdraw();
                var runtimeInstance = runtimeManager.GetRuntimeInstanceWithdraw(recaller.AppName, recaller.AppInstanceID,
                    recaller.ProcessGUID, recaller.UserID, recaller.UserName);

                runtimeInstance.OnWfProcessWithdrawed += runtimeInstance_OnWfProcessWithdrawed;
                bool isWithdrawed = runtimeInstance.Execute();

                waitHandler.WaitOne();

                return _wfExecutedResult;
            }
            catch (WfRuntimeException e)
            {
                throw new WorkflowException(string.Format("流程撤销发生异常！，详细错误：{0}", e.Message), e);
            }

        }

        private void runtimeInstance_OnWfProcessWithdrawed(object sender, WfEventArgs args)
        {
            _wfExecutedResult = args.WfExecutedResult;
            waitHandler.Set();
        }

        /// <summary>
        /// 退回流程
        /// </summary>
        /// <param name="rejector"></param>
        /// <returns></returns>
        public WfExecutedResult RejectProcess(WfAppRunner rejector)
        {
            try
            {
                WfRuntimeManager runtimeManager = new WfRuntimeManagerReject();
                var runtimeInstance = runtimeManager.GetRuntimeInstanceReject(rejector.AppName, rejector.AppInstanceID,
                    rejector.ProcessGUID, rejector.UserID, rejector.UserName);

                runtimeInstance.OnWfProcessRejected += runtimeInstance_OnWfProcessRejected;
                bool isRejected = runtimeInstance.Execute();

                waitHandler.WaitOne();

                return _wfExecutedResult;
            }
            catch (WfRuntimeException e)
            {
                throw new WorkflowException(string.Format("流程退回发生异常！，详细错误：{0}", e.Message), e);
            }
        }

        private void runtimeInstance_OnWfProcessRejected(object sender, WfEventArgs args)
        {
            _wfExecutedResult = args.WfExecutedResult;
            waitHandler.Set();
        }

        /// <summary>
        /// 流程返签
        /// </summary>
        /// <param name="ender"></param>
        /// <returns></returns>
        public WfExecutedResult ReverseProcess(WfAppRunner ender)
        {
            try
            {
                WfRuntimeManager runtimeManager = new WfRuntimeManagerReverse();
                var runtimeInstance = runtimeManager.GetRuntimeInstanceReverse(ender.AppName, ender.AppInstanceID, ender.ProcessGUID,
                    ender.UserID, ender.UserName);

                runtimeInstance.OnWfProcessReversed += runtimeInstance_OnWfProcessReversed;
                bool isReversed = runtimeInstance.Execute();

                waitHandler.WaitOne();

                return _wfExecutedResult;
            }
            catch (WfRuntimeException e)
            {
                throw new WorkflowException(string.Format("流程返签发生异常！，详细错误：{0}", e.Message), e);
            }
        }

        private void runtimeInstance_OnWfProcessReversed(object sender, WfEventArgs args)
        {
            _wfExecutedResult = args.WfExecutedResult;
            waitHandler.Set();
        }

        /// <summary>
        /// 一个流程的完整测试（开始 -> 运行 -> 撤销 -> 运行 -> 退回 -> 运行 -> 结束 -> 返签 -> 运行 -> 结束）
        /// </summary>
        /// <param name="initiator"></param>
        /// <returns></returns>
        public WfExecutedResult StartupRunningEnd(WfAppRunner initiator)
        {
            //流程开始->业务员提交
            StartProcess(initiator);

            //业务员提交->板房签字
            var banFangNodeGuid = "fc8c71c5-8786-450e-af27-9f6a9de8560f";
            PerformerList pList = new PerformerList();
            pList.Add(new Performer(20, "Zhang"));

            initiator.NextActivityPerformers = new Dictionary<Guid, PerformerList>();
            initiator.NextActivityPerformers.Add(Guid.Parse(banFangNodeGuid), pList);
            RunProcessApp(initiator);

            //板房签字->业务员签字
            //登录用户身份
            initiator.UserID = 20;
            initiator.UserName = "Zhang";

            var salesGuid = "39c71004-d822-4c15-9ff2-94ca1068d745";
            pList.Clear();
            pList.Add(new Performer(10, "Long"));

            initiator.NextActivityPerformers.Clear();
            initiator.NextActivityPerformers.Add(Guid.Parse(salesGuid), pList);
            RunProcessApp(initiator);

            //业务员签字->结束
            //登录用户身份
            initiator.UserID = 10;
            initiator.UserName = "Lhang";

            var endGuid = "b70e717a-08da-419f-b2eb-7a3d71f054de";
            pList.Clear();
            pList.Add(new Performer(10, "Long"));

            initiator.NextActivityPerformers.Clear();
            initiator.NextActivityPerformers.Add(Guid.Parse(endGuid), pList);
            RunProcessApp(initiator);

            return _wfExecutedResult;
        }
        #endregion

        #region 取消（运行的）流程、废弃执行中或执行完的流程
        /// <summary>
        /// 取消流程
        /// </summary>
        /// <param name="canceller"></param>
        /// <returns></returns>
        public bool CancelProcess(WfAppRunner canceller)
        {
            var pim = new ProcessInstanceManager();
            return pim.Cancel(canceller.AppName, canceller.AppInstanceID, canceller.ProcessGUID, 
                canceller.UserID, canceller.UserName);
        }

        /// <summary>
        /// 废弃流程
        /// </summary>
        /// <param name="discarder"></param>
        /// <returns></returns>
        public bool DiscardProcess(WfAppRunner discarder)
        {
            var pim = new ProcessInstanceManager();
            return pim.Discard(discarder.AppName, discarder.AppInstanceID, discarder.ProcessGUID, discarder.UserID, discarder.UserName);
        }
        #endregion


        #region 任务读取和处理
        /// <summary>
        /// 设置任务为已读状态
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        public bool ReadTask(WfTaskRunner runner)
        {
            bool isRead = false;
            try
            {
                var taskManager = new TaskManager();
                taskManager.Read(runner.TaskID, runner.UserID, runner.UserName);
                isRead = true;
            }
            catch (System.Exception)
            {
                throw;
            }

            return isRead;
        }

        public IList<TaskViewEntity> GetRunningTasks(TaskQueryEntity query)
        {
            int allRowsCount = 0;
            var taskManager = new TaskManager();
            return taskManager.GetRunningTasks(query, out allRowsCount).ToList();
        }

        public IList<TaskViewEntity> GetReadyTasks(TaskQueryEntity query)
        {
            int allRowsCount = 0;
            var taskManager = new TaskManager();
            return taskManager.GetReadyTasks(query, out allRowsCount).ToList();
        }
        #endregion
    }
}