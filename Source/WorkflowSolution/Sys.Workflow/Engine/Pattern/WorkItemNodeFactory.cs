using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Business;
using Sys.Workflow.Common;

namespace Sys.Workflow.Engine
{
    internal class WorkItemNodeFactory
    {
        /// <summary>
        /// 根据节点对象互获取
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        internal static NodeBase CreateNewNode(ActivityEntity activity)
        {
            NodeBase node = null;
            if (activity.NodeType == NodeTypeEnum.TaskNode)
            {
                node = new TaskNode(activity);
            }
            else
            {
                if ((activity.TaskImplementDetail.ImplementationType | ImplementationTypeEnum.Automantic)
                          == ImplementationTypeEnum.Automantic)
                {
                    if (activity.NodeType == NodeTypeEnum.PluginNode)
                        node = new PluginNode(activity);
                    else if (activity.NodeType == NodeTypeEnum.ScriptNode)
                        node = new ScriptNode(activity);
                    else
                        throw new XmlDefinitionException(string.Format("不正确的节点类型：{0},执行方法：{1}",
                            activity.NodeType.ToString(),
                            "NodeMediatorAutomantic.CreateNewNode()")
                            );
                }
                else
                {
                    throw new XmlDefinitionException(string.Format("不正确的节点类型：{0},执行方法：{1}",
                        activity.NodeType.ToString(),
                        "NodeMediatorAutomantic.CreateNewNode()")
                        );
                }
            }
            return node;
        }
    }
}
