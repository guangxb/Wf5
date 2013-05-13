using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Common;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 下一步节点的工厂类
    /// </summary>
    public class NextActivityComponentFactory
    {
        /// <summary>
        /// 创建下一步活动的节点
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        internal static NextActivityComponent CreateNextActivityComponent(TransitionEntity transition,
            ActivityEntity activity)
        {
            NextActivityComponent component = null;
            if (activity.NodeType == NodeTypeEnum.TaskNode
                || activity.NodeType == NodeTypeEnum.EndNode)
            {
                string name = "单一节点";
                component = new NextActivityItem(name, transition, activity);
            }
            else
            {
                string name = string.Empty;
                if (activity.GatewayDirectionType == Sys.Workflow.Common.GatewayDirectionEnum.AndSplit)
                {
                    name = "必全选节点";
                }
                else
                {
                    name = "或多选节点";
                }

                component = new NextActivityGateway(name, transition, activity);
            }
            return component;
        }

        /// <summary>
        /// 创建根显示节点
        /// </summary>
        /// <returns></returns>
        internal static NextActivityComponent CreateNextActivityComponent()
        {
            NextActivityComponent root = new NextActivityGateway("下一步步骤列表", null,  null);
            return root;
        }

        /// <summary>
        /// 根据现有下一步节点列表，创建新的下一步节点列表对象
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal static NextActivityComponent CreateNextActivityComponent(NextActivityComponent c)
        {
            NextActivityComponent newComp = CreateNextActivityComponent(c.Transition, c.Activity);
            return newComp;
        }
    }
}
