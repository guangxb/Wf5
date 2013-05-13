using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Common;
using Sys.Workflow.Utility;
using Sys.Workflow.DataModel;
using Sys.Workflow.Business;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 审批意见类
    /// </summary>
    internal class CommentManager
    {
        private RepositoryBase _commentRepository;
        private RepositoryBase CommentRepository
        {
            get
            {
                if (_commentRepository == null)
                {
                    _commentRepository = new RepositoryBase(null);
                }
                return _commentRepository;
            }
        }

        internal CommentEntity GetById(long commentID)
        {
            return CommentRepository.GetById<CommentEntity>(commentID.ToString());
        }

        //internal IList<CommentEntity> Select(int pageIndex,
        //   int pageSize,
        //   out long allRowsCount,
        //   string orderBy,
        //   string where,
        //   params object[] values)
        //{
        //    return CommentRepository.Select(pageIndex, pageSize, out allRowsCount, orderBy, false, where, values);
        //}

        //internal IList<CommentEntity> Select(int pageIndex,
        //   int pageSize,
        //   out long allRowsCount,
        //   string orderBy,
        //   long applicationInstanceID)
        //{
        //    string where = "ApplicationInstanceID == @0";
        //    return Select(pageIndex, pageSize, out allRowsCount, orderBy, where, applicationInstanceID);
        //}

        #region 新增、更新和删除流程数据
        internal long Insert(CommentEntity entity)
        {
            return CommentRepository.Insert(entity).CommentID;
        }

        public void Update(CommentEntity entity)
        {
            CommentRepository.Update(entity);
        }

        public void Delete(long commentID)
        {
            CommentRepository.Delete(commentID.ToString());
        }
        #endregion  
    }
}
