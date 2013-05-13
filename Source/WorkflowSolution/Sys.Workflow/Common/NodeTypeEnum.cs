using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.Common
{
    /// <summary>
    /// 活动类型
    /// </summary>
    public enum NodeTypeEnum : int
    {
        /// <summary>
        /// 开始节点
        /// </summary>
        StartNode = 1,

        /// <summary>
        /// 结束节点
        /// </summary>
        EndNode = 2,

        /// <summary>
        /// 人工任务节点
        /// </summary>
        TaskNode = 4,

        /// <summary>
        /// 网关节点
        /// </summary>
        GatewayNode = 8,

        /// <summary>
        /// 插件节点
        /// </summary>
        PluginNode = 16,

        /// <summary>
        /// 脚本节点
        /// </summary>
        ScriptNode = 32,

        /// <summary>
        /// 可执行节点
        /// </summary>
        WorkItemNode = 52
    }
}
