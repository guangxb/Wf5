using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using Sys.Workflow.Common;
using Sys.Workflow.Business;
using Sys.Workflow.DataModel;
using Sys.Workflow.Utility;

namespace Sys.Workflow.Engine
{
    internal class NodeMediatorStart : NodeMediator
    {
        internal NodeMediatorStart(ActivityForwardContext executionContext, ISession session)
            : base(executionContext, session)
        {
            
        }

        /// <summary>
        /// 执行开始节点
        /// </summary>
        /// <param name="activityExecutionObject"></param>
        /// <param name="processInstance"></param>
        internal override void ExecuteWorkItem()
        {
            try
            {
                //写入流程实例
                ProcessInstanceManager pim = new ProcessInstanceManager();
                pim.Insert(WfExecutionContext.ProcessInstance,
                    this.Session);

                //自动完成开始节点(开始节点不参与Task)
                NodeBase startNode = CreateNewNode(WfExecutionContext.Activity);
                ICompleteAutomaticlly autoStartNode = (ICompleteAutomaticlly)startNode;
                autoStartNode.CompleteAutomaticlly(WfExecutionContext.ProcessInstance,
                    null,
                    null,
                    WfExecutionContext.ActivityResource,
                    this.Session);

                //执行开始节点之后的节点集合
                ContinueForwardCurrentNode(startNode,
                   WfExecutionContext.ActivityResource);
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        internal override NodeBase CreateNewNode(ActivityEntity activity)
        {
            return new StartNode(activity);
        }
    }
}
