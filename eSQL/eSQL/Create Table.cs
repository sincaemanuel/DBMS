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
    public partial class Create_Table : Form
    {
        public Create_Table()
        {
            InitializeComponent();
        }

        private void Create_Table_Load(object sender, EventArgs e)
        {
            List<Database> list = DataLayer.Configuration.GetAllData();
            listBox1.Items.Clear();
            foreach (Database db in list)
            {
                if (db.DatabaseName != null)
                    listBox1.Items.Add(db.DatabaseName);
            }
            DataGridViewComboBoxColumn colCombo = new DataGridViewComboBoxColumn();
            colCombo.HeaderText = "Foreign Key";
            colCombo.Name = "foreignKey";
            colCombo.MaxDropDownItems = 8;
            List<string> items = DataLayer.Configuration.GetForeignKeys();
            foreach (string str in items)
            {
                colCombo.Items.Add(str);
            }
            dataGridView1.Columns.Add(colCombo);
            txtTBName.Validating += txtTBName_Validating;
            //this.dataGridView1.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dataGridView1_EditingControlShowing);
            //this.dataGridView1.CellValidating += new DataGridViewCellValidatingEventHandler(dataGridView1_CellValidating);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Index home = new Index();
            this.Dispose();
            home.Show();
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

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewColumn column1 = dataGridView1.Columns[1];
            //DataGridViewColumn column6 = dataGridView1.Columns[6];
            if (dataGridView1.CurrentCellAddress.X == column1.DisplayIndex)
            {

                ComboBox cb = e.Control as ComboBox;
                if (cb != null)
                {
                    cb.DropDownStyle = ComboBoxStyle.DropDown;
                }
            }
            //if (dataGridView1.CurrentCellAddress.X == column6.DisplayIndex)
            //{
            //    ComboBox cb = e.Control as ComboBox;
            //    if (cb != null)
            //    {
            //        cb.DropDownStyle = ComboBoxStyle.DropDown;
            //        List<string> list = Utils.DataAccess.GetForeignKeys();
            //        cb.DataSource = list;
            //    }
            //}
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewColumn column1 = dataGridView1.Columns[1];
            if (dataGridView1.CurrentCellAddress.X == column1.DisplayIndex)
            {
                DataGridViewComboBoxCell cb = dataGridView1.Rows[e.RowIndex].Cells[column1.Index] as DataGridViewComboBoxCell;
                if (cb != null)
                {
                    if (string.IsNullOrEmpty(e.FormattedValue.ToString()))
                    {
                        MessageBox.Show("You need to select a valid data type.");
                    }
                    if (!cb.Items.Contains(e.FormattedValue))
                    {
                        cb.Items.Insert(0, e.FormattedValue);
                    }
                }
                dataGridView1.CurrentCell.Value = e.FormattedValue;
                // cb.Value = e.FormattedValue;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count == 0)
            {
                MessageBox.Show("You need to select a database.");
                return;
            }
            if (string.IsNullOrEmpty(txtTBName.Text.Replace(" ", "")))
            {
                MessageBox.Show("You need to insert a table name");
                return;
            }
            if (dataGridView1.Rows.Count <= 1)
            {
                MessageBox.Show("You need to add a column at least.");
                return;
            }
            List<Database> list = DataLayer.Configuration.GetAllData();
            foreach (var item in listBox2.Items)
            {
                if (list.FirstOrDefault(x => x.DatabaseName == item.ToString()).Tables.Count(x => x.TableName == txtTBName.Text) > 0)
                {

                    MessageBox.Show("There already exists a table with this name!");
                    return;
                }
            }
            Table table = new Table();
            table.TableName = txtTBName.Text;
            List<Column> columns = new List<Column>();
            List<PrimaryKey> pklist = new List<PrimaryKey>();
            List<UniqueKey> ukList = new List<UniqueKey>();
            List<ForeignKey> fkList = new List<ForeignKey>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                Column column = new Column();
                if (row.Cells[0].Value != null)
                    if (!string.IsNullOrEmpty(row.Cells[0].Value.ToString()))
                        column.ColumnName = row.Cells[0].Value.ToString();
                if (row.Cells[1].Value != null)
                {
                    if (!string.IsNullOrEmpty(row.Cells[1].Value.ToString()))
                        column.Type = row.Cells[1].Value.ToString();
                }
                if (row.Cells[2].Value != null)
                {
                    column.IsNull = Utils.Utilities.GetBoolFromString(row.Cells[2].Value.ToString());
                }
                if (!string.IsNullOrEmpty(column.ColumnName) && !string.IsNullOrEmpty(column.Type))
                    columns.Add(column);
                if (row.Cells[3].Value != null)
                {
                    int parse = 0;
                    Int32.TryParse(row.Cells[3].Value.ToString(), out parse);
                    if (parse != 0)
                        column.Length = parse;
                }
                PrimaryKey pk = new PrimaryKey();
                if (row.Cells[4].Value != null && Utils.Utilities.GetBoolFromString(row.Cells[4].Value.ToString()))
                {
                    pk.Key = row.Cells[0].Value.ToString();
                    pklist.Add(pk);
                }
                UniqueKey uk = new UniqueKey();
                if (row.Cells[5].Value != null && Utils.Utilities.GetBoolFromString(row.Cells[5].Value.ToString()))
                {
                    uk.Key = row.Cells[0].Value.ToString();
                    ukList.Add(uk);
                }
                ForeignKey fKey = new ForeignKey();
                if (row.Cells[6].Value != null)
                {
                    fKey.Key = row.Cells[0].Value.ToString();
                    string[] key = Utils.Utilities.Split(row.Cells[6].Value.ToString());
                    fKey.RefTable = key[0];
                    fKey.RefAttribute = key[1];
                    fkList.Add(fKey);
                }
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                PrimaryKey pk = new PrimaryKey();
                if (row.Cells[3].Value != null)
                {
                    pk.Key = row.Cells[0].Value.ToString();
                }
            }
            table.PrimaryKey = pklist;
            table.UniqueKey = ukList;
            table.ForeignKey = fkList;
            table.Columns = columns;

            List<Database> dbs = new List<Database>();
            foreach (var item in listBox2.Items)
            {
                Database d = new Database();
                if (item != null)
                    d.DatabaseName = item.ToString();
                d.Tables = new List<Table>();
                d.Tables.Add(table);
                dbs.Add(d);
            }
            DataLayer.Configuration.AddTable(dbs);
            MessageBox.Show("Table " + table.TableName + " created successfully!");
        }

        private void txtTBName_Validating(object sender,
                System.ComponentModel.CancelEventArgs e)
        {
            if (txtTBName.Text.Contains(' '))
            {
                // Cancel the event and select the text to be corrected by the user.
                e.Cancel = true;
                MessageBox.Show("Empty space is not allowed in the Table Name!");
            }
        }

    }

}
