using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSQL.Model
{
    public class IndexFile
    {

        public string IndexName
        {
            get;
            set;
        }

        public int KeyLength
        {
            get;
            set;
        }

        public bool IsUnique
        {
            get;
            set;
        }

        public string IndexType
        {
            get;
            set;
        }


        public string IndexAttribute
        {
            get;
            set;
        }
    }
}
