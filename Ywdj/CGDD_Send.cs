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
//    [Description("(废弃)采购订单审核--推送MES")]
//    public class CGDD_Send: AbstractOperationServicePlugIn
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

//                    string sql = string.Format(@"/*dialect*/ exec yzx_cgdd '{0}'", FID);
//                    DynamicObjectCollection data = DBUtils.ExecuteDynamicObject(this.Context, sql);
//                    foreach (DynamicObject item in data)
//                    {
//                        string sqlzx = string.Format(@"/*dialect*/ exec yzx_cgddEntry '{0}'", FID);
//                        DynamicObjectCollection datazx = DBUtils.ExecuteDynamicObject(this.Context, sqlzx);
//                        foreach (DynamicObject itemzx in datazx)
//                        {

//                            JObject items = new JObject()
//                                {
//                                    new JProperty("purOrderItemOutId",itemzx["purOrderItemOutId"].ToString()),
//                                    new JProperty("productCostType",itemzx["productCostType"].ToString()),
//                                    new JProperty("productOutId",itemzx["productOutId"].ToString()),
//                                    new JProperty("productCode",itemzx["productCode"].ToString()),
//                                    new JProperty("purchaseAmount",itemzx["purchaseAmount"].ToString()),
//                                    new JProperty("storeUnitCode",itemzx["storeUnitCode"].ToString()),
//                                    new JProperty("limitOperate",itemzx["limitOperate"].ToString()),
//                                    new JProperty("cancelledAmount",itemzx["cancelledAmount"].ToString()),
//                                    new JProperty("canReturnAmount",itemzx["canReturnAmount"].ToString()),
//                                    new JProperty("isOverreceive",itemzx["isOverreceive"].ToString()),
//                                    new JProperty("deptCode",itemzx["deptCode"].ToString()),
//                                    new JProperty("deptName",itemzx["deptName"].ToString()),
//                                    new JProperty("giveawayFlag",itemzx["giveawayFlag"].ToString()),
//                                    new JProperty("expectedArrivalDate",itemzx["expectedArrivalDate"].ToString()),
//                                    new JProperty("taxlessUnitPrice",itemzx["taxlessUnitPrice"].ToString()),
//                                    new JProperty("taxUnitPrice",itemzx["taxUnitPrice"].ToString()),
//                                    new JProperty("remark",itemzx["remark"].ToString()),
//                                };
//                            jarzx.Add(items);
//                        }
//                        JObject date = new JObject()
//                            {
//                                    new JProperty("purOrderOutId",item["purOrderOutId"].ToString()),
//                                    new JProperty("purOrderCode",item["purOrderCode"].ToString()),
//                                    new JProperty("purOrderName",item["purOrderName"].ToString()),
//                                    new JProperty("supplierCode",item["supplierCode"].ToString()),
//                                    new JProperty("supplierName",item["supplierName"].ToString()),
//                                    new JProperty("purType",item["purType"].ToString()),
//                                    new JProperty("purDeptCode",item["purDeptCode"].ToString()),
//                                    new JProperty("purDeptName",item["purDeptName"].ToString()),
//                                    new JProperty("deptCode",item["deptCode"].ToString()),
//                                    new JProperty("deptName",item["deptName"].ToString()),
//                                    new JProperty("purchaserCode",item["purchaserCode"].ToString()),
//                                    new JProperty("remark",item["remark"].ToString()),
//                                    new JProperty("outPurTypeCode",item["outPurTypeCode"].ToString()),
//                                    new JProperty("outOrderBusinessTypeCode",item["outOrderBusinessTypeCode"].ToString()),
//                                    new JProperty("items",jarzx),
//                            };

//                        jar.Add(date);

                        
//                        var client = new RestClient(config[0]["http"].ToString() + "/api/v1/mps/openApi/purorder/syn");
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

//                        Dbhple.Log.writestr("采购订单：" + item["purOrderCode"].ToString(), body.ToString(), responseMsg.ToString(), "_采购订单");
//                        if (resultBackSave.code.ToString() != "200")
//                        {
//                            throw new KDBusinessException("", resultBackSave.msg.ToString());
//                        }
//                        //string sql1 = string.Format(@"/*dialect*/ update ");
//                        //DBUtils.ExecuteDynamicObject(this.Context, sql1);

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
