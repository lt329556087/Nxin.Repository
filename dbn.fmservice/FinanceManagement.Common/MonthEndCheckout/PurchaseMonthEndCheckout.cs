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
    public class PurchaseMonthEndCheckout : MonthEndCheckout
    {

        FMAPIService FMAPIService;
        public PurchaseMonthEndCheckout(FMAPIService FMAPIService)
        {
            this.FMAPIService = FMAPIService;
        }
        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool IsExist(FMSAccoCheckResultModel model)
        {
            var result = FMAPIService.ExistPurchaseSummary(model);
            if (result.errcode != FMErrorEnum.OK)
            {
                return false;
            }
            return result.result_code == FMBusinessResultEnum.SUCCESS;
        }

        /// <summary>
        /// 生成采购汇总数据
        /// </summary>
        /// <returns></returns>
        private bool GeneratePurchaseSummaryData(FMSAccoCheckResultModel model)
        {
            try
            {
                if (!IsExist(model))
                {
                    var result = FMAPIService.PostPurchaseSummary(model);
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
        /// 取消采购汇总数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool CancelPurchaseSummaryData(FMSAccoCheckResultModel model)
        {
            var result = FMAPIService.DeletePurchaseSummary(model);
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

                if (menuid == 99)
                {
                    //生成采购汇总表
                    var result = GeneratePurchaseSummaryData(model);
                    if (result)
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "生成采购汇总表成功", ResultState = true });
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "生成采购汇总表失败", ResultState = false });
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
                    var result = CancelPurchaseSummaryData(model);
                    if (result)
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "删除采购汇总表成功", ResultState = true });
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "删除采购汇总表失败", ResultState = false });
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
                List<FM_AccoCheckRuleODataEntity> ruleList = FMAPIService.GetAaccocheckRuleList(AccoCheckTypeEnum.采购结账.GetValue().ToString(), model.EnterpriseID.ToString());
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
                if (dataSourceEntities.Where(s=>s.DataSource== AccocheckDataSource.存货汇总表.GetValue().ToString()).Count()>0)
                {
                    var stockdatas = FMAPIService.GetStockData(model);
                    var purchaseAbs = FMAPIService.GetAbstractList(model, "201610140104402301");
                    var purchaseNames = purchaseAbs.Select(s => s.cDictName.Trim()).ToList();
                    stockdatas = stockdatas.Where(s => purchaseNames.Contains(s.SummaryType1Name)).ToList();
                    dataSourceEntities.Where(s => s.DataSource == AccocheckDataSource.存货汇总表.GetValue().ToString()).FirstOrDefault().EntityList = stockdatas.ToList<EntitySubClass>();
                }

                //采购结账启用：有未审核的单据允许结账 -- 集团级
                bool optionvalue = FMAPIService.OptionConfigValue(SystemOptionEnum.PMEnableReview_Group.GetStringValue(), model.EnterpriseID.ToString(), model.GroupID.ToString());
                for (int i = 0; i < ruleList.Count; i++)
                {
                    var item = ruleList[i];
                    if (item.CheckValue == AccocheckState.已审核.GetValue().ToString())
                    {
                        //if (!optionvalue)
                        //{
                            var result = FMAPIService.GetNotReviewNumbersExtend((long)AppIDEnum.采购单, model.EnterpriseID, model.Date);
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
                //bool optionvalue = FMAPIService.OptionConfigValue(SystemOptionEnum.PMEnableReview_Group.GetStringValue(), model.EnterpriseID.ToString(), model.GroupID.ToString());
                //if (!optionvalue)
                //{
                //    var result = FMAPIService.GetNotReviewNumbersExtend((long)AppIDEnum.采购单, model.EnterpriseID, model.Date);
                //    if (result.data.Count > 0)
                //    {
                //        var groupResult = result.data.GroupBy(s => s.MenuId).ToList();
                //        foreach (var item in groupResult)
                //        {
                //            checkmsg += "【" + item.Key + "】、";
                //        }
                //        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = $"{checkmsg.TrimEnd('、')}存在未审核单据", ResultState = false });
                //    }
                //    else
                //    {
                //        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = $"【1611051506540000101】、【2108111356400000109】、【2207211859410000109】、【2104141011350000101】、【2007092156450000101】全部已审核", ResultState = true });
                //    }
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = $"【1611051506540000101】、【2108111356400000109】、【2207211859410000109】、【2104141011350000101】、【2007092156450000101】全部已审核", ResultState = true });
                //}
                //menuid = 2;
                //var purcheasesummarys = FMAPIService.GetPurchaseSummaryDataList(model);
                //var stockdatas = FMAPIService.GetStockData(model);
                //var purchaseAbs = FMAPIService.GetAbstractList(model, "201610140104402301");
                //var purchaseNames= purchaseAbs.Select(s => s.cDictName.Trim()).ToList();
                //stockdatas=stockdatas.Where(s => purchaseNames.Contains(s.SummaryType1Name)).ToList();
                //decimal purchaseAmount = purcheasesummarys.Sum(s => s.PurchaseQuantity);
                //decimal accountQuantity = stockdatas.Sum(s => s.InboundDeliveryQuantity);
                //if (purchaseAmount != accountQuantity)
                //{
                //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【采购汇总表】采购数量合计" + purchaseAmount + "与【存货汇总表】采购入库数量合计" + accountQuantity + "不一致，差额" + (purchaseAmount - accountQuantity), ResultState = false });
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【采购汇总表】采购数量合计" + purchaseAmount + "与【存货汇总表】采购入库数量合计" + accountQuantity + "一致", ResultState = true });
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
