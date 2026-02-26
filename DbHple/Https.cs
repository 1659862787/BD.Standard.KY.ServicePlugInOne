using Kingdee.BOS;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Orm.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Standard.KY.ServicePlugInOneV23.DbHple
{
    public class Https
    {
        public static DynamicObjectCollection Http1(Context context)
        {
            string sql = string.Format(@"/*dialect*/ exec yzx_config");
            DynamicObjectCollection data = DBUtils.ExecuteDynamicObject(context, sql);
            return data;      
        }
    }
}
