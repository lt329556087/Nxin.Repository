using Architecture.Common.HttpClientUtil;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FinanceManagement.Common.MonthEndCheckout;

namespace FinanceManagement.Common.NewMakeVoucherCommon
{
    /// <summary>
    /// 物品结转
    /// </summary>
    public class SuppliesSummaryCommon : MakeVoucherBase
    {
        private const string _systemOptionWeightingAvg = "20190319132324";// 加权平均
        private const string _systemOptionOpeningSettleAccounts = "20181213092354";// 启用物品结账
        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        public SuppliesSummaryCommon(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration) : base(httpClientUtil, hostCongfiguration)
        {
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil = httpClientUtil;
        }
        public override FD_SettleReceipt getVoucherList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, AuthenticationHeaderValue token)
        {
            #region 构造物品明细表查询条件  别问我为什么。产品非得要
            List<RptSearchModel> rpts = new List<RptSearchModel>() {
             new RptSearchModel(){Text="查询时间",Value=request.Begindate+"-"+request.Enddate},
             new RptSearchModel(){Text="单位筛选",Value=model.EnterpriseName},
             new RptSearchModel(){Text="核算单元",Value=model.TicketedPointName},
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
            List<DictionaryModel> suppliesTypes = new List<DictionaryModel>();
            List<DictionaryModel> outInTypes = new List<DictionaryModel>();
            List<DictionaryModel> markets = new List<DictionaryModel>();
            reqUrl = _hostCongfiguration.QlwServiceHost + "/api/FDGoodsPurchasingSummaryRelated/GetSalesSummaryReport";
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
                List<SuppliesModelForDataResult> suppliesSummarys = GetSuppliesSummaryDataList(model, request, item, domain, isBreak);//获取物品明细表数据
                suppliesSummarys?.ForEach(summary =>
                {
                    FD_SettleReceiptDetailCommand detail = base.NewObject<SuppliesModelForDataResult>(summary, item, isDebit, formular, dictList);
                    if (item.IsCustomer) detail.CustomerID =summary.SupplierID;
                    if (item.IsMarket) detail.MarketID = summary.MarketID;
                    if (item.IsSum) detail.EnterpriseID = request.EnterpriseID;
                    Lines.Add(detail);
                });
                #region 包装钻取销售汇总表接口的必备参数
                if (item.Extends?.Count > 0)
                {
                    var productList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.物品分类).Select(s => s).ToList();
                    productList.ForEach(s =>
                    {
                        if (suppliesTypes.FindIndex(_ => _.id == s.Object) < 0)
                        {
                            suppliesTypes.Add(new DictionaryModel()
                            {
                                id = s.Object,
                                name = s.ObjectName,
                                rank = 2
                            });
                        }
                    });
                    var SuppliesAbsList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.出入库方式).Select(s => s).ToList();
                    SuppliesAbsList.ForEach(s =>
                    {
                        if (outInTypes.FindIndex(_ => _.id == s.Object) < 0)
                        {
                            outInTypes.Add(new DictionaryModel()
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

            string suppliesTypeStr = string.Empty;
            string outInTypeStr = string.Empty;
            string marketStr = string.Empty;
            if (suppliesTypes.Count > 0)//物品分类筛选条件
            {
                suppliesTypeStr = JsonConvert.SerializeObject(suppliesTypes.Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "物品类别", Value = string.Join(',', suppliesTypes.Select(s => s.name)) });
            }
            if (outInTypes.Count > 0)//出入库方式
            {
                outInTypeStr = string.Join(',', outInTypes.Select(s => s.id).Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "出入库方式", Value = string.Join(',', outInTypes.Select(s => s.name)) });
            }
            if (markets.Count > 0)//部门
            {
                marketStr = string.Join(',', markets.Select(s => s.id).Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "部门", Value = string.Join(',', markets.Select(s => s.name)) });
            }
            domain.ProductLst = suppliesTypeStr + "~" + outInTypeStr + "~" + marketStr;
            
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

        private List<SuppliesModelForDataResult> GetSuppliesSummaryDataList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, FD_SettleReceipt domain, bool isBreak)
        {
            try
            {
                string SummaryType = string.Empty;
                string SummaryTypeName = string.Empty;
                #region 构造接口参数
                SuppliesModelForRequeset param = new SuppliesModelForRequeset()
                {
                    Begindate = request.Begindate,
                    Boid = request.Boid,
                    CanWatchEntes = new List<string>() { request.EnterpriseID },
                    DataSource = 0,
                    Enddate = request.Enddate,
                    EnteID = Convert.ToInt64(request.EnterpriseID),
                    EnterpriseList = "|" + request.EnterpriseID,
                    GroupID = Convert.ToInt64(request.GroupID),
                    IsAudit = 0,
                    IsOpenAddWeight = OptionConfigValue(_systemOptionWeightingAvg, request.EnterpriseID, request.GroupID) == true ? "1" : "0",//系统选项：启用物品出库金额用“全月加权平均法”核算       20190319132324
                    IsOpenEndMoneyOption = OptionConfigValue(_systemOptionOpeningSettleAccounts, request.EnterpriseID, request.GroupID) == true ? "1" : "0",//系统选项：启用物品结账    ID：20181213092354
                    IsOpenFavor = OptionConfigValue("20181112155405", request.EnterpriseID, request.GroupID),
                    IsQueryMonthFinishData = false,
                    IsToDetail = true,
                    IsUseNewMethod = false,
                    MenuParttern = "1",
                    OwnEntes = new List<string>() { model.EnterpriseID.ToString() },
                    DepartList = string.Join(',', item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.部门).Select(s => s.Object)),
                    OutInWarehouseType = string.Join(',', item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.出入库方式).Select(s => s.Object)),
                    TicketedPointSelectList = model.TicketedPointID,
                };
                //物品分类过滤条件
                List<DictionaryModel> products = new List<DictionaryModel>();
                var productList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.物品分类).Select(s => s.Object).ToList();
                productList.ForEach(s =>
                {
                    products.Add(new DictionaryModel()
                    {
                        id = s,
                        rank = 1
                    });
                });
                param.ProductLst = JsonConvert.SerializeObject(products);
                #endregion
                List<SuppliesModelForDataResult> result = base.postActionByUrl<ResultModel<SuppliesModelForDataResult>, SuppliesModelForRequeset>(param).Data;
                if (result == null)
                {
                    return new List<SuppliesModelForDataResult>();
                }
                result = QueryDataOperation(result);
                if (item.IsSum)
                {
                    List<SuppliesModelForDataResult> list = new List<SuppliesModelForDataResult>()
                    {
                     new SuppliesModelForDataResult()
                     {
                        InQuantity = result.Sum(n => n.InQuantity),
                        InAmount = result.Sum(n => n.InAmount),
                        OutQuantity = result.Sum(n => n.OutQuantity),
                        OutAmount = result.Sum(n => n.OutAmount),
                        qcAmount = result.Sum(n => n.qcAmount),
                        qcQuantity = result.Sum(n => n.qcQuantity),
                     }
                    };
                    return list;
                }
                SuppliesGroupModel groupModel = new SuppliesGroupModel();
                if (item.IsMarket)
                {
                    groupModel.MarketId = true;

                }
                if (item.IsCustomer)
                {
                    groupModel.SupplierID = true;
                }
                result=result.GroupBy(s => groupModel.GroupBy(s)).Select(s => new SuppliesModelForDataResult
                {

                    MarketID = s.Key.MarketId,
                    SupplierID = s.Key.SupplierID,
                    InQuantity = s.Sum(n => n.InQuantity),
                    InAmount = s.Sum(n => n.InAmount),
                    OutQuantity = s.Sum(n => n.OutQuantity),
                    OutAmount = s.Sum(n => n.OutAmount),
                    qcAmount = s.Sum(n => n.qcAmount),
                    qcQuantity = s.Sum(n => n.qcQuantity),
                }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                return new List<SuppliesModelForDataResult>();
            }
        }
        public Dictionary<string, string> GetDict()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("2205141057480000102", "InAmount");//入库金额
            value.Add("2205141057480000103", "OutAmount");//出库金额
            return value;
        }
        /// <summary>
        /// 获取系统选项
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public bool OptionConfigValue(string optionId, string enterpriseId = "", string GroupID = "")
        {
            try
            {
                if (enterpriseId == "")
                {
                    return false;
                }
                BIZ_OptionConfig optionConfig = null;
                Task<ResultModel> resultModel = _httpClientUtil.PostJsonAsync<ResultModel>($"{_hostCongfiguration.QlwServiceHost}/api/BIZOptionConfig/getOptionConfig", new { OptionID = optionId, EnterpriseID = enterpriseId, GroupID = GroupID });
                if (resultModel != null && resultModel.Result.ResultState && resultModel.Result.Data != null) optionConfig = JsonConvert.DeserializeObject<List<BIZ_OptionConfig>>(resultModel.Result.Data.ToString()).FirstOrDefault();
                string result = optionConfig != null ? optionConfig.OptionValue : string.Empty;
                bool opt = result == "1" ? true : false;
                return opt;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 根据页面要显示的格式进行整理
        /// </summary>
        /// <param name="dataRes"></param>
        /// <returns></returns>
        public List<SuppliesModelForDataResult> QueryDataOperation(List<SuppliesModelForDataResult> dataRes)
        {
            if (dataRes != null)
            {
                //   cAbstract    期初结存
                var item = dataRes.Where(n => n.cAbstract == "期初结存").FirstOrDefault();
                if (item == null)
                {
                    item = new SuppliesModelForDataResult();
                }
                //修改前的 获取到除 期初结存外的所有数据 修改 为 排除 除初期结存的 和初期的 begin
                //var otherItem = dataRes.Where(n => n.cAbstract != "期初结存").ToList();
                //修改前的 获取到除 期初结存外的所有数据 修改 为 排除 除初期结存的 和初期的 end

                //修改后的 获取到除 期初结存外的所有数据 修改 为 排除 除初期结存的 和初期的 并且 通过日期和 单据号排序 begin 添加的
                var qcItem = dataRes.Where(n => (n.cDictCode == "1711221553430000102" || n.isQcDataBool) && n.cAbstract != "期初结存").ToList();
                var otherItem = dataRes.Where(n => n.cAbstract != "期初结存" && n.cDictCode != "1711221553430000102").OrderBy(n => n.dDate).ThenBy(n => n.iNum).ToList();

                // 需求：物资明细小数点差异调整，以某一物品及id唯一标识判断，当同一物品的出库完时，即入库数量=出库数量合计时，入库金额-出库金额=差异，差异调整出库金额，调整后出库金额=调整前出库金额-差异
                //1 step: 根据 出库信息筛选出 出库的明细流水号和出库的数量
                var outWarehouseItem = otherItem.Where(w => !(string.IsNullOrEmpty(w.NumericalOrderInput) || w.NumericalOrderInput == "0")).GroupBy(n => n.NumericalOrderInput).Select(g => new SuppliesModelForDataResult { NumericalOrderInput = g.Key, InOutQuantity = g.Sum(s => Math.Abs(s.InOutQuantity)), InOutAmount = g.Sum(a => a.InOutAmount) });
                //2 step: 根据 入库信息筛选出 入库的明细流水号和入库的数量
                var inWarehouseItem = otherItem.Where(w => (string.IsNullOrEmpty(w.NumericalOrderInput) || w.NumericalOrderInput == "0")).Select(g => new SuppliesModelForDataResult { NumericalOrderDetail = g.NumericalOrderDetail, InOutQuantity = g.InOutQuantity, InOutAmount = g.InOutAmount });
                //3 step: 得出 已出完库的 明细 并获取差异调整出库金额
                var outFinishItem = (from i in inWarehouseItem
                                     from o in outWarehouseItem
                                     where (i.NumericalOrderDetail != "0" && i.NumericalOrderDetail == o.NumericalOrderInput && i.InOutQuantity == o.InOutQuantity)
                                     select new SuppliesModelForDataResult { NumericalOrderInput = o.NumericalOrderInput, InOutQuantity = o.InOutQuantity, DifferencePrice = (i.InOutAmount + o.InOutAmount) }).ToList();
                //4. step: 调整出库金额，调整后出库金额=调整前出库金额-差异
                outFinishItem.ForEach(f =>
                {
                    var lastItem = otherItem.Where(o => (!(string.IsNullOrEmpty(o.NumericalOrderInput) || o.NumericalOrderInput == "0") && o.NumericalOrderInput == f.NumericalOrderInput)).LastOrDefault();
                    if (lastItem != null)
                    {
                        //delete by jingkun Xu 20190628
                        //lastItem.InOutAmount = lastItem.InOutAmount - f.DifferencePrice;
                    }
                });

                //因为 除 初期的 其他的数据 要根据 时间和 单据号排序， 并且把 期初的 放到最上面
                if (qcItem != null && qcItem.Count > 0)
                {
                    otherItem = qcItem.Union(otherItem).ToList();
                }
                //修改后的 获取到除 期初结存外的所有数据 修改 为 排除 除初期结存的 和初期的 并且 通过日期和 单据号排序 end 添加的

                if (otherItem.Count() == 0)
                {
                    otherItem = new List<SuppliesModelForDataResult>();
                }
                for (int i = 0; i < otherItem.Count(); i++)
                {
                    if (i == 0)
                    {
                        otherItem[i].qcQuantity = (otherItem[i].isQcDataBool ? otherItem[i].qcQuantity : otherItem[i].InOutQuantity) + item.qcQuantity;
                        otherItem[i].qcAmount = (otherItem[i].isQcDataBool ? otherItem[i].qcAmount : otherItem[i].InOutAmount) + item.qcAmount;
                    }
                    else
                    {
                        //期末结存 数量
                        otherItem[i].qcQuantity = otherItem[i - 1].qcQuantity + otherItem[i].InOutQuantity;

                        //期末结存 金额
                        otherItem[i].qcAmount = otherItem[i - 1].qcAmount + otherItem[i].InOutAmount;
                    }
                }

                //若 存在 初期的 数据， 要把 期初结存 这条数据删掉 begin 添加的
                if (qcItem != null && qcItem.Count > 0)
                {
                    dataRes.RemoveRange(0, dataRes.Count);
                    dataRes.AddRange(otherItem);
                }
                //若 存在 初期的 数据， 要把 期初结存 这条数据删掉 end  添加的

                if (otherItem.Count > 0)
                {
                    var lastItem = otherItem.LastOrDefault();
                    if (lastItem != null)
                    {
                        if (lastItem.qcQuantity == 0 && lastItem.qcAmount != 0)
                        {
                            lastItem.InOutAmount = lastItem.InOutAmount - lastItem.qcAmount;
                            lastItem.InOutPrice = Math.Abs(lastItem.InOutQuantity) == 0 ? Math.Abs(lastItem.InOutAmount) : (Math.Abs(lastItem.InOutAmount) / Math.Abs(lastItem.InOutQuantity));
                            lastItem.qcAmount = 0;
                        }
                    }
                }
                dataRes = dataRes.OrderBy(n => n.dDate).ThenBy(n => n.iNum).ToList();
                //// 合计
                //if (otherItem.Count > 0)
                //{
                //    dataRes.Add(new SuppliesModelForDataResult
                //    {
                //        dDate = "合计",
                //        InOutQuantity = otherItem.Sum(n => n.InOutQuantity),//decimal.Round(otherItem.Sum(n => n.InOutQuantity), 2, MidpointRounding.AwayFromZero),
                //        InOutAmount = decimal.Round(otherItem.Sum(n => n.InOutAmount), 2, MidpointRounding.AwayFromZero),
                //        qcQuantity = otherItem.Last().qcQuantity,
                //        qcAmount = decimal.Round(otherItem.Last().qcAmount, 2, MidpointRounding.AwayFromZero)
                //    });
                //}
            }
            dataRes?.ForEach(s =>
            {
                if (s.cDictCode == "1711221550310000101" ||
                    s.cDictCode == "1711221553430000102" ||
                    s.cDictCode == "1711221553430000103" ||
                    s.cDictCode == "1711221553430000104" ||
                    s.cDictCode == "1711221553430000105" ||
                    s.cDictCode == "1711221553430000106" ||
                    s.cDictCode == "1711221553430000107" ||
                    s.cDictCode == "201512172084559185")
                {
                    s.InQuantity = s.InOutQuantity;
                    s.InAmount = s.InOutAmount;
                }
                else
                {
                    s.OutQuantity = Math.Abs(s.InOutQuantity);
                    s.OutAmount = Math.Abs(s.InOutAmount);
                }
                s.InQuantity = Math.Round(s.InQuantity, 4);
                s.InAmount = Math.Round(s.InAmount, 4);
                s.OutQuantity = Math.Round(s.OutQuantity, 4);
                s.OutAmount = Math.Round(s.OutAmount, 4);
                s.qcAmount = Math.Round(s.qcAmount, 4);
                s.qcQuantity = Math.Round(s.qcQuantity, 4);
            });
            return dataRes;
        }
    }
}
