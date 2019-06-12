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

        public void GetFrcount()
        {
            Db db = new Db();

            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            var friendReqCount = db.Friends.Count(x => x.User2 == userId && x.Active == false);

            var clients = Clients.Caller;

            clients.frcount(Context.User.Identity.Name, friendReqCount);
        }


        public void GetFcount(int friendId)
        {
            Db db = new Db();

            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            var friendCount1 = db.Friends.Count(x => x.User2 == userId && x.Active == true || x.User1 == userId && x.Active == true);

            UserDTO userDTO2 = db.Users.Where(x => x.Id == friendId).FirstOrDefault();
            string username = userDTO2.Username;

            var friendCount2 = db.Friends.Count(x => x.User2 == friendId && x.Active == true || x.User1 == friendId && x.Active == true);


            var clients = Clients.All;

            clients.fcount(Context.User.Identity.Name, username, friendCount1, friendCount2);

        }

        public void NotifyOfMessage(string friend)
        {
            Db db = new Db();

            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
            int friendId = userDTO.Id;

            var messageCount = db.Messages.Count(x => x.To == friendId && x.Read == false);

            var clients = Clients.Others;

            clients.msgcount(friend, messageCount);
        }

        public void NotifyOfMessageOwner()
        {
            Db db = new Db();

            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            var messageCount = db.Messages.Count(x => x.To == userId && x.Read == false);

            var clients = Clients.Caller;

            clients.msgcount(Context.User.Identity.Name, messageCount);
        }

    }
}