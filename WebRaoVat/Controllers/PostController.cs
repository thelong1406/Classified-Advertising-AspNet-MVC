using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebRaoVat.Models;

namespace WebRaoVat.Controllers
{
    public class PostController : Controller
    {
        LoaPhatThanhEntities db = new LoaPhatThanhEntities();
        // GET: Post
        #region Post
        public ActionResult PostSomething()
        {
            if (Session["User"] != null)
            {
                PostSomething model = new PostSomething();
                model.Posts = db.Posts.ToList();
                model.Brands = db.Brands.ToList();
                model.BrandSelects = db.BrandSelects.ToList();
                model.Atributes = db.Atributes.ToList();
                model.AbtributeAndValues = db.AbtributeAndValues.ToList();
                model.Categories = db.Categories.ToList();
                model.SeccondCategories = db.SeccondCategories.ToList();
                model.ThirdCategories = db.ThirdCategories.ToList();
                model.Images = db.Images.ToList();
                model.count = db.Posts.Max(s => s.post_id);
                return View(model);
            }
            else return RedirectToAction("LoginAccount", "Login");
        }
        [HttpPost]
        public ActionResult Post(Post post)
        {
            post.date_posted = DateTime.Now;
            db.Posts.Add(post);
            db.SaveChanges();
            return Json(Url.Action("Index", "Home"));

        }
        [HttpPost]
        public ActionResult Post_Attribute(string myId, string myValue, int post_id)
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            var listAttributeId = javaScriptSerializer.Deserialize<List<string>>(myId);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var listAttributeValue = serializer.Deserialize<List<string>>(myValue);
            int i = 0;
            AbtributeAndValue abtributeAndValue = new AbtributeAndValue();
            foreach (var item in listAttributeId)
            {
                abtributeAndValue.atribute_id = int.Parse(item);
                abtributeAndValue.post_id = post_id;
                abtributeAndValue.value = listAttributeValue[i++];
                db.AbtributeAndValues.Add(abtributeAndValue);
                db.SaveChanges();
            }

            return Json(Url.Action("Index", "Home"));
        }
        #endregion

        #region LoadData
        [HttpPost]
        public JsonResult Category()
        {
            var category = db.Categories.ToList();
            List<Categories> listCategories = new List<Categories>();
            Categories tmp = null;
            foreach (var item in category)
            {
                tmp = new Categories();
                tmp.category_id = item.category_id;
                tmp.category1 = item.category1;
                tmp.image = item.image;
                listCategories.Add(tmp);
            }
            return Json(new
            {
                data = listCategories,
                status = true
            });
        }
        [HttpPost]
        public JsonResult Second_Cate(int id)
        {
            var second_cate = db.SeccondCategories.Where(s => s.category_id == id).ToList();
            List<SeccondCategories> listSecondCategories = new List<SeccondCategories>();
            SeccondCategories tmp = null;
            foreach (var item in second_cate)
            {
                tmp = new SeccondCategories();
                tmp.seccond_cate_id = item.seccond_cate_id;
                tmp.seccond_cate = item.seccond_cate;
                tmp.category_id = item.category_id;
                listSecondCategories.Add(tmp);
            }
            return Json(new
            {
                data = listSecondCategories,
                status = true
            });
        }
        [HttpPost]
        public JsonResult Third_Cate(int id)
        {
            var thirdCategory = db.ThirdCategories.Where(s => s.seccond_cate_id_1 == id).ToList();
            List<ThirdCategories> listThirdCategories = new List<ThirdCategories>();
            ThirdCategories tmp = null;
            foreach (var item in thirdCategory)
            {
                tmp = new ThirdCategories();
                tmp.seccond_cate_id_1 = item.seccond_cate_id_1;
                tmp.third_cate_id = item.third_cate_id;
                tmp.third_cate = item.third_cate;
                listThirdCategories.Add(tmp);
            }
            return Json(new
            {
                data = listThirdCategories,
                status = true
            });
        }
        [HttpPost]
        public JsonResult GetBrand(int cateId)
        {
            var brandSelect = db.BrandSelects.Where(s => s.seccond_cate_id == cateId).ToList();
            List<BrandSelects> listBrandSelect = new List<BrandSelects>();

            BrandSelects brandSelects = null;
            foreach (var item in brandSelect)
            {
                brandSelects = new BrandSelects();
                brandSelects.id = item.id;
                brandSelects.brand_id = item.brand_id;
                brandSelects.seccond_cate_id = item.seccond_cate_id;
                listBrandSelect.Add(brandSelects);

            }
            List<Brands> listBrand = new List<Brands>();
            Brands brands = null;
            foreach (var item in listBrandSelect)
            {
                brands = new Brands();
                var brand = db.Brands.Where(s => s.brand_id == item.brand_id).FirstOrDefault();
                brands.brand_id = brand.brand_id;
                brands.brand1 = brand.brand1;
                brands.logo = brand.logo;
                listBrand.Add(brands);
            }
            return Json(new
            {
                data = listBrand,
                status = true
            });
        }
        [HttpPost]
        public JsonResult BrandSelect(string brandId, int cateId)
        {
            var brandSelect = db.BrandSelects.Where(s => s.brand_id == brandId && s.seccond_cate_id == cateId).ToList();
            List<BrandSelects> listBrandSelect = new List<BrandSelects>();

            BrandSelects brandSelects = null;

            foreach (var item in brandSelect)
            {
                brandSelects = new BrandSelects();
                brandSelects.id = item.id;
                brandSelects.brand_id = item.brand_id;
                brandSelects.seccond_cate_id = item.seccond_cate_id;
                listBrandSelect.Add(brandSelects);

            }
            return Json(new
            {
                data = listBrandSelect,
                status = true
            });
        }
        [HttpPost]
        public JsonResult LoadProduct(int id)
        {
            var product = db.Products.Where(s => s.id == id).ToList();
            List<Products> listProducts = new List<Products>();
            Products tmp = null;
            foreach (var item in product)
            {
                tmp = new Products();
                tmp.id = item.id;
                tmp.product1 = item.product1;
                tmp.product_id = item.product_id;
                listProducts.Add(tmp);
            }
            return Json(new
            {
                data = listProducts,
                status = true
            });
        }
        [HttpPost]
        public JsonResult LoadAttribute(int id)
        {
            var attribute = db.Atributes.Where(s => s.category_id == id).ToList();
            List<Atributes> listAttributes = new List<Atributes>();
            Atributes tmp = null;
            foreach (var item in attribute)
            {
                tmp = new Atributes();
                tmp.atribute_id = item.atribute_id;
                tmp.atribute_name = item.atribute_name;
                tmp.category_id = item.category_id;
                listAttributes.Add(tmp);
            }
            return Json(new
            {
                data = listAttributes,
                status = true
            });
        }
        public JsonResult LoadProvince()
        {
            var province = db.Provinces.ToList();
            List<Provinces> listProvinces = new List<Provinces>();
            Provinces tmp = null;
            foreach (var item in province)
            {
                tmp = new Provinces();
                tmp.Id = item.Id;
                tmp.Name = item.Name;
                tmp.Type = item.Type;
                listProvinces.Add(tmp);
            }
            return Json(new
            {
                data = listProvinces,
                status = true
            });
        }
        public JsonResult LoadDistrict(int id)
        {
            var district = db.Districts.Where(s => s.ProvinceId == id).ToList();
            List<Districts> listDistricts = new List<Districts>();
            Districts tmp = null;
            foreach (var item in district)
            {
                tmp = new Districts();
                tmp.Id = item.Id;
                tmp.Name = item.Name;
                tmp.Type = item.Type;
                listDistricts.Add(tmp);
            }
            return Json(new
            {
                data = listDistricts,
                status = true
            });
        }
        public JsonResult LoadWard(int id)
        {
            var ward = db.Wards.Where(s => s.DistrictID == id).ToList();
            List<Wards> listWards = new List<Wards>();
            Wards tmp = null;
            foreach (var item in ward)
            {
                tmp = new Wards();
                tmp.Id = item.Id;
                tmp.Name = item.Name;
                tmp.Type = item.Type;
                listWards.Add(tmp);
            }
            return Json(new
            {
                data = listWards,
                status = true
            });
        }
        #endregion

        #region MultiImage
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveToTemp(HttpPostedFileBase file)
        {
            try
            {
                string filename = "";
                string imgepath = "Null";
                if (file != null)
                {
                    filename = file.FileName;
                    imgepath = filename;
                    string extension = Path.GetExtension(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/tmp/"), filename);
                    file.SaveAs(path);
                }
                return Json(filename, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult RemoveFile(string url)
        {
            try
            {
                var path = Path.Combine(Server.MapPath("~/"), url);
                if (System.IO.File.Exists(path))
                {
                    try
                    {
                        System.IO.File.Delete(path);
                        return Json(url, JsonRequestBehavior.AllowGet);

                    }
                    catch (System.IO.IOException e)
                    {
                        return Json(e, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(url, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// This method is used to move files from Temp folder to Destinatin folder.
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveToMainFolder(int id)
        {
            Image image = new Image();
            image.post_id = id;
            string fileName = "";
            string destFile = "";
            string sourcePath = Server.MapPath("~/Content/Images/tmp/");
            string targetPath = Server.MapPath("~/Content/Images/post/");
            if (System.IO.Directory.Exists(sourcePath))
            {
                string[] files = System.IO.Directory.GetFiles(sourcePath);
                // Copy the files. 
                foreach (string file in files)
                {
                    fileName = System.IO.Path.GetFileName(file);
                    destFile = System.IO.Path.Combine(targetPath, fileName);
                    System.IO.File.Copy(file, destFile, true);
                    image.link = "/Content/Images/post/" + fileName;
                    db.Images.Add(image);
                    db.SaveChanges();
                }
                RemoveFiles();
            }
            return Json("All Files saved Successfully.", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Make Temp folder empty once all files are copied to destination folder.
        /// </summary>
        public void RemoveFiles()
        {
            string sourcePath = Server.MapPath("~/Content/Images/tmp/");
            string[] files = System.IO.Directory.GetFiles(sourcePath);
            foreach (string file in files)
            {
                if (System.IO.File.Exists(System.IO.Path.Combine(sourcePath, file)))
                {
                    try
                    {
                        System.IO.File.Delete(file);
                    }
                    catch (System.IO.IOException e)
                    {
                        return;
                    }
                }
            }
        }
        #endregion

        public ActionResult DeletePost(int id, string curUrl) // Hàm này dùng để xử lý, khi code ở user và admin cần có code xác nhận đó là chủ post hoặc là admin
        {
            Post _post = db.Posts.Where(p => p.post_id == id).FirstOrDefault();
            string userID = _post.user_id;
            // CHeck post dang duoc giao dich
            var dealed = db.Dealeds.AsNoTracking().Where(d => d.post_id == _post.post_id).FirstOrDefault();
            if (dealed != null)
            {
                ViewBag.PostDealed = "Post have been deal!";
                return Redirect(curUrl);
            }
            else
            {
                // Delete from favorite post
                List<Favorite> favorites = db.Favorites.Where(s => s.post_id == _post.post_id).ToList();
                foreach (var item in favorites)
                {
                    db.Favorites.Remove(item);
                    db.SaveChanges();
                }
                //

                // Delete from thong bao
                List<Notification> notifications = db.Notifications.Where(s => s.post_id == _post.post_id).ToList();
                foreach (var i in notifications)
                {
                    db.Notifications.Remove(i);
                    db.SaveChanges();
                }
                //


                // Delete from abtributeand value
                List<AbtributeAndValue> abtributeAndValues = db.AbtributeAndValues.Where(s => s.post_id == _post.post_id).ToList();
                foreach (var i in abtributeAndValues)
                {
                    db.AbtributeAndValues.Remove(i);
                    db.SaveChanges();
                }
                //


                // Delete from Images
                List<Image> images = db.Images.Where(s => s.post_id == _post.post_id).ToList();
                foreach (var i in images)
                {
                    var count = db.Images.Where(s => s.link == i.link).Count();
                    if (count == 1)
                    {
                        string file = Server.MapPath("~" + i.link);
                        System.IO.File.Delete(file);
                    }

                    db.Images.Remove(i);
                    db.SaveChanges();
                }

                //
                db.Posts.Remove(_post);
                db.SaveChanges();
                return RedirectToAction("ProfileUser", "User", new { id = userID });
            }
        }

        [HttpGet]
        public ActionResult PostDetail(int id, string userId)
        {
            var checkSave = db.Favorites.Where(s => s.post_id == id && s.user_id == userId).FirstOrDefault();
            if (checkSave == null)
            {
                ViewData["Save"] = 0;
            }
            else
            {
                ViewData["Save"] = 1;
            }
            return View(db.Posts.Where(p => p.post_id == id).FirstOrDefault());
        }

        public ActionResult SavePost(int id, string userid, string urlstr)
        {
            var check = db.Favorites.Where(s => s.post_id == id && s.user_id == userid).FirstOrDefault();
            if (check == null)
            {
                Favorite favorite = new Favorite
                {
                    post_id = id,
                    user_id = userid,
                };
                db.Favorites.Add(favorite);
                db.SaveChanges();
                return Redirect(urlstr);
            }
            else
            {
                db.Favorites.Remove(check);
                db.SaveChanges();
                return Redirect(urlstr);
            }
        }
        #region EditPost
        public ActionResult EditPost(int id)
        {
            var post = db.Posts.Where(s => s.post_id == id).FirstOrDefault();
            ViewData["post"] = post;
            ViewData["Product"] = db.Products.Where(s => s.product_id == post.product_id).FirstOrDefault();
            if (post.third_cate_id != null)
            {
                ViewBag.thirdCateId = post.third_cate_id;
                ViewBag.ThirdName = post.ThirdCategory.third_cate;
            }


            return View();
        }
        public JsonResult EditAttribute(int id)
        {
            var attribute = db.AbtributeAndValues.Where(s => s.post_id == id).ToList();
            List<AttributeVal> listAttributes = new List<AttributeVal>();
            AttributeVal tmp = null;
            foreach (var item in attribute)
            {
                tmp = new AttributeVal();
                tmp.id = item.id;
                tmp.value = item.value;
                tmp.post_id = item.post_id;
                tmp.atribute_id = item.atribute_id;
                tmp.atribute_name = db.Atributes.Where(s => s.atribute_id == item.atribute_id).Select(s => s.atribute_name).FirstOrDefault();
                listAttributes.Add(tmp);
            }
            return Json(new
            {
                data = listAttributes,
                status = true
            });
        }
        [HttpPost]
        public ActionResult UpdatePost(Post post)
        {
            var post_edit = db.Posts.Find(post.post_id);
            post_edit.title = post.title;
            post_edit.content = post.content;
            post_edit.price = post.price;
            post_edit.product_id = post.product_id;
            db.SaveChanges();
            return Json(Url.Action("Index", "Home"));

        }
        [HttpPost]
        public ActionResult Update_Post_Attribute(string myId, string myValue, int post_id)
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            var listAttributeId = javaScriptSerializer.Deserialize<List<string>>(myId);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var listAttributeValue = serializer.Deserialize<List<string>>(myValue);
            int i = 0;
            AbtributeAndValue abtributeAndValue = new AbtributeAndValue();
            foreach (var item in listAttributeId)
            {
                int tmpitem = int.Parse(item);
                var tmp = db.AbtributeAndValues.Where(s => s.post_id == post_id && s.atribute_id == tmpitem).FirstOrDefault();
                var ab_val = db.AbtributeAndValues.Find(tmp.id);

                ab_val.value = listAttributeValue[i++];
                db.SaveChanges();

            }

            return Json(Url.Action("PostDetail", "Post",new {id=post_id }));
        }
        public JsonResult SaveToTmpFolder(int id)
        {
            List<Image> image = db.Images.Where(s => s.post_id == id).ToList();
            List<string> name = new List<string>();
            string fileName = "";
            string destFile = "";
            string targetPath = Server.MapPath("~/Content/Images/tmp/");

            // Copy the files. 
            foreach (var img in image)
            {
                fileName = System.IO.Path.GetFileName(img.link);
                destFile = System.IO.Path.Combine(targetPath, fileName);
                string file = Server.MapPath("~" + img.link);
                System.IO.File.Copy(file, destFile, true);
                name.Add(fileName);
            }

            return Json(new
            {
                data = name,
                status = true
            });
        }
        public JsonResult SaveEditToMainFolder(int id)
        {
            List<Image> images = db.Images.Where(s => s.post_id == id).ToList();
            foreach (var i in images)
            {
                db.Images.Remove(i);
                db.SaveChanges();
            }

            Image image = new Image();
            image.post_id = id;
            string fileName = "";
            string destFile = "";
            string sourcePath = Server.MapPath("~/Content/Images/tmp/");
            string targetPath = Server.MapPath("~/Content/Images/post/");
            if (System.IO.Directory.Exists(sourcePath))
            {
                string[] files = System.IO.Directory.GetFiles(sourcePath);
                // Copy the files. 
                foreach (string file in files)
                {
                    fileName = System.IO.Path.GetFileName(file);
                    destFile = System.IO.Path.Combine(targetPath, fileName);
                    System.IO.File.Copy(file, destFile, true);
                    image.link = "/Content/Images/post/" + fileName;
                    db.Images.Add(image);
                    db.SaveChanges();
                }
                RemoveFiles();
            }
            return Json("All Files saved Successfully.", JsonRequestBehavior.AllowGet);
        }
        #endregion
    }

}