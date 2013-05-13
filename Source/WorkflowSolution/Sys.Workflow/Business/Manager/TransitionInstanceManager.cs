using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Sys.Workflow.Common;
using Sys.Workflow.DataModel;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 节点转移管理类
    /// </summary>
    internal class TransitionInstanceManager
    {
        #region TransitionInstanceManager 属性和构造函数
        private DataAccessManager _transitionInstanceRepository;
        private DataAccessManager TransitionInstanceRepository
        {
            get
            {
                if (_transitionInstanceRepository == null)
                {
                    _transitionInstanceRepository = DataAccessFactory.Instance();
                }
                return _transitionInstanceRepository;
            }
        }
        #endregion

        internal TransitionInstanceEntity CreateTransitionInstanceObject(ProcessInstanceEntity processInstance,
            TransitionEntity transition,
            ActivityInstanceEntity fromActivityInstance,
            ActivityInstanceEntity toActivityInstance,
            TransitionTypeEnum transitionType,
            WfLogonUser logonUser,
            byte conditionParseResult)
        {
            var entity = new TransitionInstanceEntity();
            entity.AppName = processInstance.AppName;
            entity.AppInstanceID = processInstance.AppInstanceID;
            entity.TransitionInstanceGUID = Guid.NewGuid();
            entity.ProcessGUID = processInstance.ProcessGUID;
            entity.ProcessInstanceGUID = processInstance.ProcessInstanceGUID;
            entity.TransitionGUID = transition.TransitionGUID;
            entity.TransitionType = (byte)transitionType;
            entity.FromActivityGUID = transition.FromActivityGUID;
            entity.FromActivityInstanceGUID = fromActivityInstance.ActivityInstanceGUID;
            entity.FromActivityType = fromActivityInstance.ActivityType;
            entity.FromActivityName = fromActivityInstance.ActivityName;
            entity.ToActivityGUID = transition.ToActivityGUID;
            entity.ToActivityInstanceGUID = toActivityInstance.ActivityInstanceGUID;
            entity.ToActivityType = toActivityInstance.ActivityType;
            entity.ToActivityName = toActivityInstance.ActivityName;
            entity.ConditionParseResult = conditionParseResult;
            entity.CreatedByUserID = logonUser.UserID;
            entity.CreatedByUserName = logonUser.UserName;
            entity.CreatedDateTime = System.DateTime.Now;

            return entity;
        }

        #region 数据增删改查
        internal TransitionInstanceEntity GetById(Guid transitionInstanceGUID)
        {
            return TransitionInstanceRepository.GetById<TransitionInstanceEntity>(transitionInstanceGUID);
        }

        internal TransitionInstanceEntity GetEndTransition(string appName, int appInstanceID, Guid processGUID)
        {
            var nodeList = GetTransitonInstance(appName, appInstanceID, processGUID, NodeTypeEnum.EndNode).ToList();

            if (nodeList.Count > 1)
            {
                throw new WorkflowException(string.Format("没有对应的流程的结束转移数据！结束转移数据条数：{0}", nodeList.Count));
            }

            return nodeList[0];
        }

        internal TransitionInstanceEntity GetLastTaskTransition(string appName, int appInstanceID, Guid processGUID)
        {
            var nodeList = GetTransitonInstance(appName, appInstanceID, processGUID, NodeTypeEnum.TaskNode).ToList();

            if (nodeList.Count == 0)
            {
                throw new WorkflowException("没有符合条件的最后流转任务的实例数据，请查看流程其它信息！");
            }

            return nodeList[0];
        }

        internal IEnumerable<TransitionInstanceEntity> GetTransitonInstance(string appName, int appInstanceID, Guid processGUID, 
            NodeTypeEnum toActivityType)
        {
            var sql = "SELECT * FROM WfTransitionInstance WHERE AppName=@appName AND AppInstanceID=@appInstanceID AND ProcessGUID=@processGUID AND ToActivityType=@toActivityType ORDER BY CreatedDateTime DESC";
            var nodeList = TransitionInstanceRepository.Query<TransitionInstanceEntity>(sql,
                new
                {
                    appName = appName,
                    appInstanceID = appInstanceID,
                    processGUID = processGUID,
                    toActivityType = toActivityType
                });

            return nodeList;
        }

        //private IEnumerable<TransitionInstanceEntity> GetTransitonInstance(string appName, string appInstanceID, Guid processGUID)
        //{
        //    var sql = "SELECT * FROM WfTransitionInstance WHERE AppName=@appName and AppInstanceID=@appInstanceID and ProcessGUID=@processGUID and State=@state";
        //    return ProcessInstanceRepository.Query<ProcessInstanceEntity>(sql,
        //        new
        //        {
        //            appName = appName,
        //            appInstanceID = appInstanceID,
        //            processGUID = processGUID,
        //            state = (short)state
        //        });
        //}

        internal void Insert(TransitionInstanceEntity entity,
            ISession session = null)
        {
            int result = TransitionInstanceRepository.Insert(entity, session.Connection, session.Transaction);
            Debug.WriteLine(string.Format("transition instance inserted, time:{0}", System.DateTime.Now.ToString()));
        }



        /// <summary>
        /// 删除转移实例
        /// </summary>
        /// <param name="transitionInstanceGuid"></param>
        /// <param name="wfLinqDataContext"></param>
        internal void Delete(Guid transitionInstanceGuid,
            ISession session = null)
        {
            TransitionInstanceRepository.Delete<TransitionInstanceEntity>(transitionInstanceGuid, session.Connection, session.Transaction);
        }
        #endregion

        /// <summary>
        /// 判读定义的Transition是否已经被实例化执行
        /// </summary>
        internal bool IsTransiionInstancedAndConditionParsedOK(Guid transitionGUID, 
            IList<TransitionInstanceEntity> transitionInstanceList)
        {
            bool isConainedAndCompletedOK = false;
            foreach (TransitionInstanceEntity transitionInstance in transitionInstanceList)
            {
                //判断连线是否被实例化，并且条件是否满足
                if (transitionGUID == transitionInstance.TransitionGUID)
                {
                    if (transitionInstance.ConditionParseResult == (byte)ConditionParseResultEnum.Passed)
                    {
                        isConainedAndCompletedOK = true;
                        break;
                    }
                }
            }
            return isConainedAndCompletedOK;
        }
    }
}
