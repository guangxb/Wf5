using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.DataModel
{
    public partial class WfTransitionInstance
    {
        public System.Guid TransitionInstanceGUID { get; set; }
        public System.Guid TransitionGUID { get; set; }
        public System.Guid ProcessInstanceGUID { get; set; }
        public System.Guid ProcessGUID { get; set; }
        public System.Guid FromActivityInstanceGUID { get; set; }
        public System.Guid FromActivityGUID { get; set; }
        public System.Guid ToActivityInstanceGUID { get; set; }
        public System.Guid ToActivityGUID { get; set; }
        public byte ConditionParseResult { get; set; }
        public byte IsTransitionCompleted { get; set; }
        public byte RecordStatusInvalid { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public byte[] RowVersionID { get; set; }
    }
}
