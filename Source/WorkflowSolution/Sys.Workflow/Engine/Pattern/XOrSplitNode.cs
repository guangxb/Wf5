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
    /// 异或类型的节点
    /// </summary>
    internal class XOrSplitNode : NodeBase, ICompleteAutomaticlly
    {
         internal XOrSplitNode(ActivityEntity activity)
            : base(activity)
        { 
        }

         internal override void AfterActivityInstanceObjectCreated()
         {
             base.ActivityInstance.GatewayDirectionTypeID = (short)GatewayDirectionEnum.XOrSplit;
         }

        #region ICompleteAutomaticlly 成员

        public GatewayExecutedResult CompleteAutomaticlly(ProcessInstanceEntity processInstance,
            TransitionEntity fromToTransition,
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

            base.SyncActivityInstanceObjectState(NodeStateEnum.Completed);

            //写节点转移实例数据
            base.InsertTransitionInstance(processInstance,
                fromToTransition,
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
