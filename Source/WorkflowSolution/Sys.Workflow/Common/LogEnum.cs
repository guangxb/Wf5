using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.Common
{
    /// <summary>
    /// 日志事件类型
    /// </summary>
    public enum LogEventType
    {
        Warnning = 0,
        Error = 1,
    }

    /// <summary>
    /// 日志优先级
    /// </summary>
    public enum LogPriority
    {
        Emergency = 0,
        High = 1,
        Normal = 2,
        Low = 3
    }
}
