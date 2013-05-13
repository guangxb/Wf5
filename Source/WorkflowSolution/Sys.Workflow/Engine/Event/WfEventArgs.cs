using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Business;

namespace Sys.Workflow.Engine
{
    /// <summary>
    /// 工作流Event
    /// </summary>
    public class WfEventArgs : EventArgs
    {
        ///// <summary>
        ///// 流程实例ID
        ///// </summary>
        //public Guid ProcessInstanceGUID { get; set; }

        ///// <summary>
        ///// 获取下一步节点列表时的结果类型
        ///// </summary>
        //public NextActivityMatchedResult NextActivityMatchedResult { get; set; }

        /// <summary>
        /// 工作项执行结果
        /// </summary>
        public WfExecutedResult WfExecutedResult { get; set; }

        public WfEventArgs(WfExecutedResult result)
            : base()
        {
            WfExecutedResult = result;
        }
    }
}
