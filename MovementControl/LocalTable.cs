using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovementControl
{
    class TableList
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public int timestamp { get; set; }
    }
}
