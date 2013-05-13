using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Common;
using Sys.Workflow.Utility;
using Sys.Workflow.DataModel;
using Sys.Workflow.Business;

namespace Sys.Workflow
{
    public partial class WfFacade
    {
        #region 审批意见对外接口
        ///// <summary>
        ///// 选择单条审批意见
        ///// </summary>
        ///// <param name="commentID"></param>
        ///// <returns></returns>
        //public static CommentEntity SelectComment(long commentID)
        //{
        //    CommentManager cm = new CommentManager();
        //    return cm.GetById(commentID);
        //}

        ///// <summary>
        ///// 按条件查询意见
        ///// </summary>
        ///// <param name="pageIndex"></param>
        ///// <param name="pageSize"></param>
        ///// <param name="allRowsCount"></param>
        ///// <param name="orderBy"></param>
        ///// <param name="where"></param>
        ///// <param name="values"></param>
        ///// <returns></returns>
        //public static IList<CommentEntity> SelectComment(int pageIndex,
        //    int pageSize,
        //    out long allRowsCount,
        //    string orderBy,
        //    string where,
        //    params object[] values)
        //{
        //    CommentManager cm = new CommentManager();
        //    return cm.Select(pageIndex, pageSize, out allRowsCount, orderBy, where, values);
        //}

        ///// <summary>
        ///// 按应用实例ID，查询审批意见
        ///// </summary>
        ///// <param name="pageIndex"></param>
        ///// <param name="pageSize"></param>
        ///// <param name="allRowsCount"></param>
        ///// <param name="orderBy"></param>
        ///// <param name="appInstanceID"></param>
        ///// <returns></returns>
        //public static IList<CommentEntity> SelectComment(int pageIndex,
        //   int pageSize,
        //   out long allRowsCount,
        //   string orderBy,
        //   long appInstanceID)
        //{
        //    CommentManager cm = new CommentManager();

        //    string where = "AppInstanceID == @0";
        //    return cm.Select(pageIndex, pageSize, out allRowsCount, orderBy, where, appInstanceID);
        //}

        ///// <summary>
        ///// 插入意见
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        //public static long InsertComment(CommentEntity entity)
        //{
        //    try
        //    {
        //        CommentManager cm = new CommentManager();
        //        return cm.Insert(entity);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        //throw new WfDataException(string.Format("插入审批意见失败，详细信息：{0}", ex.Message), ex);
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// 更新审批意见
        ///// </summary>
        ///// <param name="entity"></param>
        //public static void UpdateComment(CommentEntity entity)
        //{
        //    try
        //    {
        //        CommentManager cm = new CommentManager();
        //        cm.Update(entity);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        //throw new WfDataException(string.Format("更新审批意见失败，详细信息：{0}", ex.Message), ex);
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// 删除审批意见
        ///// </summary>
        ///// <param name="commentID"></param>
        //public static void DeleteComment(long commentID)
        //{
        //    try
        //    {
        //        CommentManager cm = new CommentManager();
        //        cm.Delete(commentID);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        //throw new WfDataException(string.Format("删除审批意见失败，详细信息：{0}", ex.Message), ex);
        //        throw;
        //    }
        //}
        #endregion
    }
}
