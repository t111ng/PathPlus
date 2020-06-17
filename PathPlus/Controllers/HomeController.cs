using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using PathPlus.Models;
namespace PathPlus.Controllers
{
    public class HomeController : Controller
    {
        PathPlusEntities db = new PathPlusEntities();
        // GET: Home
        public ActionResult Index()
        {
            //起始頁，如果Session["account"]空的表示沒有登入，轉到登入頁面
            if (Session["account"] == null)
            {
                return RedirectToAction("Index", "Login");
            }


            //--------取貼文資料 ---------
            string ID = Session["account"].ToString();

            var post1 = (from p in db.Post
                         where p.MemberID == ID
                         //join pp in db.PostPhoto on p.PostID equals pp.PostID
                         join m in db.Member on p.MemberID equals m.MemberID
                         join c in db.PostCategory on p.CategoryID equals c.CategoryID
                         join s in db.PostStatusCategory on p.StatusCategoryID equals s.StatusCategoryID                    
                         select new { p.PostID, p.PostContent, p.PostDate, p.EditDate, m.MemberName, c.CategoryName, s.StatusCategoryName,m.Photo,p.MemberID});
           
            string[] rid = db.Relationship.Where(m => m.MemberID == ID && m.FollowDate.Year > 1773).Select(m => m.RSMemberID).ToList().ToArray();
            var post2 = (from p in db.Post
                         where rid.Contains(p.MemberID) && p.StatusCategoryID != "2"
                         //join pp in db.PostPhoto on p.PostID equals pp.PostID
                         join m in db.Member on p.MemberID equals m.MemberID
                         join c in db.PostCategory on p.CategoryID equals c.CategoryID
                         join s in db.PostStatusCategory on p.StatusCategoryID equals s.StatusCategoryID
                         select new { p.PostID, p.PostContent, p.PostDate, p.EditDate, m.MemberName, c.CategoryName, s.StatusCategoryName, m.Photo, p.MemberID});
            //var post3 = from p in db.Post
            //            where rid.Contains(p.MemberID) && p.StatusCategoryID != "2"
            //            select new { p.PostID };
            string[] rid2 = db.Post.Where(p => p.StatusCategoryID != "2").Where(p => rid.Contains(p.MemberID)).Select(p => p.PostID).ToList().ToArray();
            string[] rid3 = db.Post.Where(p => p.StatusCategoryID != "2").Where(p => p.MemberID == ID).Select(p => p.PostID).ToList().ToArray();
            var photo1 = from po in db.PostPhoto
                        where rid2.Contains(po.PostID)
                        select new { po.PostID,po.Photo};
            var photo2 = from po in db.PostPhoto
                         where rid3.Contains(po.PostID)
                         select new { po.PostID, po.Photo };
            var post = post1.Union(post2).OrderByDescending(x => x.PostDate).ToList();
            var photo3 = photo2.Union(photo1).OrderByDescending(x => x.PostID).ToList();

            


            ViewBag.post = post.ToList();
            ViewBag.photo = photo3.ToList();
            //ViewBag.comment = comment.ToList();
            //--------取貼文資料 ---------


            //--------取個人帳號---------
            var PersonalAccount = db.Member.Where(m => m.MemberID == ID).Select(m => m.Account).FirstOrDefault();
            ViewBag.personalaccount = PersonalAccount;
            //--------取個人帳號---------

            //--------取個人姓名---------
            var PersonalName = db.Member.Where(m => m.MemberID == ID).Select(m => m.MemberName).FirstOrDefault();
            ViewBag.personalname = PersonalName;
            //--------取個人姓名---------

            //--------個人圖片-----------
            var PersonalPhoto = db.Member.Where(p => p.MemberID == ID).Select(p => p.Photo).FirstOrDefault();
            ViewBag.personalphoto = PersonalPhoto;
            //--------個人圖片-----------

            //-------提取社團資訊----------(Session["account"].ToString()
            
            var jp = db.JoinGroup.Where(j => j.MemberID == ID).Select(j => j.GroupID).ToList().ToArray();
            var group = db.Group.Where(g=>g.PrivateCategoryID == "0").Where(g=> !jp.Contains(g.GroupID)).Take(5);
            ViewBag.group = group.ToList();
            //-------提取社團資訊----------


            return View();


        }
        
        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Private()
        {
            return View();
        }

        public ActionResult Terms()
        {
            return View();
        }

        public ActionResult NewPost()
        {

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPost(string Pcontent, string CID, string SCID, HttpPostedFileBase[] photo)
        //
        {



            Post newpost = new Post();
            PostPhoto newphoto = new PostPhoto();

            SelfFeature sf = new SelfFeature();
            string PID = sf.GetID("Post");

            string fileName = "";
            bool b = true;

            if (b == true)
            {
                newpost.PostID = PID;
                newpost.PostContent = Pcontent;
                newpost.PostDate = DateTime.Now;
                newpost.MemberID = Session["account"].ToString();
                newpost.CategoryID = CID;
                newpost.StatusCategoryID = SCID;

                try
                {
                    db.Post.Add(newpost);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }


                b = false;
            }

            newphoto.PostID = PID;

            for (int i = 0; i < photo.Length; i++)
            {
                if (photo[i] != null)
                {
                    if (photo[i].ContentLength > 0)
                    {

                        fileName = DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "").Replace("上午", "").Replace("下午", "") + (i + 1).ToString() + ".jpg";
                        photo[i].SaveAs(Server.MapPath("~/Photo/" + fileName));
                        newphoto.Photo = fileName;
                        

                        db.PostPhoto.Add(newphoto);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index");
        }


        //----------------------------------------------


        // GET: Posts/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Post.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Posts/Create
        public ActionResult Create()
        {
            ViewBag.MemberID = new SelectList(db.Member, "MemberID", "MemberName");
            ViewBag.StatusCategoryID = new SelectList(db.PostStatusCategory, "StatusCategoryID", "StatusCategoryName");
            ViewBag.CategoryID = new SelectList(db.PostCategory, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Posts/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PostID,PostContent,PostDate,EditDate,MemberID,CategoryID,StatusCategoryID")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Post.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MemberID = new SelectList(db.Member, "MemberID", "MemberName", post.MemberID);
            ViewBag.StatusCategoryID = new SelectList(db.PostStatusCategory, "StatusCategoryID", "StatusCategoryName", post.StatusCategoryID);
            ViewBag.CategoryID = new SelectList(db.PostCategory, "CategoryID", "CategoryName", post.CategoryID);
            return View(post);
        }

        // GET: Posts/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Post.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            ViewBag.MemberID = new SelectList(db.Member, "MemberID", "MemberName", post.MemberID);
            ViewBag.StatusCategoryID = new SelectList(db.PostStatusCategory, "StatusCategoryID", "StatusCategoryName", post.StatusCategoryID);
            ViewBag.CategoryID = new SelectList(db.PostCategory, "CategoryID", "CategoryName", post.CategoryID);
            return View(post);
        }

        // POST: Posts/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostID,PostContent,PostDate,EditDate,MemberID,CategoryID,StatusCategoryID")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MemberID = new SelectList(db.Member, "MemberID", "MemberName", post.MemberID);
            ViewBag.StatusCategoryID = new SelectList(db.PostStatusCategory, "StatusCategoryID", "StatusCategoryName", post.StatusCategoryID);
            ViewBag.CategoryID = new SelectList(db.PostCategory, "CategoryID", "CategoryName", post.CategoryID);
            return View(post);
        }

        // GET: Posts/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Post.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Post post = db.Post.Find(id);
            db.Post.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult EveryPosts(string PostID)
        {
            string ID = Session["account"].ToString();

            var comment = from c in db.Comment
                          join m in db.Member on c.MemberID equals m.MemberID                          
                          select new {m.MemberName,c.PostID,c.Comment1,m.MemberID };
             var posts = from p in db.Post
                         where p.PostID == PostID
                         join pp in db.PostPhoto on p.PostID equals pp.PostID into pps
                         join pc in comment on p.PostID equals pc.PostID into pcs
                         join m in db.Member on p.MemberID equals m.MemberID
                         select new { p.PostID,p.PostContent,pps,p.PostDate,m.MemberName,pcs,m.Photo,m.Account,m.MemberID};

            var commentlike = (db.Comment.Where(c => c.PostID == PostID && c.MemberID == ID));
            
                              
              if(commentlike.Any() == false)
            {
                ViewBag.nocomment = "true";
            }


            ViewBag.everyposts = posts.ToList();
            ViewBag.commentlike = commentlike.ToList();
            //---------取得讚數---------
            var like = db.Comment.Where(c => c.PostID == PostID).Where(c => c.Like == true).Count();
            ViewBag.like = like;
            //---------取得讚數---------

           
          
            //--------個人圖片-----------
            var PersonalPhoto = db.Member.Where(p => p.MemberID == ID).Select(p => p.Photo).FirstOrDefault();
            ViewBag.personalphoto = PersonalPhoto;
            //--------個人圖片-----------

            return View();
        }

        [HttpPost]
        public ActionResult EveryPosts(string comm, string PostID)
        {
            Comment newcomment = new Comment();

            newcomment.MemberID = Session["account"].ToString();           
            newcomment.PostID = PostID;
            newcomment.MessageDate = DateTime.Now;
            newcomment.Comment1 = comm;


            if (newcomment.Like != true)
            {
                newcomment.Like = false;
            }
            else
            {
                newcomment.Like = true;
            }

            db.Comment.Add(newcomment);
            db.SaveChanges();



            return RedirectToAction("EveryPosts","Home",new { PostID = PostID });
        }

        public ActionResult likes(string PostID)
        {
            var likes =( from c in db.Comment
                       where c.PostID == PostID                     
                       select new {c.Like});

            ViewBag.like = likes;

            return View();
        }


        [HttpPost]
        public ActionResult likes(Comment newlike)
        {
           var likeststus = (from c in db.Comment
                            where c.PostID == newlike.PostID && c.MemberID == Session["account"].ToString()
                            select new { c.Like}).FirstOrDefault().ToString();
    


            if (likeststus == null)
            {
                newlike.MemberID = Session["account"].ToString();
                newlike.PostID = newlike.PostID;
                newlike.Like = newlike.Like;
                
            }
            else if(likeststus == "false")
            {
                newlike.Like = true;
            }
            else
            {
                newlike.Like = false;
            }
             
            db.SaveChanges();            


            return RedirectToAction("EveryPosts", "Home", new { PostID = newlike.PostID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newcomment(string comm, string PostID)
        {
            Comment comment = new Comment();

            //ViewBag.memberid = Session["account"].ToString();

            comment.MemberID = Session["account"].ToString();
            comment.PostID = PostID;
            comment.MessageDate = DateTime.Now;
            comment.Comment1 = comm;

            if (comment.Like != true)
            {
                comment.Like = false;
            }
            else
            {
                comment.Like = true;
            }

            db.Comment.Add(comment);
            db.SaveChanges();


            return RedirectToAction("Index", "Home");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult newlike(Comment newlikecomment,string PostID)
        //{
        //    //dynamic comm =( from c in db.Comment
        //    //           where c.PostID == PostID && c.MemberID == Session["account"].ToString()
        //    //           select new { c.Like,c.MemberID,c.PostID }).FirstOrDefault();

        //    Comment newlikecomment1 = new Comment();



        //       if (newlikecomment.Comment1 == null ) {
        //         newlikecomment1.MemberID = Session["account"].ToString();
        //         newlikecomment1.PostID = PostID;
        //         newlikecomment1.Like = true;

        //         db.Comment.Add(newlikecomment1);
        //        }
        //       else if(newlikecomment.Like == false)
        //       {
        //        newlikecomment.Like = true;

        //       }
        //       else
        //       {
        //        newlikecomment.Like = false;
        //       }

        //    db.SaveChanges();

        //    return View();
        //}

        [HttpPost]
        public ActionResult newlike(string PostID,string postid, string likestatus) {
         

            var comm = db.Comment.Where(o => o.PostID == postid && o.MemberID == Session["account"].ToString()).FirstOrDefault();

            Comment newlikecomment1 = new Comment();



                   if (comm == null ) {
                     newlikecomment1.MemberID = Session["account"].ToString();
                     newlikecomment1.PostID = postid;
                     newlikecomment1.Like = true;

                     db.Comment.Add(newlikecomment1);
                    }
            else if (likestatus == "false")
            {
                comm.Like = false;

            }
            else
            {
                comm.Like = true;
            }

            db.SaveChanges();

            return RedirectToAction("EveryPosts", "Home", new { PostID = postid });

        }

    }
}