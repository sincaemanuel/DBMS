using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSQL.Model
{
    public class Table
    {

        public string TableName
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }

        public int RowLength
        {
            get;
            set;
        }

        internal List<Column> Columns
        {
            get;
            set;
        }


        internal List<PrimaryKey> PrimaryKey
        {
            get;
            set;
        }


        internal List<UniqueKey> UniqueKey
        {
            get;
            set;
        }


        internal List<IndexFile> IndexFile
        {
            get;
            set;
        }

        public List<ForeignKey> ForeignKey
        {
            get;
            set;
        }

    }
}
