using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.Engine
{
    /// <summary>
    /// 工作项执行状态
    /// </summary>
    public enum WfExecutedStatus
    {
        /// <summary>
        /// 未知
        /// </summary>
        NotStarted = 0x0,

        /// <summary>
        /// 执行成功
        /// </summary>
        Successed = 0x1,

        /// <summary>
        /// 发生运行时错误
        /// </summary>
        Failed = 0x2,

        /// <summary>
        /// 解析XML错误
        /// </summary>
        XmlError = 0x4,

        /// <summary>
        /// 需要其他必需的并行分支
        /// </summary>
        RequiredOtherAndSplitNode = 0x8,
        
        /// <summary>
        /// 等待其它需要合并的分支
        /// </summary>
        WaitingForOthersJoin = 0x16,

        /// <summary>
        /// 第一个满足条件的节点已经被执行，此后的节点被阻止在XOrJoin节点
        /// </summary>
        FallBehindOfXOrJoin = 0x32,

        /// <summary>
        /// 不能继续运行
        /// </summary>
        CannotContinueRunning = 0x56
    }
}
