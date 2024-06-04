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
    public class FixedAssetsMonthEndCheckout : MonthEndCheckout
    {

        FMAPIService FMAPIService;
        public FixedAssetsMonthEndCheckout(FMAPIService FMAPIService)
        {
            this.FMAPIService = FMAPIService;
        }

        /// <summary>
        /// 是否都进行了折旧计提
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool CheckDepreciationAccrued(FMSAccoCheckResultModel model)
        {
            var result = FMAPIService.CheckDepreciationAccrued(model);
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
                List<FM_AccoCheckRuleODataEntity> ruleList = FMAPIService.GetAaccocheckRuleList(AccoCheckTypeEnum.固定资产.GetValue().ToString(), model.EnterpriseID.ToString());
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
                    if (item.CheckValue == AccocheckState.已计提.GetValue().ToString())
                    {
                        bool checkvalue = CheckDepreciationAccrued(model);
                        if (!checkvalue)
                        {
                            list.Add(new ResultModel() { Code = 1, CodeNo = (i + 1), Msg = "当期折旧未计提", ResultState = false });
                        }
                        else
                        {
                            list.Add(new ResultModel() { Code = 0, CodeNo = (i + 1), Msg = "当期折旧已计提", ResultState = true });
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
                //bool checkvalue = CheckDepreciationAccrued(model);
                //if (!checkvalue)
                //{
                //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "当期折旧未计提", ResultState = false });
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "当期折旧已计提", ResultState = true });
                //}
                //menuid = 2;
                //var balancesrank = FMAPIService.RetSubjectBalance(model, 1);
                //var depreciation = FMAPIService.AssetsCardDepreciationSummary(model);
                //decimal balanceAmount = balancesrank.Where(s => s.AccoSubjectCode.StartsWith("1602")).Sum(s=>s.Credit);
                //decimal depreciationAmount = depreciation.Sum(s => s.DepreciationMonthAmount);
                //if (balanceAmount != depreciationAmount)
                //{
                //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【科目余额表】累计折旧本期贷方合计"+ balanceAmount.ToString("#,##0.00 ") + "与【折旧汇总表】本期折旧合计"+ depreciationAmount.ToString("#,##0.00 ") + "不一致，差额"+(balanceAmount- depreciationAmount).ToString("#,##0.00 ") + "元", ResultState = false });
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【科目余额表】累计折旧本期贷方合计"+ balanceAmount.ToString("#,##0.00 ") + "与【折旧汇总表】本期折旧合计"+ depreciationAmount.ToString("#,##0.00 ") + "一致", ResultState = true });
                //}
                //menuid = 3;
                ////var assetsCardsAll = FMAPIService.AssetsCardInfoAllStatus(model);
                //var assetsCards = FMAPIService.AssetsCardInfo(model);
                //var depreciationList = balancesrank.Where(s => s.AccoSubjectCode.StartsWith("1602"));
                //var depreciationBanlancesAmount = depreciationList.Sum(s => s.Show_LastCredit) - depreciationList.Sum(s => s.Show_LastDebit);
                //decimal depreciationAccumulated = assetsCards.Sum(s => s.DepreciationAccumulated);
                //if (depreciationBanlancesAmount != depreciationAccumulated)
                //{
                //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【科目余额表】累计折旧期末余额合计" + depreciationBanlancesAmount.ToString("#,##0.00 ") + "与【固定资产卡片报表】累计折旧合计" + depreciationAccumulated.ToString("#,##0.00 ") + "不一致，差额" + (depreciationBanlancesAmount - depreciationAccumulated).ToString("#,##0.00 ") + "元", ResultState = false });
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【科目余额表】累计折旧期末余额合计" + depreciationBanlancesAmount.ToString("#,##0.00 ") + "与【固定资产卡片报表】累计折旧合计" + depreciationAccumulated.ToString("#,##0.00 ") + "一致", ResultState = true });
                //}
                ////decimal depreciationAccumulated = depreciation.Sum(s => s.DepreciationAccumulated);
                ////if (depreciationBanlancesAmount != depreciationAccumulated)
                ////{
                ////    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【科目余额表】累计折旧期末余额合计" + depreciationBanlancesAmount.ToString("#,##0.00 ") + "与【折旧汇总表】累计折旧合计" + depreciationAccumulated.ToString("#,##0.00 ") + "不一致，差额" + (depreciationBanlancesAmount - depreciationAccumulated).ToString("#,##0.00 ") + "元", ResultState = false });
                ////}
                ////else
                ////{
                ////    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【科目余额表】累计折旧期末余额合计" + depreciationBanlancesAmount.ToString("#,##0.00 ") + "与【折旧汇总表】累计折旧合计" + depreciationAccumulated.ToString("#,##0.00 ") + "一致", ResultState = true });
                ////}
                //menuid = 4;
                ////var assetsCards = FMAPIService.AssetsCardInfo(model);
                //decimal balanceAmount1 = balancesrank.Where(s => s.AccoSubjectCode.StartsWith("1601")).Sum(s => s.Show_LastDebit)- balancesrank.Where(s => s.AccoSubjectCode.StartsWith("1601")).Sum(s => s.Show_LastCredit);
                //decimal originalValue = assetsCards.Sum(s => s.OriginalValue);
                //if (balanceAmount1 != originalValue)
                //{
                //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【固定资产卡片报表】资产原值合计"+ originalValue.ToString("#,##0.00 ") + "与【科目余额表】固定资产期末金额合计"+ balanceAmount1.ToString("#,##0.00 ") + "不一致，差额"+(originalValue- balanceAmount1).ToString("#,##0.00 ") + "元", ResultState = false });
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【固定资产卡片报表】资产原值合计" + originalValue.ToString("#,##0.00 ") + "与【科目余额表】固定资产期末金额合计" + balanceAmount1.ToString("#,##0.00 ") + "一致", ResultState = true });
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
