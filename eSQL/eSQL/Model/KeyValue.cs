using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSQL.Model
{
    public class KeyValue
    {
        public ObjectId id { get; set; }
        public string key { get; set; }
        public string value { get; set; }
    }
}
