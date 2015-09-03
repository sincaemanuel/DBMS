using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eSQL
{
    public partial class Create_Database : Form
    {
        public Create_Database()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDbName.Text.Replace(" ", "")))
            {
                DataLayer.Configuration.CreateDatabase(txtDbName.Text);
                Create_Database crt = new Create_Database();
                this.Dispose();
                crt.Show();

            }
            else
            {
                MessageBox.Show("Fill in the database name.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Index home = new Index();
            this.Dispose();
            home.Show();
        }

        private void Create_Database_Load(object sender, EventArgs e)
        {
            txtDbName.Validating += txtDbName_Validating;
        }

        private void txtDbName_Validating(object sender,
                System.ComponentModel.CancelEventArgs e)
        {
            if (txtDbName.Text.Contains(' '))
            {
                // Cancel the event and select the text to be corrected by the user.
                e.Cancel = true;
                MessageBox.Show("Empty space is not allowed in the Database Name!");
            }
        }
    }
}
