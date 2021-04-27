using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BasicAuthentication.Controllers
{
   
    public class ValuesController : ApiController
    {
        [BasicAuthentication]
        public string Get()
        {
            return "WebAPI Method Called";
        }
    }
}

/*$.ajax({
type:'GET',
url:"api/values/Get",
datatype:'json',
headers:
    {
        Authorization : 'Basic '+ btoa(username+':'+password)
    },
    success: function(data){
    },
    error: function(data){
    }
});*/
