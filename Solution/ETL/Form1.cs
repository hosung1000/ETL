using MySql.Data.MySqlClient;
using System;
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
        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Click += Btn_Evnet;
            button2.Click += Btn_Evnet;
        }

        private void Btn_Evnet(object o, EventArgs e)
        {
            Button btn = (Button)o;
            switch (btn.Name)
            {
                case "button1":
                    개발();
                    break;
                case "button2":
                    운영();
                    break;
                default:
                    break;
            }
        }
        private bool 개발()
        {
            try
            {
                //MessageBox.Show("개발");
                string StrConnection =
                    string.Format("server={0}", "user={1}", "password={2}", "database={3}", " 192.168.3.136", "root", "1234", "test2"); //test2의 테이블 이행

                MySqlConnection conn = GetConnection(StrConnection);
                if (conn == null)
                {
                    MessageBox.Show("접속오류");
                }

                MessageBox.Show("접속성공");
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
                //MessageBox.Show("운영");
                string StrConnection =
                    string.Format("server={0}", "user={1}", "password={2}", "database={3}", " 192.168.3.153", "root", "1234", "test");

                MySqlConnection conn = GetConnection(StrConnection);
                if (conn == null)
                {
                    MessageBox.Show("접속오류");
                }

                MessageBox.Show("접속성공");
                return true;


            }
            catch
            {
                return false;
            }
        }
        //connection 하기
        private MySqlConnection GetConnection(string StrConnection)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection();
                conn.ConnectionString = StrConnection;
                return conn;
            }
            catch
            {
                return null;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
