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
    public class SalaryDBNSummaryCommon : MakeVoucherBase
    {

        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        public SalaryDBNSummaryCommon(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration) : base(httpClientUtil, hostCongfiguration)
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
            reqUrl = _hostCongfiguration.DBN_HrServiceHost + "/api/SalaryCostshare/getSalaryInfo";
            //获取福利项
            Dictionary<string, string> dictList = GetDict();
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
                List<SalaryDBNResultData> salartResult = GetSalarySummaryDataList(request, item, token);//获取薪资表数据
                salartResult?.ForEach(summary =>
                {
                    FD_SettleReceiptDetailCommand detail = base.NewObject<SalaryDBNResultData>(summary, item, isDebit, formular, dictList);
                    if (item.IsPerson) detail.PersonID = summary.PersonId;
                    if (item.IsMarket) detail.MarketID = summary.MarketId;
                    if (item.IsSum) detail.EnterpriseID = request.EnterpriseID;
                    Lines.Add(detail);
                });
                #region 封装钻取接口的必备参数
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
        private List<SalaryDBNResultData> GetSalarySummaryDataList(FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, AuthenticationHeaderValue token)
        {
            try
            {
                //文档地址https://confluence.nxin.com/pages/viewpage.action?pageId=65049396
                #region 构造接口参数
                SalaryDBNSearchModel salaryRequest = new SalaryDBNSearchModel()
                {
                    EnterpriseId = request.EnterpriseID,
                    Date = Convert.ToDateTime(request.Enddate).ToString("yyyy-MM"),
                    MarketIds = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.部门).Select(s => s.Object).ToList(),
                    PersonIds = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.人员).Select(s => s.Object).ToList(),
                };
                var result = _httpClientUtil.PostJsonAsync<ResultModel<SalaryDBNResultData>>(base.reqUrl, salaryRequest, (a) => { a.Authorization = token; }).Result;
                if (result.Data?.Count == 0)
                {
                    return new List<SalaryDBNResultData>();
                }
                if (item.IsPerson)
                {
                    return result.Data;
                }
                if (item.IsMarket)
                {
                    List<SalaryDBNResultData> Salarys = result.Data;
                    var list = Salarys.GroupBy(s => s.MarketId)
                        .Select(d => new SalaryDBNResultData()
                        {
                            MarketId = d.Key,
                            MonthlyIncome = d.Sum(d => d.MonthlyIncome),
                            PerformanceWage = d.Sum(d => d.PerformanceWage),
                        }).ToList();
                    return list;
                }
                if (item.IsSum)
                {
                    List<SalaryDBNResultData> Salarys = result.Data;
                    List<SalaryDBNResultData> list = new List<SalaryDBNResultData>()
                    {
                        new SalaryDBNResultData()
                        {
                            MonthlyIncome =Salarys.Sum(d=>d.MonthlyIncome),
                            PerformanceWage = Salarys.Sum(d => d.PerformanceWage),
                        }
                    };
                    return list;
                }
                #endregion
                return result.Code == 0 ? result.Data : new List<SalaryDBNResultData>();
            }
            catch (Exception ex)
            {
                return new List<SalaryDBNResultData>();
            }
        }
        public Dictionary<string, string> GetDict()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("2111261612570000166", "MonthlyIncome");//月收入         
            value.Add("2111261612570000188", "PerformanceWage");// 绩效工资                
            return value;
        }
    }
    public class SalaryDBNSearchModel
    {
        public string EnterpriseId { get; set; }
        public string Date { get; set; }
        public List<string> MarketIds { get; set; }
        public List<string> PersonIds { get; set; }
    }
    public class SalaryDBNResultData
    {
        /// <summary>
        /// 
        /// </summary>
        public int RecordId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 宋玉
        /// </summary>
        public string CreatedName { get; set; }
        /// <summary>
        /// 王森
        /// </summary>
        public string ModifyName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreatedDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ModifiedDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CheckMark { get; set; }
        /// <summary>
        /// 大概多返岗山东分公司大丰港是的发
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreateEnterId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PersonId { get; set; }
        /// <summary>
        /// 宋玉
        /// </summary>
        public string PersonName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MarketId { get; set; }
        /// <summary>
        /// 前台开发部
        /// </summary>
        public string MarketName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EnterpriseId { get; set; }
        /// <summary>
        /// 大北农养猪辽宁分公司
        /// </summary>
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Days { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MonthlyIncome { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal PerformanceWage { get; set; }
    }

}
