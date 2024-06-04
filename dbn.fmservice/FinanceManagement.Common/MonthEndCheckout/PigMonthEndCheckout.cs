using Architecture.Common.HttpClientUtil;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.Common.MonthEndCheckout
{
    public class PigMonthEndCheckout : MonthEndCheckout
    {
        /// <summary>
        /// 物资结账
        /// </summary>
        FMAPIService FMAPIService;
        public PigMonthEndCheckout(FMAPIService FMAPIService)
        {
            this.FMAPIService = FMAPIService;
        }
        /// <summary>
        /// 结账
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override List<ResultModel> Checkout(FMSAccoCheckResultModel model, int menuid)
        {
            List<ResultModel> list = new List<ResultModel>();
            try
            {

                if (menuid == 99)
                {
                    ResultModel result = FMAPIService.CheckPigFarmLiveStock(model.EnterpriseID.ToString(), model.PigFarmIds, model.BeginDate, model.EndDate);
                    if (result.Code == 0)
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "猪场结账成功", ResultState = true });
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "猪场结账失败", ResultState = false });
                        return list;
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                list.Add(new ResultModel() { Code = 1, Msg = "", ResultState = false, ErrMsg = ex.ToString() });
                return list;
            }
        }
        /// <summary>
        /// 取消结账
        /// </summary>
        /// <returns></returns>
        public override List<ResultModel> CancelCheckout(FMSAccoCheckResultModel model, int menuid)
        {
            List<ResultModel> list = new List<ResultModel>();
            try
            {

                if (menuid == 99)
                {
                    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "取消猪场结账成功", ResultState = true });
                }
                return list;
            }
            catch (Exception ex)
            {
                list.Add(new ResultModel() { Code = 1, Msg = "", ResultState = false, ErrMsg = ex.ToString() });
                return list;
            }
        }
        /// <summary>
        /// 数据核对
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override List<ResultModel> DataCheck(FMSAccoCheckResultModel model)
        {
            List<ResultModel> list = new List<ResultModel>();
            try
            {

                int menuid = 1;
                //string checkmsg = string.Empty;
                //var result = FMAPIService.GetNotReviewNumbersExtend((long)AccoCheckTypeEnum.猪场结账, model.EnterpriseID, model.Date);
                //if (result.data.Count > 0)
                //{
                //    var groupResult = result.data.GroupBy(s => s.MenuId).ToList();
                //    foreach (var item in groupResult)
                //    {
                //        checkmsg += "【" + item.Key + "】、";
                //    }
                //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = $"{checkmsg.TrimEnd('、')}存在未审核单据", ResultState = false });
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = $"【2104141011350000101】、【2007092156450000101】、【2007092157330000101】、【2104141012380000101】全部已审核", ResultState = true });
                //}
                return list;
            }
            catch (Exception ex)
            {
                list.Add(new ResultModel() { Code = 1, Msg = "", ResultState = false, ErrMsg = ex.ToString() });
                return list;
            }
        }
    }
}
