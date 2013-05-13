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
    /// 并行合并节点
    /// </summary>
    internal class AndJoinNode : NodeBase, ICompleteAutomaticlly
    {
        internal AndJoinNode(ActivityEntity activity)
            : base(activity)
        {

        }

        internal override void AfterActivityInstanceObjectCreated()
        {
            //计算总需要的Token数目
            base.ActivityInstance.TokensRequired = base.GetTokensRequired();
            //进入运行状态
            base.ActivityInstance.State = (short)NodeStateEnum.Running;
            base.ActivityInstance.GatewayDirectionTypeID = (short)GatewayDirectionEnum.AndJoin;

        }

        #region ICompleteAutomaticlly 成员

        public GatewayExecutedResult CompleteAutomaticlly(ProcessInstanceEntity processInstance, 
            TransitionEntity fromTransition, 
            ActivityInstanceEntity fromActivityInstance, 
            ActivityResource activityResource, 
            ISession session)
        {
            //检查是否有运行中的合并节点实例
            ActivityInstanceEntity joinNode = base.ActivityInstanceManager.GetActivityWithRunningState(processInstance.ProcessInstanceGUID,
                base.Activity.ActivityGUID,
                session);

            if (joinNode == null)
            {
                var aiEntity = base.CreateActivityInstanceObject(processInstance, activityResource.LogonUser);
                base.InsertActivityInstance(aiEntity,
                    session);
            }
            else
            {
                //更新节点的活动实例属性
                base.ActivityInstance = joinNode;
                int tokensRequired = base.ActivityInstance.TokensRequired;
                int tokensHad = base.ActivityInstance.TokensHad;

                //更新Token数目
                base.ActivityInstanceManager.IncreaseTokensHad(base.ActivityInstance.ActivityInstanceGUID,
                    activityResource.LogonUser,
                    session);

                if ((tokensHad + 1) == tokensRequired)
                {
                    //如果达到完成节点的Token数，则设置该节点状态为完成
                    base.CompleteActivityInstance(base.ActivityInstance.ActivityInstanceGUID,
                        activityResource,
                        session);

                    SyncActivityInstanceObjectState(NodeStateEnum.Completed);
                }
            }

            GatewayExecutedResult result = GatewayExecutedResult.CreateGatewayExecutedResult(GatewayExecutedStatus.Successed);
            return result;
        }

        #endregion
    }
}
