using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.Common
{
    /// <summary>
    /// 节点运行状态
    /// </summary>
    internal enum NodeStateEnum
    {
        /// <summary>
        /// 准备状态
        /// </summary>
        Ready = 1,

        /// <summary>
        /// 运行状态
        /// </summary>
        Running = 2,

        /// <summary>
        /// 完成状态
        /// </summary>
        Completed = 4,
        
        /// <summary>
        /// 撤销状态
        /// </summary>
        Withdrawed = 8,

        /// <summary>
        /// 退回状态
        /// </summary>
        Rejected = 16
    }
}
