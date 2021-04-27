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
    public class UserService
    {
        [Description("Returns the users on the team.")]
        [WebGet(UriTemplate = "?skip={skip}&top={top}&manager={userName}")]
        public List<User> GetUsers(int skip, int top, string userName)
        {
            // Set reasonable defaults for the query string parameters
            skip = (skip >= 0) ? skip : 0;
            top = (top > 0) ? top : 25;

            using (TeamTaskObjectContext objectContext = new TeamTaskObjectContext())
            {
                // Include the where clause only if a userName was provided
                var userQuery = (string.IsNullOrWhiteSpace(userName)) ?
                    objectContext.Users :
                    objectContext.Users.Where(user => user.ManagerUserName == userName);

                return userQuery.OrderBy(user => user.UserName).Skip(skip).Take(top).ToList();
            }
        }

        [Description("Returns the details of a user on the team.")]
        [WebGet(UriTemplate = "{userName}")]
        public User GetUser(string userName)
        {
            using (TeamTaskObjectContext objectContext = new TeamTaskObjectContext())
            {
                // The 'Include' method is an ADO.NET Entity Framework feature that allows 
                //  us to load the user and his/her tasks in a single query!
                var user = objectContext.Users.Include("Tasks").FirstOrDefault(u => u.UserName == userName);
                if (user == null) 
                { 
                    throw new WebFaultException<string>( 
                        string.Format("There is no user with the userName '{0}'.", userName), 
                        HttpStatusCode.NotFound); 
                }
                return user;
            }
        }
    }
}
