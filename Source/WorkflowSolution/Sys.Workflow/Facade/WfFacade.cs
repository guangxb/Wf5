using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Sys.Workflow.Common;
using Sys.Workflow.Utility;
using Sys.Workflow.DataModel;
using Sys.Workflow.Business;

namespace Sys.Workflow
{
    /// <summary>
    /// 工作流提供给外部的接口类
    /// </summary>
    public partial class WfFacade
    {
        #region 流程定义提供的对外接口
        /// <summary>
        /// 获取流程的第一个任务节点信息
        /// </summary>
        /// <param name="processGUID"></param>
        /// <returns></returns>
        public static ActivityEntity GetFirstActivity(Guid processGUID)
        {
            try
            {
                var processModel = new ProcessModel(processGUID);
                return processModel.GetFirstActivity();
            }
            catch (System.Exception)
            {
                //throw new WfDataException(string.Format("获取流程节点信息失败，详细信息：{0}", ex.Message), ex);
                throw;
            }
        }

        /// <summary>
        /// 获取活动的定义数据
        /// </summary>
        /// <param name="processGUID"></param>
        /// <param name="activityGuid"></param>
        /// <returns></returns>
        public static ActivityEntity GetActivity(Guid processGUID,
            Guid activityGuid)
        {
            try
            {
                var processModel = new ProcessModel(processGUID);
                return processModel.GetActivity(activityGuid);
            }
            catch (System.Exception)
            {
                //throw new WfDataException(string.Format("获取流程节点信息失败，详细信息：{0}", ex.Message), ex);
                throw;
            }
        }

        /// <summary>
        /// 获取当前活动的下一活动的列表
        /// </summary>
        /// <param name="processGUID">流程定义string</param>
        /// <param name="activityGUID">活动定义string</param>
        /// <returns></returns>
        public static NextActivityMatchedResult GetNextActivityList(Guid processGUID,
            Guid ProcessInstanceGUID,
            Guid activityGUID)
        {
            try
            {
                return GetNextActivityList(processGUID, ProcessInstanceGUID, activityGUID, null);
            }
            catch (System.Exception)
            {
                //throw new WfDataException(string.Format("获取当前节点的下一节点信息失败，详细信息：{0}", ex.Message), ex);
                throw;
            }
        }

        public static NextActivityMatchedResult GetNextActivityList(Guid processGUID,
            Guid processInstanceGUID,
            Guid activityGUID,
            ConditionKeyValuePair conditionKeyValuePair)
        {
            try
            {
                //var pm = new ProcessManager();
                //var 
                var processModel = new ProcessModel(processGUID);
                var activity = processModel.GetActivity(activityGUID);

                return processModel.GetNextActivityList(processInstanceGUID, activity, conditionKeyValuePair);
            }
            catch (System.Exception)
            {
                //throw new WfDataException(string.Format("获取当前节点的下一节点信息失败，详细信息：{0}", ex.Message), ex);
                throw;
            }
        }

        public static NextActivityMatchedResult GetNextActivityList(Guid processGUID,
            Guid processInstanceGUID,
            Guid activityGUID,
            ConditionKeyValuePair conditionKeyValuePair,
            ActivityResource activityResource,
            Expression<Func<ActivityResource, ActivityEntity, bool>> expression)
        {
            try
            {
                var processModel = new ProcessModel(processGUID);
                var activity = processModel.GetActivity(activityGUID);

                return processModel.GetNextActivityList(processInstanceGUID, activity, conditionKeyValuePair, activityResource, expression);
            }
            catch (System.Exception)
            {
                //throw new WfDataException(string.Format("获取当前节点的下一节点信息失败，详细信息：{0}", ex.Message), ex);
                throw;
            }
        }

        /// <summary>
        /// 获取角色可以编辑的数据项列表
        /// </summary>
        /// <param name="processGUID">流程string</param>
        /// <param name="roleCode">角色Code</param>
        /// <returns>数据项的编码集合</returns>
        public static IList<string> GetRoleDataItems(Guid processGUID, string roleCode)
        {
            try
            {
                var processModel = new ProcessModel(processGUID);
                return processModel.GetRoleDataItems(roleCode);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        #endregion

        #region 流程运行对外提供的接口
        /// <summary>
        /// 获取一个流程实例下的任务列表
        /// </summary>
        /// <param name="appInstanceID">应用ID</param>
        /// <returns></returns>
        public static IEnumerable<TaskViewEntity> GetProcessTasks(int appInstanceID,
            Guid processInstanceGUID)
        {
            try
            {
                TaskManager tm = new TaskManager();
                return tm.GetProcessTasks(appInstanceID, processInstanceGUID);
            }
            catch (System.Exception)
            {
                //throw new WfDataException(string.Format("获取流程下的任务信息失败，详细信息：{0}", ex.Message), ex);
                throw;
            }
        }

        public static IEnumerable<TaskViewEntity> GetProcessTasksInReady(int appInstanceID,
            Guid processInstanceGUID)
        {
            try
            {
                TaskManager tm = new TaskManager();
                return tm.GetProcessTasksWithState(appInstanceID, processInstanceGUID, NodeStateEnum.Ready);
            }
            catch (System.Exception ex)
            {
                //throw new WfDataException(string.Format("获取流程下的任务信息失败，详细信息：{0}", ex.Message), ex);
                throw;
            }
        }

        public static IEnumerable<TaskViewEntity> GetProcessTasksInRunning(int appInstanceID,
            Guid processInstanceGUID)
        {
            try
            {
                TaskManager tm = new TaskManager();
                return tm.GetProcessTasksWithState(appInstanceID, processInstanceGUID, NodeStateEnum.Running);
            }
            catch (System.Exception)
            {
                //throw new WfDataException(string.Format("获取流程下的任务信息失败，详细信息：{0}", ex.Message), ex);
                throw;
            }
        }

        /// <summary>
        /// 获取一个应用下的，正在运行中的流程实例
        /// </summary>
        /// <param name="appInstanceID">应用实例Id</param>
        /// <param name="processGUID">流程定义ID</param>
        /// <returns></returns>
        public static IEnumerable<ProcessInstanceEntity> GetRunningProcess(string appName, string appInstanceID,
            Guid processGUID)
        {
            try
            {
                var pim = new ProcessInstanceManager();              
                return pim.GetRunningProcess(appName, appInstanceID, processGUID);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public static bool GetUserPermission(long appInstanceID,
            long currentUserID)
        {
            //TaskManager tm = new TaskManager();
            //return tm.GetUserPermission(appInstanceID);
            throw new Exception("not implement");
        }

        /// <summary>
        /// 取消流程执行
        /// </summary>
        /// <param name="processInstanceGUID"></param>
        public static bool AbortProcessInstance(Guid processInstanceGUID)
        {
            try
            {
                ProcessInstanceManager pim = new ProcessInstanceManager();
                return pim.Abort(processInstanceGUID);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 终止流程执行
        /// </summary>
        /// <param name="processInstanceGUID"></param>
        public static bool TerminateProcessInstance(Guid processInstanceGUID)
        {
            try
            {
                ProcessInstanceManager pim = new ProcessInstanceManager();
                return pim.Terminate(processInstanceGUID);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 删除流程实例（先要进行取消操作）
        /// </summary>
        /// <param name="processInstanceGUID"></param>
        /// <returns></returns>
        public static bool DeleteProcessInstance(Guid processInstanceGUID)
        {
            try
            {
                ProcessInstanceManager pim = new ProcessInstanceManager();
                return pim.Delete(processInstanceGUID);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 创建执行者列表
        /// </summary>
        /// <returns></returns>
        public static PerformerList CreatePerformerList()
        {
            return new PerformerList();
        }

        /// <summary>
        /// 创建活动-执行者列表
        /// </summary>
        /// <returns></returns>
        public static IDictionary<Guid, PerformerList> CreateActivityPerformerList()
        {
            return new Dictionary<Guid, PerformerList>();
        }


        /// <summary>
        /// 创建资源信息
        /// </summary>
        /// <param name="currentLogonUser"></param>
        /// <param name="nextActivityPerformers"></param>
        /// <returns></returns>
        public static ActivityResource CreateActivityResource(int userID, string userName,
            IDictionary<Guid, PerformerList> nextActivityPerformers,
            ConditionKeyValuePair conditionKeyValuePair = null)
        {
            return new ActivityResource(userID, userName, nextActivityPerformers, conditionKeyValuePair);
        }

        /// <summary>
        /// 创建条件表达式列表
        /// </summary>
        /// <returns></returns>
        public static ConditionKeyValuePair CreateConditionKeyValuePair()
        {
            return new ConditionKeyValuePair();
        }

        /// <summary>
        /// 创建用户对象
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static WfLogonUser CreateWfLogonUser(int userID, string userName)
        {
            return new WfLogonUser(userID, userName);
        }
        #endregion 
    }
}
