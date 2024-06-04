using Architecture.Common.HttpClientUtil;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http.Headers;
using FinanceManagement.Common.MonthEndCheckout;

namespace FinanceManagement.Common.NewMakeVoucherCommon
{
    /// <summary>
    /// 运费结转
    /// </summary>
    public class FreightSummaryCommon : MakeVoucherBase
    {

        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        public FreightSummaryCommon(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration) : base(httpClientUtil, hostCongfiguration)
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
            List<DictionaryModel> freightsAbstracts = new List<DictionaryModel>();
            List<DictionaryModel> suppliers = new List<DictionaryModel>();
            reqUrl = _hostCongfiguration.ReportService + "/api/RptMyFreightSummary/GetMyFreightSummaryReport";
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
                List<Freight> freightSummarys = GetFreightSummaryDataList(model, request, item, productGroupList, domain, isBreak);//获取运费汇总表数据
                freightSummarys?.ForEach(summary =>
                {
                    FD_SettleReceiptDetailCommand detail = base.NewObject<Freight>(summary, item, isDebit, formular, dictList);
                    if (item.IsCustomer) detail.CustomerID = summary.SummaryType1;
                    if (item.IsProduct) detail.ClassificationID = item.IsCustomer? summary.SummaryType2: summary.SummaryType1;//如果选择了供应商+商品分类，商品分类取SummaryType2
                    if (item.IsSum) detail.EnterpriseID = request.EnterpriseID;
                    Lines.Add(detail);
                });
                #region 包装钻取销售汇总表接口的必备参数
                if (item.Extends?.Count > 0)
                {
                    var productTypeList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.商品分类).Select(s => s).ToList();
                    productTypeList.ForEach(s =>
                    {
                        if (productTypes.FindIndex(_ => _.id == s.Object) < 0&& productGroupList.Where(n => n.Id == s.Object).FirstOrDefault() != null)
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
                    var supplierList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.供应商).Select(s => s).ToList();
                    supplierList.ForEach(s =>
                    {
                        if (suppliers.FindIndex(_ => _.id == s.Object) < 0)
                        {
                            suppliers.Add(new DictionaryModel()
                            {
                                id = s.Object,
                                name = s.ObjectName,
                            });
                        }
                    });
                    var freightAbsList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.运费摘要).Select(s => s).ToList();
                    freightAbsList.ForEach(s =>
                    {
                        if (freightsAbstracts.FindIndex(_ => _.id == s.Object) < 0)
                        {
                            freightsAbstracts.Add(new DictionaryModel()
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
            string productsStr = string.Empty;
            string abstractStr = string.Empty;
            string supplierStr = string.Empty;
            if (productTypes.Count > 0)//商品分类筛选条件
            {
                productsStr = JsonConvert.SerializeObject(productTypes.Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "商品类别", Value = string.Join(',', productTypes.Select(s => s.name)) });
            }
            if (freightsAbstracts.Count > 0)//运费摘要
            {
                abstractStr = string.Join(',', freightsAbstracts.Select(s => s.id).Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "运费摘要", Value = string.Join(',', freightsAbstracts.Select(s => s.name)) });
            }
            if (suppliers.Count > 0)//区域
            {
                supplierStr = string.Join(',', suppliers.Select(s => s.id).Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "供应商", Value = string.Join(',', suppliers.Select(s => s.name)) });
            }
            domain.ProductLst = productsStr + "~" + abstractStr + "~" + supplierStr;
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
        private List<Freight> GetFreightSummaryDataList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, List<TreeModelODataEntity> productGroupList, FD_SettleReceipt domain, bool isBreak)
        {
            try
            {
                string SummaryTypeName = string.Empty;
                #region 构造接口参数
                FreightRequest freightrequest = new FreightRequest()
                {
                    BeginDate = request.Begindate,
                    EndDate = request.Enddate,
                    EnterPriseIdList = request.EnterpriseID,
                    OnlyCombineEnte = false,
                    CarriageAbstract = "",
                    SupplierNameId = string.Join(',', item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.供应商).Select(s => s.Object)),
                    DataSource = "0",
                    IsGroupByEnteCate = false,
                    GroupID = request.GroupID,
                    EnteID = request.EnterpriseID,
                    MenuParttern = "0",
                    Boid = request.Boid,
                    SubjectLevel = 0
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
                   
                }
                freightrequest.ProductLst = JsonConvert.SerializeObject(products);
                if (item.IsCustomer)
                {
                    freightrequest.SummaryType1 = "Supplier.SupplierID";
                    SummaryTypeName += "供应商";
                }
                if (item.IsProduct)
                {
                    if (!string.IsNullOrEmpty(freightrequest.SummaryType1))
                    {
                        freightrequest.SummaryType2 = "shangpinfenleiyiji";
                        SummaryTypeName += ",商品分类";
                    }
                    else
                    {
                        freightrequest.SummaryType1 = "shangpinfenleiyiji";
                        SummaryTypeName += "商品分类";
                    }
                }
                freightrequest.SummaryType3 = "carrAbstract.CarriageAbstractID";//摘要汇总
                #endregion
                if (isBreak)
                {
                    domain.SummaryType = freightrequest.SummaryType1 + "~" + freightrequest.SummaryType2;
                    domain.SummaryTypeName = SummaryTypeName;
                }
                ResultModel<Freight> result = base.postActionByUrl<ResultModel<Freight>, dynamic>(freightrequest);
                //运费摘要过滤
                var abstractList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.运费摘要).Select(s => s.Object).ToList();
                if (abstractList.Count()>0)
                {
                    result.Data = result.Data.Where(s => abstractList.Contains(s.SummaryType3)).ToList();
                }
                if (item.IsSum)
                {
                    result.Data = new List<Freight>()
                    {
                        new Freight()
                        {
                             Quantity=(decimal)result.Data?.Sum(s=>s.Quantity),
                             Packages=(decimal)result.Data?.Sum(s=>s.Packages),
                             UnitPriceTax=(decimal)result.Data?.Sum(s=>s.UnitPriceTax),
                             Amount=(decimal)result.Data?.Sum(s=>s.Amount),
                             AmountAdjust=(decimal)result.Data?.Sum(s=>s.AmountAdjust),
                             AmountTotal=(decimal)result.Data?.Sum(s=>s.AmountTotal),
                        }
                    };
                }
                return result.Data;
            }
            catch (Exception ex)
            {
                return new List<Freight>();
            }
        }
        public Dictionary<string, string> GetDict()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("1801311901000000102", "Amount");//运费金额
            value.Add("1801311901000000103", "AmountAdjust");//运费调整
            value.Add("1801311901000000104", "AmountTotal");//实际运费
            return value;
        }
    }
}
