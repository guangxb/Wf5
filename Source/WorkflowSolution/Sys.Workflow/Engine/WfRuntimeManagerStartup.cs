using System;
using System.Threading;
using System.Data.Linq;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Common;
using Sys.Workflow.DataModel;
using Sys.Workflow.Business;
using Sys.Workflow.Utility;

namespace Sys.Workflow.Engine
{
    /// <summary>
    /// 流程启动运行时
    /// </summary>
    internal class WfRuntimeManagerStartup : WfRuntimeManager
    {
        internal override void ExecuteInstanceImp(ISession session)
        {
            //构造流程实例
            var processInstance = new ProcessInstanceManager()
                .CreateNewProcessInstanceObject(base.AppRunner.AppName, base.AppRunner.AppInstanceID,
                base.ProcessModel.ProcessEntity,
                base.AppRunner.UserID,
                base.AppRunner.UserName);

            //构造活动实例
            //1. 获取开始节点活动
            var startActivity = base.ProcessModel.GetStartActivity();

            var startExecutionContext = ActivityForwardContext.CreateStartupContext(base.ProcessModel,
                processInstance,
                startActivity,
                base.ActivityResource);

            base.ExecuteWorkItemIteraly(startExecutionContext, session);

            //构造回调函数需要的数据
            base.WfExecutedResult = WfExecutedResult.Success();
            base.WfExecutedResult.ProcessInstanceGUID = processInstance.ProcessInstanceGUID;
            base.WfExecutedResult.NextActivityTree = GetNextActivityTree(base.AppRunner.Conditions, session);
        }

        private IList<NodeView> GetNextActivityTree(IDictionary<string, string> conditions, ISession session)
        {
            //根据流程启动人信息，获取默认第一个节点办理任务
            var tm = new TaskManager();
            return tm.GetNextActivityTree(base.AppRunner, conditions, session);
        }

        internal override RuntimeManagerType GetRuntimeManagerType()
        {
            return RuntimeManagerType.StartupRuntime;
        }
    }
}
