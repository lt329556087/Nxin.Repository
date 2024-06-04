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
    /// 生产成本结转
    /// </summary>
    public class ProductionCostSummaryCommon : MakeVoucherBase
    {

        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        public ProductionCostSummaryCommon(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration) : base(httpClientUtil, hostCongfiguration)
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
            List<FD_SettleReceiptDetailCommand> Lines = new List<FD_SettleReceiptDetailCommand>();
            List<DictionaryModel> products = new List<DictionaryModel>();
            List<DictionaryModel> productTypes = new List<DictionaryModel>();
            List<DictionaryModel> cunhuoTypes = new List<DictionaryModel>();
            List<TreeModelODataEntity> productGroupList = base.GetProductGroupClassAsync(request.GroupID, token);
            //获取成本项目
            List<DictinonaryModel> dictList = GetDict();
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
                var pigcostResult = GetProductionCostSummaryDataList(request, item, productGroupList, formular, token);
                pigcostResult?.ForEach(summary =>
                {
                    FD_SettleReceiptDetailCommand detail = base.NewObjectExtend<ProductionCostSummary>(summary, item, isDebit, formular, dictList);
                    if (item.IsProduct) detail.ProductID = summary.ProductId;
                    if (item.IsSum) detail.EnterpriseID = request.EnterpriseID;
                    Lines.Add(detail);
                });
                #region 封装钻取汇总表接口的必备参数
                if (isBreak)
                {
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
                        var cunhuoList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.存货分类).Select(s => s).ToList();
                        cunhuoList.ForEach(s =>
                        {
                            if (cunhuoTypes.FindIndex(_ => _.id == s.Object) < 0)
                            {
                                cunhuoTypes.Add(new DictionaryModel()
                                {
                                    id = s.Object,
                                    name = s.ObjectName
                                });
                            }
                        });

                    }
                }
                #endregion
                isBreak = false;
            }
            string cunhuoStr = string.Empty;
            if (productTypes.Count > 0)//商品分类筛选条件
            {
                rpts.Add(new RptSearchModel() { Text = "商品类别", Value = string.Join(',', productTypes.Select(s => s.name)) });
            }
            if (products.Count > 0)//商品代号筛选条件
            {
                rpts.Add(new RptSearchModel() { Text = "商品代号", Value = string.Join(',', products.Select(s => s.name)) });
            }
            if (cunhuoTypes.Count > 0)//存货分类
            {
                cunhuoStr = string.Join(',', cunhuoTypes.Select(s => s.id).Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "存货分类", Value = string.Join(',', cunhuoTypes.Select(s => s.name)) });
            }
            var productunion = products.Union(productTypes).Distinct().ToList();
            string productsStr = JsonConvert.SerializeObject(productunion);
            domain.ProductLst = productsStr + "~" + cunhuoStr;
            rpts.Add(new RptSearchModel() { Text = "汇总方式", Value = "商品代号" });
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
        #region 成本汇总表
        private List<ProductionCostSummary> GetProductionCostSummaryDataList(FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, List<TreeModelODataEntity> productGroupList, string formular, AuthenticationHeaderValue token)
        {
            try
            {
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
                //商品代号过滤条件
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
                var formulaList = item.Formulas.Select(s => s.FormulaID).ToArray();
                List<CostSummaryReport> result1 = new List<CostSummaryReport>();
                List<MaterialsCostReport> result2 = new List<MaterialsCostReport>();
                //成本汇总表
                List<string> KeyList1 = GetReprotKey1();
                var param1 = new
                {
                    BeginDate = request.Begindate,
                    EndDate = request.Enddate,
                    EnterPriseIdList = request.EnterpriseID.ToString(),
                    UnitName = 1,
                    TableShow = 2,
                    ChengBenType = 0,
                    SummaryType1 = "b.ProductID,busipd.Specification,um.UnitName,d.ProductName",
                    StockClassification = string.Join(',', item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.存货分类).Select(s => s.Object)),
                    ProductLst = JsonConvert.SerializeObject(products)
                };
                bool iscontions1 = IsContainsMethod(formular, KeyList1);
                if (iscontions1)
                    result1 = _httpClientUtil.PostJsonAsync<ResultModel<CostSummaryReport>>(_hostCongfiguration.ReportService + "/api/RptMyCostSummary/GetMyCostSummaryReport", param1).Result.Data;
                //生产成本表
                List<string> KeyList2 = GetReprotKey2();
                var param2 = new
                {
                    Begindate = request.Begindate,
                    Enddate = request.Enddate,
                    OnlyCombineEnte = false,
                    IsEnterCate = false,
                    EnterpriseList = request.EnterpriseID,
                    CunHuoFenLei = string.Join(',', item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.存货分类).Select(s => s.Object)),
                    ProductLst = JsonConvert.SerializeObject(products),
                    MeasurementUnit = 0,
                    DataShow = 0,
                    isRateExpense = 0,
                    DataSource = 0,
                    SummaryType1 = "d.ProductName",
                    OwnEntes = new List<string>() { request.Boid },
                    CanWatchEntes = new List<string>() { request.EnterpriseID },
                    SummaryT1Rank = -1,
                    SummaryT2Rank = -1,
                    SummaryT3Rank = -1,
                    IsGroupByEnteCate = false,
                    GroupID = request.GroupID,
                    EnteID = request.EnterpriseID,
                    Boid = request.Boid,
                    MenuParttern = 0,
                };
                bool iscontions2 = IsContainsMethod(formular, KeyList2);
                if (iscontions2)
                    result2 = _httpClientUtil.PostJsonAsync<ResultModel<MaterialsCostReport>>(_hostCongfiguration.ReportService + "/api/RptRawMaterialsCost/GetRawMaterialsCostReport", param2).Result.Data;
                var summaryList = TransformationSummary(result1, result2);
                var data = dynamicGroupbySummary(summaryList, item.IsProduct);
                return data;
            }
            catch (Exception ex)
            {
                Log.Logger.Error("日志 GetProductionCostSummaryDataList :" + ex.ToString() + "\n param=" + JsonConvert.SerializeObject(item));
                return new List<ProductionCostSummary>();
            }
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
            value.Add(new DictinonaryModel("1904170956380000101", "tzAmount"));
            value.Add(new DictinonaryModel("1904170956380000102", "tzCost"));
            value.Add(new DictinonaryModel("1904170956380000103", "yclCost"));
            value.Add(new DictinonaryModel("1904170956380000104", "bzCost"));
            value.Add(new DictinonaryModel("1904170956380000105", "bcpCost"));
            value.Add(new DictinonaryModel("1904170956380000106", "zcscCost"));
            value.Add(new DictinonaryModel("1904170956380000107", "hzscCost"));
            value.Add(new DictinonaryModel("1904170956380000108", "hjscCost"));
            value.Add(new DictinonaryModel("1904170956380000109", "ccpCost"));
            value.Add(new DictinonaryModel("1904170956380000110", "costSub"));
            value.Add(new DictinonaryModel("1904170956380000111", "scfy"));
            value.Add(new DictinonaryModel("1904170956380000112", "TotalCost"));
            value.Add(new DictinonaryModel("1904170956380000113", "unitCost"));
            return value;
        }
        #endregion
        #region 数据转换
        public List<ProductionCostSummary> TransformationSummary(List<CostSummaryReport> result1, List<MaterialsCostReport> result2)
        {
            List<ProductionCostSummary> list = new List<ProductionCostSummary>();
            foreach (var item in result1)
            {
                list.Add(new ProductionCostSummary()
                {
                    ProductId = item.ProductID,
                    tzAmount = item.tzAmount,
                    tzCost = item.tzCost,
                });
            }
            foreach (var item in result2)
            {
                list.Add(new ProductionCostSummary()
                {
                    ProductId = item.ProductID,
                    yclCost = item.yclCost,
                    bzCost = item.bzCost,
                    bcpCost = item.bcpCost,
                    zcscCost = item.zcscCost,
                    hzscCost = item.hzscCost,
                    hjscCost = item.hjscCost,
                    ccpCost = item.ccpCost,
                    costSub = item.costSub,
                    scfy = item.scfy,
                    TotalCost = item.TotalCost,
                    unitCost = item.unitCost,
                });
            }
            return list;
        }
        #endregion
        public List<ProductionCostSummary> dynamicGroupbySummary(List<ProductionCostSummary> summaryList, bool isProduct)
        {
            List<ProductionCostSummary> list = new List<ProductionCostSummary>();
            if (isProduct)
            {
                list = summaryList.GroupBy(s => s.ProductId).Select(s => new ProductionCostSummary()
                {
                    ProductId = s.Key,
                    tzAmount = s.Sum(n => n.tzAmount),
                    tzCost = s.Sum(n => n.tzCost),
                    yclCost = s.Sum(n => n.yclCost),
                    bzCost = s.Sum(n => n.bzCost),
                    bcpCost = s.Sum(n => n.bcpCost),
                    zcscCost = s.Sum(n => n.zcscCost),
                    hzscCost = s.Sum(n => n.hzscCost),
                    hjscCost = s.Sum(n => n.hjscCost),
                    ccpCost = s.Sum(n => n.ccpCost),
                    costSub = s.Sum(n => n.costSub),
                    scfy = s.Sum(n => n.scfy),
                    TotalCost = s.Sum(n => n.TotalCost),
                    unitCost = s.Sum(n => n.unitCost),
                }).ToList();
            }
            else
            {
                list.Add(new ProductionCostSummary()
                {
                    ProductId = summaryList.FirstOrDefault()?.ProductId,
                    tzAmount = summaryList.Sum(n => n.tzAmount),
                    tzCost = summaryList.Sum(n => n.tzCost),
                    yclCost = summaryList.Sum(n => n.yclCost),
                    bzCost = summaryList.Sum(n => n.bzCost),
                    bcpCost = summaryList.Sum(n => n.bcpCost),
                    zcscCost = summaryList.Sum(n => n.zcscCost),
                    hzscCost = summaryList.Sum(n => n.hzscCost),
                    hjscCost = summaryList.Sum(n => n.hjscCost),
                    ccpCost = summaryList.Sum(n => n.ccpCost),
                    costSub = summaryList.Sum(n => n.costSub),
                    scfy = summaryList.Sum(n => n.scfy),
                    TotalCost = summaryList.Sum(n => n.TotalCost),
                    unitCost = summaryList.Sum(n => n.unitCost),
                });
            }
            return list;
        }


        /// <summary>
        /// 指标
        /// </summary>
        /// <returns></returns>
        public List<string> GetReprotKey1()
        {
            List<string> value = new List<string>();
            value.Add("1904170956380000101");
            value.Add("1904170956380000102");
            return value;
        }
        public List<string> GetReprotKey2()
        {
            List<string> value = new List<string>();
            value.Add("1904170956380000103");
            value.Add("1904170956380000104");
            value.Add("1904170956380000105");
            value.Add("1904170956380000106");
            value.Add("1904170956380000107");
            value.Add("1904170956380000108");
            value.Add("1904170956380000109");
            value.Add("1904170956380000110");
            value.Add("1904170956380000111");
            value.Add("1904170956380000112");
            value.Add("1904170956380000113");
            return value;
        }
        public bool IsContainsMethod(string formular, List<string> KeyList)
        {
            bool iscontains = false;
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

}
