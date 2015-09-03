using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSQL.Model
{
    public class ForeignKey
    {

        public string Key
        {
            get;
            set;
        }

        public string RefTable
        {
            get;
            set;
        }
        private string refAttribute { get; set; }

        public string RefAttribute
        {
            get;
            set;
        }
    }
}
