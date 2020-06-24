using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using PathPlus.Models;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

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

            //將會員ID放進變數
            string ID = Session["account"].ToString();
            //篩選出自己的貼文，join需要的內容的表，選擇所需欄位
            var post1 = (from p in db.Post
                         where p.MemberID == ID
                         //join pp in db.PostPhoto on p.PostID equals pp.PostID
                         join m in db.Member on p.MemberID equals m.MemberID
                         join c in db.PostCategory on p.CategoryID equals c.CategoryID
                         join s in db.PostStatusCategory on p.StatusCategoryID equals s.StatusCategoryID                    
                         select new { p.PostID, p.PostContent, p.PostDate, p.EditDate, m.MemberName, c.CategoryName, s.StatusCategoryName,m.Photo,p.MemberID});

            //Relationship表利用大於1991判斷，把是自己好友的ID找出來
            string[] rid = db.Relationship.Where(m => m.MemberID == ID && m.FollowDate.Year > 1991).Select(m => m.RSMemberID).ToList().ToArray();

            //利用上面rid找出的好友，使用contains方法，篩選出是自己好友並且狀態不等於2(2表示不公開)的貼文
            var post2 = (from p in db.Post
                         where rid.Contains(p.MemberID) && p.StatusCategoryID != "2"
                         //join pp in db.PostPhoto on p.PostID equals pp.PostID
                         join m in db.Member on p.MemberID equals m.MemberID
                         join c in db.PostCategory on p.CategoryID equals c.CategoryID
                         join s in db.PostStatusCategory on p.StatusCategoryID equals s.StatusCategoryID
                         select new { p.PostID, p.PostContent, p.PostDate, p.EditDate, m.MemberName, c.CategoryName, s.StatusCategoryName, m.Photo, p.MemberID});
            
            //利用rid找出的自己好友的ID，篩選出貼文貼文中屬於好友的的貼文ID
            string[] rid2 = db.Post.Where(p => p.StatusCategoryID != "2").Where(p => rid.Contains(p.MemberID)).Select(p => p.PostID).ToList().ToArray();
            //找出自己貼文的ID
            string[] rid3 = db.Post.Where(p => p.StatusCategoryID != "2").Where(p => p.MemberID == ID).Select(p => p.PostID).ToList().ToArray();
            //利用rid2篩選出好友的貼文ID，找出對應貼文的圖片，選取PostID、Photo兩個欄位
            var photo1 = from po in db.PostPhoto
                        where rid2.Contains(po.PostID)
                        select new { po.PostID,po.Photo};
            //利用rid3篩出自己貼文ID，找出相對應圖片，選取PostID、Photo兩個欄位
            var photo2 = from po in db.PostPhoto
                         where rid3.Contains(po.PostID)
                         select new { po.PostID, po.Photo };
            //將自己貼文與好友貼文合併，以貼文日期做排序
            var post = post1.Union(post2).OrderByDescending(x => x.PostDate).ToList();
            //將自己與好友貼文的圖片合併，以ID做排序
            var photo3 = photo2.Union(photo1).OrderByDescending(x => x.PostID).ToList();

            //合併後貼文供View使用
            ViewBag.post = post.ToList();
            //合併後貼文所發圖片供View使用
            ViewBag.photo = photo3.ToList();
            //ViewBag.comment = comment.ToList();

            //取個人帳號
            var PersonalAccount = db.Member.Where(m => m.MemberID == ID).Select(m => m.Account).FirstOrDefault();
            ViewBag.personalaccount = PersonalAccount;

            //取個人姓名
            var PersonalName = db.Member.Where(m => m.MemberID == ID).Select(m => m.MemberName).FirstOrDefault();
            ViewBag.personalname = PersonalName;

            //個人圖片
            var PersonalPhoto = db.Member.Where(p => p.MemberID == ID).Select(p => p.Photo).FirstOrDefault();
            ViewBag.personalphoto = PersonalPhoto;


            //提取自己沒加入過的社團資訊，只抓五筆
            var jp = db.JoinGroup.Where(j => j.MemberID == ID).Select(j => j.GroupID).ToList().ToArray();
            var group = db.Group.Where(g=>g.PrivateCategoryID == "0").Where(g=> !jp.Contains(g.GroupID)).Take(5);
            ViewBag.group = group.ToList();
            ViewBag.MID = Session["account"].ToString();

            //取個人ID
            ViewBag.MemberID = Session["account"].ToString();

            return View();
        }

        //頁面搜尋別人帳號功能
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult SearchBar(string keyword)
        {
            //宣告會員ID變數
            string ID = Session["account"].ToString();
            //會員資料，以ID做排序
            var Result = db.Member.Where(m => m.Account.Contains(keyword) &&m.MemberID!=ID).OrderBy(m => m.MemberID).ToList();
            //找出是自己朋友的會員，用來判斷是否顯示追蹤按鈕
            var checkfriends = db.Relationship.Where(r => r.MemberID == ID && r.FollowDate.Year > 1991).ToList();
            //如果有查到好友丟到ViewBag給View使用
            if (checkfriends.Count > 0)
                ViewBag.checkfriend = checkfriends;

            ViewBag.keyword = keyword;
            return View(Result);
        }

        //開卡
        public ActionResult Card()
        {
            return View();
        }

        //開卡
        [HttpPost]
        public ActionResult Card(Card card, string CInt, HttpPostedFileBase CPhoto, bool CardGender, string CardStatus)
        {
            //建構一個卡片表的物件出來
            Card newCard = new Card();
            //抓取最新卡片ID
            SelfFeature sf = new SelfFeature();
            string CID = sf.GetID("Card");
            //宣告圖片變數做後續使用
            string fileName = "";

            //將Card所需資料放進建構的物件
            newCard.CardID = CID;
            newCard.Interests = CInt;
            newCard.Gender = CardGender;
            newCard.MemberID = Session["account"].ToString();
            newCard.CardStatusID = CardStatus;

            //判斷傳進來的照片是否為空
            if (CPhoto != null)
            {
                //如果不是空的判斷是否圖片大小大於0
                if (CPhoto.ContentLength > 0)
                {
                    //檔案名稱使用以下規則
                    fileName = "card" + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "").Replace("上午", "").Replace("下午", "") + ".jpg";
                    //把檔案存刀設定位置及檔名
                    CPhoto.SaveAs(Server.MapPath("~/CardPhotos/" + fileName));
                    //將檔名寫到資料庫
                    newCard.Photo = fileName;
                }
            }
            //將資料存到模組
            db.Card.Add(newCard);
            //模組資料存到資料庫
            db.SaveChanges();

            return RedirectToAction("Index");
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

        //發文
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPost(string Pcontent, string CID, string SCID, HttpPostedFileBase[] photo)
        //
        {
            //建造Post表的物件(用來發文用)
            Post newpost = new Post();
            //建造PostPhoto表的物件(用來發文的圖片用)
            PostPhoto newphoto = new PostPhoto();

            //取的最新PostID，內容寫在Controller的SelfFeature
            SelfFeature sf = new SelfFeature();
            string PID = sf.GetID("Post");

            //圖片名稱，預作宣告
            string fileName = "";

            //將View傳進來的貼文資料放進建構出來Post物件
            newpost.PostID = PID;
            newpost.PostContent = Pcontent;
            newpost.PostDate = DateTime.Now;
            newpost.MemberID = Session["account"].ToString();
            newpost.CategoryID = CID;
            newpost.StatusCategoryID = SCID;

            try
            {
                //儲存進模組
                db.Post.Add(newpost);
                //儲存進資料庫
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //PostPhoto的PostID資料
            newphoto.PostID = PID;

            //Photo.Length抓取圖片數
            for (int i = 0; i < photo.Length; i++)
            {
                //判斷傳進來的圖片不是空的
                if (photo[i] != null)
                {
                    //確認圖片內容是有的
                    if (photo[i].ContentLength > 0)
                    {
                        //取的現在日期用作圖片名稱，但利用Replace將不要的丟掉
                        fileName = DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "").Replace("上午", "").Replace("下午", "") + (i + 1).ToString() + ".jpg";
                        //將圖片存進指定資料夾名且指定名稱
                        photo[i].SaveAs(Server.MapPath("~/Photo/" + fileName));
                        //存PostPhoto的圖片名稱
                        newphoto.Photo = fileName;
                        
                        //將所有資料存進模組
                        db.PostPhoto.Add(newphoto);
                        //確認存進資料庫
                        db.SaveChanges();
                    }
                }
            }
            //導回頁面
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
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Post post = db.Post.Find(id);
        //    if (post == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(post);
        //}

        // POST: Posts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    Post post = db.Post.Find(id);
        //    db.Post.Remove(post);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //單則貼文
        public ActionResult EveryPosts(string PostID)
        {
            //會員ID傳進變數裡
            string ID = Session["account"].ToString();

            //評論表join會員表取會員名稱、發文ID、留言內容、會員ID
            var comment = from c in db.Comment
                          join m in db.Member on c.MemberID equals m.MemberID                          
                          select new {m.MemberName,c.PostID,c.Comment1,m.MemberID };

            //貼文join圖片表、評論、會員，對圖片與評論做Groupjoin，where貼文ID
            //抓取貼文ID、貼文內容、該貼文所擁有的圖片、發文日期、會員名稱、貼文所擁有的留言、大頭貼、帳號、會員ID
             var posts = from p in db.Post
                         where p.PostID == PostID
                         join pp in db.PostPhoto on p.PostID equals pp.PostID into pps
                         join pc in comment on p.PostID equals pc.PostID into pcs
                         join m in db.Member on p.MemberID equals m.MemberID
                         select new { p.PostID,p.PostContent,pps,p.PostDate,m.MemberName,pcs,m.Photo,m.Account,m.MemberID};

            //抓取該筆貼文的評論
            var commentlike = (db.Comment.Where(c => c.PostID == PostID && c.MemberID == ID));
            
            //如果該貼文沒有人評論還是需要顯示點選喜歡的圖片，所以要做判斷值
            if(commentlike.Any() == false)
                ViewBag.nocomment = "true";

            //View所需貼文資訊
            ViewBag.everyposts = posts.ToList();
            //View所需評論資訊
            ViewBag.commentlike = commentlike.ToList();

            //取該貼文得讚數並且放進ViewBag
            var like = db.Comment.Where(c => c.PostID == PostID).Where(c => c.Like == true).Count();
            ViewBag.like = like;
          
            //大頭貼
            var PersonalPhoto = db.Member.Where(p => p.MemberID == ID).Select(p => p.Photo).FirstOrDefault();
            ViewBag.personalphoto = PersonalPhoto;
            
            return View();
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

        //點選喜歡(Json用)
        [HttpPost]
        public JsonResult newlike(object sender, EventArgs e)
        //public ActionResult newlike(object sender, EventArgs e) 
        {
            //接收json方法
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();

            //做一個class用來裝解開json的資料，class內容在該Action下
            checklike check = JsonConvert.DeserializeObject<checklike>(stream);
            if (check!=null)
            {
                //抓MemberID
                string MID = Session["account"].ToString();
                //找Comment(評論表)裡是否已經有該會員對該貼文的評論
                var comm = db.Comment.Where(o => o.PostID == check.postid && o.MemberID == MID).FirstOrDefault();

                //如果該會員沒有對該貼文做評論的紀錄
                if (comm == null)
                {
                    Comment commentLike = new Comment();
                    commentLike.MemberID = MID;
                    commentLike.PostID = check.postid;
                    commentLike.Like = true;

                    db.Comment.Add(commentLike);
                }
                else
                {
                    //有資料就將要改變的資料作修改
                    comm.Like = check.likestatus;

                }
                
                db.SaveChanges();
                return Json(new { msg = "成功" });
            }

            return Json(new { msg = "傳進來的值是空的" });
            //return RedirectToAction("EveryPosts", "Home", new { PostID = check.postid });
        }

        //newlike解json所需class
        public class checklike
        {
            public string postid { get; set; }    
            public bool likestatus { get; set; }    
            
        }

        public ActionResult CategoryPost(string categoryid)
        {
            var ID = Session["account"].ToString();
            var categorypost = from p in db.Post
                               where p.StatusCategoryID == "0" && p.CategoryID == categoryid
                               join m in db.Member on p.MemberID equals m.MemberID
                               join pp in db.PostPhoto on p.PostID equals pp.PostID into pps
                               select new { p.PostID, p.PostContent, pps, p.PostDate, m.MemberName, m.Photo, m.Account, m.MemberID };

            if (categorypost.Any() == false)
            {
                ViewBag.postmessage = false;
            }
            else
            {
                ViewBag.postmessage = true;
                ViewBag.post = categorypost.OrderByDescending(p => p.PostDate).ToList();
            }

            //個人圖片
            var PersonalPhoto = db.Member.Where(p => p.MemberID == ID).Select(p => p.Photo).FirstOrDefault();
            ViewBag.personalphoto = PersonalPhoto;
            return View();
        }
    }
}