using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Core;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Util;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;



namespace FinanceManagement.Common
{
    public class FinanceTradeUtil
    {
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;
        private readonly ILogger<FinanceTradeUtil> _logger;
        public FinanceTradeUtil(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration, ILogger<FinanceTradeUtil> logger)
        {
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            _logger = logger;
        }
        ///// <summary>
        ///// 资金汇总表 
        ///// </summary>
        //public List<FundSummaryResult> GetFundsSummaryReport(object param)
        //{
        //    var res = postMessage($"{_hostCongfiguration.ReportService}/api/FundsSummary/GetSummaryAsync",JsonConvert.SerializeObject(param));
        //    if (!string.IsNullOrEmpty(res))
        //    {
        //        var resultmodel =JsonConvert.DeserializeObject<ResultModel>(res);
        //        if (resultmodel?.ResultState==true && resultmodel?.Data != null)
        //        {
        //           var list = JsonConvert.DeserializeObject<List<FundSummaryResult>>(resultmodel.Data.ToString());
        //            return list;
        //        }
        //    }
        //    return null;
        //}
        #region 获取实时余额
        public Tuple<string, decimal> GetYqtBal(string acc_no, string bank_type, string client_id)
        {
            Tuple<string, decimal> retResult = null;
            decimal acctBal = 0;
            try
            {
                var retStr = GetYqtBalReq(acc_no,bank_type,client_id);
                if (string.IsNullOrEmpty(retStr))
                {
                    retResult = new Tuple<string, decimal>("查询余额返回空", acctBal);
                    return retResult;
                }
                var yqtRetStr = JsonConvert.DeserializeObject< RestfulResult>(retStr);
                if (yqtRetStr == null)
                {
                    retResult = new Tuple<string, decimal>("查询余额序列化空", acctBal);
                    return retResult;
                }
                #region
                //成功
                if (yqtRetStr.code == 0)
                {
                    if (yqtRetStr.data != null && !(yqtRetStr.data is string))
                    {
                        var objResult = JsonConvert.DeserializeObject<JObject>(yqtRetStr.data.ToString());


                        var balData = objResult.ToObject<BalDataModel>();

                        //返回状态 1:成功 0：失败
                        if (balData != null)
                        {
                            if (balData.resCode == "1")//成功
                            {
                                decimal.TryParse(string.IsNullOrEmpty(balData.accBal) ? "0" : balData.accBal, out acctBal);
                                retResult = new Tuple<string, decimal>("", acctBal);
                            }
                            else
                            {
                                retResult = new Tuple<string, decimal>("查询余额"+""+balData.resMsg + ";" + balData.resCode + "-" + balData.detailCode, acctBal);
                            }
                        }
                        else
                        {
                            retResult = new Tuple<string, decimal>("银企查询余额数据空", acctBal);
                            return retResult;
                        }

                    }
                    else
                    {
                        retResult = new Tuple<string, decimal>(string.Format("银企查询余额响应数据空{0}{1}", yqtRetStr.msg, yqtRetStr.data), acctBal);
                    }
                }
                else
                {
                    retResult = new Tuple<string, decimal>(string.Format("银企查询余额失败{0}", yqtRetStr.msg), acctBal);
                }
                #endregion
            }
            catch (Exception ex)
            {
                retResult = new Tuple<string, decimal>(string.Format("银企查询余额失败{0}", ex.Message), acctBal);
            }
            return retResult;
        }
        public string GetYqtBalReq(string acc_no, string bank_type, string client_id)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("acc_no", acc_no);
            dic.Add("client_id", client_id);
            dic.Add("bank_type", bank_type);
            dic.Add("times", DateTime.Now.ToString("yyyyMMddHHmm"));
            dic.Add("busi_no", "NX-I-005"); /*业务来源*/
            //获取sign
            var sgin = "";
            var key = _hostCongfiguration.OrderKey ?? "5dcc96e21db642dab159f97dbe61ff39";
            dic.Add("key", key); /*发送平台的key*/
            Dictionary<string, object> sortMap = new Dictionary<string, object>();

            sortMap = dic.OrderBy(t => t.Key).ToDictionary(t => t.Key, t => t.Value);

            foreach (var t in sortMap)
            {
                sgin += t.Key + t.Value;
            }
            var md5Str = RptBankAccountInquiryCommon.MD5Encrypt(sgin);

            dic.Add("signmsg", md5Str);
            dic.Add("open_req_src", "qlw-web");

            dic = dic.Where(t => t.Key != "key").ToDictionary(t => t.Key, t => t.Value);
            dic = dic.OrderBy(t => t.Key).ToDictionary(t => t.Key, t => t.Value);

            string ps = "";
            foreach (var keyValuePair in dic)
            {
                ps += keyValuePair.Key + "=" + keyValuePair.Value + "&";
            }

            ps = ps.TrimEnd('&');
            var url = $"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.jrbase.yqt.bal.get/1.0";
            var responseStr= postMessage(url, ps);
            _logger.LogInformation($"获取余额:url={url},参数={ps}");
            return responseStr;
        }
        /// <summary>
        /// 输出签名
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetSignWithKey(Dictionary<string, object> dic)
        {
            var sgin = "";
            if (dic.Any())
            {
                var key = _hostCongfiguration.OrderKey ?? "5dcc96e21db642dab159f97dbe61ff39";
                dic.Add("key", key); /*发送平台的key*/
                Dictionary<string, object> sortMap = new Dictionary<string, object>();

                sortMap = dic.OrderBy(t => t.Key).ToDictionary(t => t.Key, t => t.Value);

                foreach (var t in sortMap)
                {
                    sgin += t.Key + t.Value;
                }
            }
            var md5Str = RptBankAccountInquiryCommon.MD5Encrypt(sgin);
            return md5Str;
        }
        #endregion

        #region 资金汇总表
        public List<FundsSummaryResult> GetFundsSummaryData(RptAgingReclassificationRequest param)
        {
            try
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                var responseStr = postMessage($"{_hostCongfiguration.ReportService}/api/FundsSummary/GetSummaryAsync", JsonConvert.SerializeObject(param));
                sw.Stop();
                _logger.LogInformation("资金汇总表： 时间：" + sw.ElapsedMilliseconds);
                if (string.IsNullOrEmpty(responseStr))
                {
                    return null;
                }
                var data = JsonConvert.DeserializeObject<ResultModel<FundsSummaryResult>>(responseStr);
               
                if (data == null)
                {
                    return null;
                }
                return data.Data;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region 权限单位
        /// <summary>
        /// 获取菜单权限单位
        /// </summary>
        public async  Task<List<Biz_EnterpirseEntityODataEntity>> GetPermissionMenuEnter(PermissionEnter model)
        {
            List<Biz_EnterpirseEntityODataEntity> list = null;
            try
            {
                var url = $"{_hostCongfiguration.NxinGatewayUrl}api/nxin.permission.enterlistbymenuids.list/2.0?open_req_src=nxin_shuju&enterpriseid={model.EnterpriseID}&boid={model.Bo_ID}&menuid={model.MenuID}";
                var requestResult =await _httpClientUtil1.GetJsonAsync<RestfulResult>(url) ;
                if (requestResult?.code == 0 && requestResult?.data != null)
                {
                    list = JsonConvert.DeserializeObject<List<Biz_EnterpirseEntityODataEntity>>(requestResult.data.ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("FinanceTradeUtil/GetPermissionMenuEnter:{0},param={1}", ex.ToString(), JsonConvert.SerializeObject(model)));
                throw ex;
            }
        }
        #endregion

        #region 历史余额
        public string GetYqtAccountBalByDate(Domain.FDAccountSearch search, List<FD_AccountAODataEntity> msBankList)
        {
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();

                var mslist = msBankList.Select(p => new { acc_no = p.AccountNumber, bank_type = p.BankType, client_id = p.BankNumber, query_date = search.DataDate });
                dic.Add("data", JsonConvert.SerializeObject(mslist));
                dic.Add("times", DateTime.Now.ToString("yyyyMMddHHmm"));
                dic.Add("busi_no", "NX-I-005");

                string sign = GetSignWithKey(dic); //获取sign

                dic.Add("signmsg", sign);
                dic.Add("open_req_src", "qlw-web");
                dic = dic.Where(t => t.Key != "key").ToDictionary(t => t.Key, t => t.Value);
                dic = dic.OrderBy(t => t.Key).ToDictionary(t => t.Key, t => t.Value);

                string ps = "";
                foreach (var keyValuePair in dic)
                {
                    ps += keyValuePair.Key + "=" + keyValuePair.Value + "&";
                }

                ps = ps.TrimEnd('&');
                var url = $"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.jrbase.yqt.history.list.bal.get/1.0";
                var responseStr = postMessage(url, ps);

                _logger.LogInformation(string.Format("FinanceTradeUtil/GetYqtAccountBalByDate:历史余额--请求参数：{0},返回结果：{1}", JsonConvert.SerializeObject(dic), responseStr));

               
                return responseStr;
            }
            catch (Exception e)
            {
                _logger.LogError(string.Format("FinanceTradeUtil/GetYqtAccountBalByDate-历史余额：search={0},msBankList={1},异常：{2}", JsonConvert.SerializeObject(search), JsonConvert.SerializeObject(msBankList), e.ToString()));
            }
            return string.Empty;
        }
        #endregion
        public string postMessage(string strUrl, string strPost)
        {
            try
            {
                CookieContainer objCookieContainer = null;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.Method = "Post";
                request.Accept = "*/*";
                request.Headers.Add("Accept-Language: zh-CN,zh;q=0.8");
                request.Headers.Add("Accept-Charset: GBK,utf-8;q=0.7,*;q=0.3");
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 10000;

                request.Referer = strUrl;//.Remove(strUrl.LastIndexOf("/"));
                Console.WriteLine(strUrl);
                if (objCookieContainer == null)
                    objCookieContainer = new CookieContainer();

                request.CookieContainer = objCookieContainer;
                if (!string.IsNullOrEmpty(strPost))
                {
                    byte[] byteData = Encoding.UTF8.GetBytes(strPost.ToString().TrimEnd('&'));
                    request.ContentLength = byteData.Length;
                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(byteData, 0, byteData.Length);
                        reqStream.Close();
                    }
                }

                string strResponse = "";
                using (HttpWebResponse res = (HttpWebResponse)request.GetResponse())
                {
                    objCookieContainer = request.CookieContainer;
                    res.Cookies = objCookieContainer.GetCookies(new Uri(strUrl));

                    using (Stream resStream = res.GetResponseStream())
                    {
                        byte[] buffer = new byte[1024];
                        int read;
                        while ((read = resStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            strResponse += Encoding.UTF8.GetString(buffer, 0, read);
                        }
                    }
                    res.Close();
                }
                return strResponse;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }

    public class RestfulResult
    {
        public int code { get; set; }
        public string msg { get; set; }
        public object data { get; set; }
    }
    //实时余额
    public class BalDataModel : BaseResponse
    {
        /// <summary>
        /// 帐号
        /// </summary>
        public string accNo { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string accName { get; set; }
        /// <summary>
        /// 帐户余额
        /// </summary>
        public string accBal { get; set; }
        /// <summary>
        /// 帐户可用余额
        /// </summary>
        public string avlBal { get; set; }
        /// <summary>
        /// 帐户冻结余额
        /// </summary>
        public string frzBal { get; set; }


    }
    public class BaseResponse
    {
        /// <summary>
        /// 状态0：失败；1：成功
        /// </summary>
        public string resCode { get; set; }
        public string detailCode { get; set; }
        public string resMsg { get; set; }
    }
    //资金汇总表
    public class FundsSummaryResult
    {
        public decimal EnddingAmount { get; set; }
    }
    public class PermissionEnter 
    {
        public string EnterpriseID { get; set; }
        public string Bo_ID { get; set; }
        public string MenuID { get; set; }
    }
    public class CardResult : RestfulResult
    {
        public object errorinfo { get;set; }
    }
}
