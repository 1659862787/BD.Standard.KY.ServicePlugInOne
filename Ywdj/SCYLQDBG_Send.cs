using BD.Standard.KY.ServicePlugInOne.DbHple;
using Kingdee.BOS;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Core.Metadata.FieldElement;
using Kingdee.BOS.Orm.DataEntity;
using BD.Standard.KY.ServicePlugInOneV23.DbHple;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;


namespace BD.Standard.KY.ServicePlugInOneV23.Ywdj
{
    [Description("生产用料清单变更单审核--推送MES")]
    public class SCYLQDBG_Send : AbstractOperationServicePlugIn
    {
        public override void OnPreparePropertys(Kingdee.BOS.Core.DynamicForm.PlugIn.Args.PreparePropertysEventArgs e)
        {
            base.OnPreparePropertys(e);
            List<Field> file = this.BusinessInfo.GetFieldList();
            foreach (Field item in file)
            {
                e.FieldKeys.Add(item.Key);
            }
        }

        public override void EndOperationTransaction(EndOperationTransactionArgs e)
        {
            base.EndOperationTransaction(e);
            foreach (DynamicObject entity in e.DataEntitys)
            {
                string FID = entity["Id"].ToString();
                string FBILLNO = entity["BILLNO"].ToString();
                bool CHECKBOX = Convert.ToBoolean(entity["F_QCPR_CHECKBOX"].ToString());

                if (entity != null && !CHECKBOX)
                {
                    DynamicObjectCollection config = Https.Http1(this.Context);
                    string token = Dbhple.Hple.MesLogin(config);

                    string sql = string.Format(@"/*dialect*/ exec yzx_scylqdbgd '{0}'", FID);
                    DynamicObjectCollection data = DBUtils.ExecuteDynamicObject(this.Context, sql);
                    
                    string sqlzx = string.Format(@"/*dialect*/ exec yzx_scylqdbgdEntry '{0}'", FID);
                    DynamicObjectCollection datazx = DBUtils.ExecuteDynamicObject(this.Context, sqlzx);

                    List<DynamicObjectCollection> list = new List<DynamicObjectCollection>();
                    list.Add(data);
                    list.Add(datazx);
                    string jar = JsonUtils.ObjectJsons(list);

                    var client = new RestClient(config[0]["http"].ToString() + "/api/v1/mes/openApi/taskRaw/syn");
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
                    Dbhple.Log.writestr("生产用料清单变更单：" + FBILLNO, body.ToString(), responseMsg.ToString(), "_生产用料清单变更单");
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
