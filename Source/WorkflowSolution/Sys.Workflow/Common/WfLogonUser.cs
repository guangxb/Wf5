using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.Common
{
    public class WfLogonUser
    {
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

        internal WfLogonUser(int userID, string userName)
        {
            UserID = userID;
            UserName = userName;
        }
    }
}
