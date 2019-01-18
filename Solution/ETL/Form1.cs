using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ETL
{
    public partial class Form1 : Form
    {
        private MySqlConnection 원본, 대상;
        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Click += btn_event;
            button2.Click += btn_event;
            button3.Click += btn_event;
        }

        private void btn_event(object o, EventArgs a)
        {
            switch (((Button)o).Name)
            {
                case "button1":
                    btn_stat(개발(), (Button)o);
                    break;
                case "button2":
                    btn_stat(운영(), (Button)o);
                    break;
                case "button3":
                    이행();
                    break;
                default:
                    break;
            }
        }

        private void btn_stat(bool check, Button btn)
        {
            if (check)
            {
                btn.BackColor = Color.BlueViolet;
                btn.Enabled = false;
            }
            else
            {
                btn.BackColor = Color.OrangeRed;
                btn.Enabled = true;
            }
        }

        private bool 이행()
        {
            if (button1.BackColor == Color.BlueViolet && button2.BackColor == Color.BlueViolet)
            {
                /* 1. 원본 테이블 목록 가져오기 */
                MySqlDataReader sdr = GetReader(원본, "show tables");
                ArrayList 원본테이블 = new ArrayList();
                while (sdr.Read())
                {
                    for (int i = 0; i < sdr.FieldCount; i++)
                    {
                        //Console.WriteLine(sdr.GetValue(i));
                        원본테이블.Add(sdr.GetValue(i));
                    }
                }
                sdr.Close();

                foreach (string 테이블명 in 원본테이블)
                {
                    /* 2. 원본 테이블 데이터 가져오기 */
                    string sql = string.Format("select * from {0}", 테이블명);
                    sdr = GetReader(원본, sql);
                    int count = 0;
                    while (sdr.Read())
                    {
                        for (int i = 0; i < sdr.FieldCount; i++)
                        {
                            Console.Write("{0} : {1}\t", sdr.GetName(i), sdr.GetValue(i));
                        }
                        count++;
                        Console.WriteLine();
                    }
                    sdr.Close();

                    /* 3. 대상 테이블 확인하기 */
                    sql = string.Format("SELECT 1 FROM Information_schema.tables WHERE table_name = '{0}'", 테이블명);
                    sdr = GetReader(대상, sql);
                    bool 확인 = false;
                    while (sdr.Read())
                    {
                        확인 = true;
                    }
                    sdr.Close();
                    if (확인)
                    {
                        // 원본 테이블이 대상 데이터베이스에 존재 하기 때문에 초기화 필요
                        MessageBox.Show("테이블 존재  o");
                        sql = string.Format("TRUNCATE TABLE {0}", 테이블명);
                       
                    }
                    else
                    {
                        // 원본 테이블이 대상 데이터베이스에 존재 하지 않기 때문에 생성 필요
                        MessageBox.Show("테이블 존재 x");

                        sql = string.Format("SHOW CREATE TABLE `{0}`", 테이블명);
                        sdr = GetReader(원본, sql);
                        while (sdr.Read())
                        {
                            for (int c = 0; c < sdr.FieldCount; c++)
                            {
                                //Console.Write("{0} : {1}", sdr.GetName(c),sdr.GetValue(c));
                                sql = string.Format(sdr.GetValue(c).ToString());
                                SetData(대상,sql);
                            }
                            Console.WriteLine();
                        }
                        sdr.Close();

                    }
                    /* 4. 대상 테이블에 원본 데이터 삽입하기 */

                    /* 5. 이행 결과 리스트뷰에 표현하기 */
                    listView1.Items.Add(new ListViewItem(new string[] { listView1.Items.Count.ToString(), 테이블명, count.ToString() })); // Sample Test Data
                }
                return true;
            }
            return false;
        }

        private bool 개발()
        {
            try
            {
                string strConnection =
                    string.Format("server={0};user={1};password={2};database={3}", "192.168.3.136", "root", "1234", "test2");
                원본 = GetConnection(strConnection);
                if (원본 == null) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool 운영()
        {
            try
            {
                string strConnection =
                    string.Format("server={0};user={1};password={2};database={3}", "192.168.3.153", "root", "1234", "test");
                대상 = GetConnection(strConnection);
                if (대상 == null) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }
        //DB 접속
        private MySqlConnection GetConnection(string strConnection)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection();
                conn.ConnectionString = strConnection;
                conn.Open();
                return conn;
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
        //Select문
        private MySqlDataReader GetReader(MySqlConnection conn, string sql)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                return cmd.ExecuteReader();
            }
            catch
            {
                return null;
            }
        }
        //insert update
        private int SetData(MySqlConnection conn, string sql)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                return cmd.ExecuteNonQuery();
            }
            catch
            {
                return 0;
            }
        }       
    }
}