using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data;
using WebRaoVat.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;
//For rescale image
using System.Drawing;
using Image = System.Drawing.Image;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

namespace WebRaoVat.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        LoaPhatThanhEntities db = new LoaPhatThanhEntities();
        public User GetCurrentAdmin()
        {
            var _admin = (User)Session["Admin"];
            if (_admin != null)
            {
                var check = db.Users.Where(a => a.user_id == _admin.user_id && a.password == _admin.password).FirstOrDefault();
                if (check != null && check.permission_id == 1)
                {
                    return (User)_admin;
                }
                else
                    return null;
            }
            return null;

        }
        public ActionResult Index()
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
                return View();
            else
                return RedirectToAction("Login", "Admin");
        }
        [HttpGet]
        public PartialViewResult Login()
        {
            return PartialView();
        }
        public ActionResult Login(User _user)
        {
            var check = db.Users.Where(a => a.user_id == _user.user_id && a.password == _user.password).FirstOrDefault();
            if (check != null && (check.permission_id == 1 || check.permission_id == 2))
            {   
                Session["Admin"] = (User)check;
                Session["Admin_ID"] = check.user_id;
                return RedirectToAction("Index", "Admin");
            }
            else
                return View();
        }
        public ActionResult LogOutAdmin()
        {
            Session.Abandon();
            return Redirect("/admin/login");
        }
        #region CRUD for Users 

        [HttpGet]
        public ActionResult ManageUser(int? page, string sortOrder, string searchStr, string currentFilter)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.DateSort = sortOrder == "Date" ? "date_desc" : "Date";
                if (searchStr != null)
                {
                    page = 1;
                    searchStr.ToLower().Replace(" ", string.Empty);
                }
                else
                {
                    searchStr = currentFilter;
                }
                ViewBag.CurrentFilter = searchStr;
                var users = db.Users.Where(user => user.permission_id == 3);
                if (!String.IsNullOrEmpty(searchStr))
                {
                    users = users.Where(s => s.user_id.ToLower().Replace(" ", string.Empty).Contains(searchStr)
                                           || s.email.ToLower().Replace(" ", string.Empty).Contains(searchStr));
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        users = users.OrderByDescending(s => s.user_id);
                        break;
                    case "Date":
                        users = users.OrderBy(s => s.date_join);
                        break;
                    case "date_desc":
                        users = users.OrderByDescending(s => s.date_join);
                        break;
                    default:
                        users = users.OrderBy(s => s.user_id);
                        break;
                }
                int pageNumber = (page ?? 1);
                int pageSize = 10;
                //permission_id = 3 => user binh thuong
                return View(users.ToPagedList(pageNumber, pageSize));
            }
            else
                return RedirectToAction("Login", "Admin");
        }

        //Get permission list vao ViewBag roi return no ra
        public SelectList PermissionList()
        {
            ViewBag.permission_id = new SelectList(db.Permissions.ToList().OrderBy(p => p.permission1), "permission_id", "permission1");
            return ViewBag.permission_id;
        }

        public ActionResult CreateUser()
        {
            ViewBag.permission_id = PermissionList();
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                User _user = new User();
                return View(_user);
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        [HttpPost]
        public ActionResult CreateUser(User _user)
        {
            ViewBag.permission_id = PermissionList();
            var check = db.Users.Where(u => u.user_id == _user.user_id).FirstOrDefault();
            if (check == null)
            {
                check = db.Users.Where(u => u.email == _user.email).FirstOrDefault();
                if(check == null)
                {
                    if ((_user.rate / _user.rate_count) > 5)
                    {
                        ViewBag.Rate = "Rate/5 >= Rate Count bruh!";
                        return View();
                    }
                    if (_user.date_join == null)
                    {
                        _user.date_join = DateTime.Now;
                    }
                    _user.image = "/Content/Images/avatar_default.jpg";
                    db.Users.Add(_user);
                    db.SaveChanges();
                    return RedirectToAction("ManageUser", "Admin");
                }
                else
                {
                    ViewBag.Exist = "This email have been already in used!";
                    return View();
                }
            }
            else
            {
                ViewBag.Exist = "This username have been already in used!";
                return View();
            }
        }

        public ActionResult EditUser(string id)
        {
            ViewBag.permission_id = PermissionList();
            Session["SaveInfoUserForEdit"] = db.Users.Where(u => u.user_id == id).FirstOrDefault();
            return View(db.Users.Where(u => u.user_id == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditUser(User _user, HttpPostedFileBase UploadImage)
        {
            ViewBag.permission_id = PermissionList();
            User temp = GetCurrentAdmin();
            User user_temp = (User)Session["SaveInfoUserForEdit"];
            if (temp != null)
            {
                if ((_user.rate / _user.rate_count) > 5)
                {
                    ViewBag.Rate = "Rate/5 >= Rate Count bruh!";
                    return View();
                }
                // Truong hop de nguyen khong thay doi thong tin user
                if (_user.password == null)
                {
                    _user.password = user_temp.password;
                }

                if (_user.date_join == null)
                {
                    _user.date_join = user_temp.date_join;
                }

                if (UploadImage == null)
                { _user.image = temp.image; }
                else
                {
                    string fileName = _user.user_id;
                    string extent = Path.GetExtension(UploadImage.FileName);
                    _user.image = "/Content/Images/ava_user/" + fileName + extent;
                    var path = Path.Combine(Server.MapPath("/Content/Images/ava_user/"), fileName);
                    UploadImage.SaveAs(path);
                }
                db.Entry(_user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ManageUser", "Admin");
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        public ActionResult DeleteUser(string id)
        {

            return View(db.Users.Where(u => u.user_id == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult DeleteUser(string id, User _user)
        {
            User tmp = GetCurrentAdmin();
            if (tmp != null)
            {
                if (tmp.permission_id != 1)
                    return RedirectToAction("Error");
                else
                {
                    _user = db.Users.Where(u => u.user_id == id).FirstOrDefault();
                    // Xoa user trong bang follow
                    foreach (var item in db.Follows)
                    {
                        if (item.user_id == _user.user_id || item.follower == _user.user_id)
                            db.Follows.Remove(item);
                    }
                    foreach (var item in db.Favorites)
                    {
                        if (item.user_id == _user.user_id)
                            db.Favorites.Remove(item);
                    }
                    foreach (var item in db.Notifications)
                    {
                        if (item.user_id == _user.user_id)
                        {
                            db.Notifications.Remove(item);
                        }
                    }
                    //foreach (var item in db.PostRecomendeds)
                    //{
                    //    if (item.user_id == _user.user_id)
                    //        db.PostRecomendeds.Remove(item);
                    //}
                    // Delete posts: Đợi làm xong mục xóa post sẽ truyền hàm vào đây cho đỡ dài code
                    db.Users.Remove(_user);
                    db.SaveChanges();
                    return RedirectToAction("ManageUser", "Admin");
                }
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        #endregion //End Crud user // Con phan deleta user and posts of user

        #region CRUD for Permission
        [HttpGet]
        public ActionResult ManagePermission(int? page)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                int pageNumber = (page ?? 1);
                int pageSize = 10;
                return View(db.Permissions.ToList().OrderBy(u => u.permission_id).ToPagedList(pageNumber, pageSize));
            }
            else
                return RedirectToAction("Login", "Admin");
        }

        public ActionResult CreatePermission()
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                Permission p = new Permission();
                return View(p);
            }
            else
                return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult CreatePermission(Permission p)
        {
            if (db.Permissions.Where(per => per.permission1 == p.permission1).FirstOrDefault() == null)
            {
                db.Permissions.Add(p);
                db.SaveChanges();
                return RedirectToAction("ManagePermission");
            }
            else
            {
                ViewBag.Permission = "This permission have been already exist!";
                return View();
            }

        }
        public ActionResult EditPermission(int id)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                return View(db.Permissions.Where(p => p.permission_id == id).FirstOrDefault());
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        [HttpPost]
        public ActionResult EditPermission(Permission p)
        {
            if (db.Permissions.Where(per => per.permission1 == p.permission1).FirstOrDefault() == null)
            {
                db.Entry(p).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ManagePermission");
            }
            else
            {
                ViewBag.Permission = "This permission have been already exist!";
                return View();
            }
        }
        public ActionResult DeletePermission(int id)
        {
            User temp = GetCurrentAdmin();

            if (temp != null)
            {
                if (temp.permission_id == 1)
                    return View(db.Permissions.Where(p => p.permission_id == id).FirstOrDefault());
                else
                    return View("Error");
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        [HttpPost]
        public ActionResult DeletePermission(int id, Permission p)
        {
            if (id != 1)
            {
                p = db.Permissions.Where(per => per.permission_id == id).FirstOrDefault();
                User tmp = db.Users.Where(u => u.permission_id == p.permission_id).FirstOrDefault();
                if (tmp != null)
                {
                    ViewBag.PermissionInUsed = "This permission is current in used by some users!";
                    return View();
                }
                else
                {
                    db.Permissions.Remove(p);
                    db.SaveChanges();
                    return RedirectToAction("ManagePermission");
                }
            }
            else
            {
                ViewBag.PermissionAdmin = "This permission can't be delete";
                return View();
            }
        }

        #endregion //CRUD for Permisson

        #region Rescale Image
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        #endregion

        #region CRUD BRANDs
        [HttpGet]
        public ActionResult ManageBrands(int? page, string sortOrder, string searchStr, string currentFilter)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                if (searchStr != null)
                {
                    page = 1;
                    searchStr.ToLower();
                }
                else
                {
                    searchStr = currentFilter;
                }            
                ViewBag.CurrentFilter = searchStr;
                var brands = db.Brands.ToList();
                if (!String.IsNullOrEmpty(searchStr))
                {
                    brands = brands.Where(s => s.brand1.ToLower().Contains(searchStr)).ToList();
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        brands = brands.OrderByDescending(s => s.brand1).ToList();
                        break;
                    default:
                        brands = brands.OrderBy(s => s.brand1).ToList();
                        break;
                }
                int pageNumber = (page ?? 1);
                int pageSize = 10;
                //permission_id = 3 => user binh thuong
                return View(brands.ToPagedList(pageNumber, pageSize));
            }
            else
                return RedirectToAction("Login", "Admin");
        }

        public ActionResult CreateBrand()
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                Brand brand = new Brand();
                brand.logo = "~/Content/Images/null-image.png";
                return View(brand);
            }
            else
                return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult CreateBrand(Brand _brand, HttpPostedFileBase Logo)
        {
            if (db.Brands.Where(b => b.brand_id == _brand.brand_id).FirstOrDefault() == null)
            {
                if (ModelState.IsValid)
                {
                    //For rescale anh
                    Image _logo = Image.FromStream(Logo.InputStream, true, true);
                    var raito = _logo.Width / 100;
                    Bitmap bitmap = ResizeImage(_logo, _logo.Width / raito, _logo.Height / raito);

                    string fileName = _brand.brand_id;
                    string extent = Path.GetExtension(Logo.FileName);
                    _brand.logo = "/Content/Images/brand/" + fileName + extent;
                    var path = Path.Combine(Server.MapPath("/Content/Images/brand/"), fileName + extent);
                    bitmap.Save(path, ImageFormat.Png);
                    //

                    db.Brands.Add(_brand);
                    db.SaveChanges();
                    return RedirectToAction("ManageBrands");
                }
                else
                    return View();
            }
            else
            {
                ViewBag.Permission = "This permission have been already exist!";
                return View();
            }

        }
        public ActionResult EditBrand(string id)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                return View(db.Brands.Where(p => p.brand_id == id).FirstOrDefault());
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        [HttpPost]
        public ActionResult EditBrand(string id, Brand _brand, HttpPostedFileBase Logo)
        {
            Brand temp = db.Brands.Where(b => b.brand_id == id).FirstOrDefault();
            if (_brand.brand1 == null)
            { _brand.brand1 = temp.brand1; }
            if (Logo == null)
            {
                _brand.logo = temp.logo;
            }
            else
            {
                Image _logo = Image.FromStream(Logo.InputStream, true, true);
                var raito = _logo.Width / 100;
                Bitmap bitmap = ResizeImage(_logo, _logo.Width / raito, _logo.Height / raito);

                string fileName = _brand.brand_id;
                string extent = Path.GetExtension(Logo.FileName);
                _brand.logo = "/Content/Images/brand/" + fileName + extent;
                var path = Path.Combine(Server.MapPath("/Content/Images/brand/"), fileName + extent);
                bitmap.Save(path, ImageFormat.Png);
            }
            db.Entry(_brand).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ManageBrands");
        }
        public ActionResult DeleteBrand(string id)
        {
            id = id.Trim(' ');
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                if (temp.permission_id == 1)
                    return View(db.Brands.Where(p => p.brand_id == id).FirstOrDefault());
                else
                    return View("Error");
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        [HttpPost]
        public ActionResult DeleteBrand(string id, Brand _brand)
        {
            _brand = db.Brands.Where(t => t.brand_id == id).FirstOrDefault();
            foreach (var item in db.BrandSelects)
            {
                if (item.brand_id == _brand.brand_id)
                {
                    db.BrandSelects.Remove(item);
                }
            }
            db.Brands.Remove(_brand);
            db.SaveChanges();
            return RedirectToAction("ManageBrands");
        }
        #endregion

        #region CRUD category
        public ActionResult ManageCategories(int? page, string sortOrder)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

                var categories = db.Categories.ToList();
                switch (sortOrder)
                {
                    case "name_desc":
                        categories = categories.OrderByDescending(s => s.category1).ToList();
                        break;
                    default:
                        categories = categories.OrderBy(s => s.category1).ToList();
                        break;
                }
                int pageNumber = (page ?? 1);
                int pageSize = 10;
                return View(categories.ToPagedList(pageNumber, pageSize));
            }
            else
                return RedirectToAction("Login", "Admin");
        }

        public ActionResult CreateCategory()
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                Category cate = new Category();
                cate.image = "~/Content/Images/null-image.png";
                return View(cate);
            }
            else
                return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult CreateCategory(Category _cate, HttpPostedFileBase Upload)
        {
            int tmp = db.Categories.AsNoTracking().Count();
            if (Upload == null)
            {
                ViewBag.NullImage = "Where is your fuking image!";
                return View(_cate);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    //For rescale anh
                    Image _logo = Image.FromStream(Upload.InputStream, true, true);
                    var raito = _logo.Width / 150; // scale width = 150px
                    Bitmap bitmap = ResizeImage(_logo, _logo.Width / raito, _logo.Height / raito);

                    string fileName = "category_" + (tmp + 1);
                    string extent = Path.GetExtension(Upload.FileName);
                    _cate.image = "/Content/Images/brand/" + fileName + extent;
                    var path = Path.Combine(Server.MapPath("/Content/Images/brand/"), fileName + extent);
                    bitmap.Save(path, ImageFormat.Png);
                    //
                    db.Categories.Add(_cate);
                    db.SaveChanges();
                    return RedirectToAction("ManageCategories");
                }
                else
                    return View();
            }
        }
        public ActionResult EditCategory(int id)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                return View(db.Categories.Where(p => p.category_id == id).FirstOrDefault());
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        [HttpPost]
        public ActionResult EditCategory(int id, Category _cate, HttpPostedFileBase Upload)
        {
            var temp = db.Categories.AsNoTracking().Where(c => c.category_id == id).FirstOrDefault();
            int tmp = db.Categories.AsNoTracking().Count();
            var check = db.Categories.AsNoTracking().Where(c => c.category1 == _cate.category1).FirstOrDefault();
            if (check != null)
            {
                ViewBag.CateAlready = "Databse have alreay this category!";
                return View(_cate);
            }
            else
            {
                if (Upload == null)
                {
                    _cate.image = temp.image;
                }
                else
                {
                    Image _logo = Image.FromStream(Upload.InputStream, true, true);
                    var raito = _logo.Width / 100;
                    Bitmap bitmap = ResizeImage(_logo, _logo.Width / raito, _logo.Height / raito);

                    string fileName = "category_" + (tmp + 1);
                    string extent = Path.GetExtension(Upload.FileName);
                    _cate.image = "/Content/Images/brand/" + fileName + extent;
                    var path = Path.Combine(Server.MapPath("/Content/Images/brand/"), fileName + extent);
                    bitmap.Save(path, ImageFormat.Png);
                }
                db.Entry(_cate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ManageCategories");
            }

        }
        public ActionResult DeleteCategory(int id)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                if (temp.permission_id == 1)
                    return View(db.Categories.Where(p => p.category_id == id).FirstOrDefault());
                else
                    return View("Error");
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        [HttpPost]
        public ActionResult DeleteCategory(int id, Category _cate)
        {
            _cate = db.Categories.Where(t => t.category_id == id).FirstOrDefault();
            foreach (var item in db.SeccondCategories)
            {
                if (item.category_id == _cate.category_id)
                {
                    foreach (var t in db.ThirdCategories)
                    {
                        if (t.seccond_cate_id_1 == item.seccond_cate_id)
                        {
                            db.ThirdCategories.Remove(t);
                        }
                    }
                    db.SeccondCategories.Remove(item);
                }
            }
            db.Categories.Remove(_cate);
            db.SaveChanges();
            return RedirectToAction("ManageCategories");
        }
        #endregion // END category

        #region CRUD 2ND Category
        public SelectList CategoryList()
        {
            ViewBag.category_id = new SelectList(db.Categories.ToList().OrderBy(p => p.category_id), "category_id", "category1");
            return ViewBag.category_id;
        }
        public ActionResult Manage2ndCate(int idcate, int? page, string sortOrder)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

                var _2ndcate = db.SeccondCategories.Where(c=>c.Category.category_id == idcate).ToList();
                switch (sortOrder)
                {
                    case "name_desc":
                        _2ndcate = _2ndcate.OrderByDescending(s => s.seccond_cate).ToList();
                        break;
                    default:
                        _2ndcate = _2ndcate.OrderBy(s => s.seccond_cate).ToList();
                        break;
                }
                int pageNumber = (page ?? 1);
                int pageSize = 15;
                return View(_2ndcate.ToPagedList(pageNumber, pageSize));
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        public ActionResult Create2ndCate()
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                ViewBag.category_id = CategoryList();
                SeccondCategory s_cate = new SeccondCategory();
                return View(s_cate);
            }
            else
                return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult Create2ndCate(SeccondCategory s_cate)
        {
            var check = db.SeccondCategories.AsNoTracking().Where(c => c.seccond_cate_id == s_cate.seccond_cate_id).FirstOrDefault();
            if (check != null)
            {
                ViewBag.CateAlready = "Databse have alreay this category!";
                return View(s_cate);
            }
            else {
                if (ModelState.IsValid)
                {
                    ViewBag.category_id = CategoryList();
                    db.SeccondCategories.Add(s_cate);
                    db.SaveChanges();
                    return RedirectToAction("Manage2ndCate", "Admin", new { idcate = s_cate.category_id });
                }
                else
                    return View();
            }
        }

        public ActionResult Edit2ndCate(int id)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                ViewBag.category_id = CategoryList();
                return View(db.SeccondCategories.Where(p => p.seccond_cate_id == id).FirstOrDefault());
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        [HttpPost]
        public ActionResult Edit2ndCate(int id, SeccondCategory _cate)
        {
            ViewBag.category_id = CategoryList();
            var check = db.SeccondCategories.AsNoTracking().Where(c => c.seccond_cate == _cate.seccond_cate).FirstOrDefault();
            if (check != null)
            {
                ViewBag.CateAlready = "Databse have alreay this category!";
                return View(_cate);
            }
            else
            {
                db.Entry(_cate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Manage2ndCate", "Admin", new { idcate = _cate.category_id });
            }

        }

        public ActionResult Delete2ndCate(int id)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                if (temp.permission_id == 1)
                    return View(db.SeccondCategories.Where(p => p.seccond_cate_id == id).FirstOrDefault());
                else
                    return View("Error");
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        [HttpPost]
        public ActionResult Delete2ndCate(int id, SeccondCategory _scate)
        {
            _scate = db.SeccondCategories.Where(t => t.category_id == id).FirstOrDefault();
            foreach (var item in db.ThirdCategories)
            {
                if (item.seccond_cate_id_1 == _scate.seccond_cate_id)
                {

                    db.ThirdCategories.Remove(item);
                }
            }
            db.SeccondCategories.Remove(_scate);
            db.SaveChanges();
            return RedirectToAction("Manage2ndCate", "Admin", new { idcate = _scate.category_id });
        }
        #endregion

        public SelectList _2ndCateList()
        {
            ViewBag.seccond_cate_id_1 = new SelectList(db.SeccondCategories.ToList().OrderBy(p => p.seccond_cate_id), "seccond_cate_id", "seccond_cate");
            return ViewBag.seccond_cate_id_1;
        }
        #region CRUD 3ND Category
        public ActionResult Manage3ndCate(int id2nd, int? page, string sortOrder)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

                var _3rdcategories = db.ThirdCategories.Where(s=>s.seccond_cate_id_1 == id2nd).ToList();
                if (_3rdcategories.Count() == 0)
                {
                    var id = db.SeccondCategories.Where(s => s.seccond_cate_id == id2nd).FirstOrDefault().category_id;
                    return Redirect("/Admin/Manage2ndCate?idcate=" + id);
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        _3rdcategories = _3rdcategories.OrderByDescending(s => s.third_cate).ToList();
                        break;
                    default:
                        _3rdcategories = _3rdcategories.OrderBy(s => s.third_cate).ToList();
                        break;
                }  
                int pageNumber = (page ?? 1);
                int pageSize = 15;
                return View(_3rdcategories.ToPagedList(pageNumber, pageSize));
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        public ActionResult Create3rdCate()
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                ViewBag.seccond_cate_id_1 = _2ndCateList();
                ThirdCategory s_cate = new ThirdCategory();
                return View(s_cate);
            }
            else
                return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult Create3rdCate(ThirdCategory s_cate)
        {
            ViewBag.seccond_cate_id_1 = _2ndCateList();
            var check = db.ThirdCategories.AsNoTracking().Where(c => c.third_cate_id == s_cate.third_cate_id).FirstOrDefault();
            if (check != null)
            {
                ViewBag.CateAlready = "Databse have alreay this category!";
                return View(s_cate);
            }
            else
            {
                db.ThirdCategories.Add(s_cate);
                db.SaveChanges();
                return RedirectToAction("Manage3ndCate", "Admin", new { id2nd = s_cate.seccond_cate_id_1 });
            }
        }

        public ActionResult Edit3rdCate(int id)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                ViewBag.seccond_cate_id_1 = _2ndCateList();
                return View(db.ThirdCategories.Where(p => p.third_cate_id == id).FirstOrDefault());
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        [HttpPost]
        public ActionResult Edit3rdCate(int id, ThirdCategory _cate)
        {
            ViewBag.seccond_cate_id_1 = _2ndCateList();
            var check = db.ThirdCategories.AsNoTracking().Where(c => c.third_cate == _cate.third_cate && c.seccond_cate_id_1 == _cate.seccond_cate_id_1).FirstOrDefault();
            if (check != null)
            {
                ViewBag.CateAlready = "Databse have alreay this category!";
                return View(_cate);
            }
            else
            {
                db.Entry(_cate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Manage3ndCate", "Admin", new { id2nd = _cate.seccond_cate_id_1 });
            }
        }

        public ActionResult Delete3rdCate(int id)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                if (temp.permission_id == 1)
                    return View(db.ThirdCategories.Where(p => p.third_cate_id == id).FirstOrDefault());
                else
                    return View("Error");
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        [HttpPost]
        public ActionResult Delete3rdCate(int id, ThirdCategory _scate)
        {
            _scate = db.ThirdCategories.Where(t => t.third_cate_id == id).FirstOrDefault();
            db.ThirdCategories.Remove(_scate);
            db.SaveChanges();
            return RedirectToAction("Manage3ndCate", "Admin", new { id2nd = _scate.seccond_cate_id_1 });
        }
        #endregion


        #region CRUD atribute
        public ActionResult ManageAtribute(int? page, string sortOrder, string searchStr, string currentFilter)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                if (searchStr != null)
                {
                    page = 1;
                    searchStr = searchStr.ToLower().Replace(" ", string.Empty);
                }
                else
                {
                    searchStr = currentFilter;
                }
                ViewBag.CurrentFilter = searchStr;
                var atributes = db.Atributes.ToList();
                if (!String.IsNullOrEmpty(searchStr))
                {
                    atributes = atributes.Where(s => s.atribute_name.ToLower().Replace(" ", string.Empty).Contains(searchStr)
                                           || s.Category.category1.ToLower().Replace(" ", string.Empty).Contains(searchStr)).ToList();
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        atributes = atributes.OrderByDescending(s => s.atribute_name).ToList();
                        break;
                    default:
                        atributes = atributes.OrderBy(s => s.atribute_name).ToList();
                        break;
                }
                int pageNumber = (page ?? 1);
                int pageSize = 15;
                return View(atributes.ToPagedList(pageNumber, pageSize));
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        public ActionResult CreateAtribute()
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                ViewBag.category_id = CategoryList();
                Atribute atr = new Atribute();
                return View(atr);
            }
            else
                return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult CreateAtribute(Atribute atribute)
        {
            ViewBag.category_id = CategoryList();
            var check = db.Atributes.AsNoTracking().Where(c => c.atribute_name == atribute.atribute_name).FirstOrDefault();
            if (check != null)
            {
                ViewBag.CateAlready = "Databse have alreay this category!";
                return View(atribute);
            }
            else
            {
                db.Atributes.Add(atribute);
                db.SaveChanges();
                return RedirectToAction("ManageAtribute", "Admin");
            }
        }

        public ActionResult EditAtribute(int id)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                ViewBag.category_id = CategoryList();
                return View(db.Atributes.Where(p => p.atribute_id == id).FirstOrDefault());
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        [HttpPost]
        public ActionResult EditAtribute(int id, Atribute atribute)
        {
            ViewBag.category_id = CategoryList();
            var check = db.Atributes.AsNoTracking().Where(c => c.atribute_name == atribute.atribute_name && c.category_id == atribute.category_id).FirstOrDefault();
            if (check != null)
            {
                ViewBag.AlreadyAtr = "This atribute is already exists!";
                return View(atribute);
            }
            else
            {
                db.Entry(atribute).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ManageAtribute", "Admin");
            }
        }

        public ActionResult DeleteAtribute(int id)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                if (temp.permission_id == 1)
                    return View(db.Atributes.Where(p => p.atribute_id == id).FirstOrDefault());
                else
                    return View("Error");
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        [HttpPost]
        public ActionResult DeleteAtribute(int id, Atribute atribute)
        {
            atribute = db.Atributes.Where(t => t.atribute_id == id).FirstOrDefault();
            db.Atributes.Remove(atribute);
            db.SaveChanges();
            return RedirectToAction("ManageAtribute", "Admin");
        }
        #endregion // End region crud atribute

        #region CRUD for Product
        public ActionResult ManageProduct(int? page, string sortOrder, string searchStr, string currentFilter)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                if (searchStr != null)
                {
                    page = 1;
                    searchStr.ToLower();
                }
                else
                {
                    searchStr = currentFilter;
                }
                ViewBag.CurrentFilter = searchStr;
                var products = db.Products.ToList();
                if (!String.IsNullOrEmpty(searchStr))
                {
                    var trim = searchStr.ToLower().Replace(" ", string.Empty);
                    products = products.Where(s =>
                        (s.product1 + s.BrandSelect.Brand.brand1 + s.BrandSelect.SeccondCategory.seccond_cate).ToLower().Replace(" ", string.Empty).Contains(trim) ||
                        (s.BrandSelect.Brand.brand1 + s.product1 + s.BrandSelect.SeccondCategory.seccond_cate).ToLower().Replace(" ", string.Empty).Contains(trim) ||
                        (s.product1 + s.BrandSelect.SeccondCategory.seccond_cate + s.BrandSelect.Brand.brand1).ToLower().Replace(" ", string.Empty).Contains(trim) ||
                        (s.BrandSelect.SeccondCategory.seccond_cate + s.product1 + s.BrandSelect.Brand.brand1).ToLower().Replace(" ", string.Empty).Contains(trim) ||
                        (s.BrandSelect.Brand.brand1 + s.BrandSelect.SeccondCategory.seccond_cate + s.product1).ToLower().Replace(" ", string.Empty).Contains(trim) ||
                        (s.BrandSelect.SeccondCategory.seccond_cate + s.BrandSelect.Brand.brand1 + s.product1).ToLower().Replace(" ", string.Empty).Contains(trim)
                    ).ToList();
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        products = products.OrderByDescending(s => s.product1).ToList();
                        break;
                    default:
                        products = products.OrderBy(s => s.product1).ToList();
                        break;
                }
                int pageNumber = (page ?? 1);
                int pageSize = 15;
                return View(products.ToPagedList(pageNumber, pageSize));     
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        public SelectList _BrandSelect()
        {
            ViewBag.id = new SelectList(db.BrandSelects.ToList().OrderBy(p => p.id), "id", "Brand.brand1");
            var medlist = db.BrandSelects.Select(x => new { id = x.id, Merge = (x.Brand.brand1 + ": \t " + x.SeccondCategory.seccond_cate).ToString() });
            ViewBag.id = new SelectList(medlist, "id", "Merge");
            return ViewBag.id;
        }
        public ActionResult CreateProduct()
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                ViewBag.id = _BrandSelect();
                Product _product = new Product();
                return View(_product);
            }
            else
                return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult CreateProduct(Product _product)
        {
            ViewBag.id = _BrandSelect();
            //if(_product.id != null)
            //{
            //    ViewBag.SelectNull = "You must choose one!";
            //    return View(_product);
            //}
            var check = db.Products.AsNoTracking().Where(c => c.product1 == _product.product1 && c.id == _product.id).FirstOrDefault();
            if (check != null)
            {
                ViewBag.ProAlready = "Databse have alreay this product!";
                return View(_product);
            }
            else
            {
                db.Products.Add(_product);
                db.SaveChanges();
                return RedirectToAction("ManageProduct", "Admin");
            }
        }

        public ActionResult EditProduct(int id)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                ViewBag.id = _BrandSelect();
                return View(db.Products.Where(p => p.product_id == id).FirstOrDefault());
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        [HttpPost]
        public ActionResult EditProduct(int id, Product  _product)
        {
            ViewBag.id = _BrandSelect();
            var check = db.Products.AsNoTracking().Where(c => c.product_id == _product.id && c.id == _product.id).FirstOrDefault();
            if (check != null)
            {
                ViewBag.ProAlready = "This Product is already exists!";
                return View(_product);
            }
            else
            {
                db.Entry(_product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ManageProduct", "Admin");
            }
        }

        public ActionResult DeleteProduct(int id)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                if (temp.permission_id == 1)
                    return View(db.Products.Where(p => p.product_id == id).FirstOrDefault());
                else
                    return View("Error");
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        [HttpPost]
        public ActionResult DeleteProduct(int id, Product product)
        {
            product = db.Products.Where(p => p.product_id == id).FirstOrDefault();
            if (db.Posts.Where(p => p.product_id == id).FirstOrDefault() != null)
            {
                ViewBag.OnUse = "This product on use in some posts!";
                return View(product);
            }
            else
            {
                db.Products.Remove(product);
                db.SaveChanges();
                return RedirectToAction("ManageProduct", "Admin");
            }         
        }
        #endregion

        #region Post approve
        public ActionResult ManagePost(int? page, string sortOrder, string searchStr, string currentFilter)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.TitleSort = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
                ViewBag.DateSort = sortOrder == "Date" ? "date_desc" : "Date";
                ViewBag.PriceSort = sortOrder == "Price" ? "price_desc" : "Price";
                ViewBag.SellerSort = sortOrder == "Seller" ? "buyer" : "Seller";
                if (searchStr != null)
                {
                    page = 1;
                    searchStr = searchStr.ToLower().Replace(" ", string.Empty);
                }
                else
                {
                    searchStr = currentFilter;
                }
                ViewBag.CurrentFilter = searchStr;
                var posts = db.Posts.Where(s => s.is_approved == false);
                if (!String.IsNullOrEmpty(searchStr))
                {
                    //==============================================
                    // 1 title  2 content   3 seccondCate  4 product
                    //==============================================
                    //  1   2   3   4       3   1   2   4
                    //  1   2   4   3       3   1   4   2
                    //  1   4   2   3       3   2   1   4
                    //  1   4   3   2       3   2   4   1
                    //  1   3   4   2       3   4   1   2
                    //  1   3   2   4       3   4   2   1

                    //  2   1   3   4       4   1   2   3
                    //  2   1   4   3       4   1   3   2
                    //  2   3   1   4       4   2   1   3
                    //  2   3   4   1       4   2   3   1
                    //  2   4   1   3       4   3   1   2
                    //  2   4   3   1       4   3   2   1
                    //===============================================
                    #region AUTO COMPLETE  =>> START
                    posts = posts.Where(s =>
                    (s.title + s.content + s.SeccondCategory.seccond_cate + s.Product.product1).ToLower().Replace(" ", string.Empty).Contains(searchStr) || (s.SeccondCategory.seccond_cate + s.title + s.content + s.Product.product1).ToLower().Replace(" ", string.Empty).Contains(searchStr) ||
                    (s.title + s.content + s.Product.product1 + s.SeccondCategory.seccond_cate).ToLower().Replace(" ", string.Empty).Contains(searchStr) || (s.SeccondCategory.seccond_cate + s.title + s.Product.product1 + s.content).ToLower().Replace(" ", string.Empty).Contains(searchStr) ||
                    (s.title + s.Product.product1 + s.content + s.SeccondCategory.seccond_cate).ToLower().Replace(" ", string.Empty).Contains(searchStr) || (s.SeccondCategory.seccond_cate + s.content + s.title + s.Product.product1).ToLower().Replace(" ", string.Empty).Contains(searchStr) ||
                    (s.title + s.Product.product1 + s.SeccondCategory.seccond_cate + s.content).ToLower().Replace(" ", string.Empty).Contains(searchStr) || (s.SeccondCategory.seccond_cate + s.content + s.Product.product1 + s.title).ToLower().Replace(" ", string.Empty).Contains(searchStr) ||
                    (s.title + s.SeccondCategory.seccond_cate + s.content + s.Product.product1).ToLower().Replace(" ", string.Empty).Contains(searchStr) || (s.SeccondCategory.seccond_cate + s.Product.product1 + s.title + s.content).ToLower().Replace(" ", string.Empty).Contains(searchStr) ||
                    (s.title + s.SeccondCategory.seccond_cate + s.Product.product1 + s.content).ToLower().Replace(" ", string.Empty).Contains(searchStr) || (s.SeccondCategory.seccond_cate + s.Product.product1 + s.content + s.title).ToLower().Replace(" ", string.Empty).Contains(searchStr) ||

                    (s.content + s.title + s.SeccondCategory.seccond_cate + s.Product.product1).ToLower().Replace(" ", string.Empty).Contains(searchStr) || (s.Product.product1 + s.title + s.content + s.SeccondCategory.seccond_cate).ToLower().Replace(" ", string.Empty).Contains(searchStr) ||
                    (s.content + s.title + s.Product.product1 + s.SeccondCategory.seccond_cate).ToLower().Replace(" ", string.Empty).Contains(searchStr) || (s.Product.product1 + s.title + s.SeccondCategory.seccond_cate + s.content).ToLower().Replace(" ", string.Empty).Contains(searchStr) ||
                    (s.content + s.SeccondCategory.seccond_cate + s.title + s.Product.product1).ToLower().Replace(" ", string.Empty).Contains(searchStr) || (s.Product.product1 + s.content + s.title + s.SeccondCategory.seccond_cate).ToLower().Replace(" ", string.Empty).Contains(searchStr) ||
                    (s.content + s.SeccondCategory.seccond_cate + s.Product.product1 + s.title).ToLower().Replace(" ", string.Empty).Contains(searchStr) || (s.Product.product1 + s.content + s.SeccondCategory.seccond_cate + s.title).ToLower().Replace(" ", string.Empty).Contains(searchStr) ||
                    (s.content + s.Product.product1 + s.title + s.SeccondCategory.seccond_cate).ToLower().Replace(" ", string.Empty).Contains(searchStr) || (s.Product.product1 + s.SeccondCategory.seccond_cate + s.title + s.content).ToLower().Replace(" ", string.Empty).Contains(searchStr) ||
                    (s.content + s.Product.product1 + s.SeccondCategory.seccond_cate + s.title).ToLower().Replace(" ", string.Empty).Contains(searchStr) || (s.Product.product1 + s.SeccondCategory.seccond_cate + s.content + s.title).ToLower().Replace(" ", string.Empty).Contains(searchStr)
                    );
                    #endregion// AUTO COMPLETE    END <==
                }
                switch (sortOrder)
                {
                    case "title_desc":
                        posts = posts.OrderByDescending(s => s.title);
                        break;
                    case "Date":
                        posts = posts.OrderBy(s => s.date_posted);
                        break;
                    case "date_desc":
                        posts = posts.OrderByDescending(s => s.date_posted);
                        break;
                    case "Price":
                        posts = posts.OrderBy(s => s.price);
                        break;
                    case "price_desc":
                        posts = posts.OrderByDescending(s => s.price);
                        break;
                    case "Seller":
                        posts = posts.OrderBy(s=>s.title).Where(p=>p.is_seller == true);
                        break;
                    case "buyer":
                        posts = posts.OrderBy(s => s.title).Where(p => p.is_seller == false);
                        break;
                    default:
                        posts = posts.OrderBy(s => s.title);
                        break;
                }
                int pageNumber = (page ?? 1);
                int pageSize = 10;
                //permission_id = 3 => user binh thuong
                return View(posts.ToPagedList(pageNumber, pageSize));
            }
            else
                return RedirectToAction("Login", "Admin");
        }
        public ActionResult DetailPost(int id)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
                return View(db.Posts.Where(p => p.post_id == id).FirstOrDefault());
            else
                return RedirectToAction("Login", "Admin");
        }
        public ActionResult ApprovedPost(int id)
        {
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                Post post = db.Posts.Where(p => p.post_id == id).FirstOrDefault();
                post.is_approved = true;
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();

                List<string> list = new List<string>();
                foreach (var who_follow in db.Follows.Where(f => f.follower == post.user_id).ToList())
                {
                    if (who_follow != null)
                        list.Add(who_follow.user_id);
                }
                if (list.Count() != 0)
                {
                    foreach (var that in list)
                    {
                        Notification noti = new Notification();
                        noti.request_delivery = false;
                        noti.post_action = true;
                        noti.follow_action = false;
                        noti.post_id = post.post_id;
                        noti.time = DateTime.Now;
                        noti.user_id = post.user_id;
                        noti.value = noti.user_id + "have posted this. Click to see detail!";
                        noti.is_read = false;
                        noti.reciever = that;
                        db.Notifications.Add(noti);
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("ManagePost","Admin");

            }
            else
                return View("Error");
        }
        public ActionResult DenyPost(int id)
        {
            //@curUrl = Request.Url.ToString();
            User temp = GetCurrentAdmin();
            if (temp != null)
            {
                Post _post = db.Posts.Where(p => p.post_id == id).FirstOrDefault();
                // CHeck post dang duoc giao dich
                if(db.Dealeds.AsNoTracking().Where(d => d.post_id == _post.post_id).FirstOrDefault() != null && db.Dealeds.AsNoTracking().Where(d => d.post_id == _post.post_id).FirstOrDefault().is_delivering == true)
                {

                    ViewBag.PostInDelievering = "Post on delievering!";
                    return View(); 
                }            
                else
                {
                    // Delete from deadled
                    var deals = from deal in db.Dealeds
                                where deal.post_id == _post.post_id
                                select deal;
                    foreach (var item in deals)
                    {
                        db.Dealeds.Remove(item);
                    }
                    //

                    // Delete from favorite post
                    var favoritePosts = from fav in db.Favorites
                                        where fav.post_id == _post.post_id
                                        select fav;
                    foreach (var item in favoritePosts)
                    {
                        db.Favorites.Remove(item);
                    }
                    ///


                    // Delete from thong bao
                    var notis = from noti in db.Notifications
                                where noti.post_id == _post.post_id
                                select noti;
                    foreach (var i in notis)
                    {
                        db.Notifications.Remove(i);
                    }
                    //


                    // Delete from abtribute and value
                    var abtributeAndValues = from i in db.AbtributeAndValues
                                             where i.post_id == _post.post_id
                                             select i;
                    foreach (var i in abtributeAndValues)
                    {
                        db.AbtributeAndValues.Remove(i);
                    }
                    //


                    // Delete from Images
                    var images = from image in db.Images
                                 where image.post_id == _post.post_id
                                 select image;
                    foreach (var i in images)
                    {
                        var count = db.Images.Where(s => s.link == i.link).Count();
                        if (count == 1)
                        {
                            string file = Server.MapPath("~" + i.link);
                            System.IO.File.Delete(file);
                        }

                        db.Images.Remove(i);
                    }
                    
                    db.Posts.Remove(_post);
                    db.SaveChanges();
                    return Redirect("/Admin/ManagePost/");
                }
            }
            else
                return View("Error");
        }
        #endregion
    }
}