using System;
using System.Data;
using System.Data.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Common;
using Sys.Workflow.DataModel;

namespace Sys.Workflow.Business
{
    internal class TaskViewRepository : RepositoryBase<TaskViewEntity, vw_SSIP_WfActivityInstanceTasks>
    {
        protected override void SetLinqDataContext()
        {
            base.LinqDataContext = DataContextFactory.CreateWfLinqDataContext();
        }

        #region 业务实体转换
        /// <summary>
        /// 转换为业务实体
        /// </summary>
        /// <param name="bEntity"></param>
        /// <param name="dEntity"></param>
        /// <returns></returns>
        protected override TaskViewEntity ConvertToBusinessEntity(TaskViewEntity bEntity, vw_SSIP_WfActivityInstanceTasks dEntity)
        {
            bEntity.TaskID = dEntity.TaskID;
            bEntity.TaskTypeID = dEntity.TaskTypeID;
            bEntity.ActivityInstanceGUID = dEntity.ActivityInstanceGUID.ToString();
            bEntity.ApplicationInstanceID = dEntity.ApplicationInstanceID;
            bEntity.ProcessInstanceGUID = dEntity.ProcessInstanceGUID.ToString();
            bEntity.ProcessGUID = dEntity.ProcessGUID.ToString();
            bEntity.ProcessName = dEntity.ProcessName;
            bEntity.ActivityGUID = dEntity.ActivityGUID.ToString();
            bEntity.ActivityName = dEntity.ActivityName;
            bEntity.ActivityTypeID = dEntity.ActivityTypeID;
            bEntity.IsActivityCompleted = dEntity.IsActivityCompleted;
            bEntity.AssignedToUserID = dEntity.AssignedToUserID;
            bEntity.AssignedToUserName = dEntity.AssignedToUserName;
            bEntity.CreatedDateTime = dEntity.CreatedDateTime;
            bEntity.IsTaskCompleted = dEntity.IsTaskCompleted;
            bEntity.EndedDateTime = dEntity.EndedDateTime;
            bEntity.EndedByUserID = dEntity.EndedByUserID.Value;
            bEntity.EndedByUserName = dEntity.EndedByUserName;
            bEntity.ActivityState = dEntity.ActivityState;
            bEntity.RecordStatusInvalid = dEntity.RecordStatusInvalid;

            return bEntity;
        }

        /// <summary>
        /// 转换为数据实体
        /// </summary>
        /// <param name="bEntity"></param>
        /// <param name="dEntity"></param>
        /// <param name="isToInsert"></param>
        /// <returns></returns>
        protected override vw_SSIP_WfActivityInstanceTasks ConvertToDataEntity(TaskViewEntity bEntity, vw_SSIP_WfActivityInstanceTasks dEntity)
        {
            dEntity.TaskID = bEntity.TaskID;
            dEntity.TaskTypeID = bEntity.TaskTypeID;
            dEntity.ActivityInstanceGUID = new Guid(bEntity.ActivityInstanceGUID);
            dEntity.ApplicationInstanceID = bEntity.ApplicationInstanceID;
            dEntity.ProcessInstanceGUID = new Guid(bEntity.ProcessInstanceGUID);
            dEntity.ProcessGUID = new Guid(bEntity.ProcessGUID);
            dEntity.ProcessName = bEntity.ProcessName;
            dEntity.ActivityGUID = new Guid(bEntity.ActivityGUID);
            dEntity.ActivityName = bEntity.ActivityName;
            dEntity.ActivityTypeID = bEntity.ActivityTypeID;
            dEntity.IsActivityCompleted = bEntity.IsActivityCompleted;
            dEntity.AssignedToUserID = bEntity.AssignedToUserID;
            dEntity.AssignedToUserName = bEntity.AssignedToUserName;
            dEntity.CreatedDateTime = bEntity.CreatedDateTime;
            dEntity.IsTaskCompleted = bEntity.IsTaskCompleted;
            dEntity.EndedDateTime = bEntity.EndedDateTime;                
            dEntity.EndedByUserID = bEntity.EndedByUserID;
            dEntity.EndedByUserName = bEntity.EndedByUserName;
            dEntity.ActivityState = bEntity.ActivityState;
            dEntity.RecordStatusInvalid = bEntity.RecordStatusInvalid;

            return dEntity;
        }
        #endregion
    }
}
