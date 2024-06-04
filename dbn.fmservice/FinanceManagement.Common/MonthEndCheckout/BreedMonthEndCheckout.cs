using Architecture.Common.HttpClientUtil;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;

namespace FinanceManagement.Common.MonthEndCheckout
{
    public class BreedMonthEndCheckout : MonthEndCheckout
    {

        FMAPIService FMAPIService;
        public BreedMonthEndCheckout(FMAPIService FMAPIService)
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
                    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "猪场结账成功", ResultState = true });
                }
                return list;
            }
            catch (Exception ex)
            {
                return new List<ResultModel>() { new ResultModel() { Code = 1, Msg = ex.ToString(), ResultState = false, } };
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
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "猪场取消结账成功", ResultState = true });
                }
                return list;
            }
            catch (Exception ex)
            {
                return new List<ResultModel>() { new ResultModel() { Code = 1, Msg = ex.ToString(), ResultState = false, } };
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
