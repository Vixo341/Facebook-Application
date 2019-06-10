﻿using FacebookApplication.Models.Data;
using FacebookApplication.Models.ViewModels.Profile;
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

    }
}