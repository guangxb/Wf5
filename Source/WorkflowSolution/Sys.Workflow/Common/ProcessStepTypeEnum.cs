using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.Common
{
    public enum ProcessStepTypeEnum
    {
        /// <summary>
        /// 启动流程
        /// </summary>
        Startup = 1,

        /// <summary>
        /// 继续运行流程
        /// </summary>
        Continue = 2
    }
}
