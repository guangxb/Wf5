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
    /// <summary>
    /// 节点执行器的抽象类
    /// </summary>
    internal abstract class NodeMediator
    {
        #region 属性列表
        internal ProcessModel _processModel;
        internal ProcessModel ProcessModel
        {
            get
            {
                return _processModel;
            }
        }

        private ActivityForwardContext _wfExecutionContext;
        protected ActivityForwardContext WfExecutionContext
        {
            get
            {
                return _wfExecutionContext;
            }
        }

        protected Queue<WorkItem> _ToDoAutoWorkItemQueue;
        internal Queue<WorkItem> ToDoAutoWorkItemQueue
        {
            get
            {
                if (_ToDoAutoWorkItemQueue == null)
                    _ToDoAutoWorkItemQueue = new Queue<WorkItem>();
                
                return _ToDoAutoWorkItemQueue;
            }
        }

        private ISession _session;
        protected ISession Session
        {
            get
            {
                return _session;
            }
        }

        #endregion

        #region 抽象方法列表

        internal abstract NodeBase CreateNewNode(ActivityEntity activity);
        internal abstract void ExecuteWorkItem();
        
        #endregion

        internal NodeMediator(ActivityForwardContext executionObject, ISession session)
        {
            _wfExecutionContext = executionObject;
            _session = session;
            _processModel = executionObject.ProcessModel;
        }

        /// <summary>
        /// 遍历执行当前节点后面的节点
        /// </summary>
        /// <param name="previousActivityInstance"></param>
        /// <param name="currentNode"></param>
        internal virtual void ContinueForwardCurrentNode(NodeBase fromNode,
            ActivityResource activityResource)
        {
            var nextActivityMatchedResult = this.ProcessModel.GetNextActivityList(fromNode.Activity,
                activityResource.ConditionKeyValuePair,
                activityResource,
                (a, b) => a.NextActivityPerformers.ContainsKey(b.ActivityGUID));

            if (nextActivityMatchedResult.MatchedType != NextActivityMatchedType.Successed
                || nextActivityMatchedResult.Root.HasChildren == false)
            {
                throw new WorkflowException("没有匹配的后续流转节点，流程虽然能处理当前节点，当无法流转到下一步！");
            }

            ContinueForwardCurrentNodeRecurisivly(nextActivityMatchedResult.Root,
                fromNode,
                activityResource.ConditionKeyValuePair);

            WfExecutedResult workItemExecutedResult = WfExecutedResult.Success("执行后续节点成功！");
        }

        /// <summary>
        /// 递归执行节点
        /// </summary>
        /// <param name="root"></param>
        /// <param name="fromNode"></param>
        /// <param name="conditionKeyValuePair"></param>
        protected void ContinueForwardCurrentNodeRecurisivly(NextActivityComponent root,
            NodeBase fromNode,
            IDictionary<string, string> conditionKeyValuePair)
        {
            foreach (NextActivityComponent c in root)
            {
                if (c.HasChildren)
                {
                    //此节点类型为分支或合并节点类型：首先需要实例化当前节点(自动完成)
                    NodeBase gatewayNode = GatewayNodeFactory.CreateNewNode(c.Activity);
                    ICompleteAutomaticlly autoGatewayNode = (ICompleteAutomaticlly)gatewayNode;
                    GatewayExecutedResult gatewayResult = autoGatewayNode.CompleteAutomaticlly(WfExecutionContext.ProcessInstance,
                        c.Transition,
                        fromNode.ActivityInstance,
                        WfExecutionContext.ActivityResource,
                        Session);

                    if (gatewayResult.Status == GatewayExecutedStatus.Successed)
                    {
                        //遍历后续子节点
                        ContinueForwardCurrentNodeRecurisivly(c,
                            gatewayNode,
                            conditionKeyValuePair);
                    }
                    else
                    {
                        WfExecutedResult workItemExecutedResult = WfExecutedResult.Exception(
                            WfExecutedStatus.FallBehindOfXOrJoin, 
                            "第一个满足条件的节点已经被成功执行，此后的节点被阻止在XOrJoin节点!");
                    }
                }
                else if (c.Activity.IsWorkItemNode)
                {
                    //此节点类型为任务节点：根据fromActivityInstance的类型判断是否可以创建任务
                    if (fromNode.ActivityInstance.State == (short)NodeStateEnum.Completed)
                    {
                        //创建新任务节点
                        WorkItem workItem = (WorkItem)WorkItemNodeFactory.CreateNewNode(c.Activity);
                        workItem.CreateActivityTaskAndTransitionInstances(WfExecutionContext.ProcessInstance,
                            fromNode.ActivityInstance,
                            c.Transition,
                            c.Transition.DirectionType == TransitionDirectionTypeEnum.Loop ? 
                                TransitionTypeEnum.Loop : TransitionTypeEnum.Forward, //根据Direction方向确定是否是自身循环
                            WfExecutionContext.ActivityResource,
                            Session);

                        //新任务加入队列
                        if (workItem.IsAutomanticWorkItem)
                        {
                            ToDoAutoWorkItemQueue.Enqueue(workItem);
                        }
                    }
                    else
                    {
                        //下一步的任务节点没有创建，需给出提示信息
                        if ((fromNode.Activity.GatewayDirectionType | GatewayDirectionEnum.AllJoinType)
                            == GatewayDirectionEnum.AllJoinType)
                        {
                            WfExecutedResult workItemExecutedResult = WfExecutedResult.Exception(
                                WfExecutedStatus.WaitingForOthersJoin,
                                "等待其它需要合并的分支!");
                        }
                    }
                }
                else if (c.Activity.NodeType == NodeTypeEnum.EndNode)
                {
                    //此节点为完成结束节点，结束流程
                    NodeBase endNode = new EndNode(c.Activity);
                    ICompleteAutomaticlly autoEndNode = (ICompleteAutomaticlly)endNode;
                    autoEndNode.CompleteAutomaticlly(WfExecutionContext.ProcessInstance,
                        c.Transition,
                        fromNode.ActivityInstance,
                        WfExecutionContext.ActivityResource,
                        Session);
                }
                else
                {
                    WfExecutedResult workItemExecutedResult = WfExecutedResult.XmlError(
                        string.Format("XML文件定义了未知的节点类型，执行失败，节点类型信息：{0}", 
                        c.Activity.NodeType.ToString()));
                }
            }
        }
    }
}
