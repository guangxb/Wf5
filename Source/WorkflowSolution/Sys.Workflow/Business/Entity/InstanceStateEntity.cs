using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.Business
{
    public class InstanceStateEntity
    {
        public long PersistenceID
        {
            get;
            set;
        }

        public long InstanceID
        {
            get;
            set;
        }

        public string State
        {
            get;
            set;
        }
    }
}
