using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using System.Data;
using System.Data.SqlClient;

using System.Configuration;
namespace PathPlus.Controllers
{
    public class LoginController : Controller
    {
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PathPlusConnectionString"].ConnectionString);
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string account, string pwd)
        {

            //用帳號密碼去抓該筆會員資料
            string sql = "select * from member where Account=@account and [Password]=@pwd";
            SqlCommand cmd = new SqlCommand(sql, Conn);
            cmd.Parameters.AddWithValue("@account", account);
            cmd.Parameters.AddWithValue("@pwd", pwd);

            SqlDataReader rd;

            Conn.Open();
            rd = cmd.ExecuteReader();

            //PathPlus5Entities db = new PathPlus5Entities();
            //db.Member.Where(m => m.Account == account && m.Password == pwd).FirstOrDefault().MemberName != null

            //var r=db.Member.Where(m => m.Account == account && m.Password == pwd).FirstOrDefault();

            //判斷是否有資料(表示帳號密碼正確)
            if (rd.Read())
            {
                //把會員ID寫進Session
                Session["account"] = rd["MemberID"].ToString();

                Conn.Close();

                //轉回首頁
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Msg = "帳號或密碼錯誤";
            }

            Conn.Close();

            //如果錯誤回到登入頁
            return View();
        }
        public ActionResult Logout()
        {
            //清空登入資訊(清除Session)
            Session["account"] = null;

            //回到Login頁面
            return RedirectToAction("Index");
        }

        public ActionResult Registered()
        {
            return View();
        }

        public void aaa()
        {
            Response.Write(DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.sss"));
            Response.Write(Convert.ToDateTime(DateTime.Now).ToString("u").Substring(0, 19));
        }
        
        [HttpPost]
        public ActionResult Registered(string Name, string Mail, string Account, string Password, string address, string gender)
        {
            string sql = "insert into Member values(@MID,@Name,@Mail,@Account,@Password,@gender,'','','',@address,getdate(),0,0)";
            SqlCommand Cmd = new SqlCommand();
            
            Cmd.Parameters.AddWithValue("@Name", Name);
            Cmd.Parameters.AddWithValue("@Mail", Mail);
            Cmd.Parameters.AddWithValue("@Account", Account);
            Cmd.Parameters.AddWithValue("@Password", Password);
            Cmd.Parameters.AddWithValue("@gender", gender);
            Cmd.Parameters.AddWithValue("@address", address);

            SelfFeature getMid = new SelfFeature();
            string MID = getMid.GetID("Member");
            Cmd.Parameters.AddWithValue("@MID", MID);

            Cmd.CommandText = sql;
            Cmd.Connection = Conn;
            try
            {
                Conn.Open();
                Cmd.ExecuteNonQuery();

                Conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
           

            return RedirectToAction("Index");
        }

        //[HttpPost]
        public string accCheck(string AID)
        {

            string sql = "select * from member where Account=@account";
            SqlCommand cmd = new SqlCommand(sql, Conn);
            cmd.Parameters.AddWithValue("@account", AID);


            SqlDataReader rd;

            Conn.Open();
            rd = cmd.ExecuteReader();

            if (rd.Read())
            {
                Conn.Close();

                return "已重複";
            }

            Conn.Close();

            return "沒有重複";
        }

        public string RegistMailCheck(string Mail)
        {

            string sql = "select * from member where Mail=@Mail";
            SqlCommand cmd = new SqlCommand(sql, Conn);
            cmd.Parameters.AddWithValue("@Mail", Mail);


            SqlDataReader rd;

            Conn.Open();
            rd = cmd.ExecuteReader();

            if (rd.Read())
            {


                Conn.Close();
                ViewBag.Msg = "成功";
                return "已重複";
            }


            Conn.Close();


            return "沒有重複";
        }

    }
}