using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Business;
using Sys.Workflow.Common;
using Sys.Workflow.DataModel;

namespace Sys.Workflow.Engine
{
    internal class OrJoinNode : NodeBase, ICompleteAutomaticlly
    {
        internal OrJoinNode(ActivityEntity activity)
            : base(activity)
        {

        }

        internal override void AfterActivityInstanceObjectCreated()
        {
            //进入运行状态
            base.ActivityInstance.State = (short)NodeStateEnum.Running;
            base.ActivityInstance.GatewayDirectionTypeID = (short)GatewayDirectionEnum.OrJoin;
        }

        #region ICompleteAutomaticlly 成员

        /// <summary>
        /// OrJoin合并时的节点完成方法
        /// 1. 如果有满足条件的OrJoin前驱转移，则会重新生成新的OrJoin节点实例
        /// </summary>
        /// <param name="processInstance"></param>
        /// <param name="fromTransition"></param>
        /// <param name="fromActivityInstance"></param>
        /// <param name="activityResource"></param>
        /// <param name="wfLinqDataContext"></param>
        public GatewayExecutedResult CompleteAutomaticlly(ProcessInstanceEntity processInstance, 
            TransitionEntity fromTransition, 
            ActivityInstanceEntity fromActivityInstance, 
            ActivityResource activityResource, 
            ISession session)
        {
            var toActivityInstance = base.CreateActivityInstanceObject(processInstance, activityResource.LogonUser);

            base.InsertActivityInstance(toActivityInstance, 
                session);

            base.CompleteActivityInstance(toActivityInstance.ActivityInstanceGUID,
                activityResource,
                session);

            SyncActivityInstanceObjectState(NodeStateEnum.Completed);

            base.InsertTransitionInstance(processInstance,
                fromTransition,
                fromActivityInstance,
                toActivityInstance,
                TransitionTypeEnum.Forward,
                activityResource.LogonUser,
                session);

            GatewayExecutedResult result = GatewayExecutedResult.CreateGatewayExecutedResult(GatewayExecutedStatus.Successed);
            return result;
        }

        #endregion
    }
}
