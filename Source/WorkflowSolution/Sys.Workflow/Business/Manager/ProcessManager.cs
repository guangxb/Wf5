using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Common;
using Sys.Workflow.Utility;
using Sys.Workflow.DataModel;

namespace Sys.Workflow.Business
{
    /// <summary>
    /// 流程定义管理类
    /// </summary>
    public class ProcessManager
    {
        #region 属性、构造函数
        private DataAccessManager _processRepository;
        private DataAccessManager ProcessRepository
        {
            get
            {
                if (_processRepository == null)
                {
                    _processRepository = DataAccessFactory.Instance();
                }
                return _processRepository;
            }
        }

        public ProcessManager()
        {
        }
        #endregion

        #region 获取流程数据
        public ProcessEntity GetById(Guid processGUID)
        {
            IDbConnection conn = SessionFactory.CreateConnection();
            return ProcessRepository.GetById<ProcessEntity>(processGUID, conn, null);
        }

        public ProcessEntity GetByName(string processName)
        {
            var sql = "SELECT * FROM WfProcess WHERE ProcessName=@processName";
            var entityList = ProcessRepository.Query<ProcessEntity>(sql, new { processName = processName }).ToList();
            return entityList[0];
        }
        #endregion

        #region 新增、更新和删除流程数据
        public void Insert(ProcessEntity entity)
        {
            ISession session = SessionFactory.CreateSession();
            try
            {
                ProcessRepository.Insert<ProcessEntity>(entity, session.Connection, session.Transaction);
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

        public void Update(ProcessEntity entity)
        {
            ISession session = SessionFactory.CreateSession();
            try
            {
                ProcessRepository.Update<ProcessEntity>(entity, session.Connection, session.Transaction);
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

        public void Delete(Guid processGUID)
        {
            ISession session = SessionFactory.CreateSession();
            try
            {
                var entity = GetById(processGUID);
                ProcessRepository.Delete<ProcessEntity>(entity, session.Connection, session.Transaction);
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
        #endregion 
    }
}
