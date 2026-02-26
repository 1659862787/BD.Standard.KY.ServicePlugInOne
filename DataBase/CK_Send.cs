using BD.Standard.KY.ServicePlugInOne.DbHple;
using Kingdee.BOS;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Orm.DataEntity;
using BD.Standard.KY.ServicePlugInOneV23.DbHple;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using Kingdee.BOS.Util;
using BD.Standard.KY.ServicePlugInOneV23.Dbhple;
using System.Text;

namespace BD.Standard.KY.ServicePlugInOneV23.DataBase
{
    [HotUpdate]
    [Description("仓库审核--推送MES")]
    public class CK_Send : AbstractOperationServicePlugIn
    {
        public override void EndOperationTransaction(EndOperationTransactionArgs e)
        {
            base.EndOperationTransaction(e);
            foreach (DynamicObject entity in e.DataEntitys)
            {
                string FID = entity["Id"].ToString();
                if (entity != null)
                {
                    DynamicObjectCollection config = Https.Http1(this.Context);
                    string token = Dbhple.Hple.MesLogin(config);

                    //获取区域仓位值集
                    string sql = string.Format(@"/*dialect*/ exec yzx_ck '{0}'", FID);
                    DynamicObjectCollection datas = DBUtils.ExecuteDynamicObject(this.Context, sql);
                    JObject data = null;
                    foreach (DynamicObject item in datas)
                    {
                         data= new JObject()
                         {
                                    new JProperty("wareHorseTypeName",item["wareHorseTypeName"].ToString()),
                                    new JProperty("wareHorseName",item["wareHorseName"].ToString()),
                                    new JProperty("wareHorseCode",item["wareHorseCode"].ToString()),
                                    new JProperty("reservoirAreaName",item["reservoirAreaName"].ToString()),
                                    new JProperty("reservoirAreaCode",item["reservoirAreaCode"].ToString()),
                                    new JProperty("status",item["status"].ToString()),
                                    new JProperty("deptCode",item["deptCode"].ToString()),
                                    new JProperty("outerid",item["FSTOCKID"].ToString()),
                                    new JProperty("serial",item["serial"].ToString()),
                         };

                        //动态拼接sql生成对应区域的仓位值集
                        sql=GetStockflex(FID, item["FENTRYID"].ToString(), item["FFLEXID"].ToString());
                        Log.writestr("仓库-仓位：" + datas[0]["wareHorseName"].ToString(), sql,"" , "_仓库-仓位");

                        DynamicObjectCollection datazx = DBUtils.ExecuteDynamicObject(this.Context, sql);
                        JArray jArray = new JArray();
                        int count = 1;
                        JArray jArraydata = null;
                        foreach (DynamicObject itemzx in datazx)
                        {
                            JArray Jar = new JArray();
                            JObject jo = null;
                            string fnumbers = "";
                            //解析仓位值集，生成仓位串
                            foreach (var it in itemzx.DynamicObjectType.Properties)
                            {
                                fnumbers += itemzx[it.Name].ToString() + ".";
                                jo = new JObject()
                                {
                                    {"dimKey",it.Name.ToString()},
                                    {"locationKey", itemzx[it.Name].ToString()},
                                };
                                Jar.Add(jo);
                            }
                            fnumbers=fnumbers.Remove(fnumbers.Length-1,1);
                            //200条数据同步一次
                            if (count % 201==0 && datazx.Count!= count)
                            {
                                jArraydata = new JArray();
                                data["locationList"] =jArray;
                                jArraydata.Add(data);
                                string json1 = jArraydata.ToString();
                                Requset(config, token, datas, jArraydata.ToString());
                                jArray = new JArray();
                            }
                            JObject items = new JObject()
                            {
                                new JProperty("locationSerial",count++),
                                new JProperty("locationName",fnumbers),
                                new JProperty("erpDimensionalityJson",Jar.ToString()),
                                new JProperty("locationCode",fnumbers)
                            };
                            jArray.Add(items);
                            
                        }
                        jArraydata=new JArray();
                        data["locationList"] = jArray;
                        jArraydata.Add(data);
                        string json = jArraydata.ToString();
                        Requset(config, token, datas, jArraydata.ToString());
                    }

                }
            }
        }

        private static void Requset(DynamicObjectCollection config, string token, DynamicObjectCollection data, string jar)
        {
            var client = new RestClient(config[0]["http"].ToString() + "/api/v1/wms/openApi/horse/syn");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("AccessToken", token);
            request.AddHeader("Content-Type", "application/json");
            var body = jar.ToString();
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            var responseMsg = response.Content;//获取返回Response消息 //接收返回值 Cookie
            var resultBackSave = JsonConvert.DeserializeObject<Root>(responseMsg);

            Dbhple.Log.writestr("仓库-仓位：" + data[0]["wareHorseName"].ToString(), body.ToString(), responseMsg.ToString(), "_仓库-仓位");
            if (resultBackSave.code.ToString() != "200")
            {
                throw new KDBusinessException("", resultBackSave.msg.ToString());

            }
        }

        public class Root
        {
            /// <summary>
            /// 
            /// </summary>
            public int code { get; set; }
            /// <summary>
            /// 同步成功
            /// </summary>
            /// <summary>
            /// 操作成功
            /// </summary>
            public string msg { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string success { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string time { get; set; }
        }



        public string GetStockflex(string fstockid,string fflexentryid,string FFLEXID)
        {
            DynamicObjectCollection datazx = DBUtils.ExecuteDynamicObject(this.Context, "select sf.FFLEXID from T_BD_STOCK s join T_BD_STOCKFLEXITEM sf on  sf.fstockid=s.fstockid where s.fstockid='" + fstockid+"'");

            StringBuilder fields = new StringBuilder();
            fields.Append("select f.fnumber as FF"+FFLEXID+ ""); 
            StringBuilder sb = new StringBuilder();
            sb.Append(" from T_BD_STOCK s  \r\n");
            sb.Append(" join T_BD_STOCKFLEXITEM sf on  sf.FSTOCKID=s.FSTOCKID  \r\n");
            sb.Append(" left JOIN T_BD_STOCKFLEXDETAIL sfd ON sf.FENTRYID = sfd.FENTRYID  \r\n");
            sb.Append(" left JOIN T_BAS_FLEXVALUESENTRY f ON sfd.FFLEXENTRYID = f.FENTRYID  \r\n");
            if (datazx.Count > 1)
            {
                StringBuilder sb1 = null;
                string table = "A";
                int i = 1;
                foreach (var item in datazx)
                {
                    if (item["FFLEXID"].ToString()!=FFLEXID)
                    {
                        sb1 = new StringBuilder();
                        sb1.Append(" left join ( ");
                        sb1.Append(" select  f.fnumber,s.fstockid \r\n");
                        sb1.Append(" from T_BD_STOCK s \r\n");
                        sb1.Append(" join T_BD_STOCKFLEXITEM sf on  sf.FSTOCKID=s.FSTOCKID ");
                        sb1.Append(" left JOIN T_BD_STOCKFLEXDETAIL sfd ON sf.FENTRYID = sfd.FENTRYID \r\n");
                        sb1.Append(" left JOIN T_BAS_FLEXVALUESENTRY f ON sfd.FFLEXENTRYID = f.FENTRYID \r\n");
                        sb1.Append(" where s.fstockid='"+ fstockid + "' and sf.fflexid='"+ item["FFLEXID"].ToString() + "') ");
                        sb1.Append(table+i+" on "+ table + i+".fstockid=s.fstockid \r\n");
                      
                        fields.Append(","+ table + i+".fnumber as FF" + item["FFLEXID"].ToString() + "");
                        i++;
                        sb.Append(sb1.ToString());
                    }
                }
            }
            sb.Append(" where s.FSTOCKID='"+ fstockid + "' and f.fentryid='"+ fflexentryid + "' ");
            string sql= fields.Append(sb.ToString()).ToString();
            return sql;
        }

    }
}
