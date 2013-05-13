using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.Engine
{
    /// <summary>
    /// 运行器类型
    /// </summary>
    public enum RuntimeManagerType
    {
        /// <summary>
        /// 启动运行时
        /// </summary>
        StartupRuntime = 1,

        /// <summary>
        /// 执行运行时
        /// </summary>
        RunningRuntime = 2,

        /// <summary>
        /// 撤销运行时
        /// </summary>
        WithdrawRuntime = 3,

        /// <summary>
        /// 退回运行时
        /// </summary>
        RejectRuntime = 4,

        /// <summary>
        /// 返签运行时
        /// </summary>
        ReverseRuntime = 5
    }
}
