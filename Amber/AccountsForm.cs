using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Ozeki.VoIP;
using ProtoBuf;

namespace Amber
{
    public partial class AccountsForm : Form
    {
        private static AccountsForm _instance;
        private static readonly Softphone Softphone = new Softphone();
        private static readonly BindingList<Account> AccountsBindingList = new BindingList<Account>();

        public static AccountsForm Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                    _instance = new AccountsForm();
                return _instance;
            }

        }

        private static void LoadTasks()
        {
            try
            {
                using (var file = File.OpenRead("accounts.bin"))
                    foreach (var account in Serializer.DeserializeItems<Account>(file, PrefixStyle.Base128, 0))
                        Softphone.Register(true, account.Login, account.Login, account.Login, account.Password, account.Server, 5060);
            }

            catch (FileNotFoundException) { }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private static void SaveTasks()
        {
            using (var file = File.Create("accounts.bin"))
                 foreach (var accountBindingList in AccountsBindingList)
                     Serializer.SerializeWithLengthPrefix(file, accountBindingList, PrefixStyle.Base128);
        }

        public static void Exit()
        {
            Softphone.UnregAllPhoneLines();
            SaveTasks();
        }

        private AccountsForm() 
        {
            InitializeComponent();
            dataGridView1.DataSource = AccountsBindingList;
            Softphone.PhoneLineStateChanged += Softphone_PhoneLineStateChanged;
            LoadTasks();
        }

        static void Softphone_PhoneLineStateChanged(object sender, RegistrationStateChangedArgs e)
        {
            var sipAccount = sender as SIPAccount;
            if (e.State == RegState.RegistrationSucceeded)
            {
                lock (AccountsBindingList)
                {
                    if (sipAccount != null)
                        AccountsBindingList.Add(new Account
                            (sipAccount.UserName, sipAccount.RegisterPassword, sipAccount.DomainServerHost, e.State.ToString()));
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {

            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
            base.OnFormClosing(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Register();
        }

        void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.Enter == e.KeyCode)
                Register();
        }

        void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.Enter == e.KeyCode)
                Register();
        }

        private void Register()
        {
            Softphone.Register(true, textBox1.Text, textBox1.Text, textBox1.Text, textBox2.Text, textBox3.Text, 5060);
            textBox1.Clear();
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (MouseButtons.Right != e.Button) return;
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                Softphone.UnregPhoneLine(row.Cells[0].Value.ToString());
                dataGridView1.Rows.Remove(row);
            }
        }
    }
}
