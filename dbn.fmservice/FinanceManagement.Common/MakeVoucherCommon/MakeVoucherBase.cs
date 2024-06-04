using Architecture.Common.HttpClientUtil;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Common.MakeVoucherCommon
{
    public abstract class MakeVoucherBase
    {
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;
        public MakeVoucherBase(HttpClientUtil httpClientUtil1, HostConfiguration hostCongfiguration)
        {
            this._hostCongfiguration = hostCongfiguration;
            this._httpClientUtil1 = httpClientUtil1;
        }
        public virtual string reqUrl{get;set;}="";
        public abstract FD_SettleReceipt getVoucherList(FM_CarryForwardVoucherODataEntity model, FM_CarryForwardVoucherInterfaceSearchCommand request) ;
        public virtual decimal CalculateValue(string formula)
        {
            try
            {
                formula = formula.Replace("×", "*").Replace("÷", "/").Replace("[","").Replace("]","");
                System.Data.DataTable dt = new System.Data.DataTable();
                var obj = Convert.ToDecimal(dt.Compute(formula, "").ToString());
                return obj;
            }
            catch (Exception ex)
            {
                return 0;
            }
            return 0;
        }
        /// <summary>
        /// 会计结账
        /// </summary>
        public virtual bool VerificationCheckout(FM_CarryForwardVoucherInterfaceSearchCommand request,long appid,string type )
        {
            List<string> typeList = new List<string>() { "1911081429200000101", "1911081429200000102", "1611091727140000101" };
            if (typeList.IndexOf(type)>=0)//销售采购结转传datadate
            {
                var param = new VerificationCheckoutModel { EnterpriseID = Convert.ToInt64(request.EnterpriseID), OwnerID = Convert.ToInt64(request.Boid), DataDate = Convert.ToDateTime(request.DataDate).ToString("yyyy-MM"), AppID = appid };
                string requrl = $"{this._hostCongfiguration.QlwServiceHost}/api/{request.EnterpriseID}/{request.Boid}/FM_AccoCheck/IsLockForm";
                var res = _httpClientUtil1.PostJsonAsync<ResultModel>(requrl, param).Result;
                return (bool)res?.ResultState;
            }
            else
            {
                var param = new VerificationCheckoutModel { EnterpriseID = Convert.ToInt64(request.EnterpriseID), OwnerID = Convert.ToInt64(request.Boid), DataDate = Convert.ToDateTime(request.Enddate).ToString("yyyy-MM"), AppID = appid };
                string requrl = $"{this._hostCongfiguration.QlwServiceHost}/api/{request.EnterpriseID}/{request.Boid}/FM_AccoCheck/IsLockForm";
                var res = _httpClientUtil1.PostJsonAsync<ResultModel>(requrl, param).Result;
                return (bool)res?.ResultState;
            }
            
        }

        public virtual T1 postActionByUrl<T1, T2>(T2 param) where T1 : class, new() where T2 : class, new()
        {
            try
            {
                var res = _httpClientUtil1.PostJsonAsync<T1>(this.reqUrl, param);
                return res.Result;
            }
            catch (Exception ex)
            {
                return new T1();
            }
        }
        /// <summary>
        /// 固定公式解析
        /// </summary>
        public  virtual FD_SettleReceiptDetailCommand NewObject<T1>(T1 summary, FM_CarryForwardVoucherDetailODataEntity item,bool isDebit,string formular, Dictionary<string, string> dictList)
        {
            Type type = summary.GetType();
            FD_SettleReceiptDetailCommand detail = new FD_SettleReceiptDetailCommand()
            {
                AccoSubjectCode = item.AccoSubjectCode,
                AccoSubjectID = item.AccoSubjectID,
                ReceiptAbstractID = item.ReceiptAbstractID,
                ReceiptAbstractName=item.ReceiptAbstractName,
                LorR = isDebit,
            };
            string formularRep = formular.Replace("×", "*").Replace("÷", "/").Replace("[", "").Replace("]", "");
            if (!string.IsNullOrEmpty(formularRep))
            {
                foreach (var dict in dictList)
                {
                    var value = type.GetProperty(dict.Value).GetValue(summary).ToString();
                    formularRep = formularRep.Replace(dict.Key.ToString(), value);
                }
                if (isDebit)
                {
                    if (formularRep.Contains("+") || formularRep.Contains("-") || formularRep.Contains("*") || formularRep.Contains("/")) { detail.Debit = this.CalculateValue(formularRep); }
                    else { decimal result = 0; decimal.TryParse(formularRep, out result); detail.Debit = result; }
                }
                else
                {
                    if (formularRep.Contains("+") || formularRep.Contains("-") || formularRep.Contains("*") || formularRep.Contains("/")) { detail.Credit = this.CalculateValue(formularRep); }
                    else { decimal result = 0; decimal.TryParse(formularRep, out result); detail.Credit = result; }
                }
            }
            return detail;
        }
    }
    public class VerificationCheckoutModel: FMSBusinessResultModel
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        public long RecordID { get; set; }
        /// <summary>
        /// 明细流水号
        /// </summary>
        public long NumericalOrderDetail { get; set; }
        /// <summary>
        /// 结账类型
        /// </summary>
        public long AccoCheckType { get; set; }
        /// <summary>
        /// 菜单、步骤
        /// </summary>
        public int MenuId { get; set; }

    }
    public class FMSBusinessResultModel
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public long AppID { get; set; }

        /// <summary>
        /// 集团ID
        /// </summary>
        public long GroupID { get; set; }

        /// <summary>
        /// 企业ID
        /// </summary>
        public long EnterpriseID { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        public long OwnerID { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public string DataDate { get; set; }

        private DateTime date;
        /// <summary>
        /// DataDate 转换后的日期
        /// 使用前，必须调用 CheckDataDate() 方法
        /// </summary>
        public DateTime Date
        {
            get { return date; }
            protected set { date = value; }
        }

        /// <summary>
        /// 结束日期
        /// </summary>
        public string DataEndDate { get; set; }

        private DateTime endDate;
        /// <summary>
        /// DataEndDate 转换后的日期
        /// 使用前，必须调用 CheckDataEndDate() 方法
        /// </summary>
        public DateTime EndDate
        {
            get { return endDate; }
            protected set { endDate = value; }
        }

        /// <summary>
        /// 流水号
        /// </summary>
        public long NumericalOrder { get; set; }

        
    }
}
