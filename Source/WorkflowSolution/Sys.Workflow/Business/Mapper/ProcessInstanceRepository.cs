using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.DataModel;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 流程实例数据转换封装类
    /// </summary>
    internal class ProcessInstanceRepository : RepositoryBase<ProcessInstanceEntity, SSIP_WfProcessInstance>
    {
        protected override void SetLinqDataContext()
        {
            base.LinqDataContext = DataContextFactory.CreateWfLinqDataContext();
        }

        #region ProcessInstance 实体转换
        /// <summary>
        /// 转换成业务实体
        /// </summary>
        /// <param name="bizEntity"></param>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        protected override ProcessInstanceEntity ConvertToBusinessEntity(ProcessInstanceEntity bizEntity, 
            SSIP_WfProcessInstance dataEntity)
        {
            bizEntity.ApplicationInstanceID = dataEntity.ApplicationInstanceID;
            bizEntity.ProcessGUID = dataEntity.ProcessGUID.ToString();
            bizEntity.ProcessName = dataEntity.ProcessName;
            bizEntity.ProcessInstanceGUID = dataEntity.ProcessInstanceGUID.ToString();
            bizEntity.State = dataEntity.State;
            bizEntity.CreatedDateTime = dataEntity.CreatedDateTime;
            bizEntity.CreatedByUserID = dataEntity.CreatedByUserID.Value;
            bizEntity.CreatedByUserName = dataEntity.CreatedByUserName;
            bizEntity.LastUpdatedDateTime = dataEntity.LastUpdatedDateTime;
            bizEntity.IsProcessCompleted = dataEntity.IsProcessCompleted;
            bizEntity.EndedDateTime = dataEntity.EndedDateTime;
            bizEntity.EndedByUserID = dataEntity.EndedByUserID.Value;
            bizEntity.EndedByUserName = dataEntity.EndedByUserName;
            bizEntity.RecordStatusInvalid = dataEntity.RecordStatusInvalid;
            return bizEntity;
        }
        
        /// <summary>
        /// 转换成数据实体
        /// </summary>
        /// <param name="bizEntity"></param>
        /// <param name="dataEntity"></param>
        /// <param name="isToInsert"></param>
        /// <returns></returns>
        protected override SSIP_WfProcessInstance ConvertToDataEntity(ProcessInstanceEntity bizEntity, 
            SSIP_WfProcessInstance dataEntity)
        {
            dataEntity.ProcessInstanceGUID = new Guid(bizEntity.ProcessInstanceGUID);
            dataEntity.ApplicationInstanceID = bizEntity.ApplicationInstanceID;
            dataEntity.ProcessGUID = new Guid(bizEntity.ProcessGUID);
            dataEntity.ProcessName = bizEntity.ProcessName;
            dataEntity.State = bizEntity.State;
            dataEntity.CreatedDateTime = System.DateTime.Now;
            dataEntity.CreatedByUserID = bizEntity.CreatedByUserID;
            dataEntity.CreatedByUserName = bizEntity.CreatedByUserName;
            dataEntity.LastUpdatedDateTime = bizEntity.LastUpdatedDateTime;
            dataEntity.IsProcessCompleted = bizEntity.IsProcessCompleted;
            dataEntity.EndedDateTime = bizEntity.EndedDateTime;
            dataEntity.EndedByUserID = bizEntity.EndedByUserID;
            dataEntity.EndedByUserName = bizEntity.EndedByUserName;
            dataEntity.RecordStatusInvalid = bizEntity.RecordStatusInvalid;
            return dataEntity;
        }

        /// <summary>
        /// 取消流程实例（更改流程实例的状态位取消状态）
        /// </summary>
        /// <param name="processInstanceGUID"></param>
        internal void Abort(string processInstanceGUID)
        {
            ProcessModelDataContext dataContext = (ProcessModelDataContext)base.LinqDataContext;
            dataContext.sp_WfProcessInstance_Abort(processInstanceGUID);
        }

        /// <summary>
        /// 终止流程实例（更改流程实例的状态位终止状态）
        /// </summary>
        /// <param name="processInstanceGUID"></param>
        internal void Terminate(string processInstanceGUID)
        {
            ProcessModelDataContext dataContext = (ProcessModelDataContext)base.LinqDataContext;
            dataContext.sp_WfProcessInstance_Terminate(processInstanceGUID);
        }

        /// <summary>
        /// 删除流程实例（删除流程实例相关联的活动、任务、转移等数据）
        /// </summary>
        /// <param name="processInstanceGUID"></param>
        internal void Delete(string processInstanceGUID)
        {
            ProcessModelDataContext dataContext = (ProcessModelDataContext)base.LinqDataContext;
            dataContext.sp_WfProcessInstance_Delete(processInstanceGUID);
        }
        #endregion
    }
}
