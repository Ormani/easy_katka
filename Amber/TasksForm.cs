using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Ozeki.Media.MediaHandlers;
using Ozeki.Media.MediaHandlers.Speech;
using Ozeki.VoIP;

namespace Amber
{
    public partial class TasksForm : Form
    {
        private readonly AccountsForm _accountsForm = AccountsForm.Instance;
        private Thread _tasksThread;
        private static bool _isWorking;
        private static readonly BindingList<Task> TasksBindingList = new BindingList<Task>();
        private static readonly ThreadSafeList<CallInfo> CallList = new ThreadSafeList<CallInfo>();
        private static readonly AutoResetEvent AutoResetEvent = new AutoResetEvent(false);
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
            _tasksThread = new Thread(Start);
            _isWorking = true;
            _tasksThread.Start();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _isWorking = false;
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
            MessageBox.Show(AccountsForm.softphone.GetAvaliablePhoneLineCount().ToString());
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                foreach (var callInfo in CallList)
                {
                    
                    if ((callInfo.State != "null") && (callInfo.State != "Completed"))
                        continue;
                    if ((DateTime.Now.Hour < callInfo.StartTime) || (DateTime.Now.Hour >= callInfo.EndTime))
                        continue;

                    var phoneLine = AccountsForm.softphone.GetAvailablePhoneLine();
                    if (null != phoneLine)
                    {
                        //callInfo.SetState("Initializing");
                        StartCallHandler(callInfo, phoneLine);
                    }
                    else
                    {
                        AutoResetEvent.WaitOne();
                        phoneLine = AccountsForm.softphone.GetAvailablePhoneLine();
                        //callInfo.SetState("Initializing");
                        StartCallHandler(callInfo, phoneLine);
                    }
                }
            });
        }

        private static void Continue()
        {
            if (!_isWorking)
                return;

            new Thread(() =>
            {
                foreach (var callInfo in CallList)
                {
                    if ((callInfo.State != "null") && (callInfo.State != "Completed") && (callInfo.State != "Cancelled"))
                        continue;
                    if ((DateTime.Now.Hour < callInfo.StartTime) || (DateTime.Now.Hour >= callInfo.EndTime))
                        continue;
                    var phoneLine = AccountsForm.softphone.GetAvailablePhoneLine();
                    if (null != phoneLine)
                    {
                        //callInfo.SetState("Initializing");
                        StartCallHandler(callInfo, phoneLine);
                    }
                }
            }).Start();

            
        }

        private static void StartCallHandler(CallInfo callInfo, IPhoneLine phoneLine)
        {
            var callHandler = new CallHandler(callInfo, AccountsForm.softphone);
            callHandler.CallStateChanged += callHandler_CallStateChanged;
            callHandler.Start(phoneLine);
        }

        static void callHandler_CallStateChanged(object sender, EventArgs e)
        {
            var callState = (CallStateChangedArgs) e;
            var phoneCall = (IPhoneCall) sender;

            lock (TasksBindingList)
                foreach (var task in TasksBindingList.Where(task => phoneCall.DialInfo.DialedString == task.Number))
                {
                    task.State = phoneCall.CallState.ToString();
                    task.Login = phoneCall.PhoneLine.SIPAccount.UserName;
                    break;
                }
            foreach (var callInfo in CallList.Cast<CallInfo>().Where(call => call.PhoneNumber == phoneCall.DialInfo.DialedString))
            {
                callInfo.SetState(phoneCall.CallState.ToString());
                if (!callState.State.IsCallEnded()) return;
                callInfo.SetState("Completed");
                AutoResetEvent.Set();
                Continue();
                return;
            }
            
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

        private void dataGridView1_CellMouseClick(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Right != e.Button) return;
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                foreach (var call in CallList.Cast<CallInfo>().Where(call => (string) row.Cells[0].Value == call.PhoneNumber))
                {
                    CallList.TryRemove(call);
                    dataGridView1.Rows.Remove(row);
                    return;
                }
            }
        }
    }
}
