using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.Common
{
    /// <summary>
    /// 任务的执行者对象
    /// </summary>
    public class Performer
    {
        public Performer(int userID, string userName)
        {
            UserID = userID;
            UserName = userName;
        }

        public int UserID
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }
    }

    public class PerformerList : List<Performer>
    {
        public PerformerList()
        {
        }
    }
}
