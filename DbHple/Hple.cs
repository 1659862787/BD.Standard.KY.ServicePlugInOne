using Kingdee.BOS.Orm.DataEntity;
using BD.Standard.KY.ServicePlugInOneV23.DbHple;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Standard.KY.ServicePlugInOneV23.Dbhple
{
    public class Hple
    {
        public static string MesLogin(DynamicObjectCollection data)
        {
            JArray jar = new JArray();

            JObject objWL = new JObject()
            {
                new JProperty("sign",data[0]["sign"].ToString()),
                new JProperty("tenantCode",data[0]["tenantCode"].ToString())
               /* new JProperty("sign","NL05C2dzyKNRjiM/kfctGpY83k+K/6XI8BB7OCo0IJw/p9rYXTPI3tsz01iKdKAwZmRVlLy0+oeKsscLF96P0xQ6rEBvDTYGA46piRTUI63SYoHqDv1O0cK3fTloxfC2WW7lb+ard1HyYGhagdBd+L82TWmul680cal6Ge8fuWc="),
                new JProperty("tenantCode","kyadmin")*/
            };

            //接口地址
            
            var client = new RestClient(data[0]["http"].ToString() + "/api/v1/org/auth/tenantLogin");
            client.Timeout = -1;
            //定义get post请求
            var request = new RestRequest(Method.POST);
            //定义参数
            //request.AddHeader("kdservice-sessionid", resultBacklogin.KDSVCSessionId);
            request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("Cookie", resultBacklogin.KDSVCSessionId);
            var body = objWL;
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            var responseJG = response.Content;//获取返回Response消息
            //Dbhple.Log.writestr(body, responseJG);


            var resultBackJG = JsonConvert.DeserializeObject<Root>(responseJG);

            return resultBackJG.data.accessToken.ToString();
        }


        public class Data
        {
            /// <summary>
            /// 
            /// </summary>
            public string accessToken { get; set; }
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
            public Data data { get; set; }
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
