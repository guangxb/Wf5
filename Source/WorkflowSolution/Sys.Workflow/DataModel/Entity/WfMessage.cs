using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.DataModel
{
    public partial class WfMessage
    {
        public long MessageID { get; set; }
        public string Title { get; set; }
        public string MessageContent { get; set; }
        public long ApplicationInstanceID { get; set; }
        public long ProcessInstanceID { get; set; }
        public long ActivityInstanceID { get; set; }
        public Nullable<long> SendToUserID { get; set; }
        public string SendToUserName { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public Nullable<bool> IsRead { get; set; }
        public Nullable<System.DateTime> ReadDateTime { get; set; }
        public Nullable<bool> IsComplete { get; set; }
        public Nullable<System.DateTime> CompletedDateTime { get; set; }
        public Nullable<bool> IsValid { get; set; }
        public byte[] RowVersionID { get; set; }
    }
}
