using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Security.AccessControl;

namespace UDPChat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        UdpClient udpClient;
        Thread t;
        String MyName;
        ArrayList ips = new ArrayList();//上線人員的IP列表
        const short port = 2019;//UDP通訊埠
        string BC=IPAddress.Broadcast.ToString();//廣播位址


        private string MyIP()
        {
            string myIP=Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostEntry(myIP).AddressList;
            foreach(IPAddress ip in ipaddress)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "";
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.Text=" "+MyIP();
        }
        private void Send(string ToIP,string msg,string ToWhom)
        {
            //我是誰+IP+訊息+發給誰
            string a = MyName+":"+MyIP()+":"+msg+":"+ToWhom;
            byte[] b = Encoding.Default.GetBytes(a);
            UdpClient udpClient = new UdpClient();
            udpClient.Send(b, b.Length);
        }
        
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                t.Abort();
                udpClient.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Onlinebutton_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("請輸入暱稱");
                return;
            }
            MyName=textBox1.Text;
            listBox1.Items.Clear();//清除上線人員列表
            ips.Clear();//清除IP列表
            if (textBox1.Text == "上線")//目前離線
            {
                t = new Thread(Listen);
                t.Start();
                Send(BC, "上線","");
                Onlinebutton.Text = "離線";
            }
            else
            {
                Send(BC,"離線","");
                t.Abort();
                udpClient.Close();
                Onlinebutton.Text = "上線";
            }

        }
        private void Listen()
        {
            UdpClient UC = new UdpClient(port);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(MyIP()), port);
            while (true)
            {
                byte[] B = UC.Receive(ref iPEndPoint);
                string A = Encoding.Default.GetString(B);
                //切割訊息C[0]=發訊者 c[1]=IP c[2]=訊息 c[3]=發訊對象
                string[] C = A.Split(':');
                switch (C[2])
                {
                    case "上線":                      //有人上線
                        listBox1.Items.Add(C[0]);     //把名稱加入左側列表
                        ips.Add(C[1]);                //把IP加入arrayList
                        if (C[0] != MyName) { Send(C[1],"我也在線上",C[0]); }
                        break;
                    case "我也在線上":
                        listBox1.Items.Add(C[0]);
                        ips.Add(C[1]);
                        break;
                    case "離線":
                        listBox1.Items.Remove(C[0]);
                        ips.Remove(C[1]);
                        break;
                    default:
                        if (C[3] == "")               //公開訊息
                        {
                            listBox2.Items.Add("廣播:"+C[2]);
                        }
                        else//有指定收件者
                        {
                            listBox2.Items.Add(C[0] + "to" + C[3] + ":" + C[2]);
                        }
                        break;
                }
            }
        }
        private void PostButton_Click(object sender, EventArgs e)
        {
            listBox1.ClearSelected();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (listBox1.SelectedIndex < 0)
                {
                    Send(BC,textBox2.Text,"");
                }
                else
                {
                    Send(ips[listBox1.SelectedIndex].ToString(),textBox2.Text,listBox1.SelectedItem.ToString());
                    listBox2.Items.Add(MyName+"to"+listBox1.SelectedItem.ToString()+":"+textBox2.Text);
                }
                textBox2.Clear();
            }
        }
    }
}
