using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PathPlus.Controllers
{
    public class AdministratorLoginController : Controller
    {
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PathPlusConnectionString"].ConnectionString);

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string id, string pwd)
        {
            string sql = "select * from Administrator where Account=@id and password=@pwd";

            SqlCommand cmd = new SqlCommand(sql, Conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@pwd", pwd);

            SqlDataReader rd;

            Conn.Open();
            rd = cmd.ExecuteReader();

            if (rd.Read())
            {
                Session["Account"] = rd["Account"].ToString();
                Session["Name"] = rd["Name"].ToString();
                Session["ID"] = rd["AdministratorID"].ToString();

                Conn.Close();
                return RedirectToAction("checkCaptcha", "AdministratorLogin");
            }
            else
            {
                ViewBag.Msg = "帳號或密碼有誤";
            }

            Conn.Close();

            return View();
        }

        public ActionResult Logout()
        {
            Session["Account"] = null;
            Session["Name"] = null;
            Session["ID"] = null;
            Session.Clear();

            return RedirectToAction("Login");
        }

        public ActionResult checkCaptcha()
        {
            return View();
        }

        [HttpPost]
        public ActionResult checkCaptcha(string ValidationCode)
        {
            if (ValidationCode == Session["Code"].ToString())
            {
                return RedirectToAction("Index", "AdministratorHome");
            }
            else
            {
                ViewBag.CodeErr = "驗證碼錯誤!!";
            }
            return View();
        }

        public ActionResult getCaptcha()
        {
            string[] arrLetter = { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q",
                "R","S","T","W","X","Y","a","b","c","d","e","f","g","h","j","k","m","n","p","r","s","t","w","x",
                "y","2","3","4","5","6","7","8","9"};

            Random r = new Random();
            string strCode = "";
            int a = 0;
            for (int i = 0; i < 6; i++)
            {
                a = r.Next(arrLetter.Length);
                strCode += arrLetter[a];
            }

            Session["Code"] = strCode;

            Bitmap img = new Bitmap(280, 80);
            Graphics g = Graphics.FromImage(img);

            int intRed = r.Next(0, 256);
            int intGreen = r.Next(0, 256);
            int intBlue = r.Next(0, 256);
            g.Clear(Color.FromArgb(10, intRed, intGreen, intBlue));

            int x1, x2, y1, y2;
            for (int i = 0; i < 50; i++)
            {
                x1 = r.Next(img.Width);
                x2 = r.Next(img.Width);
                y1 = r.Next(img.Height);
                y2 = r.Next(img.Height);

                g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
            }

            for (int i = 0; i < 500; i++)
            {
                x1 = r.Next(img.Width);

                y1 = r.Next(img.Height);


                img.SetPixel(x1, y1, Color.FromArgb(r.Next(256)));
            }

            intRed = r.Next(0, 256);
            intGreen = r.Next(0, 256);
            intBlue = r.Next(0, 256);
            Color color1 = Color.FromArgb(intRed, intGreen, intBlue);

            intRed = r.Next(0, 256);
            intGreen = r.Next(0, 256);
            intBlue = r.Next(0, 256);
            Color color2 = Color.FromArgb(intRed, intGreen, intBlue);

            Rectangle MyRect = new Rectangle(0, 0, img.Width, img.Height);

            Font font = new Font("Arial Black", 40, FontStyle.Bold);
            System.Drawing.Drawing2D.LinearGradientBrush brush =
                new System.Drawing.Drawing2D.LinearGradientBrush(MyRect, color1, color2, 1f);

            g.DrawString(strCode, font, brush, 5, 5);

            Image image = img;
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

            return File(ms.GetBuffer(), "image/jpeg");
        }
    }
}