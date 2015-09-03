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
    public partial class Create_Index : Form
    {
        public Create_Index()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Index home = new Index();
            this.Dispose();
            home.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel1.Visible = true;
            var list = DataLayer.Configuration.GetDatabases();
            Database db = list.FirstOrDefault(x => x.DatabaseName == comboBox1.SelectedItem.ToString());
            if (db != null)
            {
                comboBox2.Items.Clear();
                foreach (var t in db.Tables)
                {
                    comboBox2.Items.Add(t.TableName);
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem.ToString() != "")
            {
                button1.Visible = true;
            }
        }

        private void Create_Index_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            List<Database> dbs = DataLayer.Configuration.GetDatabases();
            foreach (var db in dbs)
                comboBox1.Items.Add(db.DatabaseName);
        }
    }
}
