using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PathPlus.Models;

namespace PathPlus.Controllers
{
    public class CommentController : Controller
    {
        PathPlusEntities db = new PathPlusEntities();
        
        
       
        [ChildActionOnly]
        public PartialViewResult _CommentsForPost(string PostID)
        {            
            var comments = db.Comment.Where(m => m.PostID == PostID).OrderByDescending(m => m.SaveDate).Take(2).ToList();
            ViewBag.postid = PostID;
            return PartialView(comments);
            
        }

        public PartialViewResult _ShowLikeForPost(string PostID)
        {
            var showlike = db.Comment.Where(m => m.PostID == PostID).Where(m => m.Like == true).Count().ToString() ;
            return PartialView(showlike);
        }



        //public PartialViewResult _CreateForPost(string PostID)
        //{
        //    Comment newcomment = new Comment();

        //    newcomment.MemberID = Session["account"].ToString();
        //    newcomment.PostID = PostID;


        //    return PartialView("_CreateAComment");
        //}

        //[HttpPost]
        //public PartialViewResult _CreateForPost(string comm,string PostID)
        //{
        //    Comment comment = new Comment();

        //    //ViewBag.memberid = Session["account"].ToString();

        //    comment.MemberID = Session["account"].ToString();
        //    comment.PostID = PostID;          
        //    comment.MessageDate = DateTime.Now;
        //    comment.Comment1 = comm;


        //    if (comment.Like != true)
        //    {
        //        comment.Like = false;
        //    }
        //    else
        //    {
        //        comment.Like = true;
        //    }
            
        //    db.Comment.Add(comment);
        //    db.SaveChanges();

        //    var comments = db.Comment.Where(m => m.PostID == PostID).OrderByDescending(m => m.SaveDate).ToList();
        //    return PartialView("_CreateForPost",comments);
        //}

    }
}