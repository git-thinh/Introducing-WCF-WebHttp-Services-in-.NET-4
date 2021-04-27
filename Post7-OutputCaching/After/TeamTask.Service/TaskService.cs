//----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using TeamTask.Model;

namespace TeamTask.Service
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class TaskService
    {
        [Description("Returns the tasks that are owned by the team.")]
        [WebGet(UriTemplate = "?skip={skip}&top={top}&owner={userName}&format={format}")]
        public List<Task> GetTasks(int skip, int top, string userName, string format)
        {
            // Set reasonable defaults for the query string parameters
            skip = (skip >= 0) ? skip : 0;
            top = (top > 0) ? top : 25;

            // Set the format from the format query variable if it is available
            if (string.Equals("json", format, StringComparison.OrdinalIgnoreCase))
            {
                WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Json;
            }

            using (TeamTaskObjectContext objectContext = new TeamTaskObjectContext())
            {
                // Include the where clause only if a userName was provided
                var taskQuery = (string.IsNullOrWhiteSpace(userName)) ?
                    objectContext.Tasks :
                    objectContext.Tasks.Where(task => task.OwnerUserName == userName);

                return taskQuery.OrderBy(task => task.Id).Skip(skip).Take(top).ToList();
            }
        }

        [Description("Returns the details of a single task.")]
        [WebGet(UriTemplate = "{id}")]
        public Task GetTask(string id)
        {
            int parsedId = ParseTaskId(id);
            using (TeamTaskObjectContext objectContext = new TeamTaskObjectContext())
            {
                var task = objectContext.Tasks.FirstOrDefault(t => t.Id == parsedId);
                if (task == null)
                {
                    ThrowNoSuchTaskId(parsedId);
                }
                return task;
            }
        }

        [Description("Allows the details of a single task to be updated.")]
        [WebInvoke(UriTemplate = "{id}", Method = "PUT")]
        public Task UpdateTask(string id, Task task)
        {
            // The id from the URI determines the id of the task  
            int parsedId = ParseTaskId(id);
            task.Id = parsedId;

            using (TeamTaskObjectContext objectContext = new TeamTaskObjectContext())
            {
                objectContext.AttachTask(task);
                try
                {
                    objectContext.SaveChanges();
                }
                catch (OptimisticConcurrencyException)
                {
                    ThrowNoSuchTaskId(parsedId); 
                }
                // Because we allow partial updates, we need to refresh the task from the dB
                objectContext.Refresh(RefreshMode.StoreWins, task);
            }

            return task;
        }

        private static void ThrowNoSuchTaskId(int parsedId)
        {
            throw new WebFaultException<string>(
                string.Format("There is no task with the id '{0}'.", parsedId),
                HttpStatusCode.NotFound);
        }

        private static int ParseTaskId(string id)
        {
            int parsedId;
            if (!int.TryParse(id, out parsedId))
            {
                throw new WebFaultException<string>(
                    string.Format("The value '{0}' is not a valid task id. The id must be an integer.", id),
                    HttpStatusCode.BadRequest);
            }
            return parsedId;
        }
    }
}
