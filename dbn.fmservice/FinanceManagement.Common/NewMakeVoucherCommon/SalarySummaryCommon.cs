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
    public class SalarySummaryCommon : MakeVoucherBase
    {

        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        public SalarySummaryCommon(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration) : base(httpClientUtil, hostCongfiguration)
        {
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil = httpClientUtil;
        }
        public override FD_SettleReceipt getVoucherList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, AuthenticationHeaderValue token)
        {
            #region 构造销售汇总表查询条件  别问我为什么。产品非得要
            List<RptSearchModel> rpts = new List<RptSearchModel>() {
             new RptSearchModel(){Text="查询时间",Value= Convert.ToDateTime( request.Enddate).ToString("yyyy-MM")},
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
            List<DictionaryModel> persons = new List<DictionaryModel>();
            List<DictionaryModel> markets = new List<DictionaryModel>();
            reqUrl = _hostCongfiguration.DBN_HrServiceHost + "/api/HrSalaryInfo/getSalaryInfo";
            //获取薪资项
            List<SalarySetItem> salarySetItems = GetSalarySetItemList(request, token);
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

                List<SalaryResultData> salartResult = GetSalarySummaryDataList(request, item, token);//获取薪资表数据
                salartResult?.ForEach(summary =>
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
                    if (!string.IsNullOrEmpty(formularRep))
                    {
                        foreach (var dict in salarySetItems)
                        {
                            if (formularRep.Contains("["+dict.SetItemId+"]"))
                            {
                                var value = summary.SetItems.Where(s => (s.SetItemId) == dict.SetItemId).Sum(s => s.Amount).ToString();
                                formularRep = formularRep.Replace("[" + dict.SetItemId + "]", value);
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
                    }
                    if (item.IsPerson) detail.PersonID = summary.PersonId;
                    if (item.IsMarket) detail.MarketID = summary.MarketId;
                    if (item.IsSum) detail.EnterpriseID = summary.PaymentEnterpriseId;
                    Lines.Add(detail);
                });
                #region 封装钻取销售汇总表接口的必备参数
                if (item.Extends?.Count > 0)
                {
                    var personList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.人员).Select(s => s).ToList();
                    personList.ForEach(s =>
                    {
                        if (persons.FindIndex(_ => _.id == s.Object) < 0)
                        {
                            persons.Add(new DictionaryModel()
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
                }
                #endregion
                if (isBreak)
                {
                    if (item.IsPerson) domain.SummaryTypeName = "人员";
                    if (item.IsMarket) domain.SummaryTypeName = "部门";
                    if (item.IsSum) domain.SummaryTypeName = "单位";
                }
                isBreak = false;
            }

            string personStr = string.Empty;
            string marketStr = string.Empty;
            if (persons.Count > 0)//人员筛选条件
            {
                personStr = JsonConvert.SerializeObject(persons.Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "人员", Value = string.Join(',', persons.Select(s => s.name)) });
            }
            if (markets.Count > 0)//部门
            {
                marketStr = string.Join(',', markets.Select(s => s.id).Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "部门", Value = string.Join(',', markets.Select(s => s.name)) });
            }
            domain.ProductLst = personStr + "~" + marketStr;
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
        public List<SalarySetItem> GetSalarySetItemList(FM_CarryForwardVoucherSearchCommand request, AuthenticationHeaderValue token)
        {
            try
            {
                string url = $"{_hostCongfiguration.DBN_HrServiceHost}/api/HrSalaryInfo/getSalarySetItem";
                var res = _httpClientUtil.PostJsonAsync<ResultModel<SalarySetItem>>(url, new { EnterpriseId = request.EnterpriseID }, (a) => { a.Authorization = token; }).Result;
                if (res.Code == 0)
                {
                    res.Data.ForEach(s =>
                    {
                        s.SetItemId =  s.SetItemId;
                    });
                    return res.Data;
                }
                else
                {
                    return new List<SalarySetItem>();
                }
            }
            catch (Exception ex)
            {
                return new List<SalarySetItem>();
            }
        }
        private List<SalaryResultData> GetSalarySummaryDataList(FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, AuthenticationHeaderValue token)
        {
            try
            {
                #region 构造接口参数
                SalarySearchModel salaryRequest = new SalarySearchModel()
                {
                    EnterpriseId = request.EnterpriseID,
                    Date = Convert.ToDateTime(request.Enddate).ToString("yyyy-MM"),
                    MarketIds = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.部门).Select(s => s.Object).ToList(),
                    PersonIds = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.人员).Select(s => s.Object).ToList(),
                };
                var result = _httpClientUtil.PostJsonAsync<ResultModel<SalaryResultData>>(base.reqUrl, salaryRequest, (a) => { a.Authorization = token; }).Result;
                if (result.Data?.Count==0)
                {
                    return new List<SalaryResultData>();
                }
                if (item.IsPerson)
                {
                    return result.Data;
                }
                if (item.IsMarket)
                {
                    List<SalaryResultData> groupSalary = new List<SalaryResultData>(); 
                    List<SalaryResultData> salaries = result.Data;
                    foreach (var person in salaries.GroupBy(s => new { s.MarketId, s.MarketName }))
                    {
                        List<SalarySetItem> salaryItemAll = new List<SalarySetItem>();
                        var temp = salaries.Where(s => s.MarketId == person.Key.MarketId).ToList();
                        temp.ForEach(s =>
                        {
                            salaryItemAll.AddRange(s.SetItems);
                        });
                        person.FirstOrDefault().SetItems = salaryItemAll.GroupBy(s => new { s.SetItemId ,s.SetItemName})
                            .Select(d => new SalarySetItem()
                            {
                                SetItemId = d.Key.SetItemId,
                                SetItemName = d.Key.SetItemName,
                                NumericalOrder = d.First().NumericalOrder,
                                PersonId = d.First().PersonId,
                                Amount = d.Sum(s => s.Amount)
                            }).ToList();
                        groupSalary.Add(person.FirstOrDefault());
                    }
                    return groupSalary;
                }
                if (item.IsSum)
                {
                    List<SalarySetItem> salaryItemAll = new List<SalarySetItem>();
                    List<SalaryResultData> salaries = result.Data;
                    foreach (var person in salaries)
                    {
                        salaryItemAll.AddRange(person.SetItems);
                    }
                    salaries.FirstOrDefault().SetItems= salaryItemAll.GroupBy(s => new { s.SetItemId, s.SetItemName })
                            .Select(d => new SalarySetItem()
                            {
                                SetItemId = d.Key.SetItemId,
                                SetItemName = d.Key.SetItemName,
                                NumericalOrder = d.First().NumericalOrder,
                                PersonId = d.First().PersonId,
                                Amount = d.Sum(s => s.Amount)
                            }).ToList();
                    return salaries;
                }
                #endregion
                return result.Code == 0 ? result.Data : new List<SalaryResultData>();
            }
            catch (Exception ex)
            {
                return new List<SalaryResultData>();
            }
        }

    }
    public class SalarySetItem
    {
        public string SetItemId { get; set; }
        public string SetItemName { get; set; }
        public string NumericalOrder { get; set; }
        public string PersonId { get; set; }
        public decimal Amount { get; set; }
    }
    public class SalarySearchModel
    {
        public string EnterpriseId { get; set; }
        public string Date { get; set; }
        public List<string> MarketIds { get; set; }
        public List<string> PersonIds { get; set; }
    }
    public class SalaryResultData
    {
        public int RecordId { get; set; }
        public string NumericalOrder { get; set; }
        public DateTime Month { get; set; }
        public string PersonId { get; set; }
        public string PersonName { get; set; }
        public string PaymentEnterpriseId { get; set; }
        public string PaymentEnterpriseName { get; set; }
        public string MarketId { get; set; }
        public string MarketName { get; set; }
        public string JobId { get; set; }
        public string JobName { get; set; }
        public string PositionId { get; set; }
        public string PositionName { get; set; }
        public string PositiontypeId { get; set; }
        public string PositiontypeName { get; set; }
        public string RankId { get; set; }
        public string RankName { get; set; }
        public string GradeId { get; set; }
        public string GradeName { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal TaxExemptIncome { get; set; }
        public decimal Taxes { get; set; }
        public decimal RealWages { get; set; }
        public List<SalarySetItem> SetItems { get; set; }
    }

}
