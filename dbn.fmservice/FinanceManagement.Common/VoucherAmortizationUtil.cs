using Architecture.Common.HttpClientUtil;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using FinanceManagement.Common.MakeVoucherCommon;

namespace FinanceManagement.Common
{
    public class VoucherAmortizationUtil
    {
        HttpClientUtil _httpClientUtil1;

        HostConfiguration _hostCongfiguration;
        public VoucherAmortizationUtil(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
        }
        public FM_VoucherAmortizationRecord verificationAmortization(FD_VoucherAmortizationODataEntity model, MakeVoucherModel request, FD_VoucherAmortizationPeriodDetailODataEntity currentData, int currentIndex)
        {
            FM_VoucherAmortizationRecord result = new FM_VoucherAmortizationRecord
            {
                NumericalOrderVoucher = model.NumericalOrder,
                AmortizationName = model.AmortizationName,
                OwnerID = request.Boid,
                ResultState = false,
                EnterpriseID = request.EnterpriseID,
            };
            if (model.ImpStateID == 2)
            {
                result.ImplementResult = model.NumericalOrder + "已关闭的方案不能生成凭证";
                return result;
            }
            if (!model.IsUse)
            {
                result.ImplementResult = model.NumericalOrder + "已禁用的方案不能生成凭证";
                return result;
            }
            if (currentIndex > 0)
            {
                var previous = model.PeriodLines[currentIndex - 1];
                if (!previous.IsAmort)
                {
                    result.ImplementResult = model.NumericalOrder + "上个会计期间的摊销金额未生成凭证";
                    return result;
                }
            }
            if (currentData == null)
            {
                result.ImplementResult = model.NumericalOrder + "当期没有摊销方案，无需摊销";
                return result;
            }
            else
            {
                if (currentData.IsAmort)
                {
                    result.ImplementResult = model.NumericalOrder + "当期已摊销，无法重复摊销";
                    return result;
                }
            }
            return result;
        }
        public MakeVoucherCommon.FD_SettleReceipt getSettleReceiptModel(FD_VoucherAmortizationODataEntity model, MakeVoucherModel request, FD_VoucherAmortizationPeriodDetailODataEntity currentData, List<FM_VoucherAmortizationRelatedODataEntity> relateds, int currentIndex)
        {
            try
            {
                var creditData = model.Lines.Where(s => s.IsDebit == true).ToList();//待摊销数据
                var debitData = model.Lines.Where(s => s.IsDebit == false).ToList();//转入数据
                MakeVoucherCommon.FD_SettleReceipt domain = new MakeVoucherCommon.FD_SettleReceipt();
                domain.SettleReceipType = "201610220104402203";
                domain.TicketedPointID = model.TicketedPointID;
                domain.EnterpriseID = request.EnterpriseID;
                domain.OwnerID = request.Boid;
                domain.CurrentEnterDate = request.CurrentEnterDate;
                domain.DataDate = request.PeriodEndDate;
                domain.IsSettleCheckOut = request.IsSettleCheckOut;
                decimal creditSum = 0;
                decimal debitSum = 0;
                if (!currentData.IsLast || currentIndex == 0)//不是最后期间或第一次摊销按比例算
                {
                    decimal valueNumber = creditData.Sum(o => o.ValueNumber);//待摊销总金额
                    decimal proportion = Math.Round(currentData.AmortizationAmount / valueNumber, 4);//计算比例
                    #region 计算贷方数据
                    for (int i = 0; i < creditData.Count; i++)
                    {
                        MakeVoucherCommon.FD_SettleReceiptDetailCommand detail = new MakeVoucherCommon.FD_SettleReceiptDetailCommand()
                        {
                            AccoSubjectCode = creditData[i].AccoSubjectCode,
                            AccoSubjectID = creditData[i].AccoSubjectID,
                            ReceiptAbstractID = model.AbstractID,
                            ReceiptAbstractName = model.AbstractName,
                            LorR = false,//贷方
                            PersonID = creditData[i].PersonID,
                            CustomerID = creditData[i].CustomerID != "0" ? creditData[i].CustomerID : creditData[i].SupplierID,
                            MarketID = creditData[i].MarketID,
                            ProductID = "0",
                        };
                        if (i == creditData.Count - 1)
                        {
                            detail.Credit = currentData.AmortizationAmount - creditSum;
                        }
                        else
                        {
                            var currCredit = Math.Round(creditData[i].ValueNumber * proportion, 2);
                            creditSum += currCredit;
                            detail.Credit = currCredit;
                        }
                        domain.Relateds.Add(new FM_VoucherAmortizationRelated() { NumericalOrderVoucher = model.NumericalOrder, NumericalOrderStay = creditData[i].NumericalOrderDetail, NumericalOrderInto = currentData.NumericalOrderDetail, VoucherAmount = detail.Credit });
                        domain.Lines.Add(detail);
                    }
                    #endregion
                }
                else
                {//最后一次摊销算法
                    for (int i = 0; i < creditData.Count; i++)
                    {
                        MakeVoucherCommon.FD_SettleReceiptDetailCommand detail = new MakeVoucherCommon.FD_SettleReceiptDetailCommand()
                        {
                            AccoSubjectCode = creditData[i].AccoSubjectCode,
                            AccoSubjectID = creditData[i].AccoSubjectID,
                            ReceiptAbstractID = model.AbstractID,
                            ReceiptAbstractName=model.AbstractName,
                            LorR = false,//贷方
                            PersonID = creditData[i].PersonID,
                            CustomerID = creditData[i].CustomerID != "0" ? creditData[i].CustomerID : creditData[i].SupplierID,
                            MarketID = creditData[i].MarketID,
                            ProductID = "0",
                        };
                        decimal value = 0;
                        decimal.TryParse(relateds.Where(s => s.NumericalOrderStay == creditData[i].NumericalOrderDetail).FirstOrDefault()?.VoucherAmount.ToString(), out value);
                        detail.Credit = creditData[i].ValueNumber - value;
                        domain.Relateds.Add(new FM_VoucherAmortizationRelated() { NumericalOrderVoucher = model.NumericalOrder, NumericalOrderStay = creditData[i].NumericalOrderDetail, NumericalOrderInto = currentData.NumericalOrderDetail, VoucherAmount = detail.Credit });
                        domain.Lines.Add(detail);
                    }
                }
                #region 计算借方数据
                for (int i = 0; i < debitData.Count; i++)
                {
                    MakeVoucherCommon.FD_SettleReceiptDetailCommand detail = new MakeVoucherCommon.FD_SettleReceiptDetailCommand()
                    {
                        AccoSubjectCode = debitData[i].AccoSubjectCode,
                        AccoSubjectID = debitData[i].AccoSubjectID,
                        ReceiptAbstractID = model.AbstractID,
                        ReceiptAbstractName = model.AbstractName,
                        LorR = true,//借方
                        PersonID = debitData[i].PersonID,
                        CustomerID = debitData[i].CustomerID != "0" ? debitData[i].CustomerID : debitData[i].SupplierID,
                        MarketID = debitData[i].MarketID,
                        ProductID = "0",
                    };
                    if (i == debitData.Count - 1)
                    {
                        detail.Debit = currentData.AmortizationAmount - debitSum;
                    }
                    else
                    {
                        var currDebit = Math.Round(currentData.AmortizationAmount * debitData[i].ValueNumber, 2);
                        debitSum += currDebit;
                        detail.Debit = currDebit;
                    }
                    domain.Lines.Add(detail);
                }
                #endregion
                return domain;
            }
            catch (Exception ex)
            {
                return new MakeVoucherCommon.FD_SettleReceipt();
            }

        }
        /// <summary>
        /// 生成凭证凭证算法
        /// </summary>
        /// <param name="voucherAmortization">业务单据</param>
        /// <param name="request">请求参数</param>
        /// <param name="currentData">当前结转期间</param>
        /// <param name="currentIndex">当前结转期间索引</param>
        /// <param name="relateds">摊销关系</param>
        /// <returns></returns>
        public async Task<ResultModel> getMultipleMakeSettleResult(FD_VoucherAmortizationODataEntity voucherAmortization, MakeVoucherModel request, FD_VoucherAmortizationPeriodDetailODataEntity currentData, int currentIndex, List<FM_VoucherAmortizationRelatedODataEntity> relateds)
        {
            try
            {
                MakeVoucherCommon.FD_SettleReceipt settle = new MakeVoucherCommon.FD_SettleReceipt() { CurrentEnterDate = request.CurrentEnterDate };
                FM_VoucherAmortizationRecord record = verificationAmortization(voucherAmortization, request, currentData, currentIndex);
                if (string.IsNullOrEmpty(record.ImplementResult))
                {
                    record.ImplementResult = "执行成功！";
                    record.ResultState = true;
                    settle = getSettleReceiptModel(voucherAmortization, request, currentData, relateds, currentIndex);
                }
                settle.Lines=settle.Lines.OrderByDescending(s => s.Debit).ToList();
                settle.Records.Add(record);
                var res = await _httpClientUtil1.PostJsonAsync<ResultModel>($"{_hostCongfiguration.QlwServiceHost}/api/CreateVoucher/VoucherAmortizationInsertSettle", settle);
                return res;
            }
            catch (Exception ex)
            {
                return new ResultModel() { ResultState = false, Msg = "保存凭证失败" };
            }
           
        }
        /// <summary>
        /// 生成凭证凭证算法
        /// </summary>
        /// <param name="voucherAmortizationList">摊销结果集合</param>
        /// <param name="request">请求参数</param>
        /// <param name="relateds">摊销关系</param>
        /// <param name="records">验证结果</param>
        /// <returns></returns>
        public async Task<ResultModel> getSingleMakeSettleResult(List<FD_VoucherAmortizationODataEntity> voucherAmortizationList, MakeVoucherModel request, List<FM_VoucherAmortizationRelatedODataEntity> relateds, List<FM_VoucherAmortizationRecord> records)
        {
            try
            {
                MakeVoucherCommon.FD_SettleReceipt settle = new MakeVoucherCommon.FD_SettleReceipt() { CurrentEnterDate = request.CurrentEnterDate };
                MakeVoucherCommon.FD_SettleReceipt result = new MakeVoucherCommon.FD_SettleReceipt();
                for (int i = 0; i < voucherAmortizationList.Count; i++)
                {
                    var currentData = voucherAmortizationList[i].PeriodLines.Where(s => Convert.ToDateTime(s.AccountDate).Year == Convert.ToDateTime(request.DataDate).Year && Convert.ToDateTime(s.AccountDate).Month == Convert.ToDateTime(request.DataDate).Month).FirstOrDefault();
                    int currentIndex = voucherAmortizationList[i].PeriodLines.FindIndex(s => Convert.ToDateTime(s.AccountDate).Year == Convert.ToDateTime(request.DataDate).Year && Convert.ToDateTime(s.AccountDate).Month == Convert.ToDateTime(request.DataDate).Month);
                    result = getSettleReceiptModel(voucherAmortizationList[i], request, currentData, relateds, currentIndex);
                    if (i == 0)
                    {
                        settle = result;
                    }
                    else
                    {
                        settle.Lines.AddRange(result.Lines);
                        settle.Relateds.AddRange(result.Relateds);
                    }
                }
                records.ForEach(s => { s.ImplementResult = "执行成功"; s.ResultState = true; });
                settle.Records = records;
                settle.Lines = settle.Lines.OrderByDescending(s => s.Debit).ToList();
                var res = await _httpClientUtil1.PostJsonAsync<ResultModel>($"{_hostCongfiguration.QlwServiceHost}/api/CreateVoucher/VoucherAmortizationInsertSettle", settle);
                return res;
            }
            catch (Exception ex)
            {
                return new ResultModel() { ResultState = false, Msg = "保存凭证失败" };
                throw;
            }
            
        }
        /// <summary>
        /// 会计期间
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<List<EnterprisePeriod>> getEnterprisePeriod(EnterprisePeriodSearch model)
        {
            try
            {
                var res = await _httpClientUtil1.PostJsonAsync<ResultModel<EnterprisePeriod>>($"{_hostCongfiguration.QlwServiceHost}/api/BIZEnterprise/getEnterprisePeriod", model);
                if (res.ResultState)
                {
                    return res.Data;
                }
                else
                {
                    string newDate = model.Year + "-" + model.Month;
                    return new List<EnterprisePeriod>() { new EnterprisePeriod() { StartDate = Convert.ToDateTime(newDate).AddMonths(1).AddDays(-1) } };
                }
            }
            catch (Exception ex)
            {
                return new List<EnterprisePeriod>();
            }
        }
    }
}
