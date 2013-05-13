using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Common;
using Sys.Workflow.Engine;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 活动上的资源类
    /// </summary>
    public class ActivityResource
    {
        #region 属性、构造函数
        /// <summary>
        /// 当前登录用户信息
        /// </summary>
        public WfLogonUser LogonUser
        {
            get;
            set;
        }

        /// <summary>
        /// 带有执行人员信息的下一步节点列表
        /// </summary>
        public IDictionary<Guid, PerformerList> NextActivityPerformers
        {
            get;
            set;
        }

        public IDictionary<string, string> ConditionKeyValuePair
        {
            get;
            set;
        }

        public object[] UserParameters
        {
            get;
            set;
        }

        internal ActivityResource(int userID,
            string userName,
            IDictionary<Guid, PerformerList> nextActivityPerformers,
            IDictionary<string, string> conditionKeyValuePair = null)
        {
            LogonUser = new WfLogonUser(userID, userName);
            NextActivityPerformers = nextActivityPerformers;
            ConditionKeyValuePair = conditionKeyValuePair;
        }

        internal static IDictionary<Guid, PerformerList> CreateNextActivityPerformers(Guid activityGUID,
            int userID,
            string userName)
        {
            var performerList = new PerformerList();
            performerList.Add(new Performer(userID, userName));
            IDictionary<Guid, PerformerList> nextActivityPerformers = new Dictionary<Guid, PerformerList>();
            nextActivityPerformers.Add(activityGUID, performerList);

            return nextActivityPerformers;
        }
        #endregion
    }
}
