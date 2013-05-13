using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Sys.Workflow.Common;
using Sys.Workflow.Utility;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 连线定义个管理类
    /// </summary>
    internal class TransitionManager : ProcessDefinitionBase
    {
        internal TransitionManager(ProcessEntity processEntity)
            : base(processEntity)
        {
        }

        /// <summary>
        /// 获取活动转移的To节点信息
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <returns></returns>
        internal XmlNode GetForwardXmlTransitionNode(Guid fromActivityGUID)
        {
            XmlNode transitionNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                string.Format("{0}[@from='" + fromActivityGUID.ToString() + "']", XPDLDefinition.StrXmlTransitionPath));

            return transitionNode;
        }

        /// <summary>
        /// 获取活动转移的To节点列表
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <returns></returns>
        internal XmlNodeList GetForwardXmlTransitionNodeList(Guid fromActivityGUID)
        {
            XmlNodeList transitionNodeList = XMLHelper.GetXmlNodeListByXpath(XmlProcessDefinition,
                string.Format("{0}[@from='" + fromActivityGUID.ToString() + "']", XPDLDefinition.StrXmlTransitionPath));

            return transitionNodeList;
        }

        /// <summary>
        /// 获取节点的后续连线
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <returns></returns>
        internal TransitionEntity GetForwardTransition(Guid fromActivityGUID)
        {
            XmlNode xmlTransitionNode = GetForwardXmlTransitionNode(fromActivityGUID);
            TransitionEntity transtion = ConvertXmlTransitionNodeToTransitionEntity(xmlTransitionNode);

            return transtion;
        }

        /// <summary>
        /// 获取当前节点的后续连线的集合
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <returns></returns>
        internal IList<TransitionEntity> GetForwardTransitionList(Guid fromActivityGUID)
        {
            IList<TransitionEntity> transitionList = new List<TransitionEntity>();
            XmlNodeList transitionNodeList = GetForwardXmlTransitionNodeList(fromActivityGUID);
            foreach (XmlNode transitionNode in transitionNodeList)
            {
                TransitionEntity entity = ConvertXmlTransitionNodeToTransitionEntity(transitionNode);
                transitionList.Add(entity);
            }
            return transitionList;
        }

        /// <summary>
        /// 获取当前节点的后续连线的集合（使用条件过滤）
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <param name="conditionKeyValuePair"></param>
        /// <returns></returns>
        internal IList<TransitionEntity> GetForwardTransitionList(Guid fromActivityGUID,
            ConditionKeyValuePair conditionKeyValuePair)
        {
            IList<TransitionEntity> transitionList = new List<TransitionEntity>();
            XmlNodeList transitionNodeList = GetForwardXmlTransitionNodeList(fromActivityGUID);
            foreach (XmlNode transitionNode in transitionNodeList)
            {
                TransitionEntity entity = ConvertXmlTransitionNodeToTransitionEntity(transitionNode);
                bool isValidTranstion = IsValidTransition(entity, conditionKeyValuePair);
                if (isValidTranstion)
                {
                    transitionList.Add(entity);
                }
            }
            return transitionList;
        }

        /// <summary>
        /// XOrSplit类型下的连线列表
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <param name="conditionKeyValuePair"></param>
        /// <returns></returns>
        internal IList<TransitionEntity> GetForwardTransitionListWithConditionXOrSplit(Guid fromActivityGUID,
            ConditionKeyValuePair conditionKeyValuePair)
        {
            IList<TransitionEntity> transitionList = new List<TransitionEntity>();
            XmlNodeList transitionNodeList = GetForwardXmlTransitionNodeList(fromActivityGUID);
            foreach (XmlNode transitionNode in transitionNodeList)
            {
                TransitionEntity entity = ConvertXmlTransitionNodeToTransitionEntity(transitionNode);
                bool isValidTranstion = IsValidTransition(entity, conditionKeyValuePair);
                if (isValidTranstion)
                {
                    transitionList.Add(entity);
                    break;
                }
            }
            return transitionList;
        }


        /// <summary>
        /// 获取活动转移的节点信息
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <returns></returns>
        private XmlNode GetForwardXmlTransitionNode(Guid fromActivityGUID,
            string toActivityGUID)
        {
            XmlNode transitionNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                string.Format("{0}[@from='" + fromActivityGUID + " and to='" + toActivityGUID + "']", XPDLDefinition.StrXmlTransitionPath));
            return transitionNode;
        }

        /// <summary>
        /// 获取前驱节点的列表
        /// </summary>
        /// <param name="toActivityGUID"></param>
        /// <returns></returns>
        internal XmlNodeList GetXmlBackwardTransitonNodeList(Guid toActivityGUID)
        {
            XmlNodeList transtionNodeList = XMLHelper.GetXmlNodeListByXpath(XmlProcessDefinition,
                string.Format("{0}[@to='" + toActivityGUID + "']", XPDLDefinition.StrXmlTransitionPath));
            return transtionNodeList;
        }

        /// <summary>
        /// 获取节点的前驱连线
        /// </summary>
        /// <param name="toActivityGUID"></param>
        /// <returns></returns>
        internal IList<TransitionEntity> GetBackwardTransitionList(Guid toActivityGUID)
        {
            XmlNodeList transitionNodeList = GetXmlBackwardTransitonNodeList(toActivityGUID);
            IList<TransitionEntity> transitionList = new List<TransitionEntity>();
            foreach (XmlNode transitionNode in transitionNodeList)
            {
                TransitionEntity transition = ConvertXmlTransitionNodeToTransitionEntity(transitionNode);
                transitionList.Add(transition);
            }
            return transitionList;
        }

        /// <summary>
        /// 获取节点的前驱节点列表(Lambda表达式)
        /// </summary>
        /// <param name="activityGUID"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal IList<TransitionEntity> GetBackwardTransitionList(Guid activityGUID,
            Expression<Func<TransitionEntity, bool>> expression)
        {
            IList<TransitionEntity> transitionList = GetBackwardTransitionList(activityGUID);
            return GetBackwardTransitionList(activityGUID, expression);
        }

        /// <summary>
        /// 获取节点的前驱节点列表(Lambda表达式)
        /// </summary>
        /// <param name="activityGUID"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal IList<TransitionEntity> GetBackwardTransitionList(IList<TransitionEntity> transitionList,
            Expression<Func<TransitionEntity, bool>> expression)
        {
            IList<TransitionEntity> newTransitionList = new List<TransitionEntity>();
            foreach (TransitionEntity transition in transitionList)
            {
                if (expression.Compile().Invoke(transition))
                {
                    newTransitionList.Add(transition);
                }
            }
            return newTransitionList;
        }

        /// <summary>
        /// 根据流程定义文件，获取带有条件的节点前驱连线列表，（带有条件，可以用Lambda表达式重构）
        /// </summary>
        /// <param name="toActivityGUID"></param>
        /// <returns></returns>
        internal IList<TransitionEntity> GetBackworkTransitionListWithCondition(Guid toActivityGUID)
        {
            return GetBackwardTransitionList(toActivityGUID, 
                (t => t.Condition != null && !string.IsNullOrEmpty(t.Condition.ConditionText)));
        }

        /// <summary>
        /// 获取并行连线的，类型为必需类型
        /// </summary>
        /// <param name="toActivityGUID"></param>
        /// <returns></returns>
        internal IList<TransitionEntity> GetBackwardTransitionListNecessary(IList<TransitionEntity> transitionList)
        {
            return GetBackwardTransitionList(transitionList,
                (t => (t.GroupBehaviour.ParallelOption !=null) && (t.GroupBehaviour.ParallelOption == ParallelOptionEnum.Necessary)));
        }

        /// <summary>
        /// 获取节点前驱连线上必须的Token数目
        /// </summary>
        /// <param name="toActivityGUID"></param>
        /// <returns></returns>
        internal int GetBackwardTransitionListNecessaryCount(Guid toActivityGUID)
        {
            IList<TransitionEntity> backwardList = GetBackwardTransitionList(toActivityGUID);
            IList<TransitionEntity> necBackwardList = GetBackwardTransitionListNecessary(backwardList);
            return necBackwardList.Count;
        }

        #region 解析条件表达式
        /// <summary>
        /// 用LINQ解析条件表达式
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="conditionKeyValuePair"></param>
        /// <returns></returns>
        private bool ParseCondition(TransitionEntity transition, ConditionKeyValuePair conditionKeyValuePair)
        {
            string expression = transition.Condition.ConditionText;
            string expressionReplaced = ReplaceParameterToValue(expression, conditionKeyValuePair);

            //Expression e = System.Linq.Dynamic.DynamicExpression.Parse(typeof(Boolean), expressionReplaced);
            Expression e = null;
            LambdaExpression LE = Expression.Lambda(e);
            Func<bool> testMe = (Func<bool>)LE.Compile();
            bool result = testMe();

            return result;
        }

        /// <summary>
        /// 是否是满足条件的Transition，如果条件为空，默认是有效的。
        /// </summary>
        /// <param name="forwardTransition"></param>
        /// <param name="conditionKeyValuePair"></param>
        /// <returns></returns>
        internal bool IsValidTransition(TransitionEntity transition,
           ConditionKeyValuePair conditionKeyValuePair)
        {
            bool isValid = false;

            if (transition.Condition != null && !string.IsNullOrEmpty(transition.Condition.ConditionText))
            {
                if (conditionKeyValuePair != null)
                {
                    isValid = ParseCondition(transition, conditionKeyValuePair);
                }
            }
            else
            {
                //流程节点上定义的条件为空，则认为连线是可到达的
                isValid = true;
            }
            return isValid;
        }

        /// <summary>
        /// 判断整个连线集合，是否满足条件
        /// </summary>
        /// <param name="transitionList"></param>
        /// <param name="conditionKeyValuePair"></param>
        /// <returns></returns>
        internal bool CheckAndSplitOccurrenceCondition(List<TransitionEntity> transitionList,
            ConditionKeyValuePair conditionKeyValuePair)
        {
            bool isValidAll = true;
            foreach (TransitionEntity transition in transitionList)
            {
                //只有是必需验证的条件，采取检查
                if (transition.GroupBehaviour.ParallelOption == ParallelOptionEnum.Necessary)
                {
                    bool isVailid = IsValidTransition(transition, conditionKeyValuePair);
                    if (!isVailid)
                    {
                        isValidAll = false;
                        break;
                    }
                }
            }
            return isValidAll;
        }

        /// <summary>
        /// 取代条件表达式中的参数值
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="dictoinary"></param>
        /// <returns></returns>
        internal string ReplaceParameterToValue(string expression, Dictionary<string, string> dictoinary)
        {
            foreach (KeyValuePair<string, string> p in dictoinary)
            {
                if (!ExpressionParser.IsNumeric(p.Value))
                {
                    //字符串类型的变量处理，加上双引号。
                    string s = "\"" + p.Value + "\"";
                    expression = expression.Replace(p.Key, s);
                }
                else
                {
                    expression = expression.Replace(p.Key, p.Value);
                }
            }
            return expression;
        }
        #endregion

        #region Xml节点转换信息
        /// <summary>
        /// 把XML节点转换为ActivityEntity实体对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal TransitionEntity ConvertXmlTransitionNodeToTransitionEntity(XmlNode node)
        {
            //构造转移的基本属性
            TransitionEntity entity = new TransitionEntity();
            entity.Name = XMLHelper.GetXmlAttribute(node, "name");
            entity.TransitionGUID = Guid.Parse(XMLHelper.GetXmlAttribute(node, "id"));
            entity.FromActivityGUID = Guid.Parse(XMLHelper.GetXmlAttribute(node, "from"));
            entity.ToActivityGUID = Guid.Parse(XMLHelper.GetXmlAttribute(node, "to"));

            //构造活动节点的实体对象
            ActivityManager am = new ActivityManager(this.ProcessEntity);
            entity.FromActivity = am.GetActivity(entity.FromActivityGUID);
            entity.ToActivity = am.GetActivity(entity.ToActivityGUID);
   
            //构造转移的条件节点
            XmlNode conditionNode = node.SelectSingleNode("Condition");
            if (conditionNode != null)
            {
                entity.Condition = new ConditionEntity();
                if (!string.IsNullOrEmpty(XMLHelper.GetXmlAttribute(conditionNode, "type")))
                {
                    entity.Condition.ConditionType = (ConditionTypeEnum)Enum.Parse(typeof(ConditionTypeEnum),
                        XMLHelper.GetXmlAttribute(conditionNode, "type"));
                }

                if ((conditionNode.SelectSingleNode("ConditionText") != null)
                    && !string.IsNullOrEmpty(XMLHelper.GetXmlNodeValue(conditionNode, "ConditionText")))
                {
                    entity.Condition.ConditionText = XMLHelper.GetXmlNodeValue(conditionNode, "ConditionText");
                }
            }

            //构造转移的行为节点
            XmlNode groupBehaviourNode = node.SelectSingleNode("GroupBehaviour");
            if (groupBehaviourNode != null)
            {
                entity.GroupBehaviour = new GroupBehaviourEntity();
                if (!string.IsNullOrEmpty(XMLHelper.GetXmlAttribute(groupBehaviourNode, "priority")))
                {
                    entity.GroupBehaviour.Priority = short.Parse(XMLHelper.GetXmlAttribute(groupBehaviourNode, "priority"));
                }

                if (!string.IsNullOrEmpty(XMLHelper.GetXmlAttribute(groupBehaviourNode, "parallelOption")))
                {
                    entity.GroupBehaviour.ParallelOption = (ParallelOptionEnum)Enum.Parse(typeof(ParallelOptionEnum),
                        XMLHelper.GetXmlAttribute(groupBehaviourNode, "parallelOption"));
                }
            }
            return entity;
        }
        #endregion
    }
}
