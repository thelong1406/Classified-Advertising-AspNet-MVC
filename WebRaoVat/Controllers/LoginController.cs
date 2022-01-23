using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebRaoVat.Models;

namespace WebRaoVat.Controllers
{
    public class LoginController : Controller
    {
            LoaPhatThanhEntities db = new LoaPhatThanhEntities();
        // GET: LoginUser
        public User GetCurrentUser()
        {
            User currentUser = (User)Session["User"];
            var check = db.Users.AsNoTracking().Where(u => u.user_id == currentUser.user_id && u.password == currentUser.password).FirstOrDefault();
            if (check != null)
                return currentUser;
            else
                return null;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LoginAccount(string returnUrl)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        public ActionResult LoginUser(User _user,string returnUrl)
        {
            var check = db.Users.Where(s => s.user_id == _user.user_id && s.password == _user.password && s.permission_id==3).FirstOrDefault();
            if (check == null)
            {
                ViewBag.errorinfo = "Wrong Info";
                return View("LoginAccount");
            }
            else
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                Session["User"] = check;
                Session["User_ID"] = check.user_id;
                ViewData["countNoti"] = db.Notifications.Where(s => s.reciever == check.user_id&&s.is_read==false).Count();
                if(returnUrl!=null)
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
        }
        public ActionResult RegisterUser()
        {
            User _user = new User();
            _user.image = "~/Content/Images/avatar_default.jpg";
            return View(_user);
        }
        [HttpPost]
        public ActionResult RegisterUser(User _user, FormCollection form, HttpPostedFileBase UploadImage)
        {

            var check = db.Users.AsNoTracking().Where(model => model.user_id == _user.user_id).FirstOrDefault();
            if (check == null)
            {
                check = db.Users.AsNoTracking().Where(model => model.email == _user.email).FirstOrDefault();
                if (check == null)
                {
                    if ((string)form["ConfirmPass"] == _user.password)
                    {
                        db.Configuration.ValidateOnSaveEnabled = true;
                        _user.date_join = DateTime.Now;
                        _user.rate = 0;
                        _user.rate_count = 0;
                        _user.permission_id = 3;
                        if (UploadImage != null)
                        {
                            string filename = _user.user_id;
                            string extent = Path.GetExtension(UploadImage.FileName);
                            filename = filename + extent;
                            _user.image = "~/Content/Images/ava_user/" + filename;
                            UploadImage.SaveAs(Path.Combine(Server.MapPath("/Content/Images/ava_user/"), filename));
                        }
                        else
                            _user.image = "/Content/Images/avatar_default.jpg";
                        db.Users.Add(_user);
                        db.SaveChanges();
                        return RedirectToAction("LoginAccount", "Login");
                    }
                    else
                    {
                        ViewBag.ErrorConfirm = "Your confirm password is not match";
                        return View();
                    }
                }
                else
                {
                    ViewBag.ErrorRegister = "Email already exists";
                    return View();
                }
            }
            else
            {
                ViewBag.ErrorRegister = "Username already exists";
                return View();
            }
        }
        public ActionResult LogOutUser()
        {
            Session.Abandon();
            return RedirectToAction("LoginAccount", "Login");
        }
        public ActionResult ChangePasswordUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePasswordUser(User _user, FormCollection pass)
        {
            User temp = GetCurrentUser();
            string curpassword = pass["pass"].ToString();
            if (curpassword == null)
            {
                ViewBag.NotInputPass = "Where is your current password?";
                return View();
            }
            else
            {
                if (curpassword == temp.password)
                {
                    if (curpassword == _user.password)
                    {
                        ViewBag.WrongNewPassword = "You must input new password!";
                        return View();
                    }
                    else
                    {
                        _user.user_id = temp.user_id;
                        _user.permission_id = temp.permission_id;
                        _user.rate = temp.rate;
                        _user.rate_count = temp.rate_count;
                        _user.date_join = temp.date_join;
                        _user.phone = temp.phone;
                        _user.email = temp.email;
                        _user.image = temp.image;
                        db.Entry(_user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        Session["User"] = db.Users.Where(s => s.user_id == _user.user_id && s.password == _user.password).FirstOrDefault();
                        return RedirectToAction("ProfileUser", "User", new { @id = _user.user_id });
                    }
                }
                else
                {
                    ViewBag.WrongCurPassword = "Your current password not correct!";
                    return View();
                }
            }
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public ActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgetPassword(string email, string urlstr)
        {
            User check = db.Users.Where(s => s.email == email).FirstOrDefault();
            
            if (check != null)
            {
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Template/resetpassword.html"));
                string token = RandomString(36);
                check.Token = token;
                //db.Entry(check).State = System.Data.Entity.EntityState.Modified();
                db.SaveChanges();
                urlstr = urlstr + "/Login/ResetPassword?user_id=" + check.user_id + "&token=" + token ;
                content = content.Replace("{{Link}}", urlstr);
                new MailHelper().SendMail(email, "Reset password", content);
                return View("SendEmailDone");
            }
            else
            {
                ViewBag.Email = "Email does not exist!";
                return View();
            }
        }
        public ActionResult ResetPassword(string user_id, string token)
        {
            var check = db.Users.AsNoTracking().Where(u => u.user_id == user_id && u.Token == token).FirstOrDefault();
            ViewBag.user_id = user_id;
            ViewBag.token = token;
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(string user_id, string token, string password)
        {
            var user = db.Users.Where(s => s.user_id == user_id && s.Token == token).FirstOrDefault();
            user.password = password;
            db.SaveChanges();
            return RedirectToAction("LoginAccount");
        }
    }
}