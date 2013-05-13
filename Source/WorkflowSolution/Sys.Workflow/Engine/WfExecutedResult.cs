using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Business;
using Sys.Workflow.Common;

namespace Sys.Workflow.Engine
{
    /// <summary>
    /// 工作流执行结果类
    /// </summary>
    public class WfExecutedResult
    {
        /// <summary>
        /// 状态
        /// </summary>
        public WfExecutedStatus Status { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set;}

        /// <summary>
        /// 流程实例ID
        /// </summary>
        public Guid ProcessInstanceGUID { get; set; }

        /// <summary>
        /// 获取下一步节点列表时的结果类型
        /// </summary>
        public IList<NodeView> NextActivityTree { get; set; }

        /// <summary>
        /// 流程执行结果封装类
        /// </summary>
        /// <param name="Message">消息内容</param>
        /// <returns></returns>
        public static WfExecutedResult Success(string message = null)
        {
            var result = new WfExecutedResult();
            result.Status = WfExecutedStatus.Successed;
            result.Message = message;

            return result;
        }

        /// <summary>
        /// 流程执行结果封装类
        /// </summary>
        /// <param name="Message">消息内容</param>
        /// <returns></returns>
        public static WfExecutedResult Failed(string errorMessage)
        {
            var result = new WfExecutedResult();
            result.Status = WfExecutedStatus.Failed;
            result.Message = errorMessage;
            return result;
        }

        public static WfExecutedResult XmlError(string errorMessage)
        {
            var result = new WfExecutedResult();
            result.Status = WfExecutedStatus.XmlError;
            result.Message = errorMessage;
            return result;
        }

        /// <summary>
        /// 流程执行结果封装类
        /// </summary>
        /// <param name="status">异常状态</param>
        /// <param name="exceptionMessage">消息提示</param>
        /// <returns></returns>
        public static WfExecutedResult Exception(WfExecutedStatus status, string exceptionMessage)
        {
            var result = new WfExecutedResult();
            result.Status = status;
            result.Message = exceptionMessage;
            return result;
        }
    }

    #region WfExecutedResult 泛型类
    ///// <summary>
    ///// 响应消息类
    ///// </summary>
    //public class WfExecutedResult<T> : WfExecutedResult
    //    where T : class
    //{
    //    /// <summary>
    //    /// 业务实体
    //    /// </summary>
    //    public T ResultObject
    //    {
    //        get;
    //        set;
    //    }


    //    /// <summary>
    //    /// 响应消息封装类
    //    /// </summary>
    //    /// <param name="Status">状态:1-成功; 0-缺省; -1失败</param>
    //    /// <param name="Message">消息内容</param>
    //    /// <returns></returns>
    //    public static WfExecutedResult<T> Success(T t, string message = null)
    //    {
    //        var result = new WfExecutedResult<T>();
    //        result.ResultObject = t;
    //        result.Status = WfExecutedStatus.Successed;
    //        result.Message = message;

    //        return result;
    //    }

    //    /// <summary>
    //    /// Http 响应消息封装类
    //    /// </summary>
    //    /// <param name="Status">状态:1-成功; 0-缺省; -1失败</param>
    //    /// <param name="Message">消息内容</param>
    //    /// <returns></returns>
    //    public static WfExecutedResult<T> Failed(T t, string message = null)
    //    {
    //        var result = new WfExecutedResult<T>();
    //        result.ResultObject = t;
    //        result.Status = WfExecutedStatus.Failed;
    //        result.Message = message;

    //        return result;
    //    }

    //    public static WfExecutedResult<T> XmlError(T t, string errorMessage)
    //    {
    //        var result = new WfExecutedResult<T>();
    //        result.ResultObject = t;
    //        result.Status = WfExecutedStatus.XmlError;
    //        result.Message = errorMessage;
    //        return result;
    //    }

    //    /// <summary>
    //    /// 流程执行结果封装类
    //    /// </summary>
    //    /// <param name="status">异常状态</param>
    //    /// <param name="exceptionMessage">消息提示</param>
    //    /// <returns></returns>
    //    public static WfExecutedResult<T> Exception(T t, WfExecutedStatus status, string exceptionMessage)
    //    {
    //        var result = new WfExecutedResult<T>();
    //        result.ResultObject = t;
    //        result.Status = status;
    //        result.Message = exceptionMessage;
    //        return result;
    //    }
    //}

    #endregion
}
