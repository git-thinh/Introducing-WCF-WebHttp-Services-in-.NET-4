//----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------------------
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace TeamTask.Model
{
    public class Task
    {
        [IgnoreDataMember]
        public byte[] Version { get; set; }

        [IgnoreDataMember]
        public User Owner { get; set; }

        // Surfaced below as a TaskStatus Enum
        [IgnoreDataMember]
        public int TaskStatusCode { get; set; }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string OwnerUserName { get; set; }

        public TaskStatus Status
        {
            get
            {
                return (TaskStatus)this.TaskStatusCode;
            }
            set
            {
                this.TaskStatusCode = (int)value;
            }
        }

        internal IEnumerable<string> GetModifiedProperties()
        {
            // Create the list with the required properties
            List<string> modifiedProperties = new List<string>() { "TaskStatusCode", "OwnerUserName", "Title" };

            // Add the optional properties
            if (!string.IsNullOrWhiteSpace(this.Description))
            {
                modifiedProperties.Add("Description");
            }
            if (this.CompletedOn.HasValue)
            {
                modifiedProperties.Add("CompletedOn");
            }

            return modifiedProperties;
        }
    }
}
