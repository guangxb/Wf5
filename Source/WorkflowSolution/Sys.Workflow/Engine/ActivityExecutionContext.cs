using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Common;
using Sys.Workflow.Utility;
using Sys.Workflow.DataModel;
using Sys.Workflow.Business;

namespace Sys.Workflow.Engine
{
    ///// <summary>
    ///// 活动执行的上下文对象
    ///// </summary>
    //internal abstract class ActivityExecutionContext
    //{
    //    #region ActivityExecutionObject 属性列表

    //    internal ProcessModel ProcessModel { get; set; }
    //    internal ProcessInstanceEntity ProcessInstance { get; set; }
    //    internal ActivityEntity Activity { get; set; }
    //    internal ActivityResource ActivityResource { get; set; }
    //    internal ActivityInstanceEntity ActivityInstance { get; set; }
    //    internal long TaskID { get; set; }

    //    #endregion

    //    #region ActivityExecutionObject 构造函数
    //    /// <summary>
    //    /// 开始节点的构造执行上下文对象
    //    /// </summary>
    //    /// <param name="processModel"></param>
    //    /// <param name="processInstance"></param>
    //    /// <param name="activity"></param>
    //    /// <param name="activityResource"></param>
    //    internal ActivityExecutionContext(ProcessModel processModel,
    //        ProcessInstanceEntity processInstance,
    //        ActivityEntity activity,
    //        ActivityResource activityResource)
    //    {
    //        ProcessModel = processModel;
    //        ProcessInstance = processInstance;
    //        Activity = activity;
    //        ActivityResource = activityResource;
    //    }

    //    /// <summary>
    //    /// 根据TaskID，装载已运行节点构造执行上下文对象
    //    /// </summary>
    //    /// <param name="processModel"></param>
    //    /// <param name="activityResource"></param>
    //    internal ActivityExecutionContext(ProcessModel processModel,
    //        ActivityResource activityResource)
    //    {
    //        ProcessModel = processModel;
    //        ActivityResource = activityResource;
    //    }
    //    #endregion
    //}
}
