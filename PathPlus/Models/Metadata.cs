using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Main.Models
{
    //namespace PathPlus.Models
    //{
    public class MetaAdministrator
    {
        [DisplayName("管理者編號")]
        [Required]
        [RegularExpression("[A-Za-z0-9]{15}")]
        public string AdministratorID { get; set; }

        [DisplayName("帳號")]
        [Required(ErrorMessage = "請輸入帳號")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "長度限制為3-15個字")]
        [RegularExpression(@"\w{1,}", ErrorMessage = "必須為英數字或底線")]
        public string Account { get; set; }

        [DisplayName("密碼")]
        [Required(ErrorMessage = "請輸入密碼")]
        [StringLength(12, ErrorMessage = "長度限制6-12個字", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-zA-Z]).{2,255}$", ErrorMessage = "密碼強度不足, 請至少包含數字及英文字母")]
        public string Password { get; set; }

        [DisplayName("姓名")]
        [Required(ErrorMessage = "請輸入姓名")]
        [StringLength(30, ErrorMessage = "長度限制30個字")]
        public string Name { get; set; }

        [DisplayName("性別")]
        public bool? Gender { get; set; }

        [DisplayName("連絡電話")]
        [Required(ErrorMessage = "請輸入連絡電話")]
        [StringLength(20, ErrorMessage = "請輸入正確電話號碼", MinimumLength = 8)]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [DisplayName("職稱")]
        [Required(ErrorMessage = "請輸入職稱")]
        [StringLength(15)]
        public string Tittle { get; set; }

        [DisplayName("權限")]
        public string AuthorityCategoryID { get; set; }

        [DisplayName("登錄日期")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreateDate { get; set; }
    }
    public partial class MetaAdministratorAuthorityCategory
    {
        [DisplayName("管理者權限編號")]
        [Required]
        public string AuthorityCategoryID { get; set; }

        [DisplayName("權限內容")]
        [Required]
        [StringLength(20, ErrorMessage = "長度限制20個字")]
        public string AuthorityCategoryName { get; set; }
    }
    public partial class MetaAdvertisemenStatusCategory
    {
        [DisplayName("廣告狀態編號")]
        [Required]
        public string AdStatusCategoryID { get; set; }

        [DisplayName("狀態")]
        [Required]
        [StringLength(20, ErrorMessage = "長度限制20個字")]
        public string AdStatusCategoryName { get; set; }
    }
    public partial class MetaAdvertisement
    {
        [DisplayName("廣告編號")]
        [Required]
        [RegularExpression("[A-Za-z0-9]{15}")]
        public string AdvertisementID { get; set; }

        [DisplayName("文字")]
        [DataType(DataType.MultilineText)]
        public string AdText { get; set; }

        [DisplayName("地區")]
        [StringLength(30, ErrorMessage = "長度限制30個字")]
        public string City { get; set; }

        [DisplayName("年齡範圍")]
        [StringLength(6, ErrorMessage = "長度限制6個字")]
        public string AgeRange { get; set; }

        [DisplayName("性別")]
        public bool? Gender { get; set; }

        [DisplayName("金額")]
        [Range(0, short.MaxValue, ErrorMessage = "所填金額超過範圍")]
        [DataType(DataType.Currency)]
        public decimal? Money { get; set; }

        [DisplayName("開始日期")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DisplayName("時效")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime Limitation { get; set; }

        [DisplayName("結束日期")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime ExpireDate { get; set; }

        [DisplayName("廣告主")]
        [Required]
        public string CompanyID { get; set; }

        [DisplayName("狀態")]
        [Required]
        [DefaultValue("1")]
        public string AdStatusCategoryID { get; set; }
    }
    public partial class MetaAdvertisementPhoto
    {
        [DisplayName("廣告圖流水號")]
        [Required]
        public long AdPhotoSN { get; set; }

        [DisplayName("圖片")]
        [Required]
        [StringLength(100)]
        public string Photo { get; set; }

        [DisplayName("廣告")]
        [Required]
        public string AdvertisementID { get; set; }
    }
    public partial class MetaAdvertisers
    {
        [DisplayName("廣告主代碼")]
        [Required]
        [RegularExpression("[A-Za-z0-9]{15}")]
        public string CompanyID { get; set; }

        [DisplayName("公司名稱")]
        [StringLength(30, ErrorMessage = "長度限制30個字")]
        public string CompanyName { get; set; }

        [DisplayName("聯絡人")]
        [StringLength(30, ErrorMessage = "長度限制30個字")]
        public string ContactName { get; set; }

        [DisplayName("電話")]
        [StringLength(20, ErrorMessage = "長度限制20個字")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [DisplayName("地址")]
        [StringLength(30, ErrorMessage = "長度限制30個字")]
        public string Address { get; set; }

        [DisplayName("信箱")]
        [StringLength(65, ErrorMessage = "長度限制65個字")]
        [EmailAddress(ErrorMessage = "請輸入正確電子信箱")]
        [DataType(DataType.EmailAddress)]
        public string Mail { get; set; }
    }
    public partial class MetaAnnouncement
    {
        [DisplayName("公告編號")]
        [Required]
        public string AnnouncementID { get; set; }

        [DisplayName("內容")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [DisplayName("發佈日期")]
        [Required]
        [DataType(DataType.DateTime)]
        public System.DateTime PostDate { get; set; }

        [DisplayName("修改日期")]
        [Required]
        [DataType(DataType.DateTime)]
        public System.DateTime EditDate { get; set; }

        [DisplayName("撤銷日期")]
        [Required]
        [DataType(DataType.DateTime)]
        public System.DateTime RevokeDate { get; set; }

        [DisplayName("最後編輯者")]
        [Required]
        public string Editor { get; set; }

        [DisplayName("狀態")]
        [Required]
        [DefaultValue("0")]
        public string StatusCategoryID { get; set; }
    }
    public partial class MetaAnnouncementStatusCategory
    {
        [DisplayName("公告狀態編號")]
        [Required]
        public string StatusCategoryID { get; set; }

        [DisplayName("狀態")]
        [Required(ErrorMessage = "請輸入狀態內容")]
        [StringLength(20, ErrorMessage = "長度限制20個字")]
        public string StatusCategoryName { get; set; }
    }
    public partial class MetaCard
    {
        [DisplayName("卡號")]
        [Required]
        [RegularExpression("[A-Za-z0-9]{15}")]
        public string CardID { get; set; }

        [DisplayName("興趣專長")]
        [StringLength(100, ErrorMessage = "長度限制100個字")]
        [DataType(DataType.MultilineText)]
        public string Interests { get; set; }

        [DisplayName("照片")]
        [StringLength(100)]
        public string Photo { get; set; }

        [DisplayName("性別")]
        public bool? Gender { get; set; }

        [DisplayName("會員")]
        [Required]
        public string MemberID { get; set; }

        [DisplayName("狀態內容")]
        [Required]
        [DefaultValue("1")]
        public string CardStatusID { get; set; }
    }
    public partial class MetaCardStatusCategory
    {
        [DisplayName("卡片狀態編號")]
        [Required]
        public string CardStatusID { get; set; }

        [DisplayName("狀態內容")]
        [Required(ErrorMessage = "請輸入狀態內容")]
        [StringLength(20, ErrorMessage = "長度限制20個字")]
        public string CardStatusName { get; set; }
    }
    public partial class MetaComment
    {
        [DisplayName("會員")]
        [Required]
        public string MemberID { get; set; }

        [DisplayName("貼文")]
        [Required]
        public string PostID { get; set; }

        [DisplayName("讚")]
        [Required]
        [DefaultValue(false)] //預設沒點讚(false)
        public bool Like { get; set; }

        [DisplayName("收藏日期")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime SaveDate { get; set; }

        [DisplayName("分享日期")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ShareDate { get; set; }

        [DisplayName("檢舉日期")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ReportDate { get; set; }

        [DisplayName("檢舉原因")]
        [DataType(DataType.MultilineText)]
        public string Reason { get; set; }

        [DisplayName("留言時間")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime MessageDate { get; set; }

        [DisplayName("留言內容")]
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }
    }
    public partial class MetaCommentGroupPost
    {
        [DisplayName("會員")]
        [Required]
        public string MemberID { get; set; }

        [DisplayName("社團貼文")]
        [Required]
        public string GroupPostID { get; set; }

        [DisplayName("讚")]
        [Required]
        [DefaultValue(false)] //預設沒點讚(false)
        public bool Like { get; set; }

        [DisplayName("收藏日期")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime SaveDate { get; set; }

        [DisplayName("分享日期")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ShareDate { get; set; }

        [DisplayName("檢舉日期")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ReportDate { get; set; }

        [DisplayName("檢舉原因")]
        [DataType(DataType.MultilineText)]
        public string Reason { get; set; }

        [DisplayName("留言時間")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CommentDate { get; set; }

        [DisplayName("留言內容")]
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }
    }
    public partial class MetaDraw
    {
        [DisplayName("會員")]
        [Required]
        public string MemberID { get; set; }

        [DisplayName("卡號")]
        [Required]
        public string CardID { get; set; }

        [DisplayName("抽卡日期")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [DisplayName("已抽")]
        [Required]
        public string DrawMemberID { get; set; }

        [DisplayName("喜歡狀態")]
        [Required]
        public bool LikeStatus { get; set; }
    }
    public partial class MetaGroup
    {
        [DisplayName("社團編號")]
        [Required]
        [RegularExpression("[A-Za-z0-9]{15}")]
        public string GroupID { get; set; }

        [DisplayName("社團名稱")]
        [Required(ErrorMessage = "請輸入社團名稱")]
        [StringLength(30, ErrorMessage = "長度限制30個字")]
        public string GroupName { get; set; }

        [DisplayName("照片")]
        [StringLength(100)]
        public string Photo { get; set; }

        [DisplayName("社團簡介")]
        [StringLength(100, ErrorMessage = "長度限制100個字")]
        [DataType(DataType.MultilineText)]
        public string GroupIntroduction { get; set; }

        [DisplayName("社團資訊")]
        [StringLength(100, ErrorMessage = "長度限制100個字")]
        [DataType(DataType.MultilineText)]
        public string GroupInformation { get; set; }

        [DisplayName("創建日期")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreateDate { get; set; }

        [DisplayName("會員")]
        [Required]
        public string MemberID { get; set; }

        [DisplayName("隱私狀態")]
        [DefaultValue("0")]
        public string PrivateCategoryID { get; set; }
    }
    public partial class MetaGroupAuthorityCategory
    {
        [DisplayName("權限類別編號")]
        [Required(ErrorMessage = "請輸入社團管理權限類別編號")]
        public string AuthorityCategoryID { get; set; }

        [DisplayName("權限內容")]
        [Required(ErrorMessage = "請輸入權限內容")]
        [StringLength(20, ErrorMessage = "長度限制20個字")]
        public string AuthorityCategotyName { get; set; }
    }
    public partial class MetaGroupManagement
    {
        [DisplayName("會員")]
        [Required]
        public string MemberID { get; set; }

        [DisplayName("社團")]
        [Required]
        public string GroupID { get; set; }

        [DisplayName("加入管理日")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ManageDate { get; set; }

        [DisplayName("權限")]
        public string AuthorityCategoryID { get; set; }
    }
    public partial class MetaGroupPost
    {
        [DisplayName("社團貼文編號")]
        [Required]
        [RegularExpression("[A-Za-z0-9]{15}")]
        public string GroupPostID { get; set; }

        [DisplayName("文字內容")]
        [StringLength(100, ErrorMessage = "長度限制100個字")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [DisplayName("上傳時間")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime PostDate { get; set; }

        [DisplayName("修改時間")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EditDate { get; set; }

        [DisplayName("會員")]
        [Required]
        public string MemberID { get; set; }

        [DisplayName("社團")]
        [Required]
        public string GroupID { get; set; }

        [DisplayName("狀態")]
        [Required]
        [DefaultValue("0")]
        public string StatusCategoryID { get; set; }
    }
    public partial class MetaGroupPostPhoto
    {
        [DisplayName("社團貼文照片流水號")]
        [Required]
        public long PostPhotoSN { get; set; }

        [DisplayName("圖片")]
        [Required]
        [StringLength(100, ErrorMessage = "長度限制100個字")]
        public string Photo { get; set; }

        [DisplayName("社團貼文")]
        [Required]
        public string GroupPostID { get; set; }
    }
    public partial class MetaGroupPostStatusCategory
    {
        [DisplayName("社團貼文狀態編號")]
        [Required]
        public string StatusCategoryID { get; set; }

        [DisplayName("狀態內容")]
        [Required(ErrorMessage = "請輸入狀態內容")]
        [StringLength(20, ErrorMessage = "長度限制20個字")]
        public string StatusCategoryName { get; set; }
    }
    public partial class MetaGroupPrivateCategory
    {
        [DisplayName("社團隱私狀態編號")]
        [Required]
        public string PrivateCategoryID { get; set; }

        [DisplayName("隱私狀態內容")]
        [Required(ErrorMessage = "請輸入隱私狀態內容")]
        [StringLength(20, ErrorMessage = "長度限制20個字")]
        public string PrivateCategoryName { get; set; }
    }
    public partial class MetaJoinGroup
    {
        [DisplayName("會員")]
        [Required]
        public string MemberID { get; set; }

        [DisplayName("社團")]
        [Required]
        public string GroupID { get; set; }

        [DisplayName("加入日期")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime JoinDate { get; set; }
    }
    public partial class MetaMember
    {
        [DisplayName("會員編號")]
        [Required]
        [RegularExpression("[A-Za-z0-9]{15}")]
        public string MemberID { get; set; }

        [DisplayName("會員名稱")]
        [Required(ErrorMessage = "請輸入會員名稱")]
        [StringLength(30, ErrorMessage = "長度限制30個字")]
        public string MemberName { get; set; }

        [DisplayName("信箱")]
        [Required(ErrorMessage = "請輸入電子信箱")]
        [StringLength(64, ErrorMessage = "長度限制64個字")]
        [EmailAddress(ErrorMessage = "請輸入正確電子信箱")]
        [DataType(DataType.EmailAddress)]
        public string Mail { get; set; }

        [DisplayName("帳號")]
        [Required(ErrorMessage = "請輸入帳號")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "長度限制為3-15個字")]
        [RegularExpression(@"\w{1,}", ErrorMessage = "必須為英數字或底線")]
        public string Account { get; set; }

        [DisplayName("密碼")]
        [Required(ErrorMessage = "請輸入密碼")]
        [StringLength(12, ErrorMessage = "長度限制6-12個字", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-zA-Z]).{2,255}$", ErrorMessage = "密碼強度不足, 請至少包含數字及英文字母")]
        public string Password { get; set; }

        [DisplayName("性別")]
        public bool? Gender { get; set; }

        [DisplayName("生日")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; set; }

        [DisplayName("個人簡介")]
        [DataType(DataType.MultilineText)]
        [DefaultValue("你好，我是...")]
        public string PersonalProfile { get; set; }

        [DisplayName("大頭貼照")]
        [StringLength(100, ErrorMessage = "長度限制100個字")]
        public string Photo { get; set; }

        [DisplayName("地址")]
        [StringLength(50, ErrorMessage = "長度限制50個字")]
        public string Address { get; set; }

        [DisplayName("註冊日期")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime RegisteredDate { get; set; }

        [DisplayName("貼文數")]
        [Required]
        [DefaultValue(0)]
        public int Postcount { get; set; }

        [DisplayName("粉絲數")]
        [Required]
        [DefaultValue(0)]
        public int Fans { get; set; }

        [DisplayName("追蹤數")]
        [Required]
        [DefaultValue(0)]
        public int Follower { get; set; }

        [DisplayName("隱私狀態")]
        [Required]
        [DefaultValue("0")]
        public string PrivateCategoryID { get; set; }

        [DisplayName("帳號狀態")]
        [Required]
        [DefaultValue("0")]
        public string AccountStatusID { get; set; }
    }
    public partial class MetaMemberAccountStatusCategory
    {
        [DisplayName("帳號狀態編號")]
        [Required]
        public string AccountStatusID { get; set; }

        [DisplayName("帳號狀態內容")]
        [Required(ErrorMessage = "請輸入帳號狀態內容")]
        [StringLength(20, ErrorMessage = "長度限制20個字")]
        public string AccountStatusName { get; set; }
    }
    public partial class MetaMemberPrivateCategory
    {
        [DisplayName("隱私狀態編號")]
        [Required]
        public string PrivateCategoryID { get; set; }

        [DisplayName("隱私狀態內容")]
        [Required(ErrorMessage = "請輸入隱私狀態內容")]
        [StringLength(20, ErrorMessage = "長度限制20個字")]
        public string PrivateCategoryName { get; set; }
    }
    public partial class MetaMessage
    {
        [DisplayName("會員聊天流水號")]
        [Required]
        public long MessageSN { get; set; }

        [DisplayName("發出會員")]
        [Required]
        public string MemberID { get; set; }

        [DisplayName("接收會員")]
        [Required]
        public string AcceptMemberID { get; set; }

        [DisplayName("訊息內容")]
        [Required]
        [DataType(DataType.MultilineText)]
        public string MessageContent { get; set; }

        [DisplayName("日期")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime MessageDate { get; set; }
    }
    public partial class MetaPost
    {
        [DisplayName("貼文編號")]
        [Required]
        [RegularExpression("[A-Za-z0-9]{15}")]
        public string PostID { get; set; }

        [DisplayName("文字內容")]
        [DataType(DataType.MultilineText)]
        public string PostContent { get; set; }

        [DisplayName("上傳時間")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime PostDate { get; set; }

        [DisplayName("修改時間")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EditDate { get; set; }

        [DisplayName("會員")]
        [Required]
        public string MemberID { get; set; }

        [DisplayName("類別編號")]
        [Required(ErrorMessage = "請輸入類別")]
        public string CategoryID { get; set; }

        [DisplayName("狀態")]
        [Required]
        [DefaultValue("0")]
        public string StatusCategoryID { get; set; }
    }
    public partial class MetaPostCategory
    {
        [DisplayName("貼文類別編號")]
        [Required]
        public string CategoryID { get; set; }

        [DisplayName("類別名稱")]
        [Required(ErrorMessage = "請輸入貼文類別名稱")]
        [StringLength(30, ErrorMessage = "長度限制30個字")]
        public string CategoryName { get; set; }
    }
    public partial class MetaPostPhoto
    {
        [DisplayName("貼文附圖流水號")]
        [Required]
        public long PostPhotoSN { get; set; }

        [DisplayName("圖片")]
        [Required]
        [StringLength(100, ErrorMessage = "長度限制100個字")]
        public string Photo { get; set; }

        [DisplayName("貼文")]
        [Required]
        public string PostID { get; set; }
    }
    public partial class MetaPostStatusCategory
    {
        [DisplayName("貼文狀態編號")]
        [Required]
        public string StatusCategoryID { get; set; }

        [DisplayName("貼文狀態內容")]
        [Required(ErrorMessage = "請輸入貼文狀態內容")]
        [StringLength(20, ErrorMessage = "長度限制20個字")]
        public string StatusCategoryName { get; set; }
    }
    public partial class MetaRecord
    {
        [DisplayName("問題編號")]
        [Required]
        [RegularExpression("[A-Za-z0-9]{15}")]
        public string QuestionID { get; set; }

        [DisplayName("會員")]
        public string MemberID { get; set; }

        [DisplayName("貼文")]
        public string PostID { get; set; }

        [DisplayName("處理日期")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ProcessingDate { get; set; }

        [DisplayName("回覆訊息")]
        [DataType(DataType.MultilineText)]
        public string ReplyMessage { get; set; }
    }
    public partial class MetaRelationship
    {
        [DisplayName("會員關係流水號")]
        [Required]
        public long RelationshipSN { get; set; }

        [DisplayName("追蹤")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime FollowDate { get; set; }

        [DisplayName("封鎖")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime BlockDate { get; set; }

        [DisplayName("檢舉日期")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime ReportDate { get; set; }

        [DisplayName("檢舉原因")]
        [DataType(DataType.MultilineText)]
        public string Reason { get; set; }

        [DisplayName("被檢舉會員")]
        [Required]
        public string ReportMemberID { get; set; }
    }
    public partial class MetaTerm
    {
        [DisplayName("條款編號")]
        [Required]
        public string TermID { get; set; }

        [DisplayName("內容")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [DisplayName("發佈日期")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime PostDate { get; set; }

        [DisplayName("修改日期")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EditDate { get; set; }

        [DisplayName("撤銷日期")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime RevokeDate { get; set; }

        [DisplayName("最後編輯者")]
        [Required]
        public string Editor { get; set; }

        [DisplayName("狀態")]
        [Required]
        [DefaultValue("0")]
        public string StatusCategoryID { get; set; }
    }
    public partial class MetaTermStatusCategory
    {
        [DisplayName("狀態編號")]
        [Required]
        public string StatusCategoryID { get; set; }

        [DisplayName("狀態")]
        [Required(ErrorMessage = "請輸入狀態內容")]
        [StringLength(20, ErrorMessage = "長度限制20個字")]
        public string StatusCategoryName { get; set; }
    }


}