using System;
using System.Configuration;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;

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

    /// <summary>
    /// 数据库连接事务的Session对象
    /// </summary>
    public class Session : ISession
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        /// <summary>
        /// 数据库连接对象
        /// </summary>
        public IDbConnection Connection
        {
            get { return _connection; }
        }

        /// <summary>
        /// 数据库事务对象
        /// </summary>
        public IDbTransaction Transaction
        {
            get { return _transaction; }
        }

        public Session(IDbConnection conn)
        {
            _connection = conn;
        }

        /// <summary>
        /// 开启会话
        /// </summary>
        /// <param name="isolation"></param>
        /// <returns></returns>
        public IDbTransaction Begin(IsolationLevel isolation = IsolationLevel.ReadCommitted)
        {
            _transaction = _connection.BeginTransaction(isolation);
            return _transaction;
        }

        /// <summary>
        /// 事务提交
        /// </summary>
        public void Commit()
        {
            _transaction.Commit();
            _transaction = null;
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        public void Rollback()
        {
            _transaction.Rollback();
            _transaction = null;
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public void Dispose()
        {
            if (_connection.State != ConnectionState.Closed)
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                    _transaction = null;
                }
                _connection.Close();
                _connection = null;
            }
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Session 创建类
    /// </summary>
    public class SessionFactory
    {
        public static IDbConnection CreateConnection()
        {
            var strConn = ConfigurationManager.AppSettings["ConnectionString"].ToString();
            IDbConnection conn = new SqlConnection(strConn);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                Debug.WriteLine("session factory create connecton and opened...");
            }

            Debug.WriteLine(string.Format("crate new database connection:{0}", strConn));
            return conn;
        }

        public static ISession CreateSession()
        {
            try
            {
                IDbConnection conn = CreateConnection();
                ISession session = new Session(conn);
                session.Begin();
                return session;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}
