using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PathPlus.Models
{
    public class GroupViewModel
    {
        public List<Group> group { get; set; }
        public List<GroupManagement> groupmanagement { get; set; }
        public List<GroupPost> grouppost { get; set; }
        public List<GroupPostPhoto> grouppostphotos { get; set; }

        public List<CommentGroupPost> commentgroupposts { get; set; }
    }
}