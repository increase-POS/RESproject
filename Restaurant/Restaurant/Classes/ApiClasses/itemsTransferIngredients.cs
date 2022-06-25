using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Classes.ApiClasses
{
    class itemsTransferIngredients
    {
        public long itemsTransIngredId { get; set; }
        public Nullable<int> itemsTransId { get; set; }
        public Nullable<int> dishIngredId { get; set; }
        public byte isActive { get; set; }
        public string notes { get; set; }
    }
}
