using FacebookApplication.Models.Data;
using FacebookApplication.Models.ViewModels.Profile;
using FacebookClone.Models.ViewModels.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FacebookApplication.Controllers
{
    public class ProfileController : Controller
    {
        // GET: /
        public ActionResult Index()
        {
            return View();
        }


        // POST: Profile/LiveSearch
        [HttpPost]
        public JsonResult LiveSearch(string searchVal)
        {
            Db db = new Db();
            List<LiveSearchUserVM> usernames = db.Users.Where(x => x.Username.Contains(searchVal) && x.Username != User.Identity.Name).ToArray().Select(x => new LiveSearchUserVM(x)).ToList();
            return Json(usernames);
        }

        // POST: Profile/AddFriend
        [HttpPost]
        public void AddFriend(string friend)
        {
            Db db = new Db();

            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            UserDTO userDTO2 = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
            int friendId = userDTO2.Id;


            FriendDTO friendDTO = new FriendDTO();

            friendDTO.User1 = userId;
            friendDTO.User2 = friendId;
            friendDTO.Active = false;

            db.Friends.Add(friendDTO);

            db.SaveChanges();
        }

        // POST: Profile/DisplayFriendRequests
        [HttpPost]
        public JsonResult DisplayFriendRequests()
        {
            Db db = new Db();

            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            List<FriendRequestVM> list = db.Friends.Where(x => x.User2 == userId && x.Active == false).ToArray().Select(x => new FriendRequestVM(x)).ToList();


            List<UserDTO> users = new List<UserDTO>();

            foreach (var item in list)
            {
                var user = db.Users.Where(x => x.Id == item.User1).FirstOrDefault();
                users.Add(user);
            }

            return Json(users);
        }


        // POST: Profile/AcceptFriendRequest
        [HttpPost]
        public void AcceptFriendRequest(int friendId)
        {
            Db db = new Db();

            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            FriendDTO friendDTO = db.Friends.Where(x => x.User1 == friendId && x.User2 == userId).FirstOrDefault();

            friendDTO.Active = true;

            db.SaveChanges();
        }

        // POST: Profile/DeclineFriendRequest
        [HttpPost]
        public void DeclineFriendRequest(int friendId)
        {
            Db db = new Db();

            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            FriendDTO friendDTO = db.Friends.Where(x => x.User1 == friendId && x.User2 == userId).FirstOrDefault();

            db.Friends.Remove(friendDTO);

            db.SaveChanges();
        }

        // POST: Profile/SendMessage
        [HttpPost]
        public void SendMessage(string friend, string message)
        {
            Db db = new Db();

            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            UserDTO userDTO2 = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
            int userId2 = userDTO2.Id;


            MessageDTO dto = new MessageDTO();

            dto.From = userId;
            dto.To = userId2;
            dto.Message = message;
            dto.DateSent = DateTime.Now;
            dto.Read = false;

            db.Messages.Add(dto);
            db.SaveChanges();
        }

        // POST: Profile/DisplayUnreadMessages
        [HttpPost]
        public JsonResult DisplayUnreadMessages()
        {
            Db db = new Db();

            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            List<MessageVM> list = db.Messages.Where(x => x.To == userId && x.Read == false).ToArray().Select(x => new MessageVM(x)).ToList();

            db.Messages.Where(x => x.To == userId && x.Read == false).ToList().ForEach(x => x.Read = true);
            db.SaveChanges();

            return Json(list);
        }
    }
}