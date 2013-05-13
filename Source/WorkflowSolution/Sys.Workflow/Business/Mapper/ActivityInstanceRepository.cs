using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.DataModel;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 活动实例数据转换类
    /// </summary>
    internal class ActivityInstanceRepository : RepositoryBase<ActivityInstanceEntity, SSIP_WfActivityInstance>
    {
        protected override void SetLinqDataContext()
        {
            base.LinqDataContext = DataContextFactory.CreateWfLinqDataContext();
        }

        #region ActivityInstance 实体转换
        /// <summary>
        /// 实体转换
        /// </summary>
        /// <param name="dEntity">数据实体</param>
        /// <returns></returns>
        protected override ActivityInstanceEntity ConvertToBusinessEntity(ActivityInstanceEntity bEntity, SSIP_WfActivityInstance dEntity)
        {
            bEntity.ProcessGUID = dEntity.ProcessGUID.ToString();
            bEntity.ProcessInstanceGUID = dEntity.ProcessInstanceGUID.ToString();
            bEntity.ActivityInstanceGUID = dEntity.ActivityInstanceGUID.ToString();
            bEntity.ActivityGUID = dEntity.ActivityGUID.ToString();
            bEntity.ActivityName = dEntity.ActivityName;
            bEntity.ActivityTypeID = dEntity.ActivityTypeID;
            bEntity.GatewayDirectionTypeID = dEntity.GatewayDirectionTypeID;
            bEntity.TokensRequired = dEntity.TokensRequired;
            bEntity.TokensHad = dEntity.TokensHad;
            bEntity.CanRenewInstance = dEntity.CanRenewInstance;
            bEntity.State = dEntity.State;
            bEntity.IsActivityCompleted = dEntity.IsActivityCompleted;
            bEntity.ApplicationInstanceID = dEntity.ApplicationInstanceID;
            bEntity.CreatedDateTime = dEntity.CreatedDateTime;
            bEntity.LastUpdatedDateTime = dEntity.LastUpdatedDateTime;
            bEntity.EndedDateTime = dEntity.EndedDateTime;
            bEntity.EndedByUserID = dEntity.EndedByUserID.Value;
            bEntity.EndedByUserName = dEntity.EndedByUserName;
            bEntity.RecordStatusInvalid = dEntity.RecordStatusInvalid;
            return bEntity;
        }

        /// <summary>
        /// 实体转换
        /// </summary>
        /// <param name="dEntity">数据实体</param>
        /// <returns></returns>
        protected override SSIP_WfActivityInstance ConvertToDataEntity(ActivityInstanceEntity bEntity, SSIP_WfActivityInstance dEntity)
        {
            dEntity.ActivityInstanceGUID = new Guid(bEntity.ActivityInstanceGUID);
            dEntity.ProcessInstanceGUID = new Guid(bEntity.ProcessInstanceGUID);
            dEntity.ProcessGUID = new Guid(bEntity.ProcessGUID);
            dEntity.ActivityGUID = new Guid(bEntity.ActivityGUID);
            dEntity.ActivityName = bEntity.ActivityName;
            dEntity.ActivityTypeID = bEntity.ActivityTypeID;
            dEntity.GatewayDirectionTypeID = bEntity.GatewayDirectionTypeID;
            dEntity.TokensRequired = bEntity.TokensRequired;
            dEntity.TokensHad = bEntity.TokensHad;
            dEntity.CanRenewInstance = bEntity.CanRenewInstance;
            dEntity.State = bEntity.State;
            dEntity.IsActivityCompleted = bEntity.IsActivityCompleted;
            dEntity.ApplicationInstanceID = bEntity.ApplicationInstanceID;
            dEntity.CreatedDateTime = bEntity.CreatedDateTime;
            dEntity.LastUpdatedDateTime = bEntity.LastUpdatedDateTime;
            dEntity.EndedDateTime = bEntity.EndedDateTime;
            dEntity.EndedByUserID = bEntity.EndedByUserID;
            dEntity.EndedByUserName = bEntity.EndedByUserName;
            dEntity.RecordStatusInvalid = bEntity.RecordStatusInvalid;
            return dEntity;
        }
        #endregion
    }
}
