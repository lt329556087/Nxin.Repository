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
    public class SuppliesMonthEndCheckout : MonthEndCheckout
    {
        /// <summary>
        /// 物资结账
        /// </summary>
        FMAPIService FMAPIService;
        public SuppliesMonthEndCheckout(FMAPIService FMAPIService)
        {
            this.FMAPIService = FMAPIService;
        }
        /// <summary>
        /// 核对数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool CheckSupplies(FMSAccoCheckResultModel model)
        {
            model.OptionOpen = FMAPIService.OptionConfigValueString("20190319132324", model.EnterpriseID.ToString(), model.GroupID.ToString());//全月加权平均法
            model.OptionOpenEndMonth = FMAPIService.OptionConfigValueString("20181213092354", model.EnterpriseID.ToString(), model.GroupID.ToString());//启用物品结账
            var result = FMAPIService.CheckSupplies(model);
            if (result.errcode != FMErrorEnum.OK)
            {
                return false;
            }
            return result.result_code == FMBusinessResultEnum.SUCCESS;
        }

        /// <summary>
        /// 取消物品汇总数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool CancelSuppliesSummaryData(FMSAccoCheckResultModel model)
        {
            var result = FMAPIService.DeleteSuppliesSummary(model);
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
                    bool result = CheckSupplies(model);
                    if (result)
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "生成物品汇总表成功", ResultState = true });
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "生成物品汇总表失败", ResultState = false });
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
                    var result = CancelSuppliesSummaryData(model);
                    if (result)
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "取消物品汇总表成功", ResultState = true });
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "取消物品汇总表失败", ResultState = false });
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
                List<FM_AccoCheckRuleODataEntity> ruleList = FMAPIService.GetAaccocheckRuleList(AccoCheckTypeEnum.物品结账.GetValue().ToString(), model.EnterpriseID.ToString());
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

                //int menuid = 1;
                //var balancesrank1 = FMAPIService.RetSubjectBalance(model, 1);
                //var cailiaobalance = balancesrank1.Where(s => s.AccoSubjectCode.StartsWith("1411"));
                //var supplies = FMAPIService.GetSuppliesReport(model);
                //decimal cailiaoAmount = (decimal)cailiaobalance?.Sum(s => s.Show_LastDebit);
                //decimal qmAmount = 0;
                //if (supplies.Count > 0)
                //{
                //    qmAmount = (decimal)supplies?.Where(s => s.dDate == "合计")?.FirstOrDefault()?.qcAmount;
                //}
                //if (Math.Round( cailiaoAmount,2) != Math.Round(qmAmount,2))
                //{
                //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【物品明细表】期末金额合计" + Math.Round(qmAmount, 2).ToString("#,##0.00 ") + "元与【科目余额表】周转材料期末金额合计" + Math.Round(cailiaoAmount, 2).ToString("#,##0.00 ") + "元不一致，差额" + (Math.Round(qmAmount, 2) - Math.Round(cailiaoAmount, 2)).ToString("#,##0.00 ") + "元", ResultState = false });
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【物品明细表】期末金额合计" + Math.Round(qmAmount, 2).ToString("#,##0.00 ") + "元与【科目余额表】周转材料期末金额合计" + Math.Round(cailiaoAmount, 2).ToString("#,##0.00 ") + "元一致", ResultState = true });
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
