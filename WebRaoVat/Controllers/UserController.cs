using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebRaoVat.Models;
using System.Data.Entity;
using System.IO;

namespace WebRaoVat.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        LoaPhatThanhEntities db = new LoaPhatThanhEntities();
        public ActionResult Index()
        {
            return View();
        }
        public User GetCurrentUser()
        {
            User currentUser = new User();
            if (Session["User"] != null)
            {
                currentUser = (User)Session["User"];
                if (db.Users.Where(u => u.user_id == currentUser.user_id && u.password == currentUser.password).FirstOrDefault() != null)
                    return currentUser;
                else
                    return null;
            }
            else
                return null;
        }
        //[HttpGet]
        public ActionResult ProfileUser(string id)
        {
            User curuser = GetCurrentUser();
            if (curuser != null && curuser.user_id == id)
            {
                List<User> listUser = db.Users.AsNoTracking().ToList();
                List<Follow> followerList = db.Follows.AsNoTracking().Where(f => f.follower == id).ToList();
                int follows = db.Follows.AsNoTracking().Where(f => f.user_id == id).ToList().Count();
                int be_follows = db.Follows.AsNoTracking().Where(f => f.follower == id).ToList().Count();
                var user = (from _user in listUser
                                //join _follow in db.Follows on _user.user_id equals _follow.user_id
                            where _user.user_id == id
                            select new Detail_User()
                            {
                                Name = _user.user_id,
                                Phone = _user.phone,
                                Image = _user.image,
                                Email = _user.email,
                                Rated = (double)_user.rate / (double)_user.rate_count,
                                Date_join = _user.date_join,
                                follow = follows,
                                be_follow = be_follows,
                                Follower_list = followerList
                            }).SingleOrDefault();
                return View(user);
            }
            else
                return Redirect("/User/ViewUser/" + id);
        }
        //[HttpGet]
        public ActionResult ViewUser(string id)
        {
            User temp = GetCurrentUser();
            if (temp != null)
            {
                if (id == temp.user_id)
                {
                    return RedirectToAction("ProfileUser", "User", new { @id = temp.user_id });
                }
            }
            List<User> listUser = db.Users.ToList();
            List<Follow> followerList = db.Follows.AsNoTracking().Where(f => f.follower == id).ToList();
            int follows = db.Follows.AsNoTracking().Where(f => f.user_id == id).ToList().Count();
            int be_follows = db.Follows.AsNoTracking().Where(f => f.follower == id).ToList().Count();
            var user = (from _user in listUser
                        where _user.user_id == id
                        select new Detail_User()
                        {
                            Name = _user.user_id,
                            Phone = _user.phone,
                            Image = _user.image,
                            Email = _user.email,
                            Rated = (double)_user.rate / (double)_user.rate_count,
                            Date_join = _user.date_join,
                            follow = follows,
                            be_follow = be_follows,
                            Follower_list = followerList,
                        }).SingleOrDefault();
            return View(user);
        }
        public PartialViewResult Posts(string id)
        {
            User user = (User)Session["User"];
            List<Post> posts = db.Posts.ToList();
            List<Image> images = db.Images.ToList();
            List<Ward> lstWard = db.Wards.ToList();
            var Posts = (from post in posts
                        where post.is_approved == true && post.user_id == id && post.is_hided == false
                        orderby post.date_posted descending
                        select new Post
                        {
                            post_id = post.post_id,
                            user_id = post.user_id,
                            ward_id = post.ward_id,
                            title = post.title,
                            content = post.content,
                            date_posted = post.date_posted,
                            price = post.price,
                            is_hided = post.is_hided,
                            is_approved = post.is_approved,
                            is_seller = post.is_seller,
                            category_id = post.category_id,
                            product_id = post.product_id,
                            seccond_cate_id = post.seccond_cate_id,
                            third_cate_id = post.third_cate_id,
                            hinh = (from image in images
                                    where image.post_id == post.post_id
                                    select image.link).FirstOrDefault(),
                            Ward = (from _ward in lstWard
                                    where _ward.Id == post.ward_id
                                    select _ward).FirstOrDefault(),
                        }).ToList();
            if (user != null)
            {
                List<Favorite> save = db.Favorites.Where(s => s.user_id == user.user_id).ToList();
                ViewData["save"] = save;
            }
            else
            {
                ViewData["save"] = "";

            }
            ViewBag.userid = id;
            return PartialView(Posts);
        }
        public PartialViewResult Hided_Posts()
        {
            User user = GetCurrentUser();
            if (user != null)
            {
                List<Post> posts = db.Posts.ToList();
                List<Image> images = db.Images.ToList();
                List<Ward> lstWard = db.Wards.ToList(); 
                var Posts = (from post in posts
                             where post.is_approved == true && post.user_id == user.user_id && post.is_hided == true
                             orderby post.date_posted descending
                             select new Post
                             {
                                 post_id = post.post_id,
                                 user_id = post.user_id,
                                 ward_id = post.ward_id,
                                 title = post.title,
                                 content = post.content,
                                 date_posted = post.date_posted,
                                 price = post.price,
                                 is_hided = post.is_hided,
                                 is_approved = post.is_approved,
                                 is_seller = post.is_seller,
                                 category_id = post.category_id,
                                 product_id = post.product_id,
                                 seccond_cate_id = post.seccond_cate_id,
                                 third_cate_id = post.third_cate_id,
                                 hinh = (from image in images
                                         where image.post_id == post.post_id
                                         select image.link).FirstOrDefault(),
                                 Ward = (from _ward in lstWard
                                         where _ward.Id == post.ward_id
                                         select _ward).FirstOrDefault(),
                             }).ToList();
                return PartialView(Posts);
            }
            else
            {
                ViewBag.ForbiddenHide = "You have permission to access this!";
                return PartialView("UserContrNullView");
            }
        }
        public ActionResult FollowUser(string that_user_id)
        {
            User temp = GetCurrentUser();
            if (temp != null)
            {
                Follow follow = new Follow();
                follow.user_id = temp.user_id;
                follow.follower = that_user_id;
                db.Follows.Add(follow);
                
                Notification notification = new Notification();
                    notification.user_id = temp.user_id;
                    notification.follow_action = true;
                    notification.request_delivery = false;
                    notification.post_action = false;
                    notification.time = DateTime.Now;
                    notification.is_read = false;
                    notification.reciever = that_user_id;
                    notification.value = notification.user_id + " have followed you!";
                db.Notifications.Add(notification);
                db.SaveChanges();
                return RedirectToAction("ViewUser", "User", new { @id = that_user_id });
            }
            else
            {
                return RedirectToAction("LoginAccount", "Login");
            }
        }
        public ActionResult UnfollowUser(string that_user_id)
        {
            User temp = GetCurrentUser();
            if (temp != null)
            {
                Follow follow = db.Follows.Where(s => s.user_id == temp.user_id && s.follower == that_user_id).FirstOrDefault();
                db.Follows.Remove(follow);
                db.SaveChanges();
                return RedirectToAction("ViewUser", "User", new { @id = that_user_id });
            }
            else
            {
                return RedirectToAction("LoginAccount", "Login");
            }
        }
        public PartialViewResult FollowList(string id)
        {
            return PartialView(db.Follows.Where(f=>f.user_id == id || f.follower == id).ToList());
        }
        public PartialViewResult FollowerList(string id)
        {
            FollowList fl = new FollowList();
            var fo = db.Follows.Where(f => f.user_id == id).ToList();
            List<User> a = new List<User>();
            foreach(var item in fo)
            {
                User tmp = db.Users.Where(u => u.user_id == item.follower).FirstOrDefault();
                if (tmp != null)
                    a.Add(tmp);
                else continue;
            }
            var fower = db.Follows.Where(f => f.follower == id).ToList();
            List<User> b = new List<User>();
            foreach (var item in fower)
            {
                User tmp = db.Users.Where(u => u.user_id == item.user_id).FirstOrDefault();
                if (tmp != null)
                    b.Add(tmp);
                else continue;
            }
            fl.Follow = a.ToList();
            fl.Follower = b.ToList();
            return PartialView(fl);
        }

        public ActionResult EditProfile(string id)
        {
            User tmp = GetCurrentUser();
            if (tmp != null)
                return View(db.Users.Where(s => s.user_id == id).FirstOrDefault());
            else
                return RedirectToAction("LoginAccount", "Login");
        }
        [HttpPost]
        public ActionResult EditProfile(User u, HttpPostedFileBase UploadImage, FormCollection form)
        {
            User temp = new User();
            if (Session["User"] != null)
            {
                temp = (User)Session["User"];
                if (db.Users.AsNoTracking().Where(h => h.user_id == temp.user_id && u.password == temp.password).FirstOrDefault() != null)
                {
                    temp = null;
                }
            }
            if ((string)form["ConfirmPass"] == temp.password)
            {
                //u.user_id = temp.user_id;
                u.permission_id = temp.permission_id;
                u.rate = temp.rate;
                u.rate_count = temp.rate_count;
                u.date_join = temp.date_join;
                u.password = temp.password;
                u.email = temp.email;
                u.Token = null;
                if (u.phone == null)
                { u.phone = temp.phone; }

                if (UploadImage != null)
                {
                    string filename = temp.user_id;
                    string extent = Path.GetExtension(UploadImage.FileName);
                    filename = filename + extent;
                    u.image = "/Content/Images/ava_user/" + filename;
                    UploadImage.SaveAs(Path.Combine(Server.MapPath("/Content/Images/ava_user/"), filename));
                }
                else
                { u.image = temp.image; }
                db.Entry(u).State = EntityState.Modified;
                db.SaveChanges();
                Session["User"] = u;
                Session["User_ID"] = u.user_id;
                return RedirectToAction("ProfileUser", "User", new { @id = u.user_id });
            }
            else
            {
                ViewBag.ConfirmPassWrong = "Your password confirm is not correct!";
                return View();
            }

        }

        public ActionResult HideThisPost(int id)
        {
            User curuser = GetCurrentUser();
            if (curuser == null)
                return Redirect("/Login/LoginAccount");
            else
            {
                Post post = db.Posts.Where(p => p.post_id == id).FirstOrDefault();
                post.is_hided = true;
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return Redirect("/User/ProfileUser/" + curuser.user_id);
            }    
        }
        public ActionResult UnhideThisPost(int id)
        {
            User curuser = GetCurrentUser();
            if (curuser == null)
                return Redirect("/Login/LoginAccount");
            else
            {
                Post post = db.Posts.Where(p => p.post_id == id).FirstOrDefault();
                post.is_hided = false;
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return Redirect("/User/ProfileUser/" + curuser.user_id);
            }
        }
        public ActionResult SavedPost()
        {
            User curuser = GetCurrentUser();
            if (curuser == null)
                return Redirect("/Login/LoginAccount");
            else
            {
                List<Post> posts = db.Posts.ToList();
                List<Favorite> favorites = db.Favorites.ToList();
                List<Image> images = db.Images.ToList();
                List<Ward> lstWard = db.Wards.ToList();
                var listPost = (from post in posts
                               join favo in favorites on post.post_id equals favo.post_id
                               where favo.user_id == curuser.user_id
                               select new Post
                               {
                                   post_id = post.post_id,
                                   user_id = post.user_id,
                                   ward_id = post.ward_id,
                                   title = post.title,
                                   content = post.content,
                                   date_posted = post.date_posted,
                                   price = post.price,
                                   is_hided = post.is_hided,
                                   is_approved = post.is_approved,
                                   is_seller = post.is_seller,
                                   category_id = post.category_id,
                                   product_id = post.product_id,
                                   seccond_cate_id = post.seccond_cate_id,
                                   third_cate_id = post.third_cate_id,
                                   hinh = (from image in images
                                           where image.post_id == post.post_id
                                           select image.link).FirstOrDefault(),
                                   Ward = (from _ward in lstWard
                                           where _ward.Id == post.ward_id
                                           select _ward).FirstOrDefault(),
                               }).ToList();
                return PartialView(listPost);
            }
        }
    }
}