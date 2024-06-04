using Architecture.Common.HttpClientUtil;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FinanceManagement.Common.MonthEndCheckout;

namespace FinanceManagement.Common.MakeVoucherCommon
{
    public class SaleSummaryCommon : MakeVoucherBase
    {

        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        public SaleSummaryCommon(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration) : base(httpClientUtil, hostCongfiguration)
        {
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil = httpClientUtil;
        }
        public override FD_SettleReceipt getVoucherList(FM_CarryForwardVoucherODataEntity model, FM_CarryForwardVoucherInterfaceSearchCommand request)
        {
            #region 构造销售汇总表查询条件  别问我为什么。产品非得要
            List<RptSearchModel> rpts = new List<RptSearchModel>() {
             new RptSearchModel(){Text="查询时间",Value=Convert.ToDateTime(request.DataDate).ToString("yyyy-MM")+"-"+Convert.ToDateTime(request.DataDate).ToString("yyyy-MM")},
             new RptSearchModel(){Text="单位筛选",Value=model.EnterpriseName},
             new RptSearchModel(){Text="销售转出",Value=model.TransferAccountsAbstractName},
             new RptSearchModel(){Text="计量单位",Value="本地计量"},
             new RptSearchModel(){Text="销售类型",Value="销售"},
             new RptSearchModel(){Text="数据来源",Value="结帐数据"}
            };
            #endregion
            FD_SettleReceipt domain = new FD_SettleReceipt()
            {
                SettleReceipType = "201610220104402203",
                TicketedPointID = model.TicketedPointID,
                TransBeginDate = request.DataDate + "-01",
                TransEndDate = Convert.ToDateTime(request.DataDate + "-01").AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd")
            };
            List<FD_SettleReceiptDetailCommand> Lines = new List<FD_SettleReceiptDetailCommand>();
            List<DictionaryModel> products = new List<DictionaryModel>();
            reqUrl = _hostCongfiguration.ReportService + "/api/SaleSummary/GetSummaryAsync";
            List<SummaryType> summaryTypes = GetSummaryList(request);
            Dictionary<string, string> dictList = GetDict();
            bool isBreak = true;
            foreach (var item in model.Lines)
            {
                List<SaleSummaryResultData> saleSummarys = GetSaleSummaryDataList(model, request, item, summaryTypes, domain, isBreak);//获取销售汇总表数据
                foreach (var sale in saleSummarys)
                {
                    string str = $@"含税销售收入:{sale.SalesIncomeWithinTax},现场折扣:{sale.PresentDiscount},抹零:{sale.AmountAdjust},月度折扣:{sale.MonthlyDiscount},年度折扣:{sale.YearlyDiscount},含税销售净额:{sale.SalesIncomeWithoutTax},不含税销售净额:{sale.SalesAmountNet}";
                }
                bool isDebit = false;
                string formular = string.Empty;
                if (!string.IsNullOrEmpty(item.DebitSecFormula) && item.DebitSecFormula != "0") { isDebit = true; formular = item.DebitSecFormula; }
                if (!string.IsNullOrEmpty(item.CreditSecFormula) && item.CreditSecFormula != "0") { isDebit = false; formular = item.CreditSecFormula; }
                saleSummarys?.ForEach(summary =>
                {
                    int index = 0;
                    FD_SettleReceiptDetailCommand detail = base.NewObject<SaleSummaryResultData>(summary, item, isDebit, formular, dictList);
                    string[] summarys = summary.SummaryType.Split('~');
                    if (item.IsPerson) detail.PersonID = summarys[index++];
                    if (item.IsCustomer) detail.CustomerID = summarys[index++];
                    if (item.IsMarket) detail.MarketID = summarys[index++];
                    if (item.IsProduct) detail.ProductID = summarys[index++];
                    Lines.Add(detail);
                });
                #region 包装钻取销售汇总表接口的必备参数
                if (item.Extends?.Count > 0)
                {
                    item.Extends?.ForEach(extend =>
                    {
                        switch (extend.Sort)
                        {
                            case (int)SortTypeEnum.商品分类:
                                if (products.FindIndex(s => s.id == extend.Object) < 0)
                                {
                                    products.Add(new DictionaryModel()
                                    {
                                        id = extend.Object,
                                        name = extend.ObjectName,
                                        rank = 2
                                    });
                                }
                                break;
                            default:
                                break;
                        }
                    });
                }
                #endregion
                isBreak = false;
            }
            if (products.Count > 0)//筛选条件
            {
                domain.ProductLst = JsonConvert.SerializeObject(products.Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "商品类别", Value = string.Join(',', products.Select(s => s.name)) });
            }
            rpts.Add(new RptSearchModel() { Text = "汇总方式", Value = domain.SummaryTypeName });
            domain.RptSearchText = JsonConvert.SerializeObject(rpts);
            domain.Lines = Lines;
            return domain;
        }
        public List<SummaryType> GetSummaryList(FM_CarryForwardVoucherInterfaceSearchCommand request)
        {
            try
            {
                string url = $"{_hostCongfiguration.ReportService}/api/SaleSummary/GetSummaryTypeList";
                SaleSummaryRequest salerequest = new SaleSummaryRequest()
                {
                    GroupId = Convert.ToInt64(request.GroupID),
                    MenuParttern = 0,
                    EnteID = request.EnterpriseID,
                    EnterpriseId = Convert.ToInt64(request.EnterpriseID),
                    QiDian = 0,
                };
                var res = _httpClientUtil.PostJsonAsync<ResultModel<SummaryType>>(url, salerequest).Result;
                return res.Code == 0 ? res.Data : new List<SummaryType>();
            }
            catch (Exception ex)
            {

                return new List<SummaryType>();
            }
        }
        private List<SaleSummaryResultData> GetSaleSummaryDataList(FM_CarryForwardVoucherODataEntity model, FM_CarryForwardVoucherInterfaceSearchCommand request, FM_CarryForwardVoucherDetailODataEntity item, List<SummaryType> summaryTypes, FD_SettleReceipt domain, bool isBreak)
        {
            try
            {
                string SummaryType = string.Empty;
                string SummaryTypeName = string.Empty;
                #region 构造接口参数
                SaleSummaryRequest salerequest = new SaleSummaryRequest()
                {
                    BeginDate = request.DataDate + "-01",
                    EndDate = Convert.ToDateTime(request.DataDate + "-01").AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"),
                    SalesAbstract = model.TransferAccountsAbstract,
                    OnlyCombineEnte = false,
                    MeasurementUnit = "0",
                    IsGift = "0",
                    TicketedPoint = null,//销售汇总表结账接口始终为null
                    ReceiptType = null,//销售汇总表结账接口始终为null
                    QiDian = 0,
                    APPID = 1612062114270000101,
                    IsPig = false,
                    DataSource = 1,
                    GroupId = Convert.ToInt64(request.GroupID),
                    EnteID = request.EnterpriseID,
                    EnterpriseId = Convert.ToInt64(request.EnterpriseID),
                    MenuParttern = 0,
                    BoId = Convert.ToInt64(request.Boid),
                    OwnEntes = new List<string>() { request.EnterpriseID }
                };
                List<DictionaryModel> products = new List<DictionaryModel>();
                if (item.Extends?.Count > 0)
                {
                    item.Extends?.ForEach(extend =>
                    {
                        switch (extend.Sort)
                        {
                            case (int)SortTypeEnum.商品分类:
                                products.Add(new DictionaryModel()
                                {
                                    id = extend.Object,
                                    rank = 2
                                });
                                break;
                            default:
                                break;
                        }
                    });
                    salerequest.ProductLst = JsonConvert.SerializeObject(products);
                }
                if (item.IsPerson)
                {
                    SummaryType += summaryTypes.Find(s => s.SN == "业务员")?.SV + "=";
                    SummaryTypeName += summaryTypes.Find(s => s.SN == "业务员")?.SN + "=";
                }
                if (item.IsCustomer)
                {
                    SummaryType += summaryTypes.Find(s => s.SN == "客户" && s.PID != null)?.SV + "=";
                    SummaryTypeName += summaryTypes.Find(s => s.SN == "客户" && s.PID != null)?.SN + "=";
                }
                if (item.IsMarket)
                {
                    if (model.TransferAccountsSort == "201707270104402402")
                    {
                        SummaryType += summaryTypes.Find(s => s.SN == "部门一级")?.SV + "=";
                        SummaryTypeName += summaryTypes.Find(s => s.SN == "部门一级")?.SN + "=";
                    }
                    else if (model.TransferAccountsSort == "201707270104402403")
                    {
                        SummaryType += summaryTypes.Find(s => s.SN == "部门二级")?.SV + "=";
                        SummaryTypeName += summaryTypes.Find(s => s.SN == "部门二级")?.SN + "=";
                    }
                    else if (model.TransferAccountsSort == "201707270104402404")
                    {
                        SummaryType += summaryTypes.Find(s => s.SN == "部门三级")?.SV + "=";
                        SummaryTypeName += summaryTypes.Find(s => s.SN == "部门三级")?.SN + "=";
                    }
                    else
                    {
                        SummaryType += summaryTypes.Where(s => s.SN.Contains("部门") && s.SN.Contains("级")).OrderByDescending(s => s.Order)?.FirstOrDefault().SV + "=";
                        SummaryTypeName += summaryTypes.Where(s => s.SN.Contains("部门") && s.SN.Contains("级")).OrderByDescending(s => s.Order)?.FirstOrDefault().SN + "=";
                    }
                }
                if (item.IsProduct)
                {
                    SummaryType += summaryTypes.Find(s => s.SN == "商品代号")?.SV + "=";
                    SummaryTypeName += summaryTypes.Find(s => s.SN == "商品代号")?.SN + "=";
                }
                if (!item.IsPerson && !item.IsMarket && !item.IsCustomer && !item.IsProduct)
                {
                    SummaryType += summaryTypes.Find(s => s.SN == "单位" && s.PID != null)?.SV + "=";
                    SummaryTypeName += summaryTypes.Find(s => s.SN == "单位" && s.PID != null)?.SN + "=";
                }
                salerequest.SummaryType = !string.IsNullOrEmpty(SummaryType) ? SummaryType?.TrimEnd('=') : "";
                salerequest.SummaryTypeName = !string.IsNullOrEmpty(SummaryTypeName) ? SummaryTypeName?.TrimEnd('=') : "";
                #endregion
                if (isBreak)
                {
                    domain.SummaryType = salerequest.SummaryType;
                    domain.SummaryTypeName = salerequest.SummaryTypeName;
                }
                ResultModel<SaleSummaryResultData> result = base.postActionByUrl<ResultModel<SaleSummaryResultData>, SaleSummaryRequest>(salerequest);
                return result.Code == 0 ? result.Data : new List<SaleSummaryResultData>();
            }
            catch (Exception ex)
            {
                return new List<SaleSummaryResultData>();
            }
        }
        public Dictionary<string, string> GetDict()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("1911081429200000201", "SalesIncomeWithinTax");
            value.Add("1911081429200000202", "PresentDiscount");
            value.Add("1911081429200000203", "AmountAdjust");
            value.Add("1911081429200000204", "MonthlyDiscount");
            value.Add("1911081429200000205", "YearlyDiscount");
            value.Add("1911081429200000206", "SalesIncomeWithoutTax");
            value.Add("1911081429200000207", "SalesAmountNet");
            return value;
        }
    }
    public class SummaryType
    {
        public string SN { get; set; }
        public string SV { get; set; }
        public bool DV { get; set; }
        public string PID { get; set; }
        public int DT { get; set; }
        public int DB { get; set; }
        public int Order { get; set; }
    }
    public class DictionaryModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public int rank { get; set; }
    }
    public class SaleSummaryResultData: EntitySubClass
    {
        public decimal SalesAmount { get; set; }  //销售数量
        public decimal SalesAvgPrice { get; set; } //销售均价
        public decimal SalesIncomeWithinTax { get; set; } // 含税销售收入
        public decimal PresentDiscount { get; set; }   //现场折扣
        public decimal MonthlyDiscount { get; set; } //月度折扣
        public decimal YearlyDiscount { get; set; }   //年度折扣
        public decimal SalesIncomeWithoutTax { get; set; } // 含税销售净额
        public decimal SalesSumIncome { get; set; }
        public decimal SalesAmountNet { get; set; } // 销售净额
        public decimal SalesCost { get; set; } // 销售成本
        public decimal SalesGrossProfit { get; set; } //  销售毛利=销售净额-销售成本
        public decimal LeaveFactoryCost { get; set; } // 出厂成本
        public decimal LeaveFactoryGrossProfit { get; set; } //  出厂毛利 =销售净额-出场成本
        public string MeasureUnit { get; set; }  //计量单位
        public decimal TaxRate { get; set; }  //税率
        public decimal StandardQuantity { get; set; }
        public decimal AmountAdjust { get; set; } // 表体抹零
        public decimal Packages { get; set; }   //件数
        public decimal SalesAmountPig { get; set; } // 销售数量 -- 猪
        public decimal SalesAvgPricePig { get; set; } // 销售均价   -- 猪
        public decimal HeadAverageIncome { get; set; } // 头均收入
        public decimal HeadAverageCost { get; set; } // 头均成本
        public decimal HeadAverageProfit { get; set; } // 头均毛利
        public decimal AverageIncome { get; set; } // 斤均收入
        public decimal AverageCost { get; set; } // 斤均成本
        public decimal AverageProfit { get; set; } // 斤均毛利
        public decimal StandardTons { get; set; } // 标吨
        public string Specification { get; set; }  //规格
        public string SummaryType { get; set; }  // 汇总方式  
        public string SummaryTypeName { get; set; }  // 汇总方式  
    }

}
