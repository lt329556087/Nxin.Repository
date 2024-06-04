using System.IO;
using System.Net;
using System.Text;
using Architecture.Common.HttpClientUtil;
using FinanceManagement.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FinanceManagement.Common
{
    public class AgingDataUtil
    {
        HttpClientUtil _httpClientUtil;

        HostConfiguration _hostCongfiguration;

        public AgingDataUtil(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _httpClientUtil = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
        }

        public async Task<List<DealingOccurDataResult>> GetAgingData(DealingOccurRequest param)
        {
            var data = await _httpClientUtil.PostJsonAsync<ResultModel<DealingOccurDataResult>>($"{_hostCongfiguration.ReportService}/api/RptDealingOccur/GetSummaryReport", param);
            if (data == null)
            {
                return null;
            }
            return data.Data;
        }
    }

    public class DealingOccurRequest
    {
        /// <summary>
        /// 结束日期
        /// </summary>
        public string Enddate { get; set; }
        /// <summary>
        /// 会计科目
        /// </summary>
        public string AccountingSubjectsID { get; set; }
        /// <summary>
        /// 账龄类型
        /// </summary>
        public string IntervalType { get; set; }
        /// <summary>
        /// 人员类型
        /// </summary>
        public string CustomerType { get; set; }
        public string CustomerID { get; set; }
        public string PersonID { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string EnterpriseList { get; set; }
        public bool OnlyCombineEnte { get; set; }

        public string GroupID { get; set; }
        public string EnteID { get; set; }
        public string Boid { get; set; }

        public string MenuParttern { get; set; }

        public List<string> OwnEntes { get; set; }

    }

    public class DealingOccurDataResult
    {
        public string SummaryType1 { get; set; }
        public string SummaryType1Name { get; set; }
        public string SummaryType1FieldName { get; set; }
        public string SummaryType2 { get; set; }
        public string SummaryType2Name { get; set; }
        public string SummaryType2FieldName { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string EnterpriseName { get; set; }

        /// <summary>
        /// 期末余额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 账龄
        /// </summary>
        public List<AgingintervalData> AgingintervalDatas { get; set; }
        public string msg { get; set; }
    }

    public class AgingintervalData
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }

        public decimal Credit { get; set; }

        public decimal Debit { get; set; }
        public int Serial { get; set; }
    }


    public enum BusinessTypeEnum : long
    {
        Customer = 201611160104402101,
        Person = 201611160104402103,
    }

    /// <summary>
    /// 科目余额实时
    /// </summary>
    public class SubjectBalanceUtil
    {
        HttpClientUtil _httpClientUtil;
        HostConfiguration _hostCongfiguration;

        public SubjectBalanceUtil(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _httpClientUtil = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
        }

        public async Task<List<SubjectBalanceResult>> GetData(SubjectBalanceRequest param)
        {
            var data = await _httpClientUtil.PostJsonAsync<ResultModel<SubjectBalanceResult>>($"{_hostCongfiguration.ReportService}/api/RetSubjectBalance/GetBalanceReport", param);
            if (data == null)
            {
                return null;
            }
            return data.Data;
        }
    }

    public class SubjectBalanceRequest
    {
        public string Begindate { get; set; }
        public string Enddate { get; set; }
        public string AccountingSubjectsRadio { get; set; }
        public string AccountingSubjectsRadio2 { get; set; }
        public string AccountingType { get; set; }
        public bool OnlyCombineEnte { get; set; }
        public string EnterpriseList { get; set; }
        public string SummaryType1 { get; set; }
        public List<string> OwnEntes { get; set; }
        public List<string> CanWatchEntes { get; set; }
        public string IsGroupByEnteCate { get; set; }
        public string EnteCateSummary { get; set; }
        public string GroupID { get; set; }
        public string EnteID { get; set; }
        public string Boid { get; set; }
        public string MenuParttern { get; set; }
    }

    public class SubjectBalanceResult
    {
        public string AccoSubjectCode { get; set; }
        public string AccoSubjectName { get; set; }
        public string AccoSubjectFullName { get; set; }
        public string AccoSubjectID { get; set; }
        public bool IsLorR { get; set; }
        public bool IsCus { get; set; }
        public bool IsSup { get; set; }
        public bool IsPerson { get; set; }
        public string AccoSubjectType { get; set; }
        public decimal qcDebit { get; set; }
        public decimal qcCredit { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal DebitYear { get; set; }
        public decimal CreditYear { get; set; }
        public decimal fsDebit { get; set; }
        public decimal fsCredit { get; set; }
        public string SummaryType1 { get; set; }
        public string SummaryType1Name { get; set; }
    }


    public class FundSummaryRequest
    {
        public bool IsEnterCate { get; set; } = false;
        public string SummaryType { get; set; } = "d.AccountName";
        public List<string> SummaryTypeList { get; set; } = new List<string>() { "d.AccountName" };
        public string GroupId { get; set; }
        public string EnterpriseId { get; set; }
        public string BoId { get; set; }
        public string MenuParttern { get; set; } = "0";
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public List<string> OwnEntes { get; set; }
        public bool FilterAccount { get; set; }
        public bool OpenBankEnterConnect { get; set; }
    }

    public class FundSummaryResult
    {
        public decimal EnddingAmount { get; set; }
        public string SummaryType { get; set; }
        public string SummaryTypeName { get; set; }
    }

    public class FundSummaryUtil
    {
        HttpClientUtil _httpClientUtil;
        HostConfiguration _hostCongfiguration;

        public FundSummaryUtil(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _httpClientUtil = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
        }

        public async Task<List<FundSummaryResult>> GetData(FundSummaryRequest param)
        {
            var data = await _httpClientUtil.PostJsonAsync<ResultModel<FundSummaryResult>>($"{_hostCongfiguration.ReportService}/api/FundsSummary/GetSummaryAsync", param);
            if (data == null)
            {
                return null;
            }
            return data.Data;
        }

    }


    public class RptAgingReclassificationUtil
    {
        HttpClientUtil _httpClientUtil;
        HostConfiguration _hostCongfiguration;
        private readonly ILogger<RptAgingReclassificationUtil> _logger;
        public RptAgingReclassificationUtil(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration, ILogger<RptAgingReclassificationUtil> logger)
        {
            _httpClientUtil = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            _logger = logger;
        }

        public async Task<List<RptAgingReclassificationResult>> GetData(RptAgingReclassificationRequest param)
        {
            try
            {
                //测试代码
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                var data = await _httpClientUtil.PostJsonAsync<ResultModel<RptAgingReclassificationResult>>($"{_hostCongfiguration.ReportService}/api/RptAgingReclassification/GetSummaryReport", param);
                //var responseStr = postMessage($"{_hostCongfiguration.ReportService}/api/RptAgingReclassification/GetSummaryReport", JsonConvert.SerializeObject(param));
                //var responseStr = postMessage($"http://demorptserviceqlw.nxin.com/api/RptAgingReclassification/GetSummaryReport",JsonConvert.SerializeObject( param));
                //_logger.LogInformation(" 重分类/GetData:result：" + responseStr);
                sw.Stop();
                _logger.LogInformation(" 重分类/GetData 时间：" + sw.ElapsedMilliseconds);

                //if (string.IsNullOrEmpty(responseStr))
                //{
                //    return null;
                //}
                //var data = JsonConvert.DeserializeObject<ResultModel<RptAgingReclassificationResult>>(responseStr);
                //_logger.LogInformation(" 重分类/GetData:result：" + JsonConvert.SerializeObject(data) + "\n 参数" + JsonConvert.SerializeObject(param)+"\n 时间："+sw.ElapsedMilliseconds);
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
        #region 公用
        public string postMessage(string strUrl, string strPost)
        {
            try
            {
                CookieContainer objCookieContainer = null;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.Method = "Post";
                request.ContentType = "application/json;charset=UTF-8";
                request.Accept = "application/json;charset=UTF-8";
                request.Timeout = 180000;

                request.Referer = strUrl;
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
                        var sReader=new StreamReader(resStream, Encoding.GetEncoding("utf-8"));
                        strResponse = sReader.ReadToEnd();
                        sReader.Close();
                        //byte[] buffer = new byte[1024];
                        //int read;
                        //while ((read = resStream.Read(buffer, 0, buffer.Length)) > 0)
                        //{
                        //    strResponse += Encoding.UTF8.GetString(buffer, 0, read);
                        //}
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
        #endregion
    }

    public class RptAgingReclassificationRequest
    {
        public string Enddate { get; set; }
        public string AccountingSubjectsID { get; set; }
        public string IntervalType { get; set; }
        //1:客户 2：个人
        public string CustomerType { get; set; }
        public string CustomerID { get; set; }
        public string PersonID { get; set; }
        public string EnterpriseList { get; set; }
        public int SubjectLevel { get; set; } = 1;
        public string SummaryType1 { get; set; }
        public string SummaryType1Name { get; set; }

        public List<string> OwnEntes { get; set; }
        public List<string> CanWatchEntes { get; set; }
        public string SummaryT1Rank { get; set; }
        public string GroupID { get; set; }
        public string EnteID { get; set; }
        public string Boid { get; set; }
        public string MenuParttern { get; set; } = "0";
        public bool FilterSubject { get; set; }
        //public int BusiType { get; set; }
    }

    public class RptAgingReclassificationResult
    {
        public decimal ReclassAmount { get; set; }
        public decimal AdjustAmount { get; set; }

        public string SummaryType1 { get; set; }
        public string SummaryType1Name { get; set; }

        public string SummaryType2 { get; set; }
        public string SummaryType2Name { get; set; }
        public decimal Amount { get; set; }

        public string AccoSubjectCode { get; set; }


        public List<AgingintervalData> AgingintervalDatas { get; set; }
    }
    public class AgingReclassificationRequestModel
    {
        public string DataDate { get; set; }
        public int BusiType { get; set; }
    }
}




