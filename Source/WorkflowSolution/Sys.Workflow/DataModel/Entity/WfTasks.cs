using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.DataModel
{
    public partial class WfTasks
    {
        public long TaskID { get; set; }
        public System.Guid ActivityInstanceGUID { get; set; }
        public System.Guid ProcessInstanceGUID { get; set; }
        public short TaskTypeID { get; set; }
        public long AssignedToUserID { get; set; }
        public string AssignedToUserName { get; set; }
        public byte IsTaskCompleted { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public Nullable<System.DateTime> EndedDateTime { get; set; }
        public Nullable<long> EndedByUserID { get; set; }
        public string EndedByUserName { get; set; }
        public Nullable<System.DateTime> LastUpdatedDateTime { get; set; }
        public byte RecordStatusInvalid { get; set; }
        public byte[] RowVersionID { get; set; }

        public virtual WfActivityInstance WfActivityInstance { get; set; }
    }
}
