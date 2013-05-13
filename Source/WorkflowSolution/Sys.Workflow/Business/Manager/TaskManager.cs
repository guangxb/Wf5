using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Sys.Workflow.Common;
using Sys.Workflow.Utility;
using Sys.Workflow.DataModel;
using Sys.Workflow.DataModel.SQL;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 任务管理类：包括任务及任务视图对象
    /// </summary>
    public class TaskManager
    {
        #region TaskManager 属性列表
        private DataAccessManager _taskRepository;
        private DataAccessManager TaskRepository
        {
            get
            {
                if (_taskRepository == null)
                {
                    _taskRepository = DataAccessFactory.Instance();
                }
                return _taskRepository;
            }
        }

        private DataAccessManager _taskViewRepository;
        private DataAccessManager TaskViewRepository
        {
            get
            {
                if (_taskViewRepository == null)
                {
                    _taskViewRepository = DataAccessFactory.Instance();
                }
                return _taskViewRepository;
            }
        }

        #endregion

        #region TaskManager 任务分配视图
        public TaskViewEntity GetTaskView(long taskID)
        {
            return TaskViewRepository.GetById<TaskViewEntity>(taskID);
        }

        public TaskEntity GetTask(long taskID)
        {
            return TaskRepository.GetById<TaskEntity>(taskID);
        }


        #region TaskManager 获取当前用户的办理任务
        /// <summary>
        /// 获取当前用户运行中的任务
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        internal IEnumerable<TaskViewEntity> GetRunningTasks(TaskQueryEntity query, out int allRowsCount)
        {
            return GetTasksPaged(query, 2, out allRowsCount);
        }

        /// <summary>
        /// 获取当前用户待办的任务
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        internal IEnumerable<TaskViewEntity> GetReadyTasks(TaskQueryEntity query, out int allRowsCount)
        {
            return GetTasksPaged(query, 1, out allRowsCount);
        }

        /// <summary>
        /// 获取任务（分页）
        /// </summary>
        /// <param name="query"></param>
        /// <param name="activityState"></param>
        /// <returns></returns>
        private IEnumerable<TaskViewEntity> GetTasksPaged(TaskQueryEntity query, int activityState, out int allRowsCount)
        {
            //processState:2 -running 流程处于运行状态
            //activityType:4 -表示“任务”类型的节点
            //activityState: 1-ready（准备）, 2-running（）运行；

            ISession session = SessionFactory.CreateSession();
            string orderBySql = "ORDER BY TASKID DESC";
            string whereSql = string.Format(@"WHERE AppInstanceID={0} AND ProcessGUID='{1}' AND AssignedToUserID={2} 
                                AND ActivityState={3} AND ProcessState=2 AND ActivityType=4",
                                query.AppInstanceID, query.ProcessGUID, query.UserID, activityState);                                                        ;

            //如果数据记录数为0，则不用查询列表
            allRowsCount = TaskRepository.GetCount<TaskViewEntity>(whereSql, session.Connection, session.Transaction);
            if (allRowsCount == 0)
            {
                return null;
            }

            //查询列表数据并返回结果集
            var list = TaskRepository.GetPage<TaskViewEntity>(query.PageIndex, query.PageSize, whereSql,
                new
                {
                    appInstanceID = query.AppInstanceID,
                    processGUID = query.ProcessGUID,
                    activityState = activityState,
                    userID = query.UserID
                },
                orderBySql,
                session.Connection,
                session.Transaction);

            return list;
        }

        internal IEnumerable<TaskViewEntity> GetTaskOfMine(WfAppRunner runner)
        {
            ISession session = SessionFactory.CreateSession();
            return GetTaskOfMine(runner, session);
        }

        internal IEnumerable<TaskViewEntity> GetTaskOfMine(WfAppRunner runner, ISession session)
        {
            return GetTaskOfMine(runner.AppInstanceID, runner.ProcessGUID, runner.UserID, session);
        }

        internal IEnumerable<TaskViewEntity> GetTaskOfMine(int appInstanceID, Guid processGUID, int userID)
        {
            ISession session = SessionFactory.CreateSession();
            return GetTaskOfMine(appInstanceID, processGUID, userID, session);
        }

        /// <summary>
        /// 根据应用实例、流程GUID，办理用户Id获取任务列表
        /// </summary>
        /// <param name="appInstanceID"></param>
        /// <param name="processGUID"></param>
        /// <param name="userID"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        internal IEnumerable<TaskViewEntity> GetTaskOfMine(int appInstanceID, Guid processGUID, int userID, ISession session)
        {
            //processState:2 -running 流程处于运行状态
            //activityType:4 -表示“任务”类型的节点
            //activityState: 1-ready（准备）, 2-running（）运行；
            string sql = @"SELECT * FROM vwWfActivityInstanceTasks 
                           WHERE AppInstanceID=@appInstanceID AND ProcessGUID=@processGUID AND AssignedToUserID=@userID 
                                AND ProcessState=2 AND ActivityType=4 AND (ActivityState=1 OR ActivityState=2) 
                           ORDER BY TASKID DESC";
            var list = TaskRepository.Query<TaskViewEntity>(sql,
                new
                {
                    appInstanceID = appInstanceID,
                    processGUID = processGUID,
                    userID = userID
                },
                session.Connection,
                session.Transaction);

            return list;
        }
        #endregion


        /// <summary>
        /// 获取流程实例下的任务数据
        /// </summary>
        /// <param name="appInstanceID">应用ID</param>
        /// <param name="ProcessInstanceGUID">流程实例ID</param>
        /// <returns>任务列表数据</returns>
        internal IEnumerable<TaskViewEntity> GetProcessTasks(int appInstanceID,
            Guid processInstanceGUID)
        {
            string sql = @"SELECT * FROM vwWfActivityInstanceTasks 
                            WHERE ApplicationInstaceID=@appInstanceID 
                                AND ProcessInstanceGUID=@processInstanceGUID";
            var list = TaskRepository.Query<TaskViewEntity>(sql, 
                new { appInstanceID = appInstanceID, 
                    processInstanceGUID = processInstanceGUID 
                });
            return list;

        }

        internal IEnumerable<TaskViewEntity> GetProcessTasksWithState(int appInstanceID,
            Guid processInstanceGUID,
            NodeStateEnum state)
        {
            string sql = @"SELECT * FROM vwWfActivityInstanceTasks 
                            WHERE ApplicationInstaceID=@appInstanceID 
                                AND ProcessInstanceGUID=@processInstanceGUID and State=@state";
            var list = TaskRepository.Query<TaskViewEntity>(sql,
                new
                {
                    appInstanceID = appInstanceID,
                    processInstanceGUID = processInstanceGUID,
                    state = state
                });
            return list;
        }

       
        #endregion

        #region TaskManager 任务数据基本操作
        /// <summary>
        /// 插入任务数据
        /// </summary>
        /// <param name="entity">任务实体</param>
        /// <param name="wfLinqDataContext">linq上下文</param>
        internal void Insert(TaskEntity entity, 
            ISession session)
        {
            int result = TaskRepository.Insert(entity, session.Connection, session.Transaction);
            Debug.WriteLine(string.Format("task instance inserted, time:{0}", System.DateTime.Now.ToString()));
        }

        /// <summary>
        /// 插入任务数据
        /// </summary>
        /// <param name="activityInstance"></param>
        /// <param name="performers"></param>
        /// <param name="wfLinqDataContext"></param>
        internal void Insert(ActivityInstanceEntity activityInstance,
            PerformerList performers, 
            WfLogonUser logonUser,
            ISession session)
        {
            foreach (Performer performer in performers)
            {
                TaskEntity entity = new TaskEntity();
                entity.AppName = activityInstance.AppName;
                entity.AppInstanceID = activityInstance.AppInstanceID;
                entity.ActivityInstanceGUID = activityInstance.ActivityInstanceGUID;
                entity.ProcessInstanceGUID = activityInstance.ProcessInstanceGUID;
                entity.ActivityGUID = activityInstance.ActivityGUID;
                entity.ActivityName = activityInstance.ActivityName;
                entity.ProcessGUID = activityInstance.ProcessGUID;
                entity.TaskType = (short)TaskTypeEnum.Manual;
                entity.AssignedToUserID = performer.UserID;
                entity.AssignedToUserName = performer.UserName;
                entity.TaskState = 1; //1-待办状态
                entity.CreatedByUserID = logonUser.UserID;
                entity.CreatedByUserName = logonUser.UserName;
                entity.CreatedDateTime = System.DateTime.Now;
                entity.RecordStatusInvalid = 0;
                Insert(entity, session);
            }
        }

        /// <summary>
        /// 更新任务数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="wfLinqDataContext"></param>
        internal void Update(TaskEntity entity, ISession session)
        {
            TaskRepository.Update(entity, session.Connection, session.Transaction);
        }

        /// <summary>
        /// 读取任务，设置任务为已读状态
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        internal void Read(long taskID,
            int userID,
            string userName)
        {
            ISession session = SessionFactory.CreateSession();
            try
            {
                //置任务为处理状态
                var task = GetTask(taskID);
                var logonUser = new WfLogonUser(userID, userName);
                SetTaskState(task, logonUser, TaskStateEnum.Handling, session);

                //置活动为运行状态
                (new ActivityInstanceManager()).Read(task.ActivityInstanceGUID, logonUser, session);

                session.Commit();
            }
            catch (System.Exception e)
            {
                session.Rollback();
                throw new WorkflowException(string.Format("阅读待办任务时出错！，详细错误：{0}", e.Message), e);
            }
            finally
            {
                session.Dispose();
            }
        }

        /// <summary>
        /// 设置任务状态
        /// </summary>
        /// <param name="task"></param>
        /// <param name="logonUser"></param>
        /// <param name="taskState"></param>
        /// <param name="session"></param>
        private void SetTaskState(TaskEntity task,
            WfLogonUser logonUser,
            TaskStateEnum taskState,
            ISession session)
        {
            task.TaskState = (short)taskState;
            task.LastUpdatedByUserID = logonUser.UserID;
            task.LastUpdatedByUserName = logonUser.UserName;
            task.LastUpdatedDateTime = System.DateTime.Now;
            Update(task, session);
        }



        /// <summary>
        /// 设置任务完成
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="currentLogonUser"></param>
        /// <param name="wfLinqDataContext"></param>
        internal void Complete(long taskID,
            WfLogonUser currentLogonUser,
            ISession session)
        {
            TaskEntity task = TaskRepository.GetById<TaskEntity>(taskID);
            task.TaskState = (byte)TaskStateEnum.Completed;
            task.EndedDateTime = DateTime.Now;
            task.EndedByUserID = currentLogonUser.UserID;
            task.EndedByUserName = currentLogonUser.UserName;

            Update(task, session);
        }

        /// <summary>
        /// 任务删除
        /// </summary>
        /// <param name="taskID">任务ID</param>
        internal void Delete(long taskID)
        {
            ISession session = SessionFactory.CreateSession();
            try
            {
                TaskRepository.Delete<TaskEntity>(taskID, session.Connection, session.Transaction);
                session.Commit();
            }
            catch (System.Exception)
            {
                session.Rollback();
                throw;
            }
            finally
            {
                session.Dispose();
            }
        }
        #endregion

        #region TaskManager 获取任务下一步列表方法，供应用界面显示
        /// <summary>
        /// 获取当前办理任务的下一步
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        internal IList<NodeView> GetNextActivityTree(WfAppRunner runner, IDictionary<string, string> condition)
        {
            ISession session = SessionFactory.CreateSession();
            return GetNextActivityTree(runner, condition, session);
        }

        /// <summary>
        /// 获取当前办理任务的下一步
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="condition"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        internal IList<NodeView> GetNextActivityTree(WfAppRunner runner,
            IDictionary<string, string> condition,
            ISession session)
        {
            var processModel = new ProcessModel(runner.ProcessGUID);
            var taskList = GetTaskOfMine(runner, session).ToList();
            if (taskList == null || taskList.Count == 0)
            {
                throw new WorkflowException("没有当前你正在办理的任务，流程无法读取下一步节点！");
            }
            else if (taskList.Count > 1)
            {
                throw new WorkflowException(string.Format("当前应用ID的办理任务数目要唯一，不是正确的任务数目！错误数目：{0}", taskList.Count));
            }

            var task = taskList[0];
            var nextSteps = processModel.GetNextActivityTree(task.ActivityGUID, condition);
            return nextSteps;
        }
        #endregion
    }
}
