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
    /// 折旧汇总表
    /// </summary>
    public class DepreciationCommon : MakeVoucherBase
    {

        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        public DepreciationCommon(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration) : base(httpClientUtil, hostCongfiguration)
        {
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil = httpClientUtil;
        }
        public override FD_SettleReceipt getVoucherList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, AuthenticationHeaderValue token)
        {
            #region 构造销售汇总表查询条件  别问我为什么。产品非得要
            List<RptSearchModel> rpts = new List<RptSearchModel>() {
             new RptSearchModel(){Text="查询时间",Value=Convert.ToDateTime(request.Enddate).ToString("yyyy-MM") },
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
            reqUrl = "http://demo.s.qlw.nxin.com/api/AssetsCardDepreciationSummaryReport/GetAssetsCardDepreciationSummaryReport";
            Dictionary<string, string> dictList = GetDict();
            List<DictionaryModel> assetsClass = new List<DictionaryModel>();
            List<DictionaryModel> markets = new List<DictionaryModel>();
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
                List<AssetsCardDepreciationSummary> depreciations = GetDepreciationDataList(model, request, item, domain, isBreak);//获取折旧汇总表数据
                depreciations?.ForEach(summary =>
                {
                    FD_SettleReceiptDetailCommand detail = base.NewObject<AssetsCardDepreciationSummary>(summary, item, isDebit, formular, dictList);
                    if (item.IsMarket) detail.MarketID = summary.SummaryType1;
                    if (item.IsSum) detail.EnterpriseID =request.EnterpriseID;
                    Lines.Add(detail);
                });
                #region 包装钻取折旧汇总表接口的必备参数
                if (item.Extends?.Count > 0)
                {
                    var productList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.资产类别).Select(s => s).ToList();
                    productList.ForEach(s =>
                    {
                        if (assetsClass.FindIndex(_ => _.id == s.Object) < 0)
                        {
                            assetsClass.Add(new DictionaryModel()
                            {
                                id = s.Object,
                                name = s.ObjectName,
                                rank = 2
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
            string asstesClassStr = string.Empty;
            string marketStr = string.Empty;
            if (assetsClass.Count > 0)//资产类别筛选条件
            {
                asstesClassStr = JsonConvert.SerializeObject(assetsClass.Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "资产类别", Value = string.Join(',', assetsClass.Select(s => s.name)) });
            }
            if (markets.Count > 0)//部门
            {
                marketStr = string.Join(',', markets.Select(s => s.id).Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "部门", Value = string.Join(',', markets.Select(s => s.name)) });
            }
            domain.ProductLst = asstesClassStr + "~" + markets ;
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

        private List<AssetsCardDepreciationSummary> GetDepreciationDataList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, FD_SettleReceipt domain, bool isBreak)
        {
            try
            {
                string SummaryType = string.Empty;
                string SummaryTypeName = string.Empty;
                //SELECT MAX(Rank) Rank FROM qlw_nxin_com.biz_market WHERE EnterpriseID=@EnterpriseID
                List<Biz_Market> marketList = GetMarket(request.EnterpriseID);
                int maxMarket = marketList.Max(s => s.rank);
                #region 构造接口参数
                //资产类别条件
                var assetsClassificationList = getFA_AssetsClassificationData(request.GroupID);
                List<DictionaryModel> products = new List<DictionaryModel>();
                var productList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.资产类别).Select(s => s.Object).ToList();
                productList.ForEach(s =>
                {
                    var assetsClassification = assetsClassificationList.Where(a => a.ClassificationID == s).FirstOrDefault();
                    products.Add(new DictionaryModel()
                    {
                        id = assetsClassification?.ClassificationID,
                        rank = (int)assetsClassification?.Rank
                    });
                });
                var data = new {
                    BeginDate = Convert.ToDateTime(request.Enddate).ToString("yyyy-MM"),
                    EndDate = Convert.ToDateTime(request.Enddate).ToString("yyyy-MM"),
                    MenuParttern = 1,
                    EnteID = request.EnterpriseID,
                    GroupID = request.GroupID,
                    MarketName_id = string.Join(',', item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.部门).Select(s => s.Object).ToList()), 
                    SummaryType1 = @$"bumen{maxMarket}ji",
                    ProductLst = JsonConvert.SerializeObject(products)
                 };
                #endregion
                if (isBreak)
                {
                    domain.SummaryType = @$"bumen{maxMarket}ji";
                    domain.SummaryTypeName = "部门三级";
                }
                ResultModel<AssetsCardDepreciationSummary> result = base.postActionByUrl<ResultModel<AssetsCardDepreciationSummary>, object>(data);
                List<AssetsCardDepreciationSummary> resultList= result.ResultState? result.Data : new List<AssetsCardDepreciationSummary>();
                if (!item.IsMarket)
                {
                    return new List<AssetsCardDepreciationSummary>()
                    {
                        new AssetsCardDepreciationSummary()
                        {
                             DepreciationMonthAmount=(decimal)resultList?.Sum(s=>s.DepreciationMonthAmount)
                        }
                    };
                }
                else
                {
                    return resultList;
                }
            }
            catch (Exception ex)
            {
                return new List<AssetsCardDepreciationSummary>();
            }
        }
        public Dictionary<string, string> GetDict()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("2201101409320000152", "DepreciationMonthAmount");//本期折旧
            return value;
        }
        /// <summary>
        /// 获取资产类别
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<FA_AssetsClassification> getFA_AssetsClassificationData(string GroupID)
        {
            try
            {
                List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
                var result = _httpClientUtil.PostJsonAsync<ResultModel<FA_AssetsClassification>>($"{_hostCongfiguration._rdUrl}/api/DBNFA_AssetsClassification/GetAllData", new { EnterpriseID = GroupID }).Result;
                if (result.Code == 0)
                {
                    return result.Data;
                }
                return new List<FA_AssetsClassification>();
            }
            catch (Exception ex)
            {
                return new List<FA_AssetsClassification>();
            }
        }

        public List<Biz_Market> GetMarket(string EnterpriseID)
        {
            try
            {
                List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
                var param = new
                {
                    EnterpriseIds = EnterpriseID
                };
                var resultModel = _httpClientUtil.PostJsonAsync<ResultModel<Biz_Market>>($"{_hostCongfiguration.NxinGatewayUrl}api/nxin.qlwbase.getmarket/3.0", param).Result;
                if (resultModel.Code == 0)
                {
                    return resultModel.Data;
                }
                else
                {
                    return new List<Biz_Market>();
                }
            }
            catch (Exception ex)
            {
                return new List<Biz_Market>();
            }
        }

    }

}
