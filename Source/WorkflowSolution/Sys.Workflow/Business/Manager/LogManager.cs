using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Sys.Workflow.Common;
using Sys.Workflow.Utility;
using Sys.Workflow.DataModel;
using ServiceStack.Text;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 日志处理记录类
    /// </summary>
    public class LogManager
    {
        #region 属性、构造函数
        private DataAccessManager _logRepository;
        private DataAccessManager LogRepository
        {
            get
            {
                if (_logRepository == null)
                {
                    _logRepository = DataAccessFactory.Instance();
                }
                return _logRepository;
            }
        }

        public LogManager()
        {
        }
        #endregion

        #region 获取日志数据
        /// <summary>
        /// 获取日志记录（分页）
        /// </summary>
        /// <param name="query"></param>
        /// <param name="activityState"></param>
        /// <returns></returns>
        private IEnumerable<LogEntity> GetLogsPaged(LogQueryEntity query, out int allRowsCount)
        {
            ISession session = SessionFactory.CreateSession();
            string orderBySql = "ORDER BY LogID DESC";

            //如果数据记录数为0，则不用查询列表
            allRowsCount = LogRepository.GetCount<LogEntity>(string.Empty, session.Connection, session.Transaction);
            if (allRowsCount == 0)
            {
                return null;
            }

            //查询列表数据并返回结果集
            var list = LogRepository.GetPage<LogEntity>(query.PageIndex, query.PageSize, null,
                null,
                orderBySql,
                session.Connection,
                session.Transaction);

            return list;
        }
        #endregion

        #region 新增、更新和删除流程数据
        /// <summary>
        /// 插入流程日志数据
        /// </summary>
        /// <param name="entity"></param>
        public void Insert(object entity)
        {
            ISession session = SessionFactory.CreateSession();
            try
            {
                var log = (LogEntity)entity;
                LogRepository.Insert<LogEntity>(log, session.Connection, session.Transaction);
                session.Commit();
            }
            catch (System.Exception)
            {
                session.Rollback();
                throw;
            }
            finally
            {
                session.Dispose();
            }
        }

        /// <summary>
        /// 记录流程异常日志
        /// </summary>
        /// <param name="entity"></param>
        public static void RecordLog(string title, 
            LogEventType eventType, 
            LogPriority priority, 
            object extraObject,
            System.Exception e)
        {
            try
            {
                var log = new LogEntity();
                log.EventTypeID = (int)eventType;
                log.Priority = (int)priority;
                log.Severity = priority.ToString().ToUpper();
                log.Title = title;
                log.Timestamp = DateTime.Now;
                log.Message = e.Message;
                log.StackTrace = e.StackTrace.Length > 4000 ? e.StackTrace.Substring(0, 4000): e.StackTrace;
                if (e.InnerException != null)
                {
                    log.InnerStackTrace = e.StackTrace.Length > 4000 ? e.StackTrace.Substring(0, 4000) : e.StackTrace;
                }

                var jsonData = JsonSerializer.SerializeToString(extraObject);
                log.RequestData = jsonData.Length > 2000 ? jsonData.Substring(0, 2000) : jsonData;

                var lm = new LogManager();
                Thread thread = new Thread(new ParameterizedThreadStart(lm.Insert));
                thread.Start(log);
            }
            catch
            {
                //如果记录日志发生异常，不做处理
                ;
            }
        }
        #endregion
    }
}
