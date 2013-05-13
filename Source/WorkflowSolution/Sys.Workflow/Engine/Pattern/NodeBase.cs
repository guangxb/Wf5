using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Utility;
using Sys.Workflow.DataModel;
using Sys.Workflow.Business;
using Sys.Workflow.Common;

namespace Sys.Workflow.Engine
{
    /// <summary>
    /// 节点的基类
    /// </summary>
    public abstract class NodeBase
    {
        #region 属性和构造函数
        /// <summary>
        /// 节点定义属性
        /// </summary>
        public ActivityEntity Activity
        {
            get;
            set;
        }
        
        /// <summary>
        /// 节点实例
        /// </summary>
        public ActivityInstanceEntity ActivityInstance
        {
            get;
            set;
        }

        internal ProcessModel _processModel;
        internal ProcessModel ProcessModel
        {
            get
            {
                if (_processModel == null)
                {
                    _processModel = new ProcessModel(this.Activity.ProcessGUID);
                }
                return _processModel;
            }
        }

        /// <summary>
        /// 活动节点实例管理对象
        /// </summary>
        internal ActivityInstanceManager activityInstanceManager;
        internal ActivityInstanceManager ActivityInstanceManager
        {
            get
            {
                if (activityInstanceManager == null)
                {
                    activityInstanceManager = new ActivityInstanceManager();
                }
                return activityInstanceManager;
            }
        }
        

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="currentActivity"></param>
        public NodeBase(ActivityEntity currentActivity)
        {
            Activity = currentActivity;
        }
        #endregion

        #region 创建、插入和完成
        /// <summary>
        /// 创建节点对象
        /// </summary>
        /// <param name="processInstance">流程实例</param>
        internal ActivityInstanceEntity CreateActivityInstanceObject(ProcessInstanceEntity processInstance,
            WfLogonUser logonUser)
        {
            ActivityInstanceManager aim = new ActivityInstanceManager();
            this.ActivityInstance = aim.CreateActivityInstanceObject(processInstance.AppName,
                processInstance.AppInstanceID,
                processInstance.ProcessInstanceGUID,
                Activity,
                logonUser);

            AfterActivityInstanceObjectCreated();
            return this.ActivityInstance;
        }

        internal virtual void AfterActivityInstanceObjectCreated()
        {

        }



        /// <summary>
        /// 插入实例数据
        /// </summary>
        /// <param name="activityInstance"></param>
        /// <param name="wfLinqDataContext"></param>
        internal virtual void InsertActivityInstance(ActivityInstanceEntity activityInstance,
            ISession session)
        {
            ActivityInstanceManager.Insert(activityInstance, session);
        }

        /// <summary>
        /// 插入连线实例的方法
        /// </summary>
        /// <param name="processInstance"></param>
        /// <param name="fromToTransition"></param>
        /// <param name="fromActivityInstance"></param>
        /// <param name="toActivityInstance"></param>
        /// <param name="conditionParseResult"></param>
        /// <param name="wfLinqDataContext"></param>
        /// <returns></returns>
        internal virtual void InsertTransitionInstance(ProcessInstanceEntity processInstance,
            TransitionEntity fromToTransition,
            ActivityInstanceEntity fromActivityInstance,
            ActivityInstanceEntity toActivityInstance,
            TransitionTypeEnum transitionType,
            WfLogonUser logonUser,
            ISession session)
        {
            var tim = new TransitionInstanceManager();
            var transitionInstanceObject = tim.CreateTransitionInstanceObject(processInstance,
                fromToTransition,
                fromActivityInstance,
                toActivityInstance,
                transitionType,
                logonUser,
                (byte)ConditionParseResultEnum.Passed);

            tim.Insert(transitionInstanceObject,
                session);
        }

        /// <summary>
        /// 节点对象的完成方法
        /// </summary>
        /// <param name="ActivityInstanceGUID"></param>
        /// <param name="activityResource"></param>
        /// <param name="wfLinqDataContext"></param>
        internal virtual void CompleteActivityInstance(Guid ActivityInstanceGUID,
            ActivityResource activityResource,
            ISession session)
        {
            //设置完成状态
            ActivityInstanceManager.Complete(ActivityInstanceGUID,
                activityResource.LogonUser,
                session);
        }

        /// <summary>
        /// 同步内存活动实例的状态
        /// </summary>
        /// <param name="state"></param>
        internal void SyncActivityInstanceObjectState(NodeStateEnum state)
        {
            this.ActivityInstance.State = (short)state;
        }

        /// <summary>
        /// 获得运行节点需要的Tokens数目
        /// </summary>
        /// <returns></returns>
        internal int GetTokensRequired()
        {
            int tokensRequired = this.ProcessModel.GetBackwardTransitionListNecessaryCount(Activity.ActivityGUID);
            return tokensRequired;
        }
        #endregion
    }
}
