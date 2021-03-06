using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Classes.ApiClasses
{
    class TablesStatistics
    {
        public long branchId { get; set; }
        public string branchName { get; set; }
        public int openedCount { get; set; }   // opened tables
        public int emptyCount { get; set; }   // empty and reserved tables
        public int reservedCount { get; set; } // count for reserved but not opened tables
    }
}
