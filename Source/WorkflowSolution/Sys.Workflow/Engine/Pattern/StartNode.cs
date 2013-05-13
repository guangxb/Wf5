using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Business;
using Sys.Workflow.Common;
using Sys.Workflow.DataModel;

namespace Sys.Workflow.Engine
{
    /// <summary>
    /// 开始节点
    /// </summary>
    internal class StartNode : NodeBase, ICompleteAutomaticlly
    {
        internal StartNode(ActivityEntity activity)
            : base(activity)
        {

        }

        #region ICompleteAutomaticlly 成员

        public GatewayExecutedResult CompleteAutomaticlly(ProcessInstanceEntity processInstance,
            TransitionEntity fromToTransition,
            ActivityInstanceEntity fromActivityInstance, 
            ActivityResource activityResource, 
            ISession session)
        {
            //开始节点没前驱信息
            var toActivityInstance = base.CreateActivityInstanceObject(processInstance, activityResource.LogonUser);

            base.InsertActivityInstance(toActivityInstance,
                session);

            base.CompleteActivityInstance(toActivityInstance.ActivityInstanceGUID,
                activityResource,
                session);

            SyncActivityInstanceObjectState(NodeStateEnum.Completed);

            GatewayExecutedResult result = GatewayExecutedResult.CreateGatewayExecutedResult(GatewayExecutedStatus.Successed);
            return result;
        }

        #endregion
    }
}
