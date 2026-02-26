using Kingdee.BOS.Orm.DataEntity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BD.Standard.KY.ServicePlugInOne.DbHple
{
    public class JsonUtils
    {
        public static string ObjectJsons(List<DynamicObjectCollection> lists)
        {
            JArray dataLists = new JArray();
            JArray dataList = new JArray();
            JObject data = null;

            if (lists[0].Count == 0) return "";
            int i = lists.Count()-1;

            for (; i >= 0; i--)
            {
                foreach (DynamicObject dy in lists[i])
                {
                    data = new JObject();
                    foreach (var item in dy.DynamicObjectType.Properties)
                    {
                        if (item.Name.Equals("FBILLTYPEID") || item.Name.Equals("FBILLNO")) continue;
                        data.Add(new JProperty(item.Name, dy[item.Name].ToString()));
                    }
                    if (i == 0 && lists.Count()>1)
                    {
                        data["items"] = dataLists;
                    }
                    else if(i == 1)
                    {
                        dataLists.Add(data);
                    }
                }
            }
            dataList.Add(data);
            
            return dataList != null? dataList.ToString():"";
        }
        public static string ObjectJson(List<DynamicObjectCollection> lists)
        {
            JArray dataLists = new JArray();
            JArray dataList = new JArray();
            if (lists[0].Count == 0) return "";
            JObject data = null;
            int i = lists.Count() - 1;
            for (; i >= 0; i--)
            {
                foreach (DynamicObject dy in lists[i])
                {
                    data = new JObject();
                    foreach (var item in dy.DynamicObjectType.Properties)
                    {
                        data.Add(new JProperty(item.Name, dy[item.Name].ToString()));
                    }
                    if (i == 0 && lists.Count() > 1)
                    {
                        data["items"] = dataLists;
                    }
                    else if (i == 1)
                    {
                        dataLists.Add(data);
                    }
                }
            }
            return data != null ? data.ToString() : "";
        }

        public static string ObjectJsonst(List<DynamicObjectCollection> lists)
        {
            JArray dataLists = new JArray();
            JArray dataList = new JArray();
            JObject data = null;
            if (lists[0].Count == 0) return "";
            int i = lists.Count() - 1;
            for (; i >= 0; i--)
            {
                foreach (DynamicObject dy in lists[i])
                {
                    data = new JObject();
                    foreach (var item in dy.DynamicObjectType.Properties)
                    {
                        data.Add(new JProperty(item.Name, dy[item.Name].ToString()));
                    }
                    if (i == 0 && lists.Count() > 1)
                    {
                        data["inOutApplyItemList"] = dataLists;
                    }
                    else if (i == 1)
                    {
                        dataLists.Add(data);
                    }
                }
            }
            return data != null ? data.ToString() : "";
        }

    }
}
