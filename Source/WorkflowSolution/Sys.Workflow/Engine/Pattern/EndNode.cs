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
    /// 结束节点
    /// </summary>
    internal class EndNode : NodeBase, ICompleteAutomaticlly
    {
        internal EndNode(ActivityEntity activity)
            : base(activity)
        {

        }

        #region ICompleteAutomaticlly 成员
        /// <summary>
        /// 自动完成结束节点
        /// </summary>
        /// <param name="processInstance"></param>
        /// <param name="fromActivityInstance"></param>
        /// <param name="activityResource"></param>
        /// <param name="wfLinqDataContext"></param>
        public GatewayExecutedResult CompleteAutomaticlly(ProcessInstanceEntity processInstance,
            TransitionEntity fromToTransition,
            ActivityInstanceEntity fromActivityInstance,
            ActivityResource activityResource, 
            ISession session)
        {
            GatewayExecutedResult result = null;
            var toActivityInstance = base.CreateActivityInstanceObject(processInstance, activityResource.LogonUser);

            base.InsertActivityInstance(toActivityInstance, 
                session);

            base.CompleteActivityInstance(toActivityInstance.ActivityInstanceGUID,
                activityResource,
                session);

            //写节点转移实例数据
            base.InsertTransitionInstance(processInstance,
                fromToTransition,
                fromActivityInstance,
                toActivityInstance,
                TransitionTypeEnum.Forward,
                activityResource.LogonUser,
                session);

            //设置流程完成
            ProcessInstanceManager pim = new ProcessInstanceManager();
            pim.Complete(processInstance.ProcessInstanceGUID, activityResource.LogonUser, session);

            return result;
        }

        #endregion
    }
}
