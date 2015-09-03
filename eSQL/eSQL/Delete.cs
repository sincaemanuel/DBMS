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
using eSQL.DataLayer;

namespace eSQL
{
    public partial class Delete : Form
    {
        public Delete()
        {
            InitializeComponent();
        }

        private void Delete_Load(object sender, EventArgs e)
        {
            List<Database> list = DataLayer.Configuration.GetAllData();
            comboBox1.Items.Clear();
            foreach (Database db in list)
            {
                if (db.DatabaseName != null)
                    comboBox1.Items.Add(db.DatabaseName);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                panel2.Visible = true;
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
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null)
            {
                panel1.Visible = true;
                listBox1.Items.Clear();
                var list = DataLayer.DataAccess.GetAllTableKeys(comboBox1.SelectedItem.ToString(), comboBox2.SelectedItem.ToString());
                foreach (string kv in list)
                {
                    if (!string.IsNullOrEmpty(kv))
                        listBox1.Items.Add(kv);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Index home = new Index();
            this.Dispose();
            home.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                var tableref = DataLayer.DataAccess.CheckForReferencesFK(comboBox1.SelectedItem.ToString(), comboBox2.SelectedItem.ToString(), listBox1.SelectedItem.ToString());
                if (tableref == null)
                {
                    var tableName = comboBox2.SelectedItem.ToString();
                    DataLayer.DataAccess.Delete(comboBox1.SelectedItem.ToString(), tableName, listBox1.SelectedItem.ToString());
                    /////////////////////////////////////////

                    DataLayer.DataAccess.DeleteKeys(comboBox1.SelectedItem.ToString(), tableName, "uk", listBox1.SelectedItem.ToString());



                    DataLayer.DataAccess.DeleteKeys(comboBox1.SelectedItem.ToString(), tableName, "fk", listBox1.SelectedItem.ToString());
                    //delete reference, unique keys when deleted a primary key.
                    // sdaasdasd
                    MessageBox.Show("Deleted successfully!");
                    listBox1.Items.Clear();
                    var list = DataLayer.DataAccess.GetAllTableKeys(comboBox1.SelectedItem.ToString(), tableName);
                    foreach (string kv in list)
                    {
                        if (!string.IsNullOrEmpty(kv))
                            listBox1.Items.Add(kv);
                    }
                }
                else
                {
                    MessageBox.Show("You cannot delete this key because it has references to another table!");
                }
            }
        }

    }
}
