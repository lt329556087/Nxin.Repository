using Architecture.Common.HttpClientUtil;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Linq;

namespace FinanceManagement.Common.NewMakeVoucherCommon
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
        public virtual string reqUrl { get; set; } = "";
        public abstract FD_SettleReceipt getVoucherList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, AuthenticationHeaderValue token = null);
        public virtual decimal CalculateValue(string formula)
        {
            try
            {
                formula = formula.Replace("×", "*").Replace("÷", "/").Replace("[", "").Replace("]", "");
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
        public virtual bool VerificationCheckout(FM_CarryForwardVoucherSearchCommand request, long appid, string type)
        {
            var param = new VerificationCheckoutModel { EnterpriseID = Convert.ToInt64(request.EnterpriseID), OwnerID = Convert.ToInt64(request.Boid), DataDate = Convert.ToDateTime(request.Enddate).ToString("yyyy-MM"), AppID = appid };
            string requrl = $"{this._hostCongfiguration.QlwServiceHost}/api/{request.EnterpriseID}/{request.Boid}/FM_AccoCheck/IsLockForm";
            var res = _httpClientUtil1.PostJsonAsync<ResultModel>(requrl, param).Result;
            return (bool)res?.ResultState;

        }

        public virtual T1 postActionByUrl<T1, T2>(T2 param) where T1 : class, new() where T2 : class, new()
        {
            try
            {
                LogHelper.LogWarning("请求地址：" + reqUrl + "参数" + JsonConvert.SerializeObject(param));
                var res = _httpClientUtil1.PostJsonAsync<T1>(this.reqUrl, param);
                return res.Result;
            }
            catch (Exception ex)
            {
                return new T1();
            }
        }

        public virtual FD_SettleReceiptDetailCommand NewObject<T1>(T1 summary, FM_NewCarryForwardVoucherDetailODataEntity item, bool isDebit, string formular, Dictionary<string, string> dictList)
        {
            Type type = summary.GetType();
            FD_SettleReceiptDetailCommand detail = new FD_SettleReceiptDetailCommand()
            {
                AccoSubjectCode = item.AccoSubjectCode,
                AccoSubjectID = item.AccoSubjectID,
                ReceiptAbstractID = item.ReceiptAbstractID,
                ReceiptAbstractName = item.ReceiptAbstractName,
                LorR = isDebit,
            };
            string formularRep = formular.Replace("×", "*").Replace("÷", "/");
            if (!string.IsNullOrEmpty(formularRep))
            {
                foreach (var dict in dictList)
                {
                    try
                    {
                        var value = type.GetProperty(dict.Value).GetValue(summary).ToString();
                        formularRep = formularRep.Replace("[" + dict.Key + "]", value);
                    }
                    catch
                    {
                        formularRep = formularRep.Replace("[" + dict.Key + "]", "0");
                    }
                }
                if (isDebit)
                {
                    if (formularRep.Contains("+") || formularRep.Contains("-") || formularRep.Contains("*") || formularRep.Contains("/")) { detail.Credit = this.CalculateValue(formularRep); }
                    else { decimal result = 0; decimal.TryParse(formularRep, out result); detail.Credit = result; }
                }
                else
                {
                    if (formularRep.Contains("+") || formularRep.Contains("-") || formularRep.Contains("*") || formularRep.Contains("/")) { detail.Debit = this.CalculateValue(formularRep); }
                    else { decimal result = 0; decimal.TryParse(formularRep, out result); detail.Debit = result; }
                }
            }
            return detail;
        }
        public virtual FD_SettleReceiptDetailCommand NewObjectExtend<T1>(T1 summary, FM_NewCarryForwardVoucherDetailODataEntity item, bool isDebit, string formular, List<DictinonaryModel> dictList)
        {
            Type type = summary.GetType();
            FD_SettleReceiptDetailCommand detail = new FD_SettleReceiptDetailCommand()
            {
                AccoSubjectCode = item.AccoSubjectCode,
                AccoSubjectID = item.AccoSubjectID,
                ReceiptAbstractID = item.ReceiptAbstractID,
                ReceiptAbstractName = item.ReceiptAbstractName,
                LorR = isDebit,
            };
            string formularRep = formular.Replace("×", "*").Replace("÷", "/");
            if (!string.IsNullOrEmpty(formularRep))
            {
                foreach (var formula in item.Formulas)
                {
                    if (!string.IsNullOrEmpty(formula.FormulaID))
                    {
                        var dict = dictList.Where(s => s.Key == formula.FormulaID).FirstOrDefault();
                        if (dict != null)
                        {
                            var value = type.GetProperty(dict.Value).GetValue(summary).ToString();
                            formularRep = formularRep.Replace("[" + dict.Key + "]", value);
                        }
                    }
                }
                if (isDebit)
                {
                    if (formularRep.Contains("+") || formularRep.Contains("-") || formularRep.Contains("*") || formularRep.Contains("/")) { detail.Credit = this.CalculateValue(formularRep); }
                    else { decimal result = 0; decimal.TryParse(formularRep, out result); detail.Credit = result; }
                }
                else
                {
                    if (formularRep.Contains("+") || formularRep.Contains("-") || formularRep.Contains("*") || formularRep.Contains("/")) { detail.Debit = this.CalculateValue(formularRep); }
                    else { decimal result = 0; decimal.TryParse(formularRep, out result); detail.Debit = result; }
                }
            }
            return detail;
        }
        public virtual bool IsContainsMethod(string formular, Dictionary<string, string> KeyList)
        {
            bool iscontains = false;
            foreach (var key in KeyList)
            {
                if (formular.Contains(key.Key))
                {
                    iscontains = true;
                    break;
                }
            }
            return iscontains;
        }
        /// <summary>
        /// 获取集团商品分类
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual List<TreeModelODataEntity> GetProductGroupClassAsync(string groupid, AuthenticationHeaderValue token)
        {
            //token = new AuthenticationHeaderValue("Bearer", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJpYXQiOjE2OTM4MTUwODIsImV4cCI6MTY5MzgyMjI4MiwidXNlcl9pZCI6IjE5ODY3ODgiLCJlbnRlcnByaXNlX2lkIjoiMzM5NjA1MiIsImdyb3VwX2lkIjoiMzE0MzQ3MSIsIm1lbnVfaWQiOiIyMjA4MTgwOTI4MDQwMDAwMTA5IiwiY2hpbGRfZW50ZXJwcmlzZV9pZCI6IjAiLCJwZXJtaXNzaW9ucyI6MzEsImlzcyI6ImlgbCIsImF1ZCI6InFsdy1kYm5mbSJ9.EsUpAJdAIHtMBRcuSp_9z_tYrofXkCZQ7bmqChaavIoCUenVAms40J4iNemtAP66Bxp-jvjp0E9a0l8MgrCELfsWRhwTzQD0YExsuhVCJYhvl_u4ea2j0ei3eVFb6FwHnNThD_S0G64QlXor1z-bTw86pi7DBpHdCeeD8T0GZQjvE8f94fSCAphmMUX7erZ-Hw7FJN3rjvXOC5VSWDjgVjnsaouKnnxyHM1H0I6iNt0lPFo4rUd9l5tXy1B29iOt8V-_78WKSwh5Pdt39BHjC7bjLhp3FYwoPQRvKTVuaLBzHWMNVrFeaCrU5kIl6HQGfDsbY9HKTK9HtELv35Rq_g");
            var result = _httpClientUtil1.PostJsonAsync<List<TreeModelODataEntity>>(_hostCongfiguration._wgUrl + "/dbn/fm/api/FMBase/GetTreeModelAsync", new { SortID = 1001, GroupID = groupid }, (a) => { a.Authorization = token; }).Result;
            if (result == null)
            {
                return new List<TreeModelODataEntity>();
            }
            return result;
        }
    }
    public class VerificationCheckoutModel : FMSBusinessResultModel
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
