using Architecture.Common.HttpClientUtil;
using DBN.EncrypDecryp;
using FinanceManagement.Common.MakeVoucherCommon;
using FinanceManagement.Common.NewMakeVoucherCommon;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Common.MonthEndCheckout
{
    public class FMAPIService
    {
        private HttpClientUtil _httpClientUtil1;
        private HostConfiguration _hostCongfiguration;
        private QlwCrossDbContext _context;
        private IBiz_Related _biz_RelatedRepository;
        private AccocheckFormulaProperty _accocheckFormulaProperty;
        public FMAPIService(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration, QlwCrossDbContext context, IBiz_Related biz_RelatedRepository, AccocheckFormulaProperty accocheckFormulaProperty)
        {
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            _context = context;
            _biz_RelatedRepository = biz_RelatedRepository;
            _accocheckFormulaProperty = accocheckFormulaProperty;
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
                Task<ResultModel> resultModel = _httpClientUtil1.PostJsonAsync<ResultModel>($"{_hostCongfiguration.QlwServiceHost}/api/BIZOptionConfig/getOptionConfig", new { OptionID = optionId, EnterpriseID = enterpriseId, GroupID = GroupID });
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
        /// 获取系统选项
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public string OptionConfigValueString(string optionId, string enterpriseId = "", string GroupID = "")
        {
            try
            {
                if (enterpriseId == "")
                {
                    return string.Empty;
                }
                BIZ_OptionConfig optionConfig = null;
                Task<ResultModel> resultModel = _httpClientUtil1.PostJsonAsync<ResultModel>($"{_hostCongfiguration.QlwServiceHost}/api/BIZOptionConfig/getOptionConfig", new { OptionID = optionId, EnterpriseID = enterpriseId, GroupID = GroupID });
                if (resultModel != null && resultModel.Result.ResultState && resultModel.Result.Data != null) optionConfig = JsonConvert.DeserializeObject<List<BIZ_OptionConfig>>(resultModel.Result.Data.ToString()).FirstOrDefault();
                string result = optionConfig != null ? optionConfig.OptionValue : string.Empty;
                return result;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        /// <summary>
        ///  获取指定单据未审核的数量
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="enterpriseId"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public FMSResultModel<List<long>> GetNotReviewNumbers(long appId, long enterpriseId, DateTime dateTime)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/v2/Review/GetNotReviewNumbers";
            var jsonStr = new
            {
                AppId = appId,
                EnterpriseID = enterpriseId,
                DataDate = dateTime.ToString("yyyy-MM-dd")
            };
            return _httpClientUtil1.PostJsonAsync<FMSResultModel<List<long>>>(url, jsonStr).Result;
        }
        /// <summary>
        ///  获取指定单据未审核的数量
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="enterpriseId"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public FMSResultModel<List<CheckResultModel>> GetNotReviewNumbersExtend(long appId, long enterpriseId, DateTime dateTime)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/v2/Review/GetNotReviewNumbersExtend";
            var jsonStr = new
            {
                AppId = appId,
                EnterpriseID = enterpriseId,
                DataDate = dateTime.ToString("yyyy-MM-dd")
            };
            return _httpClientUtil1.PostJsonAsync<FMSResultModel<List<CheckResultModel>>>(url, jsonStr).Result;
        }
        /// <summary>
        ///  月末计提单据申请
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="enterpriseId"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public List<ApproveModel> GetNotReviewNumbers(long enterpriseId, DateTime dateTime)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/SADcAllowance/UnApproveList";
            var jsonStr = new
            {
                EnterpriseID = enterpriseId.ToString(),
                DataDate = dateTime.ToString("yyyy-MM")
            };
            var result = _httpClientUtil1.PostJsonAsync<ResultModel<ApproveModel>>(url, jsonStr).Result.Data;
            return result;
        }
        /// <summary>
        /// 销售汇总数据是否存在
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public FMSResultModel IsExistSalesSummary(long enterpriseID, DateTime dateTime)
        {
            var model = new
            {
                EnterpriseID = enterpriseID,
                DataDate = dateTime.ToString("yyyy-MM-01")
            };
            string url = _hostCongfiguration.QlwServiceHost + "/api/fm_sasalessummary/exist";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel>(url, model).Result;
        }

        /// <summary>
        /// EXIST 采购汇总
        /// </summary>
        /// <returns></returns>
        public FMSResultModel ExistPurchaseSummary(FMSBusResultModel model)
        {
            string url = string.Format(_hostCongfiguration.QlwServiceHost + "/api/{0}/{1}/fm_pmpurchasesummary/exist", model.EnterpriseID, model.OwnerID);
            return _httpClientUtil1.PostJsonAsync<FMSResultModel>(url, model).Result;
        }
        /// <summary>
        /// POST 采购汇总
        /// </summary>
        /// <param name="model">查询参数</param>
        /// <returns></returns>
        public FMSResultModel<long> PostPurchaseSummary(FMSBusResultModel model)
        {
            string url = string.Format(_hostCongfiguration.QlwServiceHost + "/api/{0}/{1}/FM_PMPurchaseSummary/post", model.EnterpriseID, model.OwnerID);
            return _httpClientUtil1.PostJsonAsync<FMSResultModel<long>>(url, model).Result;
        }
        /// <summary>
        /// DELETE 采购汇总
        /// </summary>
        /// <returns></returns>
        public FMSResultModel DeletePurchaseSummary(FMSBusResultModel model)
        {
            string url = string.Format(_hostCongfiguration.QlwServiceHost + "/api/{0}/{1}/fm_pmpurchasesummary/delete", model.EnterpriseID, model.OwnerID);
            return _httpClientUtil1.PostJsonAsync<FMSResultModel>(url, model).Result;
        }

        /// <summary>
        /// 销售汇总数据是否存在
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public FMSResultModel IsExistSalesSummaryFordiscount(long enterpriseID, DateTime dateTime)
        {
            var model = new
            {
                EnterpriseID = enterpriseID,
                DataDate = dateTime.ToString("yyyy-MM-01")
            };
            string url = _hostCongfiguration.QlwServiceHost + "/api/fm_sasalessummary/existfordiscount";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel>(url, model).Result;
        }
        /// <summary>
        /// 折扣分摊数据
        /// </summary>
        /// <returns></returns>
        public ResultModel ComputeSA_DiscountProvision(FMSBusResultModel model, DateTime startDate, DateTime endDate)
        {
            var _model = new
            {
                EnterpriseID = model.EnterpriseID,
                startDate = startDate,
                endDate = endDate
            };
            string url = _hostCongfiguration.QlwServiceHost + "/api/SADcAllowance/ComputeSA_DiscountProvision";
            var rm = _httpClientUtil1.PostJsonAsync<ResultModel>(url, _model).Result;
            return rm;
        }
        /// <summary>
        /// 计算销售净额
        /// </summary>
        /// <returns></returns>
        public FMSResultModel CalcAmountNet(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/fm_sasalessummary/updateamountnet";
            var rm = _httpClientUtil1.PostJsonAsync<FMSResultModel>(url, model).Result;
            return rm;
        }
        /// <summary>
        /// POST 销售汇总
        /// </summary>
        /// <param name="model">查询参数</param>
        /// <returns></returns>
        public FMSResultModel<long> PostSalesSummary(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/fm_sasalessummary/post";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel<long>>(url, model).Result;
        }
        /// <summary>
        /// 删除销售汇总数据
        /// </summary>
        /// <returns></returns>
        public FMSResultModel DeleteSalesSummary(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/fm_sasalessummary/delete";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel>(url, model).Result;
        }
        /// <summary>
        /// 取消 销售净额 折扣
        /// </summary>
        /// <returns></returns>
        public FMSResultModel CancelAmountNetAndDiscount(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/fm_sasalessummary/cancelamountnetanddiscount";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel>(url, model).Result;
        }
        /// <summary>
        /// 销售汇总表实时数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<FinanceManagement.Common.MakeVoucherCommon.SaleSummaryResultData> GetSaleSummaryData(FMSAccoCheckResultModel model)
        {
            string url1 = $@"{_hostCongfiguration.ReportService}/api/SaleSummary/GetSummaryTypeList";
            FinanceManagement.Common.MakeVoucherCommon.SaleSummaryRequest salerequest = new FinanceManagement.Common.MakeVoucherCommon.SaleSummaryRequest()
            {
                GroupId = model.GroupID,
                MenuParttern = 0,
                EnteID = model.EnterpriseID.ToString(),
                EnterpriseId = model.EnterpriseID,
                QiDian = 0,
            };
            var res = _httpClientUtil1.PostJsonAsync<ResultModel<FinanceManagement.Common.MakeVoucherCommon.SummaryType>>(url1, salerequest).Result;
            List<FinanceManagement.Common.MakeVoucherCommon.SummaryType> summaryTypes = res.Code == 0 ? res.Data : new List<FinanceManagement.Common.MakeVoucherCommon.SummaryType>();

            string url = $@"{_hostCongfiguration.ReportService}/api/SaleSummary/GetSummaryAsync";
            FinanceManagement.Common.MakeVoucherCommon.SaleSummaryRequest param = new FinanceManagement.Common.MakeVoucherCommon.SaleSummaryRequest()
            {
                BeginDate = model.BeginDate,
                EndDate = model.EndDate,
                SalesAbstract = null,
                OnlyCombineEnte = false,
                MeasurementUnit = "0",
                TicketedPoint = "",
                ReceiptType = "-1",
                QiDian = 0,
                APPID = 1612062114270000101,
                IsPig = false,
                DataSource = 0,
                GroupId = Convert.ToInt64(model.GroupID),
                EnteID = model.EnterpriseID.ToString(),
                EnterpriseId = Convert.ToInt64(model.EnterpriseID),
                MenuParttern = 0,
                BoId = Convert.ToInt64(model.OwnerID),
                OwnEntes = new List<string>() { model.EnterpriseID.ToString() },
                SummaryType = summaryTypes.Find(s => s.SN == "单位" && s.PID != null)?.SV,
                SummaryTypeName = summaryTypes.Find(s => s.SN == "单位" && s.PID != null)?.SN,
            };
            ResultModel<FinanceManagement.Common.MakeVoucherCommon.SaleSummaryResultData> result = _httpClientUtil1.PostJsonAsync<ResultModel<FinanceManagement.Common.MakeVoucherCommon.SaleSummaryResultData>>(url, param).Result;
            return result.Code == 0 ? result.Data : new List<FinanceManagement.Common.MakeVoucherCommon.SaleSummaryResultData>();
        }
        /// <summary>
        /// 折扣汇总表   --王增延
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<FinanceManagement.Common.MakeVoucherCommon.DiscountSummaryResult> GetMyDiscountSummaryReport(FMSAccoCheckResultModel model)
        {
            string url = _hostCongfiguration.ReportService + "/api/RptMyDiscountSummary/GetMyDiscountSummaryReport";
            FinanceManagement.Common.MakeVoucherCommon.DiscountSummaryRequest param = new FinanceManagement.Common.MakeVoucherCommon.DiscountSummaryRequest()
            {
                GroupID = model.GroupID.ToString(),
                EnteID = model.EnterpriseID.ToString(),
                BeginDate = model.BeginDate,
                EndDate = model.EndDate,
                MenuParttern = "0",
                EnterPriseIdList = model.EnterpriseID.ToString(),
                DataSource = 0,
                SummaryType1 = "t2.EnterpriseName",
                SummaryType1Name = null,
                Boid = model.OwnerID.ToString(),
            };
            ResultModel<FinanceManagement.Common.MakeVoucherCommon.DiscountSummaryResult> result = _httpClientUtil1.PostJsonAsync<ResultModel<FinanceManagement.Common.MakeVoucherCommon.DiscountSummaryResult>>(url, param).Result;
            if (result!=null)
            {
                if (result.Data != null)
                {
                    var data = result.Data.FirstOrDefault();
                    data.SubTotal = data.YearAmount + data.MonthAmount + data.GlitAmount;
                }
            }
            return result.ResultState ? result.Data : new List<FinanceManagement.Common.MakeVoucherCommon.DiscountSummaryResult>();
        }
        /// <summary>
        /// 存货数据   --排除养户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<StockResult> GetStockDataByAbstract(FMSAccoCheckResultModel model)
        {
            List<WarehouseModel> warehouseList = new List<WarehouseModel>();
            ResultModel<WarehouseModel> warehouseResult = _httpClientUtil1.PostJsonAsync<ResultModel<WarehouseModel>>(_hostCongfiguration.QlwServiceHost + $@"/api/SAOuterGate/GetWarehouseListOutside", new { EnterpriseID = model.EnterpriseID, CostCalculations = 1 }).Result;
            if (warehouseResult.Code == 0)
            {
                warehouseList = warehouseResult.Data;
            }
            string url = _hostCongfiguration.ReportService + $@"/api/RptStockRelated/GetStockSummaryReport";
            var param = new
            { WarehouseName_id = string.Join(',', warehouseList.Select(s => s.WarehouseID).ToArray()), Begindate = model.BeginDate, Enddate = model.EndDate, OnlyCombineEnte = false, IsEnterCate = false, MeasurementUnit = 0, ExactNumber = 2, UseMidTable = 1, SummaryType4 = "", QiDian = 0, Reviewed = 2, IsAuthority = true, IsHasBatchnumber = true, DataSource = 0, SummaryType1 = "h.cDictName", SummaryType1Name = "", SummaryType2 = "", SummaryType2Name = "", SummaryType3 = "", SummaryType3Name = "", IsGroupByEnteCate = false, SummaryT1Rank = -1, SummaryT2Rank = -1, SummaryT3Rank = -1, OwnEntes = new List<string> { model.EnterpriseID.ToString() }, CanWatchEntes = new List<string> { model.EnterpriseID.ToString() }, GroupID = model.GroupID, EnteID = model.EnterpriseID, MenuParttern = 0, Boid = model.OwnerID, MarketsUnderUser = "", Orgs = "", IsCompanyManager = false, HasReportAuth = false, SubjectLevel = 0 };
            ResultModel<StockResult> result = _httpClientUtil1.PostJsonAsync<ResultModel<StockResult>>(url, param).Result;
            return result.Code == 0 ? result.Data : new List<StockResult>();
        }
        /// <summary>
        /// 存货数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<StockResult> GetStockData(FMSAccoCheckResultModel model)
        {
            string url = _hostCongfiguration.ReportService + $@"/api/RptStockRelated/GetStockSummaryReport";
            var param = new
            { Begindate = model.BeginDate, Enddate = model.EndDate, OnlyCombineEnte = false, IsEnterCate = false, MeasurementUnit = 0, ExactNumber = 2, UseMidTable = 1, SummaryType4 = "", QiDian = 0, Reviewed = 2, IsAuthority = true, IsHasBatchnumber = true, DataSource = 0, SummaryType1 = "h.cDictName", SummaryType1Name = "", SummaryType2 = "", SummaryType2Name = "", SummaryType3 = "", SummaryType3Name = "", IsGroupByEnteCate = false, SummaryT1Rank = -1, SummaryT2Rank = -1, SummaryT3Rank = -1, OwnEntes = new List<string> { model.EnterpriseID.ToString() }, CanWatchEntes = new List<string> { model.EnterpriseID.ToString() }, GroupID = model.GroupID, EnteID = model.EnterpriseID, MenuParttern = 0, Boid = model.OwnerID, MarketsUnderUser = "", Orgs = "", IsCompanyManager = false, HasReportAuth = false, SubjectLevel = 0 };
            ResultModel<StockResult> result = _httpClientUtil1.PostJsonAsync<ResultModel<StockResult>>(url, param).Result;
            return result.Code == 0 ? result.Data : new List<StockResult>();
        }

        public List<AbstractModel> GetAbstractList(FMSAccoCheckResultModel model, string pid)
        {
            string url = _hostCongfiguration.NxinGatewayUrl + $@"api/nxin.qlwbase.dict.tagenter.list/1.0?enterpriseid={model.EnterpriseID}&pid={pid}";
            ResultModel<AbstractModel> result = _httpClientUtil1.GetJsonAsync<ResultModel<AbstractModel>>(url).Result;
            return result.Code == 0 ? result.Data : new List<AbstractModel>();
        }
        public List<AbstractModel> GetSaleAbstractList(FMSAccoCheckResultModel model, string pid)
        {
            string url = _hostCongfiguration.QlwBase + $@"/WEBAPI/ApiDataDict/GetEnterTagDictData?EnterpriseID={model.EnterpriseID}&Pid={pid}&IsGroup=0 ";
            ResultModel<AbstractModel> result = _httpClientUtil1.PostJsonAsync<ResultModel<AbstractModel>>(url, null).Result;
            return result.Code == 0 ? result.Data : new List<AbstractModel>();
        }
        /// <summary>
        /// 采购汇总表实时
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<FinanceManagement.Common.MakeVoucherCommon.PurchaseSummaryResultData> GetPurchaseSummaryDataList(FMSAccoCheckResultModel model)
        {
            string url = _hostCongfiguration.ReportService + "/api/RptPurchaseRelated/GetPurchaseSummaryReport";
            FinanceManagement.Common.MakeVoucherCommon.PurchaseSummaryRequest param = new FinanceManagement.Common.MakeVoucherCommon.PurchaseSummaryRequest()
            {
                Begindate = model.BeginDate,
                Boid = model.OwnerID.ToString(),
                CanWatchEntes = new List<string>() { model.EnterpriseID.ToString() },
                DataSource = 0,
                DateRules = null,
                Enddate = model.EndDate,
                EnteID = model.EnterpriseID,
                GroupID = model.GroupID,
                HasReportAuth = false,
                IsCompanyManager = false,
                IsEnterCate = false,
                IsGroupByEnteCate = false,
                IsSpecialProuct = false,
                MeasurementUnit = 0,
                MenuParttern = "0",
                OnlyCombineEnte = false,
                OwnEntes = new List<string>() { model.EnterpriseID.ToString() },
                QiDian = 0,
                SummaryT1Rank = "-1",
                SummaryT2Rank = "-1",
                SummaryT3Rank = "-1",
                SummaryType1 = "c.EnterpriseName",
            };
            ResultModel<FinanceManagement.Common.MakeVoucherCommon.PurchaseSummaryResultData> result = _httpClientUtil1.PostJsonAsync<ResultModel<FinanceManagement.Common.MakeVoucherCommon.PurchaseSummaryResultData>>(url, param).Result;
            return result.Code == 0 ? result.Data : new List<FinanceManagement.Common.MakeVoucherCommon.PurchaseSummaryResultData>();
        }
        /// <summary>
        /// 成本汇总表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<CostSummaryDataModel> GetCostDataList(FMSAccoCheckResultModel model, string summaryType)
        {
            string url = _hostCongfiguration.ReportService + "/api/RptMyCostSummary/GetMyCostSummaryReport";
            var param = new
            {
                BeginDate = model.BeginDate,
                EndDate = model.EndDate,
                EnterPriseIdList = model.EnterpriseID.ToString(),
                UnitName = 1,
                TableShow = 2,
                ChengBenType = 0,
                SummaryType1 = summaryType
            };
            ResultModel<CostSummaryDataModel> result = _httpClientUtil1.PostJsonAsync<ResultModel<CostSummaryDataModel>>(url, param).Result;
            return result.ResultState ? result.Data : new List<CostSummaryDataModel>();
        }
        public List<PM_PurchaseCarriage> getCarriageAmount(FMSAccoCheckResultModel model)
        {
            string url = "http://demo.s.qlw.nxin.com/api/PerformanceSummary/getCarriageAmount";
            var param = new
            {
                BeginDate = model.BeginDate,
                EndDate = model.EndDate,
                EnterpriseIds = model.EnterpriseID.ToString()
            };
            List<PM_PurchaseCarriage> result = _httpClientUtil1.PostJsonAsync<List<PM_PurchaseCarriage>>(url, param).Result;
            return result;

        }
        /// <summary>
        /// 生成存货汇总表
        /// </summary>
        /// <returns></returns>
        public FMSResultModel GenerateWarehouseBalance(long enterpriseID, DateTime dateTime)
        {
            var model = new
            {
                EnterpriseID = enterpriseID,
                DataDate = dateTime.ToString("yyyy-MM-dd")
            };
            string url = "http://demo.s.qlw.nxin.com/api/MidWarehouseBalance/Post";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel>(url, model).Result;
        }

        /// <summary>
        /// 取消存货汇总表
        /// </summary>
        /// <returns></returns>
        public FMSResultModel CancelWarehouseBalance(long enterpriseID, DateTime dateTime)
        {
            var model = new
            {
                EnterpriseID = enterpriseID,
                DataDate = dateTime.ToString("yyyy-MM-dd")
            };
            string url = "http://demo.s.qlw.nxin.com/api/MidWarehouseBalance/Delete";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel>(url, model).Result;
        }
        /// <summary>
        /// 计算销售成本和毛利
        /// </summary>
        /// <returns></returns>
        public FMSResultModel CalcSalesCostAndProfit(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/fm_sasalessummary/calcsalescostandprofit";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel>(url, model).Result;
        }

        /// <summary>
        /// 取消销售成本和毛利
        /// </summary>
        /// <returns></returns>
        public FMSResultModel CancelSalesCostAndProfit(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/fm_sasalessummary/cancelsalescostandprofit";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel>(url, model).Result;
        }
        public FMSResultModel IsExistMidSubjectBalance(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/midsubjectbalance/exist";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel>(url, model).Result;
        }
        /// <summary>
        /// POST 科目余额中间表
        /// </summary>
        /// <param name="model">查询参数</param>
        /// <returns></returns>
        public FMSResultModel<long> PostMidSubjectBalance(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/midsubjectbalance/post";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel<long>>(url, model).Result;
        }
        /// <summary>
        /// 删除 科目余额中间表
        /// </summary>
        /// <returns></returns>
        public FMSResultModel DeleteMidSubjectBalance(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/midsubjectbalance/delete";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel>(url, model).Result;
        }
        /// <summary>
        /// POST 往来余额中间表
        /// </summary>
        /// <param name="model">查询参数</param>
        /// <returns></returns>
        public FMSResultModel<long> PostMidDealingBalance(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/middealingbalance/post";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel<long>>(url, model).Result;
        }

        /// <summary>
        /// 删除 往来余额中间表
        /// </summary>
        /// <returns></returns>
        public FMSResultModel DeleteMidDealingBalance(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/middealingbalance/delete";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel>(url, model).Result;
        }

        /// <summary>
        /// 往来余额中间数据是否存在
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public FMSResultModel IsExistMidDealingBalance(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/middealingbalance/exist";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel>(url, model).Result;
        }

        /// <summary>
        /// POST 应收汇总表
        /// </summary>
        /// <param name="model">查询参数</param>
        /// <returns></returns>
        public FMSResultModel<long> PostReceivablesSummary(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/fd_receivablessummary/post";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel<long>>(url, model).Result;
        }

        /// <summary>
        /// 删除 应收汇总表
        /// </summary>
        /// <returns></returns>
        public FMSResultModel DeleteReceivablesSummary(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/fd_receivablessummary/delete";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel>(url, model).Result;
        }

        /// <summary>
        /// 应收汇总表 数据是否存在
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public FMSResultModel IsExistReceivablesSummary(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/fd_receivablessummary/exist";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel>(url, model).Result;
        }

        /// <summary>
        /// 生成资金汇总表
        /// </summary>
        /// <returns></returns>
        public FMSResultModel GenerateFundsBalance(long enterpriseID, DateTime dateTime)
        {
            var model = new
            {
                EnterpriseID = enterpriseID,
                DataDate = dateTime.ToString("yyyy-MM-dd")
            };
            string url = _hostCongfiguration.QlwServiceHost + "/api/MidFundsBalance/Post";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel>(url, model).Result;
        }

        /// <summary>
        /// 取消资金汇总表
        /// </summary>
        /// <returns></returns>
        public FMSResultModel CancelFundsBalance(long enterpriseID, DateTime dateTime)
        {
            var model = new
            {
                EnterpriseID = enterpriseID,
                DataDate = dateTime.ToString("yyyy-MM-dd")
            };
            string url = _hostCongfiguration.QlwServiceHost + "/api/MidFundsBalance/Delete";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel>(url, model).Result;
        }
        /// <summary>
        /// 查询三大财务报表的审核数据
        /// 利润表 资产表 现流表
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public FMSResultModel<List<BIZ_Reviwe>> GetDataToReport(FMSBusResultModel model)
        {
            var data = new
            {
                AppID = (long)AppIDEnum.资产负债表,
                DataDate = model.DataDate,
                EnterpriseID = model.EnterpriseID
            };
            string url = _hostCongfiguration.QlwServiceHost + "/api/v2/Review/GetDataToReport";
            return _httpClientUtil1.PostJsonAsync<FMSResultModel<List<BIZ_Reviwe>>>(url, model).Result;
        }
        /// <summary>
        /// 获取科目余额表
        /// </summary>
        /// <returns></returns>
        public List<SubjectBalance> RetSubjectBalance(FMSBusResultModel model, int rank = 0)
        {
            if (rank == 0)
            {
                rank = _httpClientUtil1.GetJsonAsync<int>(_hostCongfiguration.ReportService + $@"/api/RetSubjectBalance/GetSubjectMaxLevelByEnterId?groupId={model.GroupID}&enterId={model.EnterpriseID}").Result;
            }
            string url = _hostCongfiguration.ReportService + "/api/RetSubjectBalance/GetBalanceReport";
            var data = new SubjectBalanceRequset() { Begindate = model.BeginDate, EnterpriseList_id = model.EnterpriseID.ToString(), Enddate = model.EndDate, AccountingSubjects = "", AccountingSubjectsRadio = "", AccountingSubjects2 = "", AccountingSubjectsRadio2 = "", AccountingType = "-1", OnlyCombineEnte = false, EnterpriseList = model.EnterpriseID.ToString(), SubjectLevel = 1, SubjectLevel2 = rank, DataSource = 0, SummaryType1 = "enterName", IsGroupByEnteCate = false, SummaryT1Rank = "-1", SummaryT2Rank = "-1", SummaryT3Rank = "-1", OwnEntes = new List<string>() { model.EnterpriseID.ToString() }, CanWatchEntes = new List<string>() { model.EnterpriseID.ToString() }, GroupID = model.GroupID, EnteID = model.EnterpriseID, MenuParttern = "0", Boid = model.OwnerID.ToString() };
            var result = _httpClientUtil1.PostJsonAsync<ResultModel<SubjectBalance>>(url, data).Result;
            if (result.ResultState)
            {
                return result.Data;
            }
            else
            {
                return new List<SubjectBalance>();
            }
        }
        /// <summary>
        /// 获取资金汇总表
        /// </summary>
        /// <returns></returns>
        public List<FundsSummaryResult> FundsSummary(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.ReportService + "/api/FundsSummary/GetSummaryAsync";
            var data = new FundsSummaryRequest() { BeginDate = model.BeginDate, EndDate = model.EndDate, DataSource = 0, BoId = model.OwnerID, MenuParttern = 0, OnlyCombineEnte = false, EnterpriseId = model.EnterpriseID, GroupId = model.GroupID, SummaryTypeList = new List<string>() { "c.EnterpriseName" }, OwnEntes = new List<string>() { model.OwnerID.ToString() } };
            var result = _httpClientUtil1.PostJsonAsync<ResultModel<FundsSummaryResult>>(url, data).Result;
            if (result.CodeNo == 0)
            {
                return result.Data;
            }
            else
            {
                return new List<FundsSummaryResult>();
            }
        }
        /// <summary>
        /// 获取往来余额
        /// </summary>
        /// <returns></returns>
        public List<SettleReceiptBalance> SettleReceiptBalance(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.ReportService + "/api/RetSettleReceiptBalance/GetBalanceReport";
            var data = new SettleReceiptRequest()
            {
                Begindate = Convert.ToDateTime(model.BeginDate),
                Enddate = Convert.ToDateTime(model.EndDate),
                MenuParttern = "0",
                EnterpriseList_id = model.EnterpriseID.ToString(),
                EnteID = model.EnterpriseID,
                GroupID = model.GroupID,
                AccountingType = "2",
                DataSource = 0,
                AccountingSubjectsRadio = "",
                SubjectLevel = 1
            };
            var result = _httpClientUtil1.PostJsonAsync<ResultModel<SettleReceiptBalance>>(url, data).Result;
            if (result.ResultState)
            {
                List<SettleReceiptBalance> _res = result.Data.GroupBy(x => new
                {
                    x.EnterpriseName,
                    x.AccoSubjectCode,
                    x.AccoSubjectFullName,
                    x.CustomerID,
                    x.CustomerName,
                    x.AccoSubjectID,
                    x.AccoSubjectName,
                    x.AccoSubjectSystemID,
                    x.IsLorR,
                    x.PersonID,
                    x.Rank
                }).Select(x =>
                    new SettleReceiptBalance
                    {
                        EnterpriseName = x.Key.EnterpriseName,
                        AccoSubjectCode = x.Key.AccoSubjectCode,
                        AccoSubjectFullName = x.Key.AccoSubjectFullName,
                        CustomerID = x.Key.CustomerID,
                        CustomerName = x.Key.CustomerName,
                        AccoSubjectID = x.Key.AccoSubjectID,
                        AccoSubjectName = x.Key.AccoSubjectName,
                        AccoSubjectSystemID = x.Key.AccoSubjectSystemID,
                        IsLorR = x.Key.IsLorR,
                        PersonID = x.Key.PersonID,
                        Rank = x.Key.Rank,
                        Credit = x.Sum(n => n.Credit),
                        Debit = x.Sum(n => n.Debit),
                        fsCredit = x.Sum(n => n.fsCredit),
                        fsDebit = x.Sum(n => n.fsDebit),
                        qcCredit = x.Sum(n => n.qcCredit),
                        qcDebit = x.Sum(n => n.qcDebit)
                    }
                  ).ToList();
                return _res;
            }
            else
            {
                return new List<SettleReceiptBalance>();
            }
        }
        /// <summary>
        /// 应收账款汇总表接口
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="summaryStr">c.EnterpriseName </param>
        public List<ReceivableSummaryDataResult> ReceivableSummary(FMSBusResultModel model)
        {
            //c.EnterpriseName   cs.CustomerName
            string url = _hostCongfiguration.ReportService + "/api/RptReceivableRelated/GetReceivableSummaryReport";
            var param = new ReceivableSummaryRequest()
            {
                Begindate = model.BeginDate,
                Boid = model.OwnerID.ToString(),
                CanWatchEntes = new List<string>() { model.EnterpriseID.ToString() },
                DataSource = 0,
                DiscountSource = 0,
                EndTime = Convert.ToDateTime(model.EndDate),
                Enddate = model.EndDate,
                EnteID = model.EnterpriseID,
                GroupID = model.GroupID,
                MenuParttern = "0",
                OwnEntes = new List<string>() { model.OwnerID.ToString() },
                ScopeControl = 1,
                ShowMinus = "y",
                StartTime = Convert.ToDateTime(model.BeginDate),
                SummaryT1Rank = "-1",
                SummaryT2Rank = "-1",
                SummaryT3Rank = "-1",
                SummaryType1 = "c.EnterpriseName",
                SummaryType1Name = "",
                eDuZhanBiGongShi = 1,
                isJinRong = false,
            };
            var result = _httpClientUtil1.PostJsonAsync<ResultModel<ReceivableSummaryDataResult>>(url, param).Result;
            if (result.ResultState)
            {
                return result.Data;
            }
            else
            {
                return new List<ReceivableSummaryDataResult>();
            }
        }
        /// <summary>
        /// 应付账款汇总表接口
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="summaryStr">c.EnterpriseName </param>
        public List<PayableSummaryDataResult> PayableSummary(FMSBusResultModel model)
        {
            //"supplier"   EnterpriseName  month
            string url = _hostCongfiguration.ReportService + "/api/RptPayableRelated/GetPayableSummaryReport";

            var param = new ReceivableSummaryRequest()
            {
                Begindate = model.BeginDate,
                Boid = model.OwnerID.ToString(),
                CanWatchEntes = new List<string>() { model.EnterpriseID.ToString() },
                DataSource = 0,
                Enddate = model.EndDate,
                EnteID = model.EnterpriseID,
                GroupID = model.GroupID,
                MenuParttern = "0",
                OwnEntes = new List<string>() { model.OwnerID.ToString() },
                SummaryT1Rank = "-1",
                SummaryT2Rank = "-1",
                SummaryT3Rank = "-1",
                SummaryType1 = "supplier",
                SummaryType1Name = null,
            };
            var result = _httpClientUtil1.PostJsonAsync<ResultModel<PayableSummaryDataResult>>(url, param).Result;
            if (result.ResultState)
            {
                return result.Data;
            }
            else
            {
                return new List<PayableSummaryDataResult>();
            }
        }
        /// <summary>
        /// 获取物品明细表
        /// </summary>
        /// <returns></returns>
        public List<SuppliesModelForDataResult> GetSuppliesReport(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/FDGoodsPurchasingSummaryRelated/GetSalesSummaryReport";
            SuppliesModelForRequeset param = new SuppliesModelForRequeset()
            {
                Begindate = model.BeginDate,
                Boid = model.OwnerID.ToString(),
                CanWatchEntes = new List<string>() { model.EnterpriseID.ToString() },
                DataSource = 0,
                Enddate = model.EndDate,
                EnteID = model.EnterpriseID,
                EnterpriseList = "|" + model.EnterpriseID.ToString(),
                GroupID = model.GroupID,
                IsAudit = 0,
                IsOpenAddWeight = OptionConfigValue("20190319132324", model.EnterpriseID.ToString(), model.GroupID.ToString()) == true ? "1" : "0",//系统选项：启用物品出库金额用“全月加权平均法”核算       20190319132324
                IsOpenEndMoneyOption = OptionConfigValue("20181213092354", model.EnterpriseID.ToString(), model.GroupID.ToString()) == true ? "1" : "0",//系统选项：启用物品结账    ID：20181213092354
                IsOpenFavor = OptionConfigValue("20181112155405", model.EnterpriseID.ToString(), model.GroupID.ToString()),
                IsQueryMonthFinishData = false,
                IsToDetail = true,
                IsUseNewMethod = false,
                MenuParttern = "1",
                OwnEntes = new List<string>() { model.EnterpriseID.ToString() },
            };
            var result = _httpClientUtil1.PostJsonAsync<ResultModel<SuppliesModelForDataResult>>(url, param).Result;
            if (result.ResultState)
            {
                return QueryDataOperation(result.Data);
            }
            else
            {
                return new List<SuppliesModelForDataResult>();
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
                // 合计
                if (otherItem.Count > 0)
                {
                    dataRes.Add(new SuppliesModelForDataResult
                    {
                        dDate = "合计",
                        InOutQuantity = otherItem.Sum(n => n.InOutQuantity),//decimal.Round(otherItem.Sum(n => n.InOutQuantity), 2, MidpointRounding.AwayFromZero),
                        InOutAmount = decimal.Round(otherItem.Sum(n => n.InOutAmount), 2, MidpointRounding.AwayFromZero),
                        qcQuantity = otherItem.Last().qcQuantity,
                        qcAmount = decimal.Round(otherItem.Last().qcAmount, 2, MidpointRounding.AwayFromZero)
                    });
                }
            }
            return dataRes;
        }

        /// <summary>
        /// DELETE 物品汇总
        /// </summary>
        /// <returns></returns>
        public FMSResultModel<object> DeleteSuppliesSummary(FMSBusResultModel model)
        {
            var result = new FMSResultModel<object>();
            string url = string.Format(_hostCongfiguration.QlwServiceHost + "/api/PMSuppliesMonthBill/Delete", model.EnterpriseID, model.OwnerID);
            var rm = _httpClientUtil1.PostJsonAsync<ResultModel>(url, model).Result;
            if (rm.ResultState)
            {
                result.errcode = FMErrorEnum.OK;
                result.result_code = FMBusinessResultEnum.SUCCESS;
                result.data = rm.Data;
            }
            return result;
        }

        /// <summary>
        /// 物品结账
        /// </summary>
        /// <returns></returns>
        public FMSResultModel<object> CheckSupplies(FMSBusResultModel model)
        {
            try
            {
                var result = new FMSResultModel<object>();
                string url = _hostCongfiguration.QlwServiceHost + "/api/PMSuppliesMonthBill/GetSuppliesMonthBill";
                var rm = _httpClientUtil1.PostJsonAsync<ResultModel>(url, model).Result;
                if (rm.ResultState)
                {
                    result.errcode = FMErrorEnum.OK;
                    result.result_code = FMBusinessResultEnum.SUCCESS;
                    result.data = rm.Data;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// 是否都进行了折旧计提
        /// </summary>
        /// <returns></returns>
        public FMSResultModel<object> CheckDepreciationAccrued(FMSBusResultModel model)
        {
            var result = new FMSResultModel<object>();
            var data = new
            {
                BeginDate = model.Date.ToString("yyyy-MM-dd"),
                EnterID = model.EnterpriseID,
                GroupID = model.GroupID
            };
            string url = _hostCongfiguration.QlwServiceHost + "/api/FA_DepreciationAccrued/ValidateFADepreciationAccruedDetail";
            var rm = _httpClientUtil1.PostJsonAsync<ResultModel>(url, data).Result;
            if (rm.ResultState)
            {
                result.errcode = FMErrorEnum.OK;
                result.result_code = FMBusinessResultEnum.SUCCESS;
                result.data = rm.Data;
            }
            return result;
        }
        /// <summary>
        /// 总账与固定资产模块核对
        /// </summary>
        /// <returns></returns>
        public FMSResultModel<object> CheckFixedAssets(FMSBusResultModel model)
        {
            var result = new FMSResultModel<object>();
            string url = _hostCongfiguration.QlwServiceHost + "/api/FA_AssetsMonthBill/GetAccoMonthBill";
            var rm = _httpClientUtil1.PostJsonAsync<ResultModel>(url, model).Result;
            if (rm.ResultState)
            {
                result.errcode = FMErrorEnum.OK;
                result.result_code = FMBusinessResultEnum.SUCCESS;
                result.data = rm.Data;
            }
            return result;
        }

        /// <summary>
        /// 固定资产折旧汇总表
        /// </summary>
        /// <returns></returns>
        public List<AssetsCardDepreciationSummary> AssetsCardDepreciationSummary(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/AssetsCardDepreciationSummaryReport/GetAssetsCardDepreciationSummaryReport";
            var data = new { BeginDate = Convert.ToDateTime(model.BeginDate).ToString("yyyy-MM"), EndDate = Convert.ToDateTime(model.EndDate).ToString("yyyy-MM"), MenuParttern = 1, EnteID = model.EnterpriseID, GroupID = model.GroupID, SummaryType1 = "facf.ClassificationID" };
            var result = _httpClientUtil1.PostJsonAsync<ResultModel<AssetsCardDepreciationSummary>>(url, data).Result;
            if (result.CodeNo == 0)
            {
                return result.Data;
            }
            else
            {
                return new List<AssetsCardDepreciationSummary>();
            }
        }

        /// <summary>
        /// 固定资产卡片报表
        /// </summary>
        /// <returns></returns>
        public List<AssetsCardInfo> AssetsCardInfo(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/AssetsCardInfoReport/GetAssetsCardInfoReport";
            var data = new { BeginDate = model.BeginDate, EndDate = model.EndDate, MenuParttern = 1, EnteID = model.EnterpriseID, GroupID = model.GroupID, AssetsIsUse = 1 };
            var result = _httpClientUtil1.PostJsonAsync<ResultModel<AssetsCardInfo>>(url, data).Result;
            if (result.CodeNo == 0)
            {
                return result.Data;
            }
            else
            {
                return new List<AssetsCardInfo>();
            }
        }
        public ResultModel CheckPigFarmLiveStock(string enterpriseId, string PigFarmIds, string startDate, string endDate)
        {
            string url = $"{_hostCongfiguration.DBN_ZLWServiceHost}/basic/LiveStockAbnormalData?EnterpriseId={enterpriseId}&PigFarmIds={PigFarmIds}&StartDate={startDate}&EndDate={endDate}";
            var result = _httpClientUtil1.GetJsonAsync<ResultModel>(url).Result;
            return result;
        }
        /// <summary>
        /// 运费汇总表实时
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<Freight> GetGetMyFreightSummaryReport(FMSAccoCheckResultModel model)
        {
            string url = _hostCongfiguration.ReportService + "/api/RptMyFreightSummary/GetMyFreightSummaryReport";
            object param = new
            {
                BeginDate = model.BeginDate,
                EndDate = model.EndDate,
                EnterPriseIdList = model.EnterpriseID.ToString(),
                OnlyCombineEnte = false,
                DataSource = 0,
                SummaryType1 = "carrAbstract.CarriageAbstractName",
                SummaryType1Name = "",
                SummaryType2 = "",
                SummaryType2Name = "",
                SummaryType3 = "",
                SummaryType3Name = "",
                IsGroupByEnteCate = false,
                GroupID = model.GroupID.ToString(),
                EnteID = model.EnterpriseID.ToString(),
                MenuParttern = 0,
                Boid = model.OwnerID,
                MarketsUnderUser = "",
                IsCompanyManager = false,
                HasReportAuth = false,
                ExactNumber = ""
            };
            ResultModel<Freight> result = _httpClientUtil1.PostJsonAsync<ResultModel<Freight>>(url, param).Result;
            return result.Code == 0 ? result.Data : new List<Freight>();
        }

        /// <summary>
        /// 固定资产卡片报表-时间、单位、所有资产状态
        /// </summary>
        /// <returns></returns>
        public List<AssetsCardInfo> AssetsCardInfoAllStatus(FMSBusResultModel model)
        {
            string url = _hostCongfiguration.QlwServiceHost + "/api/AssetsCardInfoReport/GetAssetsCardInfoReport";
            var data = new { BeginDate = model.BeginDate, EndDate = model.EndDate, MenuParttern = 1, EnteID = model.EnterpriseID, GroupID = model.GroupID };
            var result = _httpClientUtil1.PostJsonAsync<ResultModel<AssetsCardInfo>>(url, data).Result;
            if (result.CodeNo == 0)
            {
                return result.Data;
            }
            else
            {
                return new List<AssetsCardInfo>();
            }
        }
        /// <summary>
        /// 猪成本变动汇总表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<CostSummary> CostSummary(FMSAccoCheckResultModel model)
        {
            //成本变动报表接口文档地址：https://confluence.nxin.com/pages/viewpage.action?pageId=65051490 接口说明
            string url = _hostCongfiguration._wgUrl + "/cost/report/CostSummary/CostSummary";
            var param = new
            {
                DataDate = model.EndDate,
                EnterpriseID = model.EnterpriseID.ToString(),
                SummaryItems = new List<string>() { "pigFarmName" },
            };
            ResultModel<CostSummary> result = _httpClientUtil1.PostJsonAsync<ResultModel<CostSummary>>(url, param).Result;
            return result.Code == 0 ? result.Data : new List<CostSummary>();
        }
        /// <summary>
        /// 羊成本变动汇总表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<CostSummary> ChangeCostSummary(FMSAccoCheckResultModel model)
        {
            //成本变动报表接口文档地址：https://confluence.nxin.com/pages/viewpage.action?pageId=65051490 接口说明
            string url = _hostCongfiguration._wgUrl + "/cost/report/Finance/YlwCostChangeSummary";
            var param = new
            {
                DataDate = model.EndDate,
                EnterpriseID = model.EnterpriseID.ToString(),
                SummaryItems = new List<string>() { "pigFarmName" },
            };
            ResultModel<CostSummary> result = _httpClientUtil1.PostJsonAsync<ResultModel<CostSummary>>(url, param, (a) => { a.Authorization = model._token; }).Result;
            return result.Code == 0 ? result.Data : new List<CostSummary>();
        }
        /// <summary>
        /// 养户成本变动汇总表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<BreederSummaryResultModel> YhPigCostSummary(FMSAccoCheckResultModel model)
        {
            //https://confluence.nxin.com/pages/viewpage.action?pageId=65050997 接口说明
            string url = _hostCongfiguration._wgUrl + "/cost/report/YhPigCostSummary/Finance/Data";
            PigSearchModel param = new PigSearchModel()
            {
                EndDate = Convert.ToDateTime(model.EndDate),
                StartDate = Convert.ToDateTime(model.BeginDate),
                EnterpriseId = model.EnterpriseID.ToString(),
                Summary = new List<string>() { "DeptId" },
                SummaryName = new List<string>() { "部门" }
            };
            var result = _httpClientUtil1.PostJsonAsync<ResultModel<BreederSummaryResultModel>>(url, param, (a) => { a.Authorization = model._token; }).Result.Data;
            return result;
        }
        /// <summary>
        /// 猪场成本明细表期末总成本接口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public decimal PigCostDetailReportEndCostTotal(FMSAccoCheckResultModel model)
        {
            //成本变动报表接口文档地址：https://confluence.nxin.com/pages/viewpage.action?pageId=65053513 接口说明
            string url = _hostCongfiguration._wgUrl + "/cost/report/Finance/PigCostDetailReportEndCostTotal";
            var param = new
            {
                StartDate = model.BeginDate,
                EndDate = model.EndDate,
            };
            //AuthenticationHeaderValue value = new AuthenticationHeaderValue("Bearer", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJpYXQiOjE2NzUyMzczMDgsImV4cCI6MTY3NTI0NDUwOCwidXNlcl9pZCI6IjE4NTUzNTkiLCJlbnRlcnByaXNlX2lkIjoiMzI3MjI4MyIsImdyb3VwX2lkIjoiMzIxMzAzOSIsIm1lbnVfaWQiOiIyMjA4MTgwOTI4MDQwMDAwMTA5IiwiY2hpbGRfZW50ZXJwcmlzZV9pZCI6IjAiLCJwZXJtaXNzaW9ucyI6MzEsImlzcyI6ImlgbCIsImF1ZCI6InFsdy1iYXNlIn0.AnlB6M8lYtZSVVGVdDHfGwgDze5VF01LkDIfPo5ntyzX1eTTBBvdly7yX7ebQ9eT6ZNBa8-lytfTmHIpNQ2DrUmI6brfAX1UtzL7nOHMaemDStmt1P9PobE-miCGFP78B6312SpHT-h7bM5uCf7PNe2SN2TjHdrxJnBtKOz2GqRT1JYSFTLOmZkNvBwNeuwcXFqSXG7GtNxQQFMaUIqgyLP49hcu7R_PNSzORfGiwygGMdaPl4a7PPzYmHh44Y6i8ALuDKQuv0ZyqeYZkOD_q-_ykrtBuR3FTxxCoZZvnt6kNsVWEqF34RZwCgy-4JoOtgzmnygPyXVt7X2VFYCUiA");
            decimal result = _httpClientUtil1.PostJsonAsync<decimal>(url, param, (a) => { a.Authorization = model._token; }).Result;
            return result;
        }
        /// <summary>
        /// 养户成本汇总表期末总成本接口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public decimal YhPigCostEndCostTotal(FMSAccoCheckResultModel model)
        {
            //成本变动报表接口文档地址：https://confluence.nxin.com/pages/viewpage.action?pageId=65053513 接口说明
            string url = _hostCongfiguration._wgUrl + "/cost/report/Finance/YhPigCostEndCostTotal";
            var param = new
            {
                StartDate = model.BeginDate,
                EndDate = model.EndDate,
            };
            decimal result = _httpClientUtil1.PostJsonAsync<decimal>(url, param, (a) => { a.Authorization = model._token; }).Result;
            return result;
        }
        /// <summary>
        /// 物资饲料运费 王增延提供`
        /// </summary>
        /// <returns></returns>
        public List<JXCModel> GetWuziList(FMSAccoCheckResultModel model)
        {
            string sql = $@" SELECT CONCAT(RAND() * 10000000)  AS Guid,t2.AmountTotal      AS Amount
                FROM nxin_qlw_business.LS_Carriage AS t1
                         INNER JOIN nxin_qlw_business.LS_Carriagedetail AS t2 ON t1.NumericalOrder = t2.NumericalOrder
                WHERE t1.CarriageType IN (201612190104402102, 2210111840400000109)
                  AND t1.EnterpriseID = {model.EnterpriseID}
                  AND t1.DataDate BETWEEN '{model.BeginDate}' AND '{model.EndDate}'
                AND t1.NumericalOrder NOT IN (
                SELECT  ls.NumericalOrder
                FROM nxin_qlw_business.ls_transportexpenses AS lt
                         INNER JOIN nxin_qlw_business.ls_transportexpensesdetail AS ltd ON lt.NumericalOrder = ltd.NumericalOrder
                         INNER JOIN nxin_qlw_business.ls_transportexpensesdetail_materialtransport AS ltm
                                    ON lt.NumericalOrder = ltm.NumericalOrder
                         INNER JOIN nxin_qlw_business.BIZ_Related AS br
                                    ON br.RelatedType = 201610210104402122 AND br.ParentType = 1612230944450000101 AND
                                       br.ChildType = 2207300028100000109
                                        AND br.ChildValueDetail = ltm.NumericalOrder AND br.ParentValueDetail = ltd.NumericalOrderDetail
                         INNER JOIN nxin_qlw_business.LS_CarriageDetail AS lsc ON lsc.NumericalOrderDetail = br.ChildValue
                         INNER JOIN nxin_qlw_business.LS_Carriage AS ls ON ls.NumericalOrder = lsc.NumericalOrder
                WHERE lt.EnterpriseID = {model.EnterpriseID} AND lt.OrganizationType=2 AND lt.DataDate BETWEEN  '{model.BeginDate}' AND '{model.EndDate}' )
                AND t1.CarriageAbstract IN (SELECT CarriageAbstractID FROM nxin_qlw_business.ls_carriageabstractset WHERE EnterpriseID={model.EnterpriseID}
                                AND IsCost=1) ";
            return _context.JXCDataSet.FromSqlRaw(sql).ToList();
        }
        /// <summary>
        /// 养护运费 王增延提供`
        /// </summary>
        /// <returns></returns>
        public List<JXCModel> GetYangHuList(FMSAccoCheckResultModel model)
        {
            string sql = $@" SELECT  CONCAT(RAND() * 10000000)  AS Guid,lsc.AmountTotal AS Amount
FROM nxin_qlw_business.ls_transportexpenses AS lt
         INNER JOIN nxin_qlw_business.ls_transportexpensesdetail AS ltd ON lt.NumericalOrder = ltd.NumericalOrder
         INNER JOIN nxin_qlw_business.ls_transportexpensesdetail_pigtransport AS ltm
                    ON lt.NumericalOrder = ltm.NumericalOrder
         INNER JOIN nxin_qlw_business.BIZ_Related AS br
                    ON br.RelatedType = 201610210104402122 AND br.ParentType = 1612230944450000101 AND
                       br.ChildType = 2207300028100000109
                        AND br.ChildValueDetail = ltm.NumericalOrder AND br.ParentValueDetail = ltd.NumericalOrderDetail
         INNER JOIN nxin_qlw_business.LS_CarriageDetail AS lsc ON lsc.NumericalOrderDetail = br.ChildValue
         INNER JOIN nxin_qlw_business.LS_Carriage AS ls ON ls.NumericalOrder = lsc.NumericalOrder
         LEFT JOIN nxin_qlw_business.biz_productdetail AS bpd
                   ON bpd.ProductID = lsc.ClassificationID AND lt.EnterpriseID = bpd.EnterpriseID
WHERE lt.EnterpriseID = {model.EnterpriseID} AND lt.OrganizationType=2 AND lt.DataDate BETWEEN '{model.BeginDate}' AND '{model.EndDate}'  -- 物流运费猪只详情信息表
UNION
SELECT CONCAT(RAND() * 10000000)  AS Guid,lsc.AmountTotal AS Amount
FROM nxin_qlw_business.ls_transportexpenses AS lt
         INNER JOIN nxin_qlw_business.ls_transportexpensesdetail AS ltd ON lt.NumericalOrder = ltd.NumericalOrder
         INNER JOIN nxin_qlw_business.ls_transportexpensesdetail_temporarypigtransport AS ltm
                    ON lt.NumericalOrder = ltm.NumericalOrder
         INNER JOIN nxin_qlw_business.BIZ_Related AS br
                    ON br.RelatedType = 201610210104402122 AND br.ParentType = 1612230944450000101 AND
                       br.ChildType = 2207300028100000109
                        AND br.ChildValueDetail = ltm.NumericalOrder AND br.ParentValueDetail = ltd.NumericalOrderDetail
         INNER JOIN nxin_qlw_business.LS_CarriageDetail AS lsc ON lsc.NumericalOrderDetail = br.ChildValue
         INNER JOIN nxin_qlw_business.LS_Carriage AS ls ON ls.NumericalOrder = lsc.NumericalOrder
         LEFT JOIN nxin_qlw_business.biz_productdetail AS bpd
                   ON bpd.ProductID = lsc.ClassificationID AND lt.EnterpriseID = bpd.EnterpriseID
WHERE lt.EnterpriseID = {model.EnterpriseID} AND lt.OrganizationType=2 AND lt.DataDate BETWEEN '{model.BeginDate}' AND '{model.EndDate}'  -- 物流运费临时猪只详情信息表
UNION
SELECT CONCAT(RAND() * 10000000)  AS Guid,lsc.AmountTotal AS Amount
FROM nxin_qlw_business.ls_transportexpenses AS lt
         INNER JOIN nxin_qlw_business.ls_transportexpensesdetail AS ltd ON lt.NumericalOrder = ltd.NumericalOrder
         INNER JOIN nxin_qlw_business.ls_transportexpensesdetail_materialtransport AS ltm
                    ON lt.NumericalOrder = ltm.NumericalOrder
         INNER JOIN nxin_qlw_business.BIZ_Related AS br
                    ON br.RelatedType = 201610210104402122 AND br.ParentType = 1612230944450000101 AND
                       br.ChildType = 2207300028100000109
                        AND br.ChildValueDetail = ltm.NumericalOrder AND br.ParentValueDetail = ltd.NumericalOrderDetail
         INNER JOIN nxin_qlw_business.LS_CarriageDetail AS lsc ON lsc.NumericalOrderDetail = br.ChildValue
         INNER JOIN nxin_qlw_business.LS_Carriage AS ls ON ls.NumericalOrder = lsc.NumericalOrder
         LEFT JOIN nxin_qlw_business.biz_productdetail AS bpd
                   ON bpd.ProductID = lsc.ClassificationID AND lt.EnterpriseID = bpd.EnterpriseID
WHERE lt.EnterpriseID = {model.EnterpriseID} AND lt.OrganizationType=2 AND lt.DataDate BETWEEN '{model.BeginDate}' AND '{model.EndDate}'; -- 物流运费饲料详情信息表 ";
            return _context.JXCDataSet.FromSqlRaw(sql).ToList();
        }
        /// <summary>
        /// 采购运费 王增延提供`
        /// </summary>
        /// <returns></returns>
        public List<JXCModel> GetCaiGouList(FMSAccoCheckResultModel model)
        {
            //            string sql = $@" -- 运费
            //SELECT CONCAT(RAND() * 10000000)  AS Guid,lsc.Amount
            //FROM nxin_qlw_business.ls_transportexpenses AS lt
            //         INNER JOIN nxin_qlw_business.ls_transportexpensesdetail AS ltd ON lt.NumericalOrder = ltd.NumericalOrder
            //         INNER JOIN nxin_qlw_business.ls_transportexpensesdetail_pigtransport AS ltm
            //                    ON lt.NumericalOrder = ltm.NumericalOrder
            //         INNER JOIN nxin_qlw_business.BIZ_Related AS br
            //                    ON br.RelatedType = 201610210104402122 
            //                    AND br.ParentType = 1612230944450000101 
            //                    AND br.ChildType = 2207300028100000109
            //					AND br.ChildValueDetail = ltm.NumericalOrder 
            //                    AND br.ParentValueDetail = ltd.NumericalOrderDetail AND br.ParentValueDetail = ltm.NumericalOrderDetail
            //         INNER JOIN nxin_qlw_business.LS_CarriageDetail AS lsc ON lsc.NumericalOrderDetail = br.ChildValue
            //         INNER JOIN nxin_qlw_business.LS_Carriage AS ls ON ls.NumericalOrder = lsc.NumericalOrder
            //         LEFT JOIN nxin_qlw_business.biz_productdetail AS bpd
            //                   ON bpd.ProductID = lsc.ClassificationID 
            //                   AND lt.EnterpriseID = bpd.EnterpriseID
            //WHERE lt.EnterpriseID = {model.EnterpriseID}  
            //  AND ls.DataDate BETWEEN '{model.BeginDate}' AND '{model.EndDate}'
            //  AND lt.OrganizationType=1
            //UNION ALL
            //SELECT CONCAT(RAND() * 10000000)  AS Guid,lsc.Amount
            //FROM nxin_qlw_business.ls_transportexpenses AS lt
            //         INNER JOIN nxin_qlw_business.ls_transportexpensesdetail AS ltd ON lt.NumericalOrder = ltd.NumericalOrder
            //         INNER JOIN nxin_qlw_business.ls_transportexpensesdetail_temporarypigtransport AS ltm
            //                    ON lt.NumericalOrder = ltm.NumericalOrder
            //         INNER JOIN nxin_qlw_business.BIZ_Related AS br
            //                    ON br.RelatedType = 201610210104402122 
            //                    AND br.ParentType = 1612230944450000101 
            //                    AND br.ChildType = 2207300028100000109
            //					AND br.ChildValueDetail = ltm.NumericalOrder 
            //                    AND br.ParentValueDetail = ltd.NumericalOrderDetail  AND br.ParentValueDetail = ltm.NumericalOrderDetail
            //         INNER JOIN nxin_qlw_business.LS_CarriageDetail AS lsc ON lsc.NumericalOrderDetail = br.ChildValue
            //         INNER JOIN nxin_qlw_business.LS_Carriage AS ls ON ls.NumericalOrder = lsc.NumericalOrder
            //         LEFT JOIN nxin_qlw_business.biz_productdetail AS bpd
            //                   ON bpd.ProductID = lsc.ClassificationID
            //                   AND lt.EnterpriseID = bpd.EnterpriseID
            //WHERE lt.EnterpriseID = {model.EnterpriseID}
            //  AND ls.DataDate BETWEEN '{model.BeginDate}' AND '{model.EndDate}'
            //  AND lt.OrganizationType=1 ";
            string sql = $@" SELECT CONCAT(RAND() * 10000000)  AS Guid,t2.AmountTotal AS Amount
 FROM `nxin_qlw_business`.LS_Carriage AS t1 
 INNER JOIN `nxin_qlw_business`.LS_CarriageDetail AS t2 ON t1.NumericalOrder=t2.NumericalOrder
 INNER JOIN `nxin_qlw_business`.BIZ_Related AS t3 ON t2.NumericalOrderDetail=t3.ChildValue AND t3.RelatedType=201610210104402122 AND t3.ParentType=1612230944450000101 AND t3.ChildType=2207300028100000109
 INNER JOIN(
 SELECT  (CASE WHEN f1.AppID=2207300028100000109 THEN 1 ELSE 2 END) AS BusinessType,f1.NumericalOrder,f1.OrganizationType,f1.OrganizationID,f2.NumericalOrderDetail,f2.YHBatchID AS BatchID,f2.ProductID FROM `nxin_qlw_business`.ls_transportexpenses AS f1 
 INNER JOIN `nxin_qlw_business`.ls_transportexpensesdetail AS f2 ON f1.NumericalOrder=f2.NumericalOrder
 WHERE f1.EnterpriseID={model.EnterpriseID}
 ) AS t4 ON t3.ChildValueDetail=t4.NumericalOrder AND t3.ParentValueDetail=t4.NumericalOrderDetail
 WHERE EnterpriseID={model.EnterpriseID} AND DataDate BETWEEN '{model.BeginDate}' AND '{model.EndDate}'    AND BusinessType=2 AND OrganizationType=1
 #饲料 BusinessType=1
 #猪只  BusinessType=2
 #猪场 OrganizationType=1
 #服务部 OrganizationType=2";
            return _context.JXCDataSet.FromSqlRaw(sql).ToList();
        }

        #region 一致性检查详情

        /// <summary>
        /// 存货数据-当前结账会计期间开始和结束日期， 汇总方式一 - 商品代号、汇总方式二空，其他查询条件默认
        /// EndingQuantity：期末数量
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<StockResult> GetStockDataByProduct(FMSAccoCheckResultModel model, string summaryType)
        {
            List<WarehouseModel> warehouseList = new List<WarehouseModel>();
            //ResultModel<WarehouseModel> warehouseResult = _httpClientUtil1.PostJsonAsync<ResultModel<WarehouseModel>>(_hostCongfiguration.NxinGatewayUrl + "api/nxin.yiyingmei.getwarehouselistoutside/1.0", new { EnterpriseID = model.EnterpriseID, CostCalculations = 1 }).Result;
            ResultModel<WarehouseModel> warehouseResult = _httpClientUtil1.PostJsonAsync<ResultModel<WarehouseModel>>(_hostCongfiguration.QlwServiceHost + $@"/api/SAOuterGate/GetWarehouseListOutside", new { EnterpriseID = model.EnterpriseID, CostCalculations = 1 }).Result;
            if (warehouseResult.Code == 0)
            {
                warehouseList = warehouseResult.Data;
            }
            string url = _hostCongfiguration.ReportService + $@"/api/RptStockRelated/GetStockSummaryReport";
            var param = new
            {
                WarehouseName_id = string.Join(',', warehouseList.Select(s => s.WarehouseID).ToArray()),
                Begindate = model.BeginDate,
                Enddate = model.EndDate,
                MeasurementUnit = 0,
                ExactNumber = 2,
                UseMidTable = 1,
                IsAuthority = true,
                IsHasBatchnumber = true,
                DataSource = 0,
                SummaryType1 = summaryType,
                IsGroupByEnteCate = false,
                GroupID = model.GroupID,
                EnteID = model.EnterpriseID,
                MenuParttern = 0,
                Boid = model.OwnerID
            };
            ResultModel<StockResult> result = _httpClientUtil1.PostJsonAsync<ResultModel<StockResult>>(url, param).Result;
            return result.Code == 0 ? result.Data : new List<StockResult>();
        }

        /// <summary>
        /// 应收账款汇总表接口
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="summaryStr">c.EnterpriseName </param>
        public List<ReceivableSummaryDataResult> ReceivableSummaryByType(FMSBusResultModel model, string summaryType)
        {
            //c.EnterpriseName   cs.CustomerName
            string url = _hostCongfiguration.ReportService + "/api/RptReceivableRelated/GetReceivableSummaryReport";
            var param = new ReceivableSummaryRequest()
            {
                Begindate = model.BeginDate,
                Boid = model.OwnerID.ToString(),
                CanWatchEntes = new List<string>() { model.EnterpriseID.ToString() },
                DataSource = 0,
                DiscountSource = 0,
                EndTime = Convert.ToDateTime(model.EndDate),
                Enddate = model.EndDate,
                EnteID = model.EnterpriseID,
                GroupID = model.GroupID,
                MenuParttern = "0",
                OwnEntes = new List<string>() { model.EnterpriseID.ToString() },
                ScopeControl = 1,
                ShowMinus = "y",
                StartTime = Convert.ToDateTime(model.BeginDate),
                SummaryT1Rank = "-1",
                SummaryT2Rank = "-1",
                SummaryT3Rank = "-1",
                SummaryType1 = summaryType,
                eDuZhanBiGongShi = 0,
                isGift = "-1",
                isJinRong = false
            };
            var result = _httpClientUtil1.PostJsonAsync<ResultModel<ReceivableSummaryDataResult>>(url, param).Result;
            if (result.ResultState)
            {
                return result.Data;
            }
            else
            {
                return new List<ReceivableSummaryDataResult>();
            }
        }
        /// <summary>
        /// 应付账款汇总表接口 同PayableSummary，汇总方式不同
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="summaryStr">c.EnterpriseName </param>
        public List<PayableSummaryDataResult> PayableSummaryByType(FMSBusResultModel model, string summaryType)
        {
            string url = _hostCongfiguration.ReportService + "/api/RptPayableRelated/GetPayableSummaryReport";

            var param = new ReceivableSummaryRequest()
            {
                Begindate = model.BeginDate,
                Boid = model.OwnerID.ToString(),
                CanWatchEntes = new List<string>() { model.EnterpriseID.ToString() },
                DataSource = 0,
                Enddate = model.EndDate,
                EnteID = model.EnterpriseID,
                GroupID = model.GroupID,
                MenuParttern = "0",
                OwnEntes = new List<string>() { model.EnterpriseID.ToString() },
                SummaryT1Rank = "-1",
                SummaryT2Rank = "-1",
                SummaryT3Rank = "-1",
                SummaryType1 = summaryType,
                SummaryType1Name = null,
            };
            var result = _httpClientUtil1.PostJsonAsync<ResultModel<PayableSummaryDataResult>>(url, param).Result;
            if (result.ResultState)
            {
                return result.Data;
            }
            else
            {
                return new List<PayableSummaryDataResult>();
            }
        }
        /// <summary>
        /// 获取往来余额-按科目
        /// </summary>
        /// <returns></returns>
        public List<SettleReceiptBalance> SettleReceiptBalanceByCode(FMSBusResultModel model, string AccoSubjectCode)
        {
            string url = _hostCongfiguration.ReportService + "/api/RetSettleReceiptBalance/GetBalanceReport";
            var data = new SettleReceiptRequest()
            {
                Begindate = Convert.ToDateTime(model.BeginDate),
                Enddate = Convert.ToDateTime(model.EndDate),
                MenuParttern = "0",
                EnterpriseList_id = model.EnterpriseID.ToString(),
                EnteID = model.EnterpriseID,
                GroupID = model.GroupID,
                AccountingType = "2",
                DataSource = 0,
                AccountingSubjectsRadio = "",
                SubjectLevel = 1
            };
            var result = _httpClientUtil1.PostJsonAsync<ResultModel<SettleReceiptBalance>>(url, data).Result;
            if (result.ResultState && result.Data != null)
            {
                var temptList = new List<SettleReceiptBalance>();
                var resultList = new List<SettleReceiptBalance>();
                // 过滤
                if (!string.IsNullOrWhiteSpace(AccoSubjectCode))
                {
                    temptList = result.Data.Where(x => x.AccoSubjectCode.StartsWith(AccoSubjectCode))?.ToList();
                }
                // 过滤没有金额数据
                temptList = temptList?.Where(m => m.Show_BegCredit != 0 || m.Show_BegDebit != 0 || m.Credit != 0 || m.Debit != 0 || m.Show_LastCredit != 0 || m.Show_LastDebit != 0).ToList();

                temptList?.ForEach(x => { resultList.Add(Dispatch(x, 1)); });

                List<SettleReceiptBalance> _res = resultList.GroupBy(x => new
                {
                    x.EnterpriseName,
                    x.AccoSubjectCode,
                    x.AccoSubjectFullName,
                    x.CustomerID,
                    x.CustomerName,
                    x.AccoSubjectID,
                    x.AccoSubjectName,
                    x.AccoSubjectSystemID,
                    x.IsLorR,
                    x.PersonID,
                    x.Rank
                }).Select(x =>
                    new SettleReceiptBalance
                    {
                        EnterpriseName = x.Key.EnterpriseName,
                        AccoSubjectCode = x.Key.AccoSubjectCode,
                        AccoSubjectFullName = x.Key.AccoSubjectFullName,
                        CustomerID = x.Key.CustomerID,
                        CustomerName = x.Key.CustomerName,
                        AccoSubjectID = x.Key.AccoSubjectID,
                        AccoSubjectName = x.Key.AccoSubjectName,
                        AccoSubjectSystemID = x.Key.AccoSubjectSystemID,
                        IsLorR = x.Key.IsLorR,
                        PersonID = x.Key.PersonID,
                        Rank = x.Key.Rank,
                        Credit = x.Sum(n => n.Credit),
                        Debit = x.Sum(n => n.Debit),
                        fsCredit = x.Sum(n => n.fsCredit),
                        fsDebit = x.Sum(n => n.fsDebit),
                        qcCredit = x.Sum(n => n.qcCredit),
                        qcDebit = x.Sum(n => n.qcDebit)
                    }
                  ).ToList();
                _res = _res?.Where(m => m.Rank == 1)?.ToList();
                return _res;
            }
            else
            {
                return new List<SettleReceiptBalance>();
            }
        }
        /// <summary>
        /// 截取级次数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private SettleReceiptBalance Dispatch(SettleReceiptBalance item, int level)
        {
            if (item != null)
            {
                //级次
                int index = (item.AccoSubjectCode.Length - 2) / 2;
                // 获取编码
                item.Rank = level;
                if (index <= level)
                {
                    return item;
                }

                string code = item.AccoSubjectCode.Substring(0, 2 + (level * 2));
                //获取名称
                var tmp = item.AccoSubjectFullName.Split('/');
                string name = "";
                if (tmp != null && tmp.Length >= level)
                {
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        if (i < level)
                        {
                            name += tmp[i] + '/';
                        }
                    }
                }
                SettleReceiptBalance _item = new SettleReceiptBalance()
                {
                    Rank = level,
                    AccoSubjectCode = code,
                    EnterpriseName = item.EnterpriseName,
                    AccoSubjectFullName = name.TrimEnd('/'),
                    CustomerName = item.CustomerName,
                    IsLorR = item.IsLorR,
                    Credit = item.Credit,
                    Debit = item.Debit,
                    qcCredit = item.qcCredit,
                    qcDebit = item.qcDebit,
                    fsCredit = item.fsCredit,
                    fsDebit = item.fsDebit,
                    CustomerID = item.CustomerID
                };
                return _item;
            }
            return null;
        }
        /// <summary>
        /// 收付款单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<PaymentreceivablesAndSettlereceipt> GetPaymentreceivables(FMSBusResultModel model)
        {
            //string url = "";
            //if (_hostCongfiguration.QlwServiceHost.Contains("sqlw.nxin.com"))
            //{
            //    url = "http://demo.s.qlw.nxin.com/api/FDPaymentReceiveSettle/GetPaymentreceivables";
            //}
            //else
            //{
            //    url = _hostCongfiguration.QlwServiceHost + "/api/FDPaymentReceiveSettle/GetPaymentreceivables";
            //}
            string url = _hostCongfiguration.QlwServiceHost + "/api/FDPaymentReceiveSettle/GetPaymentreceivables";
            var result = _httpClientUtil1.PostJsonAsync<ResultModel<PaymentreceivablesAndSettlereceipt>>(url, model).Result;
            if (result.ResultState)
            {
                return result.Data;
            }
            else
            {
                return new List<PaymentreceivablesAndSettlereceipt>();
            }
        }
        /// <summary>
        /// 收付会计凭证数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<PaymentreceivablesAndSettlereceipt> GetSettlereceipt(FMSBusResultModel model)
        {
            //string url = "";
            //if(_hostCongfiguration.QlwServiceHost.Contains( "sqlw.nxin.com"))
            //{
            //    url = "http://demo.s.qlw.nxin.com/api/FDPaymentReceiveSettle/GetSettlereceipt";
            //}
            //else
            //{
            //    url = _hostCongfiguration.QlwServiceHost + "/api/FDPaymentReceiveSettle/GetSettlereceipt";
            //}   
            string url = _hostCongfiguration.QlwServiceHost + "/api/FDPaymentReceiveSettle/GetSettlereceipt";
            var result = _httpClientUtil1.PostJsonAsync<ResultModel<PaymentreceivablesAndSettlereceipt>>(url, model).Result;
            if (result.ResultState)
            {
                return result.Data;
            }
            else
            {
                return new List<PaymentreceivablesAndSettlereceipt>();
            }
        }

        /// <summary>
        /// 应付账款汇总表接口 同PayableSummary，汇总方式不同
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="summaryStr">c.EnterpriseName </param>
        public List<PayableSummaryDataResult> PayableSummaryByCon(PayableSummaryModel model, string summaryType)
        {
            string url = _hostCongfiguration.ReportService + "/api/RptPayableRelated/GetPayableSummaryReport";

            var param = new PayableSummaryRequest()
            {
                Begindate = model.BeginDate,
                Boid = model.OwnerID.ToString(),
                CanWatchEntes = new List<string>() { model.EnteID.ToString() },
                DataSource = 0,
                Enddate = model.EndDate,
                EnteID = model.EnteID,
                GroupID = model.GroupID,
                MenuParttern = "0",
                OwnEntes = new List<string>() { model.EnteID.ToString() },
                SummaryT1Rank = "-1",
                SummaryT2Rank = "-1",
                SummaryT3Rank = "-1",
                SummaryType1 = summaryType,
                SummaryType1Name = null,
                SupplierName_id = model.SupplierName_id,
                SupplierName = model.SupplierName
            };
            var result = _httpClientUtil1.PostJsonAsync<ResultModel<PayableSummaryDataResult>>(url, param).Result;
            if (result.ResultState)
            {
                return result.Data;
            }
            else
            {
                return new List<PayableSummaryDataResult>();
            }
        }
        #endregion



        #region 一键结账公式规则
        /// <summary>
        /// 获取启用的校验规则
        /// </summary>
        /// <param name="EnterpriseID"></param>
        /// <returns></returns>
        public List<FM_AccoCheckRuleODataEntity> GetAaccocheckRuleList(string AccoCheckType, string EnterpriseID)
        {
            FormattableString sql = $@" SELECT 
                                        a.RecordID,	
                                        CONVERT(a.EnterpriseID USING utf8mb4) EnterpriseID,
                                        ent.EnterpriseName,
                                        CONVERT(a.AccoCheckType USING utf8mb4) AccoCheckType,
                                        CONVERT(a.MasterDataSource USING utf8mb4) MasterDataSource,
                                        a.MasterFormula,a.MasterSecFormula,
                                        CONVERT(a.FollowDataSource USING utf8mb4) FollowDataSource,
                                        a.FollowFormula,a.FollowSecFormula,
                                        CONVERT(a.CheckValue USING utf8mb4) CheckValue,
                                        CONVERT(a.OwnerID USING utf8mb4) OwnerID,
                                        hr.Name AS OwnerName,
                                        a.IsUse,a.CreatedDate,a.ModifiedDate
                                        FROM 
                                        `nxin_qlw_business`.FM_AccoCheckRule a
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` hr ON a.OwnerID=hr.BO_ID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON a.EnterpriseID=ent.EnterpriseID
                                        WHERE a.EnterpriseID={EnterpriseID} and a.IsUse=true and  a.AccoCheckType={AccoCheckType}";
            var list = _context.FM_AccoCheckRuleDataSet.FromSqlInterpolated(sql).ToList();
            return list;
        }
        /// <summary>
        /// 公式解析
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="formula">中文公式</param>
        /// <param name="secformula">编码公式</param>
        /// <returns></returns>
        public List<RuleEntity> ResolveFormulaSet(string appid, string formula, string secformula)
        {
            if (string.IsNullOrEmpty(secformula)) return new List<RuleEntity>();
            string[] arr1 = formula.Split(new string[] { "+", "-", "*", "/" }, StringSplitOptions.RemoveEmptyEntries);
            string[] arr2 = secformula.Split(new string[] { "+", "-", "*", "/" }, StringSplitOptions.RemoveEmptyEntries);
            //拆运算符
            //Regex reg = new Regex(@"[\+\-\×\÷]", RegexOptions.IgnoreCase);
            //MatchCollection matches = reg.Matches(formula);
            //List<string> arr_op = new List<string>();
            //List<string> arr_opM = new List<string>();
            //foreach (Match op in matches)
            //{
            //    arr_opM.Add(op.Value);
            //    arr_op.Add(op.Value.TrimEnd('[').TrimEnd('('));
            //}
            List<RuleEntity> ruleEntities = new List<RuleEntity>();
            for (int i = 0; i < arr2.Length; i++)
            {
                RuleEntity entity = new RuleEntity();
                entity.CellFormularName = arr1[i];
                entity.CellFormular = arr2[i];
                string currentCell = arr2[i].TrimStart('{').TrimEnd('}');
                //针对科目拆分
                if (currentCell.IndexOf("[") >= 0 && currentCell.IndexOf("]") >= 0)
                {
                    string accsubjetFormula = currentCell.Substring(currentCell.IndexOf('[') + 1, currentCell.IndexOf(']') - 1);
                    currentCell = currentCell.Replace("[" + accsubjetFormula + "]", "");
                    //科目编码
                    if (accsubjetFormula.IndexOf('&') >= 0)
                    {
                        string[] accsubjetArr = accsubjetFormula.Split('&', StringSplitOptions.RemoveEmptyEntries);
                        entity.AccoSubjectCode = accsubjetArr[0];
                        entity.AccoSubjectID = accsubjetArr.Length > 2 ? accsubjetArr[1] : "";
                    }
                    //科目大类
                    if (accsubjetFormula.IndexOf('|') >= 0)
                    {
                        string[] accsubjetArr = accsubjetFormula.Split('|', StringSplitOptions.RemoveEmptyEntries);
                        entity.AccoSubjectType = accsubjetArr[0];
                        entity.AccoSubjectID = accsubjetArr.Length > 2 ? accsubjetArr[1] : "";
                    }
                }
                //针对跨数据源拆分
                if (currentCell.IndexOf("【") >= 0 && currentCell.IndexOf("】") >= 0)
                {
                    string dataSource = currentCell.Substring(currentCell.IndexOf('【') + 1, currentCell.IndexOf('】') - 1);
                    entity.DataSource = dataSource;
                    currentCell = currentCell.Replace("【" + dataSource + "】", "");
                }
                //拆分单元格数据
                if (currentCell.IndexOf("(") >= 0 && currentCell.IndexOf(")") >= 0)
                {
                    string propertyFormula = currentCell.Substring(currentCell.IndexOf('(') + 1, currentCell.IndexOf(')') - 1);
                    List<AccocheckFormulaProperty> list = new List<AccocheckFormulaProperty>();
                    if (appid == "1")
                    {
                        list = _accocheckFormulaProperty.GetFormulaProperty().Where(s => s.FormulaPid == entity.DataSource).ToList();
                    }
                    else
                    {
                        entity.DataSource = appid;
                        list = _accocheckFormulaProperty.GetFormulaProperty().Where(s => s.FormulaPid == appid).ToList();
                    }
                    entity.PropertyID = propertyFormula;
                    entity.PropertyValue = list.Where(s => s.FormulaID == propertyFormula).FirstOrDefault().FormulaValue;
                    currentCell = currentCell.Replace("(" + propertyFormula + ")", "");
                }
                ruleEntities.Add(entity);
            }
            return ruleEntities;
        }
        /// <summary>
        /// 获取该公式数据
        /// </summary>
        /// <param name="formula">中文公式</param>
        /// <param name="secformula">编码公式</param>
        /// <param name="dataSourceEntities">业务数据</param>
        /// <param name="ruleEntities">规则</param>
        /// <param name="formulamsg">输出值</param>
        /// <returns></returns>
        public decimal SetFormulaValue( string formula, string secformula, List<DataSourceEntity> dataSourceEntities, List<RuleEntity> ruleEntities, out string formulamsg) 
        {
            formulamsg = formula;
            foreach (var entity in ruleEntities)
            {
                //获取需要的对应业务数据
                var data = dataSourceEntities.Where(s => s.DataSource == entity.DataSource).FirstOrDefault().EntityList;
                decimal value = 0M;
                List<EntitySubClass> list = new List<EntitySubClass>();
                //科目过滤
                if (!string.IsNullOrEmpty(entity.AccoSubjectCode))
                {
                    list = data.Where(s => s.AccoSubjectCode.StartsWith(entity.AccoSubjectCode)).ToList<EntitySubClass>();
                }
                //科目类别过滤
                else if (!string.IsNullOrEmpty(entity.AccoSubjectType))
                {
                    list = data.Where(s => s.AccoSubjectType == entity.AccoSubjectType).ToList<EntitySubClass>();
                }
                //成本汇总表特殊处理
                else if (entity.DataSource == AccocheckDataSource.成本汇总表.GetValue().ToString())
                {
                    if (entity.PropertyID == "1905141116550000122")
                        list = data.Where(s => s.SummaryType1Name == "原材料").ToList<EntitySubClass>();
                    else if (entity.PropertyID == "1905141116550000123")
                        list = data.Where(s => s.SummaryType1Name == "包装物").ToList<EntitySubClass>();
                    else if (entity.PropertyID == "1905141116550000124")
                        list = data.Where(s => s.SummaryType1Name == "半成品").ToList<EntitySubClass>();
                    else if (entity.PropertyID == "1905141116550000125")
                        list = data.Where(s => s.SummaryType1Name == "产成品").ToList<EntitySubClass>();
                    else
                        list = data;
                }
                else
                {
                    list = data;
                }
                //计算值
                foreach (var item in list)
                {
                    Type type = item.GetType();
                    decimal outvalue = 0M;
                    if (!decimal.TryParse(type.GetProperty(entity.PropertyValue).GetValue(item).ToString(), out outvalue))
                    { outvalue = 0M; }
                    value += outvalue;
                }
                entity.OutValue = value;
                //替换值
                secformula = secformula.Replace(entity.CellFormular, value.ToString());
                formulamsg = formulamsg.Replace(entity.CellFormularName, entity.CellFormularName + value.ToString("N2"));
            }
            //formulamsg = formulamsg.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", "");
            decimal result = CalculateValue(secformula);
            if (ruleEntities.Count() > 1)
            {
                formulamsg += $@",合计{result.ToString("N2")}";
            }
            return result;
        }
        public decimal CalculateValue(string formula)
        {
            try
            {
                formula = formula.Replace("×", "*").Replace("÷", "/").Replace("[", "").Replace("]", "");
                System.Data.DataTable dt = new System.Data.DataTable();
                var obj = Convert.ToDecimal(dt.Compute(formula, "").ToString());
                return obj;
            }
            catch (Exception ex)
            {
                return 0;
            }
            return 0;
        }

        public List<DataSourceEntity> GetDataSourceEntities(List<FM_AccoCheckRuleODataEntity> ruleList, FMSAccoCheckResultModel model)
        {
            List<DataSourceEntity> dataSourceEntities = new List<DataSourceEntity>();
            List<RuleEntity> ruleEntities = new List<RuleEntity>();
            foreach (var item in ruleList)
            {
                ruleEntities.AddRange(item.MasterRules);
                ruleEntities.AddRange(item.FollowRules);
            }
            List<string> appids = ruleEntities.Select(s => s.DataSource).ToList();
            appids = appids.GroupBy(p => p).Select(p => p.Key).ToList();//去重
            foreach (var appid in appids)
            {
                switch (appid)
                {
                    case "1612121627090000101":
                        var balancesList = this.RetSubjectBalance(model, 1);
                        dataSourceEntities.Add(new DataSourceEntity(AccocheckDataSource.科目余额表.GetValue().ToString(), balancesList.ToList<EntitySubClass>()));
                        break;
                    case "1612121633410000101":
                        var receiptList = this.SettleReceiptBalance(model);
                        dataSourceEntities.Add(new DataSourceEntity(AccocheckDataSource.往来余额表.GetValue().ToString(), receiptList.ToList<EntitySubClass>()));
                        break;
                    case "1612121622160000101":
                        var fundsList = this.FundsSummary(model);
                        dataSourceEntities.Add(new DataSourceEntity(AccocheckDataSource.资金汇总表.GetValue().ToString(), fundsList.ToList<EntitySubClass>()));
                        break;
                    case "1612121126590000101":
                        var payableList = this.PayableSummary(model);
                        dataSourceEntities.Add(new DataSourceEntity(AccocheckDataSource.应付账款汇总表.GetValue().ToString(), payableList.ToList<EntitySubClass>()));
                        break;
                    case "1612121330480000101":
                        var receivableList = this.ReceivableSummary(model);
                        dataSourceEntities.Add(new DataSourceEntity(AccocheckDataSource.应收账款汇总表.GetValue().ToString(), receivableList.ToList<EntitySubClass>()));
                        break;
                    case "1612062114270000101":
                        var salesummarys = this.GetSaleSummaryData(model);
                        dataSourceEntities.Add(new DataSourceEntity(AccocheckDataSource.销售汇总表.GetValue().ToString(), salesummarys.ToList<EntitySubClass>()));
                        break;
                    case "1612121053430000101":
                        var purcheasesummarys = this.GetPurchaseSummaryDataList(model);
                        dataSourceEntities.Add(new DataSourceEntity(AccocheckDataSource.采购汇总表.GetValue().ToString(), purcheasesummarys.ToList<EntitySubClass>()));
                        break;
                    case "1707311529570000105":
                        var freughtList = this.getCarriageAmount(model);
                        dataSourceEntities.Add(new DataSourceEntity(AccocheckDataSource.运费汇总表.GetValue().ToString(), freughtList.ToList<EntitySubClass>()));
                        break;
                    case "1707261523090000101":
                        var stockdatas = this.GetStockDataByAbstract(model);
                        dataSourceEntities.Add(new DataSourceEntity(AccocheckDataSource.存货汇总表.GetValue().ToString(), stockdatas.ToList<EntitySubClass>()));
                        break;
                    case "1801111409580000101":
                        var depreciation = this.AssetsCardDepreciationSummary(model);
                        dataSourceEntities.Add(new DataSourceEntity(AccocheckDataSource.折旧汇总表.GetValue().ToString(), depreciation.ToList<EntitySubClass>()));
                        break;
                    case "1712251756400000101":
                        var assetsCards = this.AssetsCardInfo(model);
                        dataSourceEntities.Add(new DataSourceEntity(AccocheckDataSource.固定资产卡片报表.GetValue().ToString(), assetsCards.ToList<EntitySubClass>()));
                        break;
                    case "1712061720030000101":
                        var supplies = this.GetSuppliesReport(model);
                        if (supplies.Count > 0)
                            supplies = supplies.Where(s => s.dDate == "合计").ToList();
                        dataSourceEntities.Add(new DataSourceEntity(AccocheckDataSource.物品明细表.GetValue().ToString(), supplies.ToList<EntitySubClass>()));
                        break;
                    case "201512289551945035":
                        var discountsummarys = this.GetMyDiscountSummaryReport(model);
                        dataSourceEntities.Add(new DataSourceEntity(AccocheckDataSource.折扣汇总表.GetValue().ToString(), discountsummarys.ToList<EntitySubClass>()));
                        break;
                    case "1709191611440000105":
                        var yhpigcostList = this.YhPigCostSummary(model);
                        //var yhPigCostAmount = this.YhPigCostEndCostTotal(model);
                        dataSourceEntities.Add(new DataSourceEntity(AccocheckDataSource.养户成本汇总表.GetValue().ToString(), yhpigcostList.ToList<EntitySubClass>()));
                        break;
                    case "1706152008290000101":
                        var costdata = this.GetCostDataList(model, "bd.cDictName");
                        dataSourceEntities.Add(new DataSourceEntity(AccocheckDataSource.成本汇总表.GetValue().ToString(), costdata.ToList<EntitySubClass>()));
                        break;
                    case "2010301143010000101":
                        var pigCostSummary = this.CostSummary(model);
                        var changeCostSummary = this.ChangeCostSummary(model);
                        dataSourceEntities.Add(new DataSourceEntity(AccocheckDataSource.成本变动汇总表.GetValue().ToString(), pigCostSummary.Union(changeCostSummary).ToList<EntitySubClass>()));
                        break;
                    case "2211231846540000109":
                        var pigCostAmount = this.PigCostDetailReportEndCostTotal(model);
                        dataSourceEntities.Add(new DataSourceEntity(AccocheckDataSource.猪场成本明细表.GetValue().ToString(), new List<CostSummary>() { new CostSummary() { pigCostAmount = pigCostAmount } }.ToList<EntitySubClass>()));
                        break;
                    default:
                        break;
                }
            }
            return dataSourceEntities;
        }
        #endregion

    }

}
