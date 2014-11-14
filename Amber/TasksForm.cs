using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
using Ozeki.Media.MediaHandlers;
using Ozeki.Media.MediaHandlers.Speech;
using ProtoBuf;

namespace Amber
{
    public partial class TasksForm : Form
    {
        private readonly AccountsForm _accountsForm = AccountsForm.Instance;
        private readonly Thread _tasksThread = new Thread(Start);
        private static readonly BindingList<Task> TasksBindingList = new BindingList<Task>();
        private static readonly ThreadSafeList<CallInfo> CallList = new ThreadSafeList<CallInfo>();
        //private static readonly object _sync = new object();

        public TasksForm()
        {
            InitializeComponent();
            ClientSize = new Size(457, 500);
            dataGridView1.DataSource = TasksBindingList;
            comboBox1.DataSource = new TextToSpeech().GetAvailableVoices();
            comboBox1.DisplayMember = "name";
            
        }

        private void accountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _accountsForm.Show();   
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _tasksThread.Start();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _tasksThread.Abort();
        }

        protected override void OnLoad(EventArgs e)
        {
            notifyIcon1.Icon = SystemIcons.Shield;
            
            notifyIcon1.Visible = false;
            Resize += TasksForm_Resize;
            base.OnLoad(e);
        }

        void TasksForm_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized) return;
            _accountsForm.Hide();
            Hide();
            notifyIcon1.Visible = true;
        }

        private static void Start()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    
                }
            }); 
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            AccountsForm.Exit();
            base.OnClosing(e);
        }

        private bool CheckNumber()
        {
            const string pattern = @"(^\+\d{1,2})?((\(\d{3}\))|(\-?\d{3}\-)|(\d{3}))((\d{3}\-\d{4})|
                                    (\d{3}\-\d\d\20.-\d\d)|(\d{7})|(\d{3}\-\d\-\d{3}))";
            lock (textBox1)
            {
                return Regex.IsMatch(textBox1.Text, pattern);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!CheckNumber())
            {
                MessageBox.Show(@"The entered number isn't valid");
                return;
            }

            lock (TasksBindingList)
                TasksBindingList.Add(new Task(textBox1.Text, richTextBox1.TextLength, "Waiting", "NULL", 
                    string.Join("-", comboBox2.Text, comboBox3.Text)));
           
            CallList.Add(new CallInfo(textBox1.Text, richTextBox1.Text, comboBox1.SelectedItem as VoiceInfo, "null",
                    Int32.Parse(comboBox2.Text), Int32.Parse(comboBox3.Text)));
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
        }
    }
}
