using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PathPlus.Models;
namespace PathPlus.Controllers
{
    public class SelfFeature
    {
        PathPlusEntities dbSF = new PathPlusEntities();

        public SelfFeature()
        {

        }
        
        public string GetID(string tableName)
        {
            //最終結果回傳的字串ID
            string strval = "";
            //用作儲存切割後的字串ID
            string substr = "";

            switch (tableName)
            {
                case "Member":
                    //找最後一筆資料
                    var Member = dbSF.Member.OrderByDescending(m => m.MemberID).FirstOrDefault();
                    //判斷該資料表是否有資料，有就從ID第5碼切割，沒有就是預設值0
                    //(不拿出去判斷原因為如果是Null還使用Null.ID，會出現未將物件參考設定為物件的執行個體)
                    substr = (Member == null) ? "0" :Member.MemberID.Substring(4);
                    //前四碼ID製作
                    strval = "M" + DateTime.Now.Year.ToString().Substring(1, 3);
                    break;
                case "Post":
                    var Post = dbSF.Post.OrderByDescending(m => m.PostID).FirstOrDefault();
                    substr = (Post == null) ? "0" : Post.PostID.Substring(4);
                    strval = "P" + DateTime.Now.Year.ToString().Substring(1, 3);
                    break;
                case "Card":
                    var Card = dbSF.Card.OrderByDescending(m => m.CardID).FirstOrDefault();
                    substr = (Card == null) ? "0" : Card.CardID.Substring(4);
                    strval = "C" + DateTime.Now.Year.ToString().Substring(1, 3);
                    break;
                case "Group":
                    var Group = dbSF.Group.OrderByDescending(m => m.GroupID).FirstOrDefault();
                    substr = (Group == null) ? "0" : Group.GroupID.Substring(4);
                    strval = "G" + DateTime.Now.Year.ToString().Substring(1, 3);
                    break;
                case "GroupPost":
                    var GroupPost = dbSF.GroupPost.OrderByDescending(m => m.GroupPostID).FirstOrDefault();
                    substr = (GroupPost == null) ? "0" : GroupPost.GroupPostID.Substring(4);
                    strval = "R" + DateTime.Now.Year.ToString().Substring(1, 3);
                    break;
                case "Advertisers":
                    var Advertisers = dbSF.Advertisers.OrderByDescending(m => m.CompanyID).FirstOrDefault();
                    substr = (Advertisers == null) ? "0" : Advertisers.CompanyID.Substring(4);
                    strval = "V" + DateTime.Now.Year.ToString().Substring(1, 3);
                    break;
                case "Advertisement":
                    var Advertisement = dbSF.Advertisement.OrderByDescending(m => m.AdvertisementID).FirstOrDefault();
                    substr = (Advertisement == null) ? "0" : Advertisement.AdvertisementID.Substring(4);
                    strval = "D" + DateTime.Now.Year.ToString().Substring(1, 3);
                    break;
                case "Announcement":
                    var Announcement = dbSF.Announcement.OrderByDescending(m => m.AnnouncementID).FirstOrDefault();
                    substr = (Announcement == null) ? "0" : Announcement.AnnouncementID.Substring(4);
                    strval = "N" + DateTime.Now.Year.ToString().Substring(1, 3);
                    break;
                case "Term":
                    var Term = dbSF.Term.OrderByDescending(m => m.TermID).FirstOrDefault();
                    substr = (Term == null) ? "0" : Term.TermID.Substring(4);
                    strval = "T" + DateTime.Now.Year.ToString().Substring(1, 3);
                    break;
                case "Administrator":
                    var Administrator = dbSF.Administrator.OrderByDescending(m => m.AdministratorID).FirstOrDefault();
                    substr = (Administrator == null) ? "0" : Administrator.AdministratorID.Substring(4);
                    strval = "A" + DateTime.Now.Year.ToString().Substring(1, 3);
                    break;
                case "Record":
                    var Record = dbSF.Record.OrderByDescending(m => m.QuestionID).FirstOrDefault();
                    substr = (Record == null) ? "0" : Record.QuestionID.Substring(4);
                    strval = "Q" + DateTime.Now.Year.ToString().Substring(1, 3);
                    break;
            }
            //將字串轉成數字+1,再轉回字串
            substr = ((Convert.ToInt64(substr)) + 1).ToString();
            //假設ID為M02000000000001,切割字串存下來一開始為00000000001,轉成數字+1過程中再轉回字串會變成2
            //所以要判斷目前切割字串中個數離11個還有多少
            //把缺少的填到最終回傳字串
            for (int i = 0; i < (11 - substr.Length); i++)
            {
                strval += 0;
            }
            //填完之後會跟最新ID結合
            strval += substr;

            return strval;
        }
    }
}

//確認值的函式
//public void checkFun()
//{
//    SelfFeature checkFun = new SelfFeature();

//    Response.Write(checkFun.GetID("Member") + "<hr/>");
//    Response.Write(checkFun.GetID("Post") + "<hr/>");
//    Response.Write(checkFun.GetID("Card") + "<hr/>");
//    Response.Write(checkFun.GetID("Group") + "<hr/>");
//    Response.Write(checkFun.GetID("GroupPost") + "<hr/>");
//    Response.Write(checkFun.GetID("Advertisers") + "<hr/>");
//    Response.Write(checkFun.GetID("Advertisement") + "<hr/>");
//    Response.Write(checkFun.GetID("Announcement") + "<hr/>");
//    Response.Write(checkFun.GetID("Term") + "<hr/>");
//    Response.Write(checkFun.GetID("Administrator") + "<hr/>");
//    Response.Write(checkFun.GetID("Record") + "<hr/>");

//}