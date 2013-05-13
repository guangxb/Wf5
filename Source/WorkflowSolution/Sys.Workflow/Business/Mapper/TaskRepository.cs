using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using System.Text;
using Sys.Workflow.DataModel;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 任务实体转换类
    /// </summary>
    internal class TaskRepository : RepositoryBase<TaskEntity, SSIP_WfTasks>
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
        protected override TaskEntity ConvertToBusinessEntity(TaskEntity bEntity, SSIP_WfTasks dEntity)
        {
            bEntity.TaskID = dEntity.TaskID;
            bEntity.TaskTypeID = dEntity.TaskTypeID;
            bEntity.ActivityInstanceGUID = dEntity.ActivityInstanceGUID.ToString();
            bEntity.ProcessInstanceGUID = dEntity.ProcessInstanceGUID.ToString();
            bEntity.AssignedToUserID = dEntity.AssignedToUserID;
            bEntity.AssignedToUserName = dEntity.AssignedToUserName;
            bEntity.CreatedDateTime = dEntity.CreatedDateTime;
            bEntity.IsTaskCompleted = dEntity.IsTaskCompleted;
            bEntity.EndedDateTime = dEntity.EndedDateTime;
            bEntity.EndedByUserID = dEntity.EndedByUserID.Value;
            bEntity.EndedByUserName = dEntity.EndedByUserName;
            bEntity.LastUpdatedDateTime = dEntity.LastUpdatedDateTime;
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
        protected override SSIP_WfTasks ConvertToDataEntity(TaskEntity bEntity, SSIP_WfTasks dEntity)
        {
            dEntity.TaskID = bEntity.TaskID;
            dEntity.TaskTypeID = bEntity.TaskTypeID;
            dEntity.ActivityInstanceGUID = new Guid(bEntity.ActivityInstanceGUID);
            dEntity.ProcessInstanceGUID = new Guid(bEntity.ProcessInstanceGUID);
            dEntity.AssignedToUserID = bEntity.AssignedToUserID;
            dEntity.AssignedToUserName = bEntity.AssignedToUserName;
            dEntity.CreatedDateTime = bEntity.CreatedDateTime;
            dEntity.IsTaskCompleted = bEntity.IsTaskCompleted;
            dEntity.EndedDateTime = bEntity.EndedDateTime;
            dEntity.EndedByUserID = bEntity.EndedByUserID;
            dEntity.EndedByUserName = bEntity.EndedByUserName;
            dEntity.LastUpdatedDateTime = bEntity.LastUpdatedDateTime;
            dEntity.RecordStatusInvalid = bEntity.RecordStatusInvalid;

            return dEntity;
        }

        
        #endregion
    }
}
