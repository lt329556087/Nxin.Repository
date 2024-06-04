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
    public class WelfareDBNSummaryCommon : MakeVoucherBase
    {

        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        public WelfareDBNSummaryCommon(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration) : base(httpClientUtil, hostCongfiguration)
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
            reqUrl = _hostCongfiguration.DBN_HrServiceHost + "/api/HrWelfCostshare/getWelfInfo";
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
                List<WelfareDBNResultData> salartResult = GetWelfareSummaryDataList(request, item, token);//获取薪资表数据
                salartResult?.ForEach(summary =>
                {
                    FD_SettleReceiptDetailCommand detail = base.NewObject<WelfareDBNResultData>(summary, item, isDebit, formular, dictList);
                    if (item.IsPerson) detail.PersonID = summary.PersonId;
                    if (item.IsMarket) detail.MarketID = summary.MarketId;
                    if (item.IsSum) detail.EnterpriseID = request.EnterpriseID;
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
        private List<WelfareDBNResultData> GetWelfareSummaryDataList(FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, AuthenticationHeaderValue token)
        {
            try
            {
                //文档地址https://confluence.nxin.com/pages/viewpage.action?pageId=65049212
                #region 构造接口参数
                WelfareDBNSearchModel salaryRequest = new WelfareDBNSearchModel()
                {
                    EnterpriseId = request.EnterpriseID,
                    Date = Convert.ToDateTime(request.Enddate).ToString("yyyy-MM"),
                    MarketIds = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.部门).Select(s => s.Object).ToList(),
                    PersonIds = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.人员).Select(s => s.Object).ToList(),
                };
                var result = _httpClientUtil.PostJsonAsync<ResultModel<WelfareDBNResultData>>(base.reqUrl, salaryRequest, (a) => { a.Authorization = token; }).Result;
                if (result.Data?.Count == 0)
                {
                    return new List<WelfareDBNResultData>();
                }
                if (item.IsPerson)
                {
                    return result.Data;
                }
                if (item.IsMarket)
                {
                    List<WelfareDBNResultData> welfares = result.Data;
                    var list = welfares.GroupBy(s => s.MarketId)
                        .Select(d => new WelfareDBNResultData()
                        {
                            MarketId = d.Key,
                            PensionBusine = d.Sum(d => d.PensionBusine),
                            MedicalBusine = d.Sum(d => d.MedicalBusine),
                            EmploymentBusine = d.Sum(d => d.EmploymentBusine),
                            IndustrialBusine = d.Sum(d => d.IndustrialBusine),
                            MaternityBusine = d.Sum(d => d.MaternityBusine),
                            CommercialBusine = d.Sum(d => d.CommercialBusine),
                            HousingBusine = d.Sum(d => d.HousingBusine),
                            CostBusine = d.Sum(d => d.CostBusine),
                            SuppHousingBusineBase = d.Sum(d => d.SuppHousingBusineBase),
                            SuppPensionBusineBase = d.Sum(d => d.SuppPensionBusineBase),
                            SuppMedicalBusineBase = d.Sum(d => d.SuppMedicalBusineBase),
                        }).ToList();
                    return list;
                }
                if (item.IsSum)
                {
                    List<WelfareDBNResultData> welfares = result.Data;
                    List<WelfareDBNResultData> list = new List<WelfareDBNResultData>()
                    {
                        new WelfareDBNResultData()
                        {
                            PensionBusine =welfares.Sum(d=>d.PensionBusine),
                            MedicalBusine = welfares.Sum(d => d.MedicalBusine),
                            EmploymentBusine = welfares.Sum(d => d.EmploymentBusine),
                            IndustrialBusine = welfares.Sum(d => d.IndustrialBusine),
                            MaternityBusine = welfares.Sum(d => d.MaternityBusine),
                            CommercialBusine = welfares.Sum(d => d.CommercialBusine),
                            HousingBusine = welfares.Sum(d => d.HousingBusine),
                            CostBusine = welfares.Sum(d => d.CostBusine),
                            SuppHousingBusineBase = welfares.Sum(d => d.SuppHousingBusineBase),
                            SuppPensionBusineBase = welfares.Sum(d => d.SuppPensionBusineBase),
                            SuppMedicalBusineBase = welfares.Sum(d => d.SuppMedicalBusineBase),
                        }
                    };
                    return list;
                }
                #endregion
                return result.Code == 0 ? result.Data : new List<WelfareDBNResultData>();
            }
            catch (Exception ex)
            {
                return new List<WelfareDBNResultData>();
            }
        }
        public Dictionary<string, string> GetDict()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("2111261612570000101", "PensionBusine");//养老保险企业金额         
            value.Add("2111261612570000103", "MedicalBusine");// 医疗保险企业金额         
            value.Add("2111261612570000105", "EmploymentBusine");// 失业保险企业金额         
            value.Add("2111261612570000107", "IndustrialBusine");// 工伤保险企业金额         
            value.Add("2111261612570000108", "MaternityBusine");// 生育保险企业金额         
            value.Add("2111261612570000109", "CommercialBusine");// 商业保险企业金额         
            value.Add("2111261612570000111", "HousingBusine");// 住房公积金企业金额         
            value.Add("2111261612570000113", "CostBusine");// 其他费用公司         
            value.Add("2111261612570000115", "SuppHousingBusineBase");// 补充公积金企业         
            value.Add("2111261612570000117", "SuppPensionBusineBase");// 补充养老企业         
            value.Add("2111261612570000119", "SuppMedicalBusineBase");// 补充大病医疗企业         
            return value;
        }
    }
    public class WelfareDBNSearchModel
    {
        public string EnterpriseId { get; set; }
        public string Date { get; set; }
        public List<string> MarketIds { get; set; }
        public List<string> PersonIds { get; set; }
    }
    public class WelfareDBNResultData
    {
        public int RecordId { get; set; }
        public DateTime DataDate { get; set; }
        public string CreatedName { get; set; }
        public string ModifyName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int CheckMark { get; set; }
        public string Remarks { get; set; }
        public string CreateEnterId { get; set; }
        public string NumericalOrder { get; set; }
        public string PersonId { get; set; }
        public string EnterpriseId { get; set; }
        public string PersonName { get; set; }
        public string MarketId { get; set; }
        public string MarketName { get; set; }
        public string EnterpriseName { get; set; }
        public decimal Days { get; set; }
        public decimal PensionBusine { get; set; }
        public decimal MedicalBusine { get; set; }
        public decimal EmploymentBusine { get; set; }
        public decimal IndustrialBusine { get; set; }
        public decimal MaternityBusine { get; set; }
        public decimal CommercialBusine { get; set; }
        public decimal HousingBusine { get; set; }
        public decimal CostBusine { get; set; }
        public decimal SuppHousingBusineBase { get; set; }
        public decimal SuppPensionBusineBase { get; set; }
        public decimal SuppMedicalBusineBase { get; set; }
    }

}
