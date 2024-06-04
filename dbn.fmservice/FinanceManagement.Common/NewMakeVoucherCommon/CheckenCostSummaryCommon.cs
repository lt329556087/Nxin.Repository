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
    public class CheckenCostSummaryCommon : MakeVoucherBase
    {

        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        public CheckenCostSummaryCommon(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration) : base(httpClientUtil, hostCongfiguration)
        {
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil = httpClientUtil;
        }
        public override FD_SettleReceipt getVoucherList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, AuthenticationHeaderValue token)
        {
            #region 构造汇总表查询条件  别问我为什么。产品非得要
            List<RptSearchModel> rpts = new List<RptSearchModel>() {
             new RptSearchModel(){Text="查询时间",Value=request.DataDate+"-"+request.DataDate},
             new RptSearchModel(){Text="报表显示",Value="详细查询"},
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
            List<DictionaryModel> checkenList = new List<DictionaryModel>();
            List<DictionaryModel> changquList = new List<DictionaryModel>();
            List<DictionaryModel> jisheList = new List<DictionaryModel>();
            List<DictionaryModel> breedList = new List<DictionaryModel>();
            List<DictionaryModel> batchList = new List<DictionaryModel>();
            reqUrl = _hostCongfiguration.QlwServiceHost + "/api/EGGFMCostChangesSummary/GetReportData";
            Dictionary<string, string> dictList = GetDict();
            bool isBreak = true;
            foreach (var item in model.Lines)
            {
                List<CheckenSummaryResultData> checkenSummarys = GetCheckenSummaryDataList(model, request, item, domain, isBreak, rpts);//获取禽成本流转汇总表数据
                bool isDebit = false;
                string formular = string.Empty;
                if (!string.IsNullOrEmpty(item.DebitSecFormula) && item.DebitSecFormula != "0") { isDebit = true; formular = item.DebitSecFormula; }
                if (!string.IsNullOrEmpty(item.CreditSecFormula) && item.CreditSecFormula != "0") { isDebit = false; formular = item.CreditSecFormula; }
                checkenSummarys?.ForEach(summary =>
                {
                    Type type = summary.GetType();
                    FD_SettleReceiptDetailCommand detail = base.NewObject<CheckenSummaryResultData>(summary, item, isDebit, formular, dictList);
                    if (item.IsPerson) { }
                    if (item.IsCustomer) { }
                    if (item.IsMarket) { }
                    if (item.IsProduct) { detail.ProductID = summary.SummaryType; }
                    Lines.Add(detail);
                });
                #region 包装钻取汇总表接口的必备参数
                if (item.Extends?.Count > 0)
                {
                    item.Extends?.ForEach(extend =>
                    {
                        switch (extend.Sort)
                        {
                            case (int)SortTypeEnum.鸡场:
                                if (checkenList.FindIndex(s => s.id == extend.Object) < 0)//去重的作用
                                {
                                    checkenList.Add(new DictionaryModel()
                                    {
                                        id = extend.Object,
                                        name = extend.ObjectName,
                                    });
                                }
                                break;
                            case (int)SortTypeEnum.厂区:
                                if (changquList.FindIndex(s => s.id == extend.Object) < 0)//去重的作用
                                {
                                    changquList.Add(new DictionaryModel()
                                    {
                                        id = extend.Object,
                                        name = extend.ObjectName,
                                    });
                                }
                                break;
                            case (int)SortTypeEnum.鸡舍:
                                if (jisheList.FindIndex(s => s.id == extend.Object) < 0)//去重的作用
                                {
                                    jisheList.Add(new DictionaryModel()
                                    {
                                        id = extend.Object,
                                        name = extend.ObjectName,
                                    });
                                }
                                break;

                            case (int)SortTypeEnum.品种:
                                if (breedList.FindIndex(s => s.id == extend.Object) < 0)//去重的作用
                                {
                                    breedList.Add(new DictionaryModel()
                                    {
                                        id = extend.Object,
                                        name = extend.ObjectName,
                                    });
                                }
                                break;
                            case (int)SortTypeEnum.批次:
                                if (batchList.FindIndex(s => s.id == extend.Object) < 0)//去重的作用
                                {
                                    batchList.Add(new DictionaryModel()
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
            string checkensStr = string.Empty;
            string batchStr = string.Empty;
            string breedStr = string.Empty;
            if (checkenList.Count > 0 || changquList.Count > 0 || jisheList.Count > 0)//鸡场筛选条件
            {
                string str1 = string.Empty;
                string strname1 = string.Empty;
                string str2 = string.Empty;
                string strname2 = string.Empty;
                string str3 = string.Empty;
                string strname3 = string.Empty;
                if (checkenList.Count > 0)
                {
                    str1 = string.Join(',', checkenList.Select(s => s.id).Distinct().ToList());
                    strname1 = string.Join(',', checkenList.Select(s => s.name).Distinct().ToList());
                }
                if (changquList.Count > 0)
                {
                    str2 = string.Join(',', changquList.Select(s => s.id).Distinct().ToList());
                    strname2 = string.Join(',', changquList.Select(s => s.name).Distinct().ToList());
                }
                if (jisheList.Count > 0)
                {
                    str3 = string.Join(',', jisheList.Select(s => s.id).Distinct().ToList());
                    strname3 = string.Join(',', jisheList.Select(s => s.name).Distinct().ToList());
                }
                checkensStr = (string.IsNullOrEmpty(str1) ? "" : str1 + ",") + "|" + (string.IsNullOrEmpty(str2) ? "" : str2 + ",") + "|" + (string.IsNullOrEmpty(str3) ? "" : str3 + ",");
                rpts.Add(new RptSearchModel() { Text = "场区栋", Value = string.Join(',', (string.IsNullOrEmpty(strname1) ? "" : strname1 + ",") + (string.IsNullOrEmpty(strname2) ? "" : strname2 + ",") + (string.IsNullOrEmpty(strname3) ? "" : strname3 + ",")) });
            }
            if (batchList.Count > 0)//批次筛选条件
            {
                batchStr = string.Join(',', batchList.Select(s => s.id).Distinct().ToList()) + ",||";
                rpts.Add(new RptSearchModel() { Text = "批次", Value = string.Join(',', batchList.Select(s => s.name).Distinct().ToList()) });
            }
            if (breedList.Count > 0)//品种筛选条件
            {
                breedStr = string.Join(',', breedList.Select(s => s.id).Distinct().ToList()) + ",";
                rpts.Add(new RptSearchModel() { Text = "品种", Value = string.Join(',', breedList.Select(s => s.name).Distinct().ToList()) });
            }
            domain.ProductLst = checkensStr + "~" + batchStr + "~" + breedStr;
            rpts.Add(new RptSearchModel() { Text = "显示项", Value = domain.SummaryTypeName });
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

        private List<CheckenSummaryResultData> GetCheckenSummaryDataList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, FD_SettleReceipt domain, bool isBreak, List<RptSearchModel> rpts)
        {
            try
            {
                #region 构造接口参数
                CheckenSummaryRequest checkenrequest = new CheckenSummaryRequest()
                {
                    BeginDate = request.DataDate,
                    EndDate = request.DataDate,
                    TableShow = "2",
                    EnterpriseID = request.EnterpriseID,
                };
                string checken_ids = string.Empty;
                string changqu_ids = string.Empty;
                string jishe_ids = string.Empty;
                string breed_ids = string.Empty;
                string batch_ids = string.Empty;
                if (item.Extends?.Count > 0)
                {
                    item.Extends?.ForEach(extend =>
                    {
                        switch (extend.Sort)
                        {
                            case (int)SortTypeEnum.鸡场:
                                checken_ids += (extend.Object + ",");
                                break;
                            case (int)SortTypeEnum.厂区:
                                changqu_ids += (extend.Object + ",");
                                break;
                            case (int)SortTypeEnum.鸡舍:
                                jishe_ids += (extend.Object + ",");
                                break;
                            case (int)SortTypeEnum.品种:
                                breed_ids += (extend.Object + ",");
                                break;
                            case (int)SortTypeEnum.批次:
                                batch_ids += (extend.Object + ",");
                                break;
                            default:
                                break;
                        }
                    });
                    if (!string.IsNullOrEmpty(checken_ids) || !string.IsNullOrEmpty(changqu_ids) || !string.IsNullOrEmpty(jishe_ids)) checkenrequest.ChickenHouseID = checken_ids + "|" + changqu_ids + "|" + jishe_ids;
                    if (!string.IsNullOrEmpty(breed_ids)) checkenrequest.BreedingID = breed_ids;
                    if (!string.IsNullOrEmpty(batch_ids)) checkenrequest.BatchID = batch_ids + "||";
                }
                #region 因为没有人员、客户、部门汇总方式，所以不做处理
                if (item.IsPerson)
                {
                }
                if (item.IsCustomer)
                {
                }
                if (item.IsMarket)
                {
                }
                #endregion 
                if (item.IsProduct)
                {
                    checkenrequest.SummaryType = "t0.ProductID";
                    checkenrequest.SummaryTypeName = "商品代号";

                }
                if (!item.IsPerson && !item.IsCustomer && !item.IsProduct && !item.IsMarket)
                {
                    checkenrequest.SummaryType = "t0.DataDate";
                    checkenrequest.SummaryTypeName = "月份";
                }
                #endregion
                if (isBreak)
                {
                    domain.SummaryType = checkenrequest.SummaryType;
                    domain.SummaryTypeName = checkenrequest.SummaryTypeName;
                }
                ResultModel<CheckenSummaryResultData> result = base.postActionByUrl<ResultModel<CheckenSummaryResultData>, CheckenSummaryRequest>(checkenrequest);
                return result.ResultState == true ? result.Data : new List<CheckenSummaryResultData>();
            }
            catch (Exception ex)
            {
                return new List<CheckenSummaryResultData>();
            }
        }
        public Dictionary<string, string> GetDict()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("1911081430200000501", "InHenCost3");
            value.Add("1911081430200000502", "OutCostPig3");
            value.Add("1911081430200000503", "OutCostPig4");
            value.Add("1911081430200000504", "OutCostPig5");
            value.Add("1911081430200000505", "InCostPig3");
            value.Add("1911081430200000506", "InHenCost7");
            value.Add("1911081430200000507", "InHenCost4");
            value.Add("1911081430200000508", "OutCostPig3_1");
            value.Add("1911081430200000509", "OutCostPig4_1");
            value.Add("1911081430200000510", "OutCostPig5_1");
            value.Add("1911081430200000511", "InCostPig3_1");
            value.Add("1911081430200000512", "InHenCost4_1");
            value.Add("1911081430200000513", "OutCostChicken21");
            value.Add("1911081430200000514", "OutCostChicken31");
            value.Add("1911081430200000515", "OutCostChicken41");
            value.Add("1911081430200000516", "OutCostChicken51");
            value.Add("1911081430200000517", "InHenCost81");
            value.Add("1911081430200000518", "OutHenCost31");
            value.Add("1911081430200000519", "OutHenCost32");
            value.Add("1911081430200000520", "OutHenCost33");
            value.Add("1911081430200000521", "OutHenCost34");
            value.Add("1911081430200000522", "OutHenCost35");
            value.Add("1911081430200000523", "OutHenCost3");
            value.Add("1911081430200000524", "OutCostChicken1");
            value.Add("1911081430200000525", "OutCostChicken2");
            value.Add("1911081430200000526", "OutCostChicken3");
            value.Add("1911081430200000527", "OutCostChicken4");
            value.Add("1911081430200000528", "OutCostChicken5");
            value.Add("1911081430200000529", "InHenCost5");
            value.Add("1911081430200000530", "InHenCost6");
            value.Add("1911081430200000531", "OutHenCost81");
            value.Add("1911081430200000532", "OutHenCost82");
            value.Add("1911081430200000533", "OutHenCost83");
            value.Add("1911081430200000534", "OutHenCost84");
            value.Add("1911081430200000535", "OutHenCost85");
            value.Add("1911081430200000536", "OutHenCost8");
            value.Add("1911081430200000537", "OutHenCost113_1");
            value.Add("1911081430200000538", "OutHenCost114_1");
            value.Add("1911081430200000539", "OutHenCost115_1");
            value.Add("1911081430200000540", "InHenCost116_1");
            value.Add("1911081430200000541", "InHenCostSum110_1");
            value.Add("1911081430200000542", "SetStockCost1");
            value.Add("1911081430200000543", "SetStockCost2");
            value.Add("1911081430200000544", "SetStockCost3");
            value.Add("1911081430200000545", "SetStockCost4");
            value.Add("1911081430200000546", "SetStockCost5");
            value.Add("1911081430200000547", "SetStockCost");
            value.Add("1911081430200000548", "SetStockCost");
            value.Add("1911081430200000549", "OutCostPig3_2");
            value.Add("1911081430200000550", "OutCostPig4_2");
            value.Add("1911081430200000551", "OutCostPig5_2");
            value.Add("1911081430200000552", "InCostPig3_2");
            value.Add("1911081430200000553", "InHenCost7");
            value.Add("1911081430200000554", "InHenCost4_2");
            value.Add("1911081430200000555", "InHenCost118");
            value.Add("1911081430200000556", "originalValueReduce");
            value.Add("1911081430200000557", "depreciationAccumulatedReduce");
            value.Add("1911081430200000558", "originalValueSalesReduce");
            value.Add("1911081430200000559", "depreciationAccumulatedSalesReduce");
            return value;
        }

    }
    public class CheckenSummaryResultData
    {
        public string SummaryType { get; set; }
        public string SummaryTypeName { get; set; }
        public string SummaryTypeFieldName { get; set; }

        public decimal InQuantity1Pig5 { get; set; }
        public decimal InCost1Pig5 { get; set; }
        public decimal OriginalValue { get; set; }
        public decimal DepreciationAccumulated { get; set; }
        public decimal InHenQuantity3 { get; set; }
        public decimal InHenCost3 { get; set; }
        public decimal OutCostPig3 { get; set; }
        public decimal OutCostPig4 { get; set; }
        public decimal OutCostPig5 { get; set; }
        public decimal InCostPig3 { get; set; }
        public decimal InHenCost7 { get; set; }
        public decimal InHenCost4 { get; set; }
        public decimal OutCostPig3_1 { get; set; }
        public decimal OutCostPig4_1 { get; set; }
        public decimal OutCostPig5_1 { get; set; }
        public decimal InCostPig3_1 { get; set; }
        public decimal InHenCost4_1 { get; set; }
        public decimal OutCostChicken21 { get; set; }
        public decimal OutCostChicken31 { get; set; }
        public decimal OutCostChicken41 { get; set; }
        public decimal OutCostChicken51 { get; set; }
        public decimal InHenCost81 { get; set; }
        public decimal OutQuantityPig21 { get; set; }
        public decimal OutHenCost31 { get; set; }
        public decimal OutHenCost32 { get; set; }
        public decimal OutHenCost33 { get; set; }
        public decimal OutHenCost34 { get; set; }
        public decimal OutHenCost35 { get; set; }
        public decimal OutHenCost3 { get; set; }
        public decimal OutQuantityChicken1 { get; set; }
        public decimal OutCostChicken1 { get; set; }
        public decimal OutCostChicken2 { get; set; }
        public decimal OutCostChicken3 { get; set; }
        public decimal OutCostChicken4 { get; set; }
        public decimal OutCostChicken5 { get; set; }
        public decimal InHenCost5 { get; set; }
        public decimal InHenQuantity6 { get; set; }
        public decimal InHenCost6 { get; set; }
        public decimal OutHenQuantity8_1 { get; set; }
        public decimal OutHenCost81 { get; set; }
        public decimal OutHenCost82 { get; set; }
        public decimal OutHenCost83 { get; set; }
        public decimal OutHenCost84 { get; set; }
        public decimal OutHenCost85 { get; set; }
        public decimal OutHenCost8 { get; set; }
        public decimal SetStockQuantity { get; set; }
        public decimal SetStockCost1 { get; set; }
        public decimal SetStockCost2 { get; set; }
        public decimal SetStockCost3 { get; set; }
        public decimal SetStockCost4 { get; set; }
        public decimal SetStockCost5 { get; set; }
        public decimal SetStockCost { get; set; }
        public decimal OutCostPig3_2 { get; set; }
        public decimal OutCostPig4_2 { get; set; }
        public decimal OutCostPig5_2 { get; set; }
        public decimal InCostPig3_2 { get; set; }
        public decimal InHenCost4_2 { get; set; }
        public decimal InHenCost118 { get; set; }
        public decimal originalQuantityReduce { get; set; }
        public decimal originalValueReduce { get; set; }
        public decimal depreciationAccumulatedReduce { get; set; }
        public decimal originalQuantitySalesReduce { get; set; }
        public decimal originalValueSalesReduce { get; set; }
        public decimal depreciationAccumulatedSalesReduce { get; set; }
        public decimal OutHenQuantity110 { get; set; }
        public decimal OutHenCost110 { get; set; }
        public decimal endOriginalValue { get; set; }
        public decimal endDepreciationAccumulated { get; set; }
        public decimal endNetValue { get; set; }
        public decimal OutHenCost113_1 { get; set; }
        public decimal OutHenCost114_1 { get; set; }
        public decimal OutHenCost115_1 { get; set; }
        public decimal InHenCost116_1 { get; set; }
        public decimal InHenCostSum110_1 { get; set; }
    }

}
