using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSQL.Model
{
    public class SelectionOperator
    {
        public string TabCol { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }

        public string Type { get; set; }
    }
}
