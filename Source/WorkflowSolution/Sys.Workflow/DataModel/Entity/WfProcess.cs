using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.DataModel
{
    public partial class WfProcess
    {
        public WfProcess()
        {
            this.WfProcessInstance = new HashSet<WfProcessInstance>();
        }

        public System.Guid ProcessGUID { get; set; }
        public string ProcessName { get; set; }
        public short AppCategoryID { get; set; }
        public string PageUrl { get; set; }
        public string XmlFileName { get; set; }
        public string XmlFilePath { get; set; }
        public string Description { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public Nullable<System.DateTime> LastUpdatedDateTime { get; set; }
        public byte[] RowVersionID { get; set; }

        public virtual ICollection<WfProcessInstance> WfProcessInstance { get; set; }
    }
}
