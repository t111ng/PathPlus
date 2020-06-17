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
            //抓取加入過的社團 要改joingroup
            string[] selfgroup = db.GroupManagement.Where(p => p.MemberID == MID).Select(p => p.GroupID).ToList().ToArray();
            //抓已參加社團所有的貼文 要改
            //from m in db.GroupManagement.Where(m=>m.MemberID==MID)
            var grouppost = from p in db.GroupPost.Where(p => selfgroup.Contains(p.GroupID))
                            join c in db.GroupPostPhoto
                            on p.GroupPostID equals c.GroupPostID
                            select new
                            {
                                c.GroupPostID,
                                c.Photo,
                            };
            //倒序(排序新到舊)抓到的社團貼文
            ViewBag.gpp = grouppost.OrderByDescending(c => c.GroupPostID).ToList();


            GroupViewModel groupviewmodel = new GroupViewModel()
            {
                group = db.Group.Where(p => selfgroup.Contains(p.GroupID)).OrderByDescending(p => p.GroupID).ToList(),
                grouppost = db.GroupPost.Where(p => selfgroup.Contains(p.GroupID)).OrderByDescending(p => p.GroupID).ToList(),

            };
            return View(groupviewmodel);
        }
        public ActionResult GroupOne(string GroupID)
        {
            if (GroupID == null)
            {
                return RedirectToAction("Index", "Home");
            }
           string MID= Session["account"].ToString();
            string[] getgroupid = db.GroupManagement.Where(p => p.MemberID == MID).Select(p => p.GroupID).ToList().ToArray();
            string[] grouppost = db.GroupPost.Where(p => p.GroupID == GroupID).Select(p => p.GroupPostID).ToList().ToArray();
            GroupViewModel groupviewmodel = new GroupViewModel()
            {
                grouppostphotos = db.GroupPostPhoto.Where(p => grouppost.Contains(p.GroupPostID)).OrderByDescending(p => p.GroupPostID).ToList(),
                
            };

            
            var getgpid = db.GroupManagement.Where(m => getgroupid.Contains(GroupID)).Count();
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
                ViewBag.gpid = db.Group.Where(m => m.GroupID == GroupID).FirstOrDefault().MemberID;
                ViewBag.session = Session["account"].ToString();
                return View(groupviewmodel);

            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GroupNewPost(GroupPost grouppost, HttpPostedFileBase[] photo, string GroupID)
        {
            //抓取grouppostid

            string MID = Session["account"].ToString();
            SelfFeature getgpid = new SelfFeature();
            string gpid = getgpid.GetID("GroupPost");

            GroupPostPhoto gpp = new GroupPostPhoto();
            GroupPost gp = new GroupPost();

            gp.GroupPostID = gpid;
            gp.Content = grouppost.Content;
            gp.PostDate = DateTime.Now;
            gp.EditDate = DateTime.Now;
            gp.GroupID = GroupID;
            gp.StatusCategoryID = grouppost.StatusCategoryID;
            gp.MemberID = MID;
            try
            {
                db.GroupPost.Add(gp);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }


            gpp.GroupPostID = gpid;

            string fileName = "";


            for (int i = 0; i < photo.Length; i++)
            {
                if (photo[i] != null)
                {
                    if (photo[i].ContentLength > 0)
                    {

                        fileName = "grouppost" + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "").Replace("上午", "").Replace("下午", "") + (i + 1).ToString() + ".jpg";
                        photo[i].SaveAs(Server.MapPath("~/Groupposts/" + fileName));
                        gpp.Photo = fileName;


                        db.GroupPostPhoto.Add(gpp);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("GroupOne", "Groups", new { GroupID = GroupID });

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editgroup(Group group, string GroupID)
        {
            string a = GroupID;
            var m = db.Group.Where(o => o.GroupID == a).FirstOrDefault();

            m.GroupName = group.GroupName == null ? m.GroupName : group.GroupName;
            m.GroupIntroduction = group.GroupIntroduction == null? m.GroupIntroduction: group.GroupIntroduction;
            m.PrivateCategoryID = group.PrivateCategoryID == null? m.PrivateCategoryID:group.PrivateCategoryID;
            
            db.SaveChanges();

            return RedirectToAction("GroupOne", "Groups", new { GroupID = GroupID });
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Groupphoto(HttpPostedFileBase photo, string GroupID)
        {
            string a = GroupID;
            var m = db.Group.Where(o => o.GroupID == a).FirstOrDefault();

            if (photo.ContentLength > 0)
            {
                string fileName = "";
                fileName = "group" + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "").Replace("上午", "").Replace("下午", "").ToString() + "p.jpg";
                photo.SaveAs(Server.MapPath("~/Groupphotos/" + fileName));
                m.Photo = fileName;
                db.SaveChanges();
            }

            return RedirectToAction("GroupOne", "Groups", new { GroupID = GroupID });


        }
        public ActionResult AllGroup()
        {
            string[] gpid = db.GroupPost.Where(p => p.StatusCategoryID != "2").Select(p => p.GroupPostID).ToList().ToArray();

            GroupViewModel GVM = new GroupViewModel()
            {
                group = db.Group.Where(m => m.PrivateCategoryID != "2").ToList(),

                grouppostphotos = db.GroupPostPhoto.Where(m => gpid.Contains(m.GroupPostID)).OrderByDescending(m => m.GroupPostID).ToList()
            };




            return View(GVM);
        }

        
        [HttpPost]
        public ActionResult searchgroupshow(string GroupName)
        {
            try
            {

                GroupViewModel gvm = new GroupViewModel
                {
                    group = db.Group.Where(m => m.GroupName.Contains(GroupName)).OrderByDescending(m => m.GroupID).ToList()
                };
                ViewBag.session = Session["account"].ToString();
                return View(gvm);
            }



            catch
            {
                return RedirectToAction("AllGroup", "Groups");
            }

        }

        public ActionResult joingroup(string GroupID)
        {
            string MID = Session["account"].ToString();
            GroupManagement GM = new GroupManagement();

            GM.MemberID = MID;
            GM.GroupID = GroupID;
            GM.ManageDate = DateTime.Now;
            GM.AuthorityCategoryID = "2";

            db.GroupManagement.Add(GM);
            db.SaveChanges();

            return RedirectToAction("GroupOne", "Groups", new { GroupID = GroupID });
        }

        public ActionResult deletegroup(string GroupID)
        {
            GroupManagement GMn = new GroupManagement();
            string MID = Session["account"].ToString();
            GMn = db.GroupManagement.Where(m => m.MemberID == MID && m.GroupID == GroupID).SingleOrDefault();
            db.GroupManagement.Remove(GMn);
            db.SaveChanges();
            return RedirectToAction("AllGroup");
        }

        public ActionResult readpost(string GroupPostID)//瀏覽單則貼文
        {

            string[] getgroupID = db.GroupPost.Where(p => p.GroupPostID == GroupPostID).Select(p => p.GroupID).ToList().ToArray();
            GroupViewModel GVM = new GroupViewModel
            {
               grouppostphotos=db.GroupPostPhoto.Where(m=>m.GroupPostID== GroupPostID).OrderBy(m=>m.GroupPostID).ToList(),
             
            };
            ViewBag.gpc = db.GroupPost.Where(m => m.GroupPostID == GroupPostID).FirstOrDefault().Content;
            ViewBag.gn = db.Group.Where(m => getgroupID.Contains(m.GroupID)).FirstOrDefault().GroupName;
            ViewBag.gp = db.Group.Where(m => getgroupID.Contains(m.GroupID)).FirstOrDefault().Photo;
            ViewBag.gpid = GroupPostID;
            ViewBag.like = db.CommentGroupPost.Where(m => m.GroupPostID == GroupPostID &&m.Like==true).Count();
            var group = from a in db.Member
                        join b in db.CommentGroupPost
                        on a.MemberID equals b.MemberID
                        where b.GroupPostID == GroupPostID
                        select new { a.MemberName, b.Comment, };

            ViewBag.postcomment = group.ToList();


            return View(GVM);
        }

        [HttpPost]
        public ActionResult readpost(string comm, string GroupPostID)//此處為留言的ACTION
        {
            
           CommentGroupPost  newcomment = new CommentGroupPost();

            newcomment.MemberID = Session["account"].ToString();
            newcomment.GroupPostID = GroupPostID;
            newcomment.SaveDate = DateTime.Now;
            newcomment.CommentDate = DateTime.Now;
            newcomment.Comment = comm;


            if (newcomment.Like != true)
            {
                newcomment.Like = false;
            }
            else
            {
                newcomment.Like = true;
            }

            db.CommentGroupPost.Add(newcomment);
            db.SaveChanges();



            return RedirectToAction("readpost", "Groups", new { GroupPostID = GroupPostID });
        }

        



    }
}
