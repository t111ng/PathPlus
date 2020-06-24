using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PathPlus.Models;

namespace PathPlus.Controllers
{
    public class GroupsController : Controller
    {
        private PathPlusEntities db = new PathPlusEntities();

        // GET: Groups
        public ActionResult Index()
        {
            //join社群類別的意思，拉關聯
            var group = db.Group.Include(g => g.GroupPrivateCategory).Include(g => g.Member);

            return View(group.ToList());
        }

        //查看該社團詳細資料
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                //回傳錯誤請求
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //搜尋該筆社團詳細資料如果沒有該筆資料回傳找不到
            Group group = db.Group.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // GET: Groups/Create
        
        public ActionResult Create()
        {

            ViewBag.PrivateCategoryID = new SelectList(db.GroupPrivateCategory, "PrivateCategoryID", "PrivateCategoryName");
            ViewBag.MemberID = new SelectList(db.Member, "MemberID", "MemberName");
            return View();
        }

        // POST: Groups/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupID,GroupName,GroupIntroduction,GroupInformation,CreateDate,MemberID,PrivateCategoryID")] Group group, HttpPostedFileBase Photo)
        {

            //新增時間
            group.CreateDate = DateTime.Now;
            //找最新ID
            SelfFeature sf = new SelfFeature();
            string GID = sf.GetID("Group");
            group.GroupID = GID;
            //var id = db.Group.OrderByDescending(p => p.GroupID).FirstOrDefault().GroupID;
            //int nid = int.Parse(id.Substring(4)) + 1;
            //id = "G" + DateTime.Now.Year.ToString().Substring(1, 3);
            //string sid = nid.ToString();
            //for (int i = 0; i < (11 - sid.Length); i++)
            //{
            //    id += 0;
            //}
            //id += sid;

            group.MemberID = Session["account"].ToString();

            Group grp = db.Group.Where(m => m.GroupName == group.GroupName).FirstOrDefault();
            //var grp = from a in db.Group where a.GroupName == group.GroupName select a;

            if (grp != null) return RedirectToAction("DupGrp");

            GroupManagement groupManagement = new GroupManagement();
            groupManagement.MemberID = Session["account"].ToString();
            groupManagement.GroupID = GID;
            groupManagement.ManageDate = DateTime.Now;
            groupManagement.AuthorityCategoryID = "0";



            //Post post = new Post();
            //var pid = db.Group.OrderByDescending(p => p.GroupID).FirstOrDefault().GroupID;
            //int npid = int.Parse(id.Substring(4)) + 1;
            //pid = "P" + DateTime.Now.Year.ToString().Substring(1, 3);
            //string spid = npid.ToString();
            //for (int i = 0; i < (11 - spid.Length); i++)
            //{
            //    pid += 0;
            //}
            //pid += spid;
            //post.PostID = pid;
            //post.PostContent = "我新增了一個群組：" + group.GroupName + "。歡迎大家加入～";
            //post.PostDate = DateTime.Now;
            //post.EditDate = DateTime.Now;
            //post.MemberID = Session["account"].ToString();
            ////post.CategoryID = "0";
            //post.StatusCategoryID = group.PrivateCategoryID;

            string fileName = "";

            if (Photo != null)
            {
                if (Photo.ContentLength > 0)
                {

                    fileName = "group" + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "").Replace("上午", "").Replace("下午", "").ToString() + ".jpg";
                    Photo.SaveAs(Server.MapPath("~/GroupPhoto/" + fileName));
                    group.Photo = fileName;

                }
            }else
            {                              
                group.Photo = "沒有頭貼.png";
            }




            if (ModelState.IsValid)
            {
                db.Group.Add(group);
                db.SaveChanges();
                db.GroupManagement.Add(groupManagement);
                db.SaveChanges();
                //db.Post.Add(post);
                //db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.PrivateCategoryID = new SelectList(db.GroupPrivateCategory, "PrivateCategoryID", "PrivateCategoryName", group.PrivateCategoryID);
            ViewBag.MemberID = new SelectList(db.Member, "MemberID", "MemberName", group.MemberID);
            return View(group);
        }

        // GET: Groups/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Group.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            ViewBag.PrivateCategoryID = new SelectList(db.GroupPrivateCategory, "PrivateCategoryID", "PrivateCategoryName", group.PrivateCategoryID);
            ViewBag.MemberID = new SelectList(db.Member, "MemberID", "MemberName", group.MemberID);
            return View(group);
        }

        // POST: Groups/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupID,GroupName,Photo,GroupIntroduction,GroupInformation,CreateDate,MemberID,PrivateCategoryID")] Group group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PrivateCategoryID = new SelectList(db.GroupPrivateCategory, "PrivateCategoryID", "PrivateCategoryName", group.PrivateCategoryID);
            ViewBag.MemberID = new SelectList(db.Member, "MemberID", "MemberName", group.MemberID);
            return View(group);
        }

        // GET: Groups/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Group.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Group group = db.Group.Find(id);
            db.Group.Remove(group);
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

        public ActionResult DupGrp()
        {
            //Response.Write("<h2>帳號重複</h2><a href='Index'>Back to List</a>");

            return View();
        }
        //---------
        //社團主頁面，顯示所有加入過的社團及社團貼文
        public ActionResult GroupHome()
        {
            //存放會員ID
            string MID = Session["account"].ToString();
            //抓取加入過的社團
            string[] selfgroup = db.JoinGroup.Where(p => p.MemberID == MID).Select(p => p.GroupID).ToList().ToArray();

            //抓已參加社團所有的貼文 沒使用到
            var grouppost = from p in db.GroupPost.Where(p => selfgroup.Contains(p.GroupID))
                            join c in db.GroupPostPhoto
                            on p.GroupPostID equals c.GroupPostID
                            select new { c.GroupPostID, c.Photo, };

            //倒序(排序新到舊)抓到的社團貼文
            ViewBag.gpp = grouppost.OrderByDescending(c => c.GroupPostID).ToList();
            //ViewModel
            GroupViewModel groupviewmodel = new GroupViewModel()
            {
                //利用selfgroup找出的已加入社團，作為條件(contains)，來抓取社團資訊並且排序
                group = db.Group.Where(p => selfgroup.Contains(p.GroupID)).OrderByDescending(p => p.GroupID).ToList(),
                //利用selfgroup找出的已加入社團，作為條件(contains)，抓取社團的貼文並且排序
                grouppost = db.GroupPost.Where(p => selfgroup.Contains(p.GroupID)).OrderByDescending(p => p.GroupID).ToList(),
            };

            //個人圖片
            var PersonalPhoto = db.Member.Where(p => p.MemberID == MID).Select(p => p.Photo).FirstOrDefault();
            ViewBag.personalphoto = PersonalPhoto;

            return View(groupviewmodel);
        }

        //顯示單一社團內容
        public ActionResult GroupOne(string GroupID)
        {
            //如果社團ID是空的倒回主頁
            if (GroupID == null)
                return RedirectToAction("Index", "Home");
            
            //抓會員ID
            string MID= Session["account"].ToString();

            //找自己社團發文，選取欄位GroupPostID
            string[] grouppost = db.GroupPost.Where(p => p.GroupID == GroupID).Select(p => p.GroupPostID).ToList().ToArray();

            //個人圖片
            var PersonalPhoto = db.Member.Where(p => p.MemberID == MID).Select(p => p.Photo).FirstOrDefault();
            ViewBag.personalphoto = PersonalPhoto;

            GroupViewModel groupviewmodel = new GroupViewModel()
            {
                grouppostphotos = db.GroupPostPhoto.Where(p => grouppost.Contains(p.GroupPostID)).OrderByDescending(p => p.GroupPostID).ToList(),
                
            };

            var getgpid = db.JoinGroup.Where(m => m.GroupID == GroupID).Count();
            if (getgpid == 0) { 
                ViewBag.tf = 1;
                }
            else
            {
                ViewBag.tf = 2;
            }
                ViewBag.gn = db.Group.Where(m => m.GroupID == GroupID).FirstOrDefault().GroupName;
                ViewBag.gi = db.Group.Where(m => m.GroupID == GroupID).FirstOrDefault().GroupIntroduction;
                ViewBag.gp = db.Group.Where(m => m.GroupID == GroupID).FirstOrDefault().Photo;
                ViewBag.GroupID = GroupID;
                //用來判斷是不是自己的貼文，來顯示相應按鈕
                ViewBag.gpid = db.Group.Where(m => m.GroupID == GroupID).FirstOrDefault().MemberID;
                ViewBag.session = Session["account"].ToString();
                return View(groupviewmodel);

            
        }

        //社團發文
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GroupNewPost(GroupPost grouppost, HttpPostedFileBase[] photo, string GroupID)
        {
            //宣告並存會員ID變數
            string MID = Session["account"].ToString();
            //抓取最新GroupPostID
            SelfFeature getgpid = new SelfFeature();
            string gpid = getgpid.GetID("GroupPost");
            //建構社團貼文圖片表物件
            GroupPostPhoto gpp = new GroupPostPhoto();
            //建構社團貼文表物件
            GroupPost gp = new GroupPost();

            //將View傳過來貼文的值放進社團貼文物件裡
            gp.GroupPostID = gpid;
            gp.Content = grouppost.Content;
            gp.PostDate = DateTime.Now;
            gp.EditDate = DateTime.Now;
            gp.GroupID = GroupID;
            gp.StatusCategoryID = grouppost.StatusCategoryID;
            gp.MemberID = MID;
            try
            {
                //資料存進模組
                db.GroupPost.Add(gp);
                //將模組資料存進資料庫
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //貼文ID
            gpp.GroupPostID = gpid;
            //宣告儲存圖片名稱變數
            string fileName = "";

            for (int i = 0; i < photo.Length; i++)
            {
                //確認看是否有圖片
                if (photo[i] != null)
                {
                    //確認圖片長度
                    if (photo[i].ContentLength > 0)
                    {
                        //圖片命名規則
                        fileName = "grouppost" + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "").Replace("上午", "").Replace("下午", "") + (i + 1).ToString() + ".jpg";
                        //儲存圖片至專案
                        photo[i].SaveAs(Server.MapPath("~/Groupposts/" + fileName));
                        //圖片名稱寫進欄位中
                        gpp.Photo = fileName;

                        //資料存進模組
                        db.GroupPostPhoto.Add(gpp);
                        //將模組資料寫進資料庫
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("GroupOne", "Groups", new { GroupID = GroupID });
        }

        //修改社團資料
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editgroup(Group group, string GroupID)
        {
            //宣告社團ID變數並放入值
            string GID = GroupID;
            //篩選要修改的社團
            var m = db.Group.Where(o => o.GroupID == GID).FirstOrDefault();
            //判斷view傳進來要修改的資料是否為空，如果為空用原本資料庫的資料
            m.GroupName = group.GroupName == null ? m.GroupName : group.GroupName;
            m.GroupIntroduction = group.GroupIntroduction == null? m.GroupIntroduction: group.GroupIntroduction;
            m.PrivateCategoryID = group.PrivateCategoryID == null? m.PrivateCategoryID:group.PrivateCategoryID;
            //將修改存進資料庫
            db.SaveChanges();

            return RedirectToAction("GroupOne", "Groups", new { GroupID = GroupID });
        }

        //單獨修改社團圖片
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Groupphoto(HttpPostedFileBase photo, string GroupID)
        {
            //宣告並存GroupID進變數
            string GID = GroupID;
            //找要修改的社團
            var m = db.Group.Where(o => o.GroupID == GID).FirstOrDefault();
            //確認傳入圖片是否正常
            if (photo.ContentLength > 0)
            {
                //宣告存放圖片名稱
                string fileName = "";
                //圖片檔名格是
                fileName = "group" + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "").Replace("上午", "").Replace("下午", "").ToString() + "p.jpg";
                //將圖片依路徑檔名儲存
                photo.SaveAs(Server.MapPath("~/Groupphotos/" + fileName));
                //修改資料庫圖片名稱
                m.Photo = fileName;
                //寫入資料庫
                db.SaveChanges();
            }

            return RedirectToAction("GroupOne", "Groups", new { GroupID = GroupID });
        }

        //抓取顯示所有社團所需資料
        public ActionResult AllGroup()
        {
            //抓所有社團貼文狀態不等於2的貼文(2表示不公開)
            string[] gpid = db.GroupPost.Where(p => p.StatusCategoryID != "2").Select(p => p.GroupPostID).ToList().ToArray();

            //使用ViewModel方式傳到view
            GroupViewModel GVM = new GroupViewModel()
            {
                //抓所有社團狀態不等於2的社團(2為不公開)
                group = db.Group.Where(m => m.PrivateCategoryID != "2").ToList(),
                //利用gpid篩選出來所有社團貼文，用來View顯示圖片用，並倒序
                grouppostphotos = db.GroupPostPhoto.Where(m => gpid.Contains(m.GroupPostID)).OrderByDescending(m => m.GroupPostID).ToList()
            };

            //存放會員ID
            string MID = Session["account"].ToString();
            //個人圖片
            var PersonalPhoto = db.Member.Where(p => p.MemberID == MID).Select(p => p.Photo).FirstOrDefault();
            ViewBag.personalphoto = PersonalPhoto;

            return View(GVM);
        }

        //用社團名稱來找社團
        [HttpPost]
        public ActionResult searchgroupshow(string GroupName)
        {
            try
            {
                //使用viewModel傳值
                GroupViewModel gvm = new GroupViewModel
                {
                    //篩選出符合傳入字串的社團
                    group = db.Group.Where(m => m.GroupName.Contains(GroupName)).OrderByDescending(m => m.GroupID).ToList()
                };

                return View(gvm);
            }
            catch
            {
                //如果發生例外導回所有社團頁面
                return RedirectToAction("AllGroup", "Groups");
            }
        }

        //加入社團
        public ActionResult joingroup(string GroupID)
        {
            //抓MemberID
            string MID = Session["account"].ToString();
            //建構加入社團的表
            JoinGroup jp = new JoinGroup();
            //將View傳入新增社團的表放進物件中
            jp.MemberID = MID;
            jp.GroupID = GroupID;
            jp.JoinDate = DateTime.Now;
            //存進模組
            db.JoinGroup.Add(jp);
            //存進資料庫
            db.SaveChanges();
            //導回頁面
            return RedirectToAction("Index", "Home", new { GroupID = GroupID });
        }

        //刪除資料庫先不做牽扯太多
        public ActionResult deletegroup(string GroupID)
        {

            GroupManagement GMn = new GroupManagement();
            string MID = Session["account"].ToString();
            GMn = db.GroupManagement.Where(m => m.MemberID == MID && m.GroupID == GroupID).SingleOrDefault();
            db.GroupManagement.Remove(GMn);
            db.SaveChanges();
            return RedirectToAction("AllGroup");
        }

        //瀏覽單則貼文
        public ActionResult readpost(string GroupPostID)
        {
            //宣告並存會員ID變數
            string MID = Session["account"].ToString();

            //抓取該貼文社團ID
            string[] getgroupID = db.GroupPost.Where(p => p.GroupPostID == GroupPostID).Select(p => p.GroupID).ToList().ToArray();
            //使用ViewModel該貼文所有照片
            GroupViewModel GVM = new GroupViewModel
            {
               grouppostphotos=db.GroupPostPhoto.Where(m=>m.GroupPostID== GroupPostID).OrderBy(m=>m.GroupPostID).ToList(),
             
            };
            //抓gpc社團貼文內容、gn社團名稱，gp社團照片、gpid社團貼文ID、like該貼文喜歡人數
            ViewBag.gpc = db.GroupPost.Where(m => m.GroupPostID == GroupPostID).FirstOrDefault().Content;
            ViewBag.gn = db.Group.Where(m => getgroupID.Contains(m.GroupID)).FirstOrDefault().GroupName;
            ViewBag.gp = db.Group.Where(m => getgroupID.Contains(m.GroupID)).FirstOrDefault().Photo;
            ViewBag.gpid = GroupPostID;
            ViewBag.like = db.CommentGroupPost.Where(m => m.GroupPostID == GroupPostID &&m.Like==true).Count();

            //抓取流言人跟名稱 應該顯示帳號就好
            var group = from a in db.Member
                        join b in db.CommentGroupPost
                        on a.MemberID equals b.MemberID
                        where b.GroupPostID == GroupPostID
                        select new { a.MemberName, b.Comment, };

            //個人圖片
            var PersonalPhoto = db.Member.Where(p => p.MemberID == MID).Select(p => p.Photo).FirstOrDefault();
            ViewBag.personalphoto = PersonalPhoto;

            ViewBag.postcomment = group.ToList();

            return View(GVM);
        }

        //社團留言
        [HttpPost]
        public ActionResult readpost(string comm, string GroupPostID)
        {
           

            //建構存新增留言的物件
            CommentGroupPost  newcomment = new CommentGroupPost();
            //將所需資料放進欄位
            newcomment.MemberID = Session["account"].ToString();
            newcomment.GroupPostID = GroupPostID;
            newcomment.SaveDate = DateTime.Now;
            newcomment.CommentDate = DateTime.Now;
            newcomment.Comment = comm;
            newcomment.Like = newcomment.Like == true ? true : false;
            
            //將資料存進模組
            db.CommentGroupPost.Add(newcomment);
            //將模組資料存進資料庫
            db.SaveChanges();

           

            ViewBag.grouppostid = GroupPostID;

            return RedirectToAction("readpost", "Groups", new { GroupPostID = GroupPostID });
        }

        



    }
}
