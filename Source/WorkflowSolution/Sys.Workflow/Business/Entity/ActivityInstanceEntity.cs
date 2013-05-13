using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Common;
using Sys.Workflow.DataModel;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 活动实例的实体对象
    /// </summary>
    [DataStorage("WfActivityInstance")]
    public class ActivityInstanceEntity
    {
        public System.Guid ActivityInstanceGUID { get; set; }
        public System.Guid ProcessInstanceGUID { get; set; }
        public string AppName { get; set; }
        public int AppInstanceID { get; set; }
        public System.Guid ProcessGUID { get; set; }
        public System.Guid ActivityGUID { get; set; }
        public string ActivityName { get; set; }
        public short ActivityType { get; set; }
        public Nullable<short> GatewayDirectionTypeID { get; set; }
        public Nullable<short> LoopTypeID { get; set; }
        public Nullable<short> SequenceOrder { get; set; }
        public byte IsVirtual { get; set; }
        public byte CanRenewInstance { get; set; }
        public int TokensRequired { get; set; }
        public int TokensHad { get; set; }
        public short State { get; set; }
        public int CreatedByUserID { get; set; }
        public string CreatedByUserName { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public Nullable<int> LastUpdatedByUserID { get; set; }
        public string LastUpdatedByUserName { get; set; }
        public Nullable<System.DateTime> LastUpdatedDateTime { get; set; }
        public byte IsActivityCompleted { get; set; }
        public Nullable<System.DateTime> EndedDateTime { get; set; }
        public Nullable<int> EndedByUserID { get; set; }
        public string EndedByUserName { get; set; }
        public byte RecordStatusInvalid { get; set; }
        
    }
}
