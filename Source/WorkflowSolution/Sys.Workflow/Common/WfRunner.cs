using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.Common
{
    /// <summary>
    /// 流程执行人(待办任务的执行者)
    /// </summary>
    public class WfTaskRunner
    {
        public long TaskID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public IDictionary<Guid, PerformerList> NextActivityPerformers { get; set; }
    }

    /// <summary>
    /// 流程执行人(业务应用的办理者)
    /// </summary>
    public class WfAppRunner
    {
        public string AppName { get; set; }
        public int AppInstanceID { get; set; }
        public Guid ProcessGUID { get; set; }
        public string FlowStatus { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public IDictionary<Guid, PerformerList> NextActivityPerformers { get; set; }
        public IDictionary<string, string> Conditions { get; set; }
    }
}
