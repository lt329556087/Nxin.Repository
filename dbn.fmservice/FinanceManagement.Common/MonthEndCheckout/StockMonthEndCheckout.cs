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
    public class StockMonthEndCheckout : MonthEndCheckout
    {
        /// <summary>
        /// 存货结账
        /// </summary>
        FMAPIService FMAPIService;
        public StockMonthEndCheckout(FMAPIService FMAPIService)
        {
            this.FMAPIService = FMAPIService;
        }
        /// <summary>
        /// 生成存货汇总表
        /// </summary>
        /// <returns></returns>
        private bool GenerateWarehouseBalance(FMSAccoCheckResultModel model)
        {
            try
            {
                var result = FMAPIService.GenerateWarehouseBalance(model.EnterpriseID, model.Date);
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
        /// 取消存货汇总表
        /// </summary>
        /// <returns></returns>
        private bool CancelWarehouseBalance(FMSAccoCheckResultModel model)
        {
            try
            {
                var result = FMAPIService.CancelWarehouseBalance(model.EnterpriseID, model.Date);
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
        /// 计算销售成本和毛利
        /// </summary>
        /// <returns></returns>
        private bool CalcSalesCostAndProfit(FMSAccoCheckResultModel model)
        {
            try
            {
                var result = FMAPIService.CalcSalesCostAndProfit(model);
                if (result.errcode != FMErrorEnum.OK)
                {
                    return false;
                }
                return result.result_code == FMBusinessResultEnum.SUCCESS;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 取消销售成本和毛利
        /// </summary>
        /// <returns></returns>
        private bool CancelSalesCostAndProfit(FMSAccoCheckResultModel model)
        {
            try
            {
                var result = FMAPIService.CancelSalesCostAndProfit(model);
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
                    bool result = GenerateWarehouseBalance(model);
                    if (result)
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "生成存货汇总表成功", ResultState = true });
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "生成存货汇总表失败", ResultState = false });
                        return list;
                    }
                }
                if (menuid == 99)
                {
                    bool result1 = CalcSalesCostAndProfit(model);
                    if (result1)
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "计算销售成本和毛利成功", ResultState = true });
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "计算销售成本和毛利失败", ResultState = false });
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
                    var result = CancelSalesCostAndProfit(model);
                    if (result)
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "取消销售成本和毛利成功", ResultState = true });
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "取消销售成本和毛利失败", ResultState = false });
                    }
                }
                if (menuid == 98)
                {
                    var result1 = CancelWarehouseBalance(model);
                    if (result1)
                    {
                        list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "取消存货汇总表成功", ResultState = true });
                    }
                    else
                    {
                        list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "取消存货汇总表失败", ResultState = false });
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
                List<FM_AccoCheckRuleODataEntity> ruleList = FMAPIService.GetAaccocheckRuleList(AccoCheckTypeEnum.存货结账.GetValue().ToString(), model.EnterpriseID.ToString());
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
                //var costsummarys = FMAPIService.GetCostDataList(model, "ente.EnterpriseName");
                //var stockdatas = FMAPIService.GetStockDataByAbstract(model);
                //decimal costQuantity =Convert.ToDecimal( costsummarys?.Sum(s => s.qmQuantity));
                //decimal stockQuantity = Convert.ToDecimal(stockdatas?.Sum(s => s.EndingQuantity));
                //if (costQuantity != stockQuantity)
                //{
                //    list.Add(new ResultModel() { Code = 1, CodeNo = menuid, Msg = "【存货汇总表】期末数量合计" + stockQuantity + "与【成本汇总表】期末数量合计" + costQuantity + "不一致，差额" + (stockQuantity - costQuantity), ResultState = false });
                //}
                //else
                //{
                //    list.Add(new ResultModel() { Code = 0, CodeNo = menuid, Msg = "【存货汇总表】期末数量合计" + stockQuantity + "与【成本汇总表】期末数量合计" + costQuantity + "一致", ResultState = true });
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
        /// 按商品代号汇总 核对期末数量差异 差异=存货-成本
        /// 成本汇总表汇总方式SummaryType1、SummaryType1Name都是商品代号名称
        /// 存货汇总表汇总方式ID、名称
        /// </summary>
        /// <param name="model"></param>
        /// <returns>ResultState true:成功 false：失败</returns>
        public override ResultModel DataDetailCheck(FMSAccoCheckResultModel model)
        {
            var resultModel = new ResultModel() { Code = 1, ResultState = false };
            try
            {
                var costsummarys = FMAPIService.GetCostDataList(model, "b.ProductID,busipd.Specification,um.UnitName,d.ProductName");
                var stockdatas = FMAPIService.GetStockDataByProduct(model, "d.ProductName");
                var dataList = new List<StockCostResult>();
                if (stockdatas?.Count > 0)
                {
                    foreach (var item in stockdatas)
                    {
                        var filterList = costsummarys?.Where(p => p.SummaryType1Name == item.SummaryType1Name);
                        //不存在
                        if (filterList == null || filterList.Count() == 0)
                        {
                            dataList.Add(new StockCostResult { SummaryType1 = item.SummaryType1, SummaryType1Name = item.SummaryType1Name, SummaryType1FieldName = item.SummaryType1FieldName, StockEndQuantity = item.EndingQuantity, CostEndQuantity = 0, DiffQuantity = item.EndingQuantity });

                        }
                        else if (item.EndingQuantity != filterList.FirstOrDefault().qmQuantity)//不相等
                        {
                            var costEndQuantity = filterList.FirstOrDefault().qmQuantity;
                            dataList.Add(new StockCostResult { SummaryType1 = item.SummaryType1, SummaryType1Name = item.SummaryType1Name, SummaryType1FieldName = item.SummaryType1FieldName, StockEndQuantity = item.EndingQuantity, CostEndQuantity = costEndQuantity, DiffQuantity = item.EndingQuantity - costEndQuantity });
                        }
                    }
                }
                if (costsummarys?.Count > 0)
                {
                    foreach (var item in costsummarys)
                    {
                        var filterList = stockdatas?.Where(p => p.SummaryType1Name == item.SummaryType1Name);
                        //不存在
                        if (filterList == null || filterList.Count() == 0)
                        {
                            dataList.Add(new StockCostResult { SummaryType1 = item.SummaryType1, SummaryType1Name = item.SummaryType1Name, SummaryType1FieldName = item.SummaryType1FieldName, StockEndQuantity = 0, CostEndQuantity = item.qmQuantity, DiffQuantity = -item.qmQuantity });
                        }
                    }
                }
                dataList = dataList?.Where(p => p.StockEndQuantity != 0 || p.CostEndQuantity != 0 || p.CostEndQuantity != 0)?.ToList();
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
        #endregion
    }
}
