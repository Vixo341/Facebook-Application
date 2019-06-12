using FacebookApplication.Models.Data;
using FacebookApplication.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FacebookApplication.Controllers
{
    public class AccountController : Controller
    {
        // GET: /
        public ActionResult Index()
        {
            // Confirm user is not logged in

            string username = User.Identity.Name;

            if (!string.IsNullOrEmpty(username))
                return Redirect("~/" + username);

            return View();
        }

        // POST: Account/CreateAccount
        [HttpPost]
        public ActionResult CreateAccount(UserVM model, HttpPostedFileBase file)
        {
            Db db = new Db();

            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            if (db.Users.Any(x => x.Username.Equals(model.Username)))
            {
                ModelState.AddModelError("", "Username " + model.Username + " is taken.");
                model.Username = "";
                return View("Index", model);
            }

            UserDTO userDTO = new UserDTO()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailAddress = model.EmailAddress,
                Username = model.Username,
                Password = model.Password
            };
            db.Users.Add(userDTO);
            db.SaveChanges();

            int userId = userDTO.Id;
            FormsAuthentication.SetAuthCookie(model.Username, false);
            var uploadsDir = new DirectoryInfo(string.Format("{0}Uploads", Server.MapPath(@"\")));

            if (file != null && file.ContentLength > 0)
            {
                string ext = file.ContentType.ToLower();

                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    ModelState.AddModelError("", "The image was not uploaded - wrong image extension.");
                    return View("Index", model);
                }

                string imageName = userId + ".jpg";
                var path = string.Format("{0}\\{1}", uploadsDir, imageName);
                file.SaveAs(path);
            }
            // Redirect
            return Redirect("~/" + model.Username);
        }


        // GET: /{username}
        [Authorize]
        public ActionResult Username(string username = "")
        {
            Db db = new Db();

            if (!db.Users.Any(x => x.Username.Equals(username)))
            {
                return Redirect("~/");
            }

            ViewBag.Username = username;

            string user = User.Identity.Name;

            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(user)).FirstOrDefault();
            ViewBag.FullName = userDTO.FirstName + " " + userDTO.LastName;

            int userId = userDTO.Id;

            ViewBag.UserId = userId;

            UserDTO userDTO2 = db.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            ViewBag.ViewingFullName = userDTO2.FirstName + " " + userDTO2.LastName;

            ViewBag.UsernameImage = userDTO2.Id + ".jpg";

            string userType = "guest";

            if (username.Equals(user))
                userType = "owner";

            ViewBag.UserType = userType;



            if (userType == "guest")
            {
                UserDTO u1 = db.Users.Where(x => x.Username.Equals(user)).FirstOrDefault();
                int id1 = u1.Id;

                UserDTO u2 = db.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
                int id2 = u2.Id;

                FriendDTO f1 = db.Friends.Where(x => x.User1 == id1 && x.User2 == id2).FirstOrDefault();
                FriendDTO f2 = db.Friends.Where(x => x.User2 == id1 && x.User1 == id2).FirstOrDefault();

                if (f1 == null && f2 == null)
                {
                    ViewBag.NotFriends = "True";
                }

                if (f1 != null)
                {
                    if (!f1.Active)
                    {
                        ViewBag.NotFriends = "Pending";
                    }
                }

                if (f2 != null)
                {
                    if (!f2.Active)
                    {
                        ViewBag.NotFriends = "Pending";
                    }
                }

            }

            var friendCount = db.Friends.Count(x => x.User2 == userId && x.Active == false);

            if (friendCount > 0)
            {
                ViewBag.FRCount = friendCount;
            }

            // Return
            return View();
        }

        // GET: account/Logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            // Redirect
            return Redirect("~/");
        }

        public ActionResult LoginPartial()
        {
            return PartialView();
        }


        // POST: account/Login
        [HttpPost]
        public string Login(string username, string password)
        {
            Db db = new Db();

            if (db.Users.Any(x => x.Username.Equals(username) && x.Password.Equals(password)))
            {
                FormsAuthentication.SetAuthCookie(username, false);
                return "ok";
            }
            else
            {
                return "problem";
            }

        }

    }
}