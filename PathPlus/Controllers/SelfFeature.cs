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
        public string GetPostID()
        {
            string strval = "";
            var result = dbSF.Post.OrderByDescending(m => m.PostID).FirstOrDefault();
            if(result == null)
            {
                strval = "P02000000000001";
            }
            else
            {
                string substr = result.PostID.Substring(2, 13);
                long intVal = (Convert.ToInt64(substr)) + 1;
                strval = "P0" + intVal.ToString();
            }
            
            return strval;
        }

        public string GetID(string table)
        {
            string strval = "";
            //string idVal = "";
            string word = "";
            string substr = "";
            switch (table)
            {
                case "Member":
                    var Member = dbSF.Member.OrderByDescending(m => m.MemberID).FirstOrDefault();
                    substr = Member.MemberID.Substring(2, 13);
                    word = "M0";
                    break;
                case "Post":
                    var Post = dbSF.Post.OrderByDescending(m => m.PostID).FirstOrDefault();
                    substr = Post.PostID.Substring(2, 13);
                    word = "P0";
                    break;
                case "Card":
                    var Card = dbSF.Card.OrderByDescending(m => m.CardID).FirstOrDefault();
                    substr = Card.CardID.Substring(2, 13);
                    word = "C0";
                    break;
                case "Group":
                    var Group = dbSF.Group.OrderByDescending(m => m.GroupID).FirstOrDefault();
                    substr = Group.GroupID.Substring(2, 13);
                    word = "G0";
                    break;
                case "GroupPost":
                    var GroupPost = dbSF.GroupPost.OrderByDescending(m => m.GroupPostID).FirstOrDefault();
                    substr = GroupPost.GroupPostID.Substring(2, 13);
                    word = "R0";
                    break;
                case "Advertisers":
                    var Advertisers = dbSF.Advertisers.OrderByDescending(m => m.CompanyID).FirstOrDefault();
                    substr = Advertisers.CompanyID.Substring(2, 13);
                    word = "V0";
                    break;
                case "Advertisement":
                    var Advertisement = dbSF.Advertisement.OrderByDescending(m => m.AdvertisementID).FirstOrDefault();
                    substr = Advertisement.AdvertisementID.Substring(2, 13);
                    word = "D0";
                    break;
                case "Announcement":
                    var Announcement = dbSF.Announcement.OrderByDescending(m => m.AnnouncementID).FirstOrDefault();
                    substr = Announcement.AnnouncementID.Substring(2, 13);
                    word = "N0";
                    break;
                case "Term":
                    var Term = dbSF.Term.OrderByDescending(m => m.TermID).FirstOrDefault();
                    substr = Term.TermID.Substring(2, 13);
                    word = "T0";
                    break;
                case "Administrator":
                    var Administrator = dbSF.Administrator.OrderByDescending(m => m.AdministratorID).FirstOrDefault();
                    substr = Administrator.AdministratorID.Substring(2, 13);
                    word = "A0";
                    break;
                case "Record":
                    var Record = dbSF.Record.OrderByDescending(m => m.QuestionID).FirstOrDefault();
                    substr = Record.QuestionID.Substring(2, 13);
                    word = "Q0";
                    break;
            }

            if (table == null)
            {
                strval = word + "2000000000001";
            }
            else
            {
                long intVal = (Convert.ToInt64(substr)) + 1;
                strval = word + intVal.ToString();
            }

            return strval;
        }
    }
}