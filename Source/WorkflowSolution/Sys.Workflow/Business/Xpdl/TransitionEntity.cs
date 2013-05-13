using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Common;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 转移定义
    /// </summary>
    public class TransitionEntity
    {
        public string Name
        {
            get;
            set;
        }

        public Guid TransitionGUID
        {
            get;
            set;
        }

        public Guid FromActivityGUID
        {
            get;
            set;
        }

        public Guid ToActivityGUID
        {
            get;
            set;
        }

        public TransitionDirectionTypeEnum DirectionType
        {
            get;
            set;
        }

        public ConditionEntity Condition
        {
            get;
            set;
        }

        public GroupBehaviourEntity GroupBehaviour
        {
            get;
            set;
        }

        public ActivityEntity FromActivity
        {
            get;
            set;
        }

        public ActivityEntity ToActivity
        {
            get;
            set;
        }
    }

    public class TransitonList : List<TransitionEntity>
    {

    }
}
