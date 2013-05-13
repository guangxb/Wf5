using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.Business
{
    public class CommentEntity
    {
        public long CommentID
        {
            get;
            set;
        }

        public short CommentTypeID
        {
            get;
            set;
        }

        public long ApplicationInstanceID
        {
            get;
            set;
        }

        public string ProcessInstanceGUID
        {
            get;
            set;
        }

        public string ActivityInstanceGUID
        {
            get;
            set;
        }

        public string Activitystring
        {
            get;
            set;
        }

        public string ActivityName
        {
            get;
            set;
        }

        public byte? IsPassed
        {
            get;
            set;
        }

        public string Comment
        {
            get;
            set;
        }

        public DateTime CreatedDateTime
        {
            get;
            set;
        }

        public long CommentedByUserID
        {
            get;
            set;
        }

        public string CommentedByUserName
        {
            get;
            set;
        }

        public DateTime? LastUpdatedDateTime
        {
            get;
            set;
        }
    }
}
