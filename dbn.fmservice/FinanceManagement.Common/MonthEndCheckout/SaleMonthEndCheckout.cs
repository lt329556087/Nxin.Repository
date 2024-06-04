using Architecture.Common.HttpClientUtil;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;

namespace FinanceManagement.Common.MonthEndCheckout
{
    public class SaleMonthEndCheckout : MonthEndCheckout
    {

        FMAPIService FMAPIService;
        public SaleMonthEndCheckout(FMAPIService FMAPIService)
        {
            this.FMAPIService = FMAPIService;
        }
        /// <summary>
        /// 是否有销售数据
        /// </summary>
        /// <param name="model"></param>
        public bool IsExist(FMSAccoCheckResultModel model)
        {
            var result = FMAPIService.IsExistSalesSummary(model.EnterpriseID, model.Date);
            if (result.errcode != FMErrorEnum.OK)
            {
                return false;
            }
            return result.result_code == FMBusinessResultEnum.SUCCESS;
        }
        /// <summary>
        /// 检测销售汇总的销售转出数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExistForDiscount(FMSAccoCheckResultModel model)
        {
            var result = FMAPIService.IsExistSalesSummaryFordiscount(model.EnterpriseID, model.Date);
            if (result.errcode != FMErrorEnum.OK)
            {
                return false;
            }
            return result.result_code == FMBusinessResultEnum.SUCCESS;
        }
        /// <summary>
        /// 生成折扣分摊数据（计算折扣）
        /// </summary>
        /// <returns></returns>
        private bool GenerateDiscountData(FMSAccoCheckResultModel model)
        {
            try
            {
                var result = FMAPIService.ComputeSA_DiscountProvision(model, Convert.ToDateTime(model.BeginDate), Convert.ToDateTime(model.EndDate));
                if (result.Code != 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 计算销售净额
        /// </summary>
        /// <returns></returns>
        private bool CalcAmountNet(FMSAccoCheckResultModel model)
        {
            try
            {
                var result = FMAPIService.CalcAmountNet(model);
                if (result.errcode != FMErrorEnum.OK)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 生成销售汇总数据
        /// </summary>
        /// <returns></returns>
        private bool GenerateSalesSummaryData(FMSAccoCheckResultModel model)
        {
            try
            {
                if (!IsExist(model))
                {
                    var result = FMAPIService.PostSalesSummary(model);
                    if (result.errcode != FMErrorEnum.OK)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 取消销售汇总数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool CancelSalesSummaryData(FMSAccoCheckResultModel model)
        {
            var result = FMAPIService.DeleteSalesSummary(model);
            if (result.errcode != FMErrorEnum.OK)
            {
                return false;
            }
            return result.result_code == FMBusinessResultEnum.SUCCESS;
        }
        /// <summary>
        /// 取消折扣数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool CancelAmountNetAndDiscount(FMSAccoCheckResultModel model)
        {
            var result = FMAPIService.CancelAmountNetAndDiscount(model);
            if (result.errcode != FMErrorEnum.OK)
            {
                return false;
            }
            return result.result_code == FMBusinessResultEnum.SUCCESS;
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

                if (menuid == 98)
                {
                    //生成销售汇总表
                    var checkSaleValue = GenerateSalesSummaryData(model);
                    if (checkSaleValue)
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "生成销售汇总表成功", ResultState = true });
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "生成销售汇总表失败", ResultState = false });
                        return list;
                    }
                }
                if (menuid == 99)
                {
                    if (IsExist(model) && IsExistForDiscount(model))
                    {
                        //分摊数据
                        var checkDiscountVale = GenerateDiscountData(model) && CalcAmountNet(model);
                        if (checkDiscountVale)
                        {
                            list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "生成折扣汇总表成功", ResultState = true });
                        }
                        else
                        {
                            list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "生成折扣汇总表失败", ResultState = false });
                            return list;
                        }
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "生成折扣汇总表成功", ResultState = true });
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
                    var cancelDiscountValue = CancelAmountNetAndDiscount(model);
                    if (cancelDiscountValue)
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "删除折扣汇总表成功", ResultState = true });
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "删除折扣汇总表失败", ResultState = false });
                        return list;
                    }
                }
                if (menuid == 98)
                {
                    var calcelSaleValue = CancelSalesSummaryData(model);
                    if (calcelSaleValue)
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "删除销售汇总表成功", ResultState = true });
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "删除销售汇总表失败", ResultState = false });
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
        /// 数据核对
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override List<ResultModel> DataCheck(FMSAccoCheckResultModel model)
        {
            List<ResultModel> list = new List<ResultModel>();
            try
            {
                //获取该模块规则
                List<FM_AccoCheckRuleODataEntity> ruleList = FMAPIService.GetAaccocheckRuleList(AccoCheckTypeEnum.销售结账.GetValue().ToString(), model.EnterpriseID.ToString());
                //拆分主、从公式数据
                foreach (var item in ruleList)
                {
                    if (!string.IsNullOrEmpty(item.MasterDataSource) && item.CheckValue != AccocheckState.已审核.GetValue().ToString())
                    {
                        item.MasterRules = FMAPIService.ResolveFormulaSet(item.MasterDataSource, item.MasterFormula, item.MasterSecFormula);
                    }
                    if (!string.IsNullOrEmpty(item.FollowDataSource) && item.CheckValue != AccocheckState.已审核.GetValue().ToString())
                    {
                        item.FollowRules = FMAPIService.ResolveFormulaSet(item.FollowDataSource, item.FollowFormula, item.FollowSecFormula);
                    }
                }
                //获取需要用到的业务数据
                List<DataSourceEntity> dataSourceEntities = FMAPIService.GetDataSourceEntities(ruleList, model);
                if (dataSourceEntities.Where(s => s.DataSource == AccocheckDataSource.存货汇总表.GetValue().ToString()).Count() > 0)
                {
                    var stockdatas = FMAPIService.GetStockData(model);
                    var saleAbs = FMAPIService.GetSaleAbstractList(model, "201610140104402501");
                    var saleNames = saleAbs.Select(s => s.cDictName.Trim()).ToList();
                    stockdatas = stockdatas.Where(s => saleNames.Contains(s.SummaryType1Name)).ToList();
                    dataSourceEntities.Where(s => s.DataSource == AccocheckDataSource.存货汇总表.GetValue().ToString()).FirstOrDefault().EntityList = stockdatas.ToList<EntitySubClass>();
                }
                //销售结账启用：有未审核的单据允许结账 -- 集团级
                bool optionvalue = FMAPIService.OptionConfigValue(SystemOptionEnum.SAEnableReview_Group.GetStringValue(), model.EnterpriseID.ToString(), model.GroupID.ToString());
                for (int i = 0; i < ruleList.Count; i++)
                {
                    var item = ruleList[i];
                    if (item.CheckValue == AccocheckState.已审核.GetValue().ToString() && item.MasterDataSource != AccocheckDataSource.折扣计提.GetValue().ToString())
                    {
                        //if (!optionvalue)
                        //{
                            var result = FMAPIService.GetNotReviewNumbersExtend((long)AppIDEnum.销售单, model.EnterpriseID, model.Date);
                            if (result.data.Count > 0)
                            {
                                var menuIdList = result.data.Select(s => s.MenuId).ToList();
                                if (menuIdList.Contains(item.MasterDataSource))
                                {
                                    list.Add(new ResultModel() { Code = 1, CodeNo = (i + 1), Msg = @$"【{item.MasterDataSource}】存在未审核单据", ResultState = false });
                                }
                                else
                                {
                                    list.Add(new ResultModel() { Code = 0, CodeNo = (i + 1), Msg = @$"【{item.MasterDataSource}】全部已审核", ResultState = true });
                                }
                            }
                            else
                            {
                                list.Add(new ResultModel() { Code = 0, CodeNo = (i + 1), Msg = @$"【{item.MasterDataSource}】全部已审核", ResultState = true });
                            }
                        //}
                        //else
                        //{
                        //    list.Add(new ResultModel() { Code = 0, CodeNo = (i + 1), Msg = @$"【{item.MasterDataSource}】全部已审核", ResultState = true });
                        //}
                    }
                    if (item.CheckValue == AccocheckState.已审核.GetValue().ToString() && item.MasterDataSource == AccocheckDataSource.折扣计提.GetValue().ToString())
                    {
                        List<ApproveModel> result1 = FMAPIService.GetNotReviewNumbers(model.EnterpriseID, model.Date);
                        if (result1?.Count > 0)
                        {
                            list.Add(new ResultModel() { Code = 1, CodeNo = (i + 1), Msg = @$"【{item.MasterDataSource}】存在未审核单据", ResultState = false });
                        }
                        else
                        {
                            list.Add(new ResultModel() { Code = 0, CodeNo = (i + 1), Msg = @$"【{item.MasterDataSource}】全部已审核", ResultState = true });
                        }
                    }
                    if (item.CheckValue == AccocheckState.一致.GetValue().ToString())
                    {
                        //获取当前行需要用到的数据
                        string mastermsg = string.Empty;
                        decimal masterValue = FMAPIService.SetFormulaValue(item.MasterFormula, item.MasterSecFormula, dataSourceEntities, item.MasterRules, out mastermsg);
                        string followmsg = string.Empty;
                        decimal fillowValue = FMAPIService.SetFormulaValue(item.FollowFormula, item.FollowSecFormula, dataSourceEntities, item.FollowRules, out followmsg);
                        if (masterValue == fillowValue)
                        {
                            //数据源为1时是合并公式,不需要指定数据来源。公式上会体现
                            list.Add(new ResultModel() { Code = 0, CodeNo = (i + 1), Msg = @$"{(item.MasterDataSource != "1" ? "【" + item.MasterDataSource + "】" : "")}{mastermsg}与{(item.FollowDataSource != "1" ? "【" + item.FollowDataSource + "】" : "")}{followmsg}一致", ResultState = true });
                        }
                        else
                        {
                            list.Add(new ResultModel() { Code = 0, CodeNo = (i + 1), Msg = @$"{(item.MasterDataSource != "1" ? "【" + item.MasterDataSource + "】" : "")}{mastermsg}与{(item.FollowDataSource != "1" ? "【" + item.FollowDataSource + "】" : "")}{followmsg}不一致,差额{masterValue - fillowValue}", ResultState = false });
                        }
                    }
                }

                #region old code
                //int menuid = 1;
                //string checkmsg = string.Empty;
                //List<ApproveModel> result1 = FMAPIService.GetNotReviewNumbers(model.EnterpriseID, model.Date);
                //if (result1?.Count > 0) checkmsg += "【折扣计提】、";
                //bool optionvalue = FMAPIService.OptionConfigValue(SystemOptionEnum.SAEnableReview_Group.GetStringValue(), model.EnterpriseID.ToString(), model.GroupID.ToString());
                //if (!optionvalue)
                //{
                //    var result = FMAPIService.GetNotReviewNumbersExtend((long)AppIDEnum.销售单, model.EnterpriseID, model.Date);
                //    if (result.data.Count > 0)
                //    {
                //        var groupResult = result.data.GroupBy(s => s.MenuId).ToList();
                //        foreach (var item in groupResult)
                //        {
                //            checkmsg += "【" + item.Key + "】、";
                //        }
                //    }
                //}
                //if (!string.IsNullOrEmpty(checkmsg))
                //{
                //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = $"{checkmsg.TrimEnd('、')}存在未审核单据", ResultState = false });
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = $"【1610311318270000101】、【2001070948380000102】、【1907161719230000102】、【2109021808150000109】、【2007092157330000101】、【2104141012380000101】、【折扣计提】全部审核", ResultState = true });
                //}
                //menuid = 2;
                //var salesummarys = FMAPIService.GetSaleSummaryData(model);
                //var stockdatas = FMAPIService.GetStockData(model);
                //var saleAbs = FMAPIService.GetSaleAbstractList(model, "201610140104402501");
                //var saleNames = saleAbs.Select(s => s.cDictName.Trim()).ToList();
                //stockdatas = stockdatas.Where(s => saleNames.Contains(s.SummaryType1Name)).ToList();
                //decimal salesAmount = salesummarys.Sum(s => s.SalesAmount);
                //decimal accountQuantity = stockdatas.Sum(s => s.OutboundDeliveryQuantity);
                //if (salesAmount != accountQuantity)
                //{
                //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【销售汇总表】销售数量合计" + salesAmount + "与【存货汇总表】销售出库数量合计" + accountQuantity + "不一致，差额" + (salesAmount - accountQuantity), ResultState = false });
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【销售汇总表】销售数量合计" + salesAmount + "与【存货汇总表】销售出库数量合计" + accountQuantity + "一致", ResultState = true });
                //}
                #endregion
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
