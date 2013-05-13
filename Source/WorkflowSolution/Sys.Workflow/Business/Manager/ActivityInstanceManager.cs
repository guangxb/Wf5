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
    /// 活动实例管理类
    /// </summary>
    internal class ActivityInstanceManager
    {
        #region ActivityInstanceManager 属性

        private DataAccessManager _activityInstanceRepository;
        private DataAccessManager ActivityInstanceRepository
        {
            get
            {
                if (_activityInstanceRepository == null)
                {
                    _activityInstanceRepository = DataAccessFactory.Instance();
                }
                return _activityInstanceRepository;
            }
        }
        #endregion
       
        #region ActivityInstanceManager 构造函数
        internal ActivityInstanceManager()
        {
        }
        #endregion

        #region ActivityInstanceManager 活动实例数据获取
        internal ActivityInstanceEntity GetById(Guid activityInstanceGUID)
        {
            return ActivityInstanceRepository.GetById<ActivityInstanceEntity>(activityInstanceGUID);
        }

        internal ActivityInstanceEntity GetById(Guid activityInstanceGUID, ISession session)
        {
            return ActivityInstanceRepository.GetById<ActivityInstanceEntity>(activityInstanceGUID, session.Connection, session.Transaction);
        }

        /// <summary>
        /// 判断有活动实例是否在运行状态
        /// </summary>
        /// <param name="activityGUID"></param>
        /// <returns></returns>
        internal ActivityInstanceEntity GetActivityWithRunningState(Guid processInstanceGUID,
            Guid activityGUID,
            ISession session)
        {
            var sql = @"SELECT * FROM WfActivityInstance 
                        WHERE ProcessInstanceGUID = @processInstanceGUID 
                            AND ActivityGUID = @activityGUID 
                            AND State = @state";

            var instanceList = ActivityInstanceRepository.Query<ActivityInstanceEntity>(
                sql, new
                {
                    processInstanceGUID = processInstanceGUID.ToString(),
                    activityGUID = activityGUID.ToString(),
                    state = (short)NodeStateEnum.Running
                }).ToList();

            if (instanceList.Count == 1)
            {
                return instanceList[0];
            }
            else
            {
                return null;
            }
        }

        internal IEnumerable<ActivityInstanceEntity> GetAcitivityInstancePaged(TaskQueryEntity query)
        {
            ISession session = SessionFactory.CreateSession();
            var whereSql = @" WHERE AppInstanceID = @appInstanceID
                            AND ProcessGUID = @processGUID";
            var orderBySql = "ORDER BY CreatedDateTime DESC";

            var entityList = ActivityInstanceRepository.GetPage<ActivityInstanceEntity>(query.PageIndex, query.PageSize, 
                whereSql, new {
                    appInstanceID = query.AppInstanceID,
                    processGUID = query.ProcessGUID
                }, 
                orderBySql,
                session.Connection, session.Transaction);

            return entityList;
        }
        #endregion

        /// <summary>
        /// 创建活动实例的对象
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="processInstance"></param>
        /// <returns></returns>
        internal ActivityInstanceEntity CreateActivityInstanceObject(string appName,
            int appInstanceID,
            Guid processInstanceGUID,
            ActivityEntity activity,
            WfLogonUser logonUser)
        {
            ActivityInstanceEntity instance = new ActivityInstanceEntity();
            instance.ActivityGUID = activity.ActivityGUID;
            instance.ActivityName = activity.ActivityName;
            instance.ActivityType = (short)activity.NodeType;
            instance.GatewayDirectionTypeID = (short)activity.GatewayDirectionType;
            instance.ProcessGUID = activity.ProcessGUID;
            instance.AppName = appName;
            instance.AppInstanceID = appInstanceID;
            instance.ProcessInstanceGUID = processInstanceGUID;
            instance.ActivityInstanceGUID = Guid.NewGuid();
            instance.TokensRequired = 1;
            instance.TokensHad = 1;
            instance.CreatedByUserID = logonUser.UserID;
            instance.CreatedByUserName = logonUser.UserName;
            instance.CreatedDateTime = System.DateTime.Now;
            instance.State = (short)NodeStateEnum.Ready;
            instance.CanRenewInstance = 0;

            return instance;
        }

        /// <summary>
        /// 更新活动节点的Token数目
        /// </summary>
        /// <param name="activityInstanceGUID"></param>
        /// <param name="logonUser"></param>
        /// <param name="wfLinqDataContext"></param>
        internal void IncreaseTokensHad(Guid activityInstanceGUID,
            WfLogonUser logonUser,
            ISession session)
        {
            ActivityInstanceEntity activityInstance = GetById(activityInstanceGUID);
            activityInstance.TokensHad += 1;
            Update(activityInstance, session);
        }

        #region 活动实例中间状态设置
        /// <summary>
        /// 活动实例被读取
        /// </summary>
        /// <param name="activityInstanceGUID"></param>
        /// <param name="logonUser"></param>
        /// <param name="session"></param>
        internal void Read(Guid activityInstanceGUID,
            WfLogonUser logonUser,
            ISession session)
        {
            SetActivityState(activityInstanceGUID, NodeStateEnum.Running, logonUser, session);
        }

        /// <summary>
        /// 撤销活动实例
        /// </summary>
        /// <param name="activityInstanceGUID"></param>
        /// <param name="logonUser"></param>
        /// <param name="session"></param>
        internal void Withdraw(Guid activityInstanceGUID,
            WfLogonUser logonUser,
            ISession session)
        {
            SetActivityState(activityInstanceGUID, NodeStateEnum.Withdrawed, logonUser, session);
        }

        /// <summary>
        /// 退回活动实例
        /// </summary>
        /// <param name="activityInstanceGUID"></param>
        /// <param name="logonUser"></param>
        /// <param name="session"></param>
        internal void Reject(Guid activityInstanceGUID,
            WfLogonUser logonUser,
            ISession session)
        {
            SetActivityState(activityInstanceGUID, NodeStateEnum.Rejected, logonUser, session);
        }

        /// <summary>
        /// 设置活动实例状态
        /// </summary>
        /// <param name="activityInstanceGUID"></param>
        /// <param name="nodeState"></param>
        /// <param name="logonUser"></param>
        /// <param name="session"></param>
        private void SetActivityState(Guid activityInstanceGUID,
            NodeStateEnum nodeState,
            WfLogonUser logonUser,
            ISession session)
        {
            var activityInstance = GetById(activityInstanceGUID);
            activityInstance.State = (short)nodeState;
            activityInstance.LastUpdatedByUserID = logonUser.UserID;
            activityInstance.LastUpdatedByUserName = logonUser.UserName;
            activityInstance.LastUpdatedDateTime = System.DateTime.Now;
            Update(activityInstance, session);
        }

        /// <summary>
        /// 活动实例完成
        /// </summary>
        /// <param name="activityInstanceGUID"></param>
        /// <param name="logonUser"></param>
        /// <param name="session"></param>
        internal void Complete(Guid activityInstanceGUID, 
            WfLogonUser logonUser,
            ISession session)
        {
            var activityInstance = GetById(activityInstanceGUID, session);
            activityInstance.State = (short)NodeStateEnum.Completed;
            activityInstance.IsActivityCompleted = 1;
            activityInstance.LastUpdatedByUserID = logonUser.UserID;
            activityInstance.LastUpdatedByUserName = logonUser.UserName;
            activityInstance.LastUpdatedDateTime = System.DateTime.Now;
            activityInstance.EndedByUserID = logonUser.UserID;
            activityInstance.EndedByUserName = logonUser.UserName;
            activityInstance.EndedDateTime = System.DateTime.Now;

            Update(activityInstance, session);
        }
        #endregion

        internal void Insert(ActivityInstanceEntity entity,
            ISession session)
        {
            int result = ActivityInstanceRepository.Insert(entity, session.Connection, session.Transaction);
            Debug.WriteLine(string.Format("activity instance inserted, ProcessInstanceGUID:{0}, time:{1}", entity.ProcessInstanceGUID,
                System.DateTime.Now.ToString()));
        }

        internal void Update(ActivityInstanceEntity entity,
            ISession session)
        {
            ActivityInstanceRepository.Update(entity, session.Connection, session.Transaction);
        }

        /// <summary>
        /// 删除活动实例
        /// </summary>
        /// <param name="activityInstanceGUID"></param>
        /// <param name="wfLinqDataContext"></param>
        internal void Delete(Guid activityInstanceGUID,
            ISession session = null)
        {
            ActivityInstanceRepository.Delete<ActivityInstanceEntity>(activityInstanceGUID, session.Connection, session.Transaction);
        }
    }
}
