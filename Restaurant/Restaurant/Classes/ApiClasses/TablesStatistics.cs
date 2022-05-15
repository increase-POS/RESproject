using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Classes.ApiClasses
{
    class TablesStatistics
    {
        public int branchId { get; set; }
        public string branchName { get; set; }
        public int openedCount { get; set; }
        public int emptyCount { get; set; }
    }
}
