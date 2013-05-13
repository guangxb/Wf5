using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Common;
using Sys.Workflow.Utility;
using Sys.Workflow.DataModel;
using Sys.Workflow.Business;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 任务视图类
    /// </summary>
    [DataStorage("vwWfActivityInstanceTasks")]
    public class TaskViewEntity
    {
        public long TaskID { get; set; }
        public string AppName { get; set; }
        public int AppInstanceID { get; set; }
        public System.Guid ProcessGUID { get; set; }
        public System.Guid ProcessInstanceGUID { get; set; }
        public System.Guid ActivityGUID { get; set; }
        public System.Guid ActivityInstanceGUID { get; set; }
        public string ActivityName { get; set; }
        public short ActivityType { get; set; }
        public short TaskType { get; set; }
        public int AssignedToUserID { get; set; }
        public string AssignedToUserName { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public Nullable<System.DateTime> EndedDateTime { get; set; }
        public Nullable<int> EndedByUserID { get; set; }
        public string EndedByUserName { get; set; }
        public short TaskState { get; set; }
        public short ActivityState { get; set; }
        public byte IsActivityCompleted { get; set; }
        public byte RecordStatusInvalid { get; set; }
        public short ProcessState { get; set; }
    }
}
