using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using System.Text;
using Sys.Workflow.DataModel;
using Sys.Workflow.Utility;

namespace Sys.Workflow.Business
{
    public class RoleRepository : RepositoryBase<RoleEntity, SSIP_Role>
    {
        protected override void SetLinqDataContext()
        {
            base.LinqDataContext = DataContextFactory.CreateWfLinqDataContext();
        }


        #region 业务实体转换
        protected override SSIP_Role ConvertToDataEntity(RoleEntity bEntity, SSIP_Role dEntity)
        {
            dEntity.RoleID = bEntity.RoleID;
            dEntity.RoleTypeID = bEntity.RoleTypeID;
            dEntity.RoleName = bEntity.RoleName;
            dEntity.RoleCode = bEntity.RoleCode;
            dEntity.Description = bEntity.Description;
            dEntity.LastUpdatedDateTime = bEntity.LastUpdatedDateTime;

            return dEntity;
        }

        protected override RoleEntity ConvertToBusinessEntity(RoleEntity bEntity, SSIP_Role dEntity)
        {
            bEntity.RoleID = dEntity.RoleID;
            bEntity.RoleTypeID = dEntity.RoleTypeID;
            bEntity.RoleName = dEntity.RoleName;
            bEntity.RoleCode = dEntity.RoleCode;
            bEntity.Description = dEntity.Description;
            bEntity.LastUpdatedDateTime = dEntity.LastUpdatedDateTime;

            return bEntity;
        }
        #endregion
    }
}
