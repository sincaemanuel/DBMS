using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSQL.Model
{
    public class Database
    {

        public string DatabaseName
        {
            get;
            set;
        }


        public List<Table> Tables
        {
            get;
            set;
        }
    }
}
