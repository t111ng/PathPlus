using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PathPlus.Models;
namespace PathPlus.Controllers
{
    public class RelationshipController : Controller
    {
        PathPlusEntities db = new PathPlusEntities();
        // GET: Relationship
        public ActionResult Index()
        {
            return View();
        }

        public bool CheckFriend(string MID,string RSMID)
        {
            var result = db.Relationship.Where(r => r.MemberID == MID && r.RSMemberID == RSMID && r.FollowDate.Year>1991).Select(r => r.FollowDate.Year).FirstOrDefault().ToString();
            //Response.Write(result);
            if (result=="0")
                return false;
            
            return true;
        }

        public bool CheckBlock(string MID, string RSMID)
        {
            var result = db.Relationship.Where(r => r.MemberID == MID && r.RSMemberID == RSMID && r.BlockDate.Year > 1991).Select(r => r.BlockDate.Year).FirstOrDefault().ToString();
            if (result == "0")
                return false;

            return true;
        }

        public bool CheckReport(string MID, string RSMID)
        {
            var result = db.Relationship.Where(r => r.MemberID == MID && r.RSMemberID == RSMID && r.ReportDate.Year > 1991).Select(r => new { r.ReportDate.Year, r.Reason }).FirstOrDefault();
            if (result == null)
                return false;

            return true;
        }

        public bool AllCheck(string MID, string RSMID)
        {
            var result = db.Relationship.Where(r => r.MemberID == MID && r.RSMemberID == RSMID ).Count();
            if (result==0)
                return false;

            return true;
        }

        public long FindSN(string MID, string RSMID)
        {
            var result = db.Relationship.Where(r => r.MemberID == MID && r.RSMemberID == RSMID).Select(r => r.RelationshipSN).FirstOrDefault();

            return result;
        }
        public void Follow(string RSMID,bool flage)
        {
            Relationship rs = new Relationship();
            string MID = Session["account"].ToString();
            if (flage)
                rs.FollowDate = DateTime.Now;
            if (AllCheck(MID, RSMID) == false)
            {
                rs.Reason = "";
                rs.MemberID = MID;
                rs.RSMemberID = RSMID;

                db.Relationship.Add(rs);
                db.SaveChanges();
            }
            else
            {
                rs.RelationshipSN = FindSN(MID, RSMID);
                rs.Reason = "";
                rs.MemberID = MID;
                rs.RSMemberID = RSMID;

                db.Relationship.AddOrUpdate(rs);
                db.SaveChanges();
            }
        }

        public void Block(string RSMID)
        {
            Relationship rs = new Relationship();
            string MID = Session["account"].ToString();
            if (AllCheck(MID, RSMID) == false)
            {
                rs.BlockDate = DateTime.Now;
                rs.Reason = "";
                rs.MemberID = MID;
                rs.RSMemberID = RSMID;

                db.Relationship.Add(rs);
                db.SaveChanges();

                Response.Write("沒紀錄");
            }
            else
            {
                rs.RelationshipSN = FindSN(MID, RSMID);
                rs.BlockDate = DateTime.Now;
                rs.Reason = "";
                rs.MemberID = MID;
                rs.RSMemberID = RSMID;

                db.Relationship.AddOrUpdate(rs);
                db.SaveChanges();
                Response.Write("有紀錄");
            }
        }
        public void Report(string RSMID,string reason)
        {
            string MID = Session["account"].ToString();
            Relationship rs = new Relationship();
            if (AllCheck(MID,RSMID) == false)
            {
                rs.ReportDate = DateTime.Now;
                rs.Reason = reason == null ? "" : reason;
                rs.MemberID = MID;
                rs.RSMemberID = RSMID;

                db.Relationship.Add(rs);
                db.SaveChanges();

                Response.Write("沒紀錄");
            }
            else
            {
                rs.RelationshipSN = FindSN(MID,RSMID);
                rs.ReportDate = DateTime.Now;
                rs.Reason = reason == null ? "" : reason;
                rs.MemberID = MID;
                rs.RSMemberID = RSMID;

                //db.Relationship.Attach(rs);
                db.Relationship.AddOrUpdate(rs);
                db.SaveChanges();

                Response.Write("有紀錄");
            }
        }

        //@Html.Action("_RelationshipForDetailPage","Relationship",new { MemberID="M02000000000001"})
        public PartialViewResult _RelationshipForDetailPage(string MemberID)
        {
            //用來取該主頁會員粉絲、追蹤、發文數的
            var member = db.Member.Find(MemberID);
            string SessionMID = Session["account"].ToString();
            //對傳進來的會員ID，判斷個人主頁應該顯示的相對應按紐
            //check:1(表示顯示可編輯資料、換大頭貼、發文)
            //check:0 && relationship:friend(顯示退追蹤、聊天)
            //check:0 && relationship:notfriend(顯示追蹤)
            //有些情況會不需要傳ID就可以查看個人頁面(像是自己的頁面)
            if (MemberID == null)
            {
                ViewBag.check = 1;
            }
            else
            {
                if (MemberID == SessionMID)
                {
                    ViewBag.check = 1;
                }
                else
                {
                    var relatioship = db.Relationship.Where(r => r.MemberID == SessionMID && r.RSMemberID == MemberID && r.FollowDate.Year > 1991 && r.BlockDate.Year < 1992 && r.ReportDate.Year < 1992).FirstOrDefault();
                    ViewBag.check = 0;
                    ViewBag.relationship = relatioship == null ? "notfriend" : "friend";
                }
            }


            ViewBag.Fans = member.Fans;
            ViewBag.Follower = member.Follower;
            ViewBag.PostCount = member.PostCount;
            ViewBag.Photo = member.Photo;
            ViewBag.RSID = MemberID;
            return PartialView();
        }

        //<a href = "@Url.Action("FollowForRelationship", "Relationship", new {RSMID=ViewBag.RSID,flage=false})" class="btn btn-info align-self-end col-md">退追蹤</a>
        public ActionResult FollowForRelationship(string RSMID, bool flage)
        {
            Follow(RSMID, flage);

            return RedirectToAction("Index", "PersonalHomePage", new { MemberID = RSMID });
        }

        //<a href = "@Url.Action("FollowForSearchbar", "Relationship", new {RSMID=ViewBag.RSID,flage=false})" class="btn btn-info align-self-end col-md">退追蹤</a>
        public ActionResult FollowForSearchbar(string RSMID, bool flage, string keyword)
        {
            Follow(RSMID, flage);

            return RedirectToAction("SearchBar", "Home", new { keyword = keyword });
        }
    }
}