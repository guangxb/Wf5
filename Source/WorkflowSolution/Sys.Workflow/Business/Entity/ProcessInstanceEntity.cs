using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.DataModel;

namespace Sys.Workflow.Business
{
    [DataStorage("WfProcessInstance")]
    public class ProcessInstanceEntity
    {
        public System.Guid ProcessInstanceGUID { get; set; }
        public System.Guid ProcessGUID { get; set; }
        public string ProcessName { get; set; }
        public string AppName { get; set; }
        public int AppInstanceID { get; set; }
        public string AppInstanceCode { get; set; }
        public short ProcessState { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public int CreatedByUserID { get; set; }
        public string CreatedByUserName { get; set; }
        public Nullable<System.DateTime> LastUpdatedDateTime { get; set; }
        public Nullable<int> LastUpdatedByUserID { get; set; }
        public string LastUpdatedByUserName { get; set; }
        public byte IsProcessCompleted { get; set; }
        public Nullable<System.DateTime> EndedDateTime { get; set; }
        public Nullable<int> EndedByUserID { get; set; }
        public string EndedByUserName { get; set; }
        public byte RecordStatusInvalid { get; set; }
    }
}
