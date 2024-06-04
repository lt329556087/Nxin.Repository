using Architecture.Common.HttpClientUtil;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace FinanceManagement.Common.MakeVoucherCommon
{
    public class SuppliesEstimationSummaryCommon : MakeVoucherBase
    {

        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        public SuppliesEstimationSummaryCommon(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration) : base(httpClientUtil, hostCongfiguration)
        {
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil = httpClientUtil;
        }
        public override FD_SettleReceipt getVoucherList(FM_CarryForwardVoucherODataEntity model, FM_CarryForwardVoucherInterfaceSearchCommand request)
        {
            #region 构造销售汇总表查询条件  别问我为什么。产品非得要
            List<RptSearchModel> rpts = new List<RptSearchModel>() {
             new RptSearchModel(){Text="查询时间",Value=request.Begindate+"-"+request.Enddate},
             new RptSearchModel(){Text="单位筛选",Value=model.EnterpriseName},
             new RptSearchModel(){Text="入库方式",Value=model.TransferAccountsAbstractName},
             new RptSearchModel(){Text="单据字",Value=model.TicketedPointName},
            };
            #endregion
            FD_SettleReceipt domain = new FD_SettleReceipt()
            {
                SettleReceipType = "201610220104402203",TicketedPointID = model.TicketedPointID,TransBeginDate = request.Begindate,TransEndDate = request.Enddate
            };
            List<FD_SettleReceiptDetailCommand> Lines = new List<FD_SettleReceiptDetailCommand>();
            List<DictionaryModel> suppliesList = new List<DictionaryModel>();
            List<DictionaryModel> suppliers = new List<DictionaryModel>();
            reqUrl = _hostCongfiguration.QlwServiceHost + "/api/FDGoodsEstimationSummaryRelated/GetEstimationSummaryReport";
            Dictionary<string, string> dictList = GetDict();
            bool isBreak = true;
            foreach (var item in model.Lines)
            {
                List<EstimationSummaryModelForDataResult> suppliesEstimationSummarys = GetSuppliesEstimationSummaryDataList(model, request, item, domain, isBreak, rpts);//获取物品结算汇总表数据
                bool isDebit = false;
                string formular = string.Empty;
                if (!string.IsNullOrEmpty(item.DebitSecFormula) && item.DebitSecFormula != "0") { isDebit = true; formular = item.DebitSecFormula; }
                if (!string.IsNullOrEmpty(item.CreditSecFormula) && item.CreditSecFormula != "0") { isDebit = false; formular = item.CreditSecFormula; }
                suppliesEstimationSummarys?.ForEach(summary =>
                {
                    int index = 1;
                    Type type = summary.GetType();
                    FD_SettleReceiptDetailCommand detail = base.NewObject<EstimationSummaryModelForDataResult>(summary, item, isDebit, formular, dictList);
                    if (item.IsPerson) { detail.PersonID = type.GetProperty("SummaryType" + index).GetValue(summary) + ""; index++; }
                    if (item.IsCustomer) { detail.CustomerID = type.GetProperty("SummaryType" + index).GetValue(summary) + ""; index++; }
                    if (item.IsMarket) { }
                    if (item.IsProduct) { detail.ProductID = type.GetProperty("SummaryType" + index).GetValue(summary) + ""; index++; }
                    Lines.Add(detail);
                });
                #region 包装钻取物品结算汇总表接口的必备参数
                if (item.Extends?.Count > 0)
                {
                    item.Extends?.ForEach(extend =>
                    {
                        switch (extend.Sort)
                        {
                            case (int)SortTypeEnum.物品分类:
                                if (suppliesList.FindIndex(s => s.id == extend.Object) < 0)//去重的作用
                                {
                                    suppliesList.Add(new DictionaryModel()
                                    {
                                        id = extend.Object,
                                        name = extend.ObjectName,
                                        rank = 1
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
            string suppliesStr = string.Empty;
            string suppliersStr = string.Empty;
            if (suppliesList.Count > 0)//物品分类筛选条件
            {
                suppliesStr = JsonConvert.SerializeObject(suppliesList.Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "物品分类", Value = string.Join(',', suppliesList.Select(s => s.name)) });
            }
            if (suppliers.Count > 0)//供应商筛选条件
            {
                suppliersStr = string.Join(',', suppliers.Select(s => s.id).Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "供应商", Value = string.Join(',', suppliers.Select(s => s.name)) });
            }
            domain.ProductLst = suppliesStr + "~" + suppliersStr;
            domain.RptSearchText = JsonConvert.SerializeObject(rpts);
            domain.Lines = Lines;
            return domain;
        }

        private List<EstimationSummaryModelForDataResult> GetSuppliesEstimationSummaryDataList(FM_CarryForwardVoucherODataEntity model, FM_CarryForwardVoucherInterfaceSearchCommand request, FM_CarryForwardVoucherDetailODataEntity item, FD_SettleReceipt domain, bool isBreak, List<RptSearchModel> rpts)
        {
            try
            {
                #region 构造接口参数
                SuppliesModelForRequeset suppliesEstimationrequest = new SuppliesModelForRequeset()
                {
                    Begindate = request.Begindate,Boid = request.Boid,CanWatchEntes = new List<string>() { request.EnterpriseID },CunHuoFenLei = null,CustomerCategory = null,CustomerCategory_id = null,
                    DataSource = 0,DateRules = null,DepartList = null,DepartTreeInputName = null,Descending = false,Enddate = request.Enddate,EnteID = Convert.ToInt64(request.EnterpriseID),EnteNameLst = null,
                    EnterpriseList = null,GiveCustomer = null,GroupID = Convert.ToInt64(request.GroupID),HasReportAuth = false,IsAudit = 0,IsDepartTreeCate = null,IsOpenAddWeight = "0",
                    IsOpenEndMoneyOption = "0",MenuParttern = "1",OnlyCombineEnte = false,OutInWarehouseType = model.TransferAccountsAbstract,OutInprojectSelectListType = null,OutInprojectSelectListTypeTwo = null,OwnEntes = new List<string>() { request.Boid },
                    Owners = null,ProductCode = null,ProductTreeLst = null,ProductTreeName = null,StoreType = null,SummarySortField = null,TicketedPointSelectList = model.TicketedPointID,WarehouseIdAddress = null,
                };
                List<DictionaryModel> suppliesList = new List<DictionaryModel>();
                string SupplierName_id = string.Empty;
                if (item.Extends?.Count > 0)
                {
                    item.Extends?.ForEach(extend =>
                    {
                        switch (extend.Sort)
                        {
                            case (int)SortTypeEnum.物品分类:
                                suppliesList.Add(new DictionaryModel()
                                {
                                    id = extend.Object,
                                    rank = 1
                                });
                                break;
                            case (int)SortTypeEnum.供应商:
                                SupplierName_id += (extend.Object + ",");
                                break;
                            default:
                                break;
                        }
                    });
                    suppliesEstimationrequest.ProductLst = JsonConvert.SerializeObject(suppliesList);
                    suppliesEstimationrequest.SupplierName_id = SupplierName_id;
                }
                if (item.IsPerson)
                {
                    suppliesEstimationrequest.SummaryType1 = "per.Name";
                    if (isBreak) rpts.Add(new RptSearchModel() { Text = "汇总方式一", Value = "经办人" });
                }
                if (item.IsCustomer)
                {
                    if (string.IsNullOrEmpty(suppliesEstimationrequest.SummaryType1))
                    {
                        suppliesEstimationrequest.SummaryType1 = "a.SupplierID";
                        if (isBreak) rpts.Add(new RptSearchModel() { Text = "汇总方式一", Value = "供应商" });
                    }
                    else
                    {
                        suppliesEstimationrequest.SummaryType2 = "a.SupplierID";
                        if (isBreak) rpts.Add(new RptSearchModel() { Text = "汇总方式二", Value = "供应商" });
                    }
                }
                if (item.IsMarket)//因为没有部门汇总方式，所以不做处理
                {

                }
                if (item.IsProduct)
                {
                    bool isSet = false;
                    if (string.IsNullOrEmpty(suppliesEstimationrequest.SummaryType1) && !isSet)
                    {
                        suppliesEstimationrequest.SummaryType1 = "b.SuppliesID";
                        if (isBreak) rpts.Add(new RptSearchModel() { Text = "汇总方式一", Value = "物品名称" });
                        isSet = true;
                    }
                    if (string.IsNullOrEmpty(suppliesEstimationrequest.SummaryType2) && !isSet)
                    {
                        suppliesEstimationrequest.SummaryType2 = "b.SuppliesID";
                        if (isBreak) rpts.Add(new RptSearchModel() { Text = "汇总方式二", Value = "物品名称" });
                        isSet = true;
                    }
                    if (string.IsNullOrEmpty(suppliesEstimationrequest.SummaryType3) && !isSet)
                    {
                        suppliesEstimationrequest.SummaryType3 = "b.SuppliesID";
                        if (isBreak) rpts.Add(new RptSearchModel() { Text = "汇总方式三", Value = "物品名称" });
                        isSet = true;
                    }
                }
                if (!item.IsPerson && !item.IsCustomer && !item.IsProduct)
                {
                    suppliesEstimationrequest.SummaryType1 = "a.EnterpriseID";
                    if (isBreak) rpts.Add(new RptSearchModel() { Text = "汇总方式一", Value = "单位" });
                }
                #endregion
                if (isBreak)
                {
                    domain.SummaryType = suppliesEstimationrequest.SummaryType1 + "~" + suppliesEstimationrequest.SummaryType2 + "~" + suppliesEstimationrequest.SummaryType3;
                    domain.SummaryTypeName = null;//钻取物品结算汇总表可不传，所以可以为空
                }
                ResultModel<EstimationSummaryModelForDataResult> result = base.postActionByUrl<ResultModel<EstimationSummaryModelForDataResult>, SuppliesModelForRequeset>(suppliesEstimationrequest);
                return result.Code == 0 ? result.Data : new List<EstimationSummaryModelForDataResult>();
            }
            catch (Exception ex)
            {
                return new List<EstimationSummaryModelForDataResult>();
            }
        }
        public Dictionary<string, string> GetDict()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("1911081429200000401", "SuppliesAmount");
            value.Add("1911081429200000402", "DiffAmount");
            value.Add("1911081429200000403", "InvoiceAmount");
            return value;
        }
    }
    public class EstimationSummaryModelForDataResult
    {
        #region 小计 及 汇总 保留字段
        #region 小计专用字段
        public double iOrder { get; set; }
        public bool IsSubTotal { get; set; }
        /// 给单位口径用
        /// </summary>
        public string SortId { get; set; }
        public string iRank { get; set; }
        #endregion
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
        #endregion 小计 及 汇总 保留字段

        public decimal SuppliesAmount { get; set; }
        public decimal SuppliesQuantity { get; set; }

        public decimal InvoiceQuantity { get; set; }
        public decimal InvoiceAmount { get; set; }

        public decimal DiffQuantity { get; set; }
        public decimal DiffAmount { get; set; }
    }

}
