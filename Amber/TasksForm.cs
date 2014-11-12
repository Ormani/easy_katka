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
        private static BindingList<Task> TasksBindingList = new BindingList<Task>();
        private static readonly List<CallInfo> _callList = new List<CallInfo>();
        private static readonly object _sync = new object();

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
            LoadTasks();
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

        private static void LoadTasks()
        {
            try
            {
                using (var file = File.OpenRead("tasks.bin"))
                {
                    foreach (var task in Serializer.DeserializeItems<Task>(file, PrefixStyle.Base128, 0))
                    {
                        TasksBindingList.Add(task);
                    }
                }
            }

            //catch (FileNotFoundException) { }

            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

        }

        private static void SaveTasks()
        {
            using (var file = File.Create("tasks.bin"))
            {
                foreach (var task in TasksBindingList)
                {
                    Serializer.SerializeWithLengthPrefix(file, task, PrefixStyle.Base128);
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            SaveTasks();
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
            lock (_sync)
            {
                TasksBindingList.Add(new Task(textBox1.Text, richTextBox1.TextLength, "Waiting", "NULL"));
                _callList.Add(new CallInfo(textBox1.Text, richTextBox1.Text, comboBox1.SelectedItem as VoiceInfo, "null"));
            }
            
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
        }

    }
}
