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
            string a = MyName+" "+MyIP()+":"+msg+" "+ToWhom;
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
    }
}
