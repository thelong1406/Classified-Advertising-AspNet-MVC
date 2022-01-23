using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using WebRaoVat.Models;

namespace WebRaoVat.Controllers
{
    public class RatingController : Controller
    {
        // GET: Rating
        LoaPhatThanhEntities db = new LoaPhatThanhEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RatingUser(string userRate, string ratedUser, string comment, int rating)
        {
            var check = db.RateUsers.AsNoTracking().Where(r => r.who_be_rated_id == ratedUser && r.who_rate_id == userRate).FirstOrDefault();
            if (check == null)
            {
                RateUser new_rating = new RateUser();
                new_rating.who_rate_id = userRate;
                new_rating.who_be_rated_id = ratedUser;
                new_rating.rate = rating;
                User _berated = db.Users.Where(u => u.user_id == ratedUser).FirstOrDefault();
                _berated.rate += rating;
                _berated.rate_count++;
                if(comment == null)
                {
                    switch (rating)
                    {
                        case 1:
                            comment = "Bad!";
                            break; 
                        case 2:
                            comment = "Not Good!";
                            break;
                        case 3:
                            comment = "Average!";
                            break;
                        case 4:
                            comment = "Good!";
                            break;
                        case 5:
                            comment = "This guy is great!";
                            break;
                    }
                }
                new_rating.comment = comment;

                db.RateUsers.Add(new_rating);
                db.Entry(_berated).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ProfileUser", "User", new { id = userRate });
            }
            else
                return RedirectToAction("ProfileUser", "User", new { id = userRate });
        }
        public PartialViewResult ShowListRate(string userID)
        {
            var listRated = db.RateUsers.Where(r => r.who_be_rated_id == userID).ToList();
            if(listRated.Count() > 0)
                return PartialView(listRated);
            else
            {
                ViewBag.EmptyRateList = "There have no rated!";
                return PartialView("NullRating");
            }
        }
    }
}