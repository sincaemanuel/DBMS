using eSQL.DataLayer;
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
    public partial class Select : Form
    {
        public Select()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Index index = new Index();
            this.Dispose();
            index.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel1.Visible = true;
            var list = DataLayer.Configuration.GetDatabases();
            Database db = list.FirstOrDefault(x => x.DatabaseName == comboBox1.SelectedItem.ToString());
            if (db != null)
            {
                listBox3.Items.Clear();
                foreach (var t in db.Tables)
                {
                    listBox3.Items.Add(t.TableName);
                }
                dataGridView1.Visible = false;
                button4.Enabled = true;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (comboBox2.SelectedItem.ToString() != "")
            //{
            //    panel2.Visible = true;
            //    listBox1.Items.Clear();
            //    listBox2.Items.Clear();
            //    var columns = DataLayer.Configuration.GetTableColumns(comboBox1.SelectedItem.ToString(), comboBox2.SelectedItem.ToString());
            //    button4.Enabled = true;
            //    dataGridView1.Visible = false;
            //    DataGridViewComboBoxColumn colCombo = new DataGridViewComboBoxColumn();
            //    colCombo.HeaderText = "Column";
            //    colCombo.Name = "Column";
            //    colCombo.MaxDropDownItems = 8;
            //    List<Column> items = DataLayer.Configuration.GetTableColumns(comboBox1.SelectedItem.ToString(), comboBox2.SelectedItem.ToString());
            //    foreach (Column str in items)
            //    {
            //        colCombo.Items.Add(str.ColumnName);
            //    }
            //    dataGridView2.Visible = true;
            //    dataGridView2.Columns.Insert(0, colCombo);
            //    foreach (var column in columns)
            //    {
            //        listBox1.Items.Add(column.ColumnName);
            //    }
            //}
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

        private void Select_Load(object sender, EventArgs e)
        {
            var list = Configuration.GetAllData();
            foreach (Database db in list)
            {
                if (db.DatabaseName != null)
                    comboBox1.Items.Add(db.DatabaseName);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count > 0)
            {

                dataGridView1.Visible = true;
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
                List<string> selectedlist = new List<string>();
                List<SelectionOperator> operators = new List<SelectionOperator>();
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    SelectionOperator op = new SelectionOperator();
                    if (dataGridView2.Rows[i].Cells[0].Value != null)
                    {
                        op.TabCol = dataGridView2.Rows[i].Cells[0].Value.ToString();
                        op.Type = DataLayer.Configuration.GetTableColumns(comboBox1.SelectedItem.ToString(), op.TabCol.Split('.')[0]).Where(x => x.ColumnName == op.TabCol.Split('.')[1]).FirstOrDefault().Type;
                    }
                    if (dataGridView2.Rows[i].Cells[1].Value != null)
                        op.Operator = dataGridView2.Rows[i].Cells[1].Value.ToString();
                    if (dataGridView2.Rows[i].Cells[2].Value != null)
                        op.Value = dataGridView2.Rows[i].Cells[2].Value.ToString();


                    if (op.TabCol != null && op.Operator != null && op.Value != null)
                        operators.Add(op);
                }


                foreach (var item in listBox2.Items)
                {
                    selectedlist.Add(item.ToString());
                }
                List<string> selectedtables = new List<string>();
                foreach (string str in selectedlist)
                {
                    string[] s = str.Split('.');
                    selectedtables.Add(s[0]);
                }
                selectedtables = selectedtables.Distinct().ToList();
                List<KeyValue> colIndex = Utils.Utilities.GetColumnIndex(selectedlist, comboBox1.SelectedItem.ToString());
                if (colIndex.Count > 0)
                {
                    List<string> result = null;
                    if (selectedtables.Count > 1)
                        result = DataLayer.DataAccess.GetKeyValuesSelectedColumns(comboBox1.SelectedItem.ToString(), colIndex, false, operators);
                    else
                        result = DataLayer.DataAccess.GetKeyValuesSelectedColumns(comboBox1.SelectedItem.ToString(), colIndex, true, operators);

                    var aux = result;
                    result = new List<string>();
                    foreach (var x in aux)
                    {
                        if (!x.Contains("NULL"))
                        {
                            result.Add(x);
                        }
                    }
                    for (int i = 0; i < selectedlist.Count; i++)
                    {
                        DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                        column.HeaderText = selectedlist[i];
                        column.Name = selectedlist[i];
                        dataGridView1.Columns.Add(column);
                    }
                    for (int i = 0; i < result.Count; i++)
                    {
                        dataGridView1.Rows.Add();
                        for (int j = 0; j < selectedlist.Count; j++)
                        {
                            dataGridView1.Rows[i].Cells[j].Value = GetColumnValue(result[i], j);
                        }
                    }
                }
            }
        }

        public static string GetColumnValue(string text, int index)
        {
            var str = text.Split('#');
            if (str[index] != null)
                return str[index];
            return "";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox3.SelectedItem != null && !Utils.Utilities.CheckForDuplicate(listBox4, listBox3.SelectedItem))
            {

                var tablerf = DataLayer.DataAccess.GetReferencedTable(comboBox1.SelectedItem.ToString(), listBox3.SelectedItem.ToString());

                if (listBox4.Items.Count > 0 && (tablerf == null && DataLayer.DataAccess.CheckIfTableisReferenced(comboBox1.SelectedItem.ToString(), listBox3.SelectedItem.ToString()) == null))
                {
                    MessageBox.Show("This table does not have references to any of the selected tables !");
                    return;
                }
                listBox4.Items.Add(listBox3.SelectedItem);
                listBox3.Items.Remove(listBox3.SelectedItem);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedItem != null && !Utils.Utilities.CheckForDuplicate(listBox3, listBox4.SelectedItem))
            {

                listBox3.Items.Add(listBox4.SelectedItem);
                listBox4.Items.Remove(listBox4.SelectedItem);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (listBox4.Items.Count > 0)
            {
                button2.Visible = true;
                //button8.Visible = true;
                button9.Visible = true;
                foreach (var item in listBox4.Items)
                {
                    if (listBox4.Items.Count > 1 && (DataLayer.DataAccess.GetReferencedTable(comboBox1.SelectedItem.ToString(), item.ToString()) == null && DataLayer.DataAccess.CheckIfTableisReferenced(comboBox1.SelectedItem.ToString(), item.ToString()) == null))
                    {
                        MessageBox.Show("The selection of tables contains tables without references. You must select a single table without references or multiple table with references between them !");
                        return;
                    }
                }
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                DataGridViewComboBoxColumn colCombo = new DataGridViewComboBoxColumn();
                colCombo.HeaderText = "Column";
                colCombo.Name = "Column";
                colCombo.MaxDropDownItems = 8;
                foreach (var item in listBox4.Items)
                {
                    var columns = DataLayer.Configuration.GetTableColumns(comboBox1.SelectedItem.ToString(), item.ToString());
                    button4.Enabled = true;
                    dataGridView1.Visible = false;
                    foreach (Column str in columns)
                    {
                        colCombo.Items.Add(item.ToString() + "." + str.ColumnName);
                    }
                    dataGridView2.Visible = true;

                    foreach (var column in columns)
                    {
                        listBox1.Items.Add(item.ToString() + "." + column.ColumnName);
                    }

                }
                dataGridView2.Columns.Clear();
                dataGridView2.Columns.Insert(0, colCombo);
                DataGridViewComboBoxColumn colOper = new DataGridViewComboBoxColumn();
                colOper.HeaderText = "Operator";
                colOper.Name = "Operator";
                colOper.MaxDropDownItems = 6;
                var list = DataLayer.Configuration.GetOperators();
                foreach (string s in list)
                {
                    colOper.Items.Add(s);
                }
                dataGridView2.Columns.Insert(1, colOper);
                DataGridViewTextBoxColumn colText = new DataGridViewTextBoxColumn();
                colText.HeaderText = "Value";
                colText.Name = "Value";
                dataGridView2.Columns.Insert(2, colText);
                if (listBox1.Items.Count > 0)
                    panel2.Visible = true;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count > 0)
            {

                dataGridView1.Visible = true;
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
                List<string> selectedlist = new List<string>();
                List<SelectionOperator> operators = new List<SelectionOperator>();
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    SelectionOperator op = new SelectionOperator();
                    if (dataGridView2.Rows[i].Cells[0].Value != null)
                    {
                        op.TabCol = dataGridView2.Rows[i].Cells[0].Value.ToString();
                        op.Type = DataLayer.Configuration.GetTableColumns(comboBox1.SelectedItem.ToString(), op.TabCol.Split('.')[0]).Where(x => x.ColumnName == op.TabCol.Split('.')[1]).FirstOrDefault().Type;
                    }
                    if (dataGridView2.Rows[i].Cells[1].Value != null)
                        op.Operator = dataGridView2.Rows[i].Cells[1].Value.ToString();
                    if (dataGridView2.Rows[i].Cells[2].Value != null)
                        op.Value = dataGridView2.Rows[i].Cells[2].Value.ToString();


                    if (op.TabCol != null && op.Operator != null && op.Value != null)
                        operators.Add(op);
                }


                foreach (var item in listBox2.Items)
                {
                    selectedlist.Add(item.ToString());
                }
                List<string> selectedtables = new List<string>();
                foreach (string str in selectedlist)
                {
                    string[] s = str.Split('.');
                    selectedtables.Add(s[0]);
                }
                selectedtables = selectedtables.Distinct().ToList();
                List<KeyValue> colIndex = Utils.Utilities.GetColumnIndex(selectedlist, comboBox1.SelectedItem.ToString());
                if (colIndex.Count > 0)
                {
                    List<string> result = null;
                    if (selectedtables.Count > 1)
                        result = DataLayer.DataAccess.GetKeyValuesSelectedColumns(comboBox1.SelectedItem.ToString(), colIndex, false, operators);
                    else
                        return;
                    for (int i = 0; i < selectedlist.Count; i++)
                    {
                        DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                        column.HeaderText = selectedlist[i];
                        column.Name = selectedlist[i];
                        dataGridView1.Columns.Add(column);
                    }
                    for (int i = 0; i < result.Count; i++)
                    {
                        dataGridView1.Rows.Add();
                        for (int j = 0; j < selectedlist.Count; j++)
                        {
                            dataGridView1.Rows[i].Cells[j].Value = GetColumnValue(result[i], j);
                        }
                    }
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count > 0)
            {

                dataGridView1.Visible = true;
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
                List<string> selectedlist = new List<string>();
                List<SelectionOperator> operators = new List<SelectionOperator>();
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    SelectionOperator op = new SelectionOperator();
                    if (dataGridView2.Rows[i].Cells[0].Value != null)
                    {
                        op.TabCol = dataGridView2.Rows[i].Cells[0].Value.ToString();
                        op.Type = DataLayer.Configuration.GetTableColumns(comboBox1.SelectedItem.ToString(), op.TabCol.Split('.')[0]).Where(x => x.ColumnName == op.TabCol.Split('.')[1]).FirstOrDefault().Type;
                    }
                    if (dataGridView2.Rows[i].Cells[1].Value != null)
                        op.Operator = dataGridView2.Rows[i].Cells[1].Value.ToString();
                    if (dataGridView2.Rows[i].Cells[2].Value != null)
                        op.Value = dataGridView2.Rows[i].Cells[2].Value.ToString();


                    if (op.TabCol != null && op.Operator != null && op.Value != null)
                        operators.Add(op);
                }


                foreach (var item in listBox2.Items)
                {
                    selectedlist.Add(item.ToString());
                }
                List<string> selectedtables = new List<string>();
                foreach (string str in selectedlist)
                {
                    string[] s = str.Split('.');
                    selectedtables.Add(s[0]);
                }
                selectedtables = selectedtables.Distinct().ToList();
                List<KeyValue> colIndex = Utils.Utilities.GetColumnIndex(selectedlist, comboBox1.SelectedItem.ToString());
                if (colIndex.Count > 0)
                {
                    List<string> result = null;
                    if (selectedtables.Count > 1)
                        result = DataLayer.DataAccess.GetKeyValuesIndexJoin(comboBox1.SelectedItem.ToString(), colIndex, false, operators);
                    else
                        return;
                    for (int i = 0; i < selectedlist.Count; i++)
                    {
                        DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                        column.HeaderText = selectedlist[i];
                        column.Name = selectedlist[i];
                        dataGridView1.Columns.Add(column);
                    }
                    for (int i = 0; i < result.Count; i++)
                    {
                        dataGridView1.Rows.Add();
                        for (int j = 0; j < selectedlist.Count; j++)
                        {
                            dataGridView1.Rows[i].Cells[j].Value = GetColumnValue(result[i], j);
                        }
                    }
                }
            }
        }
    }
}
