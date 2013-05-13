using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Sys.Workflow.Common;
using Sys.Workflow.Utility;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 活动节点管理类
    /// </summary>
    internal class ActivityManager : ProcessDefinitionBase
    {
        #region ActivityManager 属性列表、构造函数
        internal ActivityManager(ProcessEntity processEntity)
            : base(processEntity)
        {
            base.ProcessEntity = processEntity;
        }
        #endregion

        #region 获取开始节点信息
        /// <summary>
        /// 获取开始节点信息
        /// </summary>
        /// <returns></returns>
        internal ActivityEntity GetStartActivity()
        {
            string nodeType = "StartNode";

            XmlNode startTypeNode = GetXmlActivityTypeNodeFromXmlFile(nodeType);
            return ConvertXmlActivityNodeToActivityEntity(startTypeNode.ParentNode);
        }

        /// <summary>
        /// 获取流程的第一个节点
        /// </summary>
        /// <returns></returns>
        internal ActivityEntity GetFirstActivity()
        {
            ActivityEntity startActivity = GetStartActivity();
            ActivityEntity firstActivity = GetNextActivity(startActivity.ActivityGUID);
            return firstActivity;
        }

        /// <summary>
        /// 获取当前节点的下一个节点信息
        /// </summary>
        /// <param name="currentActivitystring"></param>
        /// <returns></returns>
        internal ActivityEntity GetNextActivity(Guid activityGUID)
        {
            TransitionManager tm = new TransitionManager(this.ProcessEntity);
            XmlNode transitionNode = tm.GetForwardXmlTransitionNode(activityGUID);

            return GetActivityFromTransitionTo(transitionNode);
        }

        /// <summary>
        /// 获取流程起始的活动节点列表(开始节点之后，可能有多个节点)
        /// </summary>
        /// <param name="conditionKeyValuePair">条件表达式的参数名称-参数值的集合</param>
        /// <returns></returns>
        internal NextActivityMatchedResult GetFirstActivityList(Guid processInstanceGUID,
            ConditionKeyValuePair conditionKeyValuePair)
        {
            ActivityEntity startActivity = GetStartActivity();
            return GetNextActivityList(processInstanceGUID, startActivity, conditionKeyValuePair);
        }
        #endregion

        #region 读取节点的下一步活动列表（无资源限制）

        /// <summary>
        /// 获取下一步节点列表，伴随条件信息
        /// </summary>
        /// <param name="currentActivity"></param>
        /// <param name="conditionKeyValuePair"></param>
        /// <returns></returns>
        internal NextActivityMatchedResult GetNextActivityList(Guid ProcessInstanceGUID,
            ActivityEntity currentActivity,
            ConditionKeyValuePair conditionKeyValuePair)
        {
            NextActivityMatchedResult result = null;
            NextActivityMatchedType resultType = NextActivityMatchedType.Unknown;

            //创建“下一步节点”的根节点
            NextActivityComponent root = NextActivityComponentFactory.CreateNextActivityComponent();
            NextActivityComponent child = null;

            TransitionManager tm = new TransitionManager(this.ProcessEntity);
            List<TransitionEntity> transitionList = tm.GetForwardTransitionList(currentActivity.ActivityGUID,
                conditionKeyValuePair).ToList();

            if (transitionList.Count > 0)
            {
                //遍历连线，获取下一步节点的列表
                foreach (TransitionEntity transition in transitionList)
                {
                    if (transition.ToActivity.NodeType == NodeTypeEnum.EndNode
                        || transition.ToActivity.NodeType == NodeTypeEnum.TaskNode)
                    {
                        child = NextActivityComponentFactory.CreateNextActivityComponent(transition, transition.ToActivity);
                    }
                    else if (transition.ToActivity.NodeType == NodeTypeEnum.GatewayNode)
                    {
                        NextActivityScheduleBase activitySchedule = NextActivityScheduleFactory.CreateActivitySchedule(this.ProcessEntity,
                            transition.ToActivity.GatewaySplitJoinType);

                        child = activitySchedule.GetNextActivityListFromGateway(ProcessInstanceGUID,
                            transition,
                            transition.ToActivity,
                            conditionKeyValuePair,
                            out resultType);
                    }
                    else
                    {
                        throw new XmlDefinitionException(string.Format("未知的节点类型：{0}", transition.ToActivity.NodeType.ToString()));
                    }

                    if (child != null)
                    {
                        root.Add(child);
                        resultType = NextActivityMatchedType.Successed;
                    }
                }
            }
            else
            {
                resultType = NextActivityMatchedType.NoneTransitionFilteredByCondition;
            }
            result = NextActivityMatchedResult.CreateNextActivityMatchedResultObject(resultType, root);
            return result;
        }
        #endregion

        #region 有条件和资源的获取下一步列表
        /// <summary>
        /// 获取下一步节点列表（伴随条件和资源）
        /// </summary>
        /// <param name="currentActivity"></param>
        /// <param name="conditionKeyValuePair"></param>
        /// <returns></returns>
        internal NextActivityMatchedResult GetNextActivityList(Guid processInstanceGUID,
            ActivityEntity currentActivity,
            ConditionKeyValuePair conditionKeyValuePair,
            ActivityResource activityResource,
            Expression<Func<ActivityResource, ActivityEntity, bool>> expression)
        {
            NextActivityComponent newRoot = NextActivityComponentFactory.CreateNextActivityComponent();

            //先获取未加运行时表达式过滤的下一步节点列表
            NextActivityMatchedResult result = GetNextActivityList(processInstanceGUID,
                currentActivity,
                conditionKeyValuePair);

            foreach (NextActivityComponent c in result.Root)
            {
                if (c.HasChildren)
                {
                    NextActivityComponent child = GetNextActivityListByExpressionRecurisivly(c, activityResource, expression);
                    if (child != null)
                    {
                        newRoot.Add(child);
                    }
                }
                else
                {
                    newRoot.Add(c);
                }
            }

            NextActivityMatchedResult newResult = null;
            if (newRoot.HasChildren)
            {
                newResult = NextActivityMatchedResult.CreateNextActivityMatchedResultObject(result.MatchedType, newRoot);
            }
            else
            {
                newResult = NextActivityMatchedResult.CreateNextActivityMatchedResultObject(NextActivityMatchedType.NoneTransitionFilteredByCondition,
                    newRoot);
            }
            return newResult;
        }

        /// <summary>
        /// 递归获取满足条件的下一步节点列表
        /// </summary>
        /// <param name="root"></param>
        /// <param name="activityResource"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        private NextActivityComponent GetNextActivityListByExpressionRecurisivly(NextActivityComponent root,
           ActivityResource activityResource,
           Expression<Func<ActivityResource, ActivityEntity, bool>> expression)
        {
            NextActivityComponent r1 = null;
            foreach (NextActivityComponent c in root)
            {
                if (c.HasChildren)
                {
                    NextActivityComponent child = GetNextActivityListByExpressionRecurisivly(c, activityResource, expression);
                    r1 = AddChildToNewGatewayComponent(r1, c, child);
                }
                else
                {
                    if (expression.Compile().Invoke(activityResource, c.Activity))
                    {
                        r1 = AddChildToNewGatewayComponent(r1, root, c);
                    }
                }
            }
            return r1;
        }

        /// <summary>
        /// 添加子节点到网关节点
        /// </summary>
        /// <param name="newRoot"></param>
        /// <param name="root"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        private NextActivityComponent AddChildToNewGatewayComponent(NextActivityComponent newRoot,
            NextActivityComponent root,
            NextActivityComponent child)
        {
            if ((newRoot == null) && (child != null))
                newRoot = NextActivityComponentFactory.CreateNextActivityComponent(root);

            if ((newRoot != null) && (child != null))
                newRoot.Add(child);
            return newRoot;
        }
        #endregion

        #region Xml活动节点操作
        /// <summary>
        /// 获取XML的节点信息
        /// </summary>
        /// <param name="activityGUID"></param>
        /// <returns></returns>
        private XmlNode GetXmlActivityNodeFromXmlFile(Guid activityGUID)
        {
            XmlNode xmlNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                    string.Format("{0}[@id='" + activityGUID + "']", XPDLDefinition.StrXmlActivityPath));
            return xmlNode;
        }

        /// <summary>
        /// 获取活动节点的类型信息
        /// </summary>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        private XmlNode GetXmlActivityTypeNodeFromXmlFile(string nodeType)
        {
            XmlNode typeNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                string.Format("{0}/ActivityType[@type='" + nodeType + "']", XPDLDefinition.StrXmlActivityPath));
            return typeNode;
        }

        /// <summary>
        /// 获取参与者信息
        /// </summary>
        /// <param name="participantGUID"></param>
        /// <returns></returns>
        private XmlNode GetXmlParticipantNodeFromXmlFile(Guid participantGUID)
        {
            XmlNode participantNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                string.Format("{0}[@id='" + participantGUID + "']", XPDLDefinition.StrXmlSingleParticipantPath));
            return participantNode;
        }

        /// <summary>
        /// 获取当前节点信息
        /// </summary>
        /// <returns></returns>
        internal ActivityEntity GetActivity(Guid activityGUID)
        {
            XmlNode activityNode = GetXmlActivityNodeFromXmlFile(activityGUID);

            ActivityEntity entity = ConvertXmlActivityNodeToActivityEntity(activityNode);
            return entity;
        }

        /// <summary>
        /// 获取转移上的To节点的对象
        /// </summary>
        /// <param name="transitionNode">转移的xml节点</param>
        /// <returns></returns>
        private ActivityEntity GetActivityFromTransitionTo(XmlNode transitionNode)
        {
            string nextActivityGuid = XMLHelper.GetXmlAttribute(transitionNode, "to");
            XmlNode activityNode = GetXmlActivityNodeFromXmlFile(Guid.Parse(nextActivityGuid));

            ActivityEntity entity = ConvertXmlActivityNodeToActivityEntity(activityNode);
            return entity;
        }
        #endregion
        
        #region 获取节点上的角色信息
        /// <summary>
        /// 获取角色编码信息
        /// </summary>
        /// <param name="performerGUID"></param>
        /// <returns></returns>
        private int GetRoleIdFromXmlFile(Guid performerGUID)
        {
            XmlNode performerNode = GetXmlParticipantNodeFromXmlFile(performerGUID);
            var roleId = XMLHelper.GetXmlAttribute(performerNode, "outerId");
            return int.Parse(roleId);
        }

        /// <summary>
        /// 获取节点上定义的角色code集合
        /// </summary>
        /// <param name="activityGUID"></param>
        /// <returns></returns>
        internal IList<int> GetActivityRoles(Guid activityGUID)
        {
            IList<int> roles = new List<int>();
            XmlNode xmlNode = GetXmlActivityNodeFromXmlFile(activityGUID);
            XmlNode performersNode = xmlNode.SelectSingleNode("Performers");
            
            if (performersNode != null)
            {
                foreach (XmlNode performer in performersNode.ChildNodes)
                {
                    string performerGUID = XMLHelper.GetXmlAttribute(performer, "id");
                    roles.Add(GetRoleIdFromXmlFile(Guid.Parse(performerGUID)));
                }
            }
            return roles;
        }
        #endregion

        #region
        /// <summary>
        /// 获取当前节点所要求的数据项，即数据项必须填写，才可以触发后续流程
        /// </summary>
        /// <param name="activityGUID"></param>
        /// <returns></returns>
        internal IList<string> GetActivityDataItemsRequired(Guid activityGUID)
        {
            XmlNode requiredNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
               string.Format("{0}/DataItemsRequired", XPDLDefinition.StrXmlActivityPath));

            IList<string> itemList = new List<string>();
            foreach (XmlNode dataItemNode in requiredNode.ChildNodes)
            {
                string dataItemID = XMLHelper.GetXmlAttribute(dataItemNode, "id");
                XmlNode srcDataItemNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                    string.Format("{0}[@id='" + dataItemID + "']", XPDLDefinition.StrXmlSingleDataItems));

                string dataItemCode = XMLHelper.GetXmlAttribute(srcDataItemNode, "code");
                itemList.Add(dataItemCode);
            }
            return itemList;
        }
        #endregion

        #region Xml节点转换信息
        /// <summary>
        /// 把XML节点转换为ActivityEntity实体对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal ActivityEntity ConvertXmlActivityNodeToActivityEntity(XmlNode node)
        {
            ActivityEntity entity = new ActivityEntity();
            entity.ActivityName = XMLHelper.GetXmlAttribute(node, "name");
            entity.ActivityGUID = Guid.Parse(XMLHelper.GetXmlAttribute(node, "id"));
            entity.ProcessGUID = ProcessEntity.ProcessGUID;

            //节点类型信息
            XmlNode typeNode = node.SelectSingleNode("ActivityType");
            entity.NodeType = (NodeTypeEnum)Enum.Parse(typeof(NodeTypeEnum), XMLHelper.GetXmlAttribute(typeNode, "type"));
            
            //任务完成类型信息
            XmlNode implementNode = node.SelectSingleNode("Implement");
            if (implementNode != null)
            {
                entity.TaskImplementDetail = new TaskImplementDetail();
                entity.TaskImplementDetail.ImplementationType = (ImplementationTypeEnum)Enum.Parse(typeof(ImplementationTypeEnum), XMLHelper.GetXmlAttribute(implementNode, "type"));

                //完成类型的详细信息
                XmlNode contentNode = implementNode.SelectSingleNode("Content");
                if (contentNode != null)
                {
                    entity.TaskImplementDetail.Assembly = XMLHelper.GetXmlAttribute(contentNode, "assembly");
                    entity.TaskImplementDetail.Interface = XMLHelper.GetXmlAttribute(contentNode, "interface");
                    entity.TaskImplementDetail.Method = XMLHelper.GetXmlAttribute(contentNode, "method");
                    entity.TaskImplementDetail.Content = contentNode.InnerText;
                }
            }

            //节点的Split Join 类型
            string gatewaySplitJoinType = XMLHelper.GetXmlAttribute(typeNode, "gatewaySplitJoinType");
            if (!string.IsNullOrEmpty(gatewaySplitJoinType))
            {
                entity.GatewaySplitJoinType = (GatewaySplitJoinTypeEnum)Enum.Parse(typeof(GatewaySplitJoinTypeEnum), gatewaySplitJoinType);
            }

            string gatewayDirection = XMLHelper.GetXmlAttribute(typeNode, "gatewayDirection");
            //节点的路由信息
            if (!string.IsNullOrEmpty(gatewayDirection))
            {
                entity.GatewayDirectionType = (GatewayDirectionEnum)Enum.Parse(typeof(GatewayDirectionEnum), gatewayDirection);
            }

            return entity;
        }
        #endregion
    }
}
