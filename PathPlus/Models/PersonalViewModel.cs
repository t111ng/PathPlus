using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PathPlus.Models
{
    public class PersonalViewModel
    {
        public List<Member> member { get; set; }
        public List<Post> post { get; set; }
        public List<PostPhoto> postPhoto { get; set; }
        public List<Comment> comment { get; set; }
    }
}