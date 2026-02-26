using Kingdee.BOS;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Core.Bill.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Orm.DataEntity;
using BD.Standard.KY.ServicePlugInOneV23.DbHple;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace BD.Standard.KY.ServicePlugInOneV23.Tsyw
{
    [Description("查询MES库存数据")]
    public class kc_select : AbstractBillPlugIn
    {
        //重写按钮点击事件
        public override void BarItemClick(Kingdee.BOS.Core.DynamicForm.PlugIn.Args.BarItemClickEventArgs e)
        {
            base.BarItemClick(e);

            //条件判断，当点击YDIE_ tbGetSetValue这个按钮时候触发
            if (e.BarItemKey == "YDIE_tbGetSetValue")
            {
                DynamicObjectCollection config = Https.Http1(this.Context);
                string token = Dbhple.Hple.MesLogin(config);
                Dbhple.Log.writestr("库存查询token：" + token,"","", "_库存查询");
                string sql = string.Format(@"/*dialect*/ exec yzx_kc_select");
                DynamicObjectCollection data = DBUtils.ExecuteDynamicObject(this.Context, sql);
                foreach (DynamicObject item in data)
                {
                    JObject items = new JObject()
                    {
                        new JProperty("outWareHorseId",item["FSTOCKID"].ToString()),
                    };
                    //待提供
                    
                    var client = new RestClient(config[0]["http"].ToString() + "/api/v1/wms/standingbook/total/qty/byOutWareHorseId");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("AccessToken", token);
                    request.AddHeader("Content-Type", "application/json");
                    var body = items.ToString();
                    request.AddParameter("application/json", body, ParameterType.RequestBody);

                    IRestResponse response = client.Execute(request);
                    //Console.WriteLine(response.Content);
                    var responseMsg = response.Content;//获取返回Response消息
                                                       //接收返回值 Cookie
                   
                    var resultBackSave = JsonConvert.DeserializeObject<Root>(responseMsg.ToString().Replace("0E-10", "0.00"));

                    Dbhple.Log.writestr("库存查询：" + item["orderCode"].ToString(), body.ToString(), responseMsg.ToString(), "_库存查询");

                    if (resultBackSave.code.ToString() == "200")
                    {
                        foreach (var itemS in resultBackSave.data)
                        {
                            string sql_insert = string.Format(@"/*dialect*/ exec yzx_kc_insert '{0}','{1}','{2}'", item["FSTOCKID"].ToString(),itemS.productCode, itemS.totalPaperQty);
                            DynamicObjectCollection data_insert = DBUtils.ExecuteDynamicObject(this.Context, sql_insert);
                        }

                    }
                    else
                    {
                        throw new KDBusinessException("", resultBackSave.msg.ToString());
                    }
                }
            }
        }


        public class DataItem
        {
            /// <summary>
            /// 
            /// </summary>
            public string productCode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public decimal totalPaperQty { get; set; }
        }

        public class Root
        {
            /// <summary>
            /// 
            /// </summary>
            public int code { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<DataItem> data { get; set; }
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

    }
}
