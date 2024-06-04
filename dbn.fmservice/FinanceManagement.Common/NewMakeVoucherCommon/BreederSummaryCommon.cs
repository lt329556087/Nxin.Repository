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
    /// 养户成本结转
    /// </summary>
    public class BreederSummaryCommon : MakeVoucherBase
    {

        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        public BreederSummaryCommon(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration) : base(httpClientUtil, hostCongfiguration)
        {
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil = httpClientUtil;
        }
        public override FD_SettleReceipt getVoucherList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, AuthenticationHeaderValue token)
        {
            #region 构造养户汇总表查询条件  别问我为什么。产品非得要
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
            List<DictionaryModel> products = new List<DictionaryModel>();
            List<DictionaryModel> markets = new List<DictionaryModel>();
            reqUrl = _hostCongfiguration._wgUrl + "/cost/report/YhPigCostSummary/Finance/Data";
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
                List<BreederSummaryResultModel> breederSummarys = GetBreederSummaryDataList(model, request, item, domain, token, isBreak);//获取养户汇总表数据
                breederSummarys?.ForEach(summary =>
                {
                    FD_SettleReceiptDetailCommand detail = base.NewObject<BreederSummaryResultModel>(summary, item, isDebit, formular, dictList);
                    if (item.IsProduct) detail.ProductID = summary.ProductID;
                    if (item.IsMarket) detail.MarketID = summary.DeptID;
                    if (item.IsSum) detail.EnterpriseID = request.EnterpriseID;
                    Lines.Add(detail);
                });
                #region 包装钻取养户汇总表接口的必备参数
                if (isBreak)
                {
                    if (item.Extends?.Count > 0)
                    {
                        var productList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.商品代号).Select(s => s).ToList();
                        productList.ForEach(s =>
                        {
                            if (products.FindIndex(_ => _.id == s.Object) < 0)
                            {
                                products.Add(new DictionaryModel()
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
                }
                #endregion
                isBreak = false;
            }
            string productStr = string.Empty;
            string marketStr = string.Empty;
            if (products.Count > 0)//商品代号筛选条件
            {
                productStr = string.Join(',', products.Select(s => s.id).Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "商品代号", Value = string.Join(',', products.Select(s => s.name)) });
            }
            if (markets.Count > 0)//部门
            {
                marketStr = string.Join(',', markets.Select(s => s.id).Distinct().ToList());
                rpts.Add(new RptSearchModel() { Text = "部门", Value = string.Join(',', markets.Select(s => s.name)) });
            }
            domain.ProductLst = productStr + "~" + marketStr;
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
        private List<BreederSummaryResultModel> GetBreederSummaryDataList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, FD_SettleReceipt domain, AuthenticationHeaderValue token, bool isBreak)
        {
            try
            {
                #region 构造接口参数
                PigSearchModel pigSearchModel = new PigSearchModel()
                {
                    EndDate = Convert.ToDateTime(request.Enddate),
                    StartDate = Convert.ToDateTime(request.Begindate),
                    ProductIdList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.商品代号).Select(s => s.Object).ToList(),
                    DeptIdList = item.Extends?.Where(s => s.Sort == (int)SortTypeEnum.部门).Select(s => s.Object).ToList(),
                };
                if (item.IsMarket)
                {
                    pigSearchModel.Summary.Add("DeptId");
                    pigSearchModel.SummaryName.Add("部门");
                }
                if (item.IsProduct)
                {
                    pigSearchModel.Summary.Add("ProductId");
                    pigSearchModel.SummaryName.Add("商品代号");
                }
                //防止报错  给个默认值
                if (pigSearchModel.Summary.Count == 0)
                {
                    pigSearchModel.Summary.Add("ProductId");
                    pigSearchModel.SummaryName.Add("商品代号");
                }
                #endregion
                if (isBreak)
                {
                    domain.SummaryType = string.Join(',', pigSearchModel.Summary);
                    domain.SummaryTypeName = string.Join(',', pigSearchModel.SummaryName);
                }
                //https://confluence.nxin.com/pages/viewpage.action?pageId=65050997
                //AuthenticationHeaderValue value = new AuthenticationHeaderValue("Bearer", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJpYXQiOjE2NjkxNjcwMzksImV4cCI6MTY2OTE3NDIzOSwidXNlcl9pZCI6IjIyOTIwNTciLCJlbnRlcnByaXNlX2lkIjoiMzI1NDA1MCIsImdyb3VwX2lkIjoiMzI1NDA1MSIsIm1lbnVfaWQiOiIyMjA4MTgwOTI4MDQwMDAwMTA5IiwiY2hpbGRfZW50ZXJwcmlzZV9pZCI6IjAiLCJwZXJtaXNzaW9ucyI6MzEsImlzcyI6ImlgbCIsImF1ZCI6InFsdy1iYXNlIn0.PUbhroGPCW_Z8qrgvW5QT8v4xzMqdGdB2l4nSmXHolO9H5r8vc-OtvATBr7h6W1orM8Ad8iDTDxmYgiXWf6UtNl_NI8-SVzneRNRQphT5jBXhuIsWfFDrdPS7Z7FX1VjJxjrv2OXbBRUy-wbgbb4st-xHJAPt7KxSXpzLsOqQ-Y5aDy58dgCvxt-JLlvhJeFLhQywlvKOmOsgTN2XJl-vXEyvnnWZX9MDNR2BTMes4wZZRxfXJi4oJr_hXTXhwquzaKqIDIm6Zp0nMKX4mxPZkC5upIuwNMbWWbEdKi_kOfZ4QLWFqMh-OtJGzvhtMGT8q2CgiYj_JwOddsoHJIJyg");
                var result = _httpClientUtil.PostJsonAsync<ResultModel<BreederSummaryResultModel>>(reqUrl, pigSearchModel, (a) => { a.Authorization = token; }).Result.Data;
                if (item.IsSum)
                {
                    #region 数据汇总
                    return new List<BreederSummaryResultModel>()
                    {
                        new BreederSummaryResultModel()
                        {
                            CurrentPigCost=result.Sum(s=>s.CurrentPigCost),
                            CurrentMaterial=result.Sum(s=>s.CurrentMaterial),
                            CurrentMedicant=result.Sum(s=>s.CurrentMedicant),
                            CurrentVaccine=result.Sum(s=>s.CurrentVaccine),
                            CurrentOther=result.Sum(s=>s.CurrentOther),
                            CurrentInnerDeath=result.Sum(s=>s.CurrentInnerDeath),
                            CurrentOuterDeath=result.Sum(s=>s.CurrentOuterDeath),
                            CurrentDirectFee=result.Sum(s=>s.CurrentDirectFee),
                            CurrentBuildingFee=result.Sum(s=>s.CurrentBuildingFee),
                            CurrentPredictFeedFee=result.Sum(s=>s.CurrentPredictFeedFee),
                            CurrentAdjustFeedFee=result.Sum(s=>s.CurrentAdjustFeedFee),
                            CurrentAdditionFee=result.Sum(s=>s.CurrentAdditionFee),
                            CurrentHoldUpFee=result.Sum(s=>s.CurrentHoldUpFee),
                            RecyclePigCost=result.Sum(s=>s.RecyclePigCost),
                            RecycleMaterial=result.Sum(s=>s.RecycleMaterial),
                            RecycleMedicant=result.Sum(s=>s.RecycleMedicant),
                            RecycleVaccine=result.Sum(s=>s.RecycleVaccine),
                            RecycleOther=result.Sum(s=>s.RecycleOther),
                            RecycleInnerDeath=result.Sum(s=>s.RecycleInnerDeath),
                            RecycleOuterDeath=result.Sum(s=>s.RecycleOuterDeath),
                            RecycleDirectFee=result.Sum(s=>s.RecycleDirectFee),
                            RecycleBuildingFee=result.Sum(s=>s.RecycleBuildingFee),
                            RecyclePredictFeedFee=result.Sum(s=>s.RecyclePredictFeedFee),
                            RecycleAdjustFeedFee=result.Sum(s=>s.RecycleAdjustFeedFee),
                            RecycleAdditionFee=result.Sum(s=>s.RecycleAdditionFee),
                            RecycleHoldUpFee=result.Sum(s=>s.RecycleHoldUpFee),
                            DeathPigCost=result.Sum(s=>s.DeathPigCost),
                            DeathMaterial=result.Sum(s=>s.DeathMaterial),
                            DeathMedicant=result.Sum(s=>s.DeathMedicant),
                            DeathVaccine=result.Sum(s=>s.DeathVaccine),
                            DeathOther=result.Sum(s=>s.DeathOther),
                            DeathInnerDeath=result.Sum(s=>s.DeathInnerDeath),
                            DeathOuterDeath=result.Sum(s=>s.DeathOuterDeath),
                            DeathDirectFee=result.Sum(s=>s.DeathDirectFee),
                            DeathBuildingFee=result.Sum(s=>s.DeathBuildingFee),
                            DeathPredictFeedFee=result.Sum(s=>s.DeathPredictFeedFee),
                            DeathAdjustFeedFee=result.Sum(s=>s.DeathAdjustFeedFee),
                            DeathAdditionFee=result.Sum(s=>s.DeathAdditionFee),
                            DeathHoldUpFee=result.Sum(s=>s.DeathHoldUpFee),
                            AdjustCount=result.Sum(s=>s.AdjustCount),
                            DeathPigCount=result.Sum(s=>s.DeathPigCount),
                            BeginAdditionFee=result.Sum(s=>s.BeginAdditionFee),
                            BeginAdjustFeedFee=result.Sum(s=>s.BeginAdjustFeedFee),
                            BeginBuildingFee=result.Sum(s=>s.BeginBuildingFee),
                            BeginDirectFee=result.Sum(s=>s.BeginDirectFee),
                            BeginHoldUpFee=result.Sum(s=>s.BeginHoldUpFee),
                            BeginInnerDeath=result.Sum(s=>s.BeginInnerDeath),
                            BeginMaterial=result.Sum(s=>s.BeginMaterial),
                            BeginMedicant=result.Sum(s=>s.BeginMedicant),
                            BeginOther=result.Sum(s=>s.BeginOther),
                            BeginOuterDeath=result.Sum(s=>s.BeginOuterDeath),
                            BeginPigCost=result.Sum(s=>s.BeginPigCost),
                            BeginPigCount=result.Sum(s=>s.BeginPigCount),
                            BeginPredictFeedFee=result.Sum(s=>s.BeginPredictFeedFee),
                            BeginVaccine=result.Sum(s=>s.BeginVaccine),
                            CurrentPigCount=result.Sum(s=>s.CurrentPigCount),
                            EndAdditionFee=result.Sum(s=>s.EndAdditionFee),
                            EndAdjustFeedFee=result.Sum(s=>s.EndAdjustFeedFee),
                            EndBuildingFee=result.Sum(s=>s.EndBuildingFee),
                            EndDirectFee=result.Sum(s=>s.EndDirectFee),
                            EndHoldUpFee=result.Sum(s=>s.EndHoldUpFee),
                            EndInnerDeath=result.Sum(s=>s.EndInnerDeath),
                            EndMaterial=result.Sum(s=>s.EndMaterial),
                            EndMedicant=result.Sum(s=>s.EndMedicant),
                            EndOther=result.Sum(s=>s.EndOther),
                            EndOuterDeath =result.Sum(s=>s.EndOuterDeath),
                            EndPigCost=result.Sum(s=>s.EndPigCost),
                            EndPigCount=result.Sum(s=>s.EndPigCount),
                            EndPredictFeedFee=result.Sum(s=>s.EndPredictFeedFee),
                            EndVaccine=result.Sum(s=>s.EndVaccine),
                            RecyclePigCount=result.Sum(s=>s.RecyclePigCount),
                        }
                    };
                    #endregion
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                return new List<BreederSummaryResultModel>();
            }
        }
        public Dictionary<string, string> GetDict()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("2210151500430000102", "CurrentPigCost");
            value.Add("2210151500430000103", "CurrentMaterial");
            value.Add("2210151500430000104", "CurrentMedicant");
            value.Add("2210151500430000105", "CurrentVaccine");
            value.Add("2210151500430000106", "CurrentOther");
            value.Add("2210151500430000107", "CurrentInnerDeath");
            value.Add("2210151500430000108", "CurrentOuterDeath");
            value.Add("2210151500430000109", "CurrentDirectFee");
            value.Add("2210151500430000110", "CurrentBuildingFee");
            value.Add("2210151500430000111", "CurrentPredictFeedFee");
            value.Add("2210151500430000112", "CurrentAdjustFeedFee");
            value.Add("2210151500430000113", "CurrentAdditionFee");
            value.Add("2210151500430000114", "CurrentHoldUpFee");
            value.Add("2210151500430000115", "RecyclePigCost");
            value.Add("2210151500430000116", "RecycleMaterial");
            value.Add("2210151500430000117", "RecycleMedicant");
            value.Add("2210151500430000118", "RecycleVaccine");
            value.Add("2210151500430000119", "RecycleOther");
            value.Add("2210151500430000120", "RecycleInnerDeath");
            value.Add("2210151500430000121", "RecycleOuterDeath");
            value.Add("2210151500430000122", "RecycleDirectFee");
            value.Add("2210151500430000123", "RecycleBuildingFee");
            value.Add("2210151500430000124", "RecyclePredictFeedFee");
            value.Add("2210151500430000125", "RecycleAdjustFeedFee");
            value.Add("2210151500430000126", "RecycleAdditionFee");
            value.Add("2210151500430000127", "RecycleHoldUpFee");
            value.Add("2210151500430000128", "DeathPigCost");
            value.Add("2210151500430000129", "DeathMaterial");
            value.Add("2210151500430000130", "DeathMedicant");
            value.Add("2210151500430000131", "DeathVaccine");
            value.Add("2210151500430000132", "DeathOther");
            value.Add("2210151500430000133", "DeathInnerDeath");
            value.Add("2210151500430000134", "DeathOuterDeath");
            value.Add("2210151500430000135", "DeathDirectFee");
            value.Add("2210151500430000136", "DeathBuildingFee");
            value.Add("2210151500430000137", "DeathPredictFeedFee");
            value.Add("2210151500430000138", "DeathAdjustFeedFee");
            value.Add("2210151500430000139", "DeathAdditionFee");
            value.Add("2210151500430000140", "DeathHoldUpFee");
            value.Add("2210151500430000141", "AdjustCount");
            return value;
        }
    }

}
