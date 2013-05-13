using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Utility;
using Sys.Workflow.DataModel;
using Sys.Workflow.Business;
using Sys.Workflow.Common;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 流程定义维护类的基类
    /// </summary>
    public abstract class ProcessDefinitionBase
    {
        #region 属性和构造函数
        internal ProcessEntity ProcessEntity
        {
            get;
            set;
        }

        internal XmlDocument _xmlProcessDefinition;
        internal XmlDocument XmlProcessDefinition
        {
            get
            {
                if (_xmlProcessDefinition == null)
                {
                    _xmlProcessDefinition = GetXmlProcessDefinitionFromDisk();
                }
                return _xmlProcessDefinition;
            }
        }

        internal XmlNode _xmlParticipants;
        internal XmlNode XmlParticipants
        {
            get
            {
                if (_xmlParticipants == null)
                {
                    _xmlParticipants = GetXmlParticipants();
                }
                return _xmlParticipants;
            }
        }

        internal XmlNode _xmlDataItems;
        internal XmlNode XmlDataItems
        {
            get
            {
                if (_xmlDataItems == null)
                {
                    _xmlDataItems = GetXmlDataItems();
                }
                return _xmlDataItems;
            }
        }

        internal ProcessDefinitionBase(ProcessEntity processEntity)
        {
            ProcessEntity = processEntity;
        }
        #endregion

        #region 获取流程信息
        /// <summary>
        /// 读取流程的配置文件
        /// </summary>
        /// <param name="processGUID"></param>
        /// <returns></returns>
        private XmlDocument GetXmlProcessDefinitionFromDisk()
        {
            string filePath = ProcessEntity.XmlFilePath;
            string fileName = ProcessEntity.XmlFileName;
            string serverPath = ConfigHelper.GetAppSettingString("WorkflowFileServer");
            string physicalFileName = serverPath + "\\" + filePath + "\\" + fileName;
            
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(physicalFileName);

            return xmlDoc;
        }

        /// <summary>
        /// 获取流程参与者信息
        /// </summary>
        /// <returns></returns>
        private XmlNode GetXmlParticipants()
        {
            return XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition, XPDLDefinition.StrXmlParticipantsPath);
        }

        /// <summary>
        /// 获取流程的数据集合信息
        /// </summary>
        /// <returns></returns>
        private XmlNode GetXmlDataItems()
        {
            return XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition, XPDLDefinition.StrXmlDataItems);
        }
        #endregion

    }
}
