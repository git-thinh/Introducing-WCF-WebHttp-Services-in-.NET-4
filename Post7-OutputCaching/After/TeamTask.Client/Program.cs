//----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Linq;
using Microsoft.Http;
using TeamTask.Model;

namespace TeamTask.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            using (HttpClient client = new HttpClient("http://localhost:8080/TeamTask/"))  
            {
                GetUser(client, "user1", "application/json");

                Console.WriteLine("Press Enter to call GetUsers() with top=3.  Should hit the breakpoint...");
                Console.ReadKey();
                client.Get("Users/?top=3");

                Console.WriteLine("Press Enter to call GetUsers() with top=3 again. Should hit in the cache...");
                Console.ReadKey();
                client.Get("Users/?top=3");

                Console.WriteLine("Press Enter to call GetUsers() with top=2. Should hit the breakpoint...");
                Console.ReadKey();
                client.Get("Users/?top=2");

                Console.ReadKey();
             }
        }

        static void WriteOutTask(Task task)
        {
            Console.WriteLine("  Id:      {0}", task.Id);
            Console.WriteLine("  Title:   {0}", task.Title);
            Console.WriteLine("  Status:  {0}", task.Status);
            Console.WriteLine("  Created: {0}", task.CreatedOn.ToShortDateString());
            Console.WriteLine();
        }

        static void GetUser(HttpClient client, string userName, string accepts)
        {
            Console.WriteLine("Getting user '{0}':", userName);
            using (HttpRequestMessage request = new HttpRequestMessage("GET", "Users/" + userName))
            {
                request.Headers.Accept.AddString(accepts);
                using (HttpResponseMessage response = client.Send(request))
                {
                    Console.WriteLine("  Cache-Control: {0}", response.Headers.CacheControl);
                    Console.WriteLine("  Status Code: {0}", response.StatusCode);
                    Console.WriteLine("  Content: {0}", response.Content.ReadAsString());
                }
            }
            Console.WriteLine();
        }

        static Task GetTask(HttpClient client, int id)
        {
            Console.WriteLine("Getting task '{0}':", id);
            using (HttpRequestMessage request = new HttpRequestMessage("GET", "Tasks"))
            {
                request.Headers.Accept.AddString("application/json");
                using (HttpResponseMessage response = client.Send(request))
                {
                    response.EnsureStatusIsSuccessful();
                    WriteOutContent(response.Content);
                    return response.Content.ReadAsJsonDataContract<List<Task>>()
                                           .FirstOrDefault(task => task.Id == id);
                }
            }
        }

        static Task UpdateTask(HttpClient client, Task task)
        {
            Console.WriteLine("Updating task '{0}':", task.Id);
            Console.WriteLine();

            string updateUri = "Tasks/" + task.Id.ToString();
            HttpContent content = HttpContentExtensions.CreateJsonDataContract(task);
            Console.WriteLine("Request:");
            WriteOutContent(content);

            using (HttpResponseMessage response = client.Put(updateUri, content))
            {
                response.EnsureStatusIsSuccessful();
                Console.WriteLine("Response:");
                WriteOutContent(response.Content);
                return response.Content.ReadAsJsonDataContract<Task>();              
            }
        }

        static void WriteOutContent(HttpContent content)
        {
            content.LoadIntoBuffer();
            Console.WriteLine(content.ReadAsString());
            Console.WriteLine();
        }
    }
}
