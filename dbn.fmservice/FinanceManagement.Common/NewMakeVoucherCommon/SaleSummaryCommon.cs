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
    /// 销售
    /// </summary>
    public class SaleSummaryCommon : MakeVoucherBase
    {

        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        public SaleSummaryCommon(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration) : base(httpClientUtil, hostCongfiguration)
        {
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil = httpClientUtil;
        }
        public override FD_SettleReceipt getVoucherList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, AuthenticationHeaderValue token)
        {
            #region 构造销售汇总表查询条件  别问我为什么。产品非得要
            List<RptSearchModel> rpts = new List<RptSearchModel>() {
             new RptSearchModel(){Text="查询时间",Value=request.Begindate+"-"+request.Enddate},
             new RptSearchModel(){Text="单位筛选",Value=model.EnterpriseName},
             new RptSearchModel(){Text="核算单元",Value=model.TicketedPointName},
             new RptSearchModel(){Text="计量单位",Value="本地计量"},
             new RptSearchModel(){Text="销售类型",Value="销售"},
             new RptSearchModel(){Text="数据来源",Value="实时数据"}
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
            List<DictionaryModel> productTypes = new List<DictionaryModel>();
            List<DictionaryModel> products = new List<DictionaryModel>();
            List<DictionaryModel> salesAbstracts = new List<DictionaryModel>();
            List<DictionaryModel> markets = new List<DictionaryModel>();
            reqUrl = _hostCongfiguration.ReportService + "/api/SaleSummary/GetSummaryAsync";
            List<SummaryType> summaryTypes = GetSummaryList(request);
            Dictionary<string, string> dictList = GetDict();
            List<TreeModelODataEntity> productGroupList = base.GetProductGroupClassAsync(request.GroupID, token);
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
                List<SaleSummaryResultData> saleSummarys = GetSaleSummaryDataList(model, request, item, productGroupList, summaryTypes, domain, isBreak);//获取销售汇总表数据
                foreach (var sale in saleSummarys)
                {
                    string str = $@"含税销售收入:{sale.SalesIncomeWithinTax},现场折扣:{sale.PresentDiscount},抹零:{sale.AmountAdjust},月度折扣:{sale.MonthlyDiscount},年度折扣:{sale.YearlyDiscount},含税销售净额:{sale.SalesIncomeWithoutTax},不含税销售净额:{sale.SalesAmountNet}";
                }
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
                    var productTypeList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.商品分类).Select(s => s).ToList();
                    productTypeList.ForEach(s =>
                    {
                        if (productTypes.FindIndex(_ => _.id == s.Object) < 0&& productGroupList.Where(n => n.Id == s.Object).FirstOrDefault()!=null)
                        {
                            int rank = (int)productGroupList.Where(n => n.Id == s.Object).FirstOrDefault()?.Rank;
                            productTypes.Add(new DictionaryModel()
                            {
                                id = s.Object,
                                name = s.ObjectName,
                                rank = rank
                            });

                        }
                    });
                    var productList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.商品代号).Select(s => s).ToList();
                    productList.ForEach(s =>
                    {
                        if (products.FindIndex(_ => _.id == s.Object) < 0)
                        {
                            int maxRank = productGroupList.Max(s => s.Rank) + 2;
                            products.Add(new DictionaryModel()
                            {
                                id = s.Object,
                                name = s.ObjectName,
                                rank = maxRank
                            });
                        }
                    });
                    var saleAbsList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.销售摘要).Select(s => s).ToList();
                    saleAbsList.ForEach(s =>
                    {
                        if (salesAbstracts.FindIndex(_ => _.id == s.Object) < 0)
                        {
                            salesAbstracts.Add(new DictionaryModel()
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
                isBreak = false;
            }
            string abstractStr = string.Empty;
            string marketStr = string.Empty;
            if (productTypes.Count > 0)//商品分类筛选条件
            {
                rpts.Add(new RptSearchModel() { Text = "商品类别", Value = string.Join(',', productTypes.Select(s => s.name)) });
            }
            if (products.Count > 0)//商品代号筛选条件
            { 
                rpts.Add(new RptSearchModel() { Text = "商品代号", Value = string.Join(',', products.Select(s => s.name)) });
            }
            if (salesAbstracts.Count > 0)//销售摘要
            {
                abstractStr = string.Join(',', salesAbstracts.Select(s => s.id).Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "销售摘要", Value = string.Join(',', salesAbstracts.Select(s => s.name)) });
            }
            if (markets.Count > 0)//区域
            {
                marketStr = string.Join(',', markets.Select(s => s.id).Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "区域", Value = string.Join(',', markets.Select(s => s.name)) });
            }
            var productunion = products.Union(productTypes).Distinct().ToList();
            string productsStr = JsonConvert.SerializeObject(productunion);
            domain.ProductLst = productsStr + "~" + abstractStr + "~" + marketStr;
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
        public List<SummaryType> GetSummaryList(FM_CarryForwardVoucherSearchCommand request)
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
        private List<SaleSummaryResultData> GetSaleSummaryDataList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, List<TreeModelODataEntity> productGroupList, List<SummaryType> summaryTypes, FD_SettleReceipt domain, bool isBreak)
        {
            try
            {
                string SummaryType = string.Empty;
                string SummaryTypeName = string.Empty;
                #region 构造接口参数
                SaleSummaryRequest salerequest = new SaleSummaryRequest()
                {
                    BeginDate = request.Begindate,
                    EndDate = request.Enddate,
                    SalesAbstract = string.Join(',', item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.销售摘要).Select(s => s.Object)),
                    OnlyCombineEnte = false,
                    MeasurementUnit = "0",
                    IsGift = "2",//销售+赠送
                    TicketedPoint = model.TicketedPointID,
                    ReceiptType = "-1",
                    QiDian = 0,
                    APPID = 1612062114270000101,
                    IsPig = false,
                    DataSource = 0,
                    GroupId = Convert.ToInt64(request.GroupID),
                    EnteID = request.EnterpriseID,
                    EnterpriseId = Convert.ToInt64(request.EnterpriseID),
                    MenuParttern = 0,
                    BoId = Convert.ToInt64(request.Boid),
                    OwnEntes = new List<string>() { request.EnterpriseID },
                    MarketID = string.Join(',', item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.部门).Select(s => s.Object)),
                };
                //商品分类过滤条件
                List<DictionaryModel> products = new List<DictionaryModel>();
                var productTypeList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.商品分类).Select(s => s.Object).ToList();
                foreach (var entity in productTypeList)
                {
                    if (productGroupList.Where(s => s.Id == entity).FirstOrDefault()!=null)
                    {
                        int rank = (int)productGroupList.Where(s => s.Id == entity).FirstOrDefault()?.Rank;
                        products.Add(new DictionaryModel()
                        {
                            id = entity,
                            rank = rank
                        });
                    }
                    else
                    {

                    }
                }
                var productList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.商品代号).Select(s => s.Object).ToList();
                int maxRank = productGroupList.Max(s => s.Rank) + 2;
                foreach (var entity in productList)
                {
                    products.Add(new DictionaryModel()
                    {
                        id = entity,
                        rank = maxRank
                    });
                }
                salerequest.ProductLst = JsonConvert.SerializeObject(products);
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
            value.Add("1911081429200099201", "SalesIncomeWithinTax");//含税销售额_含税销售收入
            value.Add("1911081429200099202", "PresentDiscount");//含税销售额_现场折扣
            value.Add("1911081429200099203", "AmountAdjust");//含税销售额_抹零
            value.Add("1911081429200099204", "SalesIncomeWithoutTax");//含税销售额_含税销售净额
            value.Add("1911081429200099205", "SalesAmountNet");//销售净额
            value.Add("1911081429200099206", "SalesCost");//销售毛利_销售成本
            value.Add("1911081429200099207", "SalesGrossProfit");//销售毛利_销售毛利
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
    public class SaleSummaryResultData
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
