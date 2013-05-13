using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Workflow.DataModel
{
    /// <summary>
    /// 数据连接事务的Session接口
    /// </summary>
    public interface ISession : IDisposable
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }

        IDbTransaction Begin(IsolationLevel isolation = IsolationLevel.ReadCommitted);
        void Commit();
        void Rollback();
    }
}
