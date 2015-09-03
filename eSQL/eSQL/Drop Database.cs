using eSQL.Model;
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
    public partial class Drop_Database : Form
    {
        public Drop_Database()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() != "")
            {
                DataLayer.Configuration.DropDatabase(comboBox1.SelectedItem.ToString());
                Drop_Database drop = new Drop_Database();
                this.Dispose();
                drop.Show();
            }
        }

        private void Drop_Database_Load(object sender, EventArgs e)
        {
            List<Database> list = DataLayer.Configuration.GetAllData();
            foreach (Database db in list)
            {
                if (db.DatabaseName != null)
                    comboBox1.Items.Add(db.DatabaseName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Index home = new Index();
            this.Dispose();
            home.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Visible = true;
        }
    }
}
