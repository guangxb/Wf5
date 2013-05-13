using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Business;
using Sys.Workflow.Common;

namespace Sys.Workflow.Engine
{
    internal class GatewayNodeFactory
    {
        internal static NodeBase CreateNewNode(ActivityEntity activity)
        {
            NodeBase node = null;
            if (activity.NodeType == NodeTypeEnum.GatewayNode)
            {
                if (activity.GatewayDirectionType == GatewayDirectionEnum.AndSplit)
                {
                    node = new AndSplitNode(activity);
                }
                else if (activity.GatewayDirectionType == GatewayDirectionEnum.OrSplit)
                {
                    node = new OrSplitNode(activity);
                }
                else if (activity.GatewayDirectionType == GatewayDirectionEnum.XOrSplit)
                {
                    node = new XOrSplitNode(activity);
                }
                else if (activity.GatewayDirectionType == GatewayDirectionEnum.ComplexSplit)
                {
                    node = new ComplexSplitNode(activity);
                }
                else if (activity.GatewayDirectionType == GatewayDirectionEnum.AndJoin)
                {
                    node = new AndJoinNode(activity);
                }
                else if (activity.GatewayDirectionType == GatewayDirectionEnum.OrJoin)
                {
                    node = new OrJoinNode(activity);
                }
                else if (activity.GatewayDirectionType == GatewayDirectionEnum.XOrJoin)
                {
                    node = new XOrJoinNode(activity);
                }
                else if (activity.GatewayDirectionType == GatewayDirectionEnum.ComplexJoin)
                {
                    node = new ComplexJoinNode(activity);
                }
                else
                {
                    throw new XmlDefinitionException(string.Format("不明确的节点分支Gateway类型！{0}", activity.GatewayDirectionType.ToString()));
                }
            }
            else
            {
                throw new XmlDefinitionException(string.Format("不明确的节点类型！{0}", activity.NodeType.ToString()));
            }
            return node;
        }
    }
}
