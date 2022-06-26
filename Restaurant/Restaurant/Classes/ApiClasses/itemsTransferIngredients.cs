﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Classes.ApiClasses
{
    class itemsTransferIngredients
    {
        public long itemsTransIngredId { get; set; }
        public Nullable<long> itemsTransId { get; set; }
        public Nullable<long> dishIngredId { get; set; }
        public string DishIngredientName { get; set; }
        public byte isActive { get; set; }
        public string notes { get; set; }
    }
}
