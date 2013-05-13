using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.Common
{
    public enum ProcessStateEnum
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
        /// 完成
        /// </summary>
        Completed = 4,

        /// <summary>
        /// 挂起
        /// </summary>
        Suspended = 8,

        /// <summary>
        /// 取消
        /// </summary>
        Canceled = 16,

        /// <summary>
        /// 终止
        /// </summary>
        Discarded = 32
    }
}
