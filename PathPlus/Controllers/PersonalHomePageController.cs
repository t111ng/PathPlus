using PathPlus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PathPlus.Controllers
{
    public class PersonalHomePageController : Controller
    {
        // GET: PersonalHomePage
        PathPlusEntities db = new PathPlusEntities();

        //public ActionResult Index()
        //{
        //    string MID = Session["account"].ToString();
        //    return RedirectToAction("Index", "PersonalHomePage", new { MemberID = MID });
        //}

        //個人頁顯示資料
        public ActionResult Index(string MemberID)
        {
            //Session的會員ID放進變數
            string SessionMID = Session["account"].ToString();

            string MID;

            //對傳進來的會員ID，判斷個人主頁應該顯示的相對應按紐
            //check:1(表示顯示可編輯資料、換大頭貼、發文)
            //check:0 && relationship:friend(顯示退追蹤、聊天)
            //check:0 && relationship:notfriend(顯示追蹤)
            if (MemberID==null)
            {
                 MID= SessionMID;
                ViewBag.check = 1;
            }
            else
            {
                MID = MemberID;
                if (MID == SessionMID)
                {
                    ViewBag.check = 1;
                }
                else
                {
                    var relatioship = db.Relationship.Where(r => r.MemberID == SessionMID && r.RSMemberID == MID && r.FollowDate.Year > 1991 && r.BlockDate.Year < 1992 && r.ReportDate.Year < 1992).FirstOrDefault();
                    ViewBag.check = 0;
                    ViewBag.relationship = relatioship == null ? "notfriend" : "friend";
                }
            }
            
            //篩選自己Post表裡的貼文
            string[] selfpost = db.Post.Where(p => p.MemberID == MID).Select(p => p.PostID).ToList().ToArray();
            //ViewModle
            PersonalViewModel vm = new PersonalViewModel()
            {
                //查會員自己的資料
                member = db.Member.Where(m => m.MemberID == MID).ToList(),
                //查自己的貼文並且排序最新的在前面
                post = db.Post.Where(p => p.MemberID == MID).OrderByDescending(m => m.PostDate).ToList(),
                //抓取自己發文的所有照片，透過selfpost來篩選
                postPhoto = db.PostPhoto.Where(p => selfpost.Contains(p.PostID)).OrderByDescending(p=>p.PostID).ToList()

            };

            //View所需資料，pp簡介、mn會員名稱、em信箱、ph大頭貼
            ViewBag.pp = db.Member.Where(m => m.MemberID == MID).FirstOrDefault().PersonalProfile;
            ViewBag.mn = db.Member.Where(m => m.MemberID == MID).FirstOrDefault().MemberName;
            ViewBag.em = db.Member.Where(m => m.MemberID == MID).FirstOrDefault().Mail;
            ViewBag.ph = db.Member.Where(m => m.MemberID == SessionMID).FirstOrDefault().Photo;

            //View在追蹤與退追蹤按鈕所需的會員ID
            ViewBag.RSID = MID;

            ViewBag.MID = MemberID;
            if (MemberID == null)
                ViewBag.MID = SessionMID;

            //個人圖片
            var PersonalPhoto = db.Member.Where(p => p.MemberID == SessionMID).Select(p => p.Photo).FirstOrDefault();
            ViewBag.personalphoto = PersonalPhoto;

            return View(vm);
        }

        //修改個人資料
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editpersonal(Member member)
        {
            //取的Session存的會員ID
            string MID = Session["account"].ToString();
            //找會員資料表裡的該筆會員資料
            var m = db.Member.Where(o => o.MemberID == MID).FirstOrDefault();

            //傳過來的資料如果是空的，就使用資料庫原本的資料，否則就用View傳過來的
            m.MemberName = member.MemberName == null ? m.MemberName : member.MemberName;
            m.PersonalProfile = member.PersonalProfile == null ? m.PersonalProfile : member.PersonalProfile;
            m.Mail = member.Mail == null ? m.Mail : member.Mail;
            m.Address = member.Address == null ? m.Address : member.Address;
            m.Gender = member.Gender == null ? m.Gender : member.Gender;

            try { 
            db.SaveChanges();
            }catch(Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("Index", "PersonalHomePage", new { MemberID = MID });
        }

        //修改大頭貼
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult selfphoto(HttpPostedFileBase photo)
        {
            //取的Session存的會員ID
            string MID = Session["account"].ToString();
            //從會員資料表找該會員資料
            var m = db.Member.Where(o => o.MemberID == MID).FirstOrDefault();

            //判斷是否有成功傳圖片過來，如果有圖片長度大於0
            if (photo.ContentLength > 0)
            {
                //宣告fileName變數
                string fileName = "";
                //將圖片檔名存成現在時間，並利用Replace裡的規則取代成功值
                fileName = DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "").Replace("上午", "").Replace("下午", "") .ToString() + "p.jpg";
                //將圖片存進指定位置
                photo.SaveAs(Server.MapPath("~/Selfphoto/" + fileName));
                //修改大頭貼檔名
                m.Photo = fileName;   
                //修改資料庫資料
                db.SaveChanges();
            }

            return RedirectToAction("Index");


        }

        //瀏覽單則貼文
        public ActionResult readpersonalpost(string PostID)
        {
            //取的Session存的會員ID
            string MID = Session["account"].ToString();

            //使用ViewModel
            PersonalViewModel PVM = new PersonalViewModel
            {
                //找該筆檔案發文的照片，並排序
                postPhoto=db.PostPhoto.Where(m=>m.PostID== PostID).OrderBy(m=>m.PostID).ToList(),
                
            };
            //抓取pc貼文內容、mn會員名稱、mp會員大頭貼、pid貼文ID、like按讚數
            ViewBag.pc = db.Post.Where(m => m.PostID == PostID).FirstOrDefault().PostContent;
            ViewBag.mn = db.Member.Where(m => m.MemberID== MID).FirstOrDefault().MemberName;
            ViewBag.mp = db.Member.Where(m => m.MemberID == MID).FirstOrDefault().Photo;
            ViewBag.pid = PostID;
            ViewBag.like = db.Comment.Where(m => m.PostID == PostID && m.Like == true).Count();
            //抓取留言者名稱與留言內容
            var post = from a in db.Member
                        join b in db.Comment
                        on a.MemberID equals b.MemberID
                        where b.PostID == PostID
                        select new { a.MemberName, b.Comment1, };
            //儲存上方post變數抓取內容
            ViewBag.postcomment = post.ToList();

            //抓取該筆貼文的評論
            var commentlike = (db.Comment.Where(c => c.PostID == PostID && c.MemberID == MID));

            //如果該貼文沒有人評論還是需要顯示點選喜歡的圖片，所以要做判斷值
            if (commentlike.Any() == false)
                ViewBag.nocomment = "true";

            //View所需評論資訊
            ViewBag.commentlike = commentlike.ToList();

            return View(PVM);
        }

        //此處為留言的ACTION
        [HttpPost]
        public ActionResult readpersonalpost(string comm, string PostID)
        {
            //產生一個comment物件
            Comment newcomment = new Comment();
            //將所需欄位賦值
            newcomment.MemberID = Session["account"].ToString();
            newcomment.PostID = PostID;
            newcomment.SaveDate = DateTime.Now;
            newcomment.MessageDate= DateTime.Now;
            newcomment.Comment1 = comm;
            //comment可能有流言但沒有按讚，維持是否按讚原本的狀態
            newcomment.Like = newcomment.Like == true?  true :false;

            db.Comment.Add(newcomment);
            db.SaveChanges();

            return RedirectToAction("readpersonalpost", "PersonalHomePage", new { PostID = PostID });
        }

    }

}