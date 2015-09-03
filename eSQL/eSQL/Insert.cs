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
    public partial class Insert : Form
    {
        public Insert()
        {
            InitializeComponent();
        }

        private void Insert_Load(object sender, EventArgs e)
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
                button1.Visible = true;
                int height1 = 142;
                int height2 = 140;
                string tableName = comboBox2.SelectedItem.ToString();
                List<Column> columns = DataLayer.Configuration.GetTableColumns(comboBox1.SelectedItem.ToString(), tableName);
                //columns = Utils.Utilities.AddPrimaryKeysToTop(columns, tableName);
                if (columns.Count > 0)
                {
                    TextBox[] textBoxes = new TextBox[columns.Count];
                    Label[] labels = new Label[columns.Count];
                    for (int i = 0; i < columns.Count; i++)
                    {
                        labels[i] = new Label();
                        labels[i].Name = columns[i].ColumnName;
                        labels[i].Text = columns[i].ColumnName;
                        labels[i].Location = new Point(34, height1 + 27);
                        height1 = height1 + 27;
                        labels[i].Visible = true;
                        textBoxes[i] = new TextBox();
                        if (columns[i].Length != 0)
                        {
                            textBoxes[i].MaxLength = columns[i].Length;
                        }
                        textBoxes[i].Name = columns[i].ColumnName;
                        textBoxes[i].Location = new Point(140, height2 + 26);
                        height2 = height2 + 26;
                        textBoxes[i].Visible = true;
                        if (!columns[i].IsNull)
                            textBoxes[i].Validating += Textbox_Validating;
                        this.Controls.Add(labels[i]);
                        this.Controls.Add(textBoxes[i]);

                    }
                }
            }
        }

        private void Textbox_Validating(object sender, CancelEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox != null && textbox.Text.Replace(" ", "") == "")
            {
                // Cancel the event and select the text to be corrected by the user.
                e.Cancel = true;
                MessageBox.Show("Empty space is not allowed in the " + textbox.Name + " TextBox !");
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
            KeyValue keyValue = new KeyValue();
            KeyValue kvuk = new KeyValue();
            KeyValue kvfknew = new KeyValue();
            string tableName = comboBox2.SelectedItem.ToString();
            string databaseName = comboBox1.SelectedItem.ToString();
            foreach (Control control in this.Controls)
            {
                if (control.GetType().Name == "TextBox")
                {

                    List<PrimaryKey> pkeys = DataLayer.Configuration.GetPrimaryKey(tableName);
                    if (pkeys.Count(x => x.Key == control.Name) > 0)
                    {
                        bool exists = DataLayer.DataAccess.CheckRestriction(databaseName, tableName, control.Text);
                        if (!exists)
                        {
                            MessageBox.Show("The primary key " + control.Name + " already exists!");
                            return;
                        }
                    }
                    List<UniqueKey> ukeys = DataLayer.Configuration.GetUniqueKeys(tableName);
                    if (ukeys.Count(x => x.Key == control.Name) > 0)
                    {
                        bool exists = DataLayer.DataAccess.CheckRestriction(databaseName, tableName, "uk", control.Text);
                        if (exists)
                        {
                            MessageBox.Show("The unique key " + control.Name + " already exists!");
                            return;
                        }
                    }
                    List<ForeignKey> fkeys = DataLayer.Configuration.GetForeignKeys(tableName);
                    if (fkeys != null && fkeys.Count(x => x.Key == control.Name) > 0)
                    {
                        ForeignKey fkey = fkeys.FirstOrDefault(x => x.Key == control.Name);
                        bool exists = DataLayer.DataAccess.CheckRestriction(databaseName, fkey, control.Text);
                        if (!exists)
                        {
                            MessageBox.Show("The foreign key " + control.Name + " : " + control.Text + " does not exists!");
                            return;
                        }
                    }

                    if (pkeys.Count(x => x.Key == control.Name) > 0)
                    {
                        keyValue.key += control.Text;
                        keyValue.key += "#";
                    }
                    else
                    {
                        keyValue.value += control.Text;
                        keyValue.value += "#";
                    }
                    if (ukeys.Count(x => x.Key == control.Name) > 0)
                    {
                        kvuk.key += control.Text;
                        kvuk.key += "#";
                    }
                    else if (pkeys.Count(x => x.Key == control.Name) > 0 && ukeys.Count > 0)
                    {
                        kvuk.value += control.Text;
                        kvuk.value += "#";
                    }
                    if (fkeys != null && fkeys.Count(x => x.Key == control.Name) > 0 && fkeys.Count > 0)
                    {
                        kvfknew.key += control.Text;
                    }
                    else
                        if (pkeys.Count(x => x.Key == control.Name) > 0)
                        {
                            kvfknew.value = control.Text;
                        }
                }
            }
            if (!string.IsNullOrEmpty(keyValue.key))
            {
                keyValue.key = keyValue.key.Remove(keyValue.key.Length - 1, 1);
            }
            if (keyValue.value != null && keyValue.value.Length > 0)
            {
                keyValue.value = keyValue.value.Remove(keyValue.value.Length - 1, 1);
            }
            if (!string.IsNullOrEmpty(kvuk.key))
            {
                kvuk.key = kvuk.key.Remove(kvuk.key.Length - 1, 1);
            }
            if (kvuk.value != null && kvuk.value.Length > 0)
            {
                kvuk.value = kvuk.value.Remove(kvuk.value.Length - 1, 1);
            }

            DataLayer.DataAccess.InsertInTable(comboBox1.SelectedItem.ToString(), tableName, keyValue);
            if (kvuk.key != null)
                DataLayer.DataAccess.InsertInTable(comboBox1.SelectedItem.ToString(), tableName + "uk", kvuk);
            if (kvfknew.key != null)
                DataLayer.DataAccess.UpdateExistingFK(databaseName, tableName, kvfknew);
            // DataLayer.DataAccess.InsertInTable(comboBox1.SelectedItem.ToString(), tableName + "fk", kvfk);
            MessageBox.Show("Value added successfully !");
            Insert insert = new Insert();
            this.Dispose();
            insert.Show();
        }
    }
}
