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
namespace FinanceManagement.Common
{
    public class BankAccountBalanceUtil
    {
        HttpClientUtil _httpClientUtil1;

        HostConfiguration _hostCongfiguration;
        public BankAccountBalanceUtil(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
        }
        /// <summary>
        /// 资金日记账报表接口  
        /// </summary>
        public async Task<ResultModel<CapitalJournalDataResult>> GetCapitalJournalReport(object param)
        {
            var res = await _httpClientUtil1.PostJsonAsync<ResultModel<CapitalJournalDataResult>>($"{_hostCongfiguration.ReportService}/api/RptCapitalJournalRelated/GetCapitalJournalReport", param);
            return res;
        }
        /// <summary>
        /// 银行账户报表余额接口
        /// </summary>
        public BankAccountBalanceInnerResult GetYqtAccountBalInfo(FMAccount model)
        {
            Result JsonStr = null;
            string token = GetReadToken();
            List<BankAccountInquiryRequest> mslist = new List<BankAccountInquiryRequest>();
            BankAccountInquiryRequest info = SetAccountInfo( model);
            mslist.Add(info);
            var jsonmodel = new JsonModel();
            jsonmodel.list = mslist;
            var balUrl = _hostCongfiguration.YQTBalLink + "/api/read/qryBal?access_token=" + token;
            var yqtBalStr = RptBankAccountInquiryCommon.PostRequestWithFullUrlByJson(balUrl, JsonConvert.SerializeObject(jsonmodel));
            if (!string.IsNullOrEmpty(yqtBalStr))
            {
                JsonStr = JsonConvert.DeserializeObject<Result>(yqtBalStr);
                try
                {
                    if (JsonStr.data != null && !(JsonStr.data is string))
                    {
                        var data = JsonConvert.DeserializeObject<JObject>(JsonStr.data.ToString());
                       
                            var r = data[model.AccountNumber].ToObject<BankAccountBalanceBetweenOutAndInnerResult>();
                            BankAccountBalanceInnerResult inner = new BankAccountBalanceInnerResult();
                            if (r.data != null)
                            {
                                //resultList.Add(new ExecCashSweepResult { FromAccountNo = headAccount.AccountNumber, ToAccountNo = item.AccountNumber, code = r.data.code, msg = r.data.msg + "|[acntNo:" + headAccount.AccountNumber + ",ToAccountNo:" + item.AccountNumber + ",bankType:" + headAccount.BankType + ",clientId:" + headAccount.BankNumber + ",流水号：" + serialNumber + "]", RecordIDForCashSweep = item.RecordIDForCashSweep, IsSuccess = true });
                                inner = new BankAccountBalanceInnerResult { acctBal = r.data.acctBal, acntName = r.data.acntName, acntNo = r.data.acntNo, avlBal = r.data.avlBal, frzBal = r.data.frzBal, state = r.data.state, errCode = r.errCode, errMsg = r.errMsg };

                            }
                            else
                            {
                                inner = new BankAccountBalanceInnerResult { state = r.state.ToString(), errCode = r.errCode, errMsg = r.errMsg, acntNo = r.acntNo };
                            }
                        return inner;
                    }
                    

                }
                catch (Exception e)
                {
                   
                    return new  BankAccountBalanceInnerResult();
                }

            }
            return new BankAccountBalanceInnerResult();
        }
        /// <summary>
        /// 获取资金账户信息
        /// </summary>
        public async Task< ResultModel<FMAccount>> GetFMAccountData(FMAccount model)
        {
            var res = await _httpClientUtil1.PostJsonAsync<ResultModel<FMAccount>>($"{_hostCongfiguration.QlwServiceHost}/api/FD_AccountInventory/GetAccountSearch", model);
            return res;
        }
        private BankAccountInquiryRequest SetAccountInfo(FMAccount model)
        {
            BankAccountInquiryRequest info = new BankAccountInquiryRequest();
            #region 银行类型赋值
            if (model.BankID == ((long)FMSettleReceipType.MinShengBankDicID).ToString())
            {
                info.bankType = "CMBC";
            }
            //农行
            else if (model.BankID == ((long)FMSettleReceipType.ABCBankDicID).ToString())
            {
                info.bankType = "ABC";
            }
            //光大
            else if (model.BankID == ((long)FMSettleReceipType.CEBBankDicID).ToString())
            {
                info.bankType = "CEB";
            }
            //工行
            else if (model.BankID == ((long)FMSettleReceipType.ICBCBankDicID).ToString())
            {
                info.bankType = "ICBC";
                
            }//邮储
            else if (model.BankID == ((long)FMSettleReceipType.PSBCBankDicID).ToString())
            {
                info.bankType = "PSBC";
            }
            //农业发展
            else if (model.BankID == ((long)FMSettleReceipType.ADBCBankDicID).ToString())
            {
                info.bankType = "ADBC";
            }
            //交通银行
            else if (model.BankID == ((long)FMSettleReceipType.BCMBankDicID).ToString())
            {
                info.bankType = "BCM";
            }
            else
            {
                info.bankType = null;
            }
            #endregion
            info.acntNo = model.AccountNumber;
            info.clientId = model.BankNumber;
            info.dbProv = null;
            return info;
        }

        /// <summary>
        /// 获取金融Token
        /// </summary>
        /// <returns></returns>
        public string GetReadToken()
        {
            string YQTBalUrl = _hostCongfiguration.YQTBalLink;
            var strat = _hostCongfiguration.YQTAccessToken;
            var md5Str = RptBankAccountInquiryCommon.MD5Encrypt(strat);
            var atUrl = YQTBalUrl + "/oauth/token?grant_type=client_credentials&scope=read&client_id=NX-I-005&client_secret=" + md5Str;
            if (string.IsNullOrEmpty(YQTBalUrl) || string.IsNullOrEmpty(strat)) return null;
            var yqtatStr = RptBankAccountInquiryCommon.PostRequestWithFullUrlByJson(atUrl, "");
            var atval = JsonConvert.DeserializeObject<YQTATModel>(yqtatStr);
            if (atval != null && !string.IsNullOrEmpty(atval.Access_Token))
            {
                return atval.Access_Token;
            }
            return "";
        }

    }

    public class CapitalJournalDataResult
    {
        public string NumericalOrder { get; set; }
        public DateTime DataDate { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public decimal Balance { get; set; }
    }
  
    public class YQTATModel
    {
        public string Access_Token { get; set; }
    }
    public class JsonModel
    {
        public object list { get; set; }
    }
    public class BankAccountInquiryRequest
    {
        /// <summary>
        /// 帐号 
        /// </summary>
        public string acntNo { get; set; }
        /// <summary>
        /// 银行类型
        /// </summary>
        public string bankType { get; set; }
        /// <summary>
        /// 企业客户号
        /// </summary>
        public string clientId { get; set; }
        /// <summary>
        /// 省市代码
        /// </summary>
        public string dbProv { get; set; }
    }
    public enum FMSettleReceipType : long
    {
        //民生
        MinShengBankDicID = 201611050104402114,
        //农行
        ABCBankDicID = 201611050104402102,
        //光大
        CEBBankDicID = 201611050104402111,
        //工行
        ICBCBankDicID = 201611050104402101,
        //邮储
        PSBCBankDicID = 201611050104402106,
        //中国农业发展银行
        ADBCBankDicID = 1909051418580000102,
        //交通银行
        BCMBankDicID = 201611050104402115,
    }
    public class FMAccount
    {
        public string AccountID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccountType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccountFullName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long AccoSubjectID { get; set; }
        /// <summary>
        /// 科目编码
        /// </summary>
        public string AccoSubjectCode { get; set; }

        /// <summary>
        /// 科目名称
        /// </summary>		
        public string cAccoSubjectName { get; set; }

        /// <summary>
        /// 科目全称
        /// </summary>		
        public string cAccoSubjectFullName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccountUseType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long ResponsiblePerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OwnerID { get; set; }
        public string BankID { get; set; }
        public string BankNumber { get; set; }
        public bool bEnd { get; set; }
        public string PBankAreaID { get; set; }
        public string PBankAreaName { get; set; }
        public string BankFullAreaName { get; set; }
        public string AreaID { get; set; }
        public bool OpenBankEnterConnect { get; set; }//是否开通银企连接  
        public bool CoalPayment { get; set; }//是否开通西煤支付
        public string DepositBank { get; set; }//开户行
    }
    public class FD_BalanceadJustmentModel
    {
        public string AccountID { get; set; }
        public decimal Balance { get; set; }
        public decimal AcctBal { get; set; }
        public bool IsOpenBankEnterConnect { get; set; }
    }
    public class BankAccountBalanceBetweenOutAndInnerResult
    {

        /// <summary>
        /// state
        /// </summary>
        public int state { get; set; }
        /// <summary>
        /// errCode
        /// </summary>
        public string errCode { get; set; }
        /// <summary>
        /// errMsg
        /// </summary>
        public string errMsg { get; set; }
        /// <summary>
        /// data
        /// </summary>
        public BankAccountBalanceInnerResult data { get; set; }
        /// <summary>
        /// acntNo
        /// </summary>
        public string acntNo { get; set; }
    }
    /// <summary>
    /// 查询余额 金融返回的结果
    /// </summary>
    public class BankAccountBalanceInnerResult
    {

        /// <summary>
        /// 帐号
        /// </summary>
        public string acntNo { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string acntName { get; set; }

        /// <summary>
        /// 帐户余额
        /// </summary>
        public string acctBal { get; set; }

        /// <summary>
        /// 帐户可用余额
        /// </summary>
        public string avlBal { get; set; }

        /// <summary>
        /// 帐户冻结余额
        /// </summary>
        public string frzBal { get; set; }

        /// <summary>
        /// 查询状态1：失败；0：成功
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// 错误代码
        /// </summary>
        public string errCode { get; set; }

        /// <summary>
        /// 错误描述
        /// </summary>
        public string errMsg { get; set; }
    }
}
