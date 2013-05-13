using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Common;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 活动节点属性定义
    /// </summary>
    public class ActivityEntity
    {
        public Guid ActivityGUID { get; set; }
        public Guid ProcessGUID { get; set; }
        public NodeTypeEnum NodeType{ get; set; }

        internal bool IsStartNode
        {
            get
            {
                return NodeType == NodeTypeEnum.StartNode;
            }
        }

        internal bool IsEndNode
        {
            get
            {
                return NodeType == NodeTypeEnum.EndNode;
            }
        }

        internal bool IsAutomanticWorkItem
        {
            get
            {
                if ((TaskImplementDetail != null)
                    && ((TaskImplementDetail.ImplementationType | ImplementationTypeEnum.Automantic)
                    == ImplementationTypeEnum.Automantic))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        internal bool IsWorkItemNode
        {
            get
            {
                if ((NodeType | NodeTypeEnum.WorkItemNode) == NodeTypeEnum.WorkItemNode)
                    return true;
                else
                    return false;
            }
        }

        public GatewaySplitJoinTypeEnum GatewaySplitJoinType { get; set; }
        public GatewayDirectionEnum GatewayDirectionType { get; set; }
        public string ActivityName { get; set; }
        public string Description { get; set; }
        internal byte RecordStatusInvalid { get; set; }
        public TaskImplementDetail TaskImplementDetail { get; set; }

        public IList<int> _roles;
        public IList<int> Roles
        {
            get
            {
                if (_roles == null)
                {
                    var processModel = new ProcessModel(ProcessGUID);
                    _roles = processModel.GetActivityRoles(ActivityGUID);
                }
                return _roles;
            }
        }
    }
}
