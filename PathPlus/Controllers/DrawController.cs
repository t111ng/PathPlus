using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using System.Collections;

using System.Data;
using System.Data.SqlClient;

using System.Configuration;
namespace PathPlus.Controllers
{
    public class DrawController : Controller
    {
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PathPlusConnectionString"].ConnectionString);
        SqlCommand Cmd = new SqlCommand();
        SqlDataAdapter adp = new SqlDataAdapter();

        private void CmdClose()
        {
            Conn.Close();
        }
        private SqlDataReader execCmdReader(string sql)
        {
            Cmd.CommandText = sql;
            Cmd.Connection = Conn;

            SqlDataReader rd;

            Conn.Open();

            rd = Cmd.ExecuteReader();
            return rd;

        }
        private void executeSql(string sql)
        {
            Cmd.CommandText = sql;
            Cmd.Connection = Conn;

            Conn.Open();
            Cmd.ExecuteNonQuery();

            Conn.Close();


        }

        private DataSet AdpDs(string sql)
        {
            Cmd.CommandText = sql;
            Cmd.Connection = Conn;
            adp.SelectCommand = Cmd;

            DataSet ds = new DataSet();
            adp.Fill(ds);

            return ds;
        }
        private DataTable AdpDt(string sql)
        {
            Cmd.CommandText = sql;
            Cmd.Connection = Conn;
            adp.SelectCommand = Cmd;

            DataSet ds = new DataSet();
            adp.Fill(ds);

            return ds.Tables[0];
        }
        public ActionResult Index()
        {
            string mID = Session["account"].ToString();
            string sql = "select * from Draw where MemberID=@mID";
            SqlCommand cmd = new SqlCommand(sql, Conn);
            cmd.Parameters.AddWithValue("@mID", mID);

            SqlDataReader rd;

            Conn.Open();
            rd = cmd.ExecuteReader();

            if (rd.Read())
            {

                string DrawID = rd["DrawMemberID"].ToString();
                Conn.Close();

                sql = "select Interests,Photo,Gender from Card where MemberID=@MemberID";
                cmd = new SqlCommand(sql, Conn);
                cmd.Parameters.AddWithValue("@MemberID", DrawID);

                Conn.Open();
                rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    Response.Write(rd["Interests"].ToString());
                    Response.Write(rd["Photo"].ToString());
                    Response.Write(rd["Gender"].ToString());
                    ViewBag.Interests = rd["Interests"].ToString();
                    ViewBag.Photo = rd["Photo"].ToString();
                    ViewBag.Gender = (rd["Gender"].ToString() == "1") ? "男生" : "女生";

                    Conn.Close();
                }

            }
            Conn.Close();

            return View();
        }

        public ActionResult likeStatus(string LikeStatus)
        {
            //Conn.Close();
            string sql = "update Draw set PairingStatusID=@LikeStatus where MemberID=@mID";
            string mID = Session["account"].ToString();
            SqlCommand cmd = new SqlCommand(sql, Conn);

            cmd.Parameters.AddWithValue("@LikeStatus", LikeStatus);
            cmd.Parameters.AddWithValue("@mID", mID);

            Conn.Open();

            cmd.ExecuteNonQuery();

            Conn.Close();
            return RedirectToAction("Index", "Home");
        }
        public void Draw()
        {

            //SqlCommand cmd = new SqlCommand(sql, Conn);
            //SqlDataReader rd;
            //Conn.Open();
            //rd = cmd.ExecuteReader();

            //存放可抽卡者MeberID
            string sqlCount = "select Count(*) as [count] from [Card]";
            var joinSqlCount = execCmdReader(sqlCount);
            int joinSqlCounts = 0;
            while (joinSqlCount.Read())
            {
                joinSqlCounts = int.Parse(joinSqlCount["count"].ToString());
            }
            
            string[] joinDraw = new string[joinSqlCounts];
            //計數器
            CmdClose();
            //抓取所有可參與抽卡者的SQL
            string sql = "select MemberID from [Card] where CardStatusID=0";

            //執行抓取動作
            var rd = execCmdReader(sql);

            int joinCount = 0;
            while (rd.Read())
            {
                joinDraw[joinCount] = rd["MemberID"].ToString();
                joinCount++;


            }
            //關閉連線
            CmdClose();

            //資料庫抓的欄位
            //string[] b = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16" };

            //存放配對完成MemberID，joinDraw.Length / 2比對次數
            string[] completePair = new string[joinDraw.Length / 2];
            //迴圈是否繼續做的判斷
            bool flagLoop = true;
            //產生亂數物件
            Random r = new Random();
            //判斷已經執行幾次配對
            int count = 0;
            //未配對人數
            int n = joinDraw.Length;
            //要做去除已配對的物件
            ArrayList numbers = new ArrayList(joinDraw);
            //切割的字
            string[] words = new string[2];

            //完成配對寫進資料庫的預先儲存內容
            sql = "select * from Draw";
            var ds = AdpDs(sql);

            DataRow dr;

            while (flagLoop)
            {
                //產生亂數
                int a = r.Next(0, n);

                //產生的序列如果不是0就執行，joinDraw[0]
                if (a != 0)
                {
                    //將配對的人寫到陣列裡
                    completePair[count] = numbers[0].ToString() + "," + numbers[a].ToString();
                    //增加配對次數
                    count++;
                    //因配對成功，故參加配對的人減少
                    n = n - 2;
                    //移除已配對到的人
                    numbers.RemoveAt(a);
                    numbers.RemoveAt(0);
                }
                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
                //檢查配對是否完成
                if (count == joinDraw.Length / 2)
                {
                    for (int i = 0; i < completePair.Length; i++)
                    {
                        //將配對到一組的人從同索引值裡分開
                        words = completePair[i].Split(',');
                        string CID1="", CID2="";

                        string sqlCID = "select CardID from [Card] where MemberID='" + words[0] + "'";
                        var rdCID1 = execCmdReader(sqlCID);
                        while (rdCID1.Read())
                        {
                            CID1 = rdCID1["CardID"].ToString();
                        }
                        CmdClose();

                        string sqlCID2 = "select CardID from [Card] where MemberID='" + words[1] + "'";
                        var rdCID2 = execCmdReader(sqlCID2);
                        while (rdCID2.Read())
                        {
                            CID2 = rdCID2["CardID"].ToString();
                        }
                        CmdClose();
                        //產生一個配對到的資料行
                        dr = ds.Tables[0].NewRow();
                        dr[0] = words[0];
                        dr[1] = CID1;
                        dr[2] = DateTime.Now.ToString("D");
                        dr[3] = words[1];
                        dr[4] = "2";

                        ds.Tables[0].Rows.Add(dr);

                        dr = ds.Tables[0].NewRow();
                        dr[0] = words[1];
                        dr[1] = CID2;
                        dr[2] = DateTime.Now.ToString("D");
                        dr[3] = words[0];
                        dr[4] = "2";

                        ds.Tables[0].Rows.Add(dr);

                        //這是要刪除的範例
                        //ds.Tables[0].Rows[3.Remove();
                        //檢查配對內容
                        //Response.Write(words[0] + "<hr/>" + words[1] + "<hr/>");
                    }

                    //建造一個自動判斷增刪改的物件
                    SqlCommandBuilder obj = new SqlCommandBuilder(adp);
                    //執行動作
                    try
                    {
                        adp.Update(ds);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                     
                    
                    
                    //跳出迴圈
                    flagLoop = false;
                }

            }
        }
    }
}