using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Business;

namespace Sys.Workflow.Engine
{
    /// <summary>
    /// 流程执行实例的事件完成参数类
    /// </summary>
    public class WfRunnerInstanceCompletedEventArgs : WfEventArgs
    {
        /// <summary>
        /// 获取下一步节点列表时的结果类型
        /// </summary>
        public NextActivityMatchedResult NextActivityMatchedResult
        {
            get;
            set;
        }

        /// <summary>
        /// 工作项执行结果
        /// </summary>
        public WorkflowExecutedResult WorkItemExecutedResult
        {
            get;
            set;
        }
    }
}
