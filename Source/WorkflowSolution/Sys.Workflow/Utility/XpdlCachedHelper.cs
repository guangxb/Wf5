using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Xml;

namespace Sys.Workflow.Utility
{
    /// <summary>
    /// 流程定义文件Cache
    /// </summary>
    internal class XpdlCachedHelper
    {
        private static readonly ConcurrentDictionary<Guid, XmlDocument> _xpdlCache = new ConcurrentDictionary<Guid, XmlDocument>();

        internal static XmlDocument SetXpdl(Guid processGUID, XmlDocument xmlDoc)
        {
            return _xpdlCache.GetOrAdd(processGUID, xmlDoc);
        }

        internal static XmlDocument GetXpdl(Guid processGUID)
        {
            XmlDocument xmlDoc = null;
            if (_xpdlCache.ContainsKey(processGUID))
            {
                xmlDoc = _xpdlCache[processGUID];
            }
            return xmlDoc;
        }
    }
}
