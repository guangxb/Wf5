using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Sys.Workflow.Business;
using Sys.Workflow.Engine;
using Sys.Workflow.Common;
using Sys.Workflow.Utility;
using Sys.Workflow.DataModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sys.WorkflowTest
{
    [TestClass]
    public class ProcessRunningTest
    {
        [TestMethod]
        public void StartupParalleled()
        {
            //StarterA:
            //{"UserID":"10","UserName":"Long","AppName":"SamplePrice","AppInstanceID":"100","ProcessGUID":"072af8c3-482a-4b1c-890b-685ce2fcc75d"}
            var starterA = new WfAppRunner();
            starterA.ProcessGUID = Guid.Parse("072af8c3-482a-4b1c-890b-685ce2fcc75d");
            starterA.UserID = 10;
            starterA.UserName = "Long";

            //SarterB:
            //{"AppName":"Offin","AppInstanceID":587,"UserID":"0021","UserName":"test2","ProcessGUID":"68696ea3-00ab-4b40-8fcf-9859dbbde378","FlowStatus":null,"Conditions":{"surplus":"aa"}}
            var starterB = new WfAppRunner();
            starterB.ProcessGUID = Guid.Parse("68696ea3-00ab-4b40-8fcf-9859dbbde378");
            starterB.UserID = 21;
            starterB.UserName = "test2";

            IWorkflowService serviceA, serviceB;
            for (var i = 0; i < 500; i++)
            {
                serviceA = new WorkflowService();
                starterA.AppName = "Price";
                starterA.AppInstanceID = i;
                
                serviceB = new WorkflowService();
                starterB.AppName = "Offin";
                starterB.AppInstanceID = i;

                //execute process instance
                serviceA.StartProcess(starterA);
                serviceB.StartProcess(starterB);
            }
        }
    }
}
