using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Common;
using Sys.Workflow.Engine;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 工作流服务接口
    /// </summary>
    public interface IWorkflowService
    {
        /// <summary>
        /// 启动历程
        /// </summary>
        /// <param name="starter">启动人</param>
        /// <returns></returns>
        WfExecutedResult StartProcess(WfAppRunner starter);

        /// <summary>
        /// 获取下一步办理节点树
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        IList<NodeView> GetNextActivityTree(long taskID, IDictionary<string, string> condition = null);

        /// <summary>
        /// 获取下一步办理节点树
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        IList<NodeView> GetNextActivityTree(WfAppRunner runner, IDictionary<string, string> condition = null);

        /// <summary>
        /// 运行流程（根据业务数据运行流程）
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        WfExecutedResult RunProcessApp(WfAppRunner runner);

        /// <summary>
        /// 运行流程(根据任务信息运行流程)
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        WfExecutedResult RunProcessTask(WfTaskRunner runner);

        /// <summary>
        /// 流程返签
        /// </summary>
        /// <param name="starter">返签人</param>
        /// <returns></returns>
        WfExecutedResult ReverseProcess(WfAppRunner starter);

        /// <summary>
        /// 流程撤销回当前节点：将下一步节点收回
        /// </summary>
        /// <param name="withdrawer"></param>
        /// <returns></returns>
        WfExecutedResult WithdrawProcess(WfAppRunner withdrawer);

        /// <summary>
        /// 流程退回上一步：由当前节点发起
        /// </summary>
        /// <param name="rejector"></param>
        /// <returns></returns>
        WfExecutedResult RejectProcess(WfAppRunner rejector);

        /// <summary>
        /// 流程取消：运行状态置为取消状态
        /// </summary>
        /// <param name="canceler"></param>
        /// <returns></returns>
        bool CancelProcess(WfAppRunner canceler);

        /// <summary>
        /// 流程废弃：流程由运行、完成状态置为废弃状态
        /// </summary>
        /// <param name="discarder"></param>
        /// <returns></returns>
        bool DiscardProcess(WfAppRunner discarder);

        ///// <summary>
        ///// 流程重置：流程先取消，后重新开始启动流程
        ///// </summary>
        ///// <param name="reseter"></param>
        ///// <returns></returns>
        //WfExecutedResult ResetProcess(WfAppRunner reseter);
        
        /// <summary>
        /// 流程一体化测试
        /// </summary>
        /// <param name="initiator"></param>
        /// <returns></returns>
        WfExecutedResult StartupRunningEnd(WfAppRunner initiator);

        /// <summary>
        /// 设置任务为已阅读
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        bool ReadTask(WfTaskRunner runner);

        /// <summary>
        /// 获取运行任务列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IList<TaskViewEntity> GetRunningTasks(TaskQueryEntity query);

        /// <summary>
        /// 获取待办任务列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IList<TaskViewEntity> GetReadyTasks(TaskQueryEntity query);

        /// <summary>
        /// 获取流程实例数据
        /// </summary>
        /// <param name="processInstanceGUID"></param>
        /// <returns></returns>
        ProcessInstanceEntity GetProcessInstance(Guid processInstanceGUID);

        /// <summary>
        /// 获取活动实例数据
        /// </summary>
        /// <param name="activityInstanceGUID"></param>
        /// <returns></returns>
        ActivityInstanceEntity GetActivityInstance(Guid activityInstanceGUID);
    }
}
