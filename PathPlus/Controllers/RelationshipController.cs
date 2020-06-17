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
            if (CheckFriend(MID, RSMID) == false && CheckBlock(MID, RSMID) == false && CheckReport(MID, RSMID) == false)
                return false;

            return true;
        }

        public long FindSN(string MID, string RSMID)
        {
            var result = db.Relationship.Where(r => r.MemberID == MID && r.RSMemberID == RSMID).Select(r => r.RelationshipSN).FirstOrDefault();

            return result;
        }
        public void Follow(string RSMID)
        {
            Relationship rs = new Relationship();
            string MID = Session["account"].ToString();
            if (AllCheck(MID,RSMID) == false)
            {
                rs.FollowDate = DateTime.Now;
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
                rs.FollowDate = DateTime.Now;
                rs.Reason = "";
                rs.MemberID = MID;
                rs.RSMemberID = RSMID;

                db.Relationship.AddOrUpdate(rs);
                db.SaveChanges();
                Response.Write("有紀錄");
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
    }
}