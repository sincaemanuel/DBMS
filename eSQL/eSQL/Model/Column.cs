using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSQL.Model
{
    public class Column
    {


        public string ColumnName
        {
            get;
            set;
        }
        public string Type
        {
            get;
            set;
        }

        public int Length
        {
            get;
            set;
        }

        public bool IsNull
        {
            get;
            set;
        }
    }
}
