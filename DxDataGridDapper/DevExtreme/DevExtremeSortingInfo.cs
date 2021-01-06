﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DxDataGridDapper.DevExtreme
{
    public class DevExtremeSortingInfo
    {
        /// <summary>
        /// The data field to be used for sorting.
        /// </summary>
        public string Selector { get; set; }

        /// <summary>
        /// A flag indicating whether data should be sorted in a descending order.
        /// </summary>
        public bool Desc { get; set; }
    }
}
