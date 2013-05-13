using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Common;

namespace Sys.Workflow.Engine
{
    /// <summary>
    /// 流程启动人对象
    /// </summary>
    public class WfAppInitiator
    {
        public string AppName { get; set; }
        public int AppInstanceID { get; set; }
        public string Flowstatus { get; set; }//工作流状态
        public Guid ProcessGUID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
}
