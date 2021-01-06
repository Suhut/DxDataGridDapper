using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DxDataGridDapper.DevExtreme
{
    public interface IDevExtremeHelper
    {
        public string GetFilterSqlExprByArray(IList expression);
        public string GetSortSqlExprByArray(IList expression);
    }
}
