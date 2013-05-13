using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.DataModel;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 转移实例数据操作类
    /// </summary>
    internal class TransitionInstanceRepository : RepositoryBase<TransitionInstanceEntity, SSIP_WfTransitionInstance>
    {
        protected override void SetLinqDataContext()
        {
            base.LinqDataContext = DataContextFactory.CreateWfLinqDataContext();
        }

        protected override TransitionInstanceEntity ConvertToBusinessEntity(TransitionInstanceEntity bEntity, SSIP_WfTransitionInstance dEntity)
        {
            bEntity.TransitionInstanceGUID = dEntity.TransitionInstanceGUID.ToString();
            bEntity.TransitionGUID = dEntity.TransitionGUID.ToString();
            bEntity.ProcessGUID = dEntity.ProcessGUID.ToString();
            bEntity.ProcessInstanceGUID = dEntity.ProcessInstanceGUID.ToString();
            bEntity.FromActivityInstanceGUID = dEntity.FromActivityInstanceGUID.ToString();
            bEntity.FromActivityGUID = dEntity.FromActivityGUID.ToString();
            bEntity.ToActivityInstanceGUID = dEntity.ToActivityInstanceGUID.ToString();
            bEntity.ToActivityGUID = dEntity.ToActivityGUID.ToString();
            bEntity.CreatedDateTime = dEntity.CreatedDateTime;
            bEntity.ConditionParseResult = dEntity.ConditionParseResult;
            bEntity.IsTransitionCompleted = dEntity.IsTransitionCompleted;
            bEntity.RecordStatusInvalid = dEntity.RecordStatusInvalid;
            return bEntity;
        }

        protected override SSIP_WfTransitionInstance ConvertToDataEntity(TransitionInstanceEntity bEntity, SSIP_WfTransitionInstance dEntity)
        {
            dEntity.TransitionInstanceGUID = new Guid(bEntity.TransitionInstanceGUID);
            dEntity.TransitionGUID = new Guid(bEntity.TransitionGUID);
            dEntity.ProcessGUID = new Guid(bEntity.ProcessGUID);
            dEntity.ProcessInstanceGUID = new Guid(bEntity.ProcessInstanceGUID);
            dEntity.FromActivityInstanceGUID = new Guid(bEntity.FromActivityInstanceGUID);
            dEntity.FromActivityGUID = new Guid(bEntity.FromActivityGUID);
            dEntity.ToActivityInstanceGUID = new Guid(bEntity.ToActivityInstanceGUID);
            dEntity.ToActivityGUID = new Guid(bEntity.ToActivityGUID);
            dEntity.ConditionParseResult = bEntity.ConditionParseResult;
            dEntity.IsTransitionCompleted = bEntity.IsTransitionCompleted;
            dEntity.RecordStatusInvalid = bEntity.RecordStatusInvalid;
            dEntity.CreatedDateTime = bEntity.CreatedDateTime;

            return dEntity;
        }
    }
}
