using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Sys.Workflow.Common;
using Sys.Workflow.Utility;
using Sys.Workflow.DataModel;

namespace Sys.Workflow.Business
{
    internal class ProcessInstanceManager
    {
        private DataAccessManager _processInstanceRepository;
        private DataAccessManager ProcessInstanceRepository
        {
            get
            {
                if (_processInstanceRepository == null)
                    _processInstanceRepository = DataAccessFactory.Instance();
                return _processInstanceRepository;
            }
        }


        #region ProcessInstanceManager 基本数据操作
        /// <summary>
        /// 根据GUID获取流程实例数据
        /// </summary>
        /// <param name="processInstanceGUID"></param>
        /// <returns></returns>
        internal ProcessInstanceEntity GetById(Guid processInstanceGUID)
        {
            return ProcessInstanceRepository.GetById<ProcessInstanceEntity>(processInstanceGUID);
        }

        /// <summary>
        /// 获取运行中的流程实例
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="appInstanceID"></param>
        /// <param name="processGUID"></param>
        /// <returns></returns>
        internal ProcessInstanceEntity GetRunningProcess(string appName,
            int appInstanceID,
            Guid processGUID)
        {
            ProcessInstanceEntity processInstanceEntity = null;
            var processInstanceList = GetProcessInstance(appName, appInstanceID, processGUID, ProcessStateEnum.Running).ToList();
            if (processInstanceList.Count == 1)
                processInstanceEntity = processInstanceList[0];

            return processInstanceEntity;
        }

        /// <summary>
        /// 获取已经完成的流程实例
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="appInstanceID"></param>
        /// <param name="processGUID"></param>
        /// <returns></returns>
        internal ProcessInstanceEntity GetCompletedProcess(string appName,
            int appInstanceID,
            Guid processGUID)
        {
            var processInstanceList = GetProcessInstance(appName, appInstanceID, processGUID, ProcessStateEnum.Completed).ToList();
            return processInstanceList[0];
        }

        /// <summary>
        /// 根据流程完成状态获取流程实例数据
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="appInstanceID"></param>
        /// <param name="processGUID"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private IEnumerable<ProcessInstanceEntity> GetProcessInstance(string appName,
            int appInstanceID,
            Guid processGUID,
            ProcessStateEnum state)
        {
            var sql = "SELECT * FROM WfProcessInstance WHERE AppName=@appName AND AppInstanceID=@appInstanceID AND ProcessGUID=@processGUID AND ProcessState=@state ORDER BY CreatedDateTime DESC";
            return ProcessInstanceRepository.Query<ProcessInstanceEntity>(sql, 
                new { appName = appName, 
                    appInstanceID = appInstanceID, 
                    processGUID = processGUID,
                    state = (short)state
                });
        }


        /// <summary>
        /// 流程数据插入
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="wfLinqDataContext"></param>
        /// <returns></returns>
        internal void Insert(ProcessInstanceEntity entity, ISession session = null)
        {
            int result = ProcessInstanceRepository.Insert(entity, session.Connection, session.Transaction);
            Debug.WriteLine(string.Format("process instance inserted, Guid:{0}, time:{1}", entity.ProcessInstanceGUID.ToString(), System.DateTime.Now.ToString()));
        }

        /// <summary>
        /// 流程实例更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="wfLinqDataContext"></param>
        internal void Update(ProcessInstanceEntity entity, 
            ISession session)
        {
            ProcessInstanceRepository.Update(entity, session.Connection, session.Transaction);
        }

        /// <summary>
        /// 根据流程定义，创建新的流程实例
        /// </summary>
        /// <param name="processID">流程定义ID</param>
        /// <returns>流程实例的ID</returns>
        internal ProcessInstanceEntity CreateNewProcessInstanceObject(string appName, 
            int appInstanceID,
            ProcessEntity processEntity, 
            int userID, 
            string userName)
        {
            ProcessInstanceEntity entity = new ProcessInstanceEntity();
            entity.ProcessGUID = processEntity.ProcessGUID;
            entity.ProcessInstanceGUID = Guid.NewGuid();
            entity.ProcessName = processEntity.ProcessName;
            entity.AppName = appName;
            entity.AppInstanceID = appInstanceID;
            entity.ProcessState = (int)ProcessStateEnum.Running;
            entity.CreatedByUserID = userID;
            entity.CreatedByUserName = userName;
            entity.CreatedDateTime = System.DateTime.Now;
            entity.LastUpdatedByUserID = userID;
            entity.LastUpdatedByUserName = userName;
            entity.LastUpdatedDateTime = System.DateTime.Now;
           
            return entity;
        }
        #endregion

        #region 流程业务规则处理
        /// <summary>
        /// 流程完成，设置流程的状态为完成
        /// </summary>
        /// <returns>是否成功</returns>
        internal void Complete(Guid processInstanceGUID, WfLogonUser currentUser, ISession session)
        {
            var bEntity = GetById(processInstanceGUID);
            var processState = (ProcessStateEnum)Enum.Parse(typeof(ProcessStateEnum), bEntity.ProcessState.ToString());
            if ((processState | ProcessStateEnum.Running) == ProcessStateEnum.Running)
            {
                bEntity.ProcessState = (short)ProcessStateEnum.Completed;
                bEntity.IsProcessCompleted = 1;
                bEntity.EndedDateTime = System.DateTime.Now;
                bEntity.EndedByUserID = currentUser.UserID;
                bEntity.EndedByUserName = currentUser.UserName;

                Update(bEntity, session);
            }
            else
            {
                throw new ProcessInstanceException("流程不在运行状态，不能被完成！");
            }
        }

        /// <summary>
        /// 返签流程，将流程状态置为返签，并修改流程未完成标志
        /// </summary>
        /// <param name="processInstanceGUID"></param>
        /// <param name="currentUser"></param>
        /// <param name="session"></param>
        internal void Reverse(Guid processInstanceGUID, 
            WfLogonUser currentUser, 
            ISession session)
        {
            var bEntity = GetById(processInstanceGUID);
            if (bEntity.ProcessState == (short)ProcessStateEnum.Completed)
            {
                bEntity.ProcessState = (short)ProcessStateEnum.Running;
                bEntity.IsProcessCompleted = 0;
                bEntity.LastUpdatedByUserID = currentUser.UserID;
                bEntity.LastUpdatedByUserName = currentUser.UserName;
                bEntity.LastUpdatedDateTime = System.DateTime.Now;
                
                Update(bEntity, session);
            }
            else
            {
                throw new ProcessInstanceException("流程不在运行状态，不能被完成！");
            }
        }

        /// <summary>
        /// 流程的取消操作
        /// </summary>
        /// <returns>是否成功</returns>
        internal bool Cancel(string appName, int appInstanceID, Guid processGUID, int userID, string userName)
        {
            var isCanceled = false;
            var entities = GetProcessInstance(appName, appInstanceID, processGUID, ProcessStateEnum.Running).ToList();

            if (entities == null || entities.Count == 0 || entities.Count > 1)
            {
                throw new WorkflowException("无法取消流程，错误原因：当前没有运行中的流程实例，或者同时有多个运行中的流程实例（不合法数据）!");
            }

            try
            {
                ISession session = SessionFactory.CreateSession();
                var entity = entities[0];
                entity.ProcessState = (short)ProcessStateEnum.Canceled;
                entity.RecordStatusInvalid = 1;
                entity.LastUpdatedByUserID = userID;
                entity.LastUpdatedByUserName = userName;
                entity.LastUpdatedDateTime = System.DateTime.Now;

                Update(entity, session);

                isCanceled = true;
            }
            catch(System.Exception e)
            {
                throw new WorkflowException(string.Format("取消流程实例失败，错误原因：{0}", e.Message));
            }

            return isCanceled;
        }

        /// <summary>
        /// 废弃单据下所有流程的信息
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="appInstanceID"></param>
        /// <param name="processGUID"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal bool Discard(string appName, int appInstanceID, Guid processGUID, int userID, string userName)
        {
            var isDiscarded = false;
            ISession session = SessionFactory.CreateSession();

            try
            {
                string updSql = @"UPDATE WfProcessInstance
		                         SET [ProcessState] = 32, --废弃状态
			                         [RecordStatusInvalid] = 1, --设置记录为无效状态
			                         [LastUpdatedDateTime] = GETDATE(),
			                         [LastUpdatedByUserID] = @userID,
			                         [LastUpdatedByUserName] = @userName
		                        WHERE AppInstanceID = @appInstanceID
			                        AND ProcessGUID = @processGUID
			                        AND ProcessState <> 32";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = updSql;
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = (SqlTransaction)session.Transaction;

                cmd.Parameters.Add(new SqlParameter("@userID", userID));
                cmd.Parameters.Add(new SqlParameter("@userName", userName));
                cmd.Parameters.Add(new SqlParameter("@appInstanceID", appInstanceID));
                cmd.Parameters.Add(new SqlParameter("@processGUID", processGUID));

                int result = ProcessInstanceRepository.ExecuteCommand(cmd, session);
                session.Commit();

                //返回结果大于0，表示有记录更新
                if (result > 0)
                {
                    isDiscarded = true;
                }
            }
            catch (System.Exception e)
            {
                session.Rollback();
                throw new WorkflowException(string.Format("执行废弃流程的操作失败，错误原因：{0}", e.Message));
            }
            finally
            {
                session.Dispose();
            }
            return isDiscarded;
        }

        /// <summary>
        /// 流程终止操作
        /// </summary>
        /// <returns></returns>
        internal bool Terminate(Guid processInstanceGUID)
        {
            ProcessInstanceEntity entity = ProcessInstanceRepository.GetById<ProcessInstanceEntity>(processInstanceGUID);

            if (entity.ProcessState == (int)ProcessStateEnum.Running
                || entity.ProcessState == (int)ProcessStateEnum.Ready
                || entity.ProcessState == (int)ProcessStateEnum.Suspended)
            {
                //ProcessInstanceRepository.Terminate(ProcessInstanceGUID);

                return true;
            }
            else
            {
                throw new ProcessInstanceException("流程已经结束，或者已经被取消！");
            }
        }

        /// <summary>
        /// 删除不正常的流程实例（流程在取消状态，才可以进行删除！）
        /// </summary>
        /// <param name="processInstanceGUID"></param>
        /// <returns></returns>
        internal bool Delete(Guid processInstanceGUID)
        {
            bool isDeleted = false;
            ISession session = SessionFactory.CreateSession();

            try
            {
                ProcessInstanceEntity entity = ProcessInstanceRepository.GetById<ProcessInstanceEntity>(processInstanceGUID);

                if (entity.ProcessState == (int)ProcessStateEnum.Canceled)
                {
                    ProcessInstanceRepository.Delete<ProcessInstanceEntity>(processInstanceGUID, session.Connection, session.Transaction);
                    session.Commit();
                    isDeleted = true;
                }
                else
                {
                    throw new ProcessInstanceException("流程只有先取消，才可以删除！");
                }
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

            return isDeleted;

        }
        #endregion

    }
}
