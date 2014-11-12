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
    public partial class Accounts : Form
    {
        private static Accounts instance;

        private Accounts() {
            InitializeComponent();
        }

        public static Accounts Instance()
        {
            
            
                if (instance == null)
                {
                    instance = new Accounts();
                    
                }
                return instance;
            
        }
        //public Accounts()
        //{
            //InitializeComponent();
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
