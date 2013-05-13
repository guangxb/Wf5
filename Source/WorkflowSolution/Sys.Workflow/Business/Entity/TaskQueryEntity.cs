using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 任务查询实体对象
    /// </summary>
    public class TaskQueryEntity : QueryBase
    {
        public string AppName { get; set; }
        public int AppInstanceID { get; set; }
        public Guid ProcessGUID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }

    }
}
