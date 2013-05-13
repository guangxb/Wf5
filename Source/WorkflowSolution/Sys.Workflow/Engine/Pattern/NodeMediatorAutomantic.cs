using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Sys.Workflow.Common;
using Sys.Workflow.Business;
using Sys.Workflow.DataModel;
using Sys.Workflow.Utility;

namespace Sys.Workflow.Engine
{
    internal class NodeMediatorAutomantic : NodeMediator
    {
        internal NodeMediatorAutomantic(ActivityForwardContext executionObject, ISession session)
            : base(executionObject, session)
        {

        }

        internal override void ExecuteWorkItem()
        {
            try
            {
                WorkItem workItem = (WorkItem)CreateNewNode(WfExecutionContext.Activity);
                workItem.ActivityInstance = WfExecutionContext.ActivityInstance;

                //如果是自动类型的任务节点，则要执行自动实现的内部逻辑
                if (workItem.IsAutomanticWorkItem)
                {
                    IDynamicRunable dynamicNode = (IDynamicRunable)workItem;
                    dynamicNode.InvokeMethod(workItem.Activity.TaskImplementDetail,
                        WfExecutionContext.ActivityResource.UserParameters);
                }

                //完成当前的工作项节点(自动执行的工作项，没有具体的任务生成，任务ID参数为：0)
                workItem.CompleteWorkItem(0, WfExecutionContext.ActivityResource,
                    this.Session);

                //获取下一步节点列表：并继续执行
                ContinueForwardCurrentNode(workItem,
                    WfExecutionContext.ActivityResource);
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        internal override NodeBase CreateNewNode(ActivityEntity activity)
        {
            if ((activity.TaskImplementDetail.ImplementationType | ImplementationTypeEnum.Automantic)
                   == ImplementationTypeEnum.Automantic)
            {
                if (activity.NodeType == NodeTypeEnum.PluginNode)
                    return new PluginNode(activity);
                else if (activity.NodeType == NodeTypeEnum.ScriptNode)
                    return new ScriptNode(activity);
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
    }
}
