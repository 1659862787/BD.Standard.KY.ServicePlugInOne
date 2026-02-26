using BD.Standard.KY.ServicePlugInOne.DbHple;
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
    [Description("生产订单变更单审核--推送MES")]
    public class SCDDBG : AbstractOperationServicePlugIn
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

                    string sql = string.Format(@"/*dialect*/ exec yzx_scddbgd '{0}'", FID);
                    DynamicObjectCollection data = DBUtils.ExecuteDynamicObject(this.Context, sql);
                    List<DynamicObjectCollection> list = new List<DynamicObjectCollection>();
                    list.Add(data);
                    string jar = JsonUtils.ObjectJsons(list);


                    var client = new RestClient(config[0]["http"].ToString() + "/api/v1/mes/openApi/shopTask/syn");
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
                    Dbhple.Log.writestr("生产订单变单更：" + data[0]["shopTaskNo"].ToString(), body.ToString(), responseMsg.ToString(), "_生产订单变单更");

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
