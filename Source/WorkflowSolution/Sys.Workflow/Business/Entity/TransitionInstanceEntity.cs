using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.DataModel;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 节点转移类
    /// </summary>
    [DataStorage("WfTransitionInstance")]
    internal class TransitionInstanceEntity
    {
        public System.Guid TransitionInstanceGUID { get; set; }
        public System.Guid TransitionGUID { get; set; }
        public string AppName { get; set; }
        public int AppInstanceID { get; set; }
        public System.Guid ProcessInstanceGUID { get; set; }
        public System.Guid ProcessGUID { get; set; }
        public byte TransitionType { get; set; }
        public System.Guid FromActivityInstanceGUID { get; set; }
        public System.Guid FromActivityGUID { get; set; }
        public short FromActivityType { get; set; }
        public string FromActivityName { get; set; }
        public System.Guid ToActivityInstanceGUID { get; set; }
        public System.Guid ToActivityGUID { get; set; }
        public short ToActivityType { get; set; }
        public string ToActivityName { get; set; }
        public byte ConditionParseResult { get; set; }
        public byte IsTransitionCompleted { get; set; }
        public int CreatedByUserID { get; set; }
        public string CreatedByUserName { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public Nullable<int> LastUpdatedByUserID { get; set; }
        public string LastUpdatedByUserName { get; set; }
        public Nullable<System.DateTime> LastUpdatedDateTime { get; set; }
        public byte RecordStatusInvalid { get; set; }
    }
}
