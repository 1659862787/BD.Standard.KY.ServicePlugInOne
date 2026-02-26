using Kingdee.BOS;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Orm.DataEntity;
using BD.Standard.KY.ServicePlugInOneV23.DbHple;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using RestSharp;

namespace BD.Standard.KY.ServicePlugInOneV23.Ywdj
{
    [Description("采购订单关闭与反关闭--推送MES")]
    public class CGDD_GBSend : AbstractOperationServicePlugIn
    {
        public override void EndOperationTransaction(EndOperationTransactionArgs e)
        {
            base.EndOperationTransaction(e);
            string operatype = this.FormOperation.Operation;
            foreach (DynamicObject entity in e.DataEntitys)
            {
                if (((DynamicObject)entity["PurchaseOrgId"])["Number"].Equals("200"))
                {
                    string FID = entity["Id"].ToString();
                    string fbillno = entity["BILLNO"].ToString();
                    int opera = operatype.Equals("BillClose") ? 0 : 1;

                    JObject jar = new JObject()
                {
                        new JProperty("purOrderOutId",FID),
                        new JProperty("updateType",opera),
                };
                    DynamicObjectCollection config = Https.Http1(this.Context);
                    string token = Dbhple.Hple.MesLogin(config);

                    var client = new RestClient(config[0]["http"].ToString() + "/api/v1/mps/openApi/pur/order/arrival/update/state/syn");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("AccessToken", token);
                    request.AddHeader("Content-Type", "application/json");
                    var body = jar.ToString();
                    request.AddParameter("application/json", body, ParameterType.RequestBody);

                    IRestResponse response = client.Execute(request);
                    //Console.WriteLine(response.Content);
                    var responseMsg = response.Content;//获取返回Response消息
                                                       //接收返回值 Cookie
                    var resultBackSave = JsonConvert.DeserializeObject<Root>(responseMsg);

                    Dbhple.Log.writestr("采购订单关闭与反关闭：" + fbillno, body.ToString(), responseMsg.ToString(), "_采购订单");

                    if (resultBackSave.code.ToString() != "200")
                    {
                        throw new KDBusinessException("", resultBackSave.msg.ToString());

                    }
                }
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
    }
}
