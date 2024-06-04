using Architecture.Common.HttpClientUtil;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;

namespace FinanceManagement.Common.MonthEndCheckout
{
    public class FinanceMonthEndCheckout : MonthEndCheckout
    {
        /// <summary>
        /// 财务结账
        /// </summary>
        FMAPIService FMAPIService;
        public FinanceMonthEndCheckout(FMAPIService FMAPIService)
        {
            this.FMAPIService = FMAPIService;
        }
        #region 科目余额中间表

        /// <summary>
        /// 判断科目余额中间表数据是否存在
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool IsExistMidSubjectBalance(FMSAccoCheckResultModel model)
        {
            var result = FMAPIService.IsExistMidSubjectBalance(model);
            if (result.errcode != FMErrorEnum.OK)
            {
                return false;
            }
            return result.result_code == FMBusinessResultEnum.SUCCESS;
        }
        /// <summary>
        /// 生成科目余额中间表数据
        /// </summary>
        /// <returns></returns>
        private bool GenerateMidSubjectBalanceData(FMSAccoCheckResultModel model)
        {
            try
            {
                if (!IsExistMidSubjectBalance(model))
                {
                    var result = FMAPIService.PostMidSubjectBalance(model);
                    if (result.errcode != FMErrorEnum.OK)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 取消科目余额中间表数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool CancelMidSubjectBalance(FMSAccoCheckResultModel model)
        {
            var result = FMAPIService.DeleteMidSubjectBalance(model);
            if (result.errcode != FMErrorEnum.OK)
            {
                return false;
            }
            return result.result_code == FMBusinessResultEnum.SUCCESS;
        }
        #endregion
        #region 往来余额中间表
        /// <summary>
        /// 判断往来余额中间表数据是否存在
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool IsExistMidDealingBalance(FMSAccoCheckResultModel model)
        {
            var result = FMAPIService.IsExistMidDealingBalance(model);
            if (result.errcode != FMErrorEnum.OK)
            {
                return false;
            }
            return result.result_code == FMBusinessResultEnum.SUCCESS;
        }
        /// <summary>
        /// 生成往来余额中间表数据
        /// </summary>
        /// <returns></returns>
        private bool GenerateMidDealingBalanceData(FMSAccoCheckResultModel model)
        {
            try
            {
                if (!IsExistMidDealingBalance(model))
                {
                    var result = FMAPIService.PostMidDealingBalance(model);
                    if (result.errcode != FMErrorEnum.OK)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 取消往来余额中间表数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool CancelMidDealingBalance(FMSAccoCheckResultModel model)
        {
            var result = FMAPIService.DeleteMidDealingBalance(model);
            if (result.errcode != FMErrorEnum.OK)
            {
                return false;
            }
            return result.result_code == FMBusinessResultEnum.SUCCESS;
        }

        #endregion

        #region 应收汇总表

        /// <summary>
        /// 判断应收汇总数据是否存在
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool IsExistReceivablesSummary(FMSAccoCheckResultModel model)
        {
            var result = FMAPIService.IsExistReceivablesSummary(model);
            if (result.errcode != FMErrorEnum.OK)
            {
                return false;
            }
            return result.result_code == FMBusinessResultEnum.SUCCESS;
        }
        /// <summary>
        /// 生成应收汇总数据
        /// </summary>
        /// <returns></returns>
        private bool GenerateReceivablesSummaryData(FMSAccoCheckResultModel model)
        {
            try
            {
                if (!IsExistReceivablesSummary(model))
                {
                    var result = FMAPIService.PostReceivablesSummary(model);
                    if (result.errcode != FMErrorEnum.OK)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 取消应收汇总数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool CancelReceivablesSummary(FMSAccoCheckResultModel model)
        {
            var result = FMAPIService.DeleteReceivablesSummary(model);
            if (result.errcode != FMErrorEnum.OK)
            {
                return false;
            }
            return result.result_code == FMBusinessResultEnum.SUCCESS;
        }

        #endregion

        #region 资金

        /// <summary>
        /// 生成资金汇总表
        /// </summary>
        /// <returns></returns>
        private bool GenerateFundsBalance(FMSAccoCheckResultModel model)
        {
            try
            {
                var result = FMAPIService.GenerateFundsBalance(model.EnterpriseID, model.Date);
                if (result.errcode != FMErrorEnum.OK)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 取消资金汇总表
        /// </summary>
        /// <returns></returns>
        private bool CancelFundsBalance(FMSAccoCheckResultModel model)
        {
            try
            {
                var result = FMAPIService.CancelFundsBalance(model.EnterpriseID, model.Date);
                if (result.errcode != FMErrorEnum.OK)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 资产表是否审核
        /// </summary>
        /// <returns></returns>
        private bool IsCheckToBalanceSheet(FMSAccoCheckResultModel model)
        {
            try
            {
                var result = FMAPIService.GetDataToReport(model);
                if (result.errcode != FMErrorEnum.OK)
                {
                    return false;
                }

                if (result.data != null && result.data.Count > 0)
                {
                    var obj = result.data.FirstOrDefault(m => m.Level == 2);
                    if (obj != null)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

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

                if (menuid == 96)
                {
                    bool result1 = GenerateMidSubjectBalanceData(model);
                    if (result1)
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "生成科目余额表成功", ResultState = true });
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "生成科目余额表失败", ResultState = false });
                        return list;
                    }
                }
                if (menuid == 97)
                {
                    bool result2 = GenerateMidDealingBalanceData(model);
                    if (result2)
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "生成往来余额表成功", ResultState = true });
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "生成往来余额表失败", ResultState = false });
                        return list;
                    }
                }
                if (menuid == 98)
                {
                    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "生成应收汇总表成功", ResultState = true });
                    //bool result3 = GenerateReceivablesSummaryData(model);
                    //if (result3)
                    //{
                    //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "生成应收汇总表成功", ResultState = true });
                    //}
                    //else
                    //{
                    //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "生成应收汇总表失败", ResultState = false });
                    //    return list;
                    //}
                }
                if (menuid == 99)
                {
                    bool result4 = GenerateFundsBalance(model);
                    if (result4)
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "生成资金汇总表成功", ResultState = true });
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "生成资金汇总表失败", ResultState = false });
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
                    if (IsCheckToBalanceSheet(model))
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "取消会计结账-资产负债表已审核，取消结账先取消报表审核！", ResultState = false });
                        return list;
                    }
                    var result1 = CancelFundsBalance(model);
                    if (result1)
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "取消资金汇总表成功", ResultState = true });
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "取消资金汇总表失败", ResultState = false });
                        return list;
                    }
                }
                if (menuid == 98)
                {
                    var result2 = CancelReceivablesSummary(model);
                    if (result2)
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "取消应收汇总表成功", ResultState = true });
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "取消应收汇总表失败", ResultState = false });
                        return list;
                    }
                }

                if (menuid == 97)
                {
                    var result3 = CancelMidDealingBalance(model);
                    if (result3)
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "取消往来余额汇总表成功", ResultState = true });
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "取消往来余额汇总表失败", ResultState = false });
                        return list;
                    }
                }
                if (menuid == 96)
                {
                    var result4 = CancelMidSubjectBalance(model);
                    if (result4)
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "取消科目余额汇总表成功", ResultState = true });
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "取消科目余额汇总表失败", ResultState = false });
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
                List<FM_AccoCheckRuleODataEntity> ruleList = FMAPIService.GetAaccocheckRuleList(AccoCheckTypeEnum.会计结账.GetValue().ToString(), model.EnterpriseID.ToString());
                //拆分主、从公式数据
                foreach (var item in ruleList)
                {
                    if (!string.IsNullOrEmpty(item.MasterDataSource) && item.CheckValue != AccocheckState.已审核.GetValue().ToString() && item.CheckValue != AccocheckState.无余额.GetValue().ToString())
                    {
                        item.MasterRules = FMAPIService.ResolveFormulaSet(item.MasterDataSource, item.MasterFormula, item.MasterSecFormula);
                    }
                    if (!string.IsNullOrEmpty(item.FollowDataSource) && item.CheckValue != AccocheckState.已审核.GetValue().ToString() && item.CheckValue != AccocheckState.无余额.GetValue().ToString())
                    {
                        item.FollowRules = FMAPIService.ResolveFormulaSet(item.FollowDataSource, item.FollowFormula, item.FollowSecFormula);
                    }
                }
                //获取需要用到的业务数据
                List<DataSourceEntity> dataSourceEntities = FMAPIService.GetDataSourceEntities(ruleList, model);
                for (int i = 0; i < ruleList.Count; i++)
                {
                    var item = ruleList[i];
                    if (item.CheckValue == AccocheckState.已审核.GetValue().ToString() && item.MasterDataSource == AccocheckDataSource.会计凭证.GetValue().ToString())
                    {
                        //bool optionvalue = FMAPIService.OptionConfigValue(SystemOptionEnum.FMEnableReview_Group.GetStringValue(), model.EnterpriseID.ToString(), model.GroupID.ToString());
                        //if (!optionvalue)
                        //{
                            var result = FMAPIService.GetNotReviewNumbers((long)AppIDEnum.会计凭证, model.EnterpriseID, model.Date);
                            if (result?.data.Count > 0)
                            {
                                list.Add(new ResultModel() { Code = 1, CodeNo = (i + 1), Msg = @$"【{item.MasterDataSource}】存在未审核凭证", ResultState = false });
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
                    if (item.CheckValue == AccocheckState.无余额.GetValue().ToString())
                    {
                        //获取科目余额数据
                        var balances = FMAPIService.RetSubjectBalance(model);
                        string mastermsg = string.Empty;
                        List<RuleEntity> masterRules = FMAPIService.ResolveFormulaSet(item.MasterDataSource, item.MasterFormula, item.MasterSecFormula);
                        decimal masterValue = FMAPIService.SetFormulaValue(item.MasterFormula, item.MasterSecFormula, new List<DataSourceEntity>() { new DataSourceEntity("1612121627090000101", balances.ToList<EntitySubClass>()) }, masterRules, out mastermsg);
                        if (masterValue != 0)
                        {
                            list.Add(new ResultModel() { Code = 0, CodeNo = (i + 1), Msg = @$"【{item.MasterDataSource}】{mastermsg}", ResultState = false });
                        }
                        else
                        {
                            list.Add(new ResultModel() { Code = 0, CodeNo = (i + 1), Msg = @$"【{item.MasterDataSource}】{mastermsg}", ResultState = true });
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
                //bool optionvalue = FMAPIService.OptionConfigValue(SystemOptionEnum.FMEnableReview_Group.GetStringValue(), model.EnterpriseID.ToString(), model.GroupID.ToString());
                //if (!optionvalue)
                //{
                //    var result = FMAPIService.GetNotReviewNumbers((long)AppIDEnum.会计凭证, model.EnterpriseID, model.Date);
                //    if (result?.data.Count > 0)
                //    {
                //        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【会计凭证】存在未审核凭证", ResultState = false });
                //    }
                //    else
                //    {
                //        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【会计凭证】全部已审核", ResultState = true });
                //    }
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【会计凭证】全部已审核", ResultState = true });
                //}
                ////获取科目余额数据
                //var balances = FMAPIService.RetSubjectBalance(model);
                //var balancesrank1 = FMAPIService.RetSubjectBalance(model, 1);
                //menuid = 2;
                //bool isbreak = true;
                //var sunyisubject = balances.Where(s => s.AccoSubjectType == "232").ToList();
                //foreach (var item in sunyisubject)
                //{
                //    if (item.Show_LastDebit != 0 || item.Show_LastCredit != 0)
                //    {
                //        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【科目余额表】损益类科目期末有余额", ResultState = false });
                //        isbreak = false;
                //        break;
                //    }
                //}
                //if (isbreak)
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【科目余额表】损益类科目期末无余额", ResultState = true });
                //}
                //menuid = 3;
                //bool isbreak1 = true;
                //var costsubject = balances.Where(s => s.AccoSubjectType == "231").ToList();
                //costsubject.RemoveAll(s => s.AccoSubjectFullName.Contains("待产母羊费用") || s.AccoSubjectFullName.Contains("待产母猪费用"));
                //foreach (var item in costsubject)
                //{
                //    //余额方向  
                //    if (!item.IsLorR)
                //    {
                //        //期末借方 true借
                //        if (item.Show_LastDebit != 0)
                //        {
                //            list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【科目余额表】成本类科目期末有余额", ResultState = false });
                //            isbreak1 = false;
                //        }
                //        break;
                //    }
                //    else
                //    {
                //        //期末贷方 false贷
                //        if (item.Show_LastCredit != 0)
                //        {
                //            list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【科目余额表】成本类科目期末有余额", ResultState = false });
                //            isbreak1 = false;
                //        }
                //        break;
                //    }
                //}
                //if (isbreak1)
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【科目余额表】成本类科目期末无余额", ResultState = true });
                //}
                //menuid = 4;
                //bool optionvalue4 = FMAPIService.OptionConfigValue("20220526180717", model.EnterpriseID.ToString(), model.GroupID.ToString());
                ////获取资金汇总表数据
                //List<SubjectBalance> havebalance = new List<SubjectBalance>();
                //string subjectName = string.Empty;
                //var enddingAmount = FMAPIService.FundsSummary(model).Sum(s => s.EnddingAmount);
                //if (optionvalue4)
                //{
                //    havebalance = balancesrank1.Where(s => s.AccoSubjectCode.StartsWith("1001") || s.AccoSubjectCode.StartsWith("1002")).ToList();
                //    subjectName = "库存现金+银行存款";
                //}
                //else
                //{
                //    havebalance = balancesrank1.Where(s => s.AccoSubjectCode.StartsWith("1001") || s.AccoSubjectCode.StartsWith("1002") || s.AccoSubjectCode.StartsWith("1012")).ToList();
                //    subjectName = "库存现金+银行存款+其他货币资金";
                //}
                //decimal havebalanceAmount = havebalance.Sum(s => s.Show_LastDebit) - havebalance.Sum(s => s.Show_LastCredit);
                //if (enddingAmount != havebalanceAmount)
                //{
                //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【资金汇总表】期末金额合计" + enddingAmount.ToString("#,##0.00 ") + "元与【科目余额表】(" + subjectName + ")期末金额合计" + havebalanceAmount.ToString("#,##0.00 ") + "元不一致，差额" + (enddingAmount - havebalanceAmount).ToString("#,##0.00 ") + "元", ResultState = false });
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【资金汇总表】期末金额合计" + enddingAmount.ToString("#,##0.00 ") + "元与【科目余额表】(" + subjectName + ")期末金额合计" + havebalanceAmount.ToString("#,##0.00 ") + "元一致", ResultState = true });
                //}
                //menuid = 5;
                ////获取往来余额
                //var settleAmount1 = FMAPIService.SettleReceiptBalance(model).Where(s => s.AccoSubjectCode.StartsWith("2202")).Sum(s => s.LastBalance);
                //var receiptAmount = FMAPIService.PayableSummary(model).Sum(s => s.ReceiptAmount);

                //var yingfuList = balancesrank1.Where(s => s.AccoSubjectCode.StartsWith("2202"));
                //var yingfuAmount = yingfuList.Sum(s => s.Show_LastCredit) - yingfuList.Sum(s => s.Show_LastDebit);
                //if (settleAmount1 == receiptAmount && receiptAmount == yingfuAmount)
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【往来余额表】应付账款期末金额合计" + settleAmount1.ToString("#,##0.00 ") + "、【科目余额表】应付账款期末金额合计" + yingfuAmount.ToString("#,##0.00 ") + "、【应付账款汇总表】期末金额合计" + receiptAmount.ToString("#,##0.00 ") + "三者一致", ResultState = true });
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【往来余额表】应付账款期末金额合计" + settleAmount1.ToString("#,##0.00 ") + "、【科目余额表】应付账款期末金额合计" + yingfuAmount.ToString("#,##0.00 ") + "、【应付账款汇总表】期末金额合计" + receiptAmount.ToString("#,##0.00 ") + "三者不一致", ResultState = false });
                //}
                //menuid = 6;
                ////获取往来余额
                //var settleAmount2 = FMAPIService.SettleReceiptBalance(model).Where(s => s.AccoSubjectCode.StartsWith("1122")).Sum(s => s.LastBalance);
                //var receivableAccount = FMAPIService.ReceivableSummary(model).Sum(s => s.NetReceivableAccount);
                //var yingshouList = balancesrank1.Where(s => s.AccoSubjectCode.StartsWith("1122"));
                //var yingshouAmount = yingshouList.Sum(s => s.Show_LastDebit) - yingshouList.Sum(s => s.Show_LastCredit);
                //if (settleAmount2 == receivableAccount && receivableAccount == yingshouAmount)
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【往来余额表】应收账款期末金额合计" + settleAmount2.ToString("#,##0.00 ") + "、【科目余额表】应收账款期末金额合计" + yingshouAmount.ToString("#,##0.00 ") + "、【应收账款汇总表】应收净额合计" + receivableAccount.ToString("#,##0.00 ") + "三者一致", ResultState = true });
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【往来余额表】应收账款期末金额合计" + settleAmount2.ToString("#,##0.00 ") + "、【科目余额表】应收账款期末金额合计" + yingshouAmount.ToString("#,##0.00 ") + "、【应收账款汇总表】应收净额合计" + receivableAccount.ToString("#,##0.00 ") + "三者不一致", ResultState = false });
                //}
                //menuid = 7;
                //bool optionvalue5 = FMAPIService.OptionConfigValue("20221222162521", model.EnterpriseID.ToString(), model.GroupID.ToString());
                //bool optionvalue5_2 = FMAPIService.OptionConfigValue("20230522105906", model.EnterpriseID.ToString(), model.GroupID.ToString());
                ////销售汇总表
                //var salesummarys = FMAPIService.GetSaleSummaryData(model);
                ////折旧汇总表
                //var discountsummarys = FMAPIService.GetMyDiscountSummaryReport(model);
                //var mainBusiness = balancesrank1.Where(s => s.AccoSubjectCode.StartsWith("6001"));
                //List<SubjectBalance> mainBusiness2 = new List<SubjectBalance>();
                //string msg2 = string.Empty;
                //if (optionvalue5)
                //{
                //    bool optionvalue5_1 = FMAPIService.OptionConfigValue("20230506101110", model.EnterpriseID.ToString(), model.GroupID.ToString());
                //    if (optionvalue5_1)
                //    {
                //        mainBusiness2 = balancesrank1.Where(s => s.AccoSubjectCode.StartsWith("605105") || s.AccoSubjectCode.StartsWith("605199")).ToList();
                //        msg2 = "+其他业务收入/促销品收入+其他业务收入/其他";
                //    }
                //    else
                //    {
                //        mainBusiness2 = balancesrank1.Where(s => s.AccoSubjectCode.StartsWith("6051")).ToList();
                //        msg2 = "+其他业务收入";
                //    }
                //}
                //decimal salesAmountNet = salesummarys.Sum(s => s.SalesAmountNet);
                //decimal dictcountAmountNet = discountsummarys.Sum(s => s.MonthAmount) + discountsummarys.Sum(s => s.YearAmount) + discountsummarys.Sum(s => s.GlitAmount);
                //decimal amount1 = mainBusiness.Sum(s => s.Credit) + mainBusiness2.Sum(s => s.Credit);
                //if (optionvalue5_2)
                //{
                //    if ((salesAmountNet - dictcountAmountNet) != amount1)
                //    {
                //        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【科目余额表】（主营业务收入" + msg2 + "）本期贷方合计" + amount1.ToString("#,##0.00 ") + "与【销售汇总表】销售净额合计" + salesAmountNet.ToString("#,##0.00 ") + "-【折扣汇总表】合计" + dictcountAmountNet.ToString("#,##0.00 ") + "元，合计" + (salesAmountNet - dictcountAmountNet).ToString("#,##0.00 ") + "元不一致，差额" + (amount1 - (salesAmountNet - dictcountAmountNet)).ToString("#,##0.00 ") + "元", ResultState = false });
                //    }
                //    else
                //    {
                //        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【科目余额表】（主营业务收入" + msg2 + "）本期贷方合计" + amount1.ToString("#,##0.00 ") + "与【销售汇总表】销售净额合计" + salesAmountNet.ToString("#,##0.00 ") + "-【折扣汇总表】合计" + dictcountAmountNet.ToString("#,##0.00 ") + "元，合计" + (salesAmountNet - dictcountAmountNet).ToString("#,##0.00 ") + "元一致", ResultState = true });
                //    }
                //}
                //else
                //{
                //    if (salesAmountNet != amount1)
                //    {
                //        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【科目余额表】（主营业务收入" + msg2 + "）本期贷方合计" + amount1.ToString("#,##0.00 ") + "与【销售汇总表】销售净额合计" + salesAmountNet.ToString("#,##0.00 ") + "元不一致，差额" + (amount1 - salesAmountNet).ToString("#,##0.00 ") + "元", ResultState = false });
                //    }
                //    else
                //    {
                //        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【科目余额表】（主营业务收入" + msg2 + "）本期贷方合计" + amount1.ToString("#,##0.00 ") + "与【销售汇总表】销售净额合计" + salesAmountNet.ToString("#,##0.00 ") + "元一致", ResultState = true });
                //    }
                //}
                //menuid = 8;
                //bool optionvalue7 = FMAPIService.OptionConfigValue("20230314112434", model.EnterpriseID.ToString(), model.GroupID.ToString());
                //bool optionvalue8 = FMAPIService.OptionConfigValue("20230422143937", model.EnterpriseID.ToString(), model.GroupID.ToString());
                //bool optionvalue10 = FMAPIService.OptionConfigValue("20230522105335", model.EnterpriseID.ToString(), model.GroupID.ToString());
                //string msg1 = string.Empty;
                //decimal mainBusinessAmount = 0;
                //if (optionvalue10)
                //{
                //    msg1 = "主营业务成本";
                //    mainBusinessAmount = balancesrank1.Where(s => s.AccoSubjectCode.StartsWith("6401")).Sum(s => s.Debit);
                //}
                //else
                //{
                //    msg1 = "主营业务成本-主营业务成本/合同履约成本";
                //    mainBusinessAmount = balancesrank1.Where(s => s.AccoSubjectCode.StartsWith("6401")).Sum(s => s.Debit) - balancesrank1.Where(s => s.AccoSubjectCode.StartsWith("640103")).Sum(s => s.Debit);
                //}
                //List<SubjectBalance> mainBusinessCost2 = new List<SubjectBalance>();
                //List<SubjectBalance> mainBusinessCost3 = new List<SubjectBalance>();

                //if (optionvalue5)
                //{
                //    bool optionvalue8_1 = FMAPIService.OptionConfigValue("20230508144430", model.EnterpriseID.ToString(), model.GroupID.ToString());
                //    if (optionvalue8_1)
                //    {
                //        mainBusinessCost2 = balancesrank1.Where(s => s.AccoSubjectCode.StartsWith("640205") || s.AccoSubjectCode.StartsWith("640299")).ToList();
                //        msg1 += "+其他业务成本/促销品收入+其他业务成本/其他";
                //    }
                //    else
                //    {
                //        mainBusinessCost2 = balancesrank1.Where(s => s.AccoSubjectCode.StartsWith("6402")).ToList();
                //        msg1 += "+其他业务成本";
                //    }
                //}
                //if (optionvalue8)
                //{
                //    mainBusinessCost3 = balancesrank1.Where(s => s.AccoSubjectFullName == "存货跌价准备/消耗性生物资产跌价准备" || s.AccoSubjectFullName == "生产性生物资产减值准备").ToList();
                //    if (mainBusinessCost3.Count() > 0)
                //        msg1 += "+存货跌价准备/消耗性生物资产跌价准备+生产性生物资产减值准备";
                //}
                //decimal salesCost = salesummarys.Sum(s => s.SalesCost);
                //var yhpigcostList = FMAPIService.YhPigCostSummary(model);
                //decimal yhAmount = Convert.ToDecimal(yhpigcostList?.Sum(s => s.AdjustCount));
                //decimal mainBusinessCostAmount = mainBusinessAmount + mainBusinessCost2.Sum(s => s.Debit) + mainBusinessCost3.Sum(s => s.Debit);
                //if (optionvalue7)
                //{
                //    if (mainBusinessCostAmount != (salesCost + yhAmount))
                //    {
                //        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【科目余额表】（" + msg1 + "）本期借方合计" + mainBusinessCostAmount.ToString("#,##0.00 ") + "与【销售汇总表】销售成本合计" + salesCost.ToString("#,##0.00 ") + "+【养户成本汇总表】存货调整合计" + yhAmount.ToString("#,##0.00 ") + "合计" + (salesCost + yhAmount).ToString("#,##0.00 ") + "不一致，差额" + (mainBusinessCostAmount - salesCost - yhAmount).ToString("#,##0.00 ") + "元", ResultState = false });
                //    }
                //    else
                //    {
                //        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【科目余额表】（" + msg1 + "）本期借方合计" + mainBusinessCostAmount.ToString("#,##0.00 ") + "与【销售汇总表】销售成本合计" + salesCost.ToString("#,##0.00 ") + "+【养户成本汇总表】存货调整合计" + yhAmount.ToString("#,##0.00 ") + "合计" + (salesCost + yhAmount).ToString("#,##0.00 ") + "一致", ResultState = true });
                //    }
                //}
                //else
                //{
                //    if (mainBusinessCostAmount != salesCost)
                //    {
                //        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【科目余额表】（" + msg1 + "）本期借方合计" + mainBusinessCostAmount.ToString("#,##0.00 ") + "与【销售汇总表】销售成本合计" + salesCost.ToString("#,##0.00 ") + "不一致，差额" + (mainBusinessCostAmount - salesCost).ToString("#,##0.00 ") + "元", ResultState = false });
                //    }
                //    else
                //    {
                //        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【科目余额表】（" + msg1 + "）本期借方合计" + mainBusinessCostAmount.ToString("#,##0.00 ") + "与【销售汇总表】销售成本合计" + salesCost.ToString("#,##0.00 ") + "一致", ResultState = true });
                //    }
                //}
                //menuid = 9;
                ////成本汇总表
                //var costdata = FMAPIService.GetCostDataList(model, "bd.cDictName");
                //decimal rawMaterialCost = (decimal)costdata?.Where(s => s.SummaryType1Name == "原材料").Sum(s => s.qmAmount);
                //decimal rawMaterialBalances = (decimal)balancesrank1?.Where(s => s.AccoSubjectCode.StartsWith("1403")).Sum(s => s.Show_LastDebit) - (decimal)balancesrank1?.Where(s => s.AccoSubjectCode.StartsWith("1403")).Sum(s => s.Show_LastCredit);
                //if (rawMaterialCost != rawMaterialBalances)
                //{
                //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【科目余额表】原材料期末金额合计" + rawMaterialBalances.ToString("#,##0.00 ") + "与【成本汇总表】原材料期末成本" + rawMaterialCost.ToString("#,##0.00 ") + "不一致，差额" + (rawMaterialBalances - rawMaterialCost).ToString("#,##0.00 ") + "元", ResultState = false });
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【科目余额表】原材料期末金额合计" + rawMaterialBalances.ToString("#,##0.00 ") + "与【成本汇总表】原材料期末成本" + rawMaterialCost.ToString("#,##0.00 ") + "一致", ResultState = true });
                //}

                //menuid = 10;
                //decimal packagingCost = (decimal)costdata?.Where(s => s.SummaryType1Name == "包装物").Sum(s => s.qmAmount);
                //decimal packagingBalances = (decimal)balancesrank1?.Where(s => s.AccoSubjectCode.StartsWith("1409")).Sum(s => s.Show_LastDebit) - (decimal)balancesrank1?.Where(s => s.AccoSubjectCode.StartsWith("1409")).Sum(s => s.Show_LastCredit);
                //if (packagingCost != packagingBalances)
                //{
                //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【科目余额表】包装物期末金额合计" + packagingBalances.ToString("#,##0.00 ") + "与【成本汇总表】包装物期末成本" + packagingCost.ToString("#,##0.00 ") + "不一致，差额" + (packagingBalances - packagingCost).ToString("#,##0.00 ") + "元", ResultState = false });
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【科目余额表】包装物期末金额合计" + packagingBalances.ToString("#,##0.00 ") + "与【成本汇总表】包装物期末成本" + packagingCost.ToString("#,##0.00 ") + "一致", ResultState = true });
                //}

                //menuid = 11;
                //decimal halfCost = (decimal)costdata?.Where(s => s.SummaryType1Name == "半成品").Sum(s => s.qmAmount);
                //decimal halfBalances = (decimal)balancesrank1?.Where(s => s.AccoSubjectCode.StartsWith("1412")).Sum(s => s.Show_LastDebit) - (decimal)balancesrank1?.Where(s => s.AccoSubjectCode.StartsWith("1412")).Sum(s => s.Show_LastCredit);
                //if (halfCost != halfBalances)
                //{
                //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【科目余额表】自制半成品及在产品期末金额合计" + halfBalances.ToString("#,##0.00 ") + "与【成本汇总表】半成品期末成本" + halfCost.ToString("#,##0.00 ") + "不一致，差额" + (halfBalances - halfCost).ToString("#,##0.00 ") + "元", ResultState = false });
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【科目余额表】自制半成品及在产品期末金额合计" + halfBalances.ToString("#,##0.00 ") + "与【成本汇总表】半成品期末成本" + halfCost.ToString("#,##0.00 ") + "一致", ResultState = true });
                //}


                //menuid = 12;
                //bool optionvalue9 = FMAPIService.OptionConfigValue("2305081343090000150", model.EnterpriseID.ToString(), model.GroupID.ToString());
                //bool optionvalue11 = FMAPIService.OptionConfigValue("20230626105403", model.EnterpriseID.ToString(), model.GroupID.ToString());
                //decimal completeCost = (decimal)costdata?.Where(s => s.SummaryType1Name == "产成品").Sum(s => s.qmAmount);
                //var stockList = balancesrank1?.Where(s => s.AccoSubjectCode.StartsWith("1405")).ToList();
                //var assetsList = balancesrank1?.Where(s => s.AccoSubjectCode.StartsWith("1621")).ToList();
                //var depreciationList = balancesrank1?.Where(s => s.AccoSubjectCode.StartsWith("1622")).ToList();
                //var impairmentList = balancesrank1?.Where(s => s.AccoSubjectCode.StartsWith("1623")).ToList();
                //var consumeList = balancesrank1?.Where(s => s.AccoSubjectCode.StartsWith("1421")).ToList();
                //var costList = balancesrank1?.Where(s => s.AccoSubjectCode.StartsWith("5001")).ToList() ?? new List<SubjectBalance>();
                //var otherList = balancesrank1?.Where(s => s.AccoSubjectCode.StartsWith("500199")).ToList() ?? new List<SubjectBalance>();
                //decimal stockBalances = stockList.Sum(s => s.Show_LastDebit) - stockList.Sum(s => s.Show_LastCredit);
                //decimal assetsBalances = assetsList.Sum(s => s.Show_LastDebit) - assetsList.Sum(s => s.Show_LastCredit);
                //decimal depreciationBalances = depreciationList.Sum(s => s.Show_LastCredit) - depreciationList.Sum(s => s.Show_LastDebit);
                //decimal impairmentBalances = impairmentList.Sum(s => s.Show_LastCredit) - depreciationList.Sum(s => s.Show_LastDebit);
                //decimal consumeBalances = consumeList.Sum(s => s.Show_LastDebit) - consumeList.Sum(s => s.Show_LastCredit);
                //decimal costBalances = costList.Sum(s => s.Show_LastDebit) - costList.Sum(s => s.Show_LastCredit);
                //decimal otherBalances = otherList.Sum(s => s.Show_LastDebit) - otherList.Sum(s => s.Show_LastCredit);
                //decimal completeBalances = stockBalances + assetsBalances - depreciationBalances + consumeBalances;
                //string msg = string.Empty;
                //if (stockList.Count > 0) msg += "库存商品";
                //if (assetsList.Count > 0) msg += "+生产性生物资产";
                //if (depreciationList.Count > 0) msg += "-生产性生物资产累计折旧";
                //if (optionvalue9 && impairmentList.Count > 0)
                //{
                //    msg += "-生产性生物资产减值准备";
                //    completeBalances -= impairmentBalances;
                //}
                //if (consumeList.Count > 0) msg += "+消耗性生物资产";
                //if (optionvalue11)
                //{
                //    msg += "+生产成本-生产成本/其他科目";
                //    completeBalances = completeBalances + costBalances - otherBalances;
                //}
                //else
                //{
                //    completeBalances += costBalances;
                //    msg += "+生产成本";
                //}
                //msg = msg.Length > 4 ? ("(" + msg.TrimStart(new char[] { '-', '+' }) + ")") : msg;
                //if (completeCost != completeBalances)
                //{
                //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【科目余额表】" + msg + "期末金额合计" + completeBalances.ToString("#,##0.00 ") + "与【成本汇总表】产成品期末成本" + completeCost.ToString("#,##0.00 ") + "不一致，差额" + (completeBalances - completeCost).ToString("#,##0.00 ") + "元", ResultState = false });
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【科目余额表】" + msg + "期末金额合计" + completeBalances.ToString("#,##0.00 ") + "与【成本汇总表】产成品期末成本" + completeCost.ToString("#,##0.00 ") + "一致", ResultState = true });
                //}

                //menuid = 13;
                //bool optionvalue6 = FMAPIService.OptionConfigValue("20230201132442", model.EnterpriseID.ToString(), model.GroupID.ToString());
                //var pigCostSummary = FMAPIService.CostSummary(model);
                //var changeCostSummary = FMAPIService.ChangeCostSummary(model);
                //decimal qmAmount = 0;
                //string costappid = "2005201609450000101";
                //if (pigCostSummary.Count > 0)
                //{
                //    costappid = "2005201609450000101";
                //    qmAmount = pigCostSummary.Sum(s => s.qmOrginalValue) - pigCostSummary.Sum(s => s.qmAccumulatedDepreciation) + pigCostSummary.Sum(s => s.qmPrepaidExpenses);
                //}
                //if (changeCostSummary.Count > 0)
                //{
                //    costappid = "2009090102130000101";
                //    qmAmount = changeCostSummary.Sum(s => s.qmOrginalValue) - changeCostSummary.Sum(s => s.qmAccumulatedDepreciation) + changeCostSummary.Sum(s => s.qmPrepaidExpenses);
                //}
                //decimal completeBalances1 = consumeBalances + assetsBalances - depreciationBalances;
                //string msg3 = string.Empty;
                //if (consumeList.Count > 0) msg3 += "消耗性生物资产";
                //if (assetsList.Count > 0) msg3 += "+生产性生物资产";
                //if (depreciationList.Count > 0) msg3 += "-生产性生物资产累计折旧";
                //if (optionvalue11)
                //{
                //    msg3 += "+生产成本-生产成本/其他科目";
                //    completeBalances1 = completeBalances1 + costBalances - otherBalances;
                //}
                //else
                //{
                //    completeBalances1 += costBalances;
                //    msg3 += "+生产成本";
                //}
                //msg3 = msg.Length > 1 ? ("(" + msg3.TrimStart(new char[] { '-', '+' }) + ")") : msg3;
                //if (optionvalue6)
                //{
                //    var pigCostAmount = FMAPIService.PigCostDetailReportEndCostTotal(model);
                //    var yhPigCostAmount = FMAPIService.YhPigCostEndCostTotal(model);
                //    if ((pigCostAmount + yhPigCostAmount) != completeBalances1)
                //    {
                //        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【科目余额表】" + msg3 + "期末金额合计" + completeBalances1.ToString("#,##0.00 ") + "与【猪场成本明细表】期末总成本金额" + pigCostAmount.ToString("#,##0.00 ") + "+【养户成本汇总表】期末金额" + yhPigCostAmount.ToString("#,##0.00 ") + "合计" + (pigCostAmount + yhPigCostAmount).ToString("#,##0.00 ") + "不一致，差额" + (completeBalances1 - (pigCostAmount + yhPigCostAmount)).ToString("#,##0.00 ") + "元", ResultState = false });
                //    }
                //    else
                //    {
                //        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【科目余额表】" + msg3 + "期末金额合计" + completeBalances1.ToString("#,##0.00 ") + "与【猪场成本明细表】期末总成本金额" + pigCostAmount.ToString("#,##0.00 ") + "+【养户成本汇总表】期末金额" + yhPigCostAmount.ToString("#,##0.00 ") + "合计" + (pigCostAmount + yhPigCostAmount).ToString("#,##0.00 ") + "一致", ResultState = true });
                //    }
                //}
                //else
                //{
                //    if (qmAmount != completeBalances1)
                //    {
                //        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【科目余额表】" + msg3 + "期末金额合计" + completeBalances1.ToString("#,##0.00 ") + "与【" + costappid + "】（期末原值-期末累计折旧+期末待摊费用）金额合计" + qmAmount.ToString("#,##0.00 ") + "不一致，差额" + (completeBalances1 - qmAmount).ToString("#,##0.00 ") + "元", ResultState = false });
                //    }
                //    else
                //    {
                //        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【科目余额表】" + msg3 + "期末金额合计" + completeBalances1.ToString("#,##0.00 ") + "与【" + costappid + "】（期末原值-期末累计折旧+期末待摊费用）金额合计" + qmAmount.ToString("#,##0.00 ") + "一致", ResultState = true });
                //    }
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

        #region 数据核对明细
        /// <summary>
        /// 数据明细核对
        /// 资金汇总表和科目余额表，应付账款汇总表和往来余额表，应收汇总表和往来余额表
        /// </summary>
        /// <param name="model"></param>
        /// <returns>ResultState true:成功 false：失败</returns>
        public override ResultModel DataDetailCheck(FMSAccoCheckResultModel model)
        {
            var resultModel = new ResultModel() { Code = 1, ResultState = false };
            try
            {
                dynamic dataList = null;

                switch (model.AppID)
                {
                    case 1612121126590000101://应付账款汇总表
                        dataList = PaySummaryAndSettleReceiptBalance(model);
                        break;
                    case 1612121330480000101://应收账款汇总表
                        dataList = ReceivableSummaryAndSettleReceiptBalance(model);
                        break;
                    case 1611091727140000101://会计凭证
                        dataList = PaymentreceivablesAndSettlereceipt(model);
                        break;
                }

                resultModel.Code = 0;
                resultModel.ResultState = true;
                resultModel.Data = dataList;
            }
            catch (Exception ex)
            {
                resultModel.Msg = "查询异常" + ex.Message;
            }
            return resultModel;
        }

        /// <summary>
        /// 应付账款汇总表和往来余额表核对
        /// 往来余额表查询条件：开始和结束日期，往来类型-单位往来，数据来源-实时查询，往来科目-应付账款 科目级次-第1级
        /// 应付账款汇总表查询条件：开始和结束日期，供应商空，审核-全部，汇总方式一 - 供应商、汇总方式二空
        /// 汇总方式SummaryType1、SummaryType1Name都是供应商名称，往来余额表有ID和Name
        /// 指标中有一个不相等就视作差异行
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private List<PaySummaryAndSettleBalanceResult> PaySummaryAndSettleReceiptBalance(FMSAccoCheckResultModel model)
        {
            var dataList = new List<PaySummaryAndSettleBalanceResult>();
            try
            {
                //获取往来余额  CustomerID:对方单位
                var settleList = FMAPIService.SettleReceiptBalanceByCode(model, "2202");//SettleReceiptBalance(model)?.Where(s => s.AccoSubjectCode.StartsWith("2202"));
                //应付账款汇总表 
                var payList = FMAPIService.PayableSummaryByType(model, "supplier");
                if (payList?.Count > 0)
                {
                    foreach (var item in payList)
                    {
                        var filterList = settleList?.Where(p => p.CustomerName == item.SummaryType1Name);
                        //不存在
                        if (filterList == null || filterList.Count() == 0)
                        {
                            //不要金额全为0的
                            //if (item.QCYE == 0 && item.AmountTotal == 0 && item.JSTotal == 0 && item.ReceiptAmount == 0) continue;
                            dataList.Add(new PaySummaryAndSettleBalanceResult
                            {
                                SummaryType1 = item.SummaryType1,
                                SummaryType1Name = item.SummaryType1Name,
                                QCYE = item.QCYE,
                                AmountTotal = item.AmountTotal,
                                JSTotal = item.JSTotal,
                                ReceiptAmount = item.ReceiptAmount,
                                BeginBalance = 0,
                                Debit = 0,
                                Credit = 0,
                                LastBalance = 0,
                                BeginDiffAmount = item.QCYE,
                                DebitDiffAmount = item.JSTotal,
                                CreditDiffAmount = item.AmountTotal,
                                EndDiffAmount = item.ReceiptAmount,
                                AccoSubjectCode = "",
                                AccoSubjectFullName = "",
                                CustomerID = "",
                                CustomerName = ""
                            });
                        }
                        else
                        {
                            var payModel = item;
                            var settleModel = filterList.FirstOrDefault();
                            if (settleModel.BegBalance != payModel.QCYE || settleModel.Debit != payModel.JSTotal || settleModel.Credit != payModel.AmountTotal || settleModel.LastBalance != payModel.ReceiptAmount)//不相等
                            {
                                dataList.Add(new PaySummaryAndSettleBalanceResult
                                {
                                    SummaryType1 = payModel.SummaryType1,
                                    SummaryType1Name = payModel.SummaryType1Name,
                                    QCYE = payModel.QCYE,
                                    AmountTotal = payModel.AmountTotal,
                                    JSTotal = payModel.JSTotal,
                                    ReceiptAmount = payModel.ReceiptAmount,
                                    BeginBalance = settleModel.BegBalance,
                                    Debit = settleModel.Debit,
                                    Credit = settleModel.Credit,
                                    LastBalance = settleModel.LastBalance,
                                    BeginDiffAmount = payModel.QCYE - settleModel.BegBalance,
                                    DebitDiffAmount = payModel.JSTotal - settleModel.Debit,
                                    CreditDiffAmount = payModel.AmountTotal - settleModel.Credit,
                                    EndDiffAmount = payModel.ReceiptAmount - settleModel.LastBalance,
                                    AccoSubjectCode = settleModel.AccoSubjectCode,
                                    AccoSubjectFullName = settleModel.AccoSubjectFullName,
                                    CustomerID = settleModel.CustomerID,
                                    CustomerName = settleModel.CustomerName
                                });
                            }
                        }
                    }
                }
                if (settleList?.Count() > 0)
                {
                    foreach (var item in settleList)
                    {
                        var filterList = payList?.Where(p => p.SummaryType1Name == item.CustomerName);
                        //不存在
                        if (filterList == null || filterList.Count() == 0)
                        {
                            //不要金额全为0的
                            //if (item.BegBalance == 0 && item.Debit == 0 && item.Credit == 0 && item.LastBalance == 0) continue;
                            dataList.Add(new PaySummaryAndSettleBalanceResult
                            {
                                SummaryType1 = item.CustomerID,
                                SummaryType1Name = item.CustomerName,
                                QCYE = 0,
                                AmountTotal = 0,
                                JSTotal = 0,
                                ReceiptAmount = 0,
                                BeginBalance = item.BegBalance,
                                Debit = item.Debit,
                                Credit = item.Credit,
                                LastBalance = item.LastBalance,
                                BeginDiffAmount = -item.BegBalance,
                                DebitDiffAmount = -item.Debit,
                                CreditDiffAmount = -item.Credit,
                                EndDiffAmount = -item.LastBalance,
                                AccoSubjectCode = item.AccoSubjectCode,
                                AccoSubjectFullName = item.AccoSubjectFullName,
                                CustomerID = item.CustomerID,
                                CustomerName = item.CustomerName
                            });
                        }
                    }
                }
                dataList = dataList?.Where(p => p.QCYE != 0 || p.AmountTotal != 0 || p.JSTotal != 0 || p.ReceiptAmount != 0 || p.BeginBalance != 0 || p.Debit != 0 || p.Credit != 0 || p.LastBalance != 0)?.ToList();
                return dataList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 应收账款汇总表和往来余额表核对
        /// 往来余额表查询条件：开始和结束日期，往来类型-单位往来，数据来源-实时查询，往来科目-应收账款 科目级次-第1级
        /// 应收账款汇总表查询条件：开始和结束日期，客户空，销售类型 空 汇总方式一 - 客户、汇总方式二空
        /// 汇总方式有客户ID和名称，往来余额表有ID和Name
        /// 指标中有一个不相等就视作差异行
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private List<ReceivableSummaryAndSettleBalanceResult> ReceivableSummaryAndSettleReceiptBalance(FMSAccoCheckResultModel model)
        {
            var dataList = new List<ReceivableSummaryAndSettleBalanceResult>();
            try
            {
                //获取往来余额  CustomerName:对方单位
                var settleList = FMAPIService.SettleReceiptBalanceByCode(model, "1122");//?.Where(s => s.AccoSubjectCode.StartsWith("1122"));
                //应收账款汇总表 和产品确认eDuZhanBiGongShi=0，不用选应收额度占比
                var receList = FMAPIService.ReceivableSummaryByType(model, "cs.CustomerName");
                if (receList?.Count > 0)
                {
                    foreach (var item in receList)
                    {
                        var filterList = settleList?.Where(p => p.CustomerID == item.SummaryType1);
                        //不存在
                        if (filterList == null || filterList.Count() == 0)
                        {
                            //不要金额全为0的
                            //if (item.BeginningAmount == 0 && item.Increase_SubTotal == 0 && item.Decrease_SubTotal == 0 && item.Receivable_SubTotal == 0) continue;
                            dataList.Add(new ReceivableSummaryAndSettleBalanceResult
                            {
                                SummaryType1 = item.SummaryType1,
                                SummaryType1Name = item.SummaryType1Name,
                                YSBeginningAmount = item.BeginningAmount,
                                YSIncrease_SubTotal = item.Increase_SubTotal,
                                YSDecrease_SubTotal = item.Decrease_SubTotal,
                                YSReceivable_SubTotal = item.Receivable_SubTotal,
                                BeginBalance = 0,
                                Debit = 0,
                                Credit = 0,
                                LastBalance = 0,
                                BeginDiffAmount = item.BeginningAmount,
                                DebitDiffAmount = item.Increase_SubTotal,
                                CreditDiffAmount = item.Decrease_SubTotal,
                                EndDiffAmount = item.Receivable_SubTotal,
                                AccoSubjectCode = "",
                                AccoSubjectFullName = "",
                                CustomerID = "",
                                CustomerName = ""
                            });
                        }
                        else
                        {
                            var payModel = item;
                            var settleModel = filterList.FirstOrDefault();
                            if (settleModel.BegBalance != payModel.BeginningAmount || settleModel.Debit != payModel.Increase_SubTotal || settleModel.Credit != payModel.Decrease_SubTotal || settleModel.LastBalance != payModel.Receivable_SubTotal)//不相等
                            {
                                dataList.Add(new ReceivableSummaryAndSettleBalanceResult
                                {
                                    SummaryType1 = payModel.SummaryType1,
                                    SummaryType1Name = payModel.SummaryType1Name,
                                    YSBeginningAmount = payModel.BeginningAmount,
                                    YSIncrease_SubTotal = payModel.Increase_SubTotal,
                                    YSDecrease_SubTotal = payModel.Decrease_SubTotal,
                                    YSReceivable_SubTotal = payModel.Receivable_SubTotal,
                                    BeginBalance = settleModel.BegBalance,
                                    Debit = settleModel.Debit,
                                    Credit = settleModel.Credit,
                                    LastBalance = settleModel.LastBalance,
                                    BeginDiffAmount = payModel.BeginningAmount - settleModel.BegBalance,
                                    DebitDiffAmount = payModel.Increase_SubTotal - settleModel.Debit,
                                    CreditDiffAmount = payModel.Decrease_SubTotal - settleModel.Credit,
                                    EndDiffAmount = payModel.Receivable_SubTotal - settleModel.LastBalance,
                                    AccoSubjectCode = settleModel.AccoSubjectCode,
                                    AccoSubjectFullName = settleModel.AccoSubjectFullName,
                                    CustomerID = settleModel.CustomerID,
                                    CustomerName = settleModel.CustomerName
                                });
                            }
                        }
                    }
                }
                if (settleList?.Count() > 0)
                {
                    foreach (var item in settleList)
                    {
                        var filterList = receList?.Where(p => p.SummaryType1 == item.CustomerID);
                        //不存在
                        if (filterList == null || filterList.Count() == 0)
                        {
                            //不要金额全为0的
                            //if (item.BegBalance == 0 && item.Debit == 0 && item.Credit == 0 && item.LastBalance == 0) continue;
                            dataList.Add(new ReceivableSummaryAndSettleBalanceResult
                            {
                                SummaryType1 = item.CustomerID,
                                SummaryType1Name = item.CustomerName,
                                YSBeginningAmount = 0,
                                YSIncrease_SubTotal = 0,
                                YSDecrease_SubTotal = 0,
                                YSReceivable_SubTotal = 0,
                                BeginBalance = item.BegBalance,
                                Debit = item.Debit,
                                Credit = item.Credit,
                                LastBalance = item.LastBalance,
                                BeginDiffAmount = -item.BegBalance,
                                DebitDiffAmount = -item.Debit,
                                CreditDiffAmount = -item.Credit,
                                EndDiffAmount = -item.LastBalance,
                                AccoSubjectCode = item.AccoSubjectCode,
                                AccoSubjectFullName = item.AccoSubjectFullName,
                                CustomerID = item.CustomerID,
                                CustomerName = item.CustomerName
                            });
                        }
                    }
                }
                dataList = dataList?.Where(p => p.YSBeginningAmount != 0 || p.YSIncrease_SubTotal != 0 || p.YSDecrease_SubTotal != 0 || p.YSReceivable_SubTotal != 0 || p.BeginBalance != 0 || p.Debit != 0 || p.Credit != 0 || p.LastBalance != 0)?.ToList();
                return dataList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private List<PaymentreceivablesAndSettlereceipt> PaymentreceivablesAndSettlereceipt(FMSAccoCheckResultModel model)
        {
            var dataList = new List<PaymentreceivablesAndSettlereceipt>();
            try
            {
                //获取收付款/汇总单
                var payreceList = FMAPIService.GetPaymentreceivables(model);
                //获取收付款凭证
                var settleList = FMAPIService.GetSettlereceipt(model);

                if (payreceList?.Count > 0)
                {
                    //未生成凭证的收付款单
                    var noSettleList = payreceList?.Where(p => p.SNumericalOrder == null);
                    if (noSettleList?.Count() > 0)
                    {
                        foreach (var item in noSettleList)
                        {
                            dataList.Add(SetPayReceDetailInfo(item));
                        }
                    }
                    //已生成凭证
                    var hasSettleList = payreceList?.Where(p => p.SNumericalOrder != null)?.GroupBy(p => p.SNumericalOrder);
                    if (hasSettleList?.Count() > 0)
                    {
                        foreach (var group in hasSettleList)
                        {
                            var filtersettleList = settleList?.Where(p => p.SNumericalOrder == group.Key);
                            foreach (var item in group)
                            {
                                var payrece = SetPayReceDetailInfo(item);
                                //凭证流水号未对上
                                if (filtersettleList == null || filtersettleList.Count() == 0)
                                {
                                    dataList.Add(item);
                                }
                                else
                                {
                                    var filteraccountList = filtersettleList?.Where(p => p.SAccountID == payrece.AccountID);
                                    //有凭证，但资金账户未对上
                                    if (filteraccountList == null || filteraccountList?.Count() == 0)
                                    {
                                        var settle = filtersettleList.FirstOrDefault();
                                        dataList.Add(new PaymentreceivablesAndSettlereceipt()
                                        {
                                            NumericalOrder = payrece.NumericalOrder,
                                            DataDate = payrece.DataDate,
                                            Number = payrece.Number,
                                            TicketedPointName = payrece.TicketedPointName,
                                            SettleReceipType = payrece.SettleReceipType,
                                            SettleReceipTypeName = payrece.SettleReceipTypeName,
                                            AccountID = payrece.AccountID,
                                            AccountName = payrece.AccountName,
                                            IsGroupPay = payrece.IsGroupPay,
                                            PAmount = payrece.PAmount,
                                            RAmount = payrece.RAmount,
                                            SNumericalOrder = payrece.SNumericalOrder,
                                            SDataDate = settle.SDataDate,
                                            SNumber = settle.SNumber,
                                            STicketedPointName = settle.STicketedPointName,
                                            SSettleReceipType = settle.SSettleReceipType,
                                            SSettleReceipTypeName = settle.SSettleReceipTypeName,
                                            SAccountID = null,
                                            SAccountName = null,
                                            Debit = 0,
                                            Credit = 0
                                        });
                                    }
                                    //金额不相等
                                    else if (filteraccountList.FirstOrDefault().Debit != payrece.Amount && filteraccountList.FirstOrDefault().Credit != payrece.Amount)
                                    {
                                        var settle = filteraccountList.FirstOrDefault();
                                        dataList.Add(new PaymentreceivablesAndSettlereceipt()
                                        {
                                            NumericalOrder = payrece.NumericalOrder,
                                            DataDate = payrece.DataDate,
                                            Number = payrece.Number,
                                            TicketedPointName = payrece.TicketedPointName,
                                            SettleReceipType = payrece.SettleReceipType,
                                            SettleReceipTypeName = payrece.SettleReceipTypeName,
                                            AccountID = payrece.AccountID,
                                            AccountName = payrece.AccountName,
                                            IsGroupPay = payrece.IsGroupPay,
                                            PAmount = payrece.PAmount,
                                            RAmount = payrece.RAmount,
                                            SNumericalOrder = payrece.SNumericalOrder,
                                            SDataDate = settle.SDataDate,
                                            SNumber = settle.SNumber,
                                            STicketedPointName = settle.STicketedPointName,
                                            SSettleReceipType = settle.SSettleReceipType,
                                            SSettleReceipTypeName = settle.SSettleReceipTypeName,
                                            SAccountID = settle.SAccountID,
                                            SAccountName = settle.SAccountName,
                                            Debit = settle.Debit,
                                            Credit = settle.Credit
                                        });
                                    }
                                }
                            }
                        }
                    }

                }
                //只有凭证，无收付款单
                if (settleList?.Count() > 0)
                {
                    foreach (var item in settleList)
                    {
                        var filtersettleList = payreceList?.Where(p => p.SNumericalOrder == item.SNumericalOrder);
                        //凭证
                        if (filtersettleList == null || filtersettleList.Count() == 0)
                        {
                            item.AccountID = item.SAccountID;
                            item.AccountName = item.SAccountName;
                            dataList.Add(item);
                        }
                    }
                }
                dataList = dataList?.Where(p => p.PAmount != 0 || p.RAmount != 0 || p.Debit != 0 || p.Credit != 0)?.ToList();
                return dataList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public PaymentreceivablesAndSettlereceipt SetPayReceDetailInfo(PaymentreceivablesAndSettlereceipt item)
        {
            if (item.SettleReceipType == SettleReceipTypeEnum.RSettleReceipType)//收款、汇总
            {
                item.RAmount = item.Amount;
                item.PAmount = 0;
                item.SettleReceipTypeName = "收款单";
            }
            else if (item.SettleReceipType == "201611180104402203")
            {
                item.RAmount = item.Amount;
                item.PAmount = 0;
                item.SettleReceipTypeName = "收款汇总单";
            }
            else if (item.SettleReceipType == "201611180104402202")//收款、汇总
            {
                item.RAmount = 0;
                item.PAmount = item.Amount;
                item.SettleReceipTypeName = "付款单";
            }
            else if (item.SettleReceipType == "201611180104402204")
            {
                item.RAmount = 0;
                item.PAmount = item.Amount;
                item.SettleReceipTypeName = "付款汇总单";
            }
            return item;
        }
        public static class SettleReceipTypeEnum
        {
            public static string RSettleReceipType = "201611180104402201";
            public static string RSSettleReceipType = "201611180104402203";
            public static string PSettleReceipType = "201611180104402202";
            public static string PSSettleReceipType = "201611180104402204";
        }
        #endregion
    }
}
