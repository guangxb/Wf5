using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Workflow.Common;
using Sys.Workflow.Utility;
using Sys.Workflow.DataModel;
using Sys.Workflow.Business;

namespace Sys.Workflow.Engine
{
    //internal class ActivityExecutionContexttRunning : ActivityExecutionContext
    //{
    //    internal ActivityExecutionContexttRunning(long taskID,
    //        ProcessModel processModel,
    //        ActivityResource activityResource) 
    //        : base(processModel, activityResource)
    //    {
    //        var taskInstance = (new TaskManager()).GetTaskView(taskID);
    //        var activityInstance = 
    //            (new ActivityInstanceManager()).GetById(taskInstance.ActivityInstanceGUID);

    //        var ProcessInstanceGUID = activityInstance.ProcessInstanceGUID;
    //        var processInstance = new ProcessInstanceManager()
    //            .GetById(ProcessInstanceGUID);

    //        base.ProcessInstance = processInstance;
    //        base.ActivityInstance = activityInstance;
    //        base.Activity = processModel.GetActivity(taskInstance.ActivityGUID);
    //        base.TaskID = taskID;
    //    }
    //}
}
