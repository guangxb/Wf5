using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.DataModel
{
    public partial class WfActivityInstance
    {
        public WfActivityInstance()
        {
            this.WfTasks = new HashSet<WfTasks>();
        }

        public System.Guid ActivityInstanceGUID { get; set; }
        public System.Guid ProcessInstanceGUID { get; set; }
        public long ApplicationInstanceID { get; set; }
        public System.Guid ProcessGUID { get; set; }
        public System.Guid ActivityGUID { get; set; }
        public string ActivityName { get; set; }
        public short ActivityTypeID { get; set; }
        public Nullable<short> GatewayDirectionTypeID { get; set; }
        public Nullable<short> LoopTypeID { get; set; }
        public Nullable<short> SequenceOrder { get; set; }
        public byte IsVirtual { get; set; }
        public byte CanRenewInstance { get; set; }
        public int TokensRequired { get; set; }
        public int TokensHad { get; set; }
        public short State { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public Nullable<System.DateTime> LastUpdatedDateTime { get; set; }
        public byte IsActivityCompleted { get; set; }
        public Nullable<System.DateTime> EndedDateTime { get; set; }
        public Nullable<long> EndedByUserID { get; set; }
        public string EndedByUserName { get; set; }
        public byte RecordStatusInvalid { get; set; }
        public byte[] RowVersionID { get; set; }

        public virtual WfProcessInstance WfProcessInstance { get; set; }
        public virtual ICollection<WfTasks> WfTasks { get; set; }
    }
}
