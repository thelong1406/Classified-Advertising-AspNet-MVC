using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using PagedList;
using PagedList.Mvc;
using System.Web.Mvc;
using WebRaoVat.Models;

namespace WebRaoVat.Controllers
{
    public class HomeController : Controller
    {
        LoaPhatThanhEntities db = new LoaPhatThanhEntities();
        public ActionResult Index()
        {
            List<Post> posts = db.Posts.ToList();
            List<Image> images = db.Images.ToList();

            Cate_Post myCate_Post = new Cate_Post();
            myCate_Post.Categories = db.Categories.ToList();

            myCate_Post.Posts = (from post in posts
                                 where post.is_approved == true && post.is_hided==false
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
                                 }).ToList();
            return View(myCate_Post);
        }

        public ActionResult Category_Post(int category_id = 0, int? page = 1, int second_cate_id = 0, int third_cate_id = 0, int province = 0, int district = 0, int ward = 0, int sortOrder = 3, string search = "")
        {
            User user = (User)Session["User"];

            List<Image> images = db.Images.ToList();
            List<Post> posts = db.Posts.ToList();
            List<Post> lstPosts = new List<Post>();
            List<Ward> lstWard = db.Wards.ToList();

            if (district != 0) province = db.Districts.Where(s => s.Id == district).Select(s => s.ProvinceId).FirstOrDefault();
            if (ward != 0)
            {
                province = db.Wards.Where(s => s.Id == ward).Select(s => s.District.ProvinceId).FirstOrDefault();
                district = db.Wards.Where(s => s.Id == ward).Select(s => s.DistrictID).FirstOrDefault();
            }

            if (category_id == 0 && search != "" && province == 0)
            {
                lstPosts = (from post in posts
                            where post.is_approved == true && post.is_hided == false && post.title.ToLower().Contains(search.ToLower())
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
                                Ward= (from _ward in lstWard
                                       where _ward.Id == post.ward_id
                                       select _ward).FirstOrDefault(),
                            }).ToList();
            }
            else if (category_id == 0 && search != "" && ward != 0)
            {
                lstPosts = (from post in posts
                            where post.is_approved == true && post.is_hided == false && post.title.ToLower().Contains(search.ToLower()) && post.Ward.District.ProvinceId == province && post.Ward.DistrictID == district && post.ward_id == ward
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
            }
            else if (category_id == 0 && search != "" && district != 0)
            {
                lstPosts = (from post in posts
                            where post.is_approved == true && post.is_hided == false && post.title.ToLower().Contains(search.ToLower()) && post.Ward.District.ProvinceId == province && post.Ward.DistrictID == district
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
            }
            else if (category_id == 0 && search != "" && province != 0)
            {
                lstPosts = (from post in posts
                            where post.is_approved == true && post.is_hided == false && post.title.ToLower().Contains(search.ToLower()) && post.Ward.District.ProvinceId == province
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
            }
            else if (category_id == 0 && ward != 0)
            {
                lstPosts = (from post in posts
                            where post.is_approved == true && post.Ward.District.ProvinceId == province && post.Ward.DistrictID == district && post.ward_id == ward
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
            }
            else if (category_id == 0 && district != 0)
            {
                lstPosts = (from post in posts
                            where post.is_approved == true && post.is_hided == false && post.Ward.District.ProvinceId == province && post.Ward.DistrictID == district
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
            }
            else if (category_id == 0 && province != 0)
            {
                lstPosts = (from post in posts
                            where post.is_approved == true && post.is_hided == false && post.Ward.District.ProvinceId == province
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
            }
            else if (second_cate_id == 0 && third_cate_id == 0 && province == 0)
            {
                lstPosts = (from post in posts
                            where post.is_approved == true && post.is_hided == false && post.category_id == category_id && post.title.ToLower().Contains(search.ToLower())
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
            }
            else if (second_cate_id != 0 && third_cate_id == 0 && province == 0)
            {
                lstPosts = (from post in posts
                            where post.is_approved == true && post.is_hided == false && post.category_id == category_id && post.seccond_cate_id == second_cate_id && post.title.ToLower().Contains(search.ToLower())
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
            }
            else if (second_cate_id == 0 && third_cate_id == 0 && ward != 0)
            {
                lstPosts = (from post in posts
                            where post.is_approved == true && post.is_hided == false && post.category_id == category_id && post.Ward.District.ProvinceId == province && post.Ward.DistrictID == district && post.ward_id == ward && post.title.ToLower().Contains(search.ToLower())
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
            }
            else if (second_cate_id == 0 && third_cate_id == 0 && district != 0)
            {
                lstPosts = (from post in posts
                            where post.is_approved == true && post.is_hided == false && post.category_id == category_id && post.Ward.District.ProvinceId == province && post.Ward.DistrictID == district && post.title.ToLower().Contains(search.ToLower())
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
            }
            else if (second_cate_id == 0 && third_cate_id == 0 && province != 0)
            {
                lstPosts = (from post in posts
                            where post.is_approved == true && post.is_hided == false && post.category_id == category_id && post.Ward.District.ProvinceId == province && post.title.ToLower().Contains(search.ToLower())
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
            }
            else if (second_cate_id != 0 && third_cate_id != 0 && province == 0)
            {
                lstPosts = (from post in posts
                            where post.is_approved == true && post.is_hided == false && post.category_id == category_id && post.seccond_cate_id == second_cate_id && post.third_cate_id == third_cate_id && post.title.ToLower().Contains(search.ToLower())
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
            }
            else if (second_cate_id != 0 && third_cate_id == 0 && ward != 0)
            {
                lstPosts = (from post in posts
                            where post.is_approved == true && post.is_hided == false && post.category_id == category_id && post.seccond_cate_id == second_cate_id && post.Ward.District.ProvinceId == province && post.Ward.DistrictID == district && post.ward_id == ward && post.title.ToLower().Contains(search.ToLower())
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
            }
            else if (second_cate_id != 0 && third_cate_id == 0 && district != 0)
            {
                lstPosts = (from post in posts
                            where post.is_approved == true && post.is_hided == false && post.category_id == category_id && post.seccond_cate_id == second_cate_id && post.Ward.District.ProvinceId == province && post.Ward.DistrictID == district && post.title.ToLower().Contains(search.ToLower())
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
            }
            else if (second_cate_id != 0 && third_cate_id == 0 && province != 0)
            {
                lstPosts = (from post in posts
                            where post.is_approved == true && post.is_hided == false && post.category_id == category_id && post.seccond_cate_id == second_cate_id && post.Ward.District.ProvinceId == province && post.title.ToLower().Contains(search.ToLower())
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
            }
            else if (second_cate_id != 0 && third_cate_id != 0 && ward != 0)
            {
                lstPosts = (from post in posts
                            where post.is_approved == true && post.is_hided == false && post.category_id == category_id && post.seccond_cate_id == second_cate_id && post.third_cate_id == third_cate_id && post.Ward.District.ProvinceId == province && post.Ward.DistrictID == district && post.ward_id == ward && post.title.ToLower().Contains(search.ToLower())
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
            }
            else if (second_cate_id != 0 && third_cate_id != 0 && district != 0)
            {
                lstPosts = (from post in posts
                            where post.is_approved == true && post.is_hided == false && post.category_id == category_id && post.seccond_cate_id == second_cate_id && post.third_cate_id == third_cate_id && post.Ward.District.ProvinceId == province && post.Ward.DistrictID == district && post.title.ToLower().Contains(search.ToLower())
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
            }
            else if (second_cate_id != 0 && third_cate_id != 0 && province != 0)
            {
                lstPosts = (from post in posts
                            where post.is_approved == true && post.is_hided == false && post.category_id == category_id && post.seccond_cate_id == second_cate_id && post.third_cate_id == third_cate_id && post.Ward.District.ProvinceId == province && post.title.ToLower().Contains(search.ToLower())
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
            }

            if (sortOrder == 1)
            {
                lstPosts = (from post in lstPosts
                            orderby post.price ascending
                            select post).ToList();
            }
            else if (sortOrder == 2)
            {
                lstPosts = (from post in lstPosts
                            orderby post.price descending
                            select post).ToList();
            }
            else if (sortOrder == 3)
            {
                lstPosts = (from post in lstPosts
                            orderby post.date_posted descending
                            select post).ToList();
            }
            else if (sortOrder == 4)
            {
                lstPosts = (from post in lstPosts
                            orderby post.date_posted ascending
                            select post).ToList();
            }

            ViewData["cate_id"] = category_id;
            ViewData["second_cate_id"] = second_cate_id;
            ViewData["third_cate_id"] = third_cate_id;
            ViewData["provinceid"] = province;
            ViewData["districtid"] = district;
            ViewData["wardid"] = ward;
            ViewData["sortOrder"] = sortOrder;
            ViewData["search"] = search;
            if (user != null)
            {
                List<Favorite> save = db.Favorites.Where(s => s.user_id == user.user_id).ToList();
                ViewData["save"] = save;
            }
            else
            {
                ViewData["save"] = "";

            }

            int pageNumber = (page ?? 1);
            int pageSize = 10;

            return View(lstPosts.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Filter(int category_id, int second_cate_id = 0, int third_cate_id = 0, int province = 0, int district = 0, int ward = 0, int sortOrder = 3, string search = "")
        {
            ViewData["cate_id"] = category_id;
            ViewData["second_cate_id"] = second_cate_id;
            ViewData["third_cate_id"] = third_cate_id;
            ViewData["provinceid"] = province;
            ViewData["districtid"] = district;
            ViewData["wardid"] = ward;
            ViewData["sortOrder"] = sortOrder;
            ViewData["search"] = search;

            return PartialView();
        }
    }
}