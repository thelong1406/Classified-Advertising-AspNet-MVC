using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebRaoVat.Models;
using System.Data.Entity;

namespace WebRaoVat.Controllers
{
    public class DealController : Controller
    {
        // GET: Deal
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
        NotificationController context_noti = new NotificationController();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RequestDelivery(string address, int postID, string author,string brand_pro)
        {
            User _user = GetCurrentUser();
            if (_user != null)
            {
                if(_user.user_id != author)
                {
                    Dealed new_deal = new Dealed();
                    new_deal.is_pending = true;         //Dang cho xac nhan tu nguoi gui
                    new_deal.is_delivering = false;     //Dang duoc van chuyen
                    new_deal.is_dealed = false;         //Da nhan dc hang
                    new_deal.post_id = postID;
                    new_deal.user_id = _user.user_id;
                    DateTime currtime = DateTime.Now;
                    new_deal.deal_date = currtime;
                    db.Dealeds.Add(new_deal);

                    // useeID = Session[user].user_id
                    // postID = post dang xem.post id
                    Notification noti = new Notification();
                    noti.follow_action = false;
                    noti.post_action = false;
                    noti.request_delivery = true;
                    noti.post_id = postID;
                    noti.time = currtime;
                    noti.user_id = _user.user_id;
                    noti.value = "Address: " + address;
                    noti.reciever = author;
                    noti.is_read = false;
                    db.Notifications.Add(noti);
                    db.SaveChanges();
                    return Redirect("/Post/PostDetail/" + postID);
                }
                else
                    return Redirect("/Post/PostDetail/" + postID);
            }
            else
                return RedirectToAction("LoginAccount", "Login");
        }
        public ActionResult AcceptDeal(int postID, string userID, int notiID)
        {
            //user nhan request
            Dealed dealed = db.Dealeds.Where(d => d.post_id == postID && d.is_pending == true && d.user_id == userID).FirstOrDefault();
            string id = dealed.Post.user_id;
            User user = GetCurrentUser();
            if (user != null && user.user_id == id)
            {
                Notification notification = db.Notifications.Where(n => n.notification_id == notiID).FirstOrDefault();
                notification.is_read = true;
                dealed.is_pending = false;
                dealed.is_delivering = true;
                dealed.deal_date = DateTime.Now;
                db.Entry(dealed).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Notification", new { userID = id });
            }
            else
                return RedirectToAction("LoginAccount", "Login");
        }
        public ActionResult DeleteDeal(int postID, string userID, int notiID)
        {
            //user nhan request
            Dealed dealed = db.Dealeds.Where(d => d.post_id == postID && d.user_id == userID && d.is_pending == true).FirstOrDefault();
            string id = dealed.Post.user_id;
            User user = GetCurrentUser();
            if (user != null && user.user_id == id)
            {
                Notification notification = db.Notifications.Where(n => n.notification_id == notiID).FirstOrDefault();
                notification.is_read = true;
                db.Dealeds.Remove(dealed);
                db.SaveChanges();
                return RedirectToAction("Index", "Notification", new { userID = id });
            }
            else
                return RedirectToAction("LoginAccount", "Login");
        }

        public PartialViewResult ShowDelivering(string userID)
        {
            var deliverings = db.Dealeds.Where(d => d.user_id == userID && d.is_delivering == true).ToList();
            if (deliverings.Count > 0)
                return PartialView(deliverings);
            else
            {
                ViewBag.NullDelivery = "You don't have any deal on delievering!";
                return PartialView("Nodelivery");
            }
        }
        public ActionResult DealDone(int _id)
        {
            //user request thuc hien thao tac nay
            Dealed dealed = db.Dealeds.Where(d => d.deal_id == _id).FirstOrDefault();
            User user = GetCurrentUser();
            if(user != null && user.user_id == dealed.user_id)
            {
                dealed.is_delivering = false;
                dealed.is_dealed = true;
                dealed.deal_date = DateTime.Now;
                db.Entry(dealed).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ProfileUser", "User", new { id = dealed.user_id });
            }
            else
            {
                ViewBag.Forbiden = "You don't have permission to access this!";
                return PartialView("Nodelivery");
            }
        }
        public PartialViewResult ShowDealeds(string userID)
        {
            User user = GetCurrentUser();
            if (user != null && user.user_id == userID)
            {
                var dealeds = db.Dealeds.Where(d => d.user_id == userID && d.is_dealed == true).ToList();
                if (dealeds.Count > 0)
                    return PartialView(dealeds);
                else
                {
                    ViewBag.NullDealeds = "You haven't any deaded yet!";
                    return PartialView("Nodelivery");
                }    
            }
            else
            {
                ViewBag.Forbiden = "You don't have permission to access this!";
                return PartialView("Nodelivery");
            }
        }
    }
}