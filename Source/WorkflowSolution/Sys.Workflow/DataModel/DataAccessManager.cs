using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Sys.Workflow.DataModel.SQL;

namespace Sys.Workflow.DataModel
{
    /// <summary>
    /// 数据访问管理类
    /// </summary>
    public class DataAccessManager
    {
        private MapExtension _mapExtension;
        private SqlGenerator _sqlGenerator;

        public DataAccessManager()
        {
            _sqlGenerator = new SqlGenerator();
            _mapExtension = new MapExtension(typeof(AutoEntityMapper<>), _sqlGenerator);
            
        }

        public SqlGenerator GetSqlGenerator()
        {
            if (_sqlGenerator == null)
            {
                throw new ApplicationException("SQL 生成器为空！");
            }

            return _sqlGenerator;
        }

        public MapExtension GetMapExtension()
        {
            if (_mapExtension == null)
            {
                throw new ApplicationException("SQL Map 映射为空！");
            }
            return _mapExtension;
        }

        public T GetById<T>(dynamic primaryId) where T:class
        {
            IDbConnection conn = SessionFactory.CreateConnection();
            return GetById<T>(primaryId, conn, null);
        }

        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryId"></param>
        /// <returns></returns>
        public T GetById<T>(dynamic primaryId, IDbConnection conn, IDbTransaction transaction) where T : class
        {
            IEntityMapper classMap = _mapExtension.GetMap<T>();
            string sql = _sqlGenerator.Get(classMap);      
            var key = classMap.Properties.SingleOrDefault(p => p.ColumnType != ColumnType.NotAKey);

            IDbCommand cmd = SetupCommand(sql, CommandType.Text, conn, transaction);
            SqlParameter lnParam = cmd.CreateParameter() as SqlParameter;
            lnParam.ParameterName = key.Name;
            lnParam.Value = primaryId;
            cmd.Parameters.Add(lnParam);

            T result = Query<T>(cmd, conn, transaction).SingleOrDefault();

            return result;
        }

        public IEnumerable<T> GetByIds<T>(IList<dynamic> ids) where T : class
        {
            IDbConnection conn = SessionFactory.CreateConnection();
            return GetByIds<T>(ids, conn, null);
        }

        /// <summary>
        /// 根据多个Id获取多个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IEnumerable<T> GetByIds<T>(IList<dynamic> ids, IDbConnection conn, IDbTransaction transaction) where T : class
        {
            var idsin = string.Join(",", ids.ToArray<dynamic>());

            IEntityMapper classMap = _mapExtension.GetMap<T>();
            string sql = _sqlGenerator.GetInSql(classMap);
            var key = classMap.Properties.SingleOrDefault(p => p.ColumnType != ColumnType.NotAKey);

            IDbCommand cmd = SetupCommand(sql, CommandType.Text, conn, transaction);
            SqlParameter lnParam = cmd.CreateParameter() as SqlParameter;
            lnParam.ParameterName = key.Name;
            lnParam.Value = idsin;
            cmd.Parameters.Add(lnParam);

            IEnumerable<T> dataList = Query<T>(cmd, conn, transaction);
            return dataList;
        }

        /// <summary>
        /// 根据条件筛选出数据集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <returns></returns>
        public IEnumerable<T> Get<T>(string sql, IDbConnection conn, IDbTransaction transaction) where T : class
        {
            SqlCommand cmd = new SqlCommand(sql);
            IEnumerable<T> dataList = Query<T>(cmd, conn, transaction);

            return dataList;
        }

        /// <summary>
        /// 条件分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="allRowsCount"></param>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public IEnumerable<T> GetPage<T>(int pageIndex, int pageSize, string whereSql, dynamic param, string orderBySql, 
            IDbConnection conn, IDbTransaction transaction) where T : class
        {
            var sqlBuilder = new StringBuilder(1024);
            IEntityMapper classMap = _mapExtension.GetMap<T>();

            var selectFromSql = _sqlGenerator.GetSelectFrom(classMap);
            sqlBuilder.Append(selectFromSql);
            sqlBuilder.Append(" " + whereSql + " ");
            sqlBuilder.Append(" " + orderBySql + " ");

            var sql = sqlBuilder.ToString();
            var newSql = _sqlGenerator.GetPagingSql(sql, pageIndex, pageSize);

            SqlCommand cmd = new SqlCommand(newSql);
            IEnumerable<T> entityList = Query<T>(cmd, conn, transaction);

            return entityList;
        }

        public int GetCount<T>(string whereSql, IDbConnection conn, IDbTransaction transaction)
            where T:class
        {
            IEntityMapper classMap = _mapExtension.GetMap<T>();
            var countSql = _sqlGenerator.GetCountSql(classMap, whereSql);

            SqlCommand cmd = new SqlCommand(countSql);
            return (int)ExecuteScalar(cmd, conn, transaction);
        }

        public IEnumerable<T> Query<T>(string sql, dynamic param) where T:class
        {
            ISession session = SessionFactory.CreateSession();
            return Query<T>(sql, param, session.Connection, session.Transaction);
        }

        /// <summary>
        /// 根据参数条件筛选数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string sql, dynamic param, 
            IDbConnection conn, IDbTransaction transaction) where T:class
        {
            CacheInfo cacheInfo = new CacheInfo();

            if (param != null)
            {
                var identity = new Identity(sql, CommandType.Text, conn, null, param == null ? null : param.GetType(), null);
                cacheInfo.ParamReader = SqlCached.CreateParamInfoGenerator(identity, false);
            }

            IDbCommand cmd = null;
            try
            {
                cmd = SetupCommand(sql, cacheInfo.ParamReader, param, CommandType.Text, conn, transaction);
                IDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DynamicBuilder<T> builder = DynamicBuilder<T>.CreateBuilder(reader);
                    yield return builder.Build(reader);
                }
                reader.Close();
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        /// <summary>
        /// 数据查询方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(IDbCommand cmd, IDbConnection conn, IDbTransaction transaction) where T : class
        {
            try
            {
                cmd.Connection = conn;
                cmd.Transaction = transaction;
                IDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DynamicBuilder<T> builder = DynamicBuilder<T>.CreateBuilder(reader);
                    yield return builder.Build(reader);
                }
                reader.Close();
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

              
        /// <summary>
        /// 插入单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        public int Insert<T>(T entity, IDbConnection conn, IDbTransaction transaction) where T : class
        {
            IEntityMapper classMap = _mapExtension.GetMap<T>();
            string sql = _sqlGenerator.GetInsertSql(classMap);

            int result = Execute(sql, entity, CommandType.Text, conn, transaction);
            return result;
        }

        /// <summary>
        /// 更新单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update<T>(T entity, IDbConnection conn, IDbTransaction transaction) where T : class
        {
            IEntityMapper classMap = _mapExtension.GetMap<T>();
            string sql = _sqlGenerator.GetUpdateSql(classMap);

            int result = Execute(sql, entity, CommandType.Text, conn, transaction);
            return result;
        }


        /// <summary>
        /// 删除单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryId"></param>
        /// <returns></returns>
        public void Delete<T>(dynamic primaryId, IDbConnection conn, IDbTransaction transaction) where T : class
        {
            IEntityMapper classMap = _mapExtension.GetMap<T>();
            string sql = _sqlGenerator.GetDeleteSql(classMap);      
            var key = classMap.Properties.SingleOrDefault(p => p.ColumnType != ColumnType.NotAKey);

            SqlCommand cmd = new SqlCommand(sql);
            SqlParameter lnParam = cmd.CreateParameter();
            lnParam.ParameterName = key.Name;
            lnParam.Value = primaryId;
            cmd.Parameters.Add(lnParam);

            ExecuteCommand(cmd, conn, transaction);
        }

        /// <summary>
        /// 外部接口，执行Command
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="session"></param>
        public int ExecuteCommand(IDbCommand cmd, ISession session)
        {
            return ExecuteCommand(cmd, session.Connection, session.Transaction);
        }

        /// <summary>
        /// 执行Command
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        private int ExecuteCommand(IDbCommand cmd, IDbConnection conn, IDbTransaction transaction)
        {
            try
            {
                bool wasClosed = conn.State == ConnectionState.Closed;
                if (wasClosed) conn.Open();
                cmd.Connection = conn;
                return cmd.ExecuteNonQuery();
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        private object ExecuteScalar(IDbCommand cmd, IDbConnection conn, IDbTransaction transaction)
        {
            try
            {
                bool wasClosed = conn.State == ConnectionState.Closed;
                if (wasClosed) conn.Open();
                cmd.Connection = conn;
                cmd.Transaction = transaction;
                return cmd.ExecuteScalar();
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        /// <summary>
        /// 执行SQL 语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="cmdType"></param>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        private int Execute(string sql, object param, CommandType cmdType, 
            IDbConnection conn, IDbTransaction transaction)
        {
            CacheInfo cacheInfo = new CacheInfo();
            if (param != null)
            {
                var identity = new Identity(sql, cmdType, conn, null, param == null ? null : param.GetType(), null);
                cacheInfo.ParamReader = SqlCached.CreateParamInfoGenerator(identity, false);
            }

            int result = ExecuteCommandByParamReader(sql, param == null ? null : cacheInfo.ParamReader, param, cmdType, conn, transaction);
            return result;
        }

        /// <summary>
        /// 执行Command命令
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramReader"></param>
        /// <param name="obj"></param>
        /// <param name="commandType"></param>
        /// <param name="cnn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private int ExecuteCommandByParamReader(string sql, Action<IDbCommand, object> paramReader, object obj, 
            CommandType? commandType, IDbConnection conn, IDbTransaction transaction)
        {
            IDbCommand cmd = null;
            bool wasClosed = conn.State == ConnectionState.Closed;

            try
            {
                cmd = SetupCommand(sql, paramReader, obj, commandType, conn, transaction);
                if (wasClosed) conn.Open();
                
                return cmd.ExecuteNonQuery();
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                if (cmd != null) 
                    cmd.Dispose();
            }
        }

        /// <summary>
        /// 构造Command对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramReader"></param>
        /// <param name="obj"></param>
        /// <param name="commandType"></param>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private IDbCommand SetupCommand(string sql, Action<IDbCommand, object> paramReader, object obj, 
            CommandType? commandType, IDbConnection conn, IDbTransaction transaction)
        {
            var cmd = conn.CreateCommand();
            if (transaction != null)
                cmd.Transaction = transaction;

            cmd.CommandText = sql;

            if (commandType.HasValue)
                cmd.CommandType = commandType.Value;

            if (paramReader != null)
            {
                paramReader(cmd, obj);
            }

            return cmd;

        }

        private static IDbCommand SetupCommand(string sql, CommandType? commandType,
            IDbConnection conn, IDbTransaction transaction)
        {
            var cmd = conn.CreateCommand();

            if (transaction != null)
                cmd.Transaction = transaction;

            cmd.CommandText = sql;

            if (commandType.HasValue)
                cmd.CommandType = commandType.Value;

            return cmd;
        }
    }
}

