using System;
using System.IO;
using System.Xml;
using System.Data.Linq;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Sys.Workflow.DataModel;
using Sys.Workflow.Business;
using Sys.Workflow.Utility;

namespace Sys.Workflow.Engine
{
    /// <summary>
    /// 状态持久化管理类
    /// </summary>
    public class PersistenceManager
    {
        //internal static string ROOT_NAME = "WorkflowPersistenceData";
        //internal static string NAME_SPACE = "http://www.wissip.com/WorkflowContext";
        ///// <summary>
        ///// 序列化工作流状态
        ///// </summary>
        ///// <param name="processInstanceID">流程实例Id</param>
        ///// <param name="wfContext">流程上下文对象</param>
        ///// <returns>新生成的</returns>
        //internal static long SerializeWorkflowState(long processInstanceID, WorkflowContext wfContext)
        //{
        //    string strObject = SerializationHelper.Serialize(typeof(WorkflowContext), wfContext, ROOT_NAME, NAME_SPACE);

        //    InstanceStateEntity entity = new InstanceStateEntity();
        //    entity.InstanceID = processInstanceID;
        //    entity.State = strObject;

        //    InstanceStateManager ism = new InstanceStateManager();
        //    return ism.Insert(entity);
        //}

        ///// <summary>
        ///// 反序列化工作流状态
        ///// </summary>
        ///// <param name="processInstanceID">流程实例状态</param>
        ///// <returns>工作流状态数据</returns>
        //internal static WorkflowContext DeserializeWorkflowState(long processInstanceID)
        //{
        //    InstanceStateManager ism = new InstanceStateManager();
        //    InstanceStateEntity entity = ism.Select(processInstanceID);
        //    string strObject = entity.State;

        //    object obj = SerializationHelper.Deserialize(typeof(WorkflowContext), strObject, ROOT_NAME, NAME_SPACE);
        //    return (WorkflowContext)obj;
        //}
    }
}
