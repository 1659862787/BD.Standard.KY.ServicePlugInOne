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
using BD.Standard.KY.ServicePlugInOne.DbHple;

namespace BD.Standard.KY.ServicePlugInOneV23.DataBase
{
    [Description("物料审核后全量同步--推送MES---废弃，延用历史方案")]
    public class WL_Sends : AbstractOperationServicePlugIn
    {
        public override void EndOperationTransaction(EndOperationTransactionArgs e)
        {
            base.EndOperationTransaction(e);
            string fnumber=string.Empty;
            try
            {

                DynamicObjectCollection config = Https.Http1(this.Context);
                string token = Dbhple.Hple.MesLogin(config);

                string sqlzb = string.Format(@"/*dialect*/ exec yzx_wls");
                DynamicObjectCollection datazb = DBUtils.ExecuteDynamicObject(this.Context, sqlzb);
                List<DynamicObjectCollection> list = new List<DynamicObjectCollection>();
                DynamicObjectCollection data = null;

                Dbhple.Log.writestr("物料：" + datazb.Count, "", "", "_物料");
                foreach (DynamicObject item in datazb)
                {
                    fnumber = item["productCode"].ToString();
                    data = null;
                    data.Add(item);
                    Dbhple.Log.writestr("物料：" + datazb.Count, "", "", "_物料");
                    list.Add(data);
                    string jar = JsonUtils.ObjectJsons(list);
                    Dbhple.Log.writestr("物料：" + jar, "", "", "_物料");

                    var client = new RestClient(config[0]["http"].ToString() + "/api/v1/mds/openApi/product/syn");
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

                    if (resultBackSave.code.ToString() != "200")
                    {
                        Dbhple.Log.writestr("物料：" + fnumber, body.ToString(), responseMsg.ToString(), "_物料err");
                    }
                    else
                    {
                        Dbhple.Log.writestr("物料：" + fnumber, body.ToString(), responseMsg.ToString(), "_物料");
                    }
                }
            }
            catch (Exception ex)
            {
                Dbhple.Log.writestr("物料：" + fnumber,"" , ex.Message, "_物料err");
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
