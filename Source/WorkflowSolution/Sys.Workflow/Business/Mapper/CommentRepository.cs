using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Common;
using Sys.Workflow.DataModel;

namespace Sys.Workflow.Business
{
    internal class CommentRepository : RepositoryBase<CommentEntity, SSIP_WfComment>
    {
        protected override void SetLinqDataContext()
        {
            base.LinqDataContext = DataContextFactory.CreateWfLinqDataContext();
        }

        protected override CommentEntity ConvertToBusinessEntity(CommentEntity bEntity, SSIP_WfComment dEntity)
        {
            bEntity.CommentID = dEntity.CommentID;
            bEntity.CommentTypeID = dEntity.CommentTypeID;
            bEntity.ApplicationInstanceID = dEntity.ApplicationInstanceID;
            bEntity.ProcessInstanceGUID = dEntity.ProcessInstanceGUID.ToString();
            bEntity.ActivityInstanceGUID = dEntity.ActivityInstanceGUID.ToString();
            bEntity.ActivityGUID = dEntity.ActivityGUID.ToString();
            bEntity.ActivityName = dEntity.ActivityName;
            bEntity.IsPassed = (dEntity.IsPassed != null && dEntity.IsPassed.HasValue) ? dEntity.IsPassed.Value : bEntity.IsPassed;
            bEntity.Comment = dEntity.Comment;
            bEntity.CreatedDateTime = dEntity.CreatedDateTime;
            bEntity.CommentedByUserID = dEntity.CommentedByUserID;
            bEntity.CommentedByUserName = dEntity.CommentedByUserName;
            bEntity.LastUpdatedDateTime = dEntity.LastUpdatedDateTime;

            return bEntity;
        }

        protected override SSIP_WfComment ConvertToDataEntity(CommentEntity bEntity, SSIP_WfComment dEntity)
        {
            dEntity.CommentID = bEntity.CommentID;
            dEntity.CommentTypeID = bEntity.CommentTypeID;
            dEntity.ApplicationInstanceID = bEntity.ApplicationInstanceID;
            dEntity.ActivityInstanceGUID = new Guid(bEntity.ActivityInstanceGUID);
            dEntity.ActivityGUID = new Guid(bEntity.ActivityGUID);
            dEntity.ActivityName = bEntity.ActivityName;
            dEntity.Comment = bEntity.Comment;
            dEntity.IsPassed = (bEntity.IsPassed != null && bEntity.IsPassed.HasValue) ? bEntity.IsPassed.Value : dEntity.IsPassed;
            dEntity.Comment = bEntity.Comment;
            dEntity.ProcessInstanceGUID = new Guid(bEntity.ProcessInstanceGUID);
            dEntity.CreatedDateTime = bEntity.CreatedDateTime;
            dEntity.CommentedByUserID = bEntity.CommentedByUserID;
            dEntity.CommentedByUserName = bEntity.CommentedByUserName;
            dEntity.LastUpdatedDateTime = (bEntity.LastUpdatedDateTime != null && bEntity.LastUpdatedDateTime.HasValue) ?
            bEntity.LastUpdatedDateTime : dEntity.LastUpdatedDateTime;

            return dEntity;
        }
    }
}
