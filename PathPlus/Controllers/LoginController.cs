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


            if (rd.Read())
            {
                //ViewBag.Msg = "登入成功";
                Session["account"] = rd["MemberID"].ToString();
                //["pwd"] = rd["pwd"].ToString();

                Conn.Close();
                ViewBag.Msg = "成功";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Msg = "帳號或密碼錯誤";
            }

            Conn.Close();


            return View();
        }
        public ActionResult Logout()
        {
            Session["account"] = null;


            return RedirectToAction("Index");
        }

        public ActionResult Registered()
        {
            return View();
        }
        public void Registereda()
        {
            Response.Write(DateTime.Now.ToString("yyyy/MM/dd"));
        }
        [HttpPost]
        public ActionResult Registered(string Name, string Mail, string Account, string Password, string address, string gender)
        {
            SelfFeature sf = new SelfFeature();
            string MID = sf.GetID("Member");
            String date = DateTime.Now.ToString("yyyy/MM/dd");
            string sql = "insert into Member values('@MID',@Name,@Mail,@Account,@Password,@gender,'','','',@address,getdate(),0,0)";
            SqlCommand Cmd = new SqlCommand();
            Cmd.Parameters.AddWithValue("@MID", MID);
            Cmd.Parameters.AddWithValue("@Name", Name);
            Cmd.Parameters.AddWithValue("@Mail", Mail);
            Cmd.Parameters.AddWithValue("@Account", Account);
            Cmd.Parameters.AddWithValue("@Password", Password);
            Cmd.Parameters.AddWithValue("@gender", gender);
            Cmd.Parameters.AddWithValue("@address", address);

            Cmd.CommandText = sql;
            Cmd.Connection = Conn;

            Conn.Open();
            Cmd.ExecuteNonQuery();

            Conn.Close();

            return RedirectToAction("Index");
        }

        //[HttpPost]
        public string RegistCheck(string account)
        {

            string sql = "select * from member where Account=@account";
            SqlCommand cmd = new SqlCommand(sql, Conn);
            cmd.Parameters.AddWithValue("@account", account);


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