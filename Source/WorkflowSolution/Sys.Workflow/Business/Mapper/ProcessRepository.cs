using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlTypes;
using Sys.Workflow.DataModel;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 流程定义数据转换类
    /// </summary>
    internal class ProcessRepository : RepositoryBase<ProcessEntity, SSIP_WfProcess>
    {
        protected override void SetLinqDataContext()
        {
            base.LinqDataContext = DataContextFactory.CreateWfLinqDataContext();
        }

        #region Process 数据转换
        /// <summary>
        /// 转换成业务实体
        /// </summary>
        /// <param name="bEntity"></param>
        /// <param name="dEntity"></param>
        /// <returns></returns>
        protected override ProcessEntity ConvertToBusinessEntity(ProcessEntity bEntity, SSIP_WfProcess dEntity)
        {
            bEntity.ProcessGUID = dEntity.ProcessGUID.ToString();
            bEntity.ProcessName = dEntity.ProcessName;
            bEntity.AppCategoryID = dEntity.AppCategoryID;
            bEntity.PageUrl = dEntity.PageUrl;
            bEntity.XmlFileName = dEntity.XmlFileName;
            bEntity.XmlFilePath = dEntity.XmlFilePath; 
            bEntity.Description = dEntity.Description;
            bEntity.CreatedDateTime = dEntity.CreatedDateTime;
            bEntity.LastUpdatedDateTime = dEntity.LastUpdatedDateTime;
            return bEntity;
        }

        /// <summary>
        /// 转换成数据实体
        /// </summary>
        /// <param name="bEntity"></param>
        /// <param name="dEntity"></param>
        /// <param name="isToInsert"></param>
        /// <returns></returns>
        protected override SSIP_WfProcess ConvertToDataEntity(ProcessEntity bEntity, SSIP_WfProcess dEntity)
        {
            dEntity.ProcessGUID = new Guid(bEntity.ProcessGUID);
            dEntity.ProcessName = bEntity.ProcessName;
            dEntity.AppCategoryID = bEntity.AppCategoryID;
            dEntity.PageUrl = bEntity.PageUrl;
            dEntity.XmlFileName = bEntity.XmlFileName;
            dEntity.XmlFilePath = bEntity.XmlFilePath;
            dEntity.Description = bEntity.Description;
            dEntity.CreatedDateTime = bEntity.CreatedDateTime;
            dEntity.LastUpdatedDateTime = bEntity.LastUpdatedDateTime;
            return dEntity;
        }
        #endregion

    }
}
