using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebRaoVat.Models;
using PagedList;
using System.Data.Entity;

namespace WebRaoVat.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Notification
        LoaPhatThanhEntities db = new LoaPhatThanhEntities();
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
        public ActionResult Index(string userID, int? page)
        {
            User user = GetCurrentUser();
            if (user == null)
                return RedirectToAction("LoginAccount", "Login");
            else
            {
                var unreads = db.Notifications.Where(n => n.reciever == userID && n.is_read == false).ToList();
                // lay cac thong bao chua doc
                var pending = (from deal in db.Dealeds.Where(t => t.Post.user_id == userID && t.is_pending == true)
                               select deal.post_id
                         )
                        .ToList();     // loai tru cac thong dang cho xac nhan
                ViewBag.CountUnread = unreads.Count();
                if(unreads.Count()>0)
                {
                    foreach (var item in unreads)
                    {
                        if (pending.Contains(Convert.ToInt32(item.post_id)))
                        {
                            continue;
                            // neu nhu thong bao co bai viet dang duoc cho xac nhan, se khong dc read
                        }
                        item.is_read = true;
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    }
                    db.SaveChanges();
                }
                int pageNumber = (page ?? 1);
                int pageSize = 10;
                var list = db.Notifications.Where(n => n.reciever == userID).OrderByDescending(n => n.is_read == false).ToList();
                if (list.Count() > 0)
                    return View(list.ToPagedList(pageNumber, pageSize));
                else
                {
                    ViewBag.NullNoti = "You haven't any notification yet!";
                    return View("NullNotify");
                }
                    
            }
        }
        // Ham nay se duoc viet trong luc post
        // Du lieu dua vao la User_id + post_id khi trong ham post something
        public JsonResult Noti_PostAction(string userID, int postID)
        {
            List<string> list = new List<string>();
            foreach (var who_follow in db.Follows.Where(f => f.follower == userID).ToList())
            {
                if(who_follow != null)
                    list.Add(who_follow.user_id);
            }
            if(list.Count()> 0)
            {
                foreach(var id in list)
                {
                    Notification noti = new Notification();
                    noti.request_delivery = false;
                    noti.post_action = true;
                    noti.follow_action = false;
                    noti.post_id = postID;
                    noti.time = DateTime.Now;
                    noti.user_id = userID;
                    noti.value = noti.user_id + "have posted this. Click to see detail!";
                    noti.is_read = false;
                    noti.reciever = id;
                    db.Notifications.Add(noti);
                }
                db.SaveChanges();
            }
            return Json(new
            {
                status = true
            });
        }
        public ActionResult ClearAllRead(string userID)
        {
            var reads = db.Notifications.Where(n => n.reciever == userID && n.is_read == true).ToList();
            foreach(var item in reads)
            {
                db.Notifications.Remove(item);
            }
            db.SaveChanges();
            return RedirectToAction("Index","Notification", new { userID = userID });
        }
    }
}