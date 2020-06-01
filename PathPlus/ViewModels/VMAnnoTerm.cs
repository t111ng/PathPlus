using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PathPlus.Models;
namespace PathPlus.ViewModels
{
    public class VMAnnoTerm
    {
        public List<Announcement> Announcement { get; set; }
        public List<Term> Term { get; set; }
    }
}