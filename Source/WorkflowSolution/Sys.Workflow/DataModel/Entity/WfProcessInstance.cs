using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.DataModel
{
    public partial class WfProcessInstance
    {
        public WfProcessInstance()
        {
            this.WfActivityInstance = new HashSet<WfActivityInstance>();
        }

        public System.Guid ProcessInstanceGUID { get; set; }
        public System.Guid ProcessGUID { get; set; }
        public string ProcessName { get; set; }
        public long ApplicationInstanceID { get; set; }
        public short State { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public Nullable<long> CreatedByUserID { get; set; }
        public string CreatedByUserName { get; set; }
        public Nullable<System.DateTime> LastUpdatedDateTime { get; set; }
        public byte IsProcessCompleted { get; set; }
        public Nullable<System.DateTime> EndedDateTime { get; set; }
        public Nullable<long> EndedByUserID { get; set; }
        public string EndedByUserName { get; set; }
        public byte RecordStatusInvalid { get; set; }
        public byte[] RowVersionID { get; set; }

        public virtual ICollection<WfActivityInstance> WfActivityInstance { get; set; }
        public virtual WfProcess WfProcess { get; set; }
    }
}
