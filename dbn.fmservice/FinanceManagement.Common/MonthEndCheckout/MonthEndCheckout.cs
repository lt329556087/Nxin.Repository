using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FinanceManagement.Common.MonthEndCheckout
{
    #region 月末结账工厂类

    /// <summary>
    /// 月末结账工厂类
    /// </summary>
    public class MonthEndCheckoutFactory
    {
        FMAPIService FMAPIService;
        public MonthEndCheckoutFactory(FMAPIService FMAPIService)
        {
            this.FMAPIService = FMAPIService;
        }
        public MonthEndCheckout CreateMonthEndCheckout(AccoCheckTypeEnum accoCheckType)
        {
            try
            {
                switch (accoCheckType)
                {
                    case AccoCheckTypeEnum.销售结账:
                        return new SaleMonthEndCheckout(FMAPIService);
                    case AccoCheckTypeEnum.采购结账:
                        return new PurchaseMonthEndCheckout(FMAPIService);
                    //case AccoCheckTypeEnum.折扣分摊:
                    //    return new DiscountMonthEndCheckout();
                    case AccoCheckTypeEnum.存货结账:
                        return new StockMonthEndCheckout(FMAPIService);
                    case AccoCheckTypeEnum.会计结账:
                        return new FinanceMonthEndCheckout(FMAPIService);
                    //case AccoCheckTypeEnum.出厂成本:
                    //    return new FactoryCostMonthEndCheckout();
                    case AccoCheckTypeEnum.固定资产:
                        return new FixedAssetsMonthEndCheckout(FMAPIService);
                    case AccoCheckTypeEnum.成本管理:
                        return new CostMonthEndCheckout(FMAPIService);
                    case AccoCheckTypeEnum.物品结账:
                        return new SuppliesMonthEndCheckout(FMAPIService);
                    case AccoCheckTypeEnum.猪场结账:
                        return new PigMonthEndCheckout(FMAPIService);
                    //case AccoCheckTypeEnum.羊场结账:
                    //    return new SheepMonthEndCheckout();
                    //case AccoCheckTypeEnum.应收结账:
                    //    return new ReceivablesMonthEndCheckout();
                    //case AccoCheckTypeEnum.应付结账:
                    //    return new PaymentMonthEndCheckout();
                    case AccoCheckTypeEnum.放养管理:
                        return new BreedMonthEndCheckout(FMAPIService);
                    default:
                        break;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    #endregion

    #region 月末结账抽象类

    /// <summary>
    /// 月末结账抽象类
    /// </summary>
    public abstract class MonthEndCheckout
    {
        /// <summary>
        /// 数据核对
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public abstract List<ResultModel> DataCheck(FMSAccoCheckResultModel model);
        /// <summary>
        /// 结账
        /// </summary>
        /// <param name="model.MenuId">当前结账步骤</param>
        /// <returns></returns>
        public abstract List<ResultModel> Checkout(FMSAccoCheckResultModel model, int menuid);
        /// <summary>
        /// 取消结账
        /// </summary>
        /// <param name="model.MenuId">当前结账步骤</param>
        /// <returns></returns>
        public abstract List<ResultModel> CancelCheckout(FMSAccoCheckResultModel model, int menuid);
        /// <summary>
        /// 重新结账
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual ResultModel ReCheckout(FMSAccoCheckResultModel model)
        {
            return new ResultModel();
        }

        #region 一致性检测详情
        /// <summary>
        /// 数据核对详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual ResultModel DataDetailCheck(FMSAccoCheckResultModel model)
        {
            return new ResultModel();
        }
        #endregion
     
    }

    #endregion
}
