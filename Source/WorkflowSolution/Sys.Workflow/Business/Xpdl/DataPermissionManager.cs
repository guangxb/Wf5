using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Utility;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 根据流程XML定义的数据权限管理类
    /// </summary>
    internal class DataPermissionManager : ProcessDefinitionBase
    {
        internal DataPermissionManager(ProcessEntity processEntity)
            : base(processEntity)
        {

        }

        /// <summary>
        /// 获取角色可以编辑的数据项列表
        /// </summary>
        /// <param name="roleCode"></param>
        /// <returns></returns>
        internal IList<string> GetRoleDataItems(string roleCode)
        {
            XmlNode participantNode = XMLHelper.GetXmlNodeByXpath(base.XmlProcessDefinition,
                string.Format("{0}[@code='" + roleCode + "']", XPDLDefinition.StrXmlSingleParticipantPath));
            string participantGUID = XMLHelper.GetXmlAttribute(participantNode, "id");

            XmlNode participantDataItemsNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                string.Format("{0}[@id='" + participantGUID + "']", XPDLDefinition.StrXmlParticipantDataItemPermissions));

            IList<string> itemList = new List<string>();
            foreach (XmlNode dataItemNode in participantDataItemsNode.ChildNodes)
            {
                string dataItemID = XMLHelper.GetXmlAttribute(dataItemNode, "id");
                XmlNode srcDataItemNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                    string.Format("{0}[@id='" + dataItemID + "']", XPDLDefinition.StrXmlSingleDataItems));

                string dataItemCode = XMLHelper.GetXmlAttribute(srcDataItemNode, "code");
                itemList.Add(dataItemCode);
            }
            return itemList;
        }
    }
}
