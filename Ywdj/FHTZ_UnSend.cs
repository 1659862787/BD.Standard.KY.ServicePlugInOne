using Kingdee.BOS;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
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
using System.Text;
using System.Threading.Tasks;

namespace BD.Standard.KY.ServicePlugInOneV23.Ywdj
{
    [Description("发货通知单反审核--推送MES")]
    public class FHTZ_UnSend : AbstractOperationServicePlugIn
    {
        public override void EndOperationTransaction(EndOperationTransactionArgs e)
        {
            base.EndOperationTransaction(e);
            foreach (DynamicObject entity in e.DataEntitys)
            {
                if (!((DynamicObject)entity["SaleOrgId"])["Number"].ToString().Equals("200"))
                {
                    return;
                }

                string FID = entity["Id"].ToString();
                string fbillno = entity["BILLNO"].ToString();
                JObject jar = new JObject()
                    {
                            new JProperty("outerId",FID),        
                    };
                DynamicObjectCollection config = Https.Http1(this.Context);
                string token = Dbhple.Hple.MesLogin(config);

                var client = new RestClient(config[0]["http"].ToString() + "/api/v1/wms/openApi/revocation/send/notice/syn");
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

                Dbhple.Log.writestr("发货通知反审核：" + fbillno, body.ToString(), responseMsg.ToString(), "_发货通知");

                if (resultBackSave.code.ToString() != "200")
                {
                    throw new KDBusinessException("", resultBackSave.msg.ToString());

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
