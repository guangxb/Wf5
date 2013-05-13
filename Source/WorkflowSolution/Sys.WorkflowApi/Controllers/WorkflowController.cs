using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.Http;
using System.Web.Http.Controllers;
using Sys.Workflow;
using Sys.Workflow.Common;
using Sys.Workflow.Engine;
using Sys.Workflow.Utility;
using Sys.Workflow.DataModel;
using Sys.Workflow.Business;
using Sys.WorkflowApi.Utility;

namespace Sys.WorkflowApi.Controllers
{
    //流程调用JSON格式说明：

    //startup process:
    //{"UserID":"10","UserName":"Long","AppName":"SamplePrice","AppInstanceID":"100","ProcessGUID":"072af8c3-482a-4b1c-890b-685ce2fcc75d"}

    //read task, and make activity running:
    //任务阅读：
    //{"UserID":"10","UserName":"Long","TaskID":"17"}}

    //run process app:
    ////根据业务数据运行流程
    ////前台审批办理节点：
    ////选择的下一步是“财务审批”办理节点
    //{"AppName":"SamplePrice","AppInstanceID":"100","ProcessGUID":"072af8c3-482a-4b1c-890b-685ce2fcc75d","UserID":"10","UserName":"Long","NextActivityPerformers":{"10f7481a-ad1a-40f6-aeaa-8d32ceb1fcab":[{"UserID":10,"UserName":"Long"}]}}

    //run process task:
    ////根据TaskID运行流程


    //withdraw process:
    //撤销至上一步节点（由财务审批到上一步前台办理）
    //{"UserID":"10","UserName":"Long","AppName":"SamplePrice","AppInstanceID":"100","ProcessGUID":"072af8c3-482a-4b1c-890b-685ce2fcc75d"}


    //财务审批办理节点：
    ////下一步是结束节点
    //{"UserID":"10","UserName":"Long","AppName":"SamplePrice","AppInstanceID":"100","TaskID":"5","NextActivityPerformers":{"b70e717a-08da-419f-b2eb-7a3d71f054de":[{"UserID":10,"UserName":"Long"}]}}

    //获取下一步办理步骤：
    //1) 根据应用来获取
    //GetNextSteps
    //{"AppName":"SamplePrice","AppInstanceID":915,"UserID":"10","UserName":"Long","ProcessGUID":"072af8c3-482a-4b1c-890b-685ce2fcc75d","NextActivityPerformers":{"39c71004-d822-4c15-9ff2-94ca1068d745":[{"UserID":"10","UserName":"Long"}]},"Flowstatus":"启动"}

    //2) 根据任务ID来获取
    //GetTaskNextSteps

    //撤销流程: WithdrawProcess
    //退回流程：RejectProcess
    //返签流程：ReverseProcess
    //取消运行流程：CancelProcess
    //废弃所有流程实例：DiscardProcess
    /// <summary>
    /// </summary>
    public class WorkflowController  : ApiController
    {
        public WorkflowController()
        {

        }

        #region Workflow 数据访问基本操作
        [HttpGet]
        [AllowAnonymous]
        public string Hello()
        {
            return "Hello World!";
        }

        // GET: /Workflow/
        [HttpGet]
        [AllowAnonymous]
        public object Get()
        {
            DataAccessManager repository = DataAccessFactory.Instance();
            var process = repository.GetById<ProcessEntity>("7CF9E19E-9E57-4944-BA1D-E6C78DFF5CBE");

            return process;
        }

        [HttpGet]
        [AllowAnonymous]
        public object GetProcessByName()
        {
            ProcessManager manager = new ProcessManager();
            var process = manager.GetByName("Booking Park Process");
            return process;
        }

        [HttpPost]
        [AllowAnonymous]
        public void InsertProcess(ProcessEntity obj)
        {
            ProcessManager pm = new ProcessManager();
            pm.Insert(obj);

        }

        [HttpPost]
        [AllowAnonymous]
        public void UpdateProcess(ProcessEntity obj)
        {
            ProcessManager pm = new ProcessManager();
            pm.Update(obj);
        }

        [HttpPost]
        [AllowAnonymous]
        public void RemoveProcess(Guid processGUID)
        {
            ProcessManager pm = new ProcessManager();
            pm.Delete(processGUID);
        }

        [HttpGet]
        [AllowAnonymous]
        public ProcessInstanceEntity GetProcessInstance()
        {
            var guid = Guid.Parse("84045957-5F74-4B99-9C48-01B67835535E");
            IWorkflowService service = new WorkflowService();
            var instance = service.GetProcessInstance(guid);

            return instance;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActivityInstanceEntity GetActivityInstance()
        {
            var guid = Guid.Parse("6B868C57-BF8A-461D-95E6-373621023070");
            IWorkflowService service = new WorkflowService();
            var instance = service.GetActivityInstance(guid);

            return instance;
        }
        #endregion

        #region Workflow Api访问操作

        /// <summary>
        /// 获取第一个Activity
        /// </summary>
        /// <param name="id">流程GUID</param>
        /// <returns></returns>
        //[HttpGet]
        //[AllowAnonymous]
        //public ResponseResult<string> GetFirstActivity(dynamic id)
        //{
        //    try
        //    {
        //        ActivityEntity firstActivity = WfFacade.GetFirstActivity(id);
        //        if (firstActivity != null)
        //            return ResponseResult<string>.Success(firstActivity.ActivityName);
        //        else
        //            return ResponseResult<string>.Error("读取流程第一个节点名称失败!");
        //    }
        //    catch (System.Exception e)
        //    {
        //        return ResponseResult<string>.Error(string.Format("读取流程第一个节点名称失败:{0}" + e.Message));
        //    }
        //}

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult StartProcess(WfAppRunner starter)
        {
            try
            {
                IWorkflowService wfService = new WorkflowService();
                WfExecutedResult result = wfService.StartProcess(starter);
                Guid newProcessInstanceGUID = result.ProcessInstanceGUID;
                IList<NodeView> nextStpes = result.NextActivityTree;

                return ResponseResult.Success();
            }
            catch (WorkflowException w)
            {
                return ResponseResult.Error(w.Message);
            }
        }

        /// <summary>
        /// 获取下一步办理的节点
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ResponseResult<IList<NodeView>> GetTaskNextSteps(long id)
        {
            IWorkflowService wfService = new WorkflowService();
            var nodeList = wfService.GetNextActivityTree(id);

            return ResponseResult<IList<NodeView>>.Success(nodeList);
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult<IList<NodeView>> GetNextSteps(WfAppRunner runner)
        {
            IWorkflowService wfService = new WorkflowService();
            var nodeList = wfService.GetNextActivityTree(runner);

            return ResponseResult<IList<NodeView>>.Success(nodeList);
        }

        [HttpGet]
        [AllowAnonymous]
        public ResponseResult<IDictionary<Guid, PerformerList>> GetNextActivityPerformers()
        {
            var performers = new PerformerList();
            performers.Add(new Performer(10, "Long"));

            IDictionary<Guid, PerformerList> nexts = new Dictionary<Guid, PerformerList>();
            nexts[Guid.Parse("10f7481a-ad1a-40f6-aeaa-8d32ceb1fcab")] = performers;

            return ResponseResult<IDictionary<Guid, PerformerList>>.Success(nexts);
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult ReadTask(WfTaskRunner runner)
        {
            IWorkflowService wfService = new WorkflowService();
            bool isRead = wfService.ReadTask(runner);

            return ResponseResult.Success();
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult RunProcessApp(WfAppRunner runner)
        {
            IWorkflowService wfService = new WorkflowService();
            var result = wfService.RunProcessApp(runner);

            if (result.Status == WfExecutedStatus.Successed)
                return ResponseResult.Success();
            else
                return ResponseResult.Error(result.Message);
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult RunProcessTask(WfTaskRunner runner)
        {
            IWorkflowService wfService = new WorkflowService();
            var result = wfService.RunProcessTask(runner);

            if (result.Status == WfExecutedStatus.Successed)
                return ResponseResult.Success();
            else
                return ResponseResult.Error(result.Message);
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult WithdrawProcess(WfAppRunner withdrawer)
        {
            IWorkflowService wfService = new WorkflowService();
            var result = wfService.WithdrawProcess(withdrawer);

            if (result.Status == WfExecutedStatus.Successed)
                return ResponseResult.Success();
            else
                return ResponseResult.Error(result.Message);
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult RejectProcess(WfAppRunner rejector)
        {
            IWorkflowService wfService = new WorkflowService();
            var result = wfService.RejectProcess(rejector);

            if (result.Status == WfExecutedStatus.Successed)
                return ResponseResult.Success();
            else
                return ResponseResult.Error(result.Message);
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult ReverseProcess(WfAppRunner reverser)
        {
            IWorkflowService service = new WorkflowService();
            var result = service.ReverseProcess(reverser);

            if (result.Status == WfExecutedStatus.Successed)
                return ResponseResult.Success();
            else
                return ResponseResult.Error(result.Message);
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult DiscardProcess(WfAppRunner discarder)
        {
            IWorkflowService service = new WorkflowService();
            var result = service.DiscardProcess(discarder);

            return ResponseResult.Success();
        }
        #endregion

        #region 任务数据读取操作
        [HttpPost]
        [AllowAnonymous]
        public ResponseResult GetRunningTasks(TaskQueryEntity query)
        {
            IWorkflowService service = new WorkflowService();
            var result = service.GetRunningTasks(query);

            return ResponseResult.Success();
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult GetReadyTasks(TaskQueryEntity query)
        {
            IWorkflowService service = new WorkflowService();
            var result = service.GetReadyTasks(query);

            return ResponseResult.Success();
        }
        #endregion

        #region 流程一体化测试
        [HttpPost]
        [AllowAnonymous]
        public ResponseResult StartupRunningEnd(WfAppRunner initiator)
        {
            IWorkflowService service = new WorkflowService();
            var result = service.StartupRunningEnd(initiator);

            if (result.Status == WfExecutedStatus.Successed)
                return ResponseResult.Success();
            else
                return ResponseResult.Error(result.Message);
        }
        #endregion
    }
}
