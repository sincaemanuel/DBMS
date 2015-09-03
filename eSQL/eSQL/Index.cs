using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using eSQL.Model;

namespace eSQL
{
    public partial class Index : Form
    {
        public Index()
        {
            InitializeComponent();
        }

        private void btnCrtDb_Click(object sender, EventArgs e)
        {
            Create_Database Create_Database = new Create_Database();
            this.Hide();
            Create_Database.Show();


        }

        private void btnDrpDb_Click(object sender, EventArgs e)
        {
            Drop_Database drop_database = new Drop_Database();
            this.Hide();
            drop_database.Show();
        }

        private void Index_Load(object sender, EventArgs e)
        {
            //var connectionString = "mongodb://localhost";
            //var client = new MongoClient(connectionString);
            //var database = client.GetDatabase("mydb");
            //var collection = database.GetCollection<Entity>("testData");
            //var entity = collection.Find(x => x.Name != "");
            Create_Database crt = new Create_Database();
            crt.Close();
            Drop_Database drp = new Drop_Database();
            drp.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Create_Table crttablee = new Create_Table();
            this.Hide();
            crttablee.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Drop_Table drpTable = new Drop_Table();
            this.Hide();
            drpTable.Show();
        }

        private void btnLdIndex_Click(object sender, EventArgs e)
        {
            Create_Index create_index = new Create_Index();
            this.Hide();
            create_index.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Insert insert = new Insert();
            this.Hide();
            insert.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Delete delete = new Delete();
            this.Hide();
            delete.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Create_Index index = new Create_Index();
            this.Hide();
            index.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Select select = new Select();
            this.Hide();
            select.Show();
        }

        //private void button3_Click(object sender, EventArgs e)
        //{
        //    Manage_Tables mngTables = new Manage_Tables();
        //    this.Hide();
        //    mngTables.Show();
        //}
    }
}
