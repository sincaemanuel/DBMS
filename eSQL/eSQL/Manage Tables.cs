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
    public partial class Manage_Tables : Form
    {
        public Manage_Tables()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel1.Visible = true;
            var list = DataLayer.Configuration.GetDatabases();
            Database db = list.Where(x => x.DatabaseName == comboBox1.SelectedItem.ToString()).FirstOrDefault();
            if (db != null)
            {
                comboBox2.Items.Clear();
                foreach (var t in db.Tables)
                {
                    comboBox2.Items.Add(t.TableName);
                }
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem.ToString() != "")
            {
                button1.Visible = true;
            }
        }

        private void Manage_Tables_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            List<Database> dbs = DataLayer.Configuration.GetDatabases();
            foreach (var db in dbs)
                comboBox1.Items.Add(db.DatabaseName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem.ToString() != "")
            {
                panel2.Visible = true;
                panel3.Visible = true;
                List<Column> columns = DataLayer.Configuration.GetTableColumns(comboBox2.SelectedItem.ToString());
                if (columns != null)
                {
                    listBox1.Items.Clear();
                    listBox2.Items.Clear();
                    listBox3.Items.Clear();
                    listBox4.Items.Clear();
                    foreach (string x in columns.Select(x => x.ColumnName))
                        listBox1.Items.Add(x);
                    foreach (string x in columns.Select(x => x.ColumnName))
                        listBox3.Items.Add(x);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Index index = new Index();
            this.Dispose();
            index.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null && !Utils.Utilities.CheckForDuplicate(listBox2, listBox1.SelectedItem))
            {
                listBox2.Items.Add(listBox1.SelectedItem);
                listBox1.Items.Remove(listBox1.SelectedItem);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null && !Utils.Utilities.CheckForDuplicate(listBox1, listBox2.SelectedItem))
            {
                listBox1.Items.Add(listBox2.SelectedItem);
                listBox2.Items.Remove(listBox2.SelectedItem);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox3.SelectedItem != null && !Utils.Utilities.CheckForDuplicate(listBox4, listBox3.SelectedItem))
            {
                listBox4.Items.Add(listBox3.SelectedItem);
                listBox3.Items.Remove(listBox3.SelectedItem);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedItem != null && !Utils.Utilities.CheckForDuplicate(listBox3, listBox3.SelectedItem))
            {
                listBox3.Items.Add(listBox4.SelectedItem);
                listBox4.Items.Remove(listBox4.SelectedItem);
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
