using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Common;
using Sys.Workflow.Business;
using Sys.Workflow.DataModel;

namespace Sys.Workflow.Engine
{
    /// <summary>
    /// 互斥合并类型的节点
    /// </summary>
    internal class XOrJoinNode : NodeBase, ICompleteAutomaticlly
    {
        internal XOrJoinNode(ActivityEntity activity)
            : base(activity)
        {

        }

        internal override void AfterActivityInstanceObjectCreated()
        {
            base.ActivityInstance.GatewayDirectionTypeID = (short)GatewayDirectionEnum.XOrJoin;           
        }


        #region ICompleteAutomaticlly 成员

        public GatewayExecutedResult CompleteAutomaticlly(ProcessInstanceEntity processInstance, 
            TransitionEntity fromTransition, 
            ActivityInstanceEntity fromActivityInstance, 
            ActivityResource activityResource, 
            ISession session)
        {
            GatewayExecutedResult result = GatewayExecutedResult.CreateGatewayExecutedResult(GatewayExecutedStatus.Unknown);

            bool canRenewInstance = false;

            //检查是否有运行中的合并节点实例
            ActivityInstanceEntity joinNode = base.ActivityInstanceManager.GetActivityWithRunningState(
                processInstance.ProcessInstanceGUID,
                base.Activity.ActivityGUID,
                session);

            if (joinNode == null)
            {
                canRenewInstance = true;
            }
            else
            {
                //判断是否可以激活下一步节点
                canRenewInstance = (joinNode.CanRenewInstance == 1);
                if (!canRenewInstance)
                {
                    result = GatewayExecutedResult.CreateGatewayExecutedResult(GatewayExecutedStatus.FallBehindOfXOrJoin);
                    return result;
                }
            }

            if (canRenewInstance)
            {
                var toActivityInstance = base.CreateActivityInstanceObject(processInstance, activityResource.LogonUser);
                base.InsertActivityInstance(toActivityInstance,
                    session);

                base.CompleteActivityInstance(toActivityInstance.ActivityInstanceGUID,
                    activityResource,
                    session);

                base.SyncActivityInstanceObjectState(NodeStateEnum.Completed);

                //写节点转移实例数据
                base.InsertTransitionInstance(processInstance,
                    fromTransition,
                    fromActivityInstance,
                    toActivityInstance,
                    TransitionTypeEnum.Forward,
                    activityResource.LogonUser,
                    session);

                result = GatewayExecutedResult.CreateGatewayExecutedResult(GatewayExecutedStatus.Successed);
            }
            return result;
        }

        #endregion
    }
}
