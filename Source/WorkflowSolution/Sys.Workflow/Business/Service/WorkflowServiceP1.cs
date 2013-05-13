using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.DataModel;
using Sys.Workflow.DataModel.SQL;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 工作流服务（数据查询）
    /// </summary>
    public partial class WorkflowService : IWorkflowService
    {
        #region 获取流程实例、活动实例和转移实例数据的获取
        public ProcessInstanceEntity GetProcessInstance(Guid processInstanceGUID)
        {
            var pim = new ProcessInstanceManager();
            var instance = pim.GetById(processInstanceGUID);
            return instance;
        }

        public ActivityInstanceEntity GetActivityInstance(Guid activityInstanceGUID)
        {
            var aim = new ActivityInstanceManager();
            var instance = aim.GetById(activityInstanceGUID);
            return instance;
        }
        #endregion
    }
}
