//using Kingdee.BOS;
//using Kingdee.BOS.App.Data;
//using Kingdee.BOS.Core.DynamicForm.PlugIn;
//using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
//using Kingdee.BOS.Orm.DataEntity;
//using BD.Standard.KY.ServicePlugInOneV23.DbHple;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using RestSharp;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BD.Standard.KY.ServicePlugInOneV23.Ywdj
//{
//    [Description("(废弃)委外采购退货单审核--推送MES")]
//    public class CGTH_WW_Send : AbstractOperationServicePlugIn
//    {
//        public override void EndOperationTransaction(EndOperationTransactionArgs e)
//        {
//            base.EndOperationTransaction(e);
//            foreach (DynamicObject entity in e.DataEntitys)
//            {
//                string FID = entity["Id"].ToString();
//                if (entity != null)
//                {

//                    JArray jar = new JArray();
//                    JArray jarzx = new JArray();
//                    DynamicObjectCollection config = Https.Http1(this.Context);
//                    string token = Dbhple.Hple.MesLogin(config);

//                    string sql = string.Format(@"/*dialect*/ exec yzx_cgtl_ww '{0}'", FID);
//                    DynamicObjectCollection data = DBUtils.ExecuteDynamicObject(this.Context, sql);
//                    foreach (DynamicObject item in data)
//                    {
//                        string sqlzx = string.Format(@"/*dialect*/ exec yzx_cgtl_wwEntry '{0}'", FID);
//                        DynamicObjectCollection datazx = DBUtils.ExecuteDynamicObject(this.Context, sqlzx);
//                        foreach (DynamicObject itemzx in datazx)
//                        {


//                            JObject items = new JObject()
//                                {
//                                    new JProperty("outerId",itemzx["outerId"].ToString()),
//                                    new JProperty("purOrderItemOutId",itemzx["purOrderItemOutId"].ToString()),
//                                    new JProperty("productOutId",itemzx["productOutId"].ToString()),
//                                    //new JProperty("purOrderOutId",itemzx["purOrderOutId"].ToString()),
//                                    new JProperty("storeUnit",itemzx["storeUnit"].ToString()),
//                                    new JProperty("storeUnitCode",itemzx["storeUnitCode"].ToString()),
//                                    new JProperty("purchaseAmount",itemzx["purchaseAmount"].ToString()),
//                                    new JProperty("isOperate",itemzx["isOperate"].ToString()),
//                                    new JProperty("isCancel",itemzx["isCancel"].ToString()),
//                                };
//                            jarzx.Add(items);
//                        }



//                        JObject date = new JObject()
//                            {
//                                    new JProperty("deptCode",item["deptCode"].ToString()),
//                                    new JProperty("refundCode",item["purOrderCode"].ToString()),
//                                    new JProperty("supplierCode",item["supplierCode"].ToString()),
//                                    new JProperty("purOrderOutId",item["purOrderOutId"].ToString()),
//                                    new JProperty("outerId",item["outerId"].ToString()),
//                                    new JProperty("purType",item["purType"].ToString()),
//                                    new JProperty("items",jarzx),
//                            };

//                        jar.Add(date);

//                        //待提供
                        
//                        var client = new RestClient(config[0]["http"].ToString() + "/api/v1/mps/openApi/purorder/return");
//                        client.Timeout = -1;
//                        var request = new RestRequest(Method.POST);
//                        request.AddHeader("AccessToken", token);
//                        request.AddHeader("Content-Type", "application/json");
//                        var body = jar.ToString();
//                        request.AddParameter("application/json", body, ParameterType.RequestBody);

//                        IRestResponse response = client.Execute(request);
//                        //Console.WriteLine(response.Content);
//                        var responseMsg = response.Content;//获取返回Response消息
//                                                           //接收返回值 Cookie
//                        var resultBackSave = JsonConvert.DeserializeObject<Root>(responseMsg);

//                        Dbhple.Log.writestr("委外采购退货单：" + item["purOrderCode"].ToString(), body.ToString(), responseMsg.ToString(), "_委外采购退货单");
//                        if (resultBackSave.code.ToString() != "200")
//                        {
//                            throw new KDBusinessException("", resultBackSave.msg.ToString());

//                        }
//                    }

//                }
//            }
//        }
//        public class Root
//        {
//            /// <summary>
//            /// 
//            /// </summary>
//            public int code { get; set; }
//            /// <summary>
//            /// 同步成功
//            /// </summary>
//            /// <summary>
//            /// 操作成功
//            /// </summary>
//            public string msg { get; set; }
//            /// <summary>
//            /// 
//            /// </summary>
//            public string success { get; set; }
//            /// <summary>
//            /// 
//            /// </summary>
//            public string time { get; set; }
//        }
//    }
//}
