//----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TeamTask.Model
{
    public class User
    {
        [IgnoreDataMember]
        public byte[] Version { get; set; }

        [IgnoreDataMember]
        public List<User> DirectReports { get; set; }

        [IgnoreDataMember]
        public User Manager { get; set; }

        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ManagerUserName { get; set; }
        public List<Task> Tasks { get; set; }
    }
}
