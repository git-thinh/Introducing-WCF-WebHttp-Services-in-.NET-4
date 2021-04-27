//----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------------------
using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;

namespace TeamTask.Service
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            WebServiceHostFactory factory = new WebServiceHostFactory();
            RouteTable.Routes.Add(new ServiceRoute("Tasks", factory, typeof(TaskService)));
            RouteTable.Routes.Add(new ServiceRoute("Users", factory, typeof(UserService)));
        }
    }
}
