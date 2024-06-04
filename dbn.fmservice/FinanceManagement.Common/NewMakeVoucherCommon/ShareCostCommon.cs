using Architecture.Common.HttpClientUtil;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http.Headers;

namespace FinanceManagement.Common.NewMakeVoucherCommon
{
    /// <summary>
    /// 费用分摊明细
    /// </summary>
    public class ShareCostCommon : MakeVoucherBase
    {

        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        public ShareCostCommon(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration) : base(httpClientUtil, hostCongfiguration)
        {
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil = httpClientUtil;
        }
        public override FD_SettleReceipt getVoucherList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, AuthenticationHeaderValue token)
        {
            #region 构造销售汇总表查询条件  别问我为什么。产品非得要
            List<RptSearchModel> rpts = new List<RptSearchModel>() {
             new RptSearchModel(){Text="查询时间",Value= request.Begindate+"&"+request.Enddate},
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
            List<FD_SettleReceiptDetailCommand> Lines = new List<FD_SettleReceiptDetailCommand>();
            List<DictionaryModel> expenseNatures = new List<DictionaryModel>();
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
                        var groupbypersondatas = shareCostByFormula.GroupBy(s => new { s.DeptOrOthersID,s.DeptExtendID });
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
                            detail.MarketID = item.IsMarket ?group.Key.DeptExtendID : "0";
                            Lines.Add(detail);
                        }
                    }
                    if (item.IsMarket&& !item.IsPerson)
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
                isBreak = false;
            }
            string natureStr = string.Empty;
            if (expenseNatures.Count > 0)
            {
                natureStr = JsonConvert.SerializeObject(expenseNatures.Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "费用性质", Value = string.Join(',', expenseNatures.Select(s => s.name)) });
            }
            domain.ProductLst = natureStr;
            rpts.Add(new RptSearchModel() { Text = "汇总方式", Value = "" });
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
                            data= result.Data.Where(s => costWhere.Contains(s.ExpenseNature)).ToList();
                        }
                        else
                        {
                            data= result.Data;
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

    }



}
