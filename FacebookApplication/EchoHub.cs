using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Diagnostics;
using FacebookApplication.Models.Data;

namespace FacebookApplication
{
    [HubName("echo")]
    public class EchoHub : Hub
    {
        public void Hello(string message)
        {
            //Clients.All.hello();
            Trace.WriteLine(message);

            var clients = Clients.All;
            clients.test("this is a test");
        }

        public void Notify(string friend)
        {
            Db db = new Db();

            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
            int friendId = userDTO.Id;

            var frCount = db.Friends.Count(x => x.User2 == friendId && x.Active == false);
            var clients = Clients.Others;
            clients.frnotify(friend, frCount);
        }
    }
}