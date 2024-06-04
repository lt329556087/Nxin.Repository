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
    public class CostMonthEndCheckout : MonthEndCheckout
    {

        FMAPIService FMAPIService;
        public CostMonthEndCheckout(FMAPIService FMAPIService)
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
            try
            {
                return new List<ResultModel>();
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
            try
            {
                return new List<ResultModel>();
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

                //获取该模块规则
                List<FM_AccoCheckRuleODataEntity> ruleList = FMAPIService.GetAaccocheckRuleList(AccoCheckTypeEnum.成本管理.GetValue().ToString(), model.EnterpriseID.ToString());
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
                for (int i = 0; i < ruleList.Count; i++)
                {
                    var item = ruleList[i];
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
                //var costdata = FMAPIService.GetCostDataList(model, "ente.EnterpriseName");
                //int menuid = 1;
                //var purchaseList = FMAPIService.GetPurchaseSummaryDataList(model);

                ////List<JXCModel> freughtList = new List<JXCModel>();
                ////bool optionvalue = FMAPIService.OptionConfigValue("20180606103640", model.EnterpriseID.ToString(), model.GroupID.ToString());
                ////if (optionvalue)
                ////{
                ////    freughtList.AddRange( FMAPIService.GetWuziList(model));
                ////}
                ////freughtList.AddRange(FMAPIService.GetYangHuList(model));
                ////freughtList.AddRange(FMAPIService.GetCaiGouList(model));
                //var freughtList = FMAPIService.getCarriageAmount(model);
                ////var freughtList = FMAPIService.GetGetMyFreightSummaryReport(model);
                //decimal cgAmount = (decimal)costdata?.Sum(s => s.cgAmount);
                //decimal purAmount =(decimal) purchaseList?.Sum(s => s.AmountWithoutTax);
                //decimal freughtAmount = (decimal)freughtList?.Sum(s => s.Amount);
                //decimal sumAmount = purAmount + freughtAmount;
                //if (cgAmount!= sumAmount)
                //{
                //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【成本汇总表】采购成本合计"+ cgAmount.ToString("#,##0.00 ")  + "与【采购汇总表】不含税采购金额"+ purAmount.ToString("#,##0.00 ")  +  "+【运费汇总表】实际运费" + freughtAmount.ToString("#,##0.00 ") + "合计" + sumAmount.ToString("#,##0.00 ") + "不一致，差额" + (cgAmount- purAmount- freughtAmount).ToString("#,##0.00 ") + "元", ResultState = false });
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【成本汇总表】采购成本合计" + cgAmount.ToString("#,##0.00 ") + "与【采购汇总表】不含税采购金额" + purAmount.ToString("#,##0.00 ")  + "+【运费汇总表】实际运费" + freughtAmount.ToString("#,##0.00 ") + "合计" + sumAmount.ToString("#,##0.00 ") + "一致", ResultState = true });
                //}
                //menuid = 2;
                //var saleList = FMAPIService.GetSaleSummaryData(model);
                //decimal xsAmount = (decimal)costdata?.Sum(s => s.xsAmount);
                //decimal salesCost = (decimal)saleList?.Sum(s => s.SalesCost);
                //if (xsAmount != salesCost)
                //{
                //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【成本汇总表】销售成本合计"+ xsAmount.ToString("#,##0.00 ")  + "与【销售汇总表】销售成本合计"+ salesCost.ToString("#,##0.00 ")  + "不一致，差额"+(xsAmount- salesCost).ToString("#,##0.00 ") + "元", ResultState = false });
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【成本汇总表】销售成本合计" + xsAmount.ToString("#,##0.00 ")  + "与【销售汇总表】销售成本合计" + salesCost.ToString("#,##0.00 ")  + "一致", ResultState = true });
                //}
                ////menuid = 3;
                ////var balancesrank = FMAPIService.RetSubjectBalance(model, 1);
                ////decimal scAmount = (decimal)costdata?.Sum(s => s.scAmount);
                ////decimal lastDebit = (decimal)balancesrank?.Where(s => s.AccoSubjectCode.StartsWith("5001")).Sum(s => s.Debit);
                ////if (scAmount != lastDebit)
                ////{
                ////    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【成本汇总表】生产成本合计"+ scAmount.ToString("#,##0.00 ")  + "与【科目余额表】生产成本本期借方合计"+ lastDebit.ToString("#,##0.00 ")  + "不一致，差额"+(scAmount- lastDebit).ToString("#,##0.00 ") + "元", ResultState = false });
                ////}
                ////else
                ////{
                ////    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【成本汇总表】生产成本合计" + scAmount.ToString("#,##0.00 ")  + "与【科目余额表】生产成本本期借方合计" + lastDebit.ToString("#,##0.00 ")  + "一致", ResultState = true });
                ////}
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
