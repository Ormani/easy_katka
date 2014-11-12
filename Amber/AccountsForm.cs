using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Amber
{
    public partial class AccountsForm : Form
    {
        private static AccountsForm _instance;

        public static AccountsForm Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                    _instance = new AccountsForm();
                return _instance;
            }

        }

        private AccountsForm() 
        {
            InitializeComponent();
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
            
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
