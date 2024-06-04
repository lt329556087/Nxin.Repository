using Architecture.Common.HttpClientUtil;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http.Headers;
using Serilog;
using FinanceManagement.Common.MonthEndCheckout;

namespace FinanceManagement.Common.NewMakeVoucherCommon
{
    /// <summary>
    /// 猪成本
    /// </summary>
    public class PigCostSummaryCommon : MakeVoucherBase
    {

        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        public PigCostSummaryCommon(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration) : base(httpClientUtil, hostCongfiguration)
        {
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil = httpClientUtil;
        }
        public override FD_SettleReceipt getVoucherList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, AuthenticationHeaderValue token)
        {
            #region 构造查询条件  别问我为什么。产品非得要
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
            var persons = GetPerson(new DropSelectSearch() { EnterpriseID = request.EnterpriseID });
            List<DictionaryModel> expenseNatures = new List<DictionaryModel>();
            List<FD_SettleReceiptDetailCommand> Lines = new List<FD_SettleReceiptDetailCommand>();
            List<DictionaryModel> products = new List<DictionaryModel>();
            List<DictionaryModel> markets = new List<DictionaryModel>();
            List<DictionaryModel> pigfarms = new List<DictionaryModel>();
            //获取成本项目
            List<DictinonaryModel> dictList = GetDict();
            //获取损益结转公式
            Dictionary<string, string> lossDictList = GetLossIncomDict();
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
                //取费用表逻辑
                if (formular.Contains("[2201101409320000151]") || formular.Contains("[2201101409320000152]"))
                {
                    List<ExpenseDetailsReportDto> shareCostResult = new List<ExpenseDetailsReportDto>();
                    if (formular.Contains("[2201101409320000151]"))
                    {
                        shareCostResult = GetShareCostDataList(request, item, token, 1);
                    }
                    if (formular.Contains("[2201101409320000152]"))
                    {
                        shareCostResult = GetShareCostDataList(request, item, token, 2);
                    }
                    formular = formular.Replace("[2201101409320000151]", "").Replace("[2201101409320000152]", "");
                    //获取公式项
                    var formulaList = item.Formulas.Where(s => !string.IsNullOrEmpty(s.FormulaID)).Select(s => s.FormulaID);
                    //获取公式对应的数据
                    var shareCostByFormula = shareCostResult.Where(s => formulaList.Contains(s.CostProjectID)).ToList();
                    if (!item.IsSum)
                    {
                        if (item.IsPerson)
                        {
                            //获取发生人员的信息
                            var groupbypersondatas = shareCostByFormula.GroupBy(s => new { s.DeptOrOthersID, s.DeptExtendID });
                            foreach (var group in groupbypersondatas)
                            {
                                FD_SettleReceiptDetailCommand detail = new FD_SettleReceiptDetailCommand()
                                {
                                    AccoSubjectCode = item.AccoSubjectCode,
                                    AccoSubjectID = item.AccoSubjectID,
                                    ReceiptAbstractID = item.ReceiptAbstractID,
                                    ReceiptAbstractName = item.ReceiptAbstractName,
                                    LorR = isDebit,
                                };
                                string formularRep = formular.Replace("×", "*").Replace("÷", "/");
                                foreach (var formula in formulaList)
                                {
                                    if (!string.IsNullOrEmpty(formularRep))
                                    {
                                        var value = shareCostByFormula.Where(s => s.CostProjectID == formula && s.DeptOrOthersID == group.Key.DeptOrOthersID)?.Sum(s => s.ExpenseAmount).ToString() ?? "0";
                                        formularRep = formularRep.Replace("[" + formula + "]", value);
                                    }
                                }
                                if (isDebit)
                                {
                                    if (formularRep.Contains("+") || formularRep.Contains("-") || formularRep.Contains("*") || formularRep.Contains("/")) { detail.Credit = this.CalculateValue(formularRep); }
                                    else { decimal result = 0; decimal.TryParse(formularRep, out result); detail.Credit = result; }
                                }
                                else
                                {
                                    if (formularRep.Contains("+") || formularRep.Contains("-") || formularRep.Contains("*") || formularRep.Contains("/")) { detail.Debit = this.CalculateValue(formularRep); }
                                    else { decimal result = 0; decimal.TryParse(formularRep, out result); detail.Debit = result; }
                                }
                                detail.PersonID = item.IsPerson ? persons.Where(s => s.UserID.ToString() == group.Key.DeptOrOthersID)?.FirstOrDefault()?.PersonID : "0";
                                detail.MarketID = item.IsMarket ? group.Key.DeptExtendID : "0";
                                Lines.Add(detail);
                            }
                        }
                        if (item.IsMarket && !item.IsPerson)
                        {
                            //获取发生部门的信息
                            var groupbymarketdatas = shareCostByFormula.GroupBy(s => new { s.DeptExtendID });
                            foreach (var group in groupbymarketdatas)
                            {
                                FD_SettleReceiptDetailCommand detail = new FD_SettleReceiptDetailCommand()
                                {
                                    AccoSubjectCode = item.AccoSubjectCode,
                                    AccoSubjectID = item.AccoSubjectID,
                                    ReceiptAbstractID = item.ReceiptAbstractID,
                                    ReceiptAbstractName = item.ReceiptAbstractName,
                                    LorR = isDebit,
                                };
                                string formularRep = formular.Replace("×", "*").Replace("÷", "/");
                                foreach (var formula in formulaList)
                                {
                                    if (!string.IsNullOrEmpty(formularRep))
                                    {
                                        var value = shareCostByFormula.Where(s => s.CostProjectID == formula && s.DeptExtendID == group.Key.DeptExtendID)?.Sum(s => s.ExpenseAmount).ToString() ?? "0";
                                        formularRep = formularRep.Replace("[" + formula + "]", value);
                                    }
                                }
                                if (isDebit)
                                {
                                    if (formularRep.Contains("+") || formularRep.Contains("-") || formularRep.Contains("*") || formularRep.Contains("/")) { detail.Credit = this.CalculateValue(formularRep); }
                                    else { decimal result = 0; decimal.TryParse(formularRep, out result); detail.Credit = result; }
                                }
                                else
                                {
                                    if (formularRep.Contains("+") || formularRep.Contains("-") || formularRep.Contains("*") || formularRep.Contains("/")) { detail.Debit = this.CalculateValue(formularRep); }
                                    else { decimal result = 0; decimal.TryParse(formularRep, out result); detail.Debit = result; }
                                }
                                detail.MarketID = group.Key.DeptExtendID;
                                Lines.Add(detail);
                            }
                        }
                    }
                    else
                    {
                        FD_SettleReceiptDetailCommand detail = new FD_SettleReceiptDetailCommand()
                        {
                            AccoSubjectCode = item.AccoSubjectCode,
                            AccoSubjectID = item.AccoSubjectID,
                            ReceiptAbstractID = item.ReceiptAbstractID,
                            ReceiptAbstractName = item.ReceiptAbstractName,
                            LorR = isDebit,
                        };
                        string formularRep = formular.Replace("×", "*").Replace("÷", "/");
                        foreach (var formula in formulaList)
                        {
                            if (!string.IsNullOrEmpty(formularRep))
                            {
                                var value = shareCostByFormula.Where(s => s.CostProjectID == formula)?.Sum(s => s.ExpenseAmount).ToString() ?? "0";
                                formularRep = formularRep.Replace("[" + formula + "]", value);
                            }
                        }
                        if (isDebit)
                        {
                            if (formularRep.Contains("+") || formularRep.Contains("-") || formularRep.Contains("*") || formularRep.Contains("/")) { detail.Credit = this.CalculateValue(formularRep); }
                            else { decimal result = 0; decimal.TryParse(formularRep, out result); detail.Credit = result; }
                        }
                        else
                        {
                            if (formularRep.Contains("+") || formularRep.Contains("-") || formularRep.Contains("*") || formularRep.Contains("/")) { detail.Debit = this.CalculateValue(formularRep); }
                            else { decimal result = 0; decimal.TryParse(formularRep, out result); detail.Debit = result; }
                        }
                        detail.EnterpriseID = request.EnterpriseID;
                        Lines.Add(detail);
                    }
                    #region 封装钻取接口的必备参数
                    if (item.Extends?.Count > 0 && isBreak)
                    {
                        var natures = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.费用性质).Select(s => s).ToList();
                        natures.ForEach(s =>
                        {
                            if (expenseNatures.FindIndex(_ => _.id == s.Object) < 0)
                            {
                                expenseNatures.Add(new DictionaryModel()
                                {
                                    id = s.Object,
                                    name = s.ObjectName,
                                });
                            }
                        });
                    }
                    #endregion
                }
                else if (formular.Contains("2211221508050000102") ||formular.Contains("2211221508050000103"))
                {
                    reqUrl = _hostCongfiguration.ReportService + "/api/RptSettleReceipt/GetSettleReceiptReport";
                    //损益结转
                    List<MySettleReceiptDataResult> lossIncomes = GetLossIncomeDataList(model, request, item, domain, isBreak);//获取会计辅助账数据
                    lossIncomes?.ForEach(summary =>
                    {
                        var detail = SetResultModel(summary, item, isDebit, formular, lossDictList, request);
                        Lines.Add(detail);
                    });
                }
                else
                {
                    var pigcostResult = GetPigCostSummaryDataList(request, item, domain, isBreak, formular, token);
                    pigcostResult?.ForEach(summary =>
                    {
                        FD_SettleReceiptDetailCommand detail = base.NewObjectExtend<PigCirculationSummary>(summary, item, isDebit, formular, dictList);
                        if (item.IsProduct) detail.ProductID = summary.ProductId;
                        if (item.IsMarket) detail.MarketID = summary.DeptId;
                        if (item.IsCustomer) detail.CustomerID = summary.FarmerID;
                        if (item.IsSum) detail.EnterpriseID = request.EnterpriseID;
                        Lines.Add(detail);
                    });
                    #region 封装钻取销售汇总表接口的必备参数
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
                            var marketList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.部门).Select(s => s).ToList();
                            marketList.ForEach(s =>
                            {
                                if (markets.FindIndex(_ => _.id == s.Object) < 0)
                                {
                                    markets.Add(new DictionaryModel()
                                    {
                                        id = s.Object,
                                        name = s.ObjectName,
                                    });
                                }
                            });
                            var pigfarmList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.猪场).Select(s => s).ToList();
                            pigfarmList.ForEach(s =>
                            {
                                if (pigfarms.FindIndex(_ => _.id == s.Object) < 0)
                                {
                                    pigfarms.Add(new DictionaryModel()
                                    {
                                        id = s.Object,
                                        name = s.ObjectName,
                                    });
                                }
                            });
                        }
                    }
                    #endregion
                }
                isBreak = false;
            }
            string productStr = string.Empty;
            string marketStr = string.Empty;
            string pigfarmStr = string.Empty;
            string natureStr = string.Empty;
            if (products.Count > 0)//商品代号筛选条件
            {
                productStr = string.Join(',', products.Select(s => s.id).Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "商品代号", Value = string.Join(',', products.Select(s => s.name)) });
            }
            if (markets.Count > 0)//部门
            {
                marketStr = string.Join(',', markets.Select(s => s.id).Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "部门", Value = string.Join(',', markets.Select(s => s.name)) });
            }
            if (pigfarms.Count > 0)//猪场
            {
                pigfarmStr = string.Join(',', pigfarms.Select(s => s.id).Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "猪场", Value = string.Join(',', pigfarms.Select(s => s.name)) });
            }
            if (expenseNatures.Count > 0)
            {
                natureStr = JsonConvert.SerializeObject(expenseNatures.Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "费用性质", Value = string.Join(',', expenseNatures.Select(s => s.name)) });
            }
            domain.ProductLst = productStr + "~" + marketStr+ "~" + pigfarmStr +"~" + natureStr;
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
        #region 费用报表
        private List<ExpenseDetailsReportDto> GetShareCostDataList(FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, AuthenticationHeaderValue token, int type)
        {
            try
            {
                var costWhere = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.费用性质).Select(s => s.Object).ToList();
                if (type == 1)
                {
                    //徐敬昆
                    var result = _httpClientUtil.PostJsonAsync<ResultModel<ExpenseDetailsReportDto>>(_hostCongfiguration._wgUrl + "/cost/report/ExpenseDetailsReport/Data", new { EnterpriseID = request.EnterpriseID, DateRange = request.Begindate + "&" + request.Enddate }).Result;
                    if (result.Code == 0)
                    {
                        List<ExpenseDetailsReportDto> data = new List<ExpenseDetailsReportDto>();
                        if (costWhere.Count > 0)
                        {
                            data = result.Data.Where(s => costWhere.Contains(s.ExpenseNature)).ToList();
                        }
                        else
                        {
                            data = result.Data;
                        }
                        foreach (var ss in data)
                        {
                            if (ss.CollectionType == "202202111355001102")
                            {
                                ss.DeptExtendID = ss.DeptOrOthersID;
                            }
                        }
                        return data;
                    }
                    else
                    {
                        return new List<ExpenseDetailsReportDto>();
                    }
                }
                if (type == 2)
                {
                    //石燕成
                    //该表不允许选项“人员”辅助项
                    var result = _httpClientUtil.PostJsonAsync<ResultModel<ExpenseDetailsReportDto>>(_hostCongfiguration._wgUrl + "/cost/report/ExpensesDetailNot/expenses", new { EnterpriseID = request.EnterpriseID, DataDate = request.Begindate + "&" + request.Enddate }).Result;
                    if (result.Code == 0)
                    {
                        List<ExpenseDetailsReportDto> returnlist = new List<ExpenseDetailsReportDto>();
                        List<ExpenseDetailsReportDto> resultlist = new List<ExpenseDetailsReportDto>();
                        if (costWhere.Count > 0)
                        {
                            resultlist = result.Data.Where(s => costWhere.Contains(s.Nature)).ToList();
                        }
                        else
                        {
                            resultlist = result.Data;
                        }
                        foreach (var dto in resultlist)
                        {
                            returnlist.Add(new ExpenseDetailsReportDto()
                            {
                                CostProjectTypeID = dto.CostProjectTypeID,
                                CostProjectTypeName = dto.CostProjectTypeName,
                                CostProjectID = dto.CostProjectID,
                                CostProjectCode = dto.CostProjectCode,
                                CostProjectName = dto.CostProjectName,
                                CollectionType = "202202111355001102",
                                CollectionTypeName = "部门",
                                PigFarmID = dto.PigFarmID,
                                PigFarmName = dto.PigFarmName,
                                DeptOrOthersID = dto.MarketID,
                                DeptOrOthersName = dto.MarketName,
                                DeptExtendID = dto.MarketID,
                                MarketName = dto.MarketName,
                                ExpenseAmount = dto.Amount,
                                ExpenseNature = dto.Nature,
                                ExpenseNatureName = dto.NatureName,
                                ProductionTeamID = dto.ProductLineID,
                                ProductionTeamName = dto.ProductLineName,
                            });
                        }
                        return returnlist;
                    }
                    else
                    {
                        return new List<ExpenseDetailsReportDto>();
                    }
                }
                return new List<ExpenseDetailsReportDto>();
            }
            catch (Exception ex)
            {
                return new List<ExpenseDetailsReportDto>();
            }
        }
        #endregion
        #region 猪成本汇总表
        private List<PigCirculationSummary> GetPigCostSummaryDataList(FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, FD_SettleReceipt domain, bool isBreak,string formular, AuthenticationHeaderValue token)
        {
            try
            {
                var formulaList = item.Formulas.Select(s => s.FormulaID).ToArray();
                string SummaryType = string.Empty;
                string SummaryTypeName = string.Empty;
                var pigRequest = getPigSearchModel(request, item, domain, isBreak);
                List<PigCirculation> result1 = new List<PigCirculation>();
                List<SucklingPigCirculation> result2 = new List<SucklingPigCirculation>();
                List<GrowCirculation> result3 = new List<GrowCirculation>();
                List<FallbackPigCirculation> result4 = new List<FallbackPigCirculation>();
                List<FeedCostCirculation> result5 = new List<FeedCostCirculation>();
                List<BreederSummaryResultModel> result6 = new List<BreederSummaryResultModel>();
                //AuthenticationHeaderValue value = new AuthenticationHeaderValue("Bearer", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJpYXQiOjE2NzgzNDExNjEsImV4cCI6MTY3ODM0ODM2MSwidXNlcl9pZCI6IjIyOTIwNTciLCJlbnRlcnByaXNlX2lkIjoiMzI3MjI4MyIsImdyb3VwX2lkIjoiMzIxMzAzOSIsIm1lbnVfaWQiOiIyMjA4MTgwOTI4MDQwMDAwMTA5IiwiY2hpbGRfZW50ZXJwcmlzZV9pZCI6IjAiLCJwZXJtaXNzaW9ucyI6MzEsImlzcyI6ImlgbCIsImF1ZCI6InFsdy1iYXNlIn0.iInEIxEhz6Q3W2iwRXVdacLJTpVJlPXVN-4XA2fkbV9lMhpE9LnRWJCrw4KxPnE-KE-zwhPy_qO2yz33pBG8X9aOP2xsRNdX8_FYfsaiwYxxurKsz2MYwSzNcsT9dXcdPF7Nj7kJB8sPgo6boUFQigbL_dwSqmMLCtb0S-gac6GuoN-ZpxunkyGB5783-esyb7PjkP93IXhWORROIu7pg2leKxYUzgRa5SVAF5GO4ZWeHLY1eF3s26pCdxqyowDpyEBx3P27N0Vk-_Flmhl-NozSgPfzE3XwcRXLAwXhcxOaeSn3xu-Oh8Zg8hk-fpORCDTo7m9JC0JRm6YMSYPa_w");
                //种猪流转表
                List<string> KeyList1= GetReprotKey1();
                bool iscontions1= IsContainsMethod(formular, KeyList1);
                if (iscontions1)
                    result1 = _httpClientUtil.PostJsonAsync<ResultModel<PigCirculation>>(_hostCongfiguration._wgUrl + "/cost/report/PigCirculation/finance/data", pigRequest, (a) => { a.Authorization = token; }).Result.Data;
                //仔猪流转表
                List<string> KeyList2 = GetReprotKey2();
                bool iscontions2 = IsContainsMethod(formular, KeyList2);
                if (iscontions2)
                    result2 = _httpClientUtil.PostJsonAsync<ResultModel<SucklingPigCirculation>>(_hostCongfiguration._wgUrl + "/cost/report/SucklingPigCirculation/finance/data", pigRequest, (a) => { a.Authorization = token; }).Result.Data;
                //肥猪流转表
                List<string> KeyList3 = GetReprotKey3();
                bool iscontions3 = IsContainsMethod(formular, KeyList3);
                if (iscontions3)
                    result3 = _httpClientUtil.PostJsonAsync<ResultModel<GrowCirculation>>(_hostCongfiguration._wgUrl + "/cost/report/GrowCirculation/finance/data", pigRequest, (a) => { a.Authorization = token; }).Result.Data;
                //后备猪流转表
                List<string> KeyList4 = GetReprotKey4();
                bool iscontions4 = IsContainsMethod(formular, KeyList4);
                if (iscontions4)
                    result4 = _httpClientUtil.PostJsonAsync<ResultModel<FallbackPigCirculation>>(_hostCongfiguration._wgUrl + "/cost/report/FallbackPigCirculation/finance/data", pigRequest, (a) => { a.Authorization = token; }).Result.Data;
                //饲养成本明细表  https://confluence.nxin.com/pages/viewpage.action?pageId=65050999
                List<string> KeyList5 = GetReprotKey5();
                bool iscontions5 = IsContainsMethod(formular, KeyList5);
                if (iscontions5)
                    result5 = _httpClientUtil.PostJsonAsync<ResultModel<FeedCostCirculation>>(_hostCongfiguration._wgUrl + "/cost/report/DbnFeedCost/Finance/Data", pigRequest, (a) => { a.Authorization = token; }).Result.Data;
                
                //客/商辅助项只作用于养户接口**********
                if (item.IsCustomer)
                {
                    pigRequest.Summary.Add("FarmerID");
                    pigRequest.SummaryName.Add("养户");
                    domain.SummaryType = string.Join(',', pigRequest.Summary);
                    domain.SummaryTypeName = string.Join(',', pigRequest.SummaryName);

                }
                //养户成本汇总表
                List<string> KeyList6 = GetReprotKey6();
                bool iscontions6 = IsContainsMethod(formular, KeyList6);
                if (iscontions6)
                {
                    //猪场分录不作用于养户成本表，需清空
                    pigRequest.PigFarmIdList = new List<string>();
                    result6 = _httpClientUtil.PostJsonAsync<ResultModel<BreederSummaryResultModel>>(_hostCongfiguration._wgUrl + "/cost/report/YhPigCostSummary/Finance/Data", pigRequest, (a) => { a.Authorization = token; }).Result.Data;
                }
                var summaryList = TransformationSummary(result1, result2, result3, result4, result5, result6);
                #region dynamicGroupby
                GroupModel groupModel = new GroupModel();
                if (item.IsMarket)
                {
                    groupModel.DeptId = true;
                }
                if (item.IsProduct)
                {
                    groupModel.ProductId = true;
                }
                if (item.IsCustomer)
                {
                    groupModel.FarmerID = true;
                }
                //如果辅助项没选部门或商品的话按单位汇总
                if (!item.IsProduct && !item.IsMarket && !item.IsCustomer)
                {
                    var r = dynamicGroupbySummary(summaryList, groupModel, true);
                    return r;
                }
                #endregion
                var data = dynamicGroupbySummary(summaryList, groupModel, false);
                return data;
            }
            catch (Exception ex)
            {
                Log.Logger.Error("日志 GetPigCostSummaryDataList :" + ex.ToString() + "\n param=" + JsonConvert.SerializeObject(item));
                return new List<PigCirculationSummary>();
            }
        }
        public PigSearchModel getPigSearchModel(FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, FD_SettleReceipt domain, bool isBreak)
        {
            PigSearchModel pigSearchModel = new PigSearchModel()
            {
                EndDate = Convert.ToDateTime(request.Enddate),
                StartDate = Convert.ToDateTime(request.Begindate),
                EnterpriseId = request.EnterpriseID,
                ProductIdList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.商品代号).Select(s => s.Object).ToList(),
                DeptIdList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.部门).Select(s => s.Object).ToList(),
                PigTypeIdList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.猪只类型).Select(s => s.Object).ToList(),
                PigFarmIdList= item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.猪场).Select(s => s.Object).ToList(),
            };
            if (item.IsMarket)
            {
                pigSearchModel.Summary.Add("DeptId");
                pigSearchModel.SummaryName.Add("部门");
            }
            if (item.IsProduct)
            {
                pigSearchModel.Summary.Add("ProductId");
                pigSearchModel.SummaryName.Add("商品代号");
            }
            if (isBreak)
            {
                domain.SummaryType = string.Join(',', pigSearchModel.Summary);
                domain.SummaryTypeName = string.Join(',', pigSearchModel.SummaryName);
            }
            return pigSearchModel;
        }
        #endregion
        #region 科目余额表
        private List<MySettleReceiptDataResult> GetLossIncomeDataList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, FD_SettleReceipt domain, bool isBreak)
        {
            try
            {
                #region 构造接口参数
                //参数：单位 日期 单据字 会计科目-明细表
                var data = new MySettleReceiptRequest()
                {
                    Begindate = request.Begindate,
                    Enddate = request.Enddate,
                    EnterpriseList = model.EnterpriseID,
                    TicketedPointID = model.TicketedPointID,
                    AccoSubjectIDLst = item.AccoSubjectCode,
                    reportType = "1",
                    SettleReceipType = "-1",
                    PaymentTypeID = "-1",
                    FirstProject = "-1",
                    SecondProject = "-1",
                    OnlyCombineEnte = false,
                    DataSource = 0,
                    SummaryType1 = "sg1.SettleSummaryGroupName",
                    IsGroupByEnteCate = false,
                    SummaryT1Rank = "-1",
                    SummaryT2Rank = "-1",
                    SummaryT3Rank = "-1",
                    OwnEntes = new List<string>() { model.EnterpriseID },
                    CanWatchEntes = new List<string>() { model.EnterpriseID },
                    GroupID = Convert.ToInt64(request.GroupID),
                    EnteID = Convert.ToInt64(request.EnterpriseID),
                    MenuParttern = "0",
                    Boid = model.OwnerID.ToString()
                };
                #endregion
                ResultModel<MySettleReceiptDataResult> result = base.postActionByUrl<ResultModel<MySettleReceiptDataResult>, MySettleReceiptRequest>(data);
                List<MySettleReceiptDataResult> resultList = result.ResultState ? result.Data : new List<MySettleReceiptDataResult>();

                if (item.IsSum)
                {
                    return new List<MySettleReceiptDataResult>()
                    {
                        new MySettleReceiptDataResult()
                        {
                             Credit=(decimal)resultList?.Sum(s=>s.Credit),
                             Debit=(decimal)resultList?.Sum(s=>s.Debit)
                        }
                    };
                }
                LossGroupModel groupModel = new LossGroupModel();

                if (item.IsProduct)
                {
                    groupModel.ProductId = true;
                }
                if (item.IsProject)
                {
                    groupModel.ProjectId = true;
                }
                if (item.IsPerson)
                {
                    groupModel.PersonId = true;
                }
                if (item.IsCustomer)
                {
                    groupModel.CustomerId = true;
                }
                if (item.IsMarket)
                {
                    groupModel.DeptId = true;
                }
                foreach (var item2 in resultList)
                {
                    if (string.IsNullOrEmpty(item2.ProductID))
                    {
                        item2.ProductID = "0";
                    }
                    if (string.IsNullOrEmpty(item2.ProjectID))
                    {
                        item2.ProjectID = "0";
                    }
                    if (string.IsNullOrEmpty(item2.PersonID))
                    {
                        item2.PersonID = "0";
                    }
                    if (string.IsNullOrEmpty(item2.CustomerID))
                    {
                        item2.CustomerID = "0";
                    }
                    if (string.IsNullOrEmpty(item2.MarketID))
                    {
                        item2.MarketID = "0";
                    }
                }
                resultList = dynamicGroupbySummary(resultList, groupModel);
                return resultList;
            }
            catch (Exception ex)
            {
                return new List<MySettleReceiptDataResult>();
            }
        }
        public FD_SettleReceiptDetailCommand SetResultModel(MySettleReceiptDataResult summary, FM_NewCarryForwardVoucherDetailODataEntity item, bool isDebit, string formular, Dictionary<string, string> dictList, FM_CarryForwardVoucherSearchCommand request)
        {
            FD_SettleReceiptDetailCommand detail = base.NewObject<MySettleReceiptDataResult>(summary, item, isDebit, formular, dictList);
            if (item.IsPerson) detail.PersonID = summary.PersonID;
            if (item.IsMarket) detail.MarketID = summary.MarketID;
            if (item.IsCustomer) detail.CustomerID = summary.CustomerID;
            if (item.IsProject) detail.ProjectID = summary.ProjectID;
            if (item.IsProduct) detail.ProductID = summary.ProductID;
            if (item.IsSum) detail.EnterpriseID = request.EnterpriseID;
            return detail;
        }
        public List<MySettleReceiptDataResult> dynamicGroupbySummary(List<MySettleReceiptDataResult> summaryList, LossGroupModel groupModel)
        {
            summaryList = summaryList.GroupBy(s => groupModel.GroupBy(s)).Select(s => new MySettleReceiptDataResult()
            {
                MarketID = s.Key.DeptId,
                CustomerID = s.Key.CustomerId,
                PersonID = s.Key.PersonId,
                ProjectID = s.Key.ProjectId,
                ProductID = s.Key.ProductId,
                AccoSubjectCode = s.FirstOrDefault()?.AccoSubjectCode,
                Debit = s.Sum(n => n.Debit),
                Credit = s.Sum(n => n.Credit),
                Balance = s.Sum(n => n.Balance),
            }).ToList();
            return summaryList;
        }
        public Dictionary<string, string> GetLossIncomDict()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("2211221508050000102", "Debit");//本期借方
            value.Add("2211221508050000103", "Credit");//本期贷方
            return value;
        }
        #endregion

        #region 各个指标对应属性
        /// <summary>
        /// 指标
        /// </summary>
        /// <returns></returns>
        public List<DictinonaryModel> GetDict()
        {
            List<DictinonaryModel> value = new List<DictinonaryModel>();
            value.Add(new DictinonaryModel("2211221408050000102", "FallbackInAmount1"));
            value.Add(new DictinonaryModel("2211221408050000202", "InsidePurchaseInAmount1"));
            value.Add(new DictinonaryModel("2211221408050000302", "OutPurchaseInAmount1"));
            value.Add(new DictinonaryModel("2211221408050000402", "ReadyBreedingInFeed1"));
            value.Add(new DictinonaryModel("2211221408050000502", "ReadyBreedingInVeterinaryDrug1"));
            value.Add(new DictinonaryModel("2211221408050000602", "ReadyBreedingInVaccin1"));
            value.Add(new DictinonaryModel("2211221408050000702", "ReadyBreedingInOtherMaterial1"));
            value.Add(new DictinonaryModel("2211221408050000802", "ReadyBreedingInPigTransferValue1"));
            value.Add(new DictinonaryModel("2211221408050000902", "ReadyBreedingInCost1"));
            value.Add(new DictinonaryModel("2211221408050001002", "ReadyBreedingInPigDeathValue1"));
            value.Add(new DictinonaryModel("2211221408050001102", "ReadyBreedingInPigBatchDeath1"));
            value.Add(new DictinonaryModel("2211221408050001202", "MissBreedingInFeed1"));
            value.Add(new DictinonaryModel("2211221408050001302", "MissBreedingInVeterinaryDrug1"));
            value.Add(new DictinonaryModel("2211221408050001402", "MissBreedingInVaccin1"));
            value.Add(new DictinonaryModel("2211221408050001502", "MissBreedingInOtherMaterial1"));
            value.Add(new DictinonaryModel("2211221408050001602", "MissBreedingInPigTransferValue1"));
            value.Add(new DictinonaryModel("2211221408050001702", "MissBreedingInSemen1"));
            value.Add(new DictinonaryModel("2211221408050001802", "MissBreedingInCost1"));
            value.Add(new DictinonaryModel("2211221408050001902", "MissBreedingInPigDeathValue1"));
            value.Add(new DictinonaryModel("2211221408050002002", "MissBreedingInPigBatchDeath1"));
            value.Add(new DictinonaryModel("2211221408050002102", "AllocateInAmount1"));
            value.Add(new DictinonaryModel("2211221408050002202", "AllocateInTotalDepreciation1"));
            value.Add(new DictinonaryModel("2211221408050002302", "AllocateInFeed1"));
            value.Add(new DictinonaryModel("2211221408050002402", "AllocateInVeterinaryDrug1"));
            value.Add(new DictinonaryModel("2211221408050002502", "AllocateInVaccin1"));
            value.Add(new DictinonaryModel("2211221408050002602", "AllocateInOtherMaterial1"));
            value.Add(new DictinonaryModel("2211221408050002702", "AllocateInPigTransferValue1"));
            value.Add(new DictinonaryModel("2211221408050002802", "AllocateInSemen1"));
            value.Add(new DictinonaryModel("2211221408050002902", "AllocateInCost1"));
            value.Add(new DictinonaryModel("2211221408050003002", "AllocateInPigDeathValue1"));
            value.Add(new DictinonaryModel("2211221408050003102", "AllocateInPigBatchDeath1"));
            value.Add(new DictinonaryModel("2211221408050003202", "BatchInAmount1"));
            value.Add(new DictinonaryModel("2211221408050003302", "BatchInTotalDepreciation1"));
            value.Add(new DictinonaryModel("2211221408050003402", "BatchInFeed1"));
            value.Add(new DictinonaryModel("2211221408050003502", "BatchInVeterinaryDrug1"));
            value.Add(new DictinonaryModel("2211221408050003602", "BatchInVaccin1"));
            value.Add(new DictinonaryModel("2211221408050003702", "BatchInOtherMaterial1"));
            value.Add(new DictinonaryModel("2211221408050003802", "BatchInPigTransferValue1"));
            value.Add(new DictinonaryModel("2211221408050003902", "BatchInSemen1"));
            value.Add(new DictinonaryModel("2211221408050004002", "BatchInCost1"));
            value.Add(new DictinonaryModel("2211221408050004102", "BatchInPigDeathValue1"));
            value.Add(new DictinonaryModel("2211221408050004202", "BatchInPigBatchDeath1"));
            value.Add(new DictinonaryModel("2211221408050004302", "SaleInsideInFeed1"));
            value.Add(new DictinonaryModel("2211221408050004402", "SaleInsideInVeterinaryDrug1"));
            value.Add(new DictinonaryModel("2211221408050004502", "SaleInsideInVaccin1"));
            value.Add(new DictinonaryModel("2211221408050004602", "SaleInsideInOtherMaterial1"));
            value.Add(new DictinonaryModel("2211221408050004702", "SaleInsideInPigTransferValue1"));
            value.Add(new DictinonaryModel("2211221408050004802", "SaleInsideInSemen1"));
            value.Add(new DictinonaryModel("2211221408050004902", "SaleInsideInCost1"));
            value.Add(new DictinonaryModel("2211221408050005002", "SaleInsideInPigDeathValue1"));
            value.Add(new DictinonaryModel("2211221408050005102", "SaleInsideInPigBatchDeath1"));
            value.Add(new DictinonaryModel("2211221408050005202", "SaleOutsideInFeed1"));
            value.Add(new DictinonaryModel("2211221408050005302", "SaleOutsideInVeterinaryDrug1"));
            value.Add(new DictinonaryModel("2211221408050005402", "SaleOutsideInVaccin1"));
            value.Add(new DictinonaryModel("2211221408050005502", "SaleOutsideInOtherMaterial1"));
            value.Add(new DictinonaryModel("2211221408050005602", "SaleOutsideInPigTransferValue1"));
            value.Add(new DictinonaryModel("2211221408050005702", "SaleOutsideInSemen1"));
            value.Add(new DictinonaryModel("2211221408050005802", "SaleOutsideInCost1"));
            value.Add(new DictinonaryModel("2211221408050005902", "SaleOutsideInPigDeathValue1"));
            value.Add(new DictinonaryModel("2211221408050006002", "SaleOutsideInPigBatchDeath1"));
            value.Add(new DictinonaryModel("2211221408050006102", "DeathInFeed1"));
            value.Add(new DictinonaryModel("2211221408050006202", "DeathInVeterinaryDrug1"));
            value.Add(new DictinonaryModel("2211221408050006302", "DeathInVaccin1"));
            value.Add(new DictinonaryModel("2211221408050006402", "DeathInOtherMaterial1"));
            value.Add(new DictinonaryModel("2211221408050006502", "DeathInPigTransferValue1"));
            value.Add(new DictinonaryModel("2211221408050006602", "DeathInSemen1"));
            value.Add(new DictinonaryModel("2211221408050006702", "DeathInCost1"));
            value.Add(new DictinonaryModel("2211221408050006802", "DeathInPigDeathValue1"));
            value.Add(new DictinonaryModel("2211221408050006902", "DeathInPigBatchDeath1"));
            value.Add(new DictinonaryModel("2211221408050007002", "CurrentFeed1"));
            value.Add(new DictinonaryModel("2211221408050007102", "CurrentVeterinaryDrug1"));
            value.Add(new DictinonaryModel("2211221408050007202", "CurrentVaccin1"));
            value.Add(new DictinonaryModel("2211221408050007302", "CurrentOtherMaterial1"));
            value.Add(new DictinonaryModel("2211221408050007402", "CurrentPigTransferValue1"));
            value.Add(new DictinonaryModel("2211221408050007502", "CurrentSemen1"));
            value.Add(new DictinonaryModel("2211221408050007602", "CurrentCost1"));
            value.Add(new DictinonaryModel("2211221408050007702", "ChildbirthOutFeed1"));
            value.Add(new DictinonaryModel("2211221408050007802", "ChildbirthOutVeterinaryDrug1"));
            value.Add(new DictinonaryModel("2211221408050007902", "ChildbirthOutVaccin1"));
            value.Add(new DictinonaryModel("2211221408050008002", "ChildbirthOutOtherMaterial1"));
            value.Add(new DictinonaryModel("2211221408050008102", "ChildbirthOutPigTransferValue1"));
            value.Add(new DictinonaryModel("2211221408050008202", "ChildbirthOutSemen1"));
            value.Add(new DictinonaryModel("2211221408050008302", "ChildbirthOutCost1"));
            value.Add(new DictinonaryModel("2211221408050008402", "ChildbirthOutPigDeathValue1"));
            value.Add(new DictinonaryModel("2211221408050008502", "ChildbirthOutPigBatchDeath1"));
            value.Add(new DictinonaryModel("2211221408050008602", "LactatingOutFeed1"));
            value.Add(new DictinonaryModel("2211221408050008702", "LactatingOutVeterinaryDrug1"));
            value.Add(new DictinonaryModel("2211221408050008802", "LactatingOutVaccin1"));
            value.Add(new DictinonaryModel("2211221408050008902", "LactatingOutOtherMaterial1"));
            value.Add(new DictinonaryModel("2211221408050009002", "LactatingOutPigTransferValue1"));
            value.Add(new DictinonaryModel("2211221408050009102", "LactatingOutCost1"));
            value.Add(new DictinonaryModel("2211221408050009202", "ReadyBreedingOutFeed1"));
            value.Add(new DictinonaryModel("2211221408050009302", "ReadyBreedingOutVeterinaryDrug1"));
            value.Add(new DictinonaryModel("2211221408050009402", "ReadyBreedingOutVaccin1"));
            value.Add(new DictinonaryModel("2211221408050009502", "ReadyBreedingOutOtherMaterial1"));
            value.Add(new DictinonaryModel("2211221408050009602", "ReadyBreedingOutPigTransferValue1"));
            value.Add(new DictinonaryModel("2211221408050009702", "ReadyBreedingOutCost1"));
            value.Add(new DictinonaryModel("2211221408050009802", "ReadyBreedingOutPigDeathValue1"));
            value.Add(new DictinonaryModel("2211221408050009902", "ReadyBreedingOutPigBatchDeath1"));
            value.Add(new DictinonaryModel("2211221408050010002", "MissBreedingOutFeed1"));
            value.Add(new DictinonaryModel("2211221408050010102", "MissBreedingOutVeterinaryDrug1"));
            value.Add(new DictinonaryModel("2211221408050010202", "MissBreedingOutVaccin1"));
            value.Add(new DictinonaryModel("2211221408050010302", "MissBreedingOutOtherMaterial1"));
            value.Add(new DictinonaryModel("2211221408050010402", "MissBreedingOutPigTransferValue1"));
            value.Add(new DictinonaryModel("2211221408050010502", "MissBreedingOutSemen1"));
            value.Add(new DictinonaryModel("2211221408050010602", "MissBreedingOutCost1"));
            value.Add(new DictinonaryModel("2211221408050010702", "MissBreedingOutPigDeathValue1"));
            value.Add(new DictinonaryModel("2211221408050010802", "MissBreedingOutPigBatchDeath1"));
            value.Add(new DictinonaryModel("2211221408050010902", "AllocateOutAmount1"));
            value.Add(new DictinonaryModel("2211221408050011002", "AllocateOutTotalDepreciation1"));
            value.Add(new DictinonaryModel("2211221408050011102", "AllocateOutFeed1"));
            value.Add(new DictinonaryModel("2211221408050011202", "AllocateOutVeterinaryDrug1"));
            value.Add(new DictinonaryModel("2211221408050011302", "AllocateOutVaccin1"));
            value.Add(new DictinonaryModel("2211221408050011402", "AllocateOutOtherMaterial1"));
            value.Add(new DictinonaryModel("2211221408050011502", "AllocateOutPigTransferValue1"));
            value.Add(new DictinonaryModel("2211221408050011602", "AllocateOutSemen1"));
            value.Add(new DictinonaryModel("2211221408050011702", "AllocateOutCost1"));
            value.Add(new DictinonaryModel("2211221408050011802", "AllocateOutPigDeathValue1"));
            value.Add(new DictinonaryModel("2211221408050011902", "AllocateOutPigBatchDeath1"));
            value.Add(new DictinonaryModel("2211221408050012002", "BatchOutAmount1"));
            value.Add(new DictinonaryModel("2211221408050012102", "BatchOutTotalDepreciation1"));
            value.Add(new DictinonaryModel("2211221408050012202", "BatchOutFeed1"));
            value.Add(new DictinonaryModel("2211221408050012302", "BatchOutVeterinaryDrug1"));
            value.Add(new DictinonaryModel("2211221408050012402", "BatchOutVaccin1"));
            value.Add(new DictinonaryModel("2211221408050012502", "BatchOutOtherMaterial1"));
            value.Add(new DictinonaryModel("2211221408050012602", "BatchOutPigTransferValue1"));
            value.Add(new DictinonaryModel("2211221408050012702", "BatchOutSemen1"));
            value.Add(new DictinonaryModel("2211221408050012802", "BatchOutCost1"));
            value.Add(new DictinonaryModel("2211221408050012902", "BatchOutPigDeathValue1"));
            value.Add(new DictinonaryModel("2211221408050013002", "BatchOutPigBatchDeath1"));
            value.Add(new DictinonaryModel("2211221408050013102", "SaleInsideOutAmount1"));
            value.Add(new DictinonaryModel("2211221408050013202", "SaleInsideOutTotalDepreciation1"));
            value.Add(new DictinonaryModel("2211221408050013302", "SaleInsideOutFeed1"));
            value.Add(new DictinonaryModel("2211221408050013402", "SaleInsideOutVeterinaryDrug1"));
            value.Add(new DictinonaryModel("2211221408050013502", "SaleInsideOutVaccin1"));
            value.Add(new DictinonaryModel("2211221408050013602", "SaleInsideOutOtherMaterial1"));
            value.Add(new DictinonaryModel("2211221408050013702", "SaleInsideOutPigTransferValue1"));
            value.Add(new DictinonaryModel("2211221408050013802", "SaleInsideOutSemen1"));
            value.Add(new DictinonaryModel("2211221408050013902", "SaleInsideOutCost1"));
            value.Add(new DictinonaryModel("2211221408050014002", "SaleInsideOutPigDeathValue1"));
            value.Add(new DictinonaryModel("2211221408050014102", "SaleInsideOutPigBatchDeath1"));
            value.Add(new DictinonaryModel("2211221408050014202", "SaleOutsideOutAmount1"));
            value.Add(new DictinonaryModel("2211221408050014302", "SaleOutsideOutTotalDepreciation1"));
            value.Add(new DictinonaryModel("2211221408050014402", "SaleOutsideOutFeed1"));
            value.Add(new DictinonaryModel("2211221408050014502", "SaleOutsideOutVeterinaryDrug1"));
            value.Add(new DictinonaryModel("2211221408050014602", "SaleOutsideOutVaccin1"));
            value.Add(new DictinonaryModel("2211221408050014702", "SaleOutsideOutOtherMaterial1"));
            value.Add(new DictinonaryModel("2211221408050014802", "SaleOutsideOutPigTransferValue1"));
            value.Add(new DictinonaryModel("2211221408050014902", "SaleOutsideOutSemen1"));
            value.Add(new DictinonaryModel("2211221408050015002", "SaleOutsideOutCost1"));
            value.Add(new DictinonaryModel("2211221408050015102", "SaleOutsideOutPigDeathValue1"));
            value.Add(new DictinonaryModel("2211221408050015202", "SaleOutsideOutPigBatchDeath1"));
            value.Add(new DictinonaryModel("2211221408050015302", "DeathOutAmount1"));
            value.Add(new DictinonaryModel("2211221408050015402", "DeathOutTotalDepreciation1"));
            value.Add(new DictinonaryModel("2211221408050015502", "DeathOutFeed1"));
            value.Add(new DictinonaryModel("2211221408050015602", "DeathOutVeterinaryDrug1"));
            value.Add(new DictinonaryModel("2211221408050015702", "DeathOutVaccin1"));
            value.Add(new DictinonaryModel("2211221408050015802", "DeathOutOtherMaterial1"));
            value.Add(new DictinonaryModel("2211221408050015902", "DeathOutPigTransferValue1"));
            value.Add(new DictinonaryModel("2211221408050016002", "DeathOutSemen1"));
            value.Add(new DictinonaryModel("2211221408050016102", "DeathOutCost1"));
            value.Add(new DictinonaryModel("2211221408050016202", "DeathOutPigDeathValue1"));
            value.Add(new DictinonaryModel("2211221408050016302", "DeathOutPigBatchDeath1"));
            value.Add(new DictinonaryModel("2211221408050016402", "ReadyBreedingInAmount1"));
            value.Add(new DictinonaryModel("2211221408050016502", "ReadyBreedingInTotalDepreciation1"));
            value.Add(new DictinonaryModel("2211221408050016602", "MissBreedingInAmount1"));
            value.Add(new DictinonaryModel("2211221408050016702", "MissBreedingInTotalDepreciation1"));
            value.Add(new DictinonaryModel("2211221408050016802", "WeaningInAmount1"));
            value.Add(new DictinonaryModel("2211221408050016902", "WeaningInTotalDepreciation1"));
            value.Add(new DictinonaryModel("2211221408050017002", "ReadyBreedingOutAmount1"));
            value.Add(new DictinonaryModel("2211221408050017102", "ReadyBreedingOutTotalDepreciation1"));
            value.Add(new DictinonaryModel("2211221408050017202", "MissBreedingOutAmount1"));
            value.Add(new DictinonaryModel("2211221408050017302", "MissBreedingOutTotalDepreciation1"));
            value.Add(new DictinonaryModel("2211221408050017402", "LactatingOutAmount1"));
            value.Add(new DictinonaryModel("2211221408050017502", "LactatingOutTotalDepreciation1"));
            value.Add(new DictinonaryModel("2211221408050017602", "LactatingOutPigDeathValue1"));
            value.Add(new DictinonaryModel("2211221408250000102", "InsidePurchaseInPigletCost2"));
            value.Add(new DictinonaryModel("2211221408250000202", "OutPurchaseInPigletCost2"));
            value.Add(new DictinonaryModel("2211221408250000302", "ChildbirthInPigletCost2"));
            value.Add(new DictinonaryModel("2211221408250000402", "LactatingInTransferValue2"));
            value.Add(new DictinonaryModel("2211221408250000502", "LactatingInFeed2"));
            value.Add(new DictinonaryModel("2211221408250000602", "LactatingInVeterinaryDrug2"));
            value.Add(new DictinonaryModel("2211221408250000702", "LactatingInVaccin2"));
            value.Add(new DictinonaryModel("2211221408250000802", "LactatingInOtherMaterial2"));
            value.Add(new DictinonaryModel("2211221408250000902", "LactatingInCost2"));
            value.Add(new DictinonaryModel("2211221408250001002", "LactatingInPigDeathValue2"));
            value.Add(new DictinonaryModel("2211221408250001102", "BatchInPigletCost2"));
            value.Add(new DictinonaryModel("2211221408250001202", "BatchInFeed2"));
            value.Add(new DictinonaryModel("2211221408250001302", "BatchInVeterinaryDrug2"));
            value.Add(new DictinonaryModel("2211221408250001402", "BatchInVaccin2"));
            value.Add(new DictinonaryModel("2211221408250001502", "BatchInOtherMaterial2"));
            value.Add(new DictinonaryModel("2211221408250001602", "BatchInCost2"));
            value.Add(new DictinonaryModel("2211221408250001702", "BatchInPigTransferValue2"));
            value.Add(new DictinonaryModel("2211221408250001802", "BatchInPigDeathValue2"));
            value.Add(new DictinonaryModel("2211221408250001902", "BatchInBatchInsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250002002", "BatchInBatchOutsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250002102", "AllocateInPigletCost2"));
            value.Add(new DictinonaryModel("2211221408250002202", "AllocateInFeed2"));
            value.Add(new DictinonaryModel("2211221408250002302", "AllocateInVeterinaryDrug2"));
            value.Add(new DictinonaryModel("2211221408250002402", "AllocateInVaccin2"));
            value.Add(new DictinonaryModel("2211221408250002502", "AllocateInOtherMaterial2"));
            value.Add(new DictinonaryModel("2211221408250002602", "AllocateInCost2"));
            value.Add(new DictinonaryModel("2211221408250002702", "AllocateInPigTransferValue2"));
            value.Add(new DictinonaryModel("2211221408250002802", "AllocateInPigDeathValue2"));
            value.Add(new DictinonaryModel("2211221408250002902", "AllocateInBatchInsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250003002", "AllocateInBatchOutsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250003102", "DeathInFeed2"));
            value.Add(new DictinonaryModel("2211221408250003202", "DeathInVeterinaryDrug2"));
            value.Add(new DictinonaryModel("2211221408250003302", "DeathInVaccin2"));
            value.Add(new DictinonaryModel("2211221408250003402", "DeathInOtherMaterial2"));
            value.Add(new DictinonaryModel("2211221408250003502", "DeathInCost2"));
            value.Add(new DictinonaryModel("2211221408250003602", "DeathInPigTransferValue2"));
            value.Add(new DictinonaryModel("2211221408250003702", "DeathInPigDeathValue2"));
            value.Add(new DictinonaryModel("2211221408250003802", "DeathInBatchInsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250003902", "DeathInBatchOutsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250004002", "SaleInsideInTransferValue2"));
            value.Add(new DictinonaryModel("2211221408250004102", "SaleInsideInFeed2"));
            value.Add(new DictinonaryModel("2211221408250004202", "SaleInsideInVeterinaryDrug2"));
            value.Add(new DictinonaryModel("2211221408250004302", "SaleInsideInVaccin2"));
            value.Add(new DictinonaryModel("2211221408250004402", "SaleInsideInOtherMaterial2"));
            value.Add(new DictinonaryModel("2211221408250004502", "SaleInsideInCost2"));
            value.Add(new DictinonaryModel("2211221408250004602", "SaleInsideInBatchInsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250004702", "SaleOutsideInTransferValue2"));
            value.Add(new DictinonaryModel("2211221408250004802", "SaleOutsideInFeed2"));
            value.Add(new DictinonaryModel("2211221408250004902", "SaleOutsideInVeterinaryDrug2"));
            value.Add(new DictinonaryModel("2211221408250005002", "SaleOutsideInVaccin2"));
            value.Add(new DictinonaryModel("2211221408250005102", "SaleOutsideInOtherMaterial2"));
            value.Add(new DictinonaryModel("2211221408250005202", "SaleOutsideInCost2"));
            value.Add(new DictinonaryModel("2211221408250005302", "SaleOutsideInBatchInsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250005402", "CurrentCostFeed2"));
            value.Add(new DictinonaryModel("2211221408250005502", "CurrentCostVeterinaryDrug2"));
            value.Add(new DictinonaryModel("2211221408250005602", "CurrentCostVaccin2"));
            value.Add(new DictinonaryModel("2211221408250005702", "CurrentCostOtherMaterial2"));
            value.Add(new DictinonaryModel("2211221408250005802", "CurrentCostCost2"));
            value.Add(new DictinonaryModel("2211221408250005902", "AcrossStepOutPigletCost2"));
            value.Add(new DictinonaryModel("2211221408250006002", "AcrossStepOutFeed2"));
            value.Add(new DictinonaryModel("2211221408250006102", "AcrossStepOutVeterinaryDrug2"));
            value.Add(new DictinonaryModel("2211221408250006202", "AcrossStepOutVaccin2"));
            value.Add(new DictinonaryModel("2211221408250006302", "AcrossStepOutOtherMaterial2"));
            value.Add(new DictinonaryModel("2211221408250006402", "AcrossStepOutCost2"));
            value.Add(new DictinonaryModel("2211221408250006502", "AcrossStepOutPigTransferValue2"));
            value.Add(new DictinonaryModel("2211221408250006602", "AcrossStepOutPigDeathValue2"));
            value.Add(new DictinonaryModel("2211221408250006702", "AcrossStepOutBatchInsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250006802", "AcrossStepOutBatchOutsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250006902", "BatchOutPigletCost2"));
            value.Add(new DictinonaryModel("2211221408250007002", "BatchOutFeed2"));
            value.Add(new DictinonaryModel("2211221408250007102", "BatchOutVeterinaryDrug2"));
            value.Add(new DictinonaryModel("2211221408250007202", "BatchOutVaccin2"));
            value.Add(new DictinonaryModel("2211221408250007302", "BatchOutOtherMaterial2"));
            value.Add(new DictinonaryModel("2211221408250007402", "BatchOutCost2"));
            value.Add(new DictinonaryModel("2211221408250007502", "BatchOutPigTransferValue2"));
            value.Add(new DictinonaryModel("2211221408250007602", "BatchOutPigDeathValue2"));
            value.Add(new DictinonaryModel("2211221408250007702", "BatchOutBatchInsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250007802", "BatchOutBatchOutsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250007902", "AllocateOutPigletCost2"));
            value.Add(new DictinonaryModel("2211221408250008002", "AllocateOutFeed2"));
            value.Add(new DictinonaryModel("2211221408250008102", "AllocateOutVeterinaryDrug2"));
            value.Add(new DictinonaryModel("2211221408250008202", "AllocateOutVaccin2"));
            value.Add(new DictinonaryModel("2211221408250008302", "AllocateOutOtherMaterial2"));
            value.Add(new DictinonaryModel("2211221408250008402", "AllocateOutCost2"));
            value.Add(new DictinonaryModel("2211221408250008502", "AllocateOutPigTransferValue2"));
            value.Add(new DictinonaryModel("2211221408250008602", "AllocateOutPigDeathValue2"));
            value.Add(new DictinonaryModel("2211221408250008702", "AllocateOutBatchInsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250008802", "AllocateOutBatchOutsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250008902", "DeathOutPigletCost2"));
            value.Add(new DictinonaryModel("2211221408250009002", "DeathOutFeed2"));
            value.Add(new DictinonaryModel("2211221408250009102", "DeathOutVeterinaryDrug2"));
            value.Add(new DictinonaryModel("2211221408250009202", "DeathOutVaccin2"));
            value.Add(new DictinonaryModel("2211221408250009302", "DeathOutOtherMaterial2"));
            value.Add(new DictinonaryModel("2211221408250009402", "DeathOutCost2"));
            value.Add(new DictinonaryModel("2211221408250009502", "DeathOutPigTransferValue2"));
            value.Add(new DictinonaryModel("2211221408250009602", "DeathOutPigDeathValue2"));
            value.Add(new DictinonaryModel("2211221408250009702", "DeathOutBatchInsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250009802", "DeathOutBatchOutsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250009902", "SaleInsideOutPigletCost2"));
            value.Add(new DictinonaryModel("2211221408250010002", "SaleInsideOutFeed2"));
            value.Add(new DictinonaryModel("2211221408250010102", "SaleInsideOutVeterinaryDrug2"));
            value.Add(new DictinonaryModel("2211221408250010202", "SaleInsideOutVaccin2"));
            value.Add(new DictinonaryModel("2211221408250010302", "SaleInsideOutOtherMaterial2"));
            value.Add(new DictinonaryModel("2211221408250010402", "SaleInsideOutCost2"));
            value.Add(new DictinonaryModel("2211221408250010502", "SaleInsideOutPigTransferValue2"));
            value.Add(new DictinonaryModel("2211221408250010602", "SaleInsideOutPigDeathValue2"));
            value.Add(new DictinonaryModel("2211221408250010702", "SaleInsideOutBatchInsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250010802", "SaleInsideOutBatchOutsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250010902", "SaleOutsideOutPigletCost2"));
            value.Add(new DictinonaryModel("2211221408250011002", "SaleOutsideOutFeed2"));
            value.Add(new DictinonaryModel("2211221408250011102", "SaleOutsideOutVeterinaryDrug2"));
            value.Add(new DictinonaryModel("2211221408250011202", "SaleOutsideOutVaccin2"));
            value.Add(new DictinonaryModel("2211221408250011302", "SaleOutsideOutOtherMaterial2"));
            value.Add(new DictinonaryModel("2211221408250011402", "SaleOutsideOutCost2"));
            value.Add(new DictinonaryModel("2211221408250011502", "SaleOutsideOutPigTransferValue2"));
            value.Add(new DictinonaryModel("2211221408250011602", "SaleOutsideOutPigDeathValue2"));
            value.Add(new DictinonaryModel("2211221408250011702", "SaleOutsideOutBatchInsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250011802", "SaleOutsideOutBatchOutsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250011902", "EmergenceOutPigletCost2"));
            value.Add(new DictinonaryModel("2211221408250012002", "EmergenceOutFeed2"));
            value.Add(new DictinonaryModel("2211221408250012102", "EmergenceOutVeterinaryDrug2"));
            value.Add(new DictinonaryModel("2211221408250012202", "EmergenceOutVaccin2"));
            value.Add(new DictinonaryModel("2211221408250012302", "EmergenceOutOtherMaterial2"));
            value.Add(new DictinonaryModel("2211221408250012402", "EmergenceOutCost2"));
            value.Add(new DictinonaryModel("2211221408250012502", "EmergenceOutPigTransferValue2"));
            value.Add(new DictinonaryModel("2211221408250012602", "EmergenceOutPigDeathValue2"));
            value.Add(new DictinonaryModel("2211221408250012702", "EmergenceOutBatchInsideDeath2"));
            value.Add(new DictinonaryModel("2211221408250012802", "EmergenceOutBatchOutsideDeath2"));
            value.Add(new DictinonaryModel("2211221408410000102", "AcrossStepInPigletCost3"));
            value.Add(new DictinonaryModel("2211221408410000202", "InsidePurchaseInPigletCost3"));
            value.Add(new DictinonaryModel("2211221408410000302", "OutPurchaseInPigletCost3"));
            value.Add(new DictinonaryModel("2211221408410000402", "RecycleBreederInPigletCost3"));
            value.Add(new DictinonaryModel("2211221408410000502", "BatchInPigletCost3"));
            value.Add(new DictinonaryModel("2211221408410000602", "BatchInFeed3"));
            value.Add(new DictinonaryModel("2211221408410000702", "BatchInVeterinaryDrug3"));
            value.Add(new DictinonaryModel("2211221408410000802", "BatchInVaccin3"));
            value.Add(new DictinonaryModel("2211221408410000902", "BatchInOtherMaterial3"));
            value.Add(new DictinonaryModel("2211221408410001002", "BatchInCost3"));
            value.Add(new DictinonaryModel("2211221408410001102", "BatchInBatchInsideDeath3"));
            value.Add(new DictinonaryModel("2211221408410001202", "BatchInBatchOutsideDeath3"));
            value.Add(new DictinonaryModel("2211221408410001302", "AllocateInPigletCost3"));
            value.Add(new DictinonaryModel("2211221408410001402", "AllocateInFeed3"));
            value.Add(new DictinonaryModel("2211221408410001502", "AllocateInVeterinaryDrug3"));
            value.Add(new DictinonaryModel("2211221408410001602", "AllocateInVaccin3"));
            value.Add(new DictinonaryModel("2211221408410001702", "AllocateInOtherMaterial3"));
            value.Add(new DictinonaryModel("2211221408410001802", "AllocateInCost3"));
            value.Add(new DictinonaryModel("2211221408410001902", "AllocateInBatchInsideDeath3"));
            value.Add(new DictinonaryModel("2211221408410002002", "AllocateInBatchOutsideDeath3"));
            value.Add(new DictinonaryModel("2211221408410002102", "DeathInFeed3"));
            value.Add(new DictinonaryModel("2211221408410002202", "DeathInVeterinaryDrug3"));
            value.Add(new DictinonaryModel("2211221408410002302", "DeathInVaccin3"));
            value.Add(new DictinonaryModel("2211221408410002402", "DeathInOtherMaterial3"));
            value.Add(new DictinonaryModel("2211221408410002502", "DeathInCost3"));
            value.Add(new DictinonaryModel("2211221408410002602", "DeathInBatchInsideDeath3"));
            value.Add(new DictinonaryModel("2211221408410002702", "DeathInBatchOutsideDeath3"));
            value.Add(new DictinonaryModel("2211221408410002802", "CurrentCostFeed3"));
            value.Add(new DictinonaryModel("2211221408410002902", "CurrentCostVeterinaryDrug3"));
            value.Add(new DictinonaryModel("2211221408410003002", "CurrentCostVaccin3"));
            value.Add(new DictinonaryModel("2211221408410003102", "CurrentCostOtherMaterial3"));
            value.Add(new DictinonaryModel("2211221408410003202", "CurrentCostCost3"));
            value.Add(new DictinonaryModel("2211221408410003302", "AcrossStepOutPigletCost3"));
            value.Add(new DictinonaryModel("2211221408410003402", "AcrossStepOutFeed3"));
            value.Add(new DictinonaryModel("2211221408410003502", "AcrossStepOutVeterinaryDrug3"));
            value.Add(new DictinonaryModel("2211221408410003602", "AcrossStepOutVaccin3"));
            value.Add(new DictinonaryModel("2211221408410003702", "AcrossStepOutOtherMaterial3"));
            value.Add(new DictinonaryModel("2211221408410003802", "AcrossStepOutCost3"));
            value.Add(new DictinonaryModel("2211221408410003902", "AcrossStepOutBatchInsideDeath3"));
            value.Add(new DictinonaryModel("2211221408410004002", "AcrossStepOutBatchOutsideDeath3"));
            value.Add(new DictinonaryModel("2211221408410004102", "BatchOutPigletCost3"));
            value.Add(new DictinonaryModel("2211221408410004202", "BatchOutFeed3"));
            value.Add(new DictinonaryModel("2211221408410004302", "BatchOutVeterinaryDrug3"));
            value.Add(new DictinonaryModel("2211221408410004402", "BatchOutVaccin3"));
            value.Add(new DictinonaryModel("2211221408410004502", "BatchOutOtherMaterial3"));
            value.Add(new DictinonaryModel("2211221408410004602", "BatchOutCost3"));
            value.Add(new DictinonaryModel("2211221408410004702", "BatchOutBatchInsideDeath3"));
            value.Add(new DictinonaryModel("2211221408410004802", "BatchOutBatchOutsideDeath3"));
            value.Add(new DictinonaryModel("2211221408410004902", "AllocateOutPigletCost3"));
            value.Add(new DictinonaryModel("2211221408410005002", "AllocateOutFeed3"));
            value.Add(new DictinonaryModel("2211221408410005102", "AllocateOutVeterinaryDrug3"));
            value.Add(new DictinonaryModel("2211221408410005202", "AllocateOutVaccin3"));
            value.Add(new DictinonaryModel("2211221408410005302", "AllocateOutOtherMaterial3"));
            value.Add(new DictinonaryModel("2211221408410005402", "AllocateOutCost3"));
            value.Add(new DictinonaryModel("2211221408410005502", "AllocateOutBatchInsideDeath3"));
            value.Add(new DictinonaryModel("2211221408410005602", "AllocateOutBatchOutsideDeath3"));
            value.Add(new DictinonaryModel("2211221408410005702", "DeathOutPigletCost3"));
            value.Add(new DictinonaryModel("2211221408410005802", "DeathOutFeed3"));
            value.Add(new DictinonaryModel("2211221408410005902", "DeathOutVeterinaryDrug3"));
            value.Add(new DictinonaryModel("2211221408410006002", "DeathOutVaccin3"));
            value.Add(new DictinonaryModel("2211221408410006102", "DeathOutOtherMaterial3"));
            value.Add(new DictinonaryModel("2211221408410006202", "DeathOutCost3"));
            value.Add(new DictinonaryModel("2211221408410006302", "DeathOutBatchInsideDeath3"));
            value.Add(new DictinonaryModel("2211221408410006402", "DeathOutBatchOutsideDeath3"));
            value.Add(new DictinonaryModel("2211221408410006502", "SaleInsideOutPigletCost3"));
            value.Add(new DictinonaryModel("2211221408410006602", "SaleInsideOutFeed3"));
            value.Add(new DictinonaryModel("2211221408410006702", "SaleInsideOutVeterinaryDrug3"));
            value.Add(new DictinonaryModel("2211221408410006802", "SaleInsideOutVaccin3"));
            value.Add(new DictinonaryModel("2211221408410006902", "SaleInsideOutOtherMaterial3"));
            value.Add(new DictinonaryModel("2211221408410007002", "SaleInsideOutCost3"));
            value.Add(new DictinonaryModel("2211221408410007102", "SaleInsideOutBatchInsideDeath3"));
            value.Add(new DictinonaryModel("2211221408410007202", "SaleInsideOutBatchOutsideDeath3"));
            value.Add(new DictinonaryModel("2211221408410007302", "SaleOutsideOutPigletCost3"));
            value.Add(new DictinonaryModel("2211221408410007402", "SaleOutsideOutFeed3"));
            value.Add(new DictinonaryModel("2211221408410007502", "SaleOutsideOutVeterinaryDrug3"));
            value.Add(new DictinonaryModel("2211221408410007602", "SaleOutsideOutVaccin3"));
            value.Add(new DictinonaryModel("2211221408410007702", "SaleOutsideOutOtherMaterial3"));
            value.Add(new DictinonaryModel("2211221408410007802", "SaleOutsideOutCost3"));
            value.Add(new DictinonaryModel("2211221408410007902", "SaleOutsideOutBatchInsideDeath3"));
            value.Add(new DictinonaryModel("2211221408410008002", "SaleOutsideOutBatchOutsideDeath3"));
            value.Add(new DictinonaryModel("2211221408410008102", "EmergenceOutPigletCost3"));
            value.Add(new DictinonaryModel("2211221408410008202", "EmergenceOutFeed3"));
            value.Add(new DictinonaryModel("2211221408410008302", "EmergenceOutVeterinaryDrug3"));
            value.Add(new DictinonaryModel("2211221408410008402", "EmergenceOutVaccin3"));
            value.Add(new DictinonaryModel("2211221408410008502", "EmergenceOutOtherMaterial3"));
            value.Add(new DictinonaryModel("2211221408410008602", "EmergenceOutCost3"));
            value.Add(new DictinonaryModel("2211221408410008702", "EmergenceOutBatchInsideDeath3"));
            value.Add(new DictinonaryModel("2211221408410008802", "EmergenceOutBatchOutsideDeath3"));
            value.Add(new DictinonaryModel("2211221409000000102", "AcrossStepInPigletCost4"));
            value.Add(new DictinonaryModel("2211221409000000202", "InsidePurchaseInPigletCost4"));
            value.Add(new DictinonaryModel("2211221409000000302", "OutPurchaseInPigletCost4"));
            value.Add(new DictinonaryModel("2211221409000000402", "RecycleBreederInPigletCost4"));
            value.Add(new DictinonaryModel("2211221409000000502", "BatchInPigletCost4"));
            value.Add(new DictinonaryModel("2211221409000000602", "BatchInFeed4"));
            value.Add(new DictinonaryModel("2211221409000000702", "BatchInVeterinaryDrug4"));
            value.Add(new DictinonaryModel("2211221409000000802", "BatchInVaccin4"));
            value.Add(new DictinonaryModel("2211221409000000902", "BatchInOtherMaterial4"));
            value.Add(new DictinonaryModel("2211221409000001002", "BatchInCost4"));
            value.Add(new DictinonaryModel("2211221409000001102", "BatchInBatchInsideDeath4"));
            value.Add(new DictinonaryModel("2211221409000001202", "BatchInBatchOutsideDeath4"));
            value.Add(new DictinonaryModel("2211221409000001302", "AllocateInPigletCost4"));
            value.Add(new DictinonaryModel("2211221409000001402", "AllocateInFeed4"));
            value.Add(new DictinonaryModel("2211221409000001502", "AllocateInVeterinaryDrug4"));
            value.Add(new DictinonaryModel("2211221409000001602", "AllocateInVaccin4"));
            value.Add(new DictinonaryModel("2211221409000001702", "AllocateInOtherMaterial4"));
            value.Add(new DictinonaryModel("2211221409000001802", "AllocateInCost4"));
            value.Add(new DictinonaryModel("2211221409000001902", "AllocateInBatchInsideDeath4"));
            value.Add(new DictinonaryModel("2211221409000002002", "AllocateInBatchOutsideDeath4"));
            value.Add(new DictinonaryModel("2211221409000002102", "DeathInFeed4"));
            value.Add(new DictinonaryModel("2211221409000002202", "DeathInVeterinaryDrug4"));
            value.Add(new DictinonaryModel("2211221409000002302", "DeathInVaccin4"));
            value.Add(new DictinonaryModel("2211221409000002402", "DeathInOtherMaterial4"));
            value.Add(new DictinonaryModel("2211221409000002502", "DeathInCost4"));
            value.Add(new DictinonaryModel("2211221409000002602", "DeathInBatchInsideDeath4"));
            value.Add(new DictinonaryModel("2211221409000002702", "DeathInBatchOutsideDeath4"));
            value.Add(new DictinonaryModel("2211221409000002802", "CurrentCostFeed4"));
            value.Add(new DictinonaryModel("2211221409000002902", "CurrentCostVeterinaryDrug4"));
            value.Add(new DictinonaryModel("2211221409000003002", "CurrentCostVaccin4"));
            value.Add(new DictinonaryModel("2211221409000003102", "CurrentCostOtherMaterial4"));
            value.Add(new DictinonaryModel("2211221409000003202", "CurrentCostCost4"));
            value.Add(new DictinonaryModel("2211221409000003302", "AcrossStepOutPigletCost4"));
            value.Add(new DictinonaryModel("2211221409000003402", "AcrossStepOutFeed4"));
            value.Add(new DictinonaryModel("2211221409000003502", "AcrossStepOutVeterinaryDrug4"));
            value.Add(new DictinonaryModel("2211221409000003602", "AcrossStepOutVaccin4"));
            value.Add(new DictinonaryModel("2211221409000003702", "AcrossStepOutOtherMaterial4"));
            value.Add(new DictinonaryModel("2211221409000003802", "AcrossStepOutCost4"));
            value.Add(new DictinonaryModel("2211221409000003902", "AcrossStepOutBatchInsideDeath4"));
            value.Add(new DictinonaryModel("2211221409000004002", "AcrossStepOutBatchOutsideDeath4"));
            value.Add(new DictinonaryModel("2211221409000004102", "BatchOutPigletCost4"));
            value.Add(new DictinonaryModel("2211221409000004202", "BatchOutFeed4"));
            value.Add(new DictinonaryModel("2211221409000004302", "BatchOutVeterinaryDrug4"));
            value.Add(new DictinonaryModel("2211221409000004402", "BatchOutVaccin4"));
            value.Add(new DictinonaryModel("2211221409000004502", "BatchOutOtherMaterial4"));
            value.Add(new DictinonaryModel("2211221409000004602", "BatchOutCost4"));
            value.Add(new DictinonaryModel("2211221409000004702", "BatchOutBatchInsideDeath4"));
            value.Add(new DictinonaryModel("2211221409000004802", "BatchOutBatchOutsideDeath4"));
            value.Add(new DictinonaryModel("2211221409000004902", "AllocateOutPigletCost4"));
            value.Add(new DictinonaryModel("2211221409000005002", "AllocateOutFeed4"));
            value.Add(new DictinonaryModel("2211221409000005102", "AllocateOutVeterinaryDrug4"));
            value.Add(new DictinonaryModel("2211221409000005202", "AllocateOutVaccin4"));
            value.Add(new DictinonaryModel("2211221409000005302", "AllocateOutOtherMaterial4"));
            value.Add(new DictinonaryModel("2211221409000005402", "AllocateOutCost4"));
            value.Add(new DictinonaryModel("2211221409000005502", "AllocateOutBatchInsideDeath4"));
            value.Add(new DictinonaryModel("2211221409000005602", "AllocateOutBatchOutsideDeath4"));
            value.Add(new DictinonaryModel("2211221409000005702", "DeathOutPigletCost4"));
            value.Add(new DictinonaryModel("2211221409000005802", "DeathOutFeed4"));
            value.Add(new DictinonaryModel("2211221409000005902", "DeathOutVeterinaryDrug4"));
            value.Add(new DictinonaryModel("2211221409000006002", "DeathOutVaccin4"));
            value.Add(new DictinonaryModel("2211221409000006102", "DeathOutOtherMaterial4"));
            value.Add(new DictinonaryModel("2211221409000006202", "DeathOutCost4"));
            value.Add(new DictinonaryModel("2211221409000006302", "DeathOutBatchInsideDeath4"));
            value.Add(new DictinonaryModel("2211221409000006402", "DeathOutBatchOutsideDeath4"));
            value.Add(new DictinonaryModel("2211221409000006502", "SaleInsideOutPigletCost4"));
            value.Add(new DictinonaryModel("2211221409000006602", "SaleInsideOutFeed4"));
            value.Add(new DictinonaryModel("2211221409000006702", "SaleInsideOutVeterinaryDrug4"));
            value.Add(new DictinonaryModel("2211221409000006802", "SaleInsideOutVaccin4"));
            value.Add(new DictinonaryModel("2211221409000006902", "SaleInsideOutOtherMaterial4"));
            value.Add(new DictinonaryModel("2211221409000007002", "SaleInsideOutCost4"));
            value.Add(new DictinonaryModel("2211221409000007102", "SaleInsideOutBatchInsideDeath4"));
            value.Add(new DictinonaryModel("2211221409000007202", "SaleInsideOutBatchOutsideDeath4"));
            value.Add(new DictinonaryModel("2211221409000007302", "SaleOutsideOutPigletCost4"));
            value.Add(new DictinonaryModel("2211221409000007402", "SaleOutsideOutFeed4"));
            value.Add(new DictinonaryModel("2211221409000007502", "SaleOutsideOutVeterinaryDrug4"));
            value.Add(new DictinonaryModel("2211221409000007602", "SaleOutsideOutVaccin4"));
            value.Add(new DictinonaryModel("2211221409000007702", "SaleOutsideOutOtherMaterial4"));
            value.Add(new DictinonaryModel("2211221409000007802", "SaleOutsideOutCost4"));
            value.Add(new DictinonaryModel("2211221409000007902", "SaleOutsideOutBatchInsideDeath4"));
            value.Add(new DictinonaryModel("2211221409000008002", "SaleOutsideOutBatchOutsideDeath4"));
            value.Add(new DictinonaryModel("2201181112550000012", "Depreciated5"));
            value.Add(new DictinonaryModel("2201181112550000022", "MaterialCost5"));
            value.Add(new DictinonaryModel("2201181112550000032", "MedicantCost5"));
            value.Add(new DictinonaryModel("2201181112550000042", "VaccineCost5"));
            value.Add(new DictinonaryModel("2201181112550000052", "OtherCost5"));
            value.Add(new DictinonaryModel("2201181112550000062", "FeeCost5"));
            value.Add(new DictinonaryModel("2201181112550000072", "SumCost5"));
            value.Add(new DictinonaryModel("2211221409000008202", "CurrentPigCost6"));
            value.Add(new DictinonaryModel("2211221409000008302", "CurrentMaterial6"));
            value.Add(new DictinonaryModel("2211221409000008402", "CurrentMedicant6"));
            value.Add(new DictinonaryModel("2211221409000008502", "CurrentVaccine6"));
            value.Add(new DictinonaryModel("2211221409000008602", "CurrentOther6"));
            value.Add(new DictinonaryModel("2211221409000008702", "CurrentInnerDeath6"));
            value.Add(new DictinonaryModel("2211221409000008802", "CurrentOuterDeath6"));
            value.Add(new DictinonaryModel("2211221409000008902", "CurrentDirectFee6"));
            value.Add(new DictinonaryModel("2211221409000009002", "CurrentBuildingFee6"));
            value.Add(new DictinonaryModel("2211221409000009102", "CurrentPredictFeedFee6"));
            value.Add(new DictinonaryModel("2211221409000009202", "CurrentAdjustFeedFee6"));
            value.Add(new DictinonaryModel("2211221409000009302", "CurrentAdditionFee6"));
            value.Add(new DictinonaryModel("2211221409000009402", "CurrentHoldUpFee6"));
            value.Add(new DictinonaryModel("2211221409000009502", "RecyclePigCost6"));
            value.Add(new DictinonaryModel("2211221409000009602", "RecycleMaterial6"));
            value.Add(new DictinonaryModel("2211221409000009702", "RecycleMedicant6"));
            value.Add(new DictinonaryModel("2211221409000009802", "RecycleVaccine6"));
            value.Add(new DictinonaryModel("2211221409000009902", "RecycleOther6"));
            value.Add(new DictinonaryModel("2211221409000010002", "RecycleInnerDeath6"));
            value.Add(new DictinonaryModel("2211221409000010102", "RecycleOuterDeath6"));
            value.Add(new DictinonaryModel("2211221409000010202", "RecycleDirectFee6"));
            value.Add(new DictinonaryModel("2211221409000010302", "RecycleBuildingFee6"));
            value.Add(new DictinonaryModel("2211221409000010402", "RecyclePredictFeedFee6"));
            value.Add(new DictinonaryModel("2211221409000010502", "RecycleAdjustFeedFee6"));
            value.Add(new DictinonaryModel("2211221409000010602", "RecycleAdditionFee6"));
            value.Add(new DictinonaryModel("2211221409000010702", "RecycleHoldUpFee6"));
            value.Add(new DictinonaryModel("2211221409000010802", "DeathPigCost6"));
            value.Add(new DictinonaryModel("2211221409000010902", "DeathMaterial6"));
            value.Add(new DictinonaryModel("2211221409000011002", "DeathMedicant6"));
            value.Add(new DictinonaryModel("2211221409000011102", "DeathVaccine6"));
            value.Add(new DictinonaryModel("2211221409000011202", "DeathOther6"));
            value.Add(new DictinonaryModel("2211221409000011302", "DeathInnerDeath6"));
            value.Add(new DictinonaryModel("2211221409000011402", "DeathOuterDeath6"));
            value.Add(new DictinonaryModel("2211221409000011502", "DeathDirectFee6"));
            value.Add(new DictinonaryModel("2211221409000011602", "DeathBuildingFee6"));
            value.Add(new DictinonaryModel("2211221409000011702", "DeathPredictFeedFee6"));
            value.Add(new DictinonaryModel("2211221409000011802", "DeathAdjustFeedFee6"));
            value.Add(new DictinonaryModel("2211221409000011902", "DeathAdditionFee6"));
            value.Add(new DictinonaryModel("2211221409000012002", "DeathHoldUpFee6"));
            value.Add(new DictinonaryModel("2211221409000012102", "AdjustCount6"));
            return value;
        }
        #endregion
        #region 数据转换
        public List<PigCirculationSummary> TransformationSummary(List<PigCirculation> result1, List<SucklingPigCirculation> result2, List<GrowCirculation> result3, List<FallbackPigCirculation> result4, List<FeedCostCirculation> result5, List<BreederSummaryResultModel> result6)
        {
            List<PigCirculationSummary> list = new List<PigCirculationSummary>();
            foreach (var item in result1)
            {
                list.Add(new PigCirculationSummary()
                {
                    DeptId = item.DeptId,
                    DeptName = item.DeptName,
                    NumericalOrder = item.NumericalOrder,
                    PigFarmId = item.PigFarmId,
                    PigFarmName = item.PigFarmName,
                    ProductLineId = item.ProductLineId,
                    ProductLineName = item.ProductLineName,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    BatchId = item.BatchId,
                    BatchName = item.BatchName,
                    PigId = item.PigId,
                    PigType = item.PigType,
                    PigTypeName = item.PigTypeName,
                    FallbackInAmount1 = item.FallbackInAmount,
                    InsidePurchaseInAmount1 = item.InsidePurchaseInAmount,
                    OutPurchaseInAmount1 = item.OutPurchaseInAmount,
                    ReadyBreedingInFeed1 = item.ReadyBreedingInFeed,
                    ReadyBreedingInVeterinaryDrug1 = item.ReadyBreedingInVeterinaryDrug,
                    ReadyBreedingInVaccin1 = item.ReadyBreedingInVaccin,
                    ReadyBreedingInOtherMaterial1 = item.ReadyBreedingInOtherMaterial,
                    ReadyBreedingInPigTransferValue1 = item.ReadyBreedingInPigTransferValue,
                    ReadyBreedingInCost1 = item.ReadyBreedingInCost,
                    ReadyBreedingInPigDeathValue1 = item.ReadyBreedingInPigDeathValue,
                    ReadyBreedingInPigBatchDeath1 = item.ReadyBreedingInPigBatchDeath,
                    MissBreedingInFeed1 = item.MissBreedingInFeed,
                    MissBreedingInVeterinaryDrug1 = item.MissBreedingInVeterinaryDrug,
                    MissBreedingInVaccin1 = item.MissBreedingInVaccin,
                    MissBreedingInOtherMaterial1 = item.MissBreedingInOtherMaterial,
                    MissBreedingInPigTransferValue1 = item.MissBreedingInPigTransferValue,
                    MissBreedingInSemen1 = item.MissBreedingInSemen,
                    MissBreedingInCost1 = item.MissBreedingInCost,
                    MissBreedingInPigDeathValue1 = item.MissBreedingInPigDeathValue,
                    MissBreedingInPigBatchDeath1 = item.MissBreedingInPigBatchDeath,
                    AllocateInAmount1 = item.AllocateInAmount,
                    AllocateInTotalDepreciation1 = item.AllocateInTotalDepreciation,
                    AllocateInFeed1 = item.AllocateInFeed,
                    AllocateInVeterinaryDrug1 = item.AllocateInVeterinaryDrug,
                    AllocateInVaccin1 = item.AllocateInVaccin,
                    AllocateInOtherMaterial1 = item.AllocateInOtherMaterial,
                    AllocateInPigTransferValue1 = item.AllocateInPigTransferValue,
                    AllocateInSemen1 = item.AllocateInSemen,
                    AllocateInCost1 = item.AllocateInCost,
                    AllocateInPigDeathValue1 = item.AllocateInPigDeathValue,
                    AllocateInPigBatchDeath1 = item.AllocateInPigBatchDeath,
                    BatchInAmount1 = item.BatchInAmount,
                    BatchInTotalDepreciation1 = item.BatchInTotalDepreciation,
                    BatchInFeed1 = item.BatchInFeed,
                    BatchInVeterinaryDrug1 = item.BatchInVeterinaryDrug,
                    BatchInVaccin1 = item.BatchInVaccin,
                    BatchInOtherMaterial1 = item.BatchInOtherMaterial,
                    BatchInPigTransferValue1 = item.BatchInPigTransferValue,
                    BatchInSemen1 = item.BatchInSemen,
                    BatchInCost1 = item.BatchInCost,
                    BatchInPigDeathValue1 = item.BatchInPigDeathValue,
                    BatchInPigBatchDeath1 = item.BatchInPigBatchDeath,
                    SaleInsideInFeed1 = item.SaleInsideInFeed,
                    SaleInsideInVeterinaryDrug1 = item.SaleInsideInVeterinaryDrug,
                    SaleInsideInVaccin1 = item.SaleInsideInVaccin,
                    SaleInsideInOtherMaterial1 = item.SaleInsideInOtherMaterial,
                    SaleInsideInPigTransferValue1 = item.SaleInsideInPigTransferValue,
                    SaleInsideInSemen1 = item.SaleInsideInSemen,
                    SaleInsideInCost1 = item.SaleInsideInCost,
                    SaleInsideInPigDeathValue1 = item.SaleInsideInPigDeathValue,
                    SaleInsideInPigBatchDeath1 = item.SaleInsideInPigBatchDeath,
                    SaleOutsideInFeed1 = item.SaleOutsideInFeed,
                    SaleOutsideInVeterinaryDrug1 = item.SaleOutsideInVeterinaryDrug,
                    SaleOutsideInVaccin1 = item.SaleOutsideInVaccin,
                    SaleOutsideInOtherMaterial1 = item.SaleOutsideInOtherMaterial,
                    SaleOutsideInPigTransferValue1 = item.SaleOutsideInPigTransferValue,
                    SaleOutsideInSemen1 = item.SaleOutsideInSemen,
                    SaleOutsideInCost1 = item.SaleOutsideInCost,
                    SaleOutsideInPigDeathValue1 = item.SaleOutsideInPigDeathValue,
                    SaleOutsideInPigBatchDeath1 = item.SaleOutsideInPigBatchDeath,
                    DeathInFeed1 = item.DeathInFeed,
                    DeathInVeterinaryDrug1 = item.DeathInVeterinaryDrug,
                    DeathInVaccin1 = item.DeathInVaccin,
                    DeathInOtherMaterial1 = item.DeathInOtherMaterial,
                    DeathInPigTransferValue1 = item.DeathInPigTransferValue,
                    DeathInSemen1 = item.DeathInSemen,
                    DeathInCost1 = item.DeathInCost,
                    DeathInPigDeathValue1 = item.DeathInPigDeathValue,
                    DeathInPigBatchDeath1 = item.DeathInPigBatchDeath,
                    CurrentFeed1 = item.CurrentFeed,
                    CurrentVeterinaryDrug1 = item.CurrentVeterinaryDrug,
                    CurrentVaccin1 = item.CurrentVaccin,
                    CurrentOtherMaterial1 = item.CurrentOtherMaterial,
                    CurrentPigTransferValue1 = item.CurrentPigTransferValue,
                    CurrentSemen1 = item.CurrentSemen,
                    CurrentCost1 = item.CurrentCost,
                    ChildbirthOutFeed1 = item.ChildbirthOutFeed,
                    ChildbirthOutVeterinaryDrug1 = item.ChildbirthOutVeterinaryDrug,
                    ChildbirthOutVaccin1 = item.ChildbirthOutVaccin,
                    ChildbirthOutOtherMaterial1 = item.ChildbirthOutOtherMaterial,
                    ChildbirthOutPigTransferValue1 = item.ChildbirthOutPigTransferValue,
                    ChildbirthOutSemen1 = item.ChildbirthOutSemen,
                    ChildbirthOutCost1 = item.ChildbirthOutCost,
                    ChildbirthOutPigDeathValue1 = item.ChildbirthOutPigDeathValue,
                    ChildbirthOutPigBatchDeath1 = item.ChildbirthOutPigBatchDeath,
                    LactatingOutFeed1 = item.LactatingOutFeed,
                    LactatingOutVeterinaryDrug1 = item.LactatingOutVeterinaryDrug,
                    LactatingOutVaccin1 = item.LactatingOutVaccin,
                    LactatingOutOtherMaterial1 = item.LactatingOutOtherMaterial,
                    LactatingOutPigTransferValue1 = item.LactatingOutPigTransferValue,
                    LactatingOutCost1 = item.LactatingOutCost,
                    ReadyBreedingOutFeed1 = item.ReadyBreedingOutFeed,
                    ReadyBreedingOutVeterinaryDrug1 = item.ReadyBreedingOutVeterinaryDrug,
                    ReadyBreedingOutVaccin1 = item.ReadyBreedingOutVaccin,
                    ReadyBreedingOutOtherMaterial1 = item.ReadyBreedingOutOtherMaterial,
                    ReadyBreedingOutPigTransferValue1 = item.ReadyBreedingOutPigTransferValue,
                    ReadyBreedingOutCost1 = item.ReadyBreedingOutCost,
                    ReadyBreedingOutPigDeathValue1 = item.ReadyBreedingOutPigDeathValue,
                    ReadyBreedingOutPigBatchDeath1 = item.ReadyBreedingOutPigBatchDeath,
                    MissBreedingOutFeed1 = item.MissBreedingOutFeed,
                    MissBreedingOutVeterinaryDrug1 = item.MissBreedingOutVeterinaryDrug,
                    MissBreedingOutVaccin1 = item.MissBreedingOutVaccin,
                    MissBreedingOutOtherMaterial1 = item.MissBreedingOutOtherMaterial,
                    MissBreedingOutPigTransferValue1 = item.MissBreedingOutPigTransferValue,
                    MissBreedingOutSemen1 = item.MissBreedingOutSemen,
                    MissBreedingOutCost1 = item.MissBreedingOutCost,
                    MissBreedingOutPigDeathValue1 = item.MissBreedingOutPigDeathValue,
                    MissBreedingOutPigBatchDeath1 = item.MissBreedingOutPigBatchDeath,
                    AllocateOutAmount1 = item.AllocateOutAmount,
                    AllocateOutTotalDepreciation1 = item.AllocateOutTotalDepreciation,
                    AllocateOutFeed1 = item.AllocateOutFeed,
                    AllocateOutVeterinaryDrug1 = item.AllocateOutVeterinaryDrug,
                    AllocateOutVaccin1 = item.AllocateOutVaccin,
                    AllocateOutOtherMaterial1 = item.AllocateOutOtherMaterial,
                    AllocateOutPigTransferValue1 = item.AllocateOutPigTransferValue,
                    AllocateOutSemen1 = item.AllocateOutSemen,
                    AllocateOutCost1 = item.AllocateOutCost,
                    AllocateOutPigDeathValue1 = item.AllocateOutPigDeathValue,
                    AllocateOutPigBatchDeath1 = item.AllocateOutPigBatchDeath,
                    BatchOutAmount1 = item.BatchOutAmount,
                    BatchOutTotalDepreciation1 = item.BatchOutTotalDepreciation,
                    BatchOutFeed1 = item.BatchOutFeed,
                    BatchOutVeterinaryDrug1 = item.BatchOutVeterinaryDrug,
                    BatchOutVaccin1 = item.BatchOutVaccin,
                    BatchOutOtherMaterial1 = item.BatchOutOtherMaterial,
                    BatchOutPigTransferValue1 = item.BatchOutPigTransferValue,
                    BatchOutSemen1 = item.BatchOutSemen,
                    BatchOutCost1 = item.BatchOutCost,
                    BatchOutPigDeathValue1 = item.BatchOutPigDeathValue,
                    BatchOutPigBatchDeath1 = item.BatchOutPigBatchDeath,
                    SaleInsideOutAmount1 = item.SaleInsideOutAmount,
                    SaleInsideOutTotalDepreciation1 = item.SaleInsideOutTotalDepreciation,
                    SaleInsideOutFeed1 = item.SaleInsideOutFeed,
                    SaleInsideOutVeterinaryDrug1 = item.SaleInsideOutVeterinaryDrug,
                    SaleInsideOutVaccin1 = item.SaleInsideOutVaccin,
                    SaleInsideOutOtherMaterial1 = item.SaleInsideOutOtherMaterial,
                    SaleInsideOutPigTransferValue1 = item.SaleInsideOutPigTransferValue,
                    SaleInsideOutSemen1 = item.SaleInsideOutSemen,
                    SaleInsideOutCost1 = item.SaleInsideOutCost,
                    SaleInsideOutPigDeathValue1 = item.SaleInsideOutPigDeathValue,
                    SaleInsideOutPigBatchDeath1 = item.SaleInsideOutPigBatchDeath,
                    SaleOutsideOutAmount1 = item.SaleOutsideOutAmount,
                    SaleOutsideOutTotalDepreciation1 = item.SaleOutsideOutTotalDepreciation,
                    SaleOutsideOutFeed1 = item.SaleOutsideOutFeed,
                    SaleOutsideOutVeterinaryDrug1 = item.SaleOutsideOutVeterinaryDrug,
                    SaleOutsideOutVaccin1 = item.SaleOutsideOutVaccin,
                    SaleOutsideOutOtherMaterial1 = item.SaleOutsideOutOtherMaterial,
                    SaleOutsideOutPigTransferValue1 = item.SaleOutsideOutPigTransferValue,
                    SaleOutsideOutSemen1 = item.SaleOutsideOutSemen,
                    SaleOutsideOutCost1 = item.SaleOutsideOutCost,
                    SaleOutsideOutPigDeathValue1 = item.SaleOutsideOutPigDeathValue,
                    SaleOutsideOutPigBatchDeath1 = item.SaleOutsideOutPigBatchDeath,
                    DeathOutAmount1 = item.DeathOutAmount,
                    DeathOutTotalDepreciation1 = item.DeathOutTotalDepreciation,
                    DeathOutFeed1 = item.DeathOutFeed,
                    DeathOutVeterinaryDrug1 = item.DeathOutVeterinaryDrug,
                    DeathOutVaccin1 = item.DeathOutVaccin,
                    DeathOutOtherMaterial1 = item.DeathOutOtherMaterial,
                    DeathOutPigTransferValue1 = item.DeathOutPigTransferValue,
                    DeathOutSemen1 = item.DeathOutSemen,
                    DeathOutCost1 = item.DeathOutCost,
                    DeathOutPigDeathValue1 = item.DeathOutPigDeathValue,
                    DeathOutPigBatchDeath1 = item.DeathOutPigBatchDeath,
                    ReadyBreedingInAmount1 = item.ReadyBreedingInAmount,
                    ReadyBreedingInTotalDepreciation1 = item.ReadyBreedingInTotalDepreciation,
                    MissBreedingInAmount1 = item.MissBreedingInAmount,
                    MissBreedingInTotalDepreciation1 = item.MissBreedingInTotalDepreciation,
                    WeaningInAmount1 = item.WeaningInAmount,
                    WeaningInTotalDepreciation1 = item.WeaningInTotalDepreciation,
                    ReadyBreedingOutAmount1 = item.ReadyBreedingOutAmount,
                    ReadyBreedingOutTotalDepreciation1 = item.ReadyBreedingOutTotalDepreciation,
                    MissBreedingOutAmount1 = item.MissBreedingOutAmount,
                    MissBreedingOutTotalDepreciation1 = item.MissBreedingOutTotalDepreciation,
                    LactatingOutAmount1 = item.LactatingOutAmount,
                    LactatingOutTotalDepreciation1 = item.LactatingOutTotalDepreciation,
                    LactatingOutPigDeathValue1 = item.LactatingOutPigDeathValue,
                });
            }
            foreach (var item in result2)
            {
                list.Add(new PigCirculationSummary()
                {
                    DeptId = item.DeptId,
                    DeptName = item.DeptName,
                    NumericalOrder = item.NumericalOrder,
                    PigFarmId = item.PigFarmId,
                    PigFarmName = item.PigFarmName,
                    ProductLineId = item.ProductLineId,
                    ProductLineName = item.ProductLineName,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    BatchId = item.BatchId,
                    BatchName = item.BatchName,
                    PigId = item.PigId,
                    PigType = item.PigType,
                    PigTypeName = item.PigTypeName,
                    InsidePurchaseInPigletCost2 = item.InsidePurchaseInPigletCost,
                    OutPurchaseInPigletCost2 = item.OutPurchaseInPigletCost,
                    ChildbirthInPigletCost2 = item.ChildbirthInPigletCost,
                    LactatingInTransferValue2 = item.LactatingInTransferValue,
                    LactatingInFeed2 = item.LactatingInFeed,
                    LactatingInVeterinaryDrug2 = item.LactatingInVeterinaryDrug,
                    LactatingInVaccin2 = item.LactatingInVaccin,
                    LactatingInOtherMaterial2 = item.LactatingInOtherMaterial,
                    LactatingInCost2 = item.LactatingInCost,
                    LactatingInPigDeathValue2 = item.LactatingInPigDeathValue,
                    BatchInPigletCost2 = item.BatchInPigletCost,
                    BatchInFeed2 = item.BatchInFeed,
                    BatchInVeterinaryDrug2 = item.BatchInVeterinaryDrug,
                    BatchInVaccin2 = item.BatchInVaccin,
                    BatchInOtherMaterial2 = item.BatchInOtherMaterial,
                    BatchInCost2 = item.BatchInCost,
                    BatchInPigTransferValue2 = item.BatchInPigTransferValue,
                    BatchInPigDeathValue2 = item.BatchInPigDeathValue,
                    BatchInBatchInsideDeath2 = item.BatchInBatchInsideDeath,
                    BatchInBatchOutsideDeath2 = item.BatchInBatchOutsideDeath,
                    AllocateInPigletCost2 = item.AllocateInPigletCost,
                    AllocateInFeed2 = item.AllocateInFeed,
                    AllocateInVeterinaryDrug2 = item.AllocateInVeterinaryDrug,
                    AllocateInVaccin2 = item.AllocateInVaccin,
                    AllocateInOtherMaterial2 = item.AllocateInOtherMaterial,
                    AllocateInCost2 = item.AllocateInCost,
                    AllocateInPigTransferValue2 = item.AllocateInPigTransferValue,
                    AllocateInPigDeathValue2 = item.AllocateInPigDeathValue,
                    AllocateInBatchInsideDeath2 = item.AllocateInBatchInsideDeath,
                    AllocateInBatchOutsideDeath2 = item.AllocateInBatchOutsideDeath,
                    DeathInFeed2 = item.DeathInFeed,
                    DeathInVeterinaryDrug2 = item.DeathInVeterinaryDrug,
                    DeathInVaccin2 = item.DeathInVaccin,
                    DeathInOtherMaterial2 = item.DeathInOtherMaterial,
                    DeathInCost2 = item.DeathInCost,
                    DeathInPigTransferValue2 = item.DeathInPigTransferValue,
                    DeathInPigDeathValue2 = item.DeathInPigDeathValue,
                    DeathInBatchInsideDeath2 = item.DeathInBatchInsideDeath,
                    DeathInBatchOutsideDeath2 = item.DeathInBatchOutsideDeath,
                    SaleInsideInTransferValue2 = item.SaleInsideInTransferValue,
                    SaleInsideInFeed2 = item.SaleInsideInFeed,
                    SaleInsideInVeterinaryDrug2 = item.SaleInsideInVeterinaryDrug,
                    SaleInsideInVaccin2 = item.SaleInsideInVaccin,
                    SaleInsideInOtherMaterial2 = item.SaleInsideInOtherMaterial,
                    SaleInsideInCost2 = item.SaleInsideInCost,
                    SaleInsideInBatchInsideDeath2 = item.SaleInsideInBatchInsideDeath,
                    SaleOutsideInTransferValue2 = item.SaleOutsideInTransferValue,
                    SaleOutsideInFeed2 = item.SaleOutsideInFeed,
                    SaleOutsideInVeterinaryDrug2 = item.SaleOutsideInVeterinaryDrug,
                    SaleOutsideInVaccin2 = item.SaleOutsideInVaccin,
                    SaleOutsideInOtherMaterial2 = item.SaleOutsideInOtherMaterial,
                    SaleOutsideInCost2 = item.SaleOutsideInCost,
                    SaleOutsideInBatchInsideDeath2 = item.SaleOutsideInBatchInsideDeath,
                    CurrentCostFeed2 = item.CurrentCostFeed,
                    CurrentCostVeterinaryDrug2 = item.CurrentCostVeterinaryDrug,
                    CurrentCostVaccin2 = item.CurrentCostVaccin,
                    CurrentCostOtherMaterial2 = item.CurrentCostOtherMaterial,
                    CurrentCostCost2 = item.CurrentCostCost,
                    AcrossStepOutPigletCost2 = item.AcrossStepOutPigletCost,
                    AcrossStepOutFeed2 = item.AcrossStepOutFeed,
                    AcrossStepOutVeterinaryDrug2 = item.AcrossStepOutVeterinaryDrug,
                    AcrossStepOutVaccin2 = item.AcrossStepOutVaccin,
                    AcrossStepOutOtherMaterial2 = item.AcrossStepOutOtherMaterial,
                    AcrossStepOutCost2 = item.AcrossStepOutCost,
                    AcrossStepOutPigTransferValue2 = item.AcrossStepOutPigTransferValue,
                    AcrossStepOutPigDeathValue2 = item.AcrossStepOutPigDeathValue,
                    AcrossStepOutBatchInsideDeath2 = item.AcrossStepOutBatchInsideDeath,
                    AcrossStepOutBatchOutsideDeath2 = item.AcrossStepOutBatchOutsideDeath,
                    BatchOutPigletCost2 = item.BatchOutPigletCost,
                    BatchOutFeed2 = item.BatchOutFeed,
                    BatchOutVeterinaryDrug2 = item.BatchOutVeterinaryDrug,
                    BatchOutVaccin2 = item.BatchOutVaccin,
                    BatchOutOtherMaterial2 = item.BatchOutOtherMaterial,
                    BatchOutCost2 = item.BatchOutCost,
                    BatchOutPigTransferValue2 = item.BatchOutPigTransferValue,
                    BatchOutPigDeathValue2 = item.BatchOutPigDeathValue,
                    BatchOutBatchInsideDeath2 = item.BatchOutBatchInsideDeath,
                    BatchOutBatchOutsideDeath2 = item.BatchOutBatchOutsideDeath,
                    AllocateOutPigletCost2 = item.AllocateOutPigletCost,
                    AllocateOutFeed2 = item.AllocateOutFeed,
                    AllocateOutVeterinaryDrug2 = item.AllocateOutVeterinaryDrug,
                    AllocateOutVaccin2 = item.AllocateOutVaccin,
                    AllocateOutOtherMaterial2 = item.AllocateOutOtherMaterial,
                    AllocateOutCost2 = item.AllocateOutCost,
                    AllocateOutPigTransferValue2 = item.AllocateOutPigTransferValue,
                    AllocateOutPigDeathValue2 = item.AllocateOutPigDeathValue,
                    AllocateOutBatchInsideDeath2 = item.AllocateOutBatchInsideDeath,
                    AllocateOutBatchOutsideDeath2 = item.AllocateOutBatchOutsideDeath,
                    DeathOutPigletCost2 = item.DeathOutPigletCost,
                    DeathOutFeed2 = item.DeathOutFeed,
                    DeathOutVeterinaryDrug2 = item.DeathOutVeterinaryDrug,
                    DeathOutVaccin2 = item.DeathOutVaccin,
                    DeathOutOtherMaterial2 = item.DeathOutOtherMaterial,
                    DeathOutCost2 = item.DeathOutCost,
                    DeathOutPigTransferValue2 = item.DeathOutPigTransferValue,
                    DeathOutPigDeathValue2 = item.DeathOutPigDeathValue,
                    DeathOutBatchInsideDeath2 = item.DeathOutBatchInsideDeath,
                    DeathOutBatchOutsideDeath2 = item.DeathOutBatchOutsideDeath,
                    SaleInsideOutPigletCost2 = item.SaleInsideOutPigletCost,
                    SaleInsideOutFeed2 = item.SaleInsideOutFeed,
                    SaleInsideOutVeterinaryDrug2 = item.SaleInsideOutVeterinaryDrug,
                    SaleInsideOutVaccin2 = item.SaleInsideOutVaccin,
                    SaleInsideOutOtherMaterial2 = item.SaleInsideOutOtherMaterial,
                    SaleInsideOutCost2 = item.SaleInsideOutCost,
                    SaleInsideOutPigTransferValue2 = item.SaleInsideOutPigTransferValue,
                    SaleInsideOutPigDeathValue2 = item.SaleInsideOutPigDeathValue,
                    SaleInsideOutBatchInsideDeath2 = item.SaleInsideOutBatchInsideDeath,
                    SaleInsideOutBatchOutsideDeath2 = item.SaleInsideOutBatchOutsideDeath,
                    SaleOutsideOutPigletCost2 = item.SaleOutsideOutPigletCost,
                    SaleOutsideOutFeed2 = item.SaleOutsideOutFeed,
                    SaleOutsideOutVeterinaryDrug2 = item.SaleOutsideOutVeterinaryDrug,
                    SaleOutsideOutVaccin2 = item.SaleOutsideOutVaccin,
                    SaleOutsideOutOtherMaterial2 = item.SaleOutsideOutOtherMaterial,
                    SaleOutsideOutCost2 = item.SaleOutsideOutCost,
                    SaleOutsideOutPigTransferValue2 = item.SaleOutsideOutPigTransferValue,
                    SaleOutsideOutPigDeathValue2 = item.SaleOutsideOutPigDeathValue,
                    SaleOutsideOutBatchInsideDeath2 = item.SaleOutsideOutBatchInsideDeath,
                    SaleOutsideOutBatchOutsideDeath2 = item.SaleOutsideOutBatchOutsideDeath,
                    EmergenceOutPigletCost2 = item.EmergenceOutPigletCost,
                    EmergenceOutFeed2 = item.EmergenceOutFeed,
                    EmergenceOutVeterinaryDrug2 = item.EmergenceOutVeterinaryDrug,
                    EmergenceOutVaccin2 = item.EmergenceOutVaccin,
                    EmergenceOutOtherMaterial2 = item.EmergenceOutOtherMaterial,
                    EmergenceOutCost2 = item.EmergenceOutCost,
                    EmergenceOutPigTransferValue2 = item.EmergenceOutPigTransferValue,
                    EmergenceOutPigDeathValue2 = item.EmergenceOutPigDeathValue,
                    EmergenceOutBatchInsideDeath2 = item.EmergenceOutBatchInsideDeath,
                    EmergenceOutBatchOutsideDeath2 = item.EmergenceOutBatchOutsideDeath,

                });
            }
            foreach (var item in result3)
            {
                list.Add(new PigCirculationSummary()
                {
                    DeptId = item.DeptId,
                    DeptName = item.DeptName,
                    NumericalOrder = item.NumericalOrder,
                    PigFarmId = item.PigFarmId,
                    PigFarmName = item.PigFarmName,
                    ProductLineId = item.ProductLineId,
                    ProductLineName = item.ProductLineName,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    BatchId = item.BatchId,
                    BatchName = item.BatchName,
                    PigId = item.PigId,
                    PigType = item.PigType,
                    PigTypeName = item.PigTypeName,
                    AcrossStepInPigletCost3 = item.AcrossStepInPigletCost,
                    InsidePurchaseInPigletCost3 = item.InsidePurchaseInPigletCost,
                    OutPurchaseInPigletCost3 = item.OutPurchaseInPigletCost,
                    RecycleBreederInPigletCost3 = item.RecycleBreederInPigletCost,
                    BatchInPigletCost3 = item.BatchInPigletCost,
                    BatchInFeed3 = item.BatchInFeed,
                    BatchInVeterinaryDrug3 = item.BatchInVeterinaryDrug,
                    BatchInVaccin3 = item.BatchInVaccin,
                    BatchInOtherMaterial3 = item.BatchInOtherMaterial,
                    BatchInCost3 = item.BatchInCost,
                    BatchInBatchInsideDeath3 = item.BatchInBatchInsideDeath,
                    BatchInBatchOutsideDeath3 = item.BatchInBatchOutsideDeath,
                    AllocateInPigletCost3 = item.AllocateInPigletCost,
                    AllocateInFeed3 = item.AllocateInFeed,
                    AllocateInVeterinaryDrug3 = item.AllocateInVeterinaryDrug,
                    AllocateInVaccin3 = item.AllocateInVaccin,
                    AllocateInOtherMaterial3 = item.AllocateInOtherMaterial,
                    AllocateInCost3 = item.AllocateInCost,
                    AllocateInBatchInsideDeath3 = item.AllocateInBatchInsideDeath,
                    AllocateInBatchOutsideDeath3 = item.AllocateInBatchOutsideDeath,
                    DeathInFeed3 = item.DeathInFeed,
                    DeathInVeterinaryDrug3 = item.DeathInVeterinaryDrug,
                    DeathInVaccin3 = item.DeathInVaccin,
                    DeathInOtherMaterial3 = item.DeathInOtherMaterial,
                    DeathInCost3 = item.DeathInCost,
                    DeathInBatchInsideDeath3 = item.DeathInBatchInsideDeath,
                    DeathInBatchOutsideDeath3 = item.DeathInBatchOutsideDeath,
                    CurrentCostFeed3 = item.CurrentCostFeed,
                    CurrentCostVeterinaryDrug3 = item.CurrentCostVeterinaryDrug,
                    CurrentCostVaccin3 = item.CurrentCostVaccin,
                    CurrentCostOtherMaterial3 = item.CurrentCostOtherMaterial,
                    CurrentCostCost3 = item.CurrentCostCost,
                    AcrossStepOutPigletCost3 = item.AcrossStepOutPigletCost,
                    AcrossStepOutFeed3 = item.AcrossStepOutFeed,
                    AcrossStepOutVeterinaryDrug3 = item.AcrossStepOutVeterinaryDrug,
                    AcrossStepOutVaccin3 = item.AcrossStepOutVaccin,
                    AcrossStepOutOtherMaterial3 = item.AcrossStepOutOtherMaterial,
                    AcrossStepOutCost3 = item.AcrossStepOutCost,
                    AcrossStepOutBatchInsideDeath3 = item.AcrossStepOutBatchInsideDeath,
                    AcrossStepOutBatchOutsideDeath3 = item.AcrossStepOutBatchOutsideDeath,
                    BatchOutPigletCost3 = item.BatchOutPigletCost,
                    BatchOutFeed3 = item.BatchOutFeed,
                    BatchOutVeterinaryDrug3 = item.BatchOutVeterinaryDrug,
                    BatchOutVaccin3 = item.BatchOutVaccin,
                    BatchOutOtherMaterial3 = item.BatchOutOtherMaterial,
                    BatchOutCost3 = item.BatchOutCost,
                    BatchOutBatchInsideDeath3 = item.BatchOutBatchInsideDeath,
                    BatchOutBatchOutsideDeath3 = item.BatchOutBatchOutsideDeath,
                    AllocateOutPigletCost3 = item.AllocateOutPigletCost,
                    AllocateOutFeed3 = item.AllocateOutFeed,
                    AllocateOutVeterinaryDrug3 = item.AllocateOutVeterinaryDrug,
                    AllocateOutVaccin3 = item.AllocateOutVaccin,
                    AllocateOutOtherMaterial3 = item.AllocateOutOtherMaterial,
                    AllocateOutCost3 = item.AllocateOutCost,
                    AllocateOutBatchInsideDeath3 = item.AllocateOutBatchInsideDeath,
                    AllocateOutBatchOutsideDeath3 = item.AllocateOutBatchOutsideDeath,
                    DeathOutPigletCost3 = item.DeathOutPigletCost,
                    DeathOutFeed3 = item.DeathOutFeed,
                    DeathOutVeterinaryDrug3 = item.DeathOutVeterinaryDrug,
                    DeathOutVaccin3 = item.DeathOutVaccin,
                    DeathOutOtherMaterial3 = item.DeathOutOtherMaterial,
                    DeathOutCost3 = item.DeathOutCost,
                    DeathOutBatchInsideDeath3 = item.DeathOutBatchInsideDeath,
                    DeathOutBatchOutsideDeath3 = item.DeathOutBatchOutsideDeath,
                    SaleInsideOutPigletCost3 = item.SaleInsideOutPigletCost,
                    SaleInsideOutFeed3 = item.SaleInsideOutFeed,
                    SaleInsideOutVeterinaryDrug3 = item.SaleInsideOutVeterinaryDrug,
                    SaleInsideOutVaccin3 = item.SaleInsideOutVaccin,
                    SaleInsideOutOtherMaterial3 = item.SaleInsideOutOtherMaterial,
                    SaleInsideOutCost3 = item.SaleInsideOutCost,
                    SaleInsideOutBatchInsideDeath3 = item.SaleInsideOutBatchInsideDeath,
                    SaleInsideOutBatchOutsideDeath3 = item.SaleInsideOutBatchOutsideDeath,
                    SaleOutsideOutPigletCost3 = item.SaleOutsideOutPigletCost,
                    SaleOutsideOutFeed3 = item.SaleOutsideOutFeed,
                    SaleOutsideOutVeterinaryDrug3 = item.SaleOutsideOutVeterinaryDrug,
                    SaleOutsideOutVaccin3 = item.SaleOutsideOutVaccin,
                    SaleOutsideOutOtherMaterial3 = item.SaleOutsideOutOtherMaterial,
                    SaleOutsideOutCost3 = item.SaleOutsideOutCost,
                    SaleOutsideOutBatchInsideDeath3 = item.SaleOutsideOutBatchInsideDeath,
                    SaleOutsideOutBatchOutsideDeath3 = item.SaleOutsideOutBatchOutsideDeath,
                    EmergenceOutPigletCost3 = item.EmergenceOutPigletCost,
                    EmergenceOutFeed3 = item.EmergenceOutFeed,
                    EmergenceOutVeterinaryDrug3 = item.EmergenceOutVeterinaryDrug,
                    EmergenceOutVaccin3 = item.EmergenceOutVaccin,
                    EmergenceOutOtherMaterial3 = item.EmergenceOutOtherMaterial,
                    EmergenceOutCost3 = item.EmergenceOutCost,
                    EmergenceOutBatchInsideDeath3 = item.EmergenceOutBatchInsideDeath,
                    EmergenceOutBatchOutsideDeath3 = item.EmergenceOutBatchOutsideDeath,
                });
            }
            foreach (var item in result4)
            {
                list.Add(new PigCirculationSummary()
                {
                    DeptId = item.DeptId,
                    DeptName = item.DeptName,
                    NumericalOrder = item.NumericalOrder,
                    PigFarmId = item.PigFarmId,
                    PigFarmName = item.PigFarmName,
                    ProductLineId = item.ProductLineId,
                    ProductLineName = item.ProductLineName,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    BatchId = item.BatchId,
                    BatchName = item.BatchName,
                    PigId = item.PigId,
                    PigType = item.PigType,
                    PigTypeName = item.PigTypeName,
                    AcrossStepInPigletCost4 = item.AcrossStepInPigletCost,
                    InsidePurchaseInPigletCost4 = item.InsidePurchaseInPigletCost,
                    OutPurchaseInPigletCost4 = item.OutPurchaseInPigletCost,
                    RecycleBreederInPigletCost4 = item.RecycleBreederInPigletCost,
                    BatchInPigletCost4 = item.BatchInPigletCost,
                    BatchInFeed4 = item.BatchInFeed,
                    BatchInVeterinaryDrug4 = item.BatchInVeterinaryDrug,
                    BatchInVaccin4 = item.BatchInVaccin,
                    BatchInOtherMaterial4 = item.BatchInOtherMaterial,
                    BatchInCost4 = item.BatchInCost,
                    BatchInBatchInsideDeath4 = item.BatchInBatchInsideDeath,
                    BatchInBatchOutsideDeath4 = item.BatchInBatchOutsideDeath,
                    AllocateInPigletCost4 = item.AllocateInPigletCost,
                    AllocateInFeed4 = item.AllocateInFeed,
                    AllocateInVeterinaryDrug4 = item.AllocateInVeterinaryDrug,
                    AllocateInVaccin4 = item.AllocateInVaccin,
                    AllocateInOtherMaterial4 = item.AllocateInOtherMaterial,
                    AllocateInCost4 = item.AllocateInCost,
                    AllocateInBatchInsideDeath4 = item.AllocateInBatchInsideDeath,
                    AllocateInBatchOutsideDeath4 = item.AllocateInBatchOutsideDeath,
                    DeathInFeed4 = item.DeathInFeed,
                    DeathInVeterinaryDrug4 = item.DeathInVeterinaryDrug,
                    DeathInVaccin4 = item.DeathInVaccin,
                    DeathInOtherMaterial4 = item.DeathInOtherMaterial,
                    DeathInCost4 = item.DeathInCost,
                    DeathInBatchInsideDeath4 = item.DeathInBatchInsideDeath,
                    DeathInBatchOutsideDeath4 = item.DeathInBatchOutsideDeath,
                    CurrentCostFeed4 = item.CurrentCostFeed,
                    CurrentCostVeterinaryDrug4 = item.CurrentCostVeterinaryDrug,
                    CurrentCostVaccin4 = item.CurrentCostVaccin,
                    CurrentCostOtherMaterial4 = item.CurrentCostOtherMaterial,
                    CurrentCostCost4 = item.CurrentCostCost,
                    AcrossStepOutPigletCost4 = item.AcrossStepOutPigletCost,
                    AcrossStepOutFeed4 = item.AcrossStepOutFeed,
                    AcrossStepOutVeterinaryDrug4 = item.AcrossStepOutVeterinaryDrug,
                    AcrossStepOutVaccin4 = item.AcrossStepOutVaccin,
                    AcrossStepOutOtherMaterial4 = item.AcrossStepOutOtherMaterial,
                    AcrossStepOutCost4 = item.AcrossStepOutCost,
                    AcrossStepOutBatchInsideDeath4 = item.AcrossStepOutBatchInsideDeath,
                    AcrossStepOutBatchOutsideDeath4 = item.AcrossStepOutBatchOutsideDeath,
                    BatchOutPigletCost4 = item.BatchOutPigletCost,
                    BatchOutFeed4 = item.BatchOutFeed,
                    BatchOutVeterinaryDrug4 = item.BatchOutVeterinaryDrug,
                    BatchOutVaccin4 = item.BatchOutVaccin,
                    BatchOutOtherMaterial4 = item.BatchOutOtherMaterial,
                    BatchOutCost4 = item.BatchOutCost,
                    BatchOutBatchInsideDeath4 = item.BatchOutBatchInsideDeath,
                    BatchOutBatchOutsideDeath4 = item.BatchOutBatchOutsideDeath,
                    AllocateOutPigletCost4 = item.AllocateOutPigletCost,
                    AllocateOutFeed4 = item.AllocateOutFeed,
                    AllocateOutVeterinaryDrug4 = item.AllocateOutVeterinaryDrug,
                    AllocateOutVaccin4 = item.AllocateOutVaccin,
                    AllocateOutOtherMaterial4 = item.AllocateOutOtherMaterial,
                    AllocateOutCost4 = item.AllocateOutCost,
                    AllocateOutBatchInsideDeath4 = item.AllocateOutBatchInsideDeath,
                    AllocateOutBatchOutsideDeath4 = item.AllocateOutBatchOutsideDeath,
                    DeathOutPigletCost4 = item.DeathOutPigletCost,
                    DeathOutFeed4 = item.DeathOutFeed,
                    DeathOutVeterinaryDrug4 = item.DeathOutVeterinaryDrug,
                    DeathOutVaccin4 = item.DeathOutVaccin,
                    DeathOutOtherMaterial4 = item.DeathOutOtherMaterial,
                    DeathOutCost4 = item.DeathOutCost,
                    DeathOutBatchInsideDeath4 = item.DeathOutBatchInsideDeath,
                    DeathOutBatchOutsideDeath4 = item.DeathOutBatchOutsideDeath,
                    SaleInsideOutPigletCost4 = item.SaleInsideOutPigletCost,
                    SaleInsideOutFeed4 = item.SaleInsideOutFeed,
                    SaleInsideOutVeterinaryDrug4 = item.SaleInsideOutVeterinaryDrug,
                    SaleInsideOutVaccin4 = item.SaleInsideOutVaccin,
                    SaleInsideOutOtherMaterial4 = item.SaleInsideOutOtherMaterial,
                    SaleInsideOutCost4 = item.SaleInsideOutCost,
                    SaleInsideOutBatchInsideDeath4 = item.SaleInsideOutBatchInsideDeath,
                    SaleInsideOutBatchOutsideDeath4 = item.SaleInsideOutBatchOutsideDeath,
                    SaleOutsideOutPigletCost4 = item.SaleOutsideOutPigletCost,
                    SaleOutsideOutFeed4 = item.SaleOutsideOutFeed,
                    SaleOutsideOutVeterinaryDrug4 = item.SaleOutsideOutVeterinaryDrug,
                    SaleOutsideOutVaccin4 = item.SaleOutsideOutVaccin,
                    SaleOutsideOutOtherMaterial4 = item.SaleOutsideOutOtherMaterial,
                    SaleOutsideOutCost4 = item.SaleOutsideOutCost,
                    SaleOutsideOutBatchInsideDeath4 = item.SaleOutsideOutBatchInsideDeath,
                    SaleOutsideOutBatchOutsideDeath4 = item.SaleOutsideOutBatchOutsideDeath,
                });
            }
            foreach (var item in result5)
            {
                list.Add(new PigCirculationSummary()
                {
                    DeptId = item.DeptID,
                    DeptName = item.DeptName,
                    PigFarmId = item.PigFarmID,
                    PigFarmName = item.PigFarmName,
                    ProductLineId = item.ProductLineID,
                    ProductLineName = item.ProductLineName,
                    ProductId = item.ProductID,
                    ProductName = item.ProductName,
                    BatchId = item.BatchID,
                    BatchName = item.BatchName,
                    PigId = item.PigID,
                    PigType = item.PigType,
                    FeedDays5 = item.FeedDays,
                    Depreciated5 = item.Depreciated,
                    MaterialCost5 = item.MaterialCost,
                    MedicantCost5 = item.MedicantCost,
                    VaccineCost5 = item.VaccineCost,
                    OtherCost5 = item.OtherCost,
                    FeeCost5 = item.FeeCost,
                    SumCost5 = item.SumCost,
                });
            }
            foreach (var item in result6)
            {
                list.Add(new PigCirculationSummary()
                {
                    DeptId = item.DeptID,
                    DeptName = item.DeptName,
                    FarmerID = item.FarmerID,
                    FarmerName = item.FarmerName,
                    ProductLineId = "",
                    ProductLineName = "",
                    ProductId = item.ProductID,
                    ProductName = item.ProductName,
                    BatchId = item.BatchID,
                    BatchName = item.BatchName,
                    PigId = "",
                    PigType = "",
                    AdjustCount6 = item.AdjustCount,
                    BeginPigCount6 = item.BeginPigCount,
                    BeginPigCost6 = item.BeginPigCost,
                    BeginMaterial6 = item.BeginMaterial,
                    BeginMedicant6 = item.BeginMedicant,
                    BeginVaccine6 = item.BeginVaccine,
                    BeginOther6 = item.BeginOther,
                    BeginInnerDeath6 = item.BeginInnerDeath,
                    BeginOuterDeath6 = item.BeginOuterDeath,
                    BeginDirectFee6 = item.BeginDirectFee,
                    BeginBuildingFee6 = item.BeginBuildingFee,
                    BeginPredictFeedFee6 = item.BeginPredictFeedFee,
                    BeginAdjustFeedFee6 = item.BeginAdjustFeedFee,
                    BeginAdditionFee6 = item.BeginAdditionFee,
                    BeginHoldUpFee6 = item.BeginHoldUpFee,
                    CurrentPigCount6 = item.CurrentPigCount,
                    CurrentPigCost6 = item.CurrentPigCost,
                    CurrentMaterial6 = item.CurrentMaterial,
                    CurrentMedicant6 = item.CurrentMedicant,
                    CurrentVaccine6 = item.CurrentVaccine,
                    CurrentOther6 = item.CurrentOther,
                    CurrentInnerDeath6 = item.CurrentInnerDeath,
                    CurrentOuterDeath6 = item.CurrentOuterDeath,
                    CurrentDirectFee6 = item.CurrentDirectFee,
                    CurrentBuildingFee6 = item.CurrentBuildingFee,
                    CurrentPredictFeedFee6 = item.CurrentPredictFeedFee,
                    CurrentAdjustFeedFee6 = item.CurrentAdjustFeedFee,
                    CurrentAdditionFee6 = item.CurrentAdditionFee,
                    CurrentHoldUpFee6 = item.CurrentHoldUpFee,
                    RecyclePigCount6 = item.RecyclePigCount,
                    RecyclePigCost6 = item.RecyclePigCost,
                    RecycleMaterial6 = item.RecycleMaterial,
                    RecycleMedicant6 = item.RecycleMedicant,
                    RecycleVaccine6 = item.RecycleVaccine,
                    RecycleOther6 = item.RecycleOther,
                    RecycleInnerDeath6 = item.RecycleInnerDeath,
                    RecycleOuterDeath6 = item.RecycleOuterDeath,
                    RecycleDirectFee6 = item.RecycleDirectFee,
                    RecycleBuildingFee6 = item.RecycleBuildingFee,
                    RecyclePredictFeedFee6 = item.RecyclePredictFeedFee,
                    RecycleAdjustFeedFee6 = item.RecycleAdjustFeedFee,
                    RecycleAdditionFee6 = item.RecycleAdditionFee,
                    RecycleHoldUpFee6 = item.RecycleHoldUpFee,
                    DeathPigCount6 = item.DeathPigCount,
                    DeathPigCost6 = item.DeathPigCost,
                    DeathMaterial6 = item.DeathMaterial,
                    DeathMedicant6 = item.DeathMedicant,
                    DeathVaccine6 = item.DeathVaccine,
                    DeathOther6 = item.DeathOther,
                    DeathInnerDeath6 = item.DeathInnerDeath,
                    DeathOuterDeath6 = item.DeathOuterDeath,
                    DeathDirectFee6 = item.DeathDirectFee,
                    DeathBuildingFee6 = item.DeathBuildingFee,
                    DeathPredictFeedFee6 = item.DeathPredictFeedFee,
                    DeathAdjustFeedFee6 = item.DeathAdjustFeedFee,
                    DeathAdditionFee6 = item.DeathAdditionFee,
                    DeathHoldUpFee6 = item.DeathHoldUpFee,
                    EndPigCount6 = item.EndPigCount,
                    EndPigCost6 = item.EndPigCost,
                    EndMaterial6 = item.EndMaterial,
                    EndMedicant6 = item.EndMedicant,
                    EndVaccine6 = item.EndVaccine,
                    EndOther6 = item.EndOther,
                    EndInnerDeath6 = item.EndInnerDeath,
                    EndOuterDeath6 = item.EndOuterDeath,
                    EndDirectFee6 = item.EndDirectFee,
                    EndBuildingFee6 = item.EndBuildingFee,
                    EndPredictFeedFee6 = item.EndPredictFeedFee,
                    EndAdjustFeedFee6 = item.EndAdjustFeedFee,
                    EndAdditionFee6 = item.EndAdditionFee,
                    EndHoldUpFee6 = item.EndHoldUpFee
                });
            }
            return list;
        }
        #endregion
        public List<PigCirculationSummary> dynamicGroupbySummary(List<PigCirculationSummary> summaryList, GroupModel groupModel, bool isEnter = false)
        {
            if (isEnter)
            {
                List<PigCirculationSummary> list = new List<PigCirculationSummary>()
                {
                     new PigCirculationSummary(){
                        DeptId = summaryList.FirstOrDefault()?.DeptId,
                        ProductId = summaryList.FirstOrDefault()?.ProductId,
                        FarmerID=summaryList.FirstOrDefault()?.FarmerID,
                        FallbackInAmount1= summaryList.Sum(n => n.FallbackInAmount1),
                        InsidePurchaseInAmount1 = summaryList.Sum(n => n.InsidePurchaseInAmount1),
                        OutPurchaseInAmount1 = summaryList.Sum(n => n.OutPurchaseInAmount1),
                        ReadyBreedingInFeed1 = summaryList.Sum(n => n.ReadyBreedingInFeed1),
                        ReadyBreedingInVeterinaryDrug1 = summaryList.Sum(n => n.ReadyBreedingInVeterinaryDrug1),
                        ReadyBreedingInVaccin1 = summaryList.Sum(n => n.ReadyBreedingInVaccin1),
                        ReadyBreedingInOtherMaterial1 = summaryList.Sum(n => n.ReadyBreedingInOtherMaterial1),
                        ReadyBreedingInPigTransferValue1 = summaryList.Sum(n => n.ReadyBreedingInPigTransferValue1),
                        ReadyBreedingInCost1 = summaryList.Sum(n => n.ReadyBreedingInCost1),
                        ReadyBreedingInPigDeathValue1 = summaryList.Sum(n => n.ReadyBreedingInPigDeathValue1),
                        ReadyBreedingInPigBatchDeath1 = summaryList.Sum(n => n.ReadyBreedingInPigBatchDeath1),
                        MissBreedingInFeed1 = summaryList.Sum(n => n.MissBreedingInFeed1),
                        MissBreedingInVeterinaryDrug1 = summaryList.Sum(n => n.MissBreedingInVeterinaryDrug1),
                        MissBreedingInVaccin1 = summaryList.Sum(n => n.MissBreedingInVaccin1),
                        MissBreedingInOtherMaterial1 = summaryList.Sum(n => n.MissBreedingInOtherMaterial1),
                        MissBreedingInPigTransferValue1 = summaryList.Sum(n => n.MissBreedingInPigTransferValue1),
                        MissBreedingInSemen1 = summaryList.Sum(n => n.MissBreedingInSemen1),
                        MissBreedingInCost1 = summaryList.Sum(n => n.MissBreedingInCost1),
                        MissBreedingInPigDeathValue1 = summaryList.Sum(n => n.MissBreedingInPigDeathValue1),
                        MissBreedingInPigBatchDeath1 = summaryList.Sum(n => n.MissBreedingInPigBatchDeath1),
                        AllocateInAmount1 = summaryList.Sum(n => n.AllocateInAmount1),
                        AllocateInTotalDepreciation1 = summaryList.Sum(n => n.AllocateInTotalDepreciation1),
                        AllocateInFeed1 = summaryList.Sum(n => n.AllocateInFeed1),
                        AllocateInVeterinaryDrug1 = summaryList.Sum(n => n.AllocateInVeterinaryDrug1),
                        AllocateInVaccin1 = summaryList.Sum(n => n.AllocateInVaccin1),
                        AllocateInOtherMaterial1 = summaryList.Sum(n => n.AllocateInOtherMaterial1),
                        AllocateInPigTransferValue1 = summaryList.Sum(n => n.AllocateInPigTransferValue1),
                        AllocateInSemen1 = summaryList.Sum(n => n.AllocateInSemen1),
                        AllocateInCost1 = summaryList.Sum(n => n.AllocateInCost1),
                        AllocateInPigDeathValue1 = summaryList.Sum(n => n.AllocateInPigDeathValue1),
                        AllocateInPigBatchDeath1 = summaryList.Sum(n => n.AllocateInPigBatchDeath1),
                        BatchInAmount1 = summaryList.Sum(n => n.BatchInAmount1),
                        BatchInTotalDepreciation1 = summaryList.Sum(n => n.BatchInTotalDepreciation1),
                        BatchInFeed1 = summaryList.Sum(n => n.BatchInFeed1),
                        BatchInVeterinaryDrug1 = summaryList.Sum(n => n.BatchInVeterinaryDrug1),
                        BatchInVaccin1 = summaryList.Sum(n => n.BatchInVaccin1),
                        BatchInOtherMaterial1 = summaryList.Sum(n => n.BatchInOtherMaterial1),
                        BatchInPigTransferValue1 = summaryList.Sum(n => n.BatchInPigTransferValue1),
                        BatchInSemen1 = summaryList.Sum(n => n.BatchInSemen1),
                        BatchInCost1 = summaryList.Sum(n => n.BatchInCost1),
                        BatchInPigDeathValue1 = summaryList.Sum(n => n.BatchInPigDeathValue1),
                        BatchInPigBatchDeath1 = summaryList.Sum(n => n.BatchInPigBatchDeath1),
                        SaleInsideInFeed1 = summaryList.Sum(n => n.SaleInsideInFeed1),
                        SaleInsideInVeterinaryDrug1 = summaryList.Sum(n => n.SaleInsideInVeterinaryDrug1),
                        SaleInsideInVaccin1 = summaryList.Sum(n => n.SaleInsideInVaccin1),
                        SaleInsideInOtherMaterial1 = summaryList.Sum(n => n.SaleInsideInOtherMaterial1),
                        SaleInsideInPigTransferValue1 = summaryList.Sum(n => n.SaleInsideInPigTransferValue1),
                        SaleInsideInSemen1 = summaryList.Sum(n => n.SaleInsideInSemen1),
                        SaleInsideInCost1 = summaryList.Sum(n => n.SaleInsideInCost1),
                        SaleInsideInPigDeathValue1 = summaryList.Sum(n => n.SaleInsideInPigDeathValue1),
                        SaleInsideInPigBatchDeath1 = summaryList.Sum(n => n.SaleInsideInPigBatchDeath1),
                        SaleOutsideInFeed1 = summaryList.Sum(n => n.SaleOutsideInFeed1),
                        SaleOutsideInVeterinaryDrug1 = summaryList.Sum(n => n.SaleOutsideInVeterinaryDrug1),
                        SaleOutsideInVaccin1 = summaryList.Sum(n => n.SaleOutsideInVaccin1),
                        SaleOutsideInOtherMaterial1 = summaryList.Sum(n => n.SaleOutsideInOtherMaterial1),
                        SaleOutsideInPigTransferValue1 = summaryList.Sum(n => n.SaleOutsideInPigTransferValue1),
                        SaleOutsideInSemen1 = summaryList.Sum(n => n.SaleOutsideInSemen1),
                        SaleOutsideInCost1 = summaryList.Sum(n => n.SaleOutsideInCost1),
                        SaleOutsideInPigDeathValue1 = summaryList.Sum(n => n.SaleOutsideInPigDeathValue1),
                        SaleOutsideInPigBatchDeath1 = summaryList.Sum(n => n.SaleOutsideInPigBatchDeath1),
                        DeathInFeed1 = summaryList.Sum(n => n.DeathInFeed1),
                        DeathInVeterinaryDrug1 = summaryList.Sum(n => n.DeathInVeterinaryDrug1),
                        DeathInVaccin1 = summaryList.Sum(n => n.DeathInVaccin1),
                        DeathInOtherMaterial1 = summaryList.Sum(n => n.DeathInOtherMaterial1),
                        DeathInPigTransferValue1 = summaryList.Sum(n => n.DeathInPigTransferValue1),
                        DeathInSemen1 = summaryList.Sum(n => n.DeathInSemen1),
                        DeathInCost1 = summaryList.Sum(n => n.DeathInCost1),
                        DeathInPigDeathValue1 = summaryList.Sum(n => n.DeathInPigDeathValue1),
                        DeathInPigBatchDeath1 = summaryList.Sum(n => n.DeathInPigBatchDeath1),
                        CurrentFeed1 = summaryList.Sum(n => n.CurrentFeed1),
                        CurrentVeterinaryDrug1 = summaryList.Sum(n => n.CurrentVeterinaryDrug1),
                        CurrentVaccin1 = summaryList.Sum(n => n.CurrentVaccin1),
                        CurrentOtherMaterial1 = summaryList.Sum(n => n.CurrentOtherMaterial1),
                        CurrentPigTransferValue1 = summaryList.Sum(n => n.CurrentPigTransferValue1),
                        CurrentSemen1 = summaryList.Sum(n => n.CurrentSemen1),
                        CurrentCost1 = summaryList.Sum(n => n.CurrentCost1),
                        ChildbirthOutFeed1 = summaryList.Sum(n => n.ChildbirthOutFeed1),
                        ChildbirthOutVeterinaryDrug1 = summaryList.Sum(n => n.ChildbirthOutVeterinaryDrug1),
                        ChildbirthOutVaccin1 = summaryList.Sum(n => n.ChildbirthOutVaccin1),
                        ChildbirthOutOtherMaterial1 = summaryList.Sum(n => n.ChildbirthOutOtherMaterial1),
                        ChildbirthOutPigTransferValue1 = summaryList.Sum(n => n.ChildbirthOutPigTransferValue1),
                        ChildbirthOutSemen1 = summaryList.Sum(n => n.ChildbirthOutSemen1),
                        ChildbirthOutCost1 = summaryList.Sum(n => n.ChildbirthOutCost1),
                        ChildbirthOutPigDeathValue1 = summaryList.Sum(n => n.ChildbirthOutPigDeathValue1),
                        ChildbirthOutPigBatchDeath1 = summaryList.Sum(n => n.ChildbirthOutPigBatchDeath1),
                        LactatingOutFeed1 = summaryList.Sum(n => n.LactatingOutFeed1),
                        LactatingOutVeterinaryDrug1 = summaryList.Sum(n => n.LactatingOutVeterinaryDrug1),
                        LactatingOutVaccin1 = summaryList.Sum(n => n.LactatingOutVaccin1),
                        LactatingOutOtherMaterial1 = summaryList.Sum(n => n.LactatingOutOtherMaterial1),
                        LactatingOutPigTransferValue1 = summaryList.Sum(n => n.LactatingOutPigTransferValue1),
                        LactatingOutCost1 = summaryList.Sum(n => n.LactatingOutCost1),
                        ReadyBreedingOutFeed1 = summaryList.Sum(n => n.ReadyBreedingOutFeed1),
                        ReadyBreedingOutVeterinaryDrug1 = summaryList.Sum(n => n.ReadyBreedingOutVeterinaryDrug1),
                        ReadyBreedingOutVaccin1 = summaryList.Sum(n => n.ReadyBreedingOutVaccin1),
                        ReadyBreedingOutOtherMaterial1 = summaryList.Sum(n => n.ReadyBreedingOutOtherMaterial1),
                        ReadyBreedingOutPigTransferValue1 = summaryList.Sum(n => n.ReadyBreedingOutPigTransferValue1),
                        ReadyBreedingOutCost1 = summaryList.Sum(n => n.ReadyBreedingOutCost1),
                        ReadyBreedingOutPigDeathValue1 = summaryList.Sum(n => n.ReadyBreedingOutPigDeathValue1),
                        ReadyBreedingOutPigBatchDeath1 = summaryList.Sum(n => n.ReadyBreedingOutPigBatchDeath1),
                        MissBreedingOutFeed1 = summaryList.Sum(n => n.MissBreedingOutFeed1),
                        MissBreedingOutVeterinaryDrug1 = summaryList.Sum(n => n.MissBreedingOutVeterinaryDrug1),
                        MissBreedingOutVaccin1 = summaryList.Sum(n => n.MissBreedingOutVaccin1),
                        MissBreedingOutOtherMaterial1 = summaryList.Sum(n => n.MissBreedingOutOtherMaterial1),
                        MissBreedingOutPigTransferValue1 = summaryList.Sum(n => n.MissBreedingOutPigTransferValue1),
                        MissBreedingOutSemen1 = summaryList.Sum(n => n.MissBreedingOutSemen1),
                        MissBreedingOutCost1 = summaryList.Sum(n => n.MissBreedingOutCost1),
                        MissBreedingOutPigDeathValue1 = summaryList.Sum(n => n.MissBreedingOutPigDeathValue1),
                        MissBreedingOutPigBatchDeath1 = summaryList.Sum(n => n.MissBreedingOutPigBatchDeath1),
                        AllocateOutAmount1 = summaryList.Sum(n => n.AllocateOutAmount1),
                        AllocateOutTotalDepreciation1 = summaryList.Sum(n => n.AllocateOutTotalDepreciation1),
                        AllocateOutFeed1 = summaryList.Sum(n => n.AllocateOutFeed1),
                        AllocateOutVeterinaryDrug1 = summaryList.Sum(n => n.AllocateOutVeterinaryDrug1),
                        AllocateOutVaccin1 = summaryList.Sum(n => n.AllocateOutVaccin1),
                        AllocateOutOtherMaterial1 = summaryList.Sum(n => n.AllocateOutOtherMaterial1),
                        AllocateOutPigTransferValue1 = summaryList.Sum(n => n.AllocateOutPigTransferValue1),
                        AllocateOutSemen1 = summaryList.Sum(n => n.AllocateOutSemen1),
                        AllocateOutCost1 = summaryList.Sum(n => n.AllocateOutCost1),
                        AllocateOutPigDeathValue1 = summaryList.Sum(n => n.AllocateOutPigDeathValue1),
                        AllocateOutPigBatchDeath1 = summaryList.Sum(n => n.AllocateOutPigBatchDeath1),
                        BatchOutAmount1 = summaryList.Sum(n => n.BatchOutAmount1),
                        BatchOutTotalDepreciation1 = summaryList.Sum(n => n.BatchOutTotalDepreciation1),
                        BatchOutFeed1 = summaryList.Sum(n => n.BatchOutFeed1),
                        BatchOutVeterinaryDrug1 = summaryList.Sum(n => n.BatchOutVeterinaryDrug1),
                        BatchOutVaccin1 = summaryList.Sum(n => n.BatchOutVaccin1),
                        BatchOutOtherMaterial1 = summaryList.Sum(n => n.BatchOutOtherMaterial1),
                        BatchOutPigTransferValue1 = summaryList.Sum(n => n.BatchOutPigTransferValue1),
                        BatchOutSemen1 = summaryList.Sum(n => n.BatchOutSemen1),
                        BatchOutCost1 = summaryList.Sum(n => n.BatchOutCost1),
                        BatchOutPigDeathValue1 = summaryList.Sum(n => n.BatchOutPigDeathValue1),
                        BatchOutPigBatchDeath1 = summaryList.Sum(n => n.BatchOutPigBatchDeath1),
                        SaleInsideOutAmount1 = summaryList.Sum(n => n.SaleInsideOutAmount1),
                        SaleInsideOutTotalDepreciation1 = summaryList.Sum(n => n.SaleInsideOutTotalDepreciation1),
                        SaleInsideOutFeed1 = summaryList.Sum(n => n.SaleInsideOutFeed1),
                        SaleInsideOutVeterinaryDrug1 = summaryList.Sum(n => n.SaleInsideOutVeterinaryDrug1),
                        SaleInsideOutVaccin1 = summaryList.Sum(n => n.SaleInsideOutVaccin1),
                        SaleInsideOutOtherMaterial1 = summaryList.Sum(n => n.SaleInsideOutOtherMaterial1),
                        SaleInsideOutPigTransferValue1 = summaryList.Sum(n => n.SaleInsideOutPigTransferValue1),
                        SaleInsideOutSemen1 = summaryList.Sum(n => n.SaleInsideOutSemen1),
                        SaleInsideOutCost1 = summaryList.Sum(n => n.SaleInsideOutCost1),
                        SaleInsideOutPigDeathValue1 = summaryList.Sum(n => n.SaleInsideOutPigDeathValue1),
                        SaleInsideOutPigBatchDeath1 = summaryList.Sum(n => n.SaleInsideOutPigBatchDeath1),
                        SaleOutsideOutAmount1 = summaryList.Sum(n => n.SaleOutsideOutAmount1),
                        SaleOutsideOutTotalDepreciation1 = summaryList.Sum(n => n.SaleOutsideOutTotalDepreciation1),
                        SaleOutsideOutFeed1 = summaryList.Sum(n => n.SaleOutsideOutFeed1),
                        SaleOutsideOutVeterinaryDrug1 = summaryList.Sum(n => n.SaleOutsideOutVeterinaryDrug1),
                        SaleOutsideOutVaccin1 = summaryList.Sum(n => n.SaleOutsideOutVaccin1),
                        SaleOutsideOutOtherMaterial1 = summaryList.Sum(n => n.SaleOutsideOutOtherMaterial1),
                        SaleOutsideOutPigTransferValue1 = summaryList.Sum(n => n.SaleOutsideOutPigTransferValue1),
                        SaleOutsideOutSemen1 = summaryList.Sum(n => n.SaleOutsideOutSemen1),
                        SaleOutsideOutCost1 = summaryList.Sum(n => n.SaleOutsideOutCost1),
                        SaleOutsideOutPigDeathValue1 = summaryList.Sum(n => n.SaleOutsideOutPigDeathValue1),
                        SaleOutsideOutPigBatchDeath1 = summaryList.Sum(n => n.SaleOutsideOutPigBatchDeath1),
                        DeathOutAmount1 = summaryList.Sum(n => n.DeathOutAmount1),
                        DeathOutTotalDepreciation1 = summaryList.Sum(n => n.DeathOutTotalDepreciation1),
                        DeathOutFeed1 = summaryList.Sum(n => n.DeathOutFeed1),
                        DeathOutVeterinaryDrug1 = summaryList.Sum(n => n.DeathOutVeterinaryDrug1),
                        DeathOutVaccin1 = summaryList.Sum(n => n.DeathOutVaccin1),
                        DeathOutOtherMaterial1 = summaryList.Sum(n => n.DeathOutOtherMaterial1),
                        DeathOutPigTransferValue1 = summaryList.Sum(n => n.DeathOutPigTransferValue1),
                        DeathOutSemen1 = summaryList.Sum(n => n.DeathOutSemen1),
                        DeathOutCost1 = summaryList.Sum(n => n.DeathOutCost1),
                        DeathOutPigDeathValue1 = summaryList.Sum(n => n.DeathOutPigDeathValue1),
                        DeathOutPigBatchDeath1 = summaryList.Sum(n => n.DeathOutPigBatchDeath1),
                        ReadyBreedingInAmount1 = summaryList.Sum(n => n.ReadyBreedingInAmount1),
                        ReadyBreedingInTotalDepreciation1 = summaryList.Sum(n => n.ReadyBreedingInTotalDepreciation1),
                        MissBreedingInAmount1 = summaryList.Sum(n => n.MissBreedingInAmount1),
                        MissBreedingInTotalDepreciation1 = summaryList.Sum(n => n.MissBreedingInTotalDepreciation1),
                        WeaningInAmount1 = summaryList.Sum(n => n.WeaningInAmount1),
                        WeaningInTotalDepreciation1 = summaryList.Sum(n => n.WeaningInTotalDepreciation1),
                        ReadyBreedingOutAmount1 = summaryList.Sum(n => n.ReadyBreedingOutAmount1),
                        ReadyBreedingOutTotalDepreciation1 = summaryList.Sum(n => n.ReadyBreedingOutTotalDepreciation1),
                        MissBreedingOutAmount1 = summaryList.Sum(n => n.MissBreedingOutAmount1),
                        MissBreedingOutTotalDepreciation1 = summaryList.Sum(n => n.MissBreedingOutTotalDepreciation1),
                        LactatingOutAmount1 = summaryList.Sum(n => n.LactatingOutAmount1),
                        LactatingOutTotalDepreciation1 = summaryList.Sum(n => n.LactatingOutTotalDepreciation1),
                        LactatingOutPigDeathValue1 = summaryList.Sum(n => n.LactatingOutPigDeathValue1),
                        InsidePurchaseInPigletCost2 = summaryList.Sum(n => n.InsidePurchaseInPigletCost2),
                        OutPurchaseInPigletCost2 = summaryList.Sum(n => n.OutPurchaseInPigletCost2),
                        ChildbirthInPigletCost2 = summaryList.Sum(n => n.ChildbirthInPigletCost2),
                        LactatingInTransferValue2 = summaryList.Sum(n => n.LactatingInTransferValue2),
                        LactatingInFeed2 = summaryList.Sum(n => n.LactatingInFeed2),
                        LactatingInVeterinaryDrug2 = summaryList.Sum(n => n.LactatingInVeterinaryDrug2),
                        LactatingInVaccin2 = summaryList.Sum(n => n.LactatingInVaccin2),
                        LactatingInOtherMaterial2 = summaryList.Sum(n => n.LactatingInOtherMaterial2),
                        LactatingInCost2 = summaryList.Sum(n => n.LactatingInCost2),
                        LactatingInPigDeathValue2 = summaryList.Sum(n => n.LactatingInPigDeathValue2),
                        BatchInPigletCost2 = summaryList.Sum(n => n.BatchInPigletCost2),
                        BatchInFeed2 = summaryList.Sum(n => n.BatchInFeed2),
                        BatchInVeterinaryDrug2 = summaryList.Sum(n => n.BatchInVeterinaryDrug2),
                        BatchInVaccin2 = summaryList.Sum(n => n.BatchInVaccin2),
                        BatchInOtherMaterial2 = summaryList.Sum(n => n.BatchInOtherMaterial2),
                        BatchInCost2 = summaryList.Sum(n => n.BatchInCost2),
                        BatchInPigTransferValue2 = summaryList.Sum(n => n.BatchInPigTransferValue2),
                        BatchInPigDeathValue2 = summaryList.Sum(n => n.BatchInPigDeathValue2),
                        BatchInBatchInsideDeath2 = summaryList.Sum(n => n.BatchInBatchInsideDeath2),
                        BatchInBatchOutsideDeath2 = summaryList.Sum(n => n.BatchInBatchOutsideDeath2),
                        AllocateInPigletCost2 = summaryList.Sum(n => n.AllocateInPigletCost2),
                        AllocateInFeed2 = summaryList.Sum(n => n.AllocateInFeed2),
                        AllocateInVeterinaryDrug2 = summaryList.Sum(n => n.AllocateInVeterinaryDrug2),
                        AllocateInVaccin2 = summaryList.Sum(n => n.AllocateInVaccin2),
                        AllocateInOtherMaterial2 = summaryList.Sum(n => n.AllocateInOtherMaterial2),
                        AllocateInCost2 = summaryList.Sum(n => n.AllocateInCost2),
                        AllocateInPigTransferValue2 = summaryList.Sum(n => n.AllocateInPigTransferValue2),
                        AllocateInPigDeathValue2 = summaryList.Sum(n => n.AllocateInPigDeathValue2),
                        AllocateInBatchInsideDeath2 = summaryList.Sum(n => n.AllocateInBatchInsideDeath2),
                        AllocateInBatchOutsideDeath2 = summaryList.Sum(n => n.AllocateInBatchOutsideDeath2),
                        DeathInFeed2 = summaryList.Sum(n => n.DeathInFeed2),
                        DeathInVeterinaryDrug2 = summaryList.Sum(n => n.DeathInVeterinaryDrug2),
                        DeathInVaccin2 = summaryList.Sum(n => n.DeathInVaccin2),
                        DeathInOtherMaterial2 = summaryList.Sum(n => n.DeathInOtherMaterial2),
                        DeathInCost2 = summaryList.Sum(n => n.DeathInCost2),
                        DeathInPigTransferValue2 = summaryList.Sum(n => n.DeathInPigTransferValue2),
                        DeathInPigDeathValue2 = summaryList.Sum(n => n.DeathInPigDeathValue2),
                        DeathInBatchInsideDeath2 = summaryList.Sum(n => n.DeathInBatchInsideDeath2),
                        DeathInBatchOutsideDeath2 = summaryList.Sum(n => n.DeathInBatchOutsideDeath2),
                        SaleInsideInTransferValue2 = summaryList.Sum(n => n.SaleInsideInTransferValue2),
                        SaleInsideInFeed2 = summaryList.Sum(n => n.SaleInsideInFeed2),
                        SaleInsideInVeterinaryDrug2 = summaryList.Sum(n => n.SaleInsideInVeterinaryDrug2),
                        SaleInsideInVaccin2 = summaryList.Sum(n => n.SaleInsideInVaccin2),
                        SaleInsideInOtherMaterial2 = summaryList.Sum(n => n.SaleInsideInOtherMaterial2),
                        SaleInsideInCost2 = summaryList.Sum(n => n.SaleInsideInCost2),
                        SaleInsideInBatchInsideDeath2 = summaryList.Sum(n => n.SaleInsideInBatchInsideDeath2),
                        SaleOutsideInTransferValue2 = summaryList.Sum(n => n.SaleOutsideInTransferValue2),
                        SaleOutsideInFeed2 = summaryList.Sum(n => n.SaleOutsideInFeed2),
                        SaleOutsideInVeterinaryDrug2 = summaryList.Sum(n => n.SaleOutsideInVeterinaryDrug2),
                        SaleOutsideInVaccin2 = summaryList.Sum(n => n.SaleOutsideInVaccin2),
                        SaleOutsideInOtherMaterial2 = summaryList.Sum(n => n.SaleOutsideInOtherMaterial2),
                        SaleOutsideInCost2 = summaryList.Sum(n => n.SaleOutsideInCost2),
                        SaleOutsideInBatchInsideDeath2 = summaryList.Sum(n => n.SaleOutsideInBatchInsideDeath2),
                        CurrentCostFeed2 = summaryList.Sum(n => n.CurrentCostFeed2),
                        CurrentCostVeterinaryDrug2 = summaryList.Sum(n => n.CurrentCostVeterinaryDrug2),
                        CurrentCostVaccin2 = summaryList.Sum(n => n.CurrentCostVaccin2),
                        CurrentCostOtherMaterial2 = summaryList.Sum(n => n.CurrentCostOtherMaterial2),
                        CurrentCostCost2 = summaryList.Sum(n => n.CurrentCostCost2),
                        AcrossStepOutPigletCost2 = summaryList.Sum(n => n.AcrossStepOutPigletCost2),
                        AcrossStepOutFeed2 = summaryList.Sum(n => n.AcrossStepOutFeed2),
                        AcrossStepOutVeterinaryDrug2 = summaryList.Sum(n => n.AcrossStepOutVeterinaryDrug2),
                        AcrossStepOutVaccin2 = summaryList.Sum(n => n.AcrossStepOutVaccin2),
                        AcrossStepOutOtherMaterial2 = summaryList.Sum(n => n.AcrossStepOutOtherMaterial2),
                        AcrossStepOutCost2 = summaryList.Sum(n => n.AcrossStepOutCost2),
                        AcrossStepOutPigTransferValue2 = summaryList.Sum(n => n.AcrossStepOutPigTransferValue2),
                        AcrossStepOutPigDeathValue2 = summaryList.Sum(n => n.AcrossStepOutPigDeathValue2),
                        AcrossStepOutBatchInsideDeath2 = summaryList.Sum(n => n.AcrossStepOutBatchInsideDeath2),
                        AcrossStepOutBatchOutsideDeath2 = summaryList.Sum(n => n.AcrossStepOutBatchOutsideDeath2),
                        BatchOutPigletCost2 = summaryList.Sum(n => n.BatchOutPigletCost2),
                        BatchOutFeed2 = summaryList.Sum(n => n.BatchOutFeed2),
                        BatchOutVeterinaryDrug2 = summaryList.Sum(n => n.BatchOutVeterinaryDrug2),
                        BatchOutVaccin2 = summaryList.Sum(n => n.BatchOutVaccin2),
                        BatchOutOtherMaterial2 = summaryList.Sum(n => n.BatchOutOtherMaterial2),
                        BatchOutCost2 = summaryList.Sum(n => n.BatchOutCost2),
                        BatchOutPigTransferValue2 = summaryList.Sum(n => n.BatchOutPigTransferValue2),
                        BatchOutPigDeathValue2 = summaryList.Sum(n => n.BatchOutPigDeathValue2),
                        BatchOutBatchInsideDeath2 = summaryList.Sum(n => n.BatchOutBatchInsideDeath2),
                        BatchOutBatchOutsideDeath2 = summaryList.Sum(n => n.BatchOutBatchOutsideDeath2),
                        AllocateOutPigletCost2 = summaryList.Sum(n => n.AllocateOutPigletCost2),
                        AllocateOutFeed2 = summaryList.Sum(n => n.AllocateOutFeed2),
                        AllocateOutVeterinaryDrug2 = summaryList.Sum(n => n.AllocateOutVeterinaryDrug2),
                        AllocateOutVaccin2 = summaryList.Sum(n => n.AllocateOutVaccin2),
                        AllocateOutOtherMaterial2 = summaryList.Sum(n => n.AllocateOutOtherMaterial2),
                        AllocateOutCost2 = summaryList.Sum(n => n.AllocateOutCost2),
                        AllocateOutPigTransferValue2 = summaryList.Sum(n => n.AllocateOutPigTransferValue2),
                        AllocateOutPigDeathValue2 = summaryList.Sum(n => n.AllocateOutPigDeathValue2),
                        AllocateOutBatchInsideDeath2 = summaryList.Sum(n => n.AllocateOutBatchInsideDeath2),
                        AllocateOutBatchOutsideDeath2 = summaryList.Sum(n => n.AllocateOutBatchOutsideDeath2),
                        DeathOutPigletCost2 = summaryList.Sum(n => n.DeathOutPigletCost2),
                        DeathOutFeed2 = summaryList.Sum(n => n.DeathOutFeed2),
                        DeathOutVeterinaryDrug2 = summaryList.Sum(n => n.DeathOutVeterinaryDrug2),
                        DeathOutVaccin2 = summaryList.Sum(n => n.DeathOutVaccin2),
                        DeathOutOtherMaterial2 = summaryList.Sum(n => n.DeathOutOtherMaterial2),
                        DeathOutCost2 = summaryList.Sum(n => n.DeathOutCost2),
                        DeathOutPigTransferValue2 = summaryList.Sum(n => n.DeathOutPigTransferValue2),
                        DeathOutPigDeathValue2 = summaryList.Sum(n => n.DeathOutPigDeathValue2),
                        DeathOutBatchInsideDeath2 = summaryList.Sum(n => n.DeathOutBatchInsideDeath2),
                        DeathOutBatchOutsideDeath2 = summaryList.Sum(n => n.DeathOutBatchOutsideDeath2),
                        SaleInsideOutPigletCost2 = summaryList.Sum(n => n.SaleInsideOutPigletCost2),
                        SaleInsideOutFeed2 = summaryList.Sum(n => n.SaleInsideOutFeed2),
                        SaleInsideOutVeterinaryDrug2 = summaryList.Sum(n => n.SaleInsideOutVeterinaryDrug2),
                        SaleInsideOutVaccin2 = summaryList.Sum(n => n.SaleInsideOutVaccin2),
                        SaleInsideOutOtherMaterial2 = summaryList.Sum(n => n.SaleInsideOutOtherMaterial2),
                        SaleInsideOutCost2 = summaryList.Sum(n => n.SaleInsideOutCost2),
                        SaleInsideOutPigTransferValue2 = summaryList.Sum(n => n.SaleInsideOutPigTransferValue2),
                        SaleInsideOutPigDeathValue2 = summaryList.Sum(n => n.SaleInsideOutPigDeathValue2),
                        SaleInsideOutBatchInsideDeath2 = summaryList.Sum(n => n.SaleInsideOutBatchInsideDeath2),
                        SaleInsideOutBatchOutsideDeath2 = summaryList.Sum(n => n.SaleInsideOutBatchOutsideDeath2),
                        SaleOutsideOutPigletCost2 = summaryList.Sum(n => n.SaleOutsideOutPigletCost2),
                        SaleOutsideOutFeed2 = summaryList.Sum(n => n.SaleOutsideOutFeed2),
                        SaleOutsideOutVeterinaryDrug2 = summaryList.Sum(n => n.SaleOutsideOutVeterinaryDrug2),
                        SaleOutsideOutVaccin2 = summaryList.Sum(n => n.SaleOutsideOutVaccin2),
                        SaleOutsideOutOtherMaterial2 = summaryList.Sum(n => n.SaleOutsideOutOtherMaterial2),
                        SaleOutsideOutCost2 = summaryList.Sum(n => n.SaleOutsideOutCost2),
                        SaleOutsideOutPigTransferValue2 = summaryList.Sum(n => n.SaleOutsideOutPigTransferValue2),
                        SaleOutsideOutPigDeathValue2 = summaryList.Sum(n => n.SaleOutsideOutPigDeathValue2),
                        SaleOutsideOutBatchInsideDeath2 = summaryList.Sum(n => n.SaleOutsideOutBatchInsideDeath2),
                        SaleOutsideOutBatchOutsideDeath2 = summaryList.Sum(n => n.SaleOutsideOutBatchOutsideDeath2),
                        EmergenceOutPigletCost2 = summaryList.Sum(n => n.EmergenceOutPigletCost2),
                        EmergenceOutFeed2 = summaryList.Sum(n => n.EmergenceOutFeed2),
                        EmergenceOutVeterinaryDrug2 = summaryList.Sum(n => n.EmergenceOutVeterinaryDrug2),
                        EmergenceOutVaccin2 = summaryList.Sum(n => n.EmergenceOutVaccin2),
                        EmergenceOutOtherMaterial2 = summaryList.Sum(n => n.EmergenceOutOtherMaterial2),
                        EmergenceOutCost2 = summaryList.Sum(n => n.EmergenceOutCost2),
                        EmergenceOutPigTransferValue2 = summaryList.Sum(n => n.EmergenceOutPigTransferValue2),
                        EmergenceOutPigDeathValue2 = summaryList.Sum(n => n.EmergenceOutPigDeathValue2),
                        EmergenceOutBatchInsideDeath2 = summaryList.Sum(n => n.EmergenceOutBatchInsideDeath2),
                        EmergenceOutBatchOutsideDeath2 = summaryList.Sum(n => n.EmergenceOutBatchOutsideDeath2),
                        AcrossStepInPigletCost3 = summaryList.Sum(n => n.AcrossStepInPigletCost3),
                        InsidePurchaseInPigletCost3 = summaryList.Sum(n => n.InsidePurchaseInPigletCost3),
                        OutPurchaseInPigletCost3 = summaryList.Sum(n => n.OutPurchaseInPigletCost3),
                        RecycleBreederInPigletCost3 = summaryList.Sum(n => n.RecycleBreederInPigletCost3),
                        BatchInPigletCost3 = summaryList.Sum(n => n.BatchInPigletCost3),
                        BatchInFeed3 = summaryList.Sum(n => n.BatchInFeed3),
                        BatchInVeterinaryDrug3 = summaryList.Sum(n => n.BatchInVeterinaryDrug3),
                        BatchInVaccin3 = summaryList.Sum(n => n.BatchInVaccin3),
                        BatchInOtherMaterial3 = summaryList.Sum(n => n.BatchInOtherMaterial3),
                        BatchInCost3 = summaryList.Sum(n => n.BatchInCost3),
                        BatchInBatchInsideDeath3 = summaryList.Sum(n => n.BatchInBatchInsideDeath3),
                        BatchInBatchOutsideDeath3 = summaryList.Sum(n => n.BatchInBatchOutsideDeath3),
                        AllocateInPigletCost3 = summaryList.Sum(n => n.AllocateInPigletCost3),
                        AllocateInFeed3 = summaryList.Sum(n => n.AllocateInFeed3),
                        AllocateInVeterinaryDrug3 = summaryList.Sum(n => n.AllocateInVeterinaryDrug3),
                        AllocateInVaccin3 = summaryList.Sum(n => n.AllocateInVaccin3),
                        AllocateInOtherMaterial3 = summaryList.Sum(n => n.AllocateInOtherMaterial3),
                        AllocateInCost3 = summaryList.Sum(n => n.AllocateInCost3),
                        AllocateInBatchInsideDeath3 = summaryList.Sum(n => n.AllocateInBatchInsideDeath3),
                        AllocateInBatchOutsideDeath3 = summaryList.Sum(n => n.AllocateInBatchOutsideDeath3),
                        DeathInFeed3 = summaryList.Sum(n => n.DeathInFeed3),
                        DeathInVeterinaryDrug3 = summaryList.Sum(n => n.DeathInVeterinaryDrug3),
                        DeathInVaccin3 = summaryList.Sum(n => n.DeathInVaccin3),
                        DeathInOtherMaterial3 = summaryList.Sum(n => n.DeathInOtherMaterial3),
                        DeathInCost3 = summaryList.Sum(n => n.DeathInCost3),
                        DeathInBatchInsideDeath3 = summaryList.Sum(n => n.DeathInBatchInsideDeath3),
                        DeathInBatchOutsideDeath3 = summaryList.Sum(n => n.DeathInBatchOutsideDeath3),
                        CurrentCostFeed3 = summaryList.Sum(n => n.CurrentCostFeed3),
                        CurrentCostVeterinaryDrug3 = summaryList.Sum(n => n.CurrentCostVeterinaryDrug3),
                        CurrentCostVaccin3 = summaryList.Sum(n => n.CurrentCostVaccin3),
                        CurrentCostOtherMaterial3 = summaryList.Sum(n => n.CurrentCostOtherMaterial3),
                        CurrentCostCost3 = summaryList.Sum(n => n.CurrentCostCost3),
                        AcrossStepOutPigletCost3 = summaryList.Sum(n => n.AcrossStepOutPigletCost3),
                        AcrossStepOutFeed3 = summaryList.Sum(n => n.AcrossStepOutFeed3),
                        AcrossStepOutVeterinaryDrug3 = summaryList.Sum(n => n.AcrossStepOutVeterinaryDrug3),
                        AcrossStepOutVaccin3 = summaryList.Sum(n => n.AcrossStepOutVaccin3),
                        AcrossStepOutOtherMaterial3 = summaryList.Sum(n => n.AcrossStepOutOtherMaterial3),
                        AcrossStepOutCost3 = summaryList.Sum(n => n.AcrossStepOutCost3),
                        AcrossStepOutBatchInsideDeath3 = summaryList.Sum(n => n.AcrossStepOutBatchInsideDeath3),
                        AcrossStepOutBatchOutsideDeath3 = summaryList.Sum(n => n.AcrossStepOutBatchOutsideDeath3),
                        BatchOutPigletCost3 = summaryList.Sum(n => n.BatchOutPigletCost3),
                        BatchOutFeed3 = summaryList.Sum(n => n.BatchOutFeed3),
                        BatchOutVeterinaryDrug3 = summaryList.Sum(n => n.BatchOutVeterinaryDrug3),
                        BatchOutVaccin3 = summaryList.Sum(n => n.BatchOutVaccin3),
                        BatchOutOtherMaterial3 = summaryList.Sum(n => n.BatchOutOtherMaterial3),
                        BatchOutCost3 = summaryList.Sum(n => n.BatchOutCost3),
                        BatchOutBatchInsideDeath3 = summaryList.Sum(n => n.BatchOutBatchInsideDeath3),
                        BatchOutBatchOutsideDeath3 = summaryList.Sum(n => n.BatchOutBatchOutsideDeath3),
                        AllocateOutPigletCost3 = summaryList.Sum(n => n.AllocateOutPigletCost3),
                        AllocateOutFeed3 = summaryList.Sum(n => n.AllocateOutFeed3),
                        AllocateOutVeterinaryDrug3 = summaryList.Sum(n => n.AllocateOutVeterinaryDrug3),
                        AllocateOutVaccin3 = summaryList.Sum(n => n.AllocateOutVaccin3),
                        AllocateOutOtherMaterial3 = summaryList.Sum(n => n.AllocateOutOtherMaterial3),
                        AllocateOutCost3 = summaryList.Sum(n => n.AllocateOutCost3),
                        AllocateOutBatchInsideDeath3 = summaryList.Sum(n => n.AllocateOutBatchInsideDeath3),
                        AllocateOutBatchOutsideDeath3 = summaryList.Sum(n => n.AllocateOutBatchOutsideDeath3),
                        DeathOutPigletCost3 = summaryList.Sum(n => n.DeathOutPigletCost3),
                        DeathOutFeed3 = summaryList.Sum(n => n.DeathOutFeed3),
                        DeathOutVeterinaryDrug3 = summaryList.Sum(n => n.DeathOutVeterinaryDrug3),
                        DeathOutVaccin3 = summaryList.Sum(n => n.DeathOutVaccin3),
                        DeathOutOtherMaterial3 = summaryList.Sum(n => n.DeathOutOtherMaterial3),
                        DeathOutCost3 = summaryList.Sum(n => n.DeathOutCost3),
                        DeathOutBatchInsideDeath3 = summaryList.Sum(n => n.DeathOutBatchInsideDeath3),
                        DeathOutBatchOutsideDeath3 = summaryList.Sum(n => n.DeathOutBatchOutsideDeath3),
                        SaleInsideOutPigletCost3 = summaryList.Sum(n => n.SaleInsideOutPigletCost3),
                        SaleInsideOutFeed3 = summaryList.Sum(n => n.SaleInsideOutFeed3),
                        SaleInsideOutVeterinaryDrug3 = summaryList.Sum(n => n.SaleInsideOutVeterinaryDrug3),
                        SaleInsideOutVaccin3 = summaryList.Sum(n => n.SaleInsideOutVaccin3),
                        SaleInsideOutOtherMaterial3 = summaryList.Sum(n => n.SaleInsideOutOtherMaterial3),
                        SaleInsideOutCost3 = summaryList.Sum(n => n.SaleInsideOutCost3),
                        SaleInsideOutBatchInsideDeath3 = summaryList.Sum(n => n.SaleInsideOutBatchInsideDeath3),
                        SaleInsideOutBatchOutsideDeath3 = summaryList.Sum(n => n.SaleInsideOutBatchOutsideDeath3),
                        SaleOutsideOutPigletCost3 = summaryList.Sum(n => n.SaleOutsideOutPigletCost3),
                        SaleOutsideOutFeed3 = summaryList.Sum(n => n.SaleOutsideOutFeed3),
                        SaleOutsideOutVeterinaryDrug3 = summaryList.Sum(n => n.SaleOutsideOutVeterinaryDrug3),
                        SaleOutsideOutVaccin3 = summaryList.Sum(n => n.SaleOutsideOutVaccin3),
                        SaleOutsideOutOtherMaterial3 = summaryList.Sum(n => n.SaleOutsideOutOtherMaterial3),
                        SaleOutsideOutCost3 = summaryList.Sum(n => n.SaleOutsideOutCost3),
                        SaleOutsideOutBatchInsideDeath3 = summaryList.Sum(n => n.SaleOutsideOutBatchInsideDeath3),
                        SaleOutsideOutBatchOutsideDeath3 = summaryList.Sum(n => n.SaleOutsideOutBatchOutsideDeath3),
                        EmergenceOutPigletCost3 = summaryList.Sum(n => n.EmergenceOutPigletCost3),
                        EmergenceOutFeed3 = summaryList.Sum(n => n.EmergenceOutFeed3),
                        EmergenceOutVeterinaryDrug3 = summaryList.Sum(n => n.EmergenceOutVeterinaryDrug3),
                        EmergenceOutVaccin3 = summaryList.Sum(n => n.EmergenceOutVaccin3),
                        EmergenceOutOtherMaterial3 = summaryList.Sum(n => n.EmergenceOutOtherMaterial3),
                        EmergenceOutCost3 = summaryList.Sum(n => n.EmergenceOutCost3),
                        EmergenceOutBatchInsideDeath3 = summaryList.Sum(n => n.EmergenceOutBatchInsideDeath3),
                        EmergenceOutBatchOutsideDeath3 = summaryList.Sum(n => n.EmergenceOutBatchOutsideDeath3),
                        AcrossStepInPigletCost4 = summaryList.Sum(n => n.AcrossStepInPigletCost4),
                        InsidePurchaseInPigletCost4 = summaryList.Sum(n => n.InsidePurchaseInPigletCost4),
                        OutPurchaseInPigletCost4 = summaryList.Sum(n => n.OutPurchaseInPigletCost4),
                        RecycleBreederInPigletCost4 = summaryList.Sum(n => n.RecycleBreederInPigletCost4),
                        BatchInPigletCost4 = summaryList.Sum(n => n.BatchInPigletCost4),
                        BatchInFeed4 = summaryList.Sum(n => n.BatchInFeed4),
                        BatchInVeterinaryDrug4 = summaryList.Sum(n => n.BatchInVeterinaryDrug4),
                        BatchInVaccin4 = summaryList.Sum(n => n.BatchInVaccin4),
                        BatchInOtherMaterial4 = summaryList.Sum(n => n.BatchInOtherMaterial4),
                        BatchInCost4 = summaryList.Sum(n => n.BatchInCost4),
                        BatchInBatchInsideDeath4 = summaryList.Sum(n => n.BatchInBatchInsideDeath4),
                        BatchInBatchOutsideDeath4 = summaryList.Sum(n => n.BatchInBatchOutsideDeath4),
                        AllocateInPigletCost4 = summaryList.Sum(n => n.AllocateInPigletCost4),
                        AllocateInFeed4 = summaryList.Sum(n => n.AllocateInFeed4),
                        AllocateInVeterinaryDrug4 = summaryList.Sum(n => n.AllocateInVeterinaryDrug4),
                        AllocateInVaccin4 = summaryList.Sum(n => n.AllocateInVaccin4),
                        AllocateInOtherMaterial4 = summaryList.Sum(n => n.AllocateInOtherMaterial4),
                        AllocateInCost4 = summaryList.Sum(n => n.AllocateInCost4),
                        AllocateInBatchInsideDeath4 = summaryList.Sum(n => n.AllocateInBatchInsideDeath4),
                        AllocateInBatchOutsideDeath4 = summaryList.Sum(n => n.AllocateInBatchOutsideDeath4),
                        DeathInFeed4 = summaryList.Sum(n => n.DeathInFeed4),
                        DeathInVeterinaryDrug4 = summaryList.Sum(n => n.DeathInVeterinaryDrug4),
                        DeathInVaccin4 = summaryList.Sum(n => n.DeathInVaccin4),
                        DeathInOtherMaterial4 = summaryList.Sum(n => n.DeathInOtherMaterial4),
                        DeathInCost4 = summaryList.Sum(n => n.DeathInCost4),
                        DeathInBatchInsideDeath4 = summaryList.Sum(n => n.DeathInBatchInsideDeath4),
                        DeathInBatchOutsideDeath4 = summaryList.Sum(n => n.DeathInBatchOutsideDeath4),
                        CurrentCostFeed4 = summaryList.Sum(n => n.CurrentCostFeed4),
                        CurrentCostVeterinaryDrug4 = summaryList.Sum(n => n.CurrentCostVeterinaryDrug4),
                        CurrentCostVaccin4 = summaryList.Sum(n => n.CurrentCostVaccin4),
                        CurrentCostOtherMaterial4 = summaryList.Sum(n => n.CurrentCostOtherMaterial4),
                        CurrentCostCost4 = summaryList.Sum(n => n.CurrentCostCost4),
                        AcrossStepOutPigletCost4 = summaryList.Sum(n => n.AcrossStepOutPigletCost4),
                        AcrossStepOutFeed4 = summaryList.Sum(n => n.AcrossStepOutFeed4),
                        AcrossStepOutVeterinaryDrug4 = summaryList.Sum(n => n.AcrossStepOutVeterinaryDrug4),
                        AcrossStepOutVaccin4 = summaryList.Sum(n => n.AcrossStepOutVaccin4),
                        AcrossStepOutOtherMaterial4 = summaryList.Sum(n => n.AcrossStepOutOtherMaterial4),
                        AcrossStepOutCost4 = summaryList.Sum(n => n.AcrossStepOutCost4),
                        AcrossStepOutBatchInsideDeath4 = summaryList.Sum(n => n.AcrossStepOutBatchInsideDeath4),
                        AcrossStepOutBatchOutsideDeath4 = summaryList.Sum(n => n.AcrossStepOutBatchOutsideDeath4),
                        BatchOutPigletCost4 = summaryList.Sum(n => n.BatchOutPigletCost4),
                        BatchOutFeed4 = summaryList.Sum(n => n.BatchOutFeed4),
                        BatchOutVeterinaryDrug4 = summaryList.Sum(n => n.BatchOutVeterinaryDrug4),
                        BatchOutVaccin4 = summaryList.Sum(n => n.BatchOutVaccin4),
                        BatchOutOtherMaterial4 = summaryList.Sum(n => n.BatchOutOtherMaterial4),
                        BatchOutCost4 = summaryList.Sum(n => n.BatchOutCost4),
                        BatchOutBatchInsideDeath4 = summaryList.Sum(n => n.BatchOutBatchInsideDeath4),
                        BatchOutBatchOutsideDeath4 = summaryList.Sum(n => n.BatchOutBatchOutsideDeath4),
                        AllocateOutPigletCost4 = summaryList.Sum(n => n.AllocateOutPigletCost4),
                        AllocateOutFeed4 = summaryList.Sum(n => n.AllocateOutFeed4),
                        AllocateOutVeterinaryDrug4 = summaryList.Sum(n => n.AllocateOutVeterinaryDrug4),
                        AllocateOutVaccin4 = summaryList.Sum(n => n.AllocateOutVaccin4),
                        AllocateOutOtherMaterial4 = summaryList.Sum(n => n.AllocateOutOtherMaterial4),
                        AllocateOutCost4 = summaryList.Sum(n => n.AllocateOutCost4),
                        AllocateOutBatchInsideDeath4 = summaryList.Sum(n => n.AllocateOutBatchInsideDeath4),
                        AllocateOutBatchOutsideDeath4 = summaryList.Sum(n => n.AllocateOutBatchOutsideDeath4),
                        DeathOutPigletCost4 = summaryList.Sum(n => n.DeathOutPigletCost4),
                        DeathOutFeed4 = summaryList.Sum(n => n.DeathOutFeed4),
                        DeathOutVeterinaryDrug4 = summaryList.Sum(n => n.DeathOutVeterinaryDrug4),
                        DeathOutVaccin4 = summaryList.Sum(n => n.DeathOutVaccin4),
                        DeathOutOtherMaterial4 = summaryList.Sum(n => n.DeathOutOtherMaterial4),
                        DeathOutCost4 = summaryList.Sum(n => n.DeathOutCost4),
                        DeathOutBatchInsideDeath4 = summaryList.Sum(n => n.DeathOutBatchInsideDeath4),
                        DeathOutBatchOutsideDeath4 = summaryList.Sum(n => n.DeathOutBatchOutsideDeath4),
                        SaleInsideOutPigletCost4 = summaryList.Sum(n => n.SaleInsideOutPigletCost4),
                        SaleInsideOutFeed4 = summaryList.Sum(n => n.SaleInsideOutFeed4),
                        SaleInsideOutVeterinaryDrug4 = summaryList.Sum(n => n.SaleInsideOutVeterinaryDrug4),
                        SaleInsideOutVaccin4 = summaryList.Sum(n => n.SaleInsideOutVaccin4),
                        SaleInsideOutOtherMaterial4 = summaryList.Sum(n => n.SaleInsideOutOtherMaterial4),
                        SaleInsideOutCost4 = summaryList.Sum(n => n.SaleInsideOutCost4),
                        SaleInsideOutBatchInsideDeath4 = summaryList.Sum(n => n.SaleInsideOutBatchInsideDeath4),
                        SaleInsideOutBatchOutsideDeath4 = summaryList.Sum(n => n.SaleInsideOutBatchOutsideDeath4),
                        SaleOutsideOutPigletCost4 = summaryList.Sum(n => n.SaleOutsideOutPigletCost4),
                        SaleOutsideOutFeed4 = summaryList.Sum(n => n.SaleOutsideOutFeed4),
                        SaleOutsideOutVeterinaryDrug4 = summaryList.Sum(n => n.SaleOutsideOutVeterinaryDrug4),
                        SaleOutsideOutVaccin4 = summaryList.Sum(n => n.SaleOutsideOutVaccin4),
                        SaleOutsideOutOtherMaterial4 = summaryList.Sum(n => n.SaleOutsideOutOtherMaterial4),
                        SaleOutsideOutCost4 = summaryList.Sum(n => n.SaleOutsideOutCost4),
                        SaleOutsideOutBatchInsideDeath4 = summaryList.Sum(n => n.SaleOutsideOutBatchInsideDeath4),
                        SaleOutsideOutBatchOutsideDeath4 = summaryList.Sum(n => n.SaleOutsideOutBatchOutsideDeath4),
                        FeedDays5 = summaryList.Sum(n => n.FeedDays5),
                        Depreciated5 = summaryList.Sum(n => n.Depreciated5),
                        MaterialCost5 = summaryList.Sum(n => n.MaterialCost5),
                        MedicantCost5 = summaryList.Sum(n => n.MedicantCost5),
                        VaccineCost5 = summaryList.Sum(n => n.VaccineCost5),
                        OtherCost5 = summaryList.Sum(n => n.OtherCost5),
                        FeeCost5 = summaryList.Sum(n => n.FeeCost5),
                        SumCost5 = summaryList.Sum(n => n.SumCost5),
                        CurrentPigCost6=summaryList.Sum(n=>n.CurrentPigCost6),
                        CurrentMaterial6=summaryList.Sum(n=>n.CurrentMaterial6),
                        CurrentMedicant6=summaryList.Sum(n=>n.CurrentMedicant6),
                        CurrentVaccine6=summaryList.Sum(n=>n.CurrentVaccine6),
                        CurrentOther6=summaryList.Sum(n=>n.CurrentOther6),
                        CurrentInnerDeath6=summaryList.Sum(n=>n.CurrentInnerDeath6),
                        CurrentOuterDeath6=summaryList.Sum(n=>n.CurrentOuterDeath6),
                        CurrentDirectFee6=summaryList.Sum(n=>n.CurrentDirectFee6),
                        CurrentBuildingFee6=summaryList.Sum(n=>n.CurrentBuildingFee6),
                        CurrentPredictFeedFee6=summaryList.Sum(n=>n.CurrentPredictFeedFee6),
                        CurrentAdjustFeedFee6=summaryList.Sum(n=>n.CurrentAdjustFeedFee6),
                        CurrentAdditionFee6=summaryList.Sum(n=>n.CurrentAdditionFee6),
                        CurrentHoldUpFee6=summaryList.Sum(n=>n.CurrentHoldUpFee6),
                        RecyclePigCost6=summaryList.Sum(n=>n.RecyclePigCost6),
                        RecycleMaterial6=summaryList.Sum(n=>n.RecycleMaterial6),
                        RecycleMedicant6=summaryList.Sum(n=>n.RecycleMedicant6),
                        RecycleVaccine6=summaryList.Sum(n=>n.RecycleVaccine6),
                        RecycleOther6=summaryList.Sum(n=>n.RecycleOther6),
                        RecycleInnerDeath6=summaryList.Sum(n=>n.RecycleInnerDeath6),
                        RecycleOuterDeath6=summaryList.Sum(n=>n.RecycleOuterDeath6),
                        RecycleDirectFee6=summaryList.Sum(n=>n.RecycleDirectFee6),
                        RecycleBuildingFee6=summaryList.Sum(n=>n.RecycleBuildingFee6),
                        RecyclePredictFeedFee6=summaryList.Sum(n=>n.RecyclePredictFeedFee6),
                        RecycleAdjustFeedFee6=summaryList.Sum(n=>n.RecycleAdjustFeedFee6),
                        RecycleAdditionFee6=summaryList.Sum(n=>n.RecycleAdditionFee6),
                        RecycleHoldUpFee6=summaryList.Sum(n=>n.RecycleHoldUpFee6),
                        DeathPigCost6=summaryList.Sum(n=>n.DeathPigCost6),
                        DeathMaterial6=summaryList.Sum(n=>n.DeathMaterial6),
                        DeathMedicant6=summaryList.Sum(n=>n.DeathMedicant6),
                        DeathVaccine6=summaryList.Sum(n=>n.DeathVaccine6),
                        DeathOther6=summaryList.Sum(n=>n.DeathOther6),
                        DeathInnerDeath6=summaryList.Sum(n=>n.DeathInnerDeath6),
                        DeathOuterDeath6=summaryList.Sum(n=>n.DeathOuterDeath6),
                        DeathDirectFee6=summaryList.Sum(n=>n.DeathDirectFee6),
                        DeathBuildingFee6=summaryList.Sum(n=>n.DeathBuildingFee6),
                        DeathPredictFeedFee6=summaryList.Sum(n=>n.DeathPredictFeedFee6),
                        DeathAdjustFeedFee6=summaryList.Sum(n=>n.DeathAdjustFeedFee6),
                        DeathAdditionFee6=summaryList.Sum(n=>n.DeathAdditionFee6),
                        DeathHoldUpFee6=summaryList.Sum(n=>n.DeathHoldUpFee6),
                        AdjustCount6=summaryList.Sum(n=>n.AdjustCount6),
                        DeathPigCount6=summaryList.Sum(n=>n.DeathPigCount6),
                        BeginAdditionFee6=summaryList.Sum(n=>n.BeginAdditionFee6),
                        BeginAdjustFeedFee6=summaryList.Sum(n=>n.BeginAdjustFeedFee6),
                        BeginBuildingFee6=summaryList.Sum(n=>n.BeginBuildingFee6),
                        BeginDirectFee6=summaryList.Sum(n=>n.BeginDirectFee6),
                        BeginHoldUpFee6=summaryList.Sum(n=>n.BeginHoldUpFee6),
                        BeginInnerDeath6=summaryList.Sum(n=>n.BeginInnerDeath6),
                        BeginMaterial6=summaryList.Sum(n=>n.BeginMaterial6),
                        BeginMedicant6=summaryList.Sum(n=>n.BeginMedicant6),
                        BeginOther6=summaryList.Sum(n=>n.BeginOther6),
                        BeginOuterDeath6=summaryList.Sum(n=>n.BeginOuterDeath6),
                        BeginPigCost6=summaryList.Sum(n=>n.BeginPigCost6),
                        BeginPigCount6=summaryList.Sum(n=>n.BeginPigCount6),
                        BeginPredictFeedFee6=summaryList.Sum(n=>n.BeginPredictFeedFee6),
                        BeginVaccine6=summaryList.Sum(n=>n.BeginVaccine6),
                        CurrentPigCount6=summaryList.Sum(n=>n.CurrentPigCount6),
                        EndAdditionFee6=summaryList.Sum(n=>n.EndAdditionFee6),
                        EndAdjustFeedFee6=summaryList.Sum(n=>n.EndAdjustFeedFee6),
                        EndBuildingFee6=summaryList.Sum(n=>n.EndBuildingFee6),
                        EndDirectFee6=summaryList.Sum(n=>n.EndDirectFee6),
                        EndHoldUpFee6=summaryList.Sum(n=>n.EndHoldUpFee6),
                        EndInnerDeath6=summaryList.Sum(n=>n.EndInnerDeath6),
                        EndMaterial6=summaryList.Sum(n=>n.EndMaterial6),
                        EndMedicant6=summaryList.Sum(n=>n.EndMedicant6),
                        EndOther6=summaryList.Sum(n=>n.EndOther6),
                        EndOuterDeath6 =summaryList.Sum(n=>n.EndOuterDeath6),
                        EndPigCost6=summaryList.Sum(n=>n.EndPigCost6),
                        EndPigCount6=summaryList.Sum(n=>n.EndPigCount6),
                        EndPredictFeedFee6=summaryList.Sum(n=>n.EndPredictFeedFee6),
                        EndVaccine6=summaryList.Sum(n=>n.EndVaccine6),
                        RecyclePigCount6=summaryList.Sum(n=>n.RecyclePigCount6),
                     }
                };
                return list;
            }
            summaryList = summaryList.GroupBy(s => groupModel.GroupBy(s)).Select(s => new PigCirculationSummary()
            {
                DeptId = s.Key.DeptId,
                ProductId = s.Key.ProductId,
                FarmerID=s.Key.FarmerID,
                FallbackInAmount1 = s.Sum(n => n.FallbackInAmount1),
                InsidePurchaseInAmount1 = s.Sum(n => n.InsidePurchaseInAmount1),
                OutPurchaseInAmount1 = s.Sum(n => n.OutPurchaseInAmount1),
                ReadyBreedingInFeed1 = s.Sum(n => n.ReadyBreedingInFeed1),
                ReadyBreedingInVeterinaryDrug1 = s.Sum(n => n.ReadyBreedingInVeterinaryDrug1),
                ReadyBreedingInVaccin1 = s.Sum(n => n.ReadyBreedingInVaccin1),
                ReadyBreedingInOtherMaterial1 = s.Sum(n => n.ReadyBreedingInOtherMaterial1),
                ReadyBreedingInPigTransferValue1 = s.Sum(n => n.ReadyBreedingInPigTransferValue1),
                ReadyBreedingInCost1 = s.Sum(n => n.ReadyBreedingInCost1),
                ReadyBreedingInPigDeathValue1 = s.Sum(n => n.ReadyBreedingInPigDeathValue1),
                ReadyBreedingInPigBatchDeath1 = s.Sum(n => n.ReadyBreedingInPigBatchDeath1),
                MissBreedingInFeed1 = s.Sum(n => n.MissBreedingInFeed1),
                MissBreedingInVeterinaryDrug1 = s.Sum(n => n.MissBreedingInVeterinaryDrug1),
                MissBreedingInVaccin1 = s.Sum(n => n.MissBreedingInVaccin1),
                MissBreedingInOtherMaterial1 = s.Sum(n => n.MissBreedingInOtherMaterial1),
                MissBreedingInPigTransferValue1 = s.Sum(n => n.MissBreedingInPigTransferValue1),
                MissBreedingInSemen1 = s.Sum(n => n.MissBreedingInSemen1),
                MissBreedingInCost1 = s.Sum(n => n.MissBreedingInCost1),
                MissBreedingInPigDeathValue1 = s.Sum(n => n.MissBreedingInPigDeathValue1),
                MissBreedingInPigBatchDeath1 = s.Sum(n => n.MissBreedingInPigBatchDeath1),
                AllocateInAmount1 = s.Sum(n => n.AllocateInAmount1),
                AllocateInTotalDepreciation1 = s.Sum(n => n.AllocateInTotalDepreciation1),
                AllocateInFeed1 = s.Sum(n => n.AllocateInFeed1),
                AllocateInVeterinaryDrug1 = s.Sum(n => n.AllocateInVeterinaryDrug1),
                AllocateInVaccin1 = s.Sum(n => n.AllocateInVaccin1),
                AllocateInOtherMaterial1 = s.Sum(n => n.AllocateInOtherMaterial1),
                AllocateInPigTransferValue1 = s.Sum(n => n.AllocateInPigTransferValue1),
                AllocateInSemen1 = s.Sum(n => n.AllocateInSemen1),
                AllocateInCost1 = s.Sum(n => n.AllocateInCost1),
                AllocateInPigDeathValue1 = s.Sum(n => n.AllocateInPigDeathValue1),
                AllocateInPigBatchDeath1 = s.Sum(n => n.AllocateInPigBatchDeath1),
                BatchInAmount1 = s.Sum(n => n.BatchInAmount1),
                BatchInTotalDepreciation1 = s.Sum(n => n.BatchInTotalDepreciation1),
                BatchInFeed1 = s.Sum(n => n.BatchInFeed1),
                BatchInVeterinaryDrug1 = s.Sum(n => n.BatchInVeterinaryDrug1),
                BatchInVaccin1 = s.Sum(n => n.BatchInVaccin1),
                BatchInOtherMaterial1 = s.Sum(n => n.BatchInOtherMaterial1),
                BatchInPigTransferValue1 = s.Sum(n => n.BatchInPigTransferValue1),
                BatchInSemen1 = s.Sum(n => n.BatchInSemen1),
                BatchInCost1 = s.Sum(n => n.BatchInCost1),
                BatchInPigDeathValue1 = s.Sum(n => n.BatchInPigDeathValue1),
                BatchInPigBatchDeath1 = s.Sum(n => n.BatchInPigBatchDeath1),
                SaleInsideInFeed1 = s.Sum(n => n.SaleInsideInFeed1),
                SaleInsideInVeterinaryDrug1 = s.Sum(n => n.SaleInsideInVeterinaryDrug1),
                SaleInsideInVaccin1 = s.Sum(n => n.SaleInsideInVaccin1),
                SaleInsideInOtherMaterial1 = s.Sum(n => n.SaleInsideInOtherMaterial1),
                SaleInsideInPigTransferValue1 = s.Sum(n => n.SaleInsideInPigTransferValue1),
                SaleInsideInSemen1 = s.Sum(n => n.SaleInsideInSemen1),
                SaleInsideInCost1 = s.Sum(n => n.SaleInsideInCost1),
                SaleInsideInPigDeathValue1 = s.Sum(n => n.SaleInsideInPigDeathValue1),
                SaleInsideInPigBatchDeath1 = s.Sum(n => n.SaleInsideInPigBatchDeath1),
                SaleOutsideInFeed1 = s.Sum(n => n.SaleOutsideInFeed1),
                SaleOutsideInVeterinaryDrug1 = s.Sum(n => n.SaleOutsideInVeterinaryDrug1),
                SaleOutsideInVaccin1 = s.Sum(n => n.SaleOutsideInVaccin1),
                SaleOutsideInOtherMaterial1 = s.Sum(n => n.SaleOutsideInOtherMaterial1),
                SaleOutsideInPigTransferValue1 = s.Sum(n => n.SaleOutsideInPigTransferValue1),
                SaleOutsideInSemen1 = s.Sum(n => n.SaleOutsideInSemen1),
                SaleOutsideInCost1 = s.Sum(n => n.SaleOutsideInCost1),
                SaleOutsideInPigDeathValue1 = s.Sum(n => n.SaleOutsideInPigDeathValue1),
                SaleOutsideInPigBatchDeath1 = s.Sum(n => n.SaleOutsideInPigBatchDeath1),
                DeathInFeed1 = s.Sum(n => n.DeathInFeed1),
                DeathInVeterinaryDrug1 = s.Sum(n => n.DeathInVeterinaryDrug1),
                DeathInVaccin1 = s.Sum(n => n.DeathInVaccin1),
                DeathInOtherMaterial1 = s.Sum(n => n.DeathInOtherMaterial1),
                DeathInPigTransferValue1 = s.Sum(n => n.DeathInPigTransferValue1),
                DeathInSemen1 = s.Sum(n => n.DeathInSemen1),
                DeathInCost1 = s.Sum(n => n.DeathInCost1),
                DeathInPigDeathValue1 = s.Sum(n => n.DeathInPigDeathValue1),
                DeathInPigBatchDeath1 = s.Sum(n => n.DeathInPigBatchDeath1),
                CurrentFeed1 = s.Sum(n => n.CurrentFeed1),
                CurrentVeterinaryDrug1 = s.Sum(n => n.CurrentVeterinaryDrug1),
                CurrentVaccin1 = s.Sum(n => n.CurrentVaccin1),
                CurrentOtherMaterial1 = s.Sum(n => n.CurrentOtherMaterial1),
                CurrentPigTransferValue1 = s.Sum(n => n.CurrentPigTransferValue1),
                CurrentSemen1 = s.Sum(n => n.CurrentSemen1),
                CurrentCost1 = s.Sum(n => n.CurrentCost1),
                ChildbirthOutFeed1 = s.Sum(n => n.ChildbirthOutFeed1),
                ChildbirthOutVeterinaryDrug1 = s.Sum(n => n.ChildbirthOutVeterinaryDrug1),
                ChildbirthOutVaccin1 = s.Sum(n => n.ChildbirthOutVaccin1),
                ChildbirthOutOtherMaterial1 = s.Sum(n => n.ChildbirthOutOtherMaterial1),
                ChildbirthOutPigTransferValue1 = s.Sum(n => n.ChildbirthOutPigTransferValue1),
                ChildbirthOutSemen1 = s.Sum(n => n.ChildbirthOutSemen1),
                ChildbirthOutCost1 = s.Sum(n => n.ChildbirthOutCost1),
                ChildbirthOutPigDeathValue1 = s.Sum(n => n.ChildbirthOutPigDeathValue1),
                ChildbirthOutPigBatchDeath1 = s.Sum(n => n.ChildbirthOutPigBatchDeath1),
                LactatingOutFeed1 = s.Sum(n => n.LactatingOutFeed1),
                LactatingOutVeterinaryDrug1 = s.Sum(n => n.LactatingOutVeterinaryDrug1),
                LactatingOutVaccin1 = s.Sum(n => n.LactatingOutVaccin1),
                LactatingOutOtherMaterial1 = s.Sum(n => n.LactatingOutOtherMaterial1),
                LactatingOutPigTransferValue1 = s.Sum(n => n.LactatingOutPigTransferValue1),
                LactatingOutCost1 = s.Sum(n => n.LactatingOutCost1),
                ReadyBreedingOutFeed1 = s.Sum(n => n.ReadyBreedingOutFeed1),
                ReadyBreedingOutVeterinaryDrug1 = s.Sum(n => n.ReadyBreedingOutVeterinaryDrug1),
                ReadyBreedingOutVaccin1 = s.Sum(n => n.ReadyBreedingOutVaccin1),
                ReadyBreedingOutOtherMaterial1 = s.Sum(n => n.ReadyBreedingOutOtherMaterial1),
                ReadyBreedingOutPigTransferValue1 = s.Sum(n => n.ReadyBreedingOutPigTransferValue1),
                ReadyBreedingOutCost1 = s.Sum(n => n.ReadyBreedingOutCost1),
                ReadyBreedingOutPigDeathValue1 = s.Sum(n => n.ReadyBreedingOutPigDeathValue1),
                ReadyBreedingOutPigBatchDeath1 = s.Sum(n => n.ReadyBreedingOutPigBatchDeath1),
                MissBreedingOutFeed1 = s.Sum(n => n.MissBreedingOutFeed1),
                MissBreedingOutVeterinaryDrug1 = s.Sum(n => n.MissBreedingOutVeterinaryDrug1),
                MissBreedingOutVaccin1 = s.Sum(n => n.MissBreedingOutVaccin1),
                MissBreedingOutOtherMaterial1 = s.Sum(n => n.MissBreedingOutOtherMaterial1),
                MissBreedingOutPigTransferValue1 = s.Sum(n => n.MissBreedingOutPigTransferValue1),
                MissBreedingOutSemen1 = s.Sum(n => n.MissBreedingOutSemen1),
                MissBreedingOutCost1 = s.Sum(n => n.MissBreedingOutCost1),
                MissBreedingOutPigDeathValue1 = s.Sum(n => n.MissBreedingOutPigDeathValue1),
                MissBreedingOutPigBatchDeath1 = s.Sum(n => n.MissBreedingOutPigBatchDeath1),
                AllocateOutAmount1 = s.Sum(n => n.AllocateOutAmount1),
                AllocateOutTotalDepreciation1 = s.Sum(n => n.AllocateOutTotalDepreciation1),
                AllocateOutFeed1 = s.Sum(n => n.AllocateOutFeed1),
                AllocateOutVeterinaryDrug1 = s.Sum(n => n.AllocateOutVeterinaryDrug1),
                AllocateOutVaccin1 = s.Sum(n => n.AllocateOutVaccin1),
                AllocateOutOtherMaterial1 = s.Sum(n => n.AllocateOutOtherMaterial1),
                AllocateOutPigTransferValue1 = s.Sum(n => n.AllocateOutPigTransferValue1),
                AllocateOutSemen1 = s.Sum(n => n.AllocateOutSemen1),
                AllocateOutCost1 = s.Sum(n => n.AllocateOutCost1),
                AllocateOutPigDeathValue1 = s.Sum(n => n.AllocateOutPigDeathValue1),
                AllocateOutPigBatchDeath1 = s.Sum(n => n.AllocateOutPigBatchDeath1),
                BatchOutAmount1 = s.Sum(n => n.BatchOutAmount1),
                BatchOutTotalDepreciation1 = s.Sum(n => n.BatchOutTotalDepreciation1),
                BatchOutFeed1 = s.Sum(n => n.BatchOutFeed1),
                BatchOutVeterinaryDrug1 = s.Sum(n => n.BatchOutVeterinaryDrug1),
                BatchOutVaccin1 = s.Sum(n => n.BatchOutVaccin1),
                BatchOutOtherMaterial1 = s.Sum(n => n.BatchOutOtherMaterial1),
                BatchOutPigTransferValue1 = s.Sum(n => n.BatchOutPigTransferValue1),
                BatchOutSemen1 = s.Sum(n => n.BatchOutSemen1),
                BatchOutCost1 = s.Sum(n => n.BatchOutCost1),
                BatchOutPigDeathValue1 = s.Sum(n => n.BatchOutPigDeathValue1),
                BatchOutPigBatchDeath1 = s.Sum(n => n.BatchOutPigBatchDeath1),
                SaleInsideOutAmount1 = s.Sum(n => n.SaleInsideOutAmount1),
                SaleInsideOutTotalDepreciation1 = s.Sum(n => n.SaleInsideOutTotalDepreciation1),
                SaleInsideOutFeed1 = s.Sum(n => n.SaleInsideOutFeed1),
                SaleInsideOutVeterinaryDrug1 = s.Sum(n => n.SaleInsideOutVeterinaryDrug1),
                SaleInsideOutVaccin1 = s.Sum(n => n.SaleInsideOutVaccin1),
                SaleInsideOutOtherMaterial1 = s.Sum(n => n.SaleInsideOutOtherMaterial1),
                SaleInsideOutPigTransferValue1 = s.Sum(n => n.SaleInsideOutPigTransferValue1),
                SaleInsideOutSemen1 = s.Sum(n => n.SaleInsideOutSemen1),
                SaleInsideOutCost1 = s.Sum(n => n.SaleInsideOutCost1),
                SaleInsideOutPigDeathValue1 = s.Sum(n => n.SaleInsideOutPigDeathValue1),
                SaleInsideOutPigBatchDeath1 = s.Sum(n => n.SaleInsideOutPigBatchDeath1),
                SaleOutsideOutAmount1 = s.Sum(n => n.SaleOutsideOutAmount1),
                SaleOutsideOutTotalDepreciation1 = s.Sum(n => n.SaleOutsideOutTotalDepreciation1),
                SaleOutsideOutFeed1 = s.Sum(n => n.SaleOutsideOutFeed1),
                SaleOutsideOutVeterinaryDrug1 = s.Sum(n => n.SaleOutsideOutVeterinaryDrug1),
                SaleOutsideOutVaccin1 = s.Sum(n => n.SaleOutsideOutVaccin1),
                SaleOutsideOutOtherMaterial1 = s.Sum(n => n.SaleOutsideOutOtherMaterial1),
                SaleOutsideOutPigTransferValue1 = s.Sum(n => n.SaleOutsideOutPigTransferValue1),
                SaleOutsideOutSemen1 = s.Sum(n => n.SaleOutsideOutSemen1),
                SaleOutsideOutCost1 = s.Sum(n => n.SaleOutsideOutCost1),
                SaleOutsideOutPigDeathValue1 = s.Sum(n => n.SaleOutsideOutPigDeathValue1),
                SaleOutsideOutPigBatchDeath1 = s.Sum(n => n.SaleOutsideOutPigBatchDeath1),
                DeathOutAmount1 = s.Sum(n => n.DeathOutAmount1),
                DeathOutTotalDepreciation1 = s.Sum(n => n.DeathOutTotalDepreciation1),
                DeathOutFeed1 = s.Sum(n => n.DeathOutFeed1),
                DeathOutVeterinaryDrug1 = s.Sum(n => n.DeathOutVeterinaryDrug1),
                DeathOutVaccin1 = s.Sum(n => n.DeathOutVaccin1),
                DeathOutOtherMaterial1 = s.Sum(n => n.DeathOutOtherMaterial1),
                DeathOutPigTransferValue1 = s.Sum(n => n.DeathOutPigTransferValue1),
                DeathOutSemen1 = s.Sum(n => n.DeathOutSemen1),
                DeathOutCost1 = s.Sum(n => n.DeathOutCost1),
                DeathOutPigDeathValue1 = s.Sum(n => n.DeathOutPigDeathValue1),
                DeathOutPigBatchDeath1 = s.Sum(n => n.DeathOutPigBatchDeath1),
                ReadyBreedingInAmount1 = s.Sum(n => n.ReadyBreedingInAmount1),
                ReadyBreedingInTotalDepreciation1 = s.Sum(n => n.ReadyBreedingInTotalDepreciation1),
                MissBreedingInAmount1 = s.Sum(n => n.MissBreedingInAmount1),
                MissBreedingInTotalDepreciation1 = s.Sum(n => n.MissBreedingInTotalDepreciation1),
                WeaningInAmount1 = s.Sum(n => n.WeaningInAmount1),
                WeaningInTotalDepreciation1 = s.Sum(n => n.WeaningInTotalDepreciation1),
                ReadyBreedingOutAmount1 = s.Sum(n => n.ReadyBreedingOutAmount1),
                ReadyBreedingOutTotalDepreciation1 = s.Sum(n => n.ReadyBreedingOutTotalDepreciation1),
                MissBreedingOutAmount1 = s.Sum(n => n.MissBreedingOutAmount1),
                MissBreedingOutTotalDepreciation1 = s.Sum(n => n.MissBreedingOutTotalDepreciation1),
                LactatingOutAmount1 = s.Sum(n => n.LactatingOutAmount1),
                LactatingOutTotalDepreciation1 = s.Sum(n => n.LactatingOutTotalDepreciation1),
                LactatingOutPigDeathValue1 = s.Sum(n => n.LactatingOutPigDeathValue1),
                InsidePurchaseInPigletCost2 = s.Sum(n => n.InsidePurchaseInPigletCost2),
                OutPurchaseInPigletCost2 = s.Sum(n => n.OutPurchaseInPigletCost2),
                ChildbirthInPigletCost2 = s.Sum(n => n.ChildbirthInPigletCost2),
                LactatingInTransferValue2 = s.Sum(n => n.LactatingInTransferValue2),
                LactatingInFeed2 = s.Sum(n => n.LactatingInFeed2),
                LactatingInVeterinaryDrug2 = s.Sum(n => n.LactatingInVeterinaryDrug2),
                LactatingInVaccin2 = s.Sum(n => n.LactatingInVaccin2),
                LactatingInOtherMaterial2 = s.Sum(n => n.LactatingInOtherMaterial2),
                LactatingInCost2 = s.Sum(n => n.LactatingInCost2),
                LactatingInPigDeathValue2 = s.Sum(n => n.LactatingInPigDeathValue2),
                BatchInPigletCost2 = s.Sum(n => n.BatchInPigletCost2),
                BatchInFeed2 = s.Sum(n => n.BatchInFeed2),
                BatchInVeterinaryDrug2 = s.Sum(n => n.BatchInVeterinaryDrug2),
                BatchInVaccin2 = s.Sum(n => n.BatchInVaccin2),
                BatchInOtherMaterial2 = s.Sum(n => n.BatchInOtherMaterial2),
                BatchInCost2 = s.Sum(n => n.BatchInCost2),
                BatchInPigTransferValue2 = s.Sum(n => n.BatchInPigTransferValue2),
                BatchInPigDeathValue2 = s.Sum(n => n.BatchInPigDeathValue2),
                BatchInBatchInsideDeath2 = s.Sum(n => n.BatchInBatchInsideDeath2),
                BatchInBatchOutsideDeath2 = s.Sum(n => n.BatchInBatchOutsideDeath2),
                AllocateInPigletCost2 = s.Sum(n => n.AllocateInPigletCost2),
                AllocateInFeed2 = s.Sum(n => n.AllocateInFeed2),
                AllocateInVeterinaryDrug2 = s.Sum(n => n.AllocateInVeterinaryDrug2),
                AllocateInVaccin2 = s.Sum(n => n.AllocateInVaccin2),
                AllocateInOtherMaterial2 = s.Sum(n => n.AllocateInOtherMaterial2),
                AllocateInCost2 = s.Sum(n => n.AllocateInCost2),
                AllocateInPigTransferValue2 = s.Sum(n => n.AllocateInPigTransferValue2),
                AllocateInPigDeathValue2 = s.Sum(n => n.AllocateInPigDeathValue2),
                AllocateInBatchInsideDeath2 = s.Sum(n => n.AllocateInBatchInsideDeath2),
                AllocateInBatchOutsideDeath2 = s.Sum(n => n.AllocateInBatchOutsideDeath2),
                DeathInFeed2 = s.Sum(n => n.DeathInFeed2),
                DeathInVeterinaryDrug2 = s.Sum(n => n.DeathInVeterinaryDrug2),
                DeathInVaccin2 = s.Sum(n => n.DeathInVaccin2),
                DeathInOtherMaterial2 = s.Sum(n => n.DeathInOtherMaterial2),
                DeathInCost2 = s.Sum(n => n.DeathInCost2),
                DeathInPigTransferValue2 = s.Sum(n => n.DeathInPigTransferValue2),
                DeathInPigDeathValue2 = s.Sum(n => n.DeathInPigDeathValue2),
                DeathInBatchInsideDeath2 = s.Sum(n => n.DeathInBatchInsideDeath2),
                DeathInBatchOutsideDeath2 = s.Sum(n => n.DeathInBatchOutsideDeath2),
                SaleInsideInTransferValue2 = s.Sum(n => n.SaleInsideInTransferValue2),
                SaleInsideInFeed2 = s.Sum(n => n.SaleInsideInFeed2),
                SaleInsideInVeterinaryDrug2 = s.Sum(n => n.SaleInsideInVeterinaryDrug2),
                SaleInsideInVaccin2 = s.Sum(n => n.SaleInsideInVaccin2),
                SaleInsideInOtherMaterial2 = s.Sum(n => n.SaleInsideInOtherMaterial2),
                SaleInsideInCost2 = s.Sum(n => n.SaleInsideInCost2),
                SaleInsideInBatchInsideDeath2 = s.Sum(n => n.SaleInsideInBatchInsideDeath2),
                SaleOutsideInTransferValue2 = s.Sum(n => n.SaleOutsideInTransferValue2),
                SaleOutsideInFeed2 = s.Sum(n => n.SaleOutsideInFeed2),
                SaleOutsideInVeterinaryDrug2 = s.Sum(n => n.SaleOutsideInVeterinaryDrug2),
                SaleOutsideInVaccin2 = s.Sum(n => n.SaleOutsideInVaccin2),
                SaleOutsideInOtherMaterial2 = s.Sum(n => n.SaleOutsideInOtherMaterial2),
                SaleOutsideInCost2 = s.Sum(n => n.SaleOutsideInCost2),
                SaleOutsideInBatchInsideDeath2 = s.Sum(n => n.SaleOutsideInBatchInsideDeath2),
                CurrentCostFeed2 = s.Sum(n => n.CurrentCostFeed2),
                CurrentCostVeterinaryDrug2 = s.Sum(n => n.CurrentCostVeterinaryDrug2),
                CurrentCostVaccin2 = s.Sum(n => n.CurrentCostVaccin2),
                CurrentCostOtherMaterial2 = s.Sum(n => n.CurrentCostOtherMaterial2),
                CurrentCostCost2 = s.Sum(n => n.CurrentCostCost2),
                AcrossStepOutPigletCost2 = s.Sum(n => n.AcrossStepOutPigletCost2),
                AcrossStepOutFeed2 = s.Sum(n => n.AcrossStepOutFeed2),
                AcrossStepOutVeterinaryDrug2 = s.Sum(n => n.AcrossStepOutVeterinaryDrug2),
                AcrossStepOutVaccin2 = s.Sum(n => n.AcrossStepOutVaccin2),
                AcrossStepOutOtherMaterial2 = s.Sum(n => n.AcrossStepOutOtherMaterial2),
                AcrossStepOutCost2 = s.Sum(n => n.AcrossStepOutCost2),
                AcrossStepOutPigTransferValue2 = s.Sum(n => n.AcrossStepOutPigTransferValue2),
                AcrossStepOutPigDeathValue2 = s.Sum(n => n.AcrossStepOutPigDeathValue2),
                AcrossStepOutBatchInsideDeath2 = s.Sum(n => n.AcrossStepOutBatchInsideDeath2),
                AcrossStepOutBatchOutsideDeath2 = s.Sum(n => n.AcrossStepOutBatchOutsideDeath2),
                BatchOutPigletCost2 = s.Sum(n => n.BatchOutPigletCost2),
                BatchOutFeed2 = s.Sum(n => n.BatchOutFeed2),
                BatchOutVeterinaryDrug2 = s.Sum(n => n.BatchOutVeterinaryDrug2),
                BatchOutVaccin2 = s.Sum(n => n.BatchOutVaccin2),
                BatchOutOtherMaterial2 = s.Sum(n => n.BatchOutOtherMaterial2),
                BatchOutCost2 = s.Sum(n => n.BatchOutCost2),
                BatchOutPigTransferValue2 = s.Sum(n => n.BatchOutPigTransferValue2),
                BatchOutPigDeathValue2 = s.Sum(n => n.BatchOutPigDeathValue2),
                BatchOutBatchInsideDeath2 = s.Sum(n => n.BatchOutBatchInsideDeath2),
                BatchOutBatchOutsideDeath2 = s.Sum(n => n.BatchOutBatchOutsideDeath2),
                AllocateOutPigletCost2 = s.Sum(n => n.AllocateOutPigletCost2),
                AllocateOutFeed2 = s.Sum(n => n.AllocateOutFeed2),
                AllocateOutVeterinaryDrug2 = s.Sum(n => n.AllocateOutVeterinaryDrug2),
                AllocateOutVaccin2 = s.Sum(n => n.AllocateOutVaccin2),
                AllocateOutOtherMaterial2 = s.Sum(n => n.AllocateOutOtherMaterial2),
                AllocateOutCost2 = s.Sum(n => n.AllocateOutCost2),
                AllocateOutPigTransferValue2 = s.Sum(n => n.AllocateOutPigTransferValue2),
                AllocateOutPigDeathValue2 = s.Sum(n => n.AllocateOutPigDeathValue2),
                AllocateOutBatchInsideDeath2 = s.Sum(n => n.AllocateOutBatchInsideDeath2),
                AllocateOutBatchOutsideDeath2 = s.Sum(n => n.AllocateOutBatchOutsideDeath2),
                DeathOutPigletCost2 = s.Sum(n => n.DeathOutPigletCost2),
                DeathOutFeed2 = s.Sum(n => n.DeathOutFeed2),
                DeathOutVeterinaryDrug2 = s.Sum(n => n.DeathOutVeterinaryDrug2),
                DeathOutVaccin2 = s.Sum(n => n.DeathOutVaccin2),
                DeathOutOtherMaterial2 = s.Sum(n => n.DeathOutOtherMaterial2),
                DeathOutCost2 = s.Sum(n => n.DeathOutCost2),
                DeathOutPigTransferValue2 = s.Sum(n => n.DeathOutPigTransferValue2),
                DeathOutPigDeathValue2 = s.Sum(n => n.DeathOutPigDeathValue2),
                DeathOutBatchInsideDeath2 = s.Sum(n => n.DeathOutBatchInsideDeath2),
                DeathOutBatchOutsideDeath2 = s.Sum(n => n.DeathOutBatchOutsideDeath2),
                SaleInsideOutPigletCost2 = s.Sum(n => n.SaleInsideOutPigletCost2),
                SaleInsideOutFeed2 = s.Sum(n => n.SaleInsideOutFeed2),
                SaleInsideOutVeterinaryDrug2 = s.Sum(n => n.SaleInsideOutVeterinaryDrug2),
                SaleInsideOutVaccin2 = s.Sum(n => n.SaleInsideOutVaccin2),
                SaleInsideOutOtherMaterial2 = s.Sum(n => n.SaleInsideOutOtherMaterial2),
                SaleInsideOutCost2 = s.Sum(n => n.SaleInsideOutCost2),
                SaleInsideOutPigTransferValue2 = s.Sum(n => n.SaleInsideOutPigTransferValue2),
                SaleInsideOutPigDeathValue2 = s.Sum(n => n.SaleInsideOutPigDeathValue2),
                SaleInsideOutBatchInsideDeath2 = s.Sum(n => n.SaleInsideOutBatchInsideDeath2),
                SaleInsideOutBatchOutsideDeath2 = s.Sum(n => n.SaleInsideOutBatchOutsideDeath2),
                SaleOutsideOutPigletCost2 = s.Sum(n => n.SaleOutsideOutPigletCost2),
                SaleOutsideOutFeed2 = s.Sum(n => n.SaleOutsideOutFeed2),
                SaleOutsideOutVeterinaryDrug2 = s.Sum(n => n.SaleOutsideOutVeterinaryDrug2),
                SaleOutsideOutVaccin2 = s.Sum(n => n.SaleOutsideOutVaccin2),
                SaleOutsideOutOtherMaterial2 = s.Sum(n => n.SaleOutsideOutOtherMaterial2),
                SaleOutsideOutCost2 = s.Sum(n => n.SaleOutsideOutCost2),
                SaleOutsideOutPigTransferValue2 = s.Sum(n => n.SaleOutsideOutPigTransferValue2),
                SaleOutsideOutPigDeathValue2 = s.Sum(n => n.SaleOutsideOutPigDeathValue2),
                SaleOutsideOutBatchInsideDeath2 = s.Sum(n => n.SaleOutsideOutBatchInsideDeath2),
                SaleOutsideOutBatchOutsideDeath2 = s.Sum(n => n.SaleOutsideOutBatchOutsideDeath2),
                EmergenceOutPigletCost2 = s.Sum(n => n.EmergenceOutPigletCost2),
                EmergenceOutFeed2 = s.Sum(n => n.EmergenceOutFeed2),
                EmergenceOutVeterinaryDrug2 = s.Sum(n => n.EmergenceOutVeterinaryDrug2),
                EmergenceOutVaccin2 = s.Sum(n => n.EmergenceOutVaccin2),
                EmergenceOutOtherMaterial2 = s.Sum(n => n.EmergenceOutOtherMaterial2),
                EmergenceOutCost2 = s.Sum(n => n.EmergenceOutCost2),
                EmergenceOutPigTransferValue2 = s.Sum(n => n.EmergenceOutPigTransferValue2),
                EmergenceOutPigDeathValue2 = s.Sum(n => n.EmergenceOutPigDeathValue2),
                EmergenceOutBatchInsideDeath2 = s.Sum(n => n.EmergenceOutBatchInsideDeath2),
                EmergenceOutBatchOutsideDeath2 = s.Sum(n => n.EmergenceOutBatchOutsideDeath2),
                AcrossStepInPigletCost3 = s.Sum(n => n.AcrossStepInPigletCost3),
                InsidePurchaseInPigletCost3 = s.Sum(n => n.InsidePurchaseInPigletCost3),
                OutPurchaseInPigletCost3 = s.Sum(n => n.OutPurchaseInPigletCost3),
                RecycleBreederInPigletCost3 = s.Sum(n => n.RecycleBreederInPigletCost3),
                BatchInPigletCost3 = s.Sum(n => n.BatchInPigletCost3),
                BatchInFeed3 = s.Sum(n => n.BatchInFeed3),
                BatchInVeterinaryDrug3 = s.Sum(n => n.BatchInVeterinaryDrug3),
                BatchInVaccin3 = s.Sum(n => n.BatchInVaccin3),
                BatchInOtherMaterial3 = s.Sum(n => n.BatchInOtherMaterial3),
                BatchInCost3 = s.Sum(n => n.BatchInCost3),
                BatchInBatchInsideDeath3 = s.Sum(n => n.BatchInBatchInsideDeath3),
                BatchInBatchOutsideDeath3 = s.Sum(n => n.BatchInBatchOutsideDeath3),
                AllocateInPigletCost3 = s.Sum(n => n.AllocateInPigletCost3),
                AllocateInFeed3 = s.Sum(n => n.AllocateInFeed3),
                AllocateInVeterinaryDrug3 = s.Sum(n => n.AllocateInVeterinaryDrug3),
                AllocateInVaccin3 = s.Sum(n => n.AllocateInVaccin3),
                AllocateInOtherMaterial3 = s.Sum(n => n.AllocateInOtherMaterial3),
                AllocateInCost3 = s.Sum(n => n.AllocateInCost3),
                AllocateInBatchInsideDeath3 = s.Sum(n => n.AllocateInBatchInsideDeath3),
                AllocateInBatchOutsideDeath3 = s.Sum(n => n.AllocateInBatchOutsideDeath3),
                DeathInFeed3 = s.Sum(n => n.DeathInFeed3),
                DeathInVeterinaryDrug3 = s.Sum(n => n.DeathInVeterinaryDrug3),
                DeathInVaccin3 = s.Sum(n => n.DeathInVaccin3),
                DeathInOtherMaterial3 = s.Sum(n => n.DeathInOtherMaterial3),
                DeathInCost3 = s.Sum(n => n.DeathInCost3),
                DeathInBatchInsideDeath3 = s.Sum(n => n.DeathInBatchInsideDeath3),
                DeathInBatchOutsideDeath3 = s.Sum(n => n.DeathInBatchOutsideDeath3),
                CurrentCostFeed3 = s.Sum(n => n.CurrentCostFeed3),
                CurrentCostVeterinaryDrug3 = s.Sum(n => n.CurrentCostVeterinaryDrug3),
                CurrentCostVaccin3 = s.Sum(n => n.CurrentCostVaccin3),
                CurrentCostOtherMaterial3 = s.Sum(n => n.CurrentCostOtherMaterial3),
                CurrentCostCost3 = s.Sum(n => n.CurrentCostCost3),
                AcrossStepOutPigletCost3 = s.Sum(n => n.AcrossStepOutPigletCost3),
                AcrossStepOutFeed3 = s.Sum(n => n.AcrossStepOutFeed3),
                AcrossStepOutVeterinaryDrug3 = s.Sum(n => n.AcrossStepOutVeterinaryDrug3),
                AcrossStepOutVaccin3 = s.Sum(n => n.AcrossStepOutVaccin3),
                AcrossStepOutOtherMaterial3 = s.Sum(n => n.AcrossStepOutOtherMaterial3),
                AcrossStepOutCost3 = s.Sum(n => n.AcrossStepOutCost3),
                AcrossStepOutBatchInsideDeath3 = s.Sum(n => n.AcrossStepOutBatchInsideDeath3),
                AcrossStepOutBatchOutsideDeath3 = s.Sum(n => n.AcrossStepOutBatchOutsideDeath3),
                BatchOutPigletCost3 = s.Sum(n => n.BatchOutPigletCost3),
                BatchOutFeed3 = s.Sum(n => n.BatchOutFeed3),
                BatchOutVeterinaryDrug3 = s.Sum(n => n.BatchOutVeterinaryDrug3),
                BatchOutVaccin3 = s.Sum(n => n.BatchOutVaccin3),
                BatchOutOtherMaterial3 = s.Sum(n => n.BatchOutOtherMaterial3),
                BatchOutCost3 = s.Sum(n => n.BatchOutCost3),
                BatchOutBatchInsideDeath3 = s.Sum(n => n.BatchOutBatchInsideDeath3),
                BatchOutBatchOutsideDeath3 = s.Sum(n => n.BatchOutBatchOutsideDeath3),
                AllocateOutPigletCost3 = s.Sum(n => n.AllocateOutPigletCost3),
                AllocateOutFeed3 = s.Sum(n => n.AllocateOutFeed3),
                AllocateOutVeterinaryDrug3 = s.Sum(n => n.AllocateOutVeterinaryDrug3),
                AllocateOutVaccin3 = s.Sum(n => n.AllocateOutVaccin3),
                AllocateOutOtherMaterial3 = s.Sum(n => n.AllocateOutOtherMaterial3),
                AllocateOutCost3 = s.Sum(n => n.AllocateOutCost3),
                AllocateOutBatchInsideDeath3 = s.Sum(n => n.AllocateOutBatchInsideDeath3),
                AllocateOutBatchOutsideDeath3 = s.Sum(n => n.AllocateOutBatchOutsideDeath3),
                DeathOutPigletCost3 = s.Sum(n => n.DeathOutPigletCost3),
                DeathOutFeed3 = s.Sum(n => n.DeathOutFeed3),
                DeathOutVeterinaryDrug3 = s.Sum(n => n.DeathOutVeterinaryDrug3),
                DeathOutVaccin3 = s.Sum(n => n.DeathOutVaccin3),
                DeathOutOtherMaterial3 = s.Sum(n => n.DeathOutOtherMaterial3),
                DeathOutCost3 = s.Sum(n => n.DeathOutCost3),
                DeathOutBatchInsideDeath3 = s.Sum(n => n.DeathOutBatchInsideDeath3),
                DeathOutBatchOutsideDeath3 = s.Sum(n => n.DeathOutBatchOutsideDeath3),
                SaleInsideOutPigletCost3 = s.Sum(n => n.SaleInsideOutPigletCost3),
                SaleInsideOutFeed3 = s.Sum(n => n.SaleInsideOutFeed3),
                SaleInsideOutVeterinaryDrug3 = s.Sum(n => n.SaleInsideOutVeterinaryDrug3),
                SaleInsideOutVaccin3 = s.Sum(n => n.SaleInsideOutVaccin3),
                SaleInsideOutOtherMaterial3 = s.Sum(n => n.SaleInsideOutOtherMaterial3),
                SaleInsideOutCost3 = s.Sum(n => n.SaleInsideOutCost3),
                SaleInsideOutBatchInsideDeath3 = s.Sum(n => n.SaleInsideOutBatchInsideDeath3),
                SaleInsideOutBatchOutsideDeath3 = s.Sum(n => n.SaleInsideOutBatchOutsideDeath3),
                SaleOutsideOutPigletCost3 = s.Sum(n => n.SaleOutsideOutPigletCost3),
                SaleOutsideOutFeed3 = s.Sum(n => n.SaleOutsideOutFeed3),
                SaleOutsideOutVeterinaryDrug3 = s.Sum(n => n.SaleOutsideOutVeterinaryDrug3),
                SaleOutsideOutVaccin3 = s.Sum(n => n.SaleOutsideOutVaccin3),
                SaleOutsideOutOtherMaterial3 = s.Sum(n => n.SaleOutsideOutOtherMaterial3),
                SaleOutsideOutCost3 = s.Sum(n => n.SaleOutsideOutCost3),
                SaleOutsideOutBatchInsideDeath3 = s.Sum(n => n.SaleOutsideOutBatchInsideDeath3),
                SaleOutsideOutBatchOutsideDeath3 = s.Sum(n => n.SaleOutsideOutBatchOutsideDeath3),
                EmergenceOutPigletCost3 = s.Sum(n => n.EmergenceOutPigletCost3),
                EmergenceOutFeed3 = s.Sum(n => n.EmergenceOutFeed3),
                EmergenceOutVeterinaryDrug3 = s.Sum(n => n.EmergenceOutVeterinaryDrug3),
                EmergenceOutVaccin3 = s.Sum(n => n.EmergenceOutVaccin3),
                EmergenceOutOtherMaterial3 = s.Sum(n => n.EmergenceOutOtherMaterial3),
                EmergenceOutCost3 = s.Sum(n => n.EmergenceOutCost3),
                EmergenceOutBatchInsideDeath3 = s.Sum(n => n.EmergenceOutBatchInsideDeath3),
                EmergenceOutBatchOutsideDeath3 = s.Sum(n => n.EmergenceOutBatchOutsideDeath3),
                AcrossStepInPigletCost4 = s.Sum(n => n.AcrossStepInPigletCost4),
                InsidePurchaseInPigletCost4 = s.Sum(n => n.InsidePurchaseInPigletCost4),
                OutPurchaseInPigletCost4 = s.Sum(n => n.OutPurchaseInPigletCost4),
                RecycleBreederInPigletCost4 = s.Sum(n => n.RecycleBreederInPigletCost4),
                BatchInPigletCost4 = s.Sum(n => n.BatchInPigletCost4),
                BatchInFeed4 = s.Sum(n => n.BatchInFeed4),
                BatchInVeterinaryDrug4 = s.Sum(n => n.BatchInVeterinaryDrug4),
                BatchInVaccin4 = s.Sum(n => n.BatchInVaccin4),
                BatchInOtherMaterial4 = s.Sum(n => n.BatchInOtherMaterial4),
                BatchInCost4 = s.Sum(n => n.BatchInCost4),
                BatchInBatchInsideDeath4 = s.Sum(n => n.BatchInBatchInsideDeath4),
                BatchInBatchOutsideDeath4 = s.Sum(n => n.BatchInBatchOutsideDeath4),
                AllocateInPigletCost4 = s.Sum(n => n.AllocateInPigletCost4),
                AllocateInFeed4 = s.Sum(n => n.AllocateInFeed4),
                AllocateInVeterinaryDrug4 = s.Sum(n => n.AllocateInVeterinaryDrug4),
                AllocateInVaccin4 = s.Sum(n => n.AllocateInVaccin4),
                AllocateInOtherMaterial4 = s.Sum(n => n.AllocateInOtherMaterial4),
                AllocateInCost4 = s.Sum(n => n.AllocateInCost4),
                AllocateInBatchInsideDeath4 = s.Sum(n => n.AllocateInBatchInsideDeath4),
                AllocateInBatchOutsideDeath4 = s.Sum(n => n.AllocateInBatchOutsideDeath4),
                DeathInFeed4 = s.Sum(n => n.DeathInFeed4),
                DeathInVeterinaryDrug4 = s.Sum(n => n.DeathInVeterinaryDrug4),
                DeathInVaccin4 = s.Sum(n => n.DeathInVaccin4),
                DeathInOtherMaterial4 = s.Sum(n => n.DeathInOtherMaterial4),
                DeathInCost4 = s.Sum(n => n.DeathInCost4),
                DeathInBatchInsideDeath4 = s.Sum(n => n.DeathInBatchInsideDeath4),
                DeathInBatchOutsideDeath4 = s.Sum(n => n.DeathInBatchOutsideDeath4),
                CurrentCostFeed4 = s.Sum(n => n.CurrentCostFeed4),
                CurrentCostVeterinaryDrug4 = s.Sum(n => n.CurrentCostVeterinaryDrug4),
                CurrentCostVaccin4 = s.Sum(n => n.CurrentCostVaccin4),
                CurrentCostOtherMaterial4 = s.Sum(n => n.CurrentCostOtherMaterial4),
                CurrentCostCost4 = s.Sum(n => n.CurrentCostCost4),
                AcrossStepOutPigletCost4 = s.Sum(n => n.AcrossStepOutPigletCost4),
                AcrossStepOutFeed4 = s.Sum(n => n.AcrossStepOutFeed4),
                AcrossStepOutVeterinaryDrug4 = s.Sum(n => n.AcrossStepOutVeterinaryDrug4),
                AcrossStepOutVaccin4 = s.Sum(n => n.AcrossStepOutVaccin4),
                AcrossStepOutOtherMaterial4 = s.Sum(n => n.AcrossStepOutOtherMaterial4),
                AcrossStepOutCost4 = s.Sum(n => n.AcrossStepOutCost4),
                AcrossStepOutBatchInsideDeath4 = s.Sum(n => n.AcrossStepOutBatchInsideDeath4),
                AcrossStepOutBatchOutsideDeath4 = s.Sum(n => n.AcrossStepOutBatchOutsideDeath4),
                BatchOutPigletCost4 = s.Sum(n => n.BatchOutPigletCost4),
                BatchOutFeed4 = s.Sum(n => n.BatchOutFeed4),
                BatchOutVeterinaryDrug4 = s.Sum(n => n.BatchOutVeterinaryDrug4),
                BatchOutVaccin4 = s.Sum(n => n.BatchOutVaccin4),
                BatchOutOtherMaterial4 = s.Sum(n => n.BatchOutOtherMaterial4),
                BatchOutCost4 = s.Sum(n => n.BatchOutCost4),
                BatchOutBatchInsideDeath4 = s.Sum(n => n.BatchOutBatchInsideDeath4),
                BatchOutBatchOutsideDeath4 = s.Sum(n => n.BatchOutBatchOutsideDeath4),
                AllocateOutPigletCost4 = s.Sum(n => n.AllocateOutPigletCost4),
                AllocateOutFeed4 = s.Sum(n => n.AllocateOutFeed4),
                AllocateOutVeterinaryDrug4 = s.Sum(n => n.AllocateOutVeterinaryDrug4),
                AllocateOutVaccin4 = s.Sum(n => n.AllocateOutVaccin4),
                AllocateOutOtherMaterial4 = s.Sum(n => n.AllocateOutOtherMaterial4),
                AllocateOutCost4 = s.Sum(n => n.AllocateOutCost4),
                AllocateOutBatchInsideDeath4 = s.Sum(n => n.AllocateOutBatchInsideDeath4),
                AllocateOutBatchOutsideDeath4 = s.Sum(n => n.AllocateOutBatchOutsideDeath4),
                DeathOutPigletCost4 = s.Sum(n => n.DeathOutPigletCost4),
                DeathOutFeed4 = s.Sum(n => n.DeathOutFeed4),
                DeathOutVeterinaryDrug4 = s.Sum(n => n.DeathOutVeterinaryDrug4),
                DeathOutVaccin4 = s.Sum(n => n.DeathOutVaccin4),
                DeathOutOtherMaterial4 = s.Sum(n => n.DeathOutOtherMaterial4),
                DeathOutCost4 = s.Sum(n => n.DeathOutCost4),
                DeathOutBatchInsideDeath4 = s.Sum(n => n.DeathOutBatchInsideDeath4),
                DeathOutBatchOutsideDeath4 = s.Sum(n => n.DeathOutBatchOutsideDeath4),
                SaleInsideOutPigletCost4 = s.Sum(n => n.SaleInsideOutPigletCost4),
                SaleInsideOutFeed4 = s.Sum(n => n.SaleInsideOutFeed4),
                SaleInsideOutVeterinaryDrug4 = s.Sum(n => n.SaleInsideOutVeterinaryDrug4),
                SaleInsideOutVaccin4 = s.Sum(n => n.SaleInsideOutVaccin4),
                SaleInsideOutOtherMaterial4 = s.Sum(n => n.SaleInsideOutOtherMaterial4),
                SaleInsideOutCost4 = s.Sum(n => n.SaleInsideOutCost4),
                SaleInsideOutBatchInsideDeath4 = s.Sum(n => n.SaleInsideOutBatchInsideDeath4),
                SaleInsideOutBatchOutsideDeath4 = s.Sum(n => n.SaleInsideOutBatchOutsideDeath4),
                SaleOutsideOutPigletCost4 = s.Sum(n => n.SaleOutsideOutPigletCost4),
                SaleOutsideOutFeed4 = s.Sum(n => n.SaleOutsideOutFeed4),
                SaleOutsideOutVeterinaryDrug4 = s.Sum(n => n.SaleOutsideOutVeterinaryDrug4),
                SaleOutsideOutVaccin4 = s.Sum(n => n.SaleOutsideOutVaccin4),
                SaleOutsideOutOtherMaterial4 = s.Sum(n => n.SaleOutsideOutOtherMaterial4),
                SaleOutsideOutCost4 = s.Sum(n => n.SaleOutsideOutCost4),
                SaleOutsideOutBatchInsideDeath4 = s.Sum(n => n.SaleOutsideOutBatchInsideDeath4),
                SaleOutsideOutBatchOutsideDeath4 = s.Sum(n => n.SaleOutsideOutBatchOutsideDeath4),
                FeedDays5 = s.Sum(n => n.FeedDays5),
                Depreciated5 = s.Sum(n => n.Depreciated5),
                MaterialCost5 = s.Sum(n => n.MaterialCost5),
                MedicantCost5 = s.Sum(n => n.MedicantCost5),
                VaccineCost5 = s.Sum(n => n.VaccineCost5),
                OtherCost5 = s.Sum(n => n.OtherCost5),
                FeeCost5 = s.Sum(n => n.FeeCost5),
                SumCost5 = s.Sum(n => n.SumCost5),
                CurrentPigCost6 = s.Sum(n => n.CurrentPigCost6),
                CurrentMaterial6 = s.Sum(n => n.CurrentMaterial6),
                CurrentMedicant6 = s.Sum(n => n.CurrentMedicant6),
                CurrentVaccine6 = s.Sum(n => n.CurrentVaccine6),
                CurrentOther6 = s.Sum(n => n.CurrentOther6),
                CurrentInnerDeath6 = s.Sum(n => n.CurrentInnerDeath6),
                CurrentOuterDeath6 = s.Sum(n => n.CurrentOuterDeath6),
                CurrentDirectFee6 = s.Sum(n => n.CurrentDirectFee6),
                CurrentBuildingFee6 = s.Sum(n => n.CurrentBuildingFee6),
                CurrentPredictFeedFee6 = s.Sum(n => n.CurrentPredictFeedFee6),
                CurrentAdjustFeedFee6 = s.Sum(n => n.CurrentAdjustFeedFee6),
                CurrentAdditionFee6 = s.Sum(n => n.CurrentAdditionFee6),
                CurrentHoldUpFee6 = s.Sum(n => n.CurrentHoldUpFee6),
                RecyclePigCost6 = s.Sum(n => n.RecyclePigCost6),
                RecycleMaterial6 = s.Sum(n => n.RecycleMaterial6),
                RecycleMedicant6 = s.Sum(n => n.RecycleMedicant6),
                RecycleVaccine6 = s.Sum(n => n.RecycleVaccine6),
                RecycleOther6 = s.Sum(n => n.RecycleOther6),
                RecycleInnerDeath6 = s.Sum(n => n.RecycleInnerDeath6),
                RecycleOuterDeath6 = s.Sum(n => n.RecycleOuterDeath6),
                RecycleDirectFee6 = s.Sum(n => n.RecycleDirectFee6),
                RecycleBuildingFee6 = s.Sum(n => n.RecycleBuildingFee6),
                RecyclePredictFeedFee6 = s.Sum(n => n.RecyclePredictFeedFee6),
                RecycleAdjustFeedFee6 = s.Sum(n => n.RecycleAdjustFeedFee6),
                RecycleAdditionFee6 = s.Sum(n => n.RecycleAdditionFee6),
                RecycleHoldUpFee6 = s.Sum(n => n.RecycleHoldUpFee6),
                DeathPigCost6 = s.Sum(n => n.DeathPigCost6),
                DeathMaterial6 = s.Sum(n => n.DeathMaterial6),
                DeathMedicant6 = s.Sum(n => n.DeathMedicant6),
                DeathVaccine6 = s.Sum(n => n.DeathVaccine6),
                DeathOther6 = s.Sum(n => n.DeathOther6),
                DeathInnerDeath6 = s.Sum(n => n.DeathInnerDeath6),
                DeathOuterDeath6 = s.Sum(n => n.DeathOuterDeath6),
                DeathDirectFee6 = s.Sum(n => n.DeathDirectFee6),
                DeathBuildingFee6 = s.Sum(n => n.DeathBuildingFee6),
                DeathPredictFeedFee6 = s.Sum(n => n.DeathPredictFeedFee6),
                DeathAdjustFeedFee6 = s.Sum(n => n.DeathAdjustFeedFee6),
                DeathAdditionFee6 = s.Sum(n => n.DeathAdditionFee6),
                DeathHoldUpFee6 = s.Sum(n => n.DeathHoldUpFee6),
                AdjustCount6 = s.Sum(n => n.AdjustCount6),
                DeathPigCount6 = s.Sum(n => n.DeathPigCount6),
                BeginAdditionFee6 = s.Sum(n => n.BeginAdditionFee6),
                BeginAdjustFeedFee6 = s.Sum(n => n.BeginAdjustFeedFee6),
                BeginBuildingFee6 = s.Sum(n => n.BeginBuildingFee6),
                BeginDirectFee6 = s.Sum(n => n.BeginDirectFee6),
                BeginHoldUpFee6 = s.Sum(n => n.BeginHoldUpFee6),
                BeginInnerDeath6 = s.Sum(n => n.BeginInnerDeath6),
                BeginMaterial6 = s.Sum(n => n.BeginMaterial6),
                BeginMedicant6 = s.Sum(n => n.BeginMedicant6),
                BeginOther6 = s.Sum(n => n.BeginOther6),
                BeginOuterDeath6 = s.Sum(n => n.BeginOuterDeath6),
                BeginPigCost6 = s.Sum(n => n.BeginPigCost6),
                BeginPigCount6 = s.Sum(n => n.BeginPigCount6),
                BeginPredictFeedFee6 = s.Sum(n => n.BeginPredictFeedFee6),
                BeginVaccine6 = s.Sum(n => n.BeginVaccine6),
                CurrentPigCount6 = s.Sum(n => n.CurrentPigCount6),
                EndAdditionFee6 = s.Sum(n => n.EndAdditionFee6),
                EndAdjustFeedFee6 = s.Sum(n => n.EndAdjustFeedFee6),
                EndBuildingFee6 = s.Sum(n => n.EndBuildingFee6),
                EndDirectFee6 = s.Sum(n => n.EndDirectFee6),
                EndHoldUpFee6 = s.Sum(n => n.EndHoldUpFee6),
                EndInnerDeath6 = s.Sum(n => n.EndInnerDeath6),
                EndMaterial6 = s.Sum(n => n.EndMaterial6),
                EndMedicant6 = s.Sum(n => n.EndMedicant6),
                EndOther6 = s.Sum(n => n.EndOther6),
                EndOuterDeath6 = s.Sum(n => n.EndOuterDeath6),
                EndPigCost6 = s.Sum(n => n.EndPigCost6),
                EndPigCount6 = s.Sum(n => n.EndPigCount6),
                EndPredictFeedFee6 = s.Sum(n => n.EndPredictFeedFee6),
                EndVaccine6 = s.Sum(n => n.EndVaccine6),
                RecyclePigCount6 = s.Sum(n => n.RecyclePigCount6),
            }).ToList();
            return summaryList;
        }
        /// <summary>
        /// 获取当前单位人员信息
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public List<PM_Person> GetPerson(DropSelectSearch searchModel)
        {
            try
            {
                var param = new
                {
                    EnterpriseID = searchModel.EnterpriseID,
                    Type = 4,
                };
                var resultModel = _httpClientUtil.PostJsonAsync<ResultModel<PM_Person>>($"{_hostCongfiguration.QlwServiceHost}/api/BIZPerson/getPerson", param).Result;
                if (resultModel.ResultState)
                {
                    var person = resultModel.Data;
                    person = person.Where((x, i) => person.FindIndex(z => z.PersonID == x.PersonID) == i).ToList();
                    return person;
                }
                else
                {
                    return new List<PM_Person>();
                }
            }
            catch (Exception ex)
            {
                return new List<PM_Person>();
            }
        }


        /// <summary>
        /// 指标
        /// </summary>
        /// <returns></returns>
        public List<string> GetReprotKey1()
        {
            List<string> value = new List<string>();
            value.Add("2211221408050000102");
            value.Add("2211221408050000202");
            value.Add("2211221408050000302");
            value.Add("2211221408050000402");
            value.Add("2211221408050000502");
            value.Add("2211221408050000602");
            value.Add("2211221408050000702");
            value.Add("2211221408050000802");
            value.Add("2211221408050000902");
            value.Add("2211221408050001002");
            value.Add("2211221408050001102");
            value.Add("2211221408050001202");
            value.Add("2211221408050001302");
            value.Add("2211221408050001402");
            value.Add("2211221408050001502");
            value.Add("2211221408050001602");
            value.Add("2211221408050001702");
            value.Add("2211221408050001802");
            value.Add("2211221408050001902");
            value.Add("2211221408050002002");
            value.Add("2211221408050002102");
            value.Add("2211221408050002202");
            value.Add("2211221408050002302");
            value.Add("2211221408050002402");
            value.Add("2211221408050002502");
            value.Add("2211221408050002602");
            value.Add("2211221408050002702");
            value.Add("2211221408050002802");
            value.Add("2211221408050002902");
            value.Add("2211221408050003002");
            value.Add("2211221408050003102");
            value.Add("2211221408050003202");
            value.Add("2211221408050003302");
            value.Add("2211221408050003402");
            value.Add("2211221408050003502");
            value.Add("2211221408050003602");
            value.Add("2211221408050003702");
            value.Add("2211221408050003802");
            value.Add("2211221408050003902");
            value.Add("2211221408050004002");
            value.Add("2211221408050004102");
            value.Add("2211221408050004202");
            value.Add("2211221408050004302");
            value.Add("2211221408050004402");
            value.Add("2211221408050004502");
            value.Add("2211221408050004602");
            value.Add("2211221408050004702");
            value.Add("2211221408050004802");
            value.Add("2211221408050004902");
            value.Add("2211221408050005002");
            value.Add("2211221408050005102");
            value.Add("2211221408050005202");
            value.Add("2211221408050005302");
            value.Add("2211221408050005402");
            value.Add("2211221408050005502");
            value.Add("2211221408050005602");
            value.Add("2211221408050005702");
            value.Add("2211221408050005802");
            value.Add("2211221408050005902");
            value.Add("2211221408050006002");
            value.Add("2211221408050006102");
            value.Add("2211221408050006202");
            value.Add("2211221408050006302");
            value.Add("2211221408050006402");
            value.Add("2211221408050006502");
            value.Add("2211221408050006602");
            value.Add("2211221408050006702");
            value.Add("2211221408050006802");
            value.Add("2211221408050006902");
            value.Add("2211221408050007002");
            value.Add("2211221408050007102");
            value.Add("2211221408050007202");
            value.Add("2211221408050007302");
            value.Add("2211221408050007402");
            value.Add("2211221408050007502");
            value.Add("2211221408050007602");
            value.Add("2211221408050007702");
            value.Add("2211221408050007802");
            value.Add("2211221408050007902");
            value.Add("2211221408050008002");
            value.Add("2211221408050008102");
            value.Add("2211221408050008202");
            value.Add("2211221408050008302");
            value.Add("2211221408050008402");
            value.Add("2211221408050008502");
            value.Add("2211221408050008602");
            value.Add("2211221408050008702");
            value.Add("2211221408050008802");
            value.Add("2211221408050008902");
            value.Add("2211221408050009002");
            value.Add("2211221408050009102");
            value.Add("2211221408050009202");
            value.Add("2211221408050009302");
            value.Add("2211221408050009402");
            value.Add("2211221408050009502");
            value.Add("2211221408050009602");
            value.Add("2211221408050009702");
            value.Add("2211221408050009802");
            value.Add("2211221408050009902");
            value.Add("2211221408050010002");
            value.Add("2211221408050010102");
            value.Add("2211221408050010202");
            value.Add("2211221408050010302");
            value.Add("2211221408050010402");
            value.Add("2211221408050010502");
            value.Add("2211221408050010602");
            value.Add("2211221408050010702");
            value.Add("2211221408050010802");
            value.Add("2211221408050010902");
            value.Add("2211221408050011002");
            value.Add("2211221408050011102");
            value.Add("2211221408050011202");
            value.Add("2211221408050011302");
            value.Add("2211221408050011402");
            value.Add("2211221408050011502");
            value.Add("2211221408050011602");
            value.Add("2211221408050011702");
            value.Add("2211221408050011802");
            value.Add("2211221408050011902");
            value.Add("2211221408050012002");
            value.Add("2211221408050012102");
            value.Add("2211221408050012202");
            value.Add("2211221408050012302");
            value.Add("2211221408050012402");
            value.Add("2211221408050012502");
            value.Add("2211221408050012602");
            value.Add("2211221408050012702");
            value.Add("2211221408050012802");
            value.Add("2211221408050012902");
            value.Add("2211221408050013002");
            value.Add("2211221408050013102");
            value.Add("2211221408050013202");
            value.Add("2211221408050013302");
            value.Add("2211221408050013402");
            value.Add("2211221408050013502");
            value.Add("2211221408050013602");
            value.Add("2211221408050013702");
            value.Add("2211221408050013802");
            value.Add("2211221408050013902");
            value.Add("2211221408050014002");
            value.Add("2211221408050014102");
            value.Add("2211221408050014202");
            value.Add("2211221408050014302");
            value.Add("2211221408050014402");
            value.Add("2211221408050014502");
            value.Add("2211221408050014602");
            value.Add("2211221408050014702");
            value.Add("2211221408050014802");
            value.Add("2211221408050014902");
            value.Add("2211221408050015002");
            value.Add("2211221408050015102");
            value.Add("2211221408050015202");
            value.Add("2211221408050015302");
            value.Add("2211221408050015402");
            value.Add("2211221408050015502");
            value.Add("2211221408050015602");
            value.Add("2211221408050015702");
            value.Add("2211221408050015802");
            value.Add("2211221408050015902");
            value.Add("2211221408050016002");
            value.Add("2211221408050016102");
            value.Add("2211221408050016202");
            value.Add("2211221408050016302");
            value.Add("2211221408050016402");
            value.Add("2211221408050016502");
            value.Add("2211221408050016602");
            value.Add("2211221408050016702");
            value.Add("2211221408050016802");
            value.Add("2211221408050016902");
            value.Add("2211221408050017002");
            value.Add("2211221408050017102");
            value.Add("2211221408050017202");
            value.Add("2211221408050017302");
            value.Add("2211221408050017402");
            value.Add("2211221408050017502");
            value.Add("2211221408050017602");
            return value;
        }
        public List<string> GetReprotKey2()
        {
            List<string> value = new List<string>();
            value.Add("2211221408250000102");
            value.Add("2211221408250000202");
            value.Add("2211221408250000302");
            value.Add("2211221408250000402");
            value.Add("2211221408250000502");
            value.Add("2211221408250000602");
            value.Add("2211221408250000702");
            value.Add("2211221408250000802");
            value.Add("2211221408250000902");
            value.Add("2211221408250001002");
            value.Add("2211221408250001102");
            value.Add("2211221408250001202");
            value.Add("2211221408250001302");
            value.Add("2211221408250001402");
            value.Add("2211221408250001502");
            value.Add("2211221408250001602");
            value.Add("2211221408250001702");
            value.Add("2211221408250001802");
            value.Add("2211221408250001902");
            value.Add("2211221408250002002");
            value.Add("2211221408250002102");
            value.Add("2211221408250002202");
            value.Add("2211221408250002302");
            value.Add("2211221408250002402");
            value.Add("2211221408250002502");
            value.Add("2211221408250002602");
            value.Add("2211221408250002702");
            value.Add("2211221408250002802");
            value.Add("2211221408250002902");
            value.Add("2211221408250003002");
            value.Add("2211221408250003102");
            value.Add("2211221408250003202");
            value.Add("2211221408250003302");
            value.Add("2211221408250003402");
            value.Add("2211221408250003502");
            value.Add("2211221408250003602");
            value.Add("2211221408250003702");
            value.Add("2211221408250003802");
            value.Add("2211221408250003902");
            value.Add("2211221408250004002");
            value.Add("2211221408250004102");
            value.Add("2211221408250004202");
            value.Add("2211221408250004302");
            value.Add("2211221408250004402");
            value.Add("2211221408250004502");
            value.Add("2211221408250004602");
            value.Add("2211221408250004702");
            value.Add("2211221408250004802");
            value.Add("2211221408250004902");
            value.Add("2211221408250005002");
            value.Add("2211221408250005102");
            value.Add("2211221408250005202");
            value.Add("2211221408250005302");
            value.Add("2211221408250005402");
            value.Add("2211221408250005502");
            value.Add("2211221408250005602");
            value.Add("2211221408250005702");
            value.Add("2211221408250005802");
            value.Add("2211221408250005902");
            value.Add("2211221408250006002");
            value.Add("2211221408250006102");
            value.Add("2211221408250006202");
            value.Add("2211221408250006302");
            value.Add("2211221408250006402");
            value.Add("2211221408250006502");
            value.Add("2211221408250006602");
            value.Add("2211221408250006702");
            value.Add("2211221408250006802");
            value.Add("2211221408250006902");
            value.Add("2211221408250007002");
            value.Add("2211221408250007102");
            value.Add("2211221408250007202");
            value.Add("2211221408250007302");
            value.Add("2211221408250007402");
            value.Add("2211221408250007502");
            value.Add("2211221408250007602");
            value.Add("2211221408250007702");
            value.Add("2211221408250007802");
            value.Add("2211221408250007902");
            value.Add("2211221408250008002");
            value.Add("2211221408250008102");
            value.Add("2211221408250008202");
            value.Add("2211221408250008302");
            value.Add("2211221408250008402");
            value.Add("2211221408250008502");
            value.Add("2211221408250008602");
            value.Add("2211221408250008702");
            value.Add("2211221408250008802");
            value.Add("2211221408250008902");
            value.Add("2211221408250009002");
            value.Add("2211221408250009102");
            value.Add("2211221408250009202");
            value.Add("2211221408250009302");
            value.Add("2211221408250009402");
            value.Add("2211221408250009502");
            value.Add("2211221408250009602");
            value.Add("2211221408250009702");
            value.Add("2211221408250009802");
            value.Add("2211221408250009902");
            value.Add("2211221408250010002");
            value.Add("2211221408250010102");
            value.Add("2211221408250010202");
            value.Add("2211221408250010302");
            value.Add("2211221408250010402");
            value.Add("2211221408250010502");
            value.Add("2211221408250010602");
            value.Add("2211221408250010702");
            value.Add("2211221408250010802");
            value.Add("2211221408250010902");
            value.Add("2211221408250011002");
            value.Add("2211221408250011102");
            value.Add("2211221408250011202");
            value.Add("2211221408250011302");
            value.Add("2211221408250011402");
            value.Add("2211221408250011502");
            value.Add("2211221408250011602");
            value.Add("2211221408250011702");
            value.Add("2211221408250011802");
            value.Add("2211221408250011902");
            value.Add("2211221408250012002");
            value.Add("2211221408250012102");
            value.Add("2211221408250012202");
            value.Add("2211221408250012302");
            value.Add("2211221408250012402");
            value.Add("2211221408250012502");
            value.Add("2211221408250012602");
            value.Add("2211221408250012702");
            value.Add("2211221408250012802");
            return value;
        }
        public List<string> GetReprotKey3()
        {
            List<string> value = new List<string>();
            value.Add("2211221408410000102");
            value.Add("2211221408410000202");
            value.Add("2211221408410000302");
            value.Add("2211221408410000402");
            value.Add("2211221408410000502");
            value.Add("2211221408410000602");
            value.Add("2211221408410000702");
            value.Add("2211221408410000802");
            value.Add("2211221408410000902");
            value.Add("2211221408410001002");
            value.Add("2211221408410001102");
            value.Add("2211221408410001202");
            value.Add("2211221408410001302");
            value.Add("2211221408410001402");
            value.Add("2211221408410001502");
            value.Add("2211221408410001602");
            value.Add("2211221408410001702");
            value.Add("2211221408410001802");
            value.Add("2211221408410001902");
            value.Add("2211221408410002002");
            value.Add("2211221408410002102");
            value.Add("2211221408410002202");
            value.Add("2211221408410002302");
            value.Add("2211221408410002402");
            value.Add("2211221408410002502");
            value.Add("2211221408410002602");
            value.Add("2211221408410002702");
            value.Add("2211221408410002802");
            value.Add("2211221408410002902");
            value.Add("2211221408410003002");
            value.Add("2211221408410003102");
            value.Add("2211221408410003202");
            value.Add("2211221408410003302");
            value.Add("2211221408410003402");
            value.Add("2211221408410003502");
            value.Add("2211221408410003602");
            value.Add("2211221408410003702");
            value.Add("2211221408410003802");
            value.Add("2211221408410003902");
            value.Add("2211221408410004002");
            value.Add("2211221408410004102");
            value.Add("2211221408410004202");
            value.Add("2211221408410004302");
            value.Add("2211221408410004402");
            value.Add("2211221408410004502");
            value.Add("2211221408410004602");
            value.Add("2211221408410004702");
            value.Add("2211221408410004802");
            value.Add("2211221408410004902");
            value.Add("2211221408410005002");
            value.Add("2211221408410005102");
            value.Add("2211221408410005202");
            value.Add("2211221408410005302");
            value.Add("2211221408410005402");
            value.Add("2211221408410005502");
            value.Add("2211221408410005602");
            value.Add("2211221408410005702");
            value.Add("2211221408410005802");
            value.Add("2211221408410005902");
            value.Add("2211221408410006002");
            value.Add("2211221408410006102");
            value.Add("2211221408410006202");
            value.Add("2211221408410006302");
            value.Add("2211221408410006402");
            value.Add("2211221408410006502");
            value.Add("2211221408410006602");
            value.Add("2211221408410006702");
            value.Add("2211221408410006802");
            value.Add("2211221408410006902");
            value.Add("2211221408410007002");
            value.Add("2211221408410007102");
            value.Add("2211221408410007202");
            value.Add("2211221408410007302");
            value.Add("2211221408410007402");
            value.Add("2211221408410007502");
            value.Add("2211221408410007602");
            value.Add("2211221408410007702");
            value.Add("2211221408410007802");
            value.Add("2211221408410007902");
            value.Add("2211221408410008002");
            value.Add("2211221408410008102");
            value.Add("2211221408410008202");
            value.Add("2211221408410008302");
            value.Add("2211221408410008402");
            value.Add("2211221408410008502");
            value.Add("2211221408410008602");
            value.Add("2211221408410008702");
            value.Add("2211221408410008802");
            return value;
        }
        public List<string> GetReprotKey4()
        {
            List<string> value = new List<string>();
            value.Add("2211221409000000102");
            value.Add("2211221409000000202");
            value.Add("2211221409000000302");
            value.Add("2211221409000000402");
            value.Add("2211221409000000502");
            value.Add("2211221409000000602");
            value.Add("2211221409000000702");
            value.Add("2211221409000000802");
            value.Add("2211221409000000902");
            value.Add("2211221409000001002");
            value.Add("2211221409000001102");
            value.Add("2211221409000001202");
            value.Add("2211221409000001302");
            value.Add("2211221409000001402");
            value.Add("2211221409000001502");
            value.Add("2211221409000001602");
            value.Add("2211221409000001702");
            value.Add("2211221409000001802");
            value.Add("2211221409000001902");
            value.Add("2211221409000002002");
            value.Add("2211221409000002102");
            value.Add("2211221409000002202");
            value.Add("2211221409000002302");
            value.Add("2211221409000002402");
            value.Add("2211221409000002502");
            value.Add("2211221409000002602");
            value.Add("2211221409000002702");
            value.Add("2211221409000002802");
            value.Add("2211221409000002902");
            value.Add("2211221409000003002");
            value.Add("2211221409000003102");
            value.Add("2211221409000003202");
            value.Add("2211221409000003302");
            value.Add("2211221409000003402");
            value.Add("2211221409000003502");
            value.Add("2211221409000003602");
            value.Add("2211221409000003702");
            value.Add("2211221409000003802");
            value.Add("2211221409000003902");
            value.Add("2211221409000004002");
            value.Add("2211221409000004102");
            value.Add("2211221409000004202");
            value.Add("2211221409000004302");
            value.Add("2211221409000004402");
            value.Add("2211221409000004502");
            value.Add("2211221409000004602");
            value.Add("2211221409000004702");
            value.Add("2211221409000004802");
            value.Add("2211221409000004902");
            value.Add("2211221409000005002");
            value.Add("2211221409000005102");
            value.Add("2211221409000005202");
            value.Add("2211221409000005302");
            value.Add("2211221409000005402");
            value.Add("2211221409000005502");
            value.Add("2211221409000005602");
            value.Add("2211221409000005702");
            value.Add("2211221409000005802");
            value.Add("2211221409000005902");
            value.Add("2211221409000006002");
            value.Add("2211221409000006102");
            value.Add("2211221409000006202");
            value.Add("2211221409000006302");
            value.Add("2211221409000006402");
            value.Add("2211221409000006502");
            value.Add("2211221409000006602");
            value.Add("2211221409000006702");
            value.Add("2211221409000006802");
            value.Add("2211221409000006902");
            value.Add("2211221409000007002");
            value.Add("2211221409000007102");
            value.Add("2211221409000007202");
            value.Add("2211221409000007302");
            value.Add("2211221409000007402");
            value.Add("2211221409000007502");
            value.Add("2211221409000007602");
            value.Add("2211221409000007702");
            value.Add("2211221409000007802");
            value.Add("2211221409000007902");
            value.Add("2211221409000008002");
            return value;
        }
        public List<string> GetReprotKey5()
        {
            List<string> value = new List<string>();
            value.Add("2201181112550000012");
            value.Add("2201181112550000022");
            value.Add("2201181112550000032");
            value.Add("2201181112550000042");
            value.Add("2201181112550000052");
            value.Add("2201181112550000062");
            value.Add("2201181112550000072");
            return value;
        }
        public List<string> GetReprotKey6()
        {
            List<string> value = new List<string>();
            value.Add("2211221409000008202");
            value.Add("2211221409000008302");
            value.Add("2211221409000008402");
            value.Add("2211221409000008502");
            value.Add("2211221409000008602");
            value.Add("2211221409000008702");
            value.Add("2211221409000008802");
            value.Add("2211221409000008902");
            value.Add("2211221409000009002");
            value.Add("2211221409000009102");
            value.Add("2211221409000009202");
            value.Add("2211221409000009302");
            value.Add("2211221409000009402");
            value.Add("2211221409000009502");
            value.Add("2211221409000009602");
            value.Add("2211221409000009702");
            value.Add("2211221409000009802");
            value.Add("2211221409000009902");
            value.Add("2211221409000010002");
            value.Add("2211221409000010102");
            value.Add("2211221409000010202");
            value.Add("2211221409000010302");
            value.Add("2211221409000010402");
            value.Add("2211221409000010502");
            value.Add("2211221409000010602");
            value.Add("2211221409000010702");
            value.Add("2211221409000010802");
            value.Add("2211221409000010902");
            value.Add("2211221409000011002");
            value.Add("2211221409000011102");
            value.Add("2211221409000011202");
            value.Add("2211221409000011302");
            value.Add("2211221409000011402");
            value.Add("2211221409000011502");
            value.Add("2211221409000011602");
            value.Add("2211221409000011702");
            value.Add("2211221409000011802");
            value.Add("2211221409000011902");
            value.Add("2211221409000012002");
            value.Add("2211221409000012102");
            return value;
        }
        public bool IsContainsMethod(string formular, List<string > KeyList)
        {
            bool iscontains= false;
            foreach (var key in KeyList)
            {
                if (formular.Contains(key))
                {
                    iscontains = true;
                    break;
                }
            }
            return iscontains;
        }
    }
    public class DictinonaryModel
    {

        public DictinonaryModel(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
