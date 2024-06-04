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
    public class PurchaseSummaryCommon : MakeVoucherBase
    {

        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        public PurchaseSummaryCommon(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration) : base(httpClientUtil, hostCongfiguration)
        {
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil = httpClientUtil;
        }
        public  override FD_SettleReceipt getVoucherList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, AuthenticationHeaderValue token)
        {
            #region 构造采购汇总表查询条件  别问我为什么。产品非得要
            List<RptSearchModel> rpts = new List<RptSearchModel>() {
             new RptSearchModel(){Text="查询时间",Value=request.Begindate+"-"+request.Enddate},
             new RptSearchModel(){Text="单位筛选",Value=model.EnterpriseName},
             new RptSearchModel(){Text="核算单元",Value=model.TicketedPointName},
             new RptSearchModel(){Text="计量单位",Value="本地计量"},
             new RptSearchModel(){Text="数据来源",Value="结帐数据"},
             new RptSearchModel(){Text="显示位数",Value="保留两位"}
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
            List<DictionaryModel> purAbstracts = new List<DictionaryModel>();
            List<DictionaryModel> suppliers = new List<DictionaryModel>();
            reqUrl = _hostCongfiguration.ReportService + "/api/RptPurchaseRelated/GetPurchaseSummaryReport";
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
                List<PurchaseSummaryResultData> purchaseSummarys = GetPurchaseSummaryDataList(model, request, item, productGroupList, domain, isBreak, rpts);//获取采购汇总表数据
                purchaseSummarys?.ForEach(summary =>
                {
                    int index = 1;
                    Type type = summary.GetType();
                    FD_SettleReceiptDetailCommand detail = base.NewObject<PurchaseSummaryResultData>(summary, item, isDebit, formular, dictList);
                    if (item.IsPerson) { detail.PersonID = type.GetProperty("SummaryType" + index).GetValue(summary) + ""; index++; }
                    if (item.IsCustomer) { detail.CustomerID = type.GetProperty("SummaryType" + index).GetValue(summary) + ""; index++; }
                    if (item.IsMarket) { }
                    if (item.IsProduct) { detail.ProductID = type.GetProperty("SummaryType" + index).GetValue(summary) + ""; index++; }
                    Lines.Add(detail);
                });
                #region 包装钻取采购汇总表接口的必备参数
                if (item.Extends?.Count > 0)
                {
                    item.Extends?.ForEach(extend =>
                    {
                        switch (extend.Sort)
                        {
                            case (int)SortTypeEnum.商品分类:
                                if (products.FindIndex(s => s.id == extend.Object) < 0&& productGroupList.Where(s => s.Id == extend.Object).FirstOrDefault()!=null)//去重的作用
                                {
                                    int rank = (int)productGroupList.Where(s => s.Id == extend.Object).FirstOrDefault()?.Rank;
                                    products.Add(new DictionaryModel()
                                    {
                                        id = extend.Object,
                                        name = extend.ObjectName,
                                        rank = rank
                                    });
                                }
                                break;
                            case (int)SortTypeEnum.采购摘要:
                                if (purAbstracts.FindIndex(s => s.id == extend.Object) < 0)//去重的作用
                                {
                                    purAbstracts.Add(new DictionaryModel()
                                    {
                                        id = extend.Object,
                                        name = extend.ObjectName,
                                    });
                                }
                                break;
                            case (int)SortTypeEnum.供应商:
                                if (suppliers.FindIndex(s => s.id == extend.Object) < 0)//去重的作用
                                {
                                    suppliers.Add(new DictionaryModel()
                                    {
                                        id = extend.Object,
                                        name = extend.ObjectName,
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
            string productsStr = string.Empty;
            string suppliersStr = string.Empty;
            string purAbstractStr = string.Empty;
            if (products.Count > 0)//商品分类筛选条件
            {
                productsStr = JsonConvert.SerializeObject(products.Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "商品类别", Value = string.Join(',', products.Select(s => s.name)) });
            }
            if (purAbstracts.Count > 0)//采购摘要
            {
                purAbstractStr = string.Join(',', purAbstracts.Select(s => s.id).Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "采购摘要", Value = string.Join(',', purAbstracts.Select(s => s.name)) });
            }
            if (suppliers.Count > 0)//供应商筛选条件
            {
                suppliersStr = string.Join(',', suppliers.Select(s => s.id).Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "供应商", Value = string.Join(',', suppliers.Select(s => s.name)) });
            }
            domain.ProductLst = productsStr + "~" + purAbstractStr + "~" + suppliersStr;
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

        private List<PurchaseSummaryResultData> GetPurchaseSummaryDataList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, List<TreeModelODataEntity> productGroupList, FD_SettleReceipt domain, bool isBreak, List<RptSearchModel> rpts)
        {
            try
            {
                #region 构造接口参数
                PurchaseSummaryRequest purchaserequest = new PurchaseSummaryRequest()
                {
                    Begindate = request.Begindate,
                    Boid = request.Boid,
                    CanWatchEntes = new List<string>() { request.EnterpriseID },
                    DataSource = 0,
                    DateRules = null,
                    Enddate =request.Enddate,
                    EnteID = Convert.ToInt64(request.EnterpriseID),
                    GroupID = Convert.ToInt64(request.GroupID),
                    HasReportAuth = false,
                    IsCompanyManager = false,
                    IsEnterCate = false,
                    IsGroupByEnteCate = false,
                    IsSpecialProuct = false,
                    MeasurementUnit = 0,
                    MenuParttern = "0",
                    OnlyCombineEnte = false,
                    OwnEntes = new List<string>() { request.EnterpriseID },
                    PurchaseAbstract = string.Join(',', item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.采购摘要).Select(s => s.Object)),
                    QiDian = 0,
                    SummaryT1Rank = "-1",
                    SummaryT2Rank = "-1",
                    SummaryT3Rank = "-1",
                    TicketedPoint = model.TicketedPointID,
                };
                List<DictionaryModel> products = new List<DictionaryModel>();
                string SupplierName_id = string.Empty;
                if (item.Extends?.Count > 0)
                {
                    item.Extends?.ForEach(extend =>
                    {
                        switch (extend.Sort)
                        {
                            case (int)SortTypeEnum.商品分类:
                                if (productGroupList.Where(s => s.Id == extend.Object).FirstOrDefault() != null)
                                {
                                    int rank = (int)productGroupList.Where(s => s.Id == extend.Object).FirstOrDefault()?.Rank;
                                    products.Add(new DictionaryModel()
                                    {
                                        id = extend.Object,
                                        rank = rank
                                    });
                                }
                                else
                                {

                                }
                                break;
                            case (int)SortTypeEnum.供应商:
                                SupplierName_id += (extend.Object + ",");
                                break;
                            default:
                                break;
                        }
                    });
                    purchaserequest.ProductLst = JsonConvert.SerializeObject(products);
                    purchaserequest.SupplierName_id = SupplierName_id;
                }
                if (item.IsPerson)
                {
                    purchaserequest.SummaryType1 = "h.Name";
                    if (isBreak) rpts.Add(new RptSearchModel() { Text = "汇总方式一", Value = "采购员" });
                }
                if (item.IsCustomer)
                {
                    if (string.IsNullOrEmpty(purchaserequest.SummaryType1))
                    {
                        purchaserequest.SummaryType1 = "j.CustomerName";
                        if (isBreak) rpts.Add(new RptSearchModel() { Text = "汇总方式一", Value = "供应商" });
                    }
                    else
                    {
                        purchaserequest.SummaryType2 = "j.CustomerName";
                        if (isBreak) rpts.Add(new RptSearchModel() { Text = "汇总方式二", Value = "供应商" });
                    }
                }
                if (item.IsMarket)//因为没有部门汇总方式，所以不做处理
                {

                }
                if (item.IsProduct)
                {
                    bool isSet = false;
                    if (string.IsNullOrEmpty(purchaserequest.SummaryType1) && !isSet)
                    {
                        purchaserequest.SummaryType1 = "d.ProductName";
                        if (isBreak) rpts.Add(new RptSearchModel() { Text = "汇总方式一", Value = "商品代号" });
                        isSet = true;
                    }
                    if (string.IsNullOrEmpty(purchaserequest.SummaryType2) && !isSet)
                    {
                        purchaserequest.SummaryType2 = "d.ProductName";
                        if (isBreak) rpts.Add(new RptSearchModel() { Text = "汇总方式二", Value = "商品代号" });
                        isSet = true;
                    }
                    if (string.IsNullOrEmpty(purchaserequest.SummaryType3) && !isSet)
                    {
                        purchaserequest.SummaryType3 = "d.ProductName";
                        if (isBreak) rpts.Add(new RptSearchModel() { Text = "汇总方式三", Value = "商品代号" });
                        isSet = true;
                    }
                }
                if (!item.IsPerson && !item.IsCustomer && !item.IsProduct)
                {
                    purchaserequest.SummaryType1 = "c.EnterpriseName";
                    if (isBreak) rpts.Add(new RptSearchModel() { Text = "汇总方式一", Value = "单位" });
                }
                #endregion
                if (isBreak)
                {
                    domain.SummaryType = purchaserequest.SummaryType1 + "~" + purchaserequest.SummaryType2 + "~" + purchaserequest.SummaryType3;
                    domain.SummaryTypeName = null;//钻取采购汇总表可不传，所以可以为空
                }
                
                ResultModel<PurchaseSummaryResultData> result = base.postActionByUrl<ResultModel<PurchaseSummaryResultData>, PurchaseSummaryRequest>(purchaserequest);
                return result.Code == 0 ? result.Data : new List<PurchaseSummaryResultData>();
            }
            catch (Exception ex)
            {
                return new List<PurchaseSummaryResultData>();
            }
        }
        public Dictionary<string, string> GetDict()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("2202031628080000202", "_purchaseAmount");
            value.Add("2202031628080000203", "_adjustAmount");
            value.Add("2202031628080000204", "_purchaseTotalWithinTax");
            value.Add("2202031628080000205", "_amountWithoutTax");
            value.Add("2202031628080000206", "_fare");
            return value;
        }

    }
    public class PurchaseSummaryResultData
    {
        public decimal _purchaseAmount =>Math.Round(this.PurchaseAmount,2);
        public decimal _adjustAmount => Math.Round(this.AdjustAmount, 2);
        public decimal _purchaseTotalWithinTax => Math.Round(this.PurchaseTotalWithinTax, 2);
        public decimal _amountWithoutTax => Math.Round(this.AmountWithoutTax, 2);
        public decimal _fare => Math.Round(this.Fare, 2);
        /// <summary>
        /// 采购结算数量
        /// </summary>
        public decimal PurchaseSettlementQuantity { get; set; }
        /// <summary>
        /// 采购数量/入库数量
        /// </summary>
        public decimal PurchaseQuantity { get; set; }
        /// <summary>
        /// 采购件数
        /// </summary>
        public decimal PurchasePackages { get; set; }
        /// <summary>
        ///  采购金额
        /// </summary>
        public decimal PurchaseAmount { get; set; }
        /// <summary>
        /// 采购均价
        /// </summary>
        public decimal PurchaseAvgPrice { get; set; }
        /// <summary>
        /// 不含税采购金额
        /// </summary>
        public decimal AmountWithoutTax { get; set; }
        /// <summary>
        /// 调整金额/运费
        /// </summary>
        public decimal AdjustAmount { get; set; }
        /// <summary>
        /// 结算金额
        /// </summary>
        public decimal PurchaseTotalWithinTax { get; set; }
        /// <summary>
        /// 计量单位
        /// </summary>
        public string MeasureUnit { get; set; }
        /// <summary>
        /// 标准计量单位
        /// </summary>
        public string biaozhunUnit { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        public decimal TaxRate { get; set; }
        /// <summary>
        /// 市场单价
        /// </summary>
        public decimal UnitPriceTax { get; set; }
        /// <summary>
        /// 市场金额
        /// </summary>
        public decimal TotalPriceTax { get; set; }
        /// <summary>
        /// 结算单价
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 折扣金额
        /// </summary>
        public decimal TotalDiscount { get; set; }

        /// <summary>
        /// 判断系统选项是否启用 显示运费
        /// </summary>
        public bool IsShowFare { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        public decimal Fare { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalFare { get; set; }
        /// <summary>
        /// 已开发票
        /// </summary>
        public decimal YiKaiFaPiao { get; set; }
        /// <summary>
        /// 未开发票
        /// </summary>
        public decimal WeiKaiFaPiao { get; set; }
        #region 小计专用字段
        public int iOrder { get; set; }
        public bool IsSubTotal { get; set; }
        #endregion
        #region 汇总方式
        /// <summary>
        /// 汇总方式1
        /// </summary>
        public string SummaryType1 { get; set; }
        public string SummaryType1Name { get; set; }
        public string SummaryType1FieldName { get; set; }
        public string SummaryType2 { get; set; }
        public string SummaryType2Name { get; set; }
        public string SummaryType2FieldName { get; set; }
        public string SummaryType3 { get; set; }
        public string SummaryType3Name { get; set; }
        public string SummaryType3FieldName { get; set; }
        #endregion
    }

}
