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

        //留言顯示做成PartialView
        [ChildActionOnly]
        public PartialViewResult _CommentsForPost(string PostID)
        {            
            //抓取發文的留言以儲存時間做排序
            var comments = db.Comment.Where(m => m.PostID == PostID).OrderByDescending(m => m.SaveDate).Take(2).ToList();
            //給View做使用
            ViewBag.postid = PostID;
            return PartialView(comments);
        }

        //喜歡該貼文人數，做成PartialView
        public PartialViewResult _ShowLikeForPost(string PostID)
        {
            //找出該貼文喜歡的人，使用count找出人數
            var showlike = db.Comment.Where(m => m.PostID == PostID).Where(m => m.Like == true).Count().ToString() ;
            return PartialView(showlike);
        }
    }
}