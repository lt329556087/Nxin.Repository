using Architecture.Common.HttpClientUtil;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace FinanceManagement.Common.NewMakeVoucherCommon
{
    public class CheckenCirculationCostSummaryCommon : MakeVoucherBase
    {

        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        
        public CheckenCirculationCostSummaryCommon(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration) : base(httpClientUtil, hostCongfiguration)
        {
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil = httpClientUtil;
        }
        public override FD_SettleReceipt getVoucherList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, AuthenticationHeaderValue token)
        {
            #region 构造销售汇总表查询条件  别问我为什么。产品非得要
            List<RptSearchModel> rpts = new List<RptSearchModel>() {
             new RptSearchModel(){Text="查询时间",Value= request.Begindate+"~"+request.Enddate },
             new RptSearchModel(){Text="单位筛选",Value=model.EnterpriseName},
            };
            #endregion
            FD_SettleReceipt domain = new FD_SettleReceipt()
            {
                SettleReceipType = request.SettleReceipType,
                TicketedPointID = model.TicketedPointID,
                TransBeginDate = request.Begindate,
                TransEndDate = request.Enddate
            };
            List<FD_SettleReceiptDetailCommand> Lines = new List<FD_SettleReceiptDetailCommand>();
            List<DictionaryModel> products = new List<DictionaryModel>();
            List<DictionaryModel> chickenFarms = new List<DictionaryModel>();
            //获取禽成本项目
            Dictionary<string, string> dictList1 = GetCostFlowDict();
            //获取孵化成本项
            Dictionary<string, string> dictList2 = GetHatchCostFlowDict();
            //获取蛋成本明细项目
            Dictionary<string, string> dictList3 = GetEggCostFlowDict(token);
            bool isBreak = true;
            foreach (var item in model.Lines)
            {
                item.DebitSecFormula = item.DebitSecFormula ?? "0";
                item.CreditSecFormula = item.CreditSecFormula ?? "0";
                if (item.DebitSecFormula.Contains("2208081518000000151") || item.CreditSecFormula.Contains("2208081518000000152")) continue;
                bool isDebit = false;//借贷方标识  false为借，true为贷
                string formular = string.Empty;
                if (!string.IsNullOrEmpty(item.DebitSecFormula) && item.DebitSecFormula != "0") { isDebit = false; formular = item.DebitSecFormula; }
                if (!string.IsNullOrEmpty(item.CreditSecFormula) && item.CreditSecFormula != "0") { isDebit = true; formular = item.CreditSecFormula; }
                bool iscontions1=  base.IsContainsMethod(formular, dictList1);
                if (iscontions1)
                {
                    var chickencostResult1 = GetCheckenCirculationCostSummaryDataList(request, item, domain, token, isBreak);
                    chickencostResult1?.ForEach(summary =>
                    {
                        FD_SettleReceiptDetailCommand detail = base.NewObject<GetCheckenCirculationCostModel>(summary, item, isDebit, formular, dictList1);
                        if (item.IsProduct) detail.ProductID = summary.ProductID;
                        if (item.IsSum) detail.EnterpriseID = request.EnterpriseID;
                        Lines.Add(detail);
                    });
                }
                bool iscontions2 = base.IsContainsMethod(formular, dictList2);
                if (iscontions2)
                {
                    var chickencostResult2 = GetCheckenHatchFlowCostSummaryDataList(request, item, domain, token, isBreak);
                    chickencostResult2?.ForEach(summary =>
                    {
                        FD_SettleReceiptDetailCommand detail = base.NewObject<CheckenHatchFlowCostSummaryModel>(summary, item, isDebit, formular, dictList2);
                        if (item.IsProduct) detail.ProductID = summary.BroodProductID;
                        if (item.IsSum) detail.EnterpriseID = request.EnterpriseID;
                        Lines.Add(detail);
                    });
                }
                bool iscontions3 = base.IsContainsMethod(formular, dictList3);
                if (iscontions3)
                {
                    var chickencostResult3 = GetEggHatchFlowCostSummaryDataList(request, item, domain, token, isBreak);
                    chickencostResult3?.ForEach(summary =>
                    {
                        FD_SettleReceiptDetailCommand detail = base.NewObject<EggHatchFlowCostSummaryModel>(summary, item, isDebit, formular, dictList3);
                        if (item.IsProduct) detail.ProductID = summary.ProductID;
                        if (item.IsSum) detail.EnterpriseID = request.EnterpriseID;
                        Lines.Add(detail);
                    });
                }
                #region 封装钻取报表接口的必备参数
                if (isBreak)
                {
                    if (item.Extends?.Count > 0)
                    {
                        var productList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.商品代号).Select(s => s).ToList();
                        productList.ForEach(s =>
                        {
                            if (products.FindIndex(_ => _.id == s.Object) < 0)
                            {
                                products.Add(new DictionaryModel()
                                {
                                    id = s.Object,
                                    name = s.ObjectName,
                                });
                            }
                        });
                        var chickenFarmList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.养殖场).Select(s => s).ToList();
                        chickenFarmList.ForEach(s =>
                        {
                            if (chickenFarms.FindIndex(_ => _.id == s.Object) < 0)
                            {
                                chickenFarms.Add(new DictionaryModel()
                                {
                                    id = s.Object,
                                    name = s.ObjectName,
                                });
                            }
                        });

                    }
                }
                #endregion
                isBreak = false;
            }
            string productStr = string.Empty;
            string chickenFarmStr = string.Empty;
            if (products.Count > 0)//商品代号筛选条件
            {
                productStr = string.Join(',', products.Select(s => s.id).Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "商品代号", Value = string.Join(',', products.Select(s => s.name)) });
            }
            if (chickenFarms.Count > 0)//养殖场筛选条件
            {
                chickenFarmStr = string.Join(',', chickenFarms.Select(s => s.id).Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "养殖场", Value = string.Join(',', chickenFarms.Select(s => s.name)) });
            }

            domain.ProductLst = productStr+"~"+ chickenFarmStr;
            rpts.Add(new RptSearchModel() { Text = "汇总方式", Value = domain.SummaryTypeName });
            domain.RptSearchText = JsonConvert.SerializeObject(rpts);
            #region 解析借方合计金额和贷方合计金额
            var debit = model.Lines.LastOrDefault(s => s.DebitSecFormula.Contains("2208081518000000151"));
            var credit = model.Lines.LastOrDefault(s => s.CreditSecFormula.Contains("2208081518000000152"));
            if (debit != null)
            {
                Lines.Add(new FD_SettleReceiptDetailCommand()
                {
                    AccoSubjectCode = debit.AccoSubjectCode,
                    AccoSubjectID = debit.AccoSubjectID,
                    ReceiptAbstractID = debit.ReceiptAbstractID,
                    ReceiptAbstractName = debit.ReceiptAbstractName,
                    Debit = Lines.Sum(s => s.Credit),
                    LorR = false,
                });
                domain.Lines = Lines;
                return domain;
            }
            if (credit != null)
            {
                Lines.Add(new FD_SettleReceiptDetailCommand()
                {
                    AccoSubjectCode = credit.AccoSubjectCode,
                    AccoSubjectID = credit.AccoSubjectID,
                    ReceiptAbstractID = credit.ReceiptAbstractID,
                    ReceiptAbstractName = credit.ReceiptAbstractName,
                    Credit = Lines.Sum(s => s.Debit),
                    LorR = true,
                });
                domain.Lines = Lines;
                return domain;
            }
            #endregion
            domain.Lines = Lines;
            return domain;
        }
        private List<GetCheckenCirculationCostModel> GetCheckenCirculationCostSummaryDataList(FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, FD_SettleReceipt domain, AuthenticationHeaderValue token, bool isBreak)
        {
            try
            {
                string SummaryType = string.Empty;
                string SummaryTypeName = string.Empty;
                //李文飞接口提供
                //AuthenticationHeaderValue value = new AuthenticationHeaderValue("Bearer", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJpYXQiOjE2NjkxNjcwMzksImV4cCI6MTY2OTE3NDIzOSwidXNlcl9pZCI6IjIyOTIwNTciLCJlbnRlcnByaXNlX2lkIjoiMzI1NDA1MCIsImdyb3VwX2lkIjoiMzI1NDA1MSIsIm1lbnVfaWQiOiIyMjA4MTgwOTI4MDQwMDAwMTA5IiwiY2hpbGRfZW50ZXJwcmlzZV9pZCI6IjAiLCJwZXJtaXNzaW9ucyI6MzEsImlzcyI6ImlgbCIsImF1ZCI6InFsdy1iYXNlIn0.PUbhroGPCW_Z8qrgvW5QT8v4xzMqdGdB2l4nSmXHolO9H5r8vc-OtvATBr7h6W1orM8Ad8iDTDxmYgiXWf6UtNl_NI8-SVzneRNRQphT5jBXhuIsWfFDrdPS7Z7FX1VjJxjrv2OXbBRUy-wbgbb4st-xHJAPt7KxSXpzLsOqQ-Y5aDy58dgCvxt-JLlvhJeFLhQywlvKOmOsgTN2XJl-vXEyvnnWZX9MDNR2BTMes4wZZRxfXJi4oJr_hXTXhwquzaKqIDIm6Zp0nMKX4mxPZkC5upIuwNMbWWbEdKi_kOfZ4QLWFqMh-OtJGzvhtMGT8q2CgiYj_JwOddsoHJIJyg");
              
                List<GetCheckenCirculationCostModel> result = _httpClientUtil.PostJsonAsync<ResultModel<GetCheckenCirculationCostModel>>(_hostCongfiguration._wgUrl + "/q/reportbreed/ZqCostFlow/data", new { DataDate = new List<string>() { request.Begindate, request.Enddate } }, (a) => { a.Authorization = token; }).Result.Data;
                List<string> ProductIds = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.商品代号).Select(s => s.Object).ToList();
                List<string> ChickenFarmIds = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.养殖场).Select(s => s.Object).ToList();
                if (ProductIds.Count>0)
                {
                    result = result.Where(s => ProductIds.Contains(s.ProductID)).ToList();
                }
                if (ChickenFarmIds.Count > 0)
                {
                    result = result.Where(s => ChickenFarmIds.Contains(s.ChickenFarmID)).ToList();
                }
                if (item.IsProduct)
                {
                    if (isBreak)
                    {
                        domain.SummaryType = "ProductID";
                        domain.SummaryTypeName = "商品代号";
                    }
                }
                if ((!item.IsPerson && !item.IsMarket && !item.IsCustomer && !item.IsProduct&&!item.IsPigFram)|| item.IsSum)
                {
                    if (isBreak)
                    {
                        domain.SummaryType = "EnterpriseID";
                        domain.SummaryTypeName = "单位";
                    }
                    return dynamicGroupbySummary(result, true);
                }
                return dynamicGroupbySummary(result,  false);
            }
            catch (Exception ex)
            {
                return new List<GetCheckenCirculationCostModel>();
            }
        }
        private List<CheckenHatchFlowCostSummaryModel> GetCheckenHatchFlowCostSummaryDataList(FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, FD_SettleReceipt domain, AuthenticationHeaderValue token, bool isBreak)
        {
            try
            {
                string SummaryType = string.Empty;
                string SummaryTypeName = string.Empty;
                //李文飞接口提供
                //AuthenticationHeaderValue value = new AuthenticationHeaderValue("Bearer", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJpYXQiOjE2NjkxNjcwMzksImV4cCI6MTY2OTE3NDIzOSwidXNlcl9pZCI6IjIyOTIwNTciLCJlbnRlcnByaXNlX2lkIjoiMzI1NDA1MCIsImdyb3VwX2lkIjoiMzI1NDA1MSIsIm1lbnVfaWQiOiIyMjA4MTgwOTI4MDQwMDAwMTA5IiwiY2hpbGRfZW50ZXJwcmlzZV9pZCI6IjAiLCJwZXJtaXNzaW9ucyI6MzEsImlzcyI6ImlgbCIsImF1ZCI6InFsdy1iYXNlIn0.PUbhroGPCW_Z8qrgvW5QT8v4xzMqdGdB2l4nSmXHolO9H5r8vc-OtvATBr7h6W1orM8Ad8iDTDxmYgiXWf6UtNl_NI8-SVzneRNRQphT5jBXhuIsWfFDrdPS7Z7FX1VjJxjrv2OXbBRUy-wbgbb4st-xHJAPt7KxSXpzLsOqQ-Y5aDy58dgCvxt-JLlvhJeFLhQywlvKOmOsgTN2XJl-vXEyvnnWZX9MDNR2BTMes4wZZRxfXJi4oJr_hXTXhwquzaKqIDIm6Zp0nMKX4mxPZkC5upIuwNMbWWbEdKi_kOfZ4QLWFqMh-OtJGzvhtMGT8q2CgiYj_JwOddsoHJIJyg");

                List<CheckenHatchFlowCostSummaryModel> result = _httpClientUtil.PostJsonAsync<ResultModel<CheckenHatchFlowCostSummaryModel>>(_hostCongfiguration._wgUrl + "/q/reportbreed/ZqHatchCostFlow/data", new { DataDate = new List<string>() { request.Begindate, request.Enddate } }, (a) => { a.Authorization = token; }).Result.Data;
                List<string> ProductIds = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.商品代号).Select(s => s.Object).ToList();
                List<string> ChickenFarmIds = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.养殖场).Select(s => s.Object).ToList();
                if (ProductIds.Count > 0)
                {
                    result = result.Where(s => ProductIds.Contains(s.BroodProductID)).ToList();
                }
                if (ChickenFarmIds.Count > 0)
                {
                    result = result.Where(s => ChickenFarmIds.Contains(s.FarmID)).ToList();
                }
                if (item.IsProduct)
                {
                    if (isBreak)
                    {
                        domain.SummaryType = "ProductID";
                        domain.SummaryTypeName = "商品代号";
                    }
                }
                if ((!item.IsPerson && !item.IsMarket && !item.IsCustomer && !item.IsProduct && !item.IsPigFram) || item.IsSum)
                {
                    if (isBreak)
                    {
                        domain.SummaryType = "EnterpriseID";
                        domain.SummaryTypeName = "单位";
                    }
                    return dynamicGroupbySummary(result,  true);
                }
                return dynamicGroupbySummary(result,  false);
            }
            catch (Exception ex)
            {
                return new List<CheckenHatchFlowCostSummaryModel>();
            }
        }
        private List<EggHatchFlowCostSummaryModel> GetEggHatchFlowCostSummaryDataList(FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, FD_SettleReceipt domain, AuthenticationHeaderValue token, bool isBreak)
        {
            try
            {
                string SummaryType = string.Empty;
                string SummaryTypeName = string.Empty;
                //李文飞接口提供
                //AuthenticationHeaderValue value = new AuthenticationHeaderValue("Bearer", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJpYXQiOjE2NjkxNjcwMzksImV4cCI6MTY2OTE3NDIzOSwidXNlcl9pZCI6IjIyOTIwNTciLCJlbnRlcnByaXNlX2lkIjoiMzI1NDA1MCIsImdyb3VwX2lkIjoiMzI1NDA1MSIsIm1lbnVfaWQiOiIyMjA4MTgwOTI4MDQwMDAwMTA5IiwiY2hpbGRfZW50ZXJwcmlzZV9pZCI6IjAiLCJwZXJtaXNzaW9ucyI6MzEsImlzcyI6ImlgbCIsImF1ZCI6InFsdy1iYXNlIn0.PUbhroGPCW_Z8qrgvW5QT8v4xzMqdGdB2l4nSmXHolO9H5r8vc-OtvATBr7h6W1orM8Ad8iDTDxmYgiXWf6UtNl_NI8-SVzneRNRQphT5jBXhuIsWfFDrdPS7Z7FX1VjJxjrv2OXbBRUy-wbgbb4st-xHJAPt7KxSXpzLsOqQ-Y5aDy58dgCvxt-JLlvhJeFLhQywlvKOmOsgTN2XJl-vXEyvnnWZX9MDNR2BTMes4wZZRxfXJi4oJr_hXTXhwquzaKqIDIm6Zp0nMKX4mxPZkC5upIuwNMbWWbEdKi_kOfZ4QLWFqMh-OtJGzvhtMGT8q2CgiYj_JwOddsoHJIJyg");

                List<EggHatchFlowCostSummaryModel> result = _httpClientUtil.PostJsonAsync<ResultModel<EggHatchFlowCostSummaryModel>>(_hostCongfiguration._wgUrl + "/q/reportbreed/ZqEggCostFlow/data", new { DataDate = new List<string>() { request.Begindate, request.Enddate } }, (a) => { a.Authorization = token; }).Result.Data;
                List<string> ProductIds = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.商品代号).Select(s => s.Object).ToList();
                List<string> ChickenFarmIds = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.养殖场).Select(s => s.Object).ToList();
                if (ProductIds.Count > 0)
                {
                    result = result.Where(s => ProductIds.Contains(s.ProductID)).ToList();
                }
                if (ChickenFarmIds.Count > 0)
                {
                    result = result.Where(s => ChickenFarmIds.Contains(s.ChickenFarmID)).ToList();
                }
                //辅助项
                //if (item.IsProduct)
                //{
                //    if (isBreak)
                //    {
                //        domain.SummaryType = "ProductID";
                //        domain.SummaryTypeName = "商品代号";
                //    }
                //}
                //if ((!item.IsPerson && !item.IsMarket && !item.IsCustomer && !item.IsProduct && !item.IsPigFram) || item.IsSum)
                //{
                //    if (isBreak)
                //    {
                //        domain.SummaryType = "EnterpriseID";
                //        domain.SummaryTypeName = "单位";
                //    }
                //    return dynamicGroupbySummary(result, true);
                //}
                return dynamicGroupbySummary(result, false);
            }
            catch (Exception ex)
            {
                return new List<EggHatchFlowCostSummaryModel>();
            }
        }

        #region 各个指标对应属性
        /// <summary>
        /// 指标
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetCostFlowDict()
        {
           Dictionary<string,string>value=new Dictionary<string,string>();
            value.Add("2208151434260000102","OutCostPig3");
            value.Add("2208151434260000103","OutCostPig5");
            value.Add("2208151434260000104","OutCostPig4");
            value.Add("2208151434260000105","OutCostHen6");
            value.Add("2208151434260000106","InCostPig3");
            value.Add("2208151434260000107","InHenCost7");
            value.Add("2208151434260000108","InHenCost4");
            value.Add("2208151434260000109","InHenCost3");
            value.Add("2208151434260000110","InHenCost9_1");
            value.Add("2208151434260000111","InHenCost3_1");
            value.Add("2208151434260000112","InHenCost11");
            value.Add("2208151434260000113","OutCostPig3_1");
            value.Add("2208151434260000114","OutCostPig5_1");
            value.Add("2208151434260000115","OutCostPig4_1");
            value.Add("2208151434260000116","OutCostHen6_1");
            value.Add("2208151434260000117","InCostPig3_1");
            value.Add("2208151434260000118","InHenCost4_1");
            value.Add("2208151434260000119","OutCostChicken21");
            value.Add("2208151434260000120","OutCostChicken41");
            value.Add("2208151434260000121","OutCostChicken31");
            value.Add("2208151434260000122","OutCostChicken71");
            value.Add("2208151434260000123","OutCostChicken51");
            value.Add("2208151434260000124","InHenCost81");
            value.Add("2208151434260000125","OutCostChicken1");
            value.Add("2208151434260000126","OutCostChicken2");
            value.Add("2208151434260000127","OutCostChicken4");
            value.Add("2208151434260000128","OutCostChicken3");
            value.Add("2208151434260000129","OutCostChicken7");
            value.Add("2208151434260000130","OutCostChicken5");
            value.Add("2208151434260000131","InHenCost5");
            value.Add("2208151434260000132","InHenCost6");
            value.Add("2208151434260000133","OutHenCost31");
            value.Add("2208151434260000134","OutHenCost32");
            value.Add("2208151434260000135","OutHenCost34");
            value.Add("2208151434260000136","OutHenCost33");
            value.Add("2208151434260000137","OutHenCost37");
            value.Add("2208151434260000138","OutHenCost35");
            value.Add("2208151434260000139","OutHenCost3");
            value.Add("2208151434260000140","OutHenCost81");
            value.Add("2208151434260000141","OutHenCost82");
            value.Add("2208151434260000142","OutHenCost84");
            value.Add("2208151434260000143","OutHenCost83");
            value.Add("2208151434260000144","OutHenCost87");
            value.Add("2208151434260000145","OutHenCost85");
            value.Add("2208151434260000146","OutHenCost8");
            value.Add("2208151434260000147","OutHenCost113_1");
            value.Add("2208151434260000148","OutHenCost115_1");
            value.Add("2208151434260000149","OutHenCost114_1");
            value.Add("2208151434260000150","OutHenCost117_1");
            value.Add("2208151434260000151","InHenCost116_1");
            value.Add("2208151434260000152","InHenCostSum110_1");
            value.Add("2208151434260000153","SetStockCost1");
            value.Add("2208151434260000154","SetStockCost2");
            value.Add("2208151434260000155","SetStockCost4");
            value.Add("2208151434260000156","SetStockCost3");
            value.Add("2208151434260000157","SetStockCost7");
            value.Add("2208151434260000158","SetStockCost5");
            value.Add("2208151434260000159","SetStockCost");
            value.Add("2208151434260000160","SetStockCost_2");
            value.Add("2208151434260000161","InHenCost3_2");
            value.Add("2208151434260000162","InHenCost9_2");
            value.Add("2208151434260000163","InHenCost10_2");
            value.Add("2208151434260000164","InHenCost12");
            value.Add("2208151434260000165","OutCostPig3_2");
            value.Add("2208151434260000166","OutCostPig5_2");
            value.Add("2208151434260000167","OutCostPig4_2");
            value.Add("2208151434260000168","OutCostHen6_2");
            value.Add("2208151434260000169","InCostPig3_2");
            value.Add("2208151434260000170","InHenCost7_2");
            value.Add("2208151434260000171","InHenCost4_2");
            value.Add("2208151434260000172","InHenCost118");
            value.Add("2208151434260000173","OutHenCost150");
            value.Add("2208151434260000174","OutHenCost151");
            value.Add("2208151434260000175","InHenCost152");
            value.Add("2208151434260000176","originalValueReduce");
            value.Add("2208151434260000177","depreciationAccumulatedReduce");
            value.Add("2208151434260000178","originalValueSalesReduce");
            value.Add("2208151434260000179","depreciationAccumulatedSalesReduce");
            value.Add("2208151434260000181", "OutHouseCost1");
            value.Add("2208151434260000182", "OutHouseCost2");
            value.Add("2208151434260000183", "OutHouseCost4");
            value.Add("2208151434260000184", "OutHouseCost3");
            value.Add("2208151434260000185", "OutHouseCost6");
            value.Add("2208151434260000186", "OutHouseCost5");
            value.Add("2208151434260000187", "OutHouseCost");
            value.Add("2208151434260000188", "originalValueHouseReduce");
            value.Add("2208151434260000189", "depreciationAccumulatedHouseReduce");
            return value;
        }
        public Dictionary<string, string> GetHatchCostFlowDict()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("2208151434260000202", "BreedInCost2");
            value.Add("2208151434260000203", "BreedInCost31");
            value.Add("2208151434260000204", "BreedInCost32");
            value.Add("2208151434260000205", "BreedInCost33");
            value.Add("2208151434260000206", "BreedInCost4");
            value.Add("2208151434260000207", "BreedInCost5");
            value.Add("2208151434260000208", "BreedInCost61");
            value.Add("2208151434260000209", "BreedInCost62");
            value.Add("2208151434260000210", "BreedInCost63");
            value.Add("2208151434260000211", "BreedInCost6");
            return value;
        }
        /// <summary>
        /// 蛋成本明细表指标数据
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetEggCostFlowDict(AuthenticationHeaderValue token)
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            List<BIZ_DataDictODataEntity> list = new List<BIZ_DataDictODataEntity>();
            var result = _httpClientUtil.PostJsonAsync<List<BIZ_DataDictODataEntity>>($"{_hostCongfiguration._wgUrl}/q/reportbreed/PoultryCostProject", null,
                    (a) => { a.Authorization = token; }).Result;
            foreach (var item in result.Where(m=>m.DataDictType == "2208151434260000301"))
            {
                value.Add(item.DataDictID, item.DataDictField);
            }
            return value;
        }
        #endregion
        public List<GetCheckenCirculationCostModel> dynamicGroupbySummary(List<GetCheckenCirculationCostModel> summaryList,  bool isEnter = false)
        {
            if (isEnter)
            {
                List<GetCheckenCirculationCostModel> list = new List<GetCheckenCirculationCostModel>()
                {
                     new GetCheckenCirculationCostModel(){
                        ProductID = summaryList.FirstOrDefault()?.ProductID,
                        InQuantity1Pig5= summaryList.Sum(n => n.InQuantity1Pig5),
                        InCost1Pig5 = summaryList.Sum(n => n.InCost1Pig5),
                        OriginalValue = summaryList.Sum(n => n.OriginalValue),
                        DepreciationAccumulated = summaryList.Sum(n => n.DepreciationAccumulated),
                        InNetValue = summaryList.Sum(n => n.InNetValue),
                        InHenCost7 = summaryList.Sum(n => n.InHenCost7),
                        InHenCost7_2 = summaryList.Sum(n => n.InHenCost7_2),
                        OutCostPig3 = summaryList.Sum(n => n.OutCostPig3),
                        OutCostPig4 = summaryList.Sum(n => n.OutCostPig4),
                        OutCostPig5 = summaryList.Sum(n => n.OutCostPig5),
                        OutCostHen6 = summaryList.Sum(n => n.OutCostHen6),
                        InHenCost4 = summaryList.Sum(n => n.InHenCost4),
                        InHenQuantity3 = summaryList.Sum(n => n.InHenQuantity3),
                        InHenCost3 = summaryList.Sum(n => n.InHenCost3),
                        InHenCost9_1 = summaryList.Sum(n => n.InHenCost9_1),
                        InHenCost3_1 = summaryList.Sum(n => n.InHenCost3_1),
                        OutCostPig3_1 = summaryList.Sum(n => n.OutCostPig3_1),
                        OutCostPig4_1 = summaryList.Sum(n => n.OutCostPig4_1),
                        OutCostPig5_1 = summaryList.Sum(n => n.OutCostPig5_1),
                        OutCostHen6_1 = summaryList.Sum(n => n.OutCostHen6_1),
                        InHenCost4_1 = summaryList.Sum(n => n.InHenCost4_1),
                        OutCostChicken21 = summaryList.Sum(n => n.OutCostChicken21),
                        OutCostChicken31 = summaryList.Sum(n => n.OutCostChicken31),
                        OutCostChicken41 = summaryList.Sum(n => n.OutCostChicken41),
                        OutCostChicken71 = summaryList.Sum(n => n.OutCostChicken71),
                        InHenCost81 = summaryList.Sum(n => n.InHenCost81),
                        OutQuantityChicken1 = summaryList.Sum(n => n.OutQuantityChicken1),
                        OutCostChicken1 = summaryList.Sum(n => n.OutCostChicken1),
                        OutCostChicken2 = summaryList.Sum(n => n.OutCostChicken2),
                        OutCostChicken3 = summaryList.Sum(n => n.OutCostChicken3),
                        OutCostChicken4 = summaryList.Sum(n => n.OutCostChicken4),
                        OutCostChicken7 = summaryList.Sum(n => n.OutCostChicken7),
                        InHenCost5 = summaryList.Sum(n => n.InHenCost5),
                        InHenQuantity6 = summaryList.Sum(n => n.InHenQuantity6),
                        InHenCost6 = summaryList.Sum(n => n.InHenCost6),
                        OutQuantityPig21 = summaryList.Sum(n => n.OutQuantityPig21),
                        OutHenCost31 = summaryList.Sum(n => n.OutHenCost31),
                        OutHenCost32 = summaryList.Sum(n => n.OutHenCost32),
                        OutHenCost33 = summaryList.Sum(n => n.OutHenCost33),
                        OutHenCost34 = summaryList.Sum(n => n.OutHenCost34),
                        OutHenCost37 = summaryList.Sum(n => n.OutHenCost37),
                        OutHenCost3 = summaryList.Sum(n => n.OutHenCost3),
                        OutHenQuantity8_1 = summaryList.Sum(n => n.OutHenQuantity8_1),
                        OutHenCost81 = summaryList.Sum(n => n.OutHenCost81),
                        OutHenCost82 = summaryList.Sum(n => n.OutHenCost82),
                        OutHenCost83 = summaryList.Sum(n => n.OutHenCost83),
                        OutHenCost84 = summaryList.Sum(n => n.OutHenCost84),
                        OutHenCost87 = summaryList.Sum(n => n.OutHenCost87),
                        OutHenCost8 = summaryList.Sum(n => n.OutHenCost8),
                        SetStockQuantity = summaryList.Sum(n => n.SetStockQuantity),
                        SetStockCost1 = summaryList.Sum(n => n.SetStockCost1),
                        SetStockCost2 = summaryList.Sum(n => n.SetStockCost2),
                        SetStockCost3 = summaryList.Sum(n => n.SetStockCost3),
                        SetStockCost4 = summaryList.Sum(n => n.SetStockCost4),
                        SetStockCost7 = summaryList.Sum(n => n.SetStockCost7),
                        SetStockCost = summaryList.Sum(n => n.SetStockCost),
                        SetStockQuantity2 = summaryList.Sum(n => n.SetStockQuantity2),
                        SetStockCost_2 = summaryList.Sum(n => n.SetStockCost_2),
                        InHenQuantity3_2 = summaryList.Sum(n => n.InHenQuantity3_2),
                        InHenCost3_2 = summaryList.Sum(n => n.InHenCost3_2),
                        InHenCost9_2 = summaryList.Sum(n => n.InHenCost9_2),
                        InHenCost10_2 = summaryList.Sum(n => n.InHenCost10_2),
                        OutCostPig3_2 = summaryList.Sum(n => n.OutCostPig3_2),
                        OutCostPig4_2 = summaryList.Sum(n => n.OutCostPig4_2),
                        OutCostPig5_2 = summaryList.Sum(n => n.OutCostPig5_2),
                        OutCostHen6_2 = summaryList.Sum(n => n.OutCostHen6_2),
                        InHenCost4_2 = summaryList.Sum(n => n.InHenCost4_2),
                        InHenCost118 = summaryList.Sum(n => n.InHenCost118),
                        OutHenQuantity150 = summaryList.Sum(n => n.OutHenQuantity150),
                        OutHenCost150 = summaryList.Sum(n => n.OutHenCost150),
                        OutHenCost151 = summaryList.Sum(n => n.OutHenCost151),
                        OutHenCost152 = summaryList.Sum(n => n.OutHenCost152),
                        InHenQuantity152 = summaryList.Sum(n => n.InHenQuantity152),
                        InHenCost152 = summaryList.Sum(n => n.InHenCost152),
                        InHenQuantity11 = summaryList.Sum(n => n.InHenQuantity11),
                        InHenCost11 = summaryList.Sum(n => n.InHenCost11),
                        InHenQuantity12 = summaryList.Sum(n => n.InHenQuantity12),
                        InHenCost12 = summaryList.Sum(n => n.InHenCost12),
                        OutHenQuantity12 = summaryList.Sum(n => n.OutHenQuantity12),
                        OutHenQuantity11 = summaryList.Sum(n => n.OutHenQuantity11),
                        originalQuantityReduce = summaryList.Sum(n => n.originalQuantityReduce),
                        originalValueReduce = summaryList.Sum(n => n.originalValueReduce),
                        depreciationAccumulatedReduce = summaryList.Sum(n => n.depreciationAccumulatedReduce),
                        originalQuantitySalesReduce = summaryList.Sum(n => n.originalQuantitySalesReduce),
                        originalValueSalesReduce = summaryList.Sum(n => n.originalValueSalesReduce),
                        depreciationAccumulatedSalesReduce = summaryList.Sum(n => n.depreciationAccumulatedSalesReduce),
                        OutHenQuantity110 = summaryList.Sum(n => n.OutHenQuantity110),
                        OutHenCost110 = summaryList.Sum(n => n.OutHenCost110),
                        endOriginalValue = summaryList.Sum(n => n.endOriginalValue),
                        endDepreciationAccumulated = summaryList.Sum(n => n.endDepreciationAccumulated),
                        endNetValue = summaryList.Sum(n => n.endNetValue),
                        OutHenCost113_1 = summaryList.Sum(n => n.OutHenCost113_1),
                        OutHenCost114_1 = summaryList.Sum(n => n.OutHenCost114_1),
                        OutHenCost115_1 = summaryList.Sum(n => n.OutHenCost115_1),
                        OutHenCost117_1 = summaryList.Sum(n => n.OutHenCost117_1),
                        InHenCostSum110_1 = summaryList.Sum(n => n.InHenCostSum110_1),
                        InCostPig3 = summaryList.Sum(n => n.InCostPig3),
                        InCostPig3_1 = summaryList.Sum(n => n.InCostPig3_1),
                        OutCostChicken51 = summaryList.Sum(n => n.OutCostChicken51),
                        OutCostChicken5 = summaryList.Sum(n => n.OutCostChicken5),
                        OutHenCost35 = summaryList.Sum(n => n.OutHenCost35),
                        OutHenCost85 = summaryList.Sum(n => n.OutHenCost85),
                        InHenCost116_1 = summaryList.Sum(n => n.InHenCost116_1),
                        SetStockCost5 = summaryList.Sum(n => n.SetStockCost5),
                        InCostPig3_2 = summaryList.Sum(n => n.InCostPig3_2),
                        SHUIFEI101 = summaryList.Sum(n => n.SHUIFEI101),
                        SHUIFEI102 = summaryList.Sum(n => n.SHUIFEI102),
                        SHUIFEI103 = summaryList.Sum(n => n.SHUIFEI103),
                        SHUIFEI104 = summaryList.Sum(n => n.SHUIFEI104),
                        SHUIFEI105 = summaryList.Sum(n => n.SHUIFEI105),
                        SHUIFEI106 = summaryList.Sum(n => n.SHUIFEI106),
                        SHUIFEI107 = summaryList.Sum(n => n.SHUIFEI107),
                        SHUIFEI108 = summaryList.Sum(n => n.SHUIFEI108),
                        SHUIFEI109 = summaryList.Sum(n => n.SHUIFEI109),
                        DIANFEI101 = summaryList.Sum(n => n.DIANFEI101),
                        DIANFEI102 = summaryList.Sum(n => n.DIANFEI102),
                        DIANFEI103 = summaryList.Sum(n => n.DIANFEI103),
                        DIANFEI104 = summaryList.Sum(n => n.DIANFEI104),
                        DIANFEI105 = summaryList.Sum(n => n.DIANFEI105),
                        DIANFEI106 = summaryList.Sum(n => n.DIANFEI106),
                        DIANFEI107 = summaryList.Sum(n => n.DIANFEI107),
                        DIANFEI108 = summaryList.Sum(n => n.DIANFEI108),
                        DIANFEI109 = summaryList.Sum(n => n.DIANFEI109),
                        SHEBEIZHEJIU101 = summaryList.Sum(n => n.SHEBEIZHEJIU101),
                        SHEBEIZHEJIU102 = summaryList.Sum(n => n.SHEBEIZHEJIU102),
                        SHEBEIZHEJIU103 = summaryList.Sum(n => n.SHEBEIZHEJIU103),
                        SHEBEIZHEJIU104 = summaryList.Sum(n => n.SHEBEIZHEJIU104),
                        SHEBEIZHEJIU105 = summaryList.Sum(n => n.SHEBEIZHEJIU105),
                        SHEBEIZHEJIU106 = summaryList.Sum(n => n.SHEBEIZHEJIU106),
                        SHEBEIZHEJIU107 = summaryList.Sum(n => n.SHEBEIZHEJIU107),
                        SHEBEIZHEJIU108 = summaryList.Sum(n => n.SHEBEIZHEJIU108),
                        SHEBEIZHEJIU109 = summaryList.Sum(n => n.SHEBEIZHEJIU109),
                        RENGONG101 = summaryList.Sum(n => n.RENGONG101),
                        RENGONG102 = summaryList.Sum(n => n.RENGONG102),
                        RENGONG103 = summaryList.Sum(n => n.RENGONG103),
                        RENGONG104 = summaryList.Sum(n => n.RENGONG104),
                        RENGONG105 = summaryList.Sum(n => n.RENGONG105),
                        RENGONG106 = summaryList.Sum(n => n.RENGONG106),
                        RENGONG107 = summaryList.Sum(n => n.RENGONG107),
                        RENGONG108 = summaryList.Sum(n => n.RENGONG108),
                        RENGONG109 = summaryList.Sum(n => n.RENGONG109),
                     }
                };
                return list;
            }
            summaryList = summaryList.GroupBy(s =>s.ProductID).Select(s => new GetCheckenCirculationCostModel()
            {
                ProductID = s.Key,
                InQuantity1Pig5 = s.Sum(n => n.InQuantity1Pig5),
                InCost1Pig5 = s.Sum(n => n.InCost1Pig5),
                OriginalValue = s.Sum(n => n.OriginalValue),
                DepreciationAccumulated = s.Sum(n => n.DepreciationAccumulated),
                InNetValue = s.Sum(n => n.InNetValue),
                InHenCost7 = s.Sum(n => n.InHenCost7),
                InHenCost7_2 = s.Sum(n => n.InHenCost7_2),
                OutCostPig3 = s.Sum(n => n.OutCostPig3),
                OutCostPig4 = s.Sum(n => n.OutCostPig4),
                OutCostPig5 = s.Sum(n => n.OutCostPig5),
                OutCostHen6 = s.Sum(n => n.OutCostHen6),
                InHenCost4 = s.Sum(n => n.InHenCost4),
                InHenQuantity3 = s.Sum(n => n.InHenQuantity3),
                InHenCost3 = s.Sum(n => n.InHenCost3),
                InHenCost9_1 = s.Sum(n => n.InHenCost9_1),
                InHenCost3_1 = s.Sum(n => n.InHenCost3_1),
                OutCostPig3_1 = s.Sum(n => n.OutCostPig3_1),
                OutCostPig4_1 = s.Sum(n => n.OutCostPig4_1),
                OutCostPig5_1 = s.Sum(n => n.OutCostPig5_1),
                OutCostHen6_1 = s.Sum(n => n.OutCostHen6_1),
                InHenCost4_1 = s.Sum(n => n.InHenCost4_1),
                OutCostChicken21 = s.Sum(n => n.OutCostChicken21),
                OutCostChicken31 = s.Sum(n => n.OutCostChicken31),
                OutCostChicken41 = s.Sum(n => n.OutCostChicken41),
                OutCostChicken71 = s.Sum(n => n.OutCostChicken71),
                InHenCost81 = s.Sum(n => n.InHenCost81),
                OutQuantityChicken1 = s.Sum(n => n.OutQuantityChicken1),
                OutCostChicken1 = s.Sum(n => n.OutCostChicken1),
                OutCostChicken2 = s.Sum(n => n.OutCostChicken2),
                OutCostChicken3 = s.Sum(n => n.OutCostChicken3),
                OutCostChicken4 = s.Sum(n => n.OutCostChicken4),
                OutCostChicken7 = s.Sum(n => n.OutCostChicken7),
                InHenCost5 = s.Sum(n => n.InHenCost5),
                InHenQuantity6 = s.Sum(n => n.InHenQuantity6),
                InHenCost6 = s.Sum(n => n.InHenCost6),
                OutQuantityPig21 = s.Sum(n => n.OutQuantityPig21),
                OutHenCost31 = s.Sum(n => n.OutHenCost31),
                OutHenCost32 = s.Sum(n => n.OutHenCost32),
                OutHenCost33 = s.Sum(n => n.OutHenCost33),
                OutHenCost34 = s.Sum(n => n.OutHenCost34),
                OutHenCost37 = s.Sum(n => n.OutHenCost37),
                OutHenCost3 = s.Sum(n => n.OutHenCost3),
                OutHenQuantity8_1 = s.Sum(n => n.OutHenQuantity8_1),
                OutHenCost81 = s.Sum(n => n.OutHenCost81),
                OutHenCost82 = s.Sum(n => n.OutHenCost82),
                OutHenCost83 = s.Sum(n => n.OutHenCost83),
                OutHenCost84 = s.Sum(n => n.OutHenCost84),
                OutHenCost87 = s.Sum(n => n.OutHenCost87),
                OutHenCost8 = s.Sum(n => n.OutHenCost8),
                SetStockQuantity = s.Sum(n => n.SetStockQuantity),
                SetStockCost1 = s.Sum(n => n.SetStockCost1),
                SetStockCost2 = s.Sum(n => n.SetStockCost2),
                SetStockCost3 = s.Sum(n => n.SetStockCost3),
                SetStockCost4 = s.Sum(n => n.SetStockCost4),
                SetStockCost7 = s.Sum(n => n.SetStockCost7),
                SetStockCost = s.Sum(n => n.SetStockCost),
                SetStockQuantity2 = s.Sum(n => n.SetStockQuantity2),
                SetStockCost_2 = s.Sum(n => n.SetStockCost_2),
                InHenQuantity3_2 = s.Sum(n => n.InHenQuantity3_2),
                InHenCost3_2 = s.Sum(n => n.InHenCost3_2),
                InHenCost9_2 = s.Sum(n => n.InHenCost9_2),
                InHenCost10_2 = s.Sum(n => n.InHenCost10_2),
                OutCostPig3_2 = s.Sum(n => n.OutCostPig3_2),
                OutCostPig4_2 = s.Sum(n => n.OutCostPig4_2),
                OutCostPig5_2 = s.Sum(n => n.OutCostPig5_2),
                OutCostHen6_2 = s.Sum(n => n.OutCostHen6_2),
                InHenCost4_2 = s.Sum(n => n.InHenCost4_2),
                InHenCost118 = s.Sum(n => n.InHenCost118),
                OutHenQuantity150 = s.Sum(n => n.OutHenQuantity150),
                OutHenCost150 = s.Sum(n => n.OutHenCost150),
                OutHenCost151 = s.Sum(n => n.OutHenCost151),
                OutHenCost152 = s.Sum(n => n.OutHenCost152),
                InHenQuantity152 = s.Sum(n => n.InHenQuantity152),
                InHenCost152 = s.Sum(n => n.InHenCost152),
                InHenQuantity11 = s.Sum(n => n.InHenQuantity11),
                InHenCost11 = s.Sum(n => n.InHenCost11),
                InHenQuantity12 = s.Sum(n => n.InHenQuantity12),
                InHenCost12 = s.Sum(n => n.InHenCost12),
                OutHenQuantity12 = s.Sum(n => n.OutHenQuantity12),
                OutHenQuantity11 = s.Sum(n => n.OutHenQuantity11),
                originalQuantityReduce = s.Sum(n => n.originalQuantityReduce),
                originalValueReduce = s.Sum(n => n.originalValueReduce),
                depreciationAccumulatedReduce = s.Sum(n => n.depreciationAccumulatedReduce),
                originalQuantitySalesReduce = s.Sum(n => n.originalQuantitySalesReduce),
                originalValueSalesReduce = s.Sum(n => n.originalValueSalesReduce),
                depreciationAccumulatedSalesReduce = s.Sum(n => n.depreciationAccumulatedSalesReduce),
                OutHenQuantity110 = s.Sum(n => n.OutHenQuantity110),
                OutHenCost110 = s.Sum(n => n.OutHenCost110),
                endOriginalValue = s.Sum(n => n.endOriginalValue),
                endDepreciationAccumulated = s.Sum(n => n.endDepreciationAccumulated),
                endNetValue = s.Sum(n => n.endNetValue),
                OutHenCost113_1 = s.Sum(n => n.OutHenCost113_1),
                OutHenCost114_1 = s.Sum(n => n.OutHenCost114_1),
                OutHenCost115_1 = s.Sum(n => n.OutHenCost115_1),
                OutHenCost117_1 = s.Sum(n => n.OutHenCost117_1),
                InHenCostSum110_1 = s.Sum(n => n.InHenCostSum110_1),
                InCostPig3 = s.Sum(n => n.InCostPig3),
                InCostPig3_1 = s.Sum(n => n.InCostPig3_1),
                OutCostChicken51 = s.Sum(n => n.OutCostChicken51),
                OutCostChicken5 = s.Sum(n => n.OutCostChicken5),
                OutHenCost35 = s.Sum(n => n.OutHenCost35),
                OutHenCost85 = s.Sum(n => n.OutHenCost85),
                InHenCost116_1 = s.Sum(n => n.InHenCost116_1),
                SetStockCost5 = s.Sum(n => n.SetStockCost5),
                InCostPig3_2 = s.Sum(n => n.InCostPig3_2),
                SHUIFEI101 = s.Sum(n => n.SHUIFEI101),
                SHUIFEI102 = s.Sum(n => n.SHUIFEI102),
                SHUIFEI103 = s.Sum(n => n.SHUIFEI103),
                SHUIFEI104 = s.Sum(n => n.SHUIFEI104),
                SHUIFEI105 = s.Sum(n => n.SHUIFEI105),
                SHUIFEI106 = s.Sum(n => n.SHUIFEI106),
                SHUIFEI107 = s.Sum(n => n.SHUIFEI107),
                SHUIFEI108 = s.Sum(n => n.SHUIFEI108),
                SHUIFEI109 = s.Sum(n => n.SHUIFEI109),
                DIANFEI101 = s.Sum(n => n.DIANFEI101),
                DIANFEI102 = s.Sum(n => n.DIANFEI102),
                DIANFEI103 = s.Sum(n => n.DIANFEI103),
                DIANFEI104 = s.Sum(n => n.DIANFEI104),
                DIANFEI105 = s.Sum(n => n.DIANFEI105),
                DIANFEI106 = s.Sum(n => n.DIANFEI106),
                DIANFEI107 = s.Sum(n => n.DIANFEI107),
                DIANFEI108 = s.Sum(n => n.DIANFEI108),
                DIANFEI109 = s.Sum(n => n.DIANFEI109),
                SHEBEIZHEJIU101 = s.Sum(n => n.SHEBEIZHEJIU101),
                SHEBEIZHEJIU102 = s.Sum(n => n.SHEBEIZHEJIU102),
                SHEBEIZHEJIU103 = s.Sum(n => n.SHEBEIZHEJIU103),
                SHEBEIZHEJIU104 = s.Sum(n => n.SHEBEIZHEJIU104),
                SHEBEIZHEJIU105 = s.Sum(n => n.SHEBEIZHEJIU105),
                SHEBEIZHEJIU106 = s.Sum(n => n.SHEBEIZHEJIU106),
                SHEBEIZHEJIU107 = s.Sum(n => n.SHEBEIZHEJIU107),
                SHEBEIZHEJIU108 = s.Sum(n => n.SHEBEIZHEJIU108),
                SHEBEIZHEJIU109 = s.Sum(n => n.SHEBEIZHEJIU109),
                RENGONG101 = s.Sum(n => n.RENGONG101),
                RENGONG102 = s.Sum(n => n.RENGONG102),
                RENGONG103 = s.Sum(n => n.RENGONG103),
                RENGONG104 = s.Sum(n => n.RENGONG104),
                RENGONG105 = s.Sum(n => n.RENGONG105),
                RENGONG106 = s.Sum(n => n.RENGONG106),
                RENGONG107 = s.Sum(n => n.RENGONG107),
                RENGONG108 = s.Sum(n => n.RENGONG108),
                RENGONG109 = s.Sum(n => n.RENGONG109),
            }).ToList();
            return summaryList;
        }

        public List<CheckenHatchFlowCostSummaryModel> dynamicGroupbySummary(List<CheckenHatchFlowCostSummaryModel> summaryList,  bool isEnter = false)
        {
            if (isEnter)
            {
                List<CheckenHatchFlowCostSummaryModel> list = new List<CheckenHatchFlowCostSummaryModel>()
                {
                     new CheckenHatchFlowCostSummaryModel(){
                        BroodProductID = summaryList.FirstOrDefault()?.BroodProductID,
                        BreedInCost2= summaryList.Sum(n => n.BreedInCost2),
                        BreedInCost31 = summaryList.Sum(n => n.BreedInCost31),
                        BreedInCost32 = summaryList.Sum(n => n.BreedInCost32),
                        BreedInCost33 = summaryList.Sum(n => n.BreedInCost33),
                        BreedInCost4 = summaryList.Sum(n => n.BreedInCost4),
                        BreedInCost5 = summaryList.Sum(n => n.BreedInCost5),
                        BreedInCost61 = summaryList.Sum(n => n.BreedInCost61),
                        BreedInCost62 = summaryList.Sum(n => n.BreedInCost62),
                        BreedInCost63 = summaryList.Sum(n => n.BreedInCost63),
                        BreedInCost6 = summaryList.Sum(n => n.BreedInCost6),
                     }
                };
                return list;
            }
            summaryList = summaryList.GroupBy(s => s.BroodProductID).Select(s => new CheckenHatchFlowCostSummaryModel()
            {
                BroodProductID = s.Key,
                BreedInCost2 = s.Sum(n => n.BreedInCost2),
                BreedInCost31 = s.Sum(n => n.BreedInCost31),
                BreedInCost32 = s.Sum(n => n.BreedInCost32),
                BreedInCost33 = s.Sum(n => n.BreedInCost33),
                BreedInCost4 = s.Sum(n => n.BreedInCost4),
                BreedInCost5 = s.Sum(n => n.BreedInCost5),
                BreedInCost61 = s.Sum(n => n.BreedInCost61),
                BreedInCost62 = s.Sum(n => n.BreedInCost62),
                BreedInCost63 = s.Sum(n => n.BreedInCost63),
                BreedInCost6 = s.Sum(n => n.BreedInCost6),
            }).ToList();
            return summaryList;
        }
        public List<EggHatchFlowCostSummaryModel> dynamicGroupbySummary(List<EggHatchFlowCostSummaryModel> summaryList, bool isEnter = false)
        {
            if (isEnter)
            {
                List<EggHatchFlowCostSummaryModel> list = new List<EggHatchFlowCostSummaryModel>()
                {
                     new EggHatchFlowCostSummaryModel(){
                        ProductID = summaryList.FirstOrDefault()?.ProductID,
                        ZDInQuantity100= summaryList.Sum(n => n.ZDInQuantity100),
                        ZDInCost100= summaryList.Sum(n => n.ZDInCost100),
                        ZDInQuantity201= summaryList.Sum(n => n.ZDInQuantity201),
                        ZDInCost201= summaryList.Sum(n => n.ZDInCost201),
                        ZDInCost211= summaryList.Sum(n => n.ZDInCost211),
                        ZDInCost212= summaryList.Sum(n => n.ZDInCost212),
                        ZDInCost213= summaryList.Sum(n => n.ZDInCost213),
                        ZDInCost214= summaryList.Sum(n => n.ZDInCost214),
                        ZDInCost215= summaryList.Sum(n => n.ZDInCost215),
                        ZDInCost221= summaryList.Sum(n => n.ZDInCost221),
                        ZDInQuantity230= summaryList.Sum(n => n.ZDInQuantity230),
                        ZDInCost230= summaryList.Sum(n => n.ZDInCost230),
                        ZDInCost2301= summaryList.Sum(n => n.ZDInCost2301),
                        ZDInPrice2301= summaryList.Sum(n => n.ZDInPrice2301),
                        ZDInQuantity2302= summaryList.Sum(n => n.ZDInQuantity2302),
                        ZDInPrice2302= summaryList.Sum(n => n.ZDInPrice2302),
                        ZDInQuantity231= summaryList.Sum(n => n.ZDInQuantity231),
                        ZDInCost231= summaryList.Sum(n => n.ZDInCost231),
                        ZDInQuantity232= summaryList.Sum(n => n.ZDInQuantity232),
                        ZDInCost232= summaryList.Sum(n => n.ZDInCost232),
                        ZDInQuantity233= summaryList.Sum(n => n.ZDInQuantity233),
                        ZDInCost233= summaryList.Sum(n => n.ZDInCost233),
                        ZDInQuantity234= summaryList.Sum(n => n.ZDInQuantity234),
                        ZDInCost234= summaryList.Sum(n => n.ZDInCost234),
                        ZDInCost235= summaryList.Sum(n => n.ZDInCost235),
                        ZDInQuantity235= summaryList.Sum(n => n.ZDInQuantity235),
                        ZDInQuantity240= summaryList.Sum(n => n.ZDInQuantity240),
                        ZDInCost240= summaryList.Sum(n => n.ZDInCost240),
                        ZHIJIERENGONGJZHIGONGGONGZI101= summaryList.Sum(n => n.ZHIJIERENGONGJZHIGONGGONGZI101),
                        ZDOutQuantity301= summaryList.Sum(n => n.ZDOutQuantity301),
                        ZDOutCost301= summaryList.Sum(n => n.ZDOutCost301),
                        ZDOutQuantity302= summaryList.Sum(n => n.ZDOutQuantity302),
                        ZDOutCost302= summaryList.Sum(n => n.ZDOutCost302),
                        ZDOutQuantity303= summaryList.Sum(n => n.ZDOutQuantity303),
                        ZDOutCost303= summaryList.Sum(n => n.ZDOutCost303),
                        ZDOutQuantity304= summaryList.Sum(n => n.ZDOutQuantity304),
                        ZDOutCost304= summaryList.Sum(n => n.ZDOutCost304),
                        ZDOutQuantity305= summaryList.Sum(n => n.ZDOutQuantity305),
                        ZDOutCost305= summaryList.Sum(n => n.ZDOutCost305),
                        ZDOutQuantity310= summaryList.Sum(n => n.ZDOutQuantity310),
                        ZDOutCost310= summaryList.Sum(n => n.ZDOutCost310),
                        ZDOutQuantity401= summaryList.Sum(n => n.ZDOutQuantity401),
                        ZDOutCost401= summaryList.Sum(n => n.ZDOutCost401),
                        ZDInPrice233= summaryList.Sum(n => n.ZDInPrice233),
                        ZDInCost2331= summaryList.Sum(n => n.ZDInCost2331),
                        ZDInCost2332= summaryList.Sum(n => n.ZDInCost2332),
                        ZDOutCost402= summaryList.Sum(n => n.ZDOutCost402),
                        SHUIDIANFEI110= summaryList.Sum(n => n.SHUIDIANFEI110),
                        ZDInCost220= summaryList.Sum(n => n.ZDInCost220),
                     }
                };
                return list;
            }
            summaryList = summaryList.GroupBy(s => s.ProductID).Select(s => new EggHatchFlowCostSummaryModel()
            {
                ProductID = s.Key,
                ZDInQuantity100 = s.Sum(n => n.ZDInQuantity100),
                ZDInCost100 = s.Sum(n => n.ZDInCost100),
                ZDInQuantity201 = s.Sum(n => n.ZDInQuantity201),
                ZDInCost201 = s.Sum(n => n.ZDInCost201),
                ZDInCost211 = s.Sum(n => n.ZDInCost211),
                ZDInCost212 = s.Sum(n => n.ZDInCost212),
                ZDInCost213 = s.Sum(n => n.ZDInCost213),
                ZDInCost214 = s.Sum(n => n.ZDInCost214),
                ZDInCost215 = s.Sum(n => n.ZDInCost215),
                ZDInCost221 = s.Sum(n => n.ZDInCost221),
                ZDInQuantity230 = s.Sum(n => n.ZDInQuantity230),
                ZDInCost230 = s.Sum(n => n.ZDInCost230),
                ZDInCost2301 = s.Sum(n => n.ZDInCost2301),
                ZDInPrice2301 = s.Sum(n => n.ZDInPrice2301),
                ZDInQuantity2302 = s.Sum(n => n.ZDInQuantity2302),
                ZDInPrice2302 = s.Sum(n => n.ZDInPrice2302),
                ZDInQuantity231 = s.Sum(n => n.ZDInQuantity231),
                ZDInCost231 = s.Sum(n => n.ZDInCost231),
                ZDInQuantity232 = s.Sum(n => n.ZDInQuantity232),
                ZDInCost232 = s.Sum(n => n.ZDInCost232),
                ZDInQuantity233 = s.Sum(n => n.ZDInQuantity233),
                ZDInCost233 = s.Sum(n => n.ZDInCost233),
                ZDInQuantity234 = s.Sum(n => n.ZDInQuantity234),
                ZDInCost234 = s.Sum(n => n.ZDInCost234),
                ZDInCost235 = s.Sum(n => n.ZDInCost235),
                ZDInQuantity235 = s.Sum(n => n.ZDInQuantity235),
                ZDInQuantity240 = s.Sum(n => n.ZDInQuantity240),
                ZDInCost240 = s.Sum(n => n.ZDInCost240),
                ZHIJIERENGONGJZHIGONGGONGZI101 = s.Sum(n => n.ZHIJIERENGONGJZHIGONGGONGZI101),
                ZDOutQuantity301 = s.Sum(n => n.ZDOutQuantity301),
                ZDOutCost301 = s.Sum(n => n.ZDOutCost301),
                ZDOutQuantity302 = s.Sum(n => n.ZDOutQuantity302),
                ZDOutCost302 = s.Sum(n => n.ZDOutCost302),
                ZDOutQuantity303 = s.Sum(n => n.ZDOutQuantity303),
                ZDOutCost303 = s.Sum(n => n.ZDOutCost303),
                ZDOutQuantity304 = s.Sum(n => n.ZDOutQuantity304),
                ZDOutCost304 = s.Sum(n => n.ZDOutCost304),
                ZDOutQuantity305 = s.Sum(n => n.ZDOutQuantity305),
                ZDOutCost305 = s.Sum(n => n.ZDOutCost305),
                ZDOutQuantity310 = s.Sum(n => n.ZDOutQuantity310),
                ZDOutCost310 = s.Sum(n => n.ZDOutCost310),
                ZDOutQuantity401 = s.Sum(n => n.ZDOutQuantity401),
                ZDOutCost401 = s.Sum(n => n.ZDOutCost401),
                ZDInPrice233 = s.Sum(n => n.ZDInPrice233),
                ZDInCost2331 = s.Sum(n => n.ZDInCost2331),
                ZDInCost2332 = s.Sum(n => n.ZDInCost2332),
                ZDOutCost402 = s.Sum(n => n.ZDOutCost402),
                SHUIDIANFEI110 = s.Sum(n => n.SHUIDIANFEI110),
                ZDInCost220 = s.Sum(n => n.ZDInCost220),
            }).ToList();
            return summaryList;
        }
    }
}
