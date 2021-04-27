//----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------------------
using System.Data.Objects;
using System;

namespace TeamTask.Model
{
    public class TeamTaskObjectContext : ObjectContext
    {
        private ObjectSet<User> _users;
        private ObjectSet<Task> _tasks;

        public TeamTaskObjectContext()
            : base("name=TeamTaskEntities")
        { }

        public ObjectSet<User> Users
        {
            get { return _users ?? (_users = CreateObjectSet<User>()); }
        }
        
        public ObjectSet<Task> Tasks
        {
            get { return _tasks ?? (_tasks = CreateObjectSet<Task>()); }
        }

        public void AttachTask(Task task)
        {
            this.Tasks.Attach(task);
            ObjectStateEntry entry = this.ObjectStateManager.GetObjectStateEntry(task);
            foreach (string propertyName in task.GetModifiedProperties())
            {
                entry.SetModifiedProperty(propertyName);
            }
        }
    }
}
