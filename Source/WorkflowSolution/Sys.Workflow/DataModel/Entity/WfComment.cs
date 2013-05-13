using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.DataModel
{
    public partial class WfComment
    {
        public long CommentID { get; set; }
        public short CommentTypeID { get; set; }
        public long ApplicationInstanceID { get; set; }
        public System.Guid ProcessInstanceGUID { get; set; }
        public System.Guid ActivityInstanceGUID { get; set; }
        public System.Guid ActivityGUID { get; set; }
        public string ActivityName { get; set; }
        public Nullable<byte> IsPassed { get; set; }
        public string Comment { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public long CommentedByUserID { get; set; }
        public string CommentedByUserName { get; set; }
        public Nullable<System.DateTime> LastUpdatedDateTime { get; set; }
        public byte[] RowVersionID { get; set; }
    }
}
