using Architecture.Common.HttpClientUtil;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using FinanceManagement.Common.NewMakeVoucherCommon;
using System.Net.Http.Headers;
using DBN.EncrypDecryp;
using FinanceManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;
using static FinanceManagement.Common.NewMakeVoucherCommon.Biz_EnterprisePeriodInfo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Ocsp;
using System.ComponentModel;

namespace FinanceManagement.Common
{
    public class FMBaseCommon
    {
        HttpClientUtil _httpClientUtil1;
        IServiceProvider _serviceProvider;
        HostConfiguration _hostCongfiguration;
        private QlwCrossDbContext _context;
        private ILogger<FMBaseCommon> _logger;
        private const long _inWarehousePid = 1711231120560000101;// 入库pid 
        private const long _outWarehousePid = 1711231121250000101;// 出库pid 
        private const long _transformInWarehouseId = 201512172835330670;// 调拨出库单id 
        private const long _transformOutWarehouseId = 201512172084559185;// 调拨入库单id 
        public FMBaseCommon(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration, QlwCrossDbContext context, IServiceProvider serviceProvider)
        {
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            _context = context;
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetService<ILogger<FMBaseCommon>>();
        }
        /// <summary>
        /// 获取单据字
        /// </summary>
        public async Task<ResultModel<FM_TicketedPoint>> GetSymbol(string enterID)
        {
            var res = await _httpClientUtil1.PostJsonAsync<ResultModel<FM_TicketedPoint>>($"{_hostCongfiguration.QlwServiceHost}/api/BIZTicketedPoint/getTicketedPoint", new { EnterpriseID = enterID });
            return res;
        }
        /// <summary>
        /// 获取权限单位,字符串逗号分隔返回
        /// </summary>
        /// <param name="enteid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public string GetAuthorEnterpise(string enteid, string userid)
        {
            //权限单位集合
            var enterprises = _httpClientUtil1.GetJsonAsync<ResultModel<Enterprise>>($"{_hostCongfiguration.NxinGatewayUrl}api/nxin.permission.enterlistbymenuids.list/2.0?open_req_src=nxin_shuju&menuid=0&enterpriseid={enteid}&boid={userid}").Result;
            var entes = enteid;
            if (enterprises.Data != null)
            {
                if (enterprises.Data.Count > 0)
                {
                    entes = string.Join(",", enterprises.Data.Select(m => m.EnterpriseID));
                }
            }
            return entes;
        }
        /// <summary>
        /// 获取权限单位
        /// </summary>
        /// <param name="enteid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<Enterprise> GetAuthorEnterpiseToModel(string enteid, string userid)
        {
            //权限单位集合
            var enterprises = _httpClientUtil1.GetJsonAsync<ResultModel<Enterprise>>($"{_hostCongfiguration.NxinGatewayUrl}api/nxin.permission.enterlistbymenuids.list/2.0?open_req_src=nxin_shuju&menuid=0&enterpriseid={enteid}&boid={userid}").Result;
            if (enterprises.Data != null)
            {
                return enterprises.Data;
            }
            else
            {
                return new List<Enterprise>();
            }
        }
        /// <summary>
        /// 获取系统选项
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public string OptionConfigValue(string optionId, string enterpriseId = "")
        {
            try
            {
                if (enterpriseId == "")
                {
                    return string.Empty;
                }
                BIZ_OptionConfig optionConfig = null;
                Task<ResultModel> resultModel = _httpClientUtil1.PostJsonAsync<ResultModel>($"{_hostCongfiguration.QlwServiceHost}/api/BIZOptionConfig/getOptionConfig", new { OptionID = optionId, EnterpriseID = enterpriseId });

                if (resultModel != null && resultModel.Result.ResultState && resultModel.Result.Data != null) optionConfig = JsonConvert.DeserializeObject<List<BIZ_OptionConfig>>(resultModel.Result.Data.ToString()).FirstOrDefault();
                return optionConfig != null ? optionConfig.OptionValue : string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 名称：获取系统选项设置值
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="enterpriseId"></param>
        /// https://open.t.nxin.com/api/nxin.usercenter.system.option.get/1.0
        /// <returns></returns>
        public int OptionConfigValueNew(string optionId, string enterpriseId = "", string scopeCode = "4")
        {
            try
            {
                if (enterpriseId == "")
                {
                    return 0;
                }
                OptionConfigNew optionConfig = new OptionConfigNew() { OptionSwitch = 0 };
                Task<ResultModel> resultModel = _httpClientUtil1.GetJsonAsync<ResultModel>($"{_hostCongfiguration.NxinGatewayUrl}api/nxin.usercenter.system.option.get/1.0?open_req_src=nxin_shuju&optionId={optionId}&scopeCode={scopeCode}&enterId={enterpriseId}");

                if (resultModel != null && resultModel.Result.Data != null) optionConfig = JsonConvert.DeserializeObject<List<OptionConfigNew>>(resultModel.Result.Data.ToString()).FirstOrDefault();
                return optionConfig != null ? optionConfig.OptionSwitch : 0;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("获取系统选项异常：" + ex.ToString());
                return 0;
            }
        }
        /// <summary>
        /// 获取月末结账-会计结账
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public ResultModel CheckFMAccount(string ownerID, string dataDate, string appId, string enterpriseID = "")
        {
            try
            {
                Task<ResultModel> resultModel = _httpClientUtil1.PostJsonAsync<ResultModel>($"{_hostCongfiguration.QlwServiceHost}/api/{enterpriseID}/{ownerID}/fm_accocheck/islockform", new { enterpriseID, ownerID, dataDate, appID = appId });
                return resultModel.Result;
            }
            catch (Exception ex)
            {
                return new ResultModel() { Msg = "获取结账异常" };
            }
        }
        /// <summary>
        /// 按结算类型获取摘要
        /// </summary>
        public async Task<List<ReceiptAbstract>> GetReceiptAbstractList(string enterID, string relationId = "2", string EnterDate = "")
        {
            int type = 0;
            string ReceiptType = "-1";
            long EnterpriseIDs = -1;
            if (!String.IsNullOrEmpty(relationId))
            {
                string[] contentID = relationId.ToString().Split('*');
                //如果支出类别根据公司加载
                if (contentID.Length >= 3)
                {
                    if (!String.IsNullOrEmpty(contentID[0]))
                        type = int.Parse(contentID[0]);
                    if (!String.IsNullOrEmpty(contentID[1]))
                        ReceiptType = contentID[1];
                    if (!String.IsNullOrEmpty(contentID[2]))
                        EnterpriseIDs = Convert.ToInt64(contentID[2]);
                }
                //支出类别不需要公司加载
                else if (contentID.Length >= 2 && contentID.Length < 3)
                {
                    if (!String.IsNullOrEmpty(contentID[0]))
                        type = int.Parse(contentID[0]);
                    if (!String.IsNullOrEmpty(contentID[1]))
                        ReceiptType = contentID[1];
                }
                //不需要单据id
                else
                {
                    type = int.Parse(relationId.ToString());
                }
            }
            return await GetReceiptAbstractList(type, ReceiptType, EnterpriseIDs, enterID, EnterDate);
        }
        private async Task<List<ReceiptAbstract>> GetReceiptAbstractList(int id, string ReceiptType = "-1", long EnterpriseIDs = -1, string enterID = "0", string EnterDate = "")
        {
            string SettleSummaryCode = "0";
            switch (id)
            {
                case 0:
                    SettleSummaryCode = "01";
                    break;
                case 1:
                    SettleSummaryCode = "02";
                    break;
                case 2:
                    SettleSummaryCode = "03";
                    break;
                case 3:
                    SettleSummaryCode = String.Empty;
                    break;
            }
            var res = await _httpClientUtil1.PostJsonAsync<ResultModel<ReceiptAbstract>>($"{_hostCongfiguration.QlwServiceHost}/api/BIZSettleSummary/getSettleSummary", new { EnterpriseID = EnterpriseIDs == -1 ? Convert.ToInt64(enterID) : EnterpriseIDs, SettleSummaryCode = SettleSummaryCode, ReceiptType = ReceiptType, EnterDate = string.IsNullOrEmpty(EnterDate) ? DateTime.Now.ToString("yyyy-MM-dd") : EnterDate });
            if (res != null && res.ResultState && res.Data != null)
            {
                return res.Data;
            }
            return new List<ReceiptAbstract>();
        }
        public async Task<List<Biz_Settlesummary>> GetReceiptAbstractAllList(string EnterpriseID, string LastDate = "")
        {
            var res = await _httpClientUtil1.PostJsonAsync<ResultModel<Biz_Settlesummary>>($"{_hostCongfiguration.QlwBase}/WEBAPI/ApiSettlesummary/GetSettlesummary?EnterpriseID={EnterpriseID}&LastDate={(string.IsNullOrEmpty(LastDate) ? DateTime.Now.ToString("yyyy-MM") : LastDate)}", null);
            if (res.Code == 0)
            {
                return res.Data;
            }
            return new List<Biz_Settlesummary>();
        }
        /// <summary>
        /// 获取会计凭证最大号
        /// </summary>
        /// <param name="DataDate"></param>
        /// <returns></returns>
        public long GetMaxNumber(string EnterpriseID, string DataDate)
        {
            var result = _httpClientUtil1.PostJsonAsync<long>($"http://demo.s.qlw.nxin.com/api/CreateVoucher/GetNumber", new { EnterpriseID = EnterpriseID, DataDate = DataDate }).Result;
            return result;
        }

        public async Task<List<Subject>> GetSubjectList(long id, long enteID = 0, string EnterDate = "")
        {
            List<Subject> totalSubject = new List<Subject>();
            ResultModel<Subject> resultModel = new ResultModel<Subject>();
            var requestResult = string.Empty;
            if (id != 0)
                resultModel = await _httpClientUtil1.PostJsonAsync<ResultModel<Subject>>($"{_hostCongfiguration.QlwServiceHost}/api/BIZAccoSubject/getAccoSubject", new { EnterpriseID = enteID, AccoSubjectID = id, EnterDate = string.IsNullOrEmpty(EnterDate) ? DateTime.Now.ToString("yyyy-MM-dd") : EnterDate });
            else
                resultModel = await _httpClientUtil1.PostJsonAsync<ResultModel<Subject>>($"{_hostCongfiguration.QlwServiceHost}/api/BIZAccoSubject/getAccoSubject", new { EnterpriseID = enteID, EnterDate = string.IsNullOrEmpty(EnterDate) ? DateTime.Now.ToString("yyyy-MM-dd") : EnterDate });
            if (resultModel.ResultState)
            {
                totalSubject = resultModel.Data;
                foreach (var item in totalSubject)
                {
                    item.AccoSubjectCode = item.cAccoSubjectCode;
                }
            }
            return totalSubject;
        }
        public List<Biz_Subject> GetSubjectList(string EnterpriseID, AuthenticationHeaderValue authentication)
        {
            ResultModel<Biz_Subject> resultModel = _httpClientUtil1.GetJsonAsync<string, ResultModel<Biz_Subject>>($"{_hostCongfiguration._wgUrl}/base/finance/oq/AccosubjectAnyOData?dDate={DateTime.Now.ToString("yyyy-MM")}&EnterpriseId={EnterpriseID}&$orderby=AccoSubjectCode", null).Result;
            return resultModel.value;
        }
        public List<SortType> GetSortTypeList(string type)
        {
            List<SortType> list = new List<SortType>();
            switch (type)
            {
                case "1911081429200000101":
                    list.Add(new SortType() { SortID = SortTypeEnum.商品分类.GetIntValue(), SortName = "商品分类", Type = type });
                    break;
                case "1911081429200000102":
                    list.Add(new SortType() { SortID = SortTypeEnum.商品分类.GetIntValue(), SortName = "商品分类", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.供应商.GetIntValue(), SortName = "供应商", Type = type });
                    break;
                case "1911081429200000103":
                    list.Add(new SortType() { SortID = SortTypeEnum.供应商.GetIntValue(), SortName = "供应商", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.物品分类.GetIntValue(), SortName = "物品分类", Type = type });
                    break;
                case "1911081429200000104":
                case "1911081429200099105":
                    list.Add(new SortType() { SortID = SortTypeEnum.鸡场.GetIntValue(), SortName = "鸡场", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.品种.GetIntValue(), SortName = "品种", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.批次.GetIntValue(), SortName = "批次", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.厂区.GetIntValue(), SortName = "厂区", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.鸡舍.GetIntValue(), SortName = "鸡舍", Type = type });
                    break;
                case "1911081429200099106":
                    list.Add(new SortType() { SortID = SortTypeEnum.商品代号.GetIntValue(), SortName = "商品代号", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.养殖场.GetIntValue(), SortName = "养殖场", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.商品分类.GetIntValue(), SortName = "商品分类", Type = type });
                    break;
                case "1911081429200099101":
                    list.Add(new SortType() { SortID = SortTypeEnum.销售摘要.GetIntValue(), SortName = "销售摘要", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.商品分类.GetIntValue(), SortName = "商品分类", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.商品代号.GetIntValue(), SortName = "商品代号", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.部门.GetIntValue(), SortName = "部门", Type = type });
                    break;
                case "1911081429200099111":
                    list.Add(new SortType() { SortID = SortTypeEnum.采购摘要.GetIntValue(), SortName = "采购摘要", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.商品分类.GetIntValue(), SortName = "商品分类", Type = type });
                    break;
                case "1911081429200099102":
                    list.Add(new SortType() { SortID = SortTypeEnum.人员.GetIntValue(), SortName = "人员", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.部门.GetIntValue(), SortName = "部门", Type = type });
                    break;
                case "1911081429200099103":
                    list.Add(new SortType() { SortID = SortTypeEnum.人员.GetIntValue(), SortName = "人员", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.部门.GetIntValue(), SortName = "部门", Type = type });
                    break;
                case "1911081429200099104":
                    list.Add(new SortType() { SortID = SortTypeEnum.部门.GetIntValue(), SortName = "部门", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.商品代号.GetIntValue(), SortName = "商品代号", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.猪只类型.GetIntValue(), SortName = "猪只类型", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.猪场.GetIntValue(), SortName = "猪场", Type = type });
                    break;
                case "1911081429200099107":
                    list.Add(new SortType() { SortID = SortTypeEnum.部门.GetIntValue(), SortName = "部门", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.商品代号.GetIntValue(), SortName = "商品代号", Type = type });
                    break;
                case "1911081429200099108":
                    list.Add(new SortType() { SortID = SortTypeEnum.费用性质.GetIntValue(), SortName = "费用性质", Type = type });
                    break;
                case "1911081429200099110":
                    list.Add(new SortType() { SortID = SortTypeEnum.部门.GetIntValue(), SortName = "部门", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.资产类别.GetIntValue(), SortName = "资产类别", Type = type });
                    break;
                case "1911081429200099112":
                    list.Add(new SortType() { SortID = SortTypeEnum.物品分类.GetIntValue(), SortName = "物品分类", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.部门.GetIntValue(), SortName = "部门", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.出入库方式.GetIntValue(), SortName = "出入库方式", Type = type });
                    break;
                case "1911081429200099114":
                    list.Add(new SortType() { SortID = SortTypeEnum.人员.GetIntValue(), SortName = "人员", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.发票类型.GetIntValue(), SortName = "发票类型", Type = type });
                    break;
                case "1911081429200099115":
                    list.Add(new SortType() { SortID = SortTypeEnum.运费摘要.GetIntValue(), SortName = "运费摘要", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.商品分类.GetIntValue(), SortName = "商品分类", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.供应商.GetIntValue(), SortName = "供应商", Type = type });
                    break;
                case "1911081429200099116":
                    list.Add(new SortType() { SortID = SortTypeEnum.商品代号.GetIntValue(), SortName = "商品代号", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.商品分类.GetIntValue(), SortName = "商品分类", Type = type });
                    list.Add(new SortType() { SortID = SortTypeEnum.存货分类.GetIntValue(), SortName = "存货分类", Type = type });
                    break;
                default:
                    break;
            }
            return list;
        }
        public FM_CarryForwardVoucherODataEntity GetDataSource(string type)
        {
            FM_CarryForwardVoucherODataEntity model = new FM_CarryForwardVoucherODataEntity();
            switch (type)
            {
                case "1911081429200000101":
                case "1911081429200099101":
                    model.DataSource = "1612062114270000101";
                    model.DataSourceName = "销售汇总表";
                    break;
                case "1911081429200000102":
                case "1911081429200099111":
                    model.DataSource = "1612121053430000101";
                    model.DataSourceName = "采购汇总表";
                    break;
                case "1911081429200000103":
                    model.DataSource = "2110141622570000109";
                    model.DataSourceName = "物品结算汇总表";
                    break;
                case "1911081429200000104":
                case "1911081429200099105":
                    model.DataSource = "2111091945560000109";
                    model.DataSourceName = "禽养殖成本流转表";
                    break;
                case "1911081429200099106":
                    model.DataSource = "2208151434260000109";
                    model.DataSourceName = "种禽成本流转表/孵化成本流转表/蛋品成本明细表";
                    break;
                case "1911081429200099102":
                    model.DataSource = "1612131431310000101";
                    model.DataSourceName = "薪资汇总表";
                    break;
                case "1911081429200099103":
                    model.DataSource = "1612131432070000101";
                    model.DataSourceName = "福利汇总表";
                    break;
                case "1911081429200099104":
                    model.DataSource = "1612131439999999999";
                    model.DataSourceName = "种猪流转表/仔猪流转表/肥猪流转表/后备猪流转表/饲养成本明细表/养户成本汇总表/费用分摊明细表/会计辅助账";
                    break;
                case "1911081429200099107":
                    model.DataSource = "2210120959160000109";
                    model.DataSourceName = "养户成本汇总表";
                    break;
                case "1911081429200099108":
                    model.DataSource = "2301101437400000109";
                    model.DataSourceName = "费用分摊明细表";
                    break;
                case "1911081429200099109":
                    model.DataSource = "1612121635080000101";
                    model.DataSourceName = "会计辅助账";
                    //model.DataSource = "1612121627090000101";
                    //model.DataSourceName = "科目余额表";
                    break;
                case "1911081429200099110":
                    model.DataSource = "1801111409580000101";
                    model.DataSourceName = "折旧汇总表";
                    break;
                case "1911081429200099112":
                    model.DataSource = "1712061720030000101";
                    model.DataSourceName = "物品明细表";
                    break;
                case "1911081429200099113":
                    model.DataSource = "1911251404260000102";
                    model.DataSourceName = "预收款核销";
                    break;
                case "1911081429200099114":
                    model.DataSource = "1912091322170000102";
                    model.DataSourceName = "税费抵扣查询";
                    break;
                case "1911081429200099115":
                    model.DataSource = "1707311529570000105";
                    model.DataSourceName = "运费汇总表";
                    break;
                case "1911081429200099116":
                    model.DataSource = "1612131439999999998";
                    model.DataSourceName = "成本汇总表/生产成本表";
                    break;
                default:
                    break;
            }
            return model;
        }

        public List<Biz_Enterprise> GetGroupPermissonEnterData(string EnterpriseId, string UserId)
        {
            List<Biz_Enterprise> enterprises = new List<Biz_Enterprise>();
            var url = string.Format(_hostCongfiguration.baseQlwServiceUrl + "/base/permission/UserEnter?EnterpriseID={0}&BO_ID={1}", EnterpriseId, UserId);
            var resultModel = _httpClientUtil1.GetJsonAsync<ResultModel<Biz_Enterprise>>(url).Result;
            if (resultModel.Code == 0)
            {
                enterprises = resultModel.Data.Where(s => s.IsUse == 1).ToList();
            }
            return enterprises;
        }

        /// <summary>
        /// 单位科目(可任何级次)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enteID"></param>
        /// <param name="EnterDate"></param>
        /// <returns></returns>
        public async Task<List<Subject>> GetEnterSubjectList(long id, long enteID = 0, string EnterDate = "")
        {
            List<Subject> totalSubject = new List<Subject>();
            ResultModel<Subject> resultModel = new ResultModel<Subject>();
            var requestResult = string.Empty;
            if (id != 0)
                resultModel = await _httpClientUtil1.PostJsonAsync<ResultModel<Subject>>($"{_hostCongfiguration.QlwServiceHost}/api/BIZAccoSubject/getAccoSubject", new { EnterpriseID = enteID, AccoSubjectID = id, EnterDate = string.IsNullOrEmpty(EnterDate) ? DateTime.Now.ToString("yyyy-MM-dd") : EnterDate });
            else
                resultModel = await _httpClientUtil1.PostJsonAsync<ResultModel<Subject>>($"{_hostCongfiguration.QlwServiceHost}/api/BIZAccoSubject/getAccoSubject", new { EnterpriseID = enteID, EnterDate = string.IsNullOrEmpty(EnterDate) ? DateTime.Now.ToString("yyyy-MM-dd") : EnterDate });
            if (resultModel.ResultState)
            {
                totalSubject = resultModel.Data;
                //foreach (var item in totalSubject)
                //{
                //    item.AccoSubjectCode = item.cAccoSubjectCode;
                //}
            }
            return totalSubject;
        }


        /// <summary>
        /// 获取当前单位的部门
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetMarket(DropSelectSearch searchModel)
        {
            try
            {
                List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
                var param = new
                {
                    EnterpriseIds = searchModel.EnterpriseID
                };
                var resultModel = _httpClientUtil1.PostJsonAsync<ResultModel<Biz_Market>>($"{_hostCongfiguration.NxinGatewayUrl}api/nxin.qlwbase.getmarket/3.0", param).Result;
                if (resultModel.Code == 0)
                {
                    model = (from s in resultModel.Data
                             select new TreeModelODataEntity()
                             {
                                 Id = s.marketId,
                                 cName = s.fullName,
                                 Pid = s.pid.ToString(),
                                 ExtendId = s.isEnd == 1 ? "true" : "false"
                             }).ToList();
                }
                return model;
            }
            catch (Exception ex)
            {
                return new List<TreeModelODataEntity>();
            }
        }

        /// <summary>
        /// 获取当前单位人员信息
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetPerson(DropSelectSearch searchModel)
        {
            try
            {
                List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
                var param = new
                {
                    EnterpriseID = searchModel.EnterpriseID,
                    Type = 4,
                };
                var resultModel = _httpClientUtil1.PostJsonAsync<ResultModel<PM_Person>>($"{_hostCongfiguration.QlwServiceHost}/api/BIZPerson/getPerson", param).Result;
                if (resultModel.ResultState)
                {
                    string value = OptionConfigValue("20200113135321");//启用单据中人员选择下拉过滤离职人员
                    bool opt = value == "1" ? true : false;
                    if (opt)
                    {
                        resultModel.Data.RemoveAll(_ => _.PersonState == "201610290104402103");//过滤离职人员
                    }
                    model = resultModel.Data.Select(Data => new TreeModelODataEntity { Id = Data.PersonID.ToString(), cName = Data.PersonName, ExtendId = Data.UserID.ToString(), Pid = "0" }).ToList();
                    model = model.Where((x, i) => model.FindIndex(z => z.Id == x.Id) == i).ToList();
                }
                return model;
            }
            catch (Exception ex)
            {
                return new List<TreeModelODataEntity>();
            }
        }

        /// <summary>
        /// 获取当前单位的客户信息
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetCustomer(DropSelectSearch searchModel)
        {
            try
            {
                List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
                var param = new
                {
                    EnterpriseID = searchModel.EnterpriseID,
                };
                var resultModel = _httpClientUtil1.PostJsonAsync<ResultModel<FM_Customer>>($"{_hostCongfiguration.QlwServiceHost}/api/CustomerBaseInfo/getBaseCustomer", param).Result;
                if (resultModel.ResultState)
                {
                    model = resultModel.Data.Select(Data => new TreeModelODataEntity { Id = Data.CustomerID, cName = Data.CustomerName }).ToList();
                }
                return model;
            }
            catch (Exception ex)
            {
                return new List<TreeModelODataEntity>();
            }
        }

        /// <summary>
        /// 获取当前单位的客户信息
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public List<FM_Customer> GetCustomerList(string EnterpriseID)
        {
            try
            {
                List<FM_Customer> model = new List<FM_Customer>();
                var param = new
                {
                    EnterpriseID = EnterpriseID,
                };
                var resultModel = _httpClientUtil1.PostJsonAsync<ResultModel<FM_Customer>>($"{_hostCongfiguration.QlwServiceHost}/api/CustomerBaseInfo/getBaseCustomer", param).Result;
                if (resultModel.ResultState)
                {
                    model = resultModel.Data;
                }
                return model;
            }
            catch (Exception ex)
            {
                return new List<FM_Customer>();
            }
        }

        /// <summary>
        /// 获取当前单位的鸡场接口
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetChickenFarm(DropSelectSearch searchModel)
        {
            try
            {
                List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
                var param = new
                {
                    Boid = searchModel.Boid,
                    enteid = searchModel.EnterpriseID,
                };
                var resultModel = _httpClientUtil1.PostJsonAsync<ResultModel<ChickenFarm>>($"{_hostCongfiguration.QlwServiceHost}/api/RptEggManagerReport/GetJurisdictionChickenFarmList", param).Result;
                if (resultModel.ResultState)
                {
                    model = resultModel.Data.Select(Data => new TreeModelODataEntity { Id = Data.ID, cName = Data.Name }).ToList();
                }
                return model;
            }
            catch (Exception ex)
            {
                return new List<TreeModelODataEntity>();
            }
        }
        /// <summary>
        /// 获取当前单位的批次接口
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetBatching(DropSelectSearch searchModel)
        {
            try
            {
                List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
                var param = new
                {
                    enteid = searchModel.EnterpriseID,
                };
                var resultModel = _httpClientUtil1.PostJsonAsync<ResultModel<ChickenFarm>>($"{_hostCongfiguration.QlwServiceHost}/api/RptEggManagerReport/GetBatchingSetList", param).Result;
                if (resultModel.ResultState)
                {
                    model = resultModel.Data.Select(Data => new TreeModelODataEntity { Id = Data.ID, cName = Data.Name }).ToList();
                }
                return model;
            }
            catch (Exception ex)
            {
                return new List<TreeModelODataEntity>();
            }
        }
        /// <summary>
        /// 获取当前单位的品种接口
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetBreeding(DropSelectSearch searchModel)
        {
            try
            {
                List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
                var param = new
                {
                    EnterpriseID = searchModel.EnterpriseID,
                };
                var resultModel = _httpClientUtil1.PostJsonAsync<ResultModel<Breeding>>($"{_hostCongfiguration.QlwServiceHost}/api/BIZ_BreedingSet/GetList", param).Result;
                if (resultModel.ResultState)
                {
                    model = resultModel.Data.Select(Data => new TreeModelODataEntity { Id = Data.BreedingID, cName = Data.BreedingName }).ToList();
                }
                return model;
            }
            catch (Exception ex)
            {
                return new List<TreeModelODataEntity>();
            }
        }
        public Task<ResultModel> GetCheckenMealJurisdiction(DropSelectSearch searchModel)
        {
            //商品代蛋禽（禽联网）2110281756480000101  测试
            //商品代蛋禽（禽联网）1712111732400000110  正式
            var resultModel = _httpClientUtil1.GetJsonAsync<ResultModel>($"{_hostCongfiguration.NxinGatewayUrl}api/nxin.qlw.sc.isenteropenapp/1.0?open_req_src=nxin_shuju&enter_id={searchModel.EnterpriseID}&type=201710240104402022&id={_hostCongfiguration.CheckenAppId}");
            return resultModel;
        }

        /// <summary>
        /// 获取厂区和鸡舍接口
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="type">2厂区  3鸡舍</param>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetJurisdictionList(DropSelectSearch searchModel, string type)
        {
            ResultModel<Jurisdiction> resultModel = new ResultModel<Jurisdiction>();
            try
            {
                List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
                List<TreeModelODataEntity> chickenList = GetChickenFarm(searchModel);

                var param = new
                {
                    enteid = searchModel.EnterpriseID,
                    Param = type,
                    menuparttern = "2",
                    ChickenFarmIDs = chickenList.Count > 0 ? string.Join(',', chickenList.Select(s => s.Id)) : "",
                    enteids = new List<string> { }
                };
                resultModel = _httpClientUtil1.PostJsonAsync<ResultModel<Jurisdiction>>($"{_hostCongfiguration.ReportService}/api/RptEggManagerReport/GetJurisdictionList", param).Result;
                if (resultModel.ResultState)
                {
                    model = resultModel.Data.Select(Data => new TreeModelODataEntity { Id = Data.ID, cName = Data.Name }).ToList();
                }
                return model;
            }
            catch (Exception ex)
            {
                return new List<TreeModelODataEntity>();
            }
        }
        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<TreeModelODataEntity> getProductData(DropSelectSearch searchModel)
        {
            try
            {
                List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
                string interfaceWhere = string.Empty;
                //猪成本结转只取“生猪”品类的数据
                if (searchModel.TransferAccountsType == "1911081429200099104")
                {
                    interfaceWhere = "&sysclsid=4606";
                }
                var result = _httpClientUtil1.GetJsonAsync<ResultModel<ProductInfo>>($"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.qlwbase.kingdee.product.get/1.0?open_req_src=nxin_shuju&enterpriseid={searchModel.EnterpriseID}{interfaceWhere}").Result;
                if (result.Code == 0)
                {
                    model = result.Data.Select(Data => new TreeModelODataEntity { Id = Data.productId, cName = Data.productName }).ToList();
                    return model;
                }
                return new List<TreeModelODataEntity>();
            }
            catch (Exception ex)
            {
                return new List<TreeModelODataEntity>();
            }
        }

        /// <summary>
        /// 获取资产类别
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<TreeModelODataEntity> getFA_AssetsClassificationData(string GroupID)
        {
            try
            {
                List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
                var result = _httpClientUtil1.PostJsonAsync<ResultModel<FA_AssetsClassification>>($"{_hostCongfiguration._rdUrl}/api/DBNFA_AssetsClassification/GetAllData", new { EnterpriseID = GroupID }).Result;
                if (result.Code == 0)
                {
                    model = result.Data.Where(s => s.Rank != 0).Select(Data => new TreeModelODataEntity { Id = Data.ClassificationID, cName = Data.ClassificationName, Pid = Data.PID, ExtendId = Data.Rank.ToString() }).ToList();
                    return model;
                }
                return new List<TreeModelODataEntity>();
            }
            catch (Exception ex)
            {
                return new List<TreeModelODataEntity>();
            }
        }
        /// <summary>
        /// 获取禽养殖场
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<TreeModelODataEntity> getChickenFarmList(string EnterpriseID)
        {
            try
            {
                List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
                var result = _httpClientUtil1.GetJsonAsync<ResultModel<ChickenFarmModel>>($"{_hostCongfiguration._wgUrl}/qr/qxz/QlwPoultryBaseInterface2/ChickenFarmList?Pids=" + EnterpriseID, null).Result;
                if (result.Code == 0)
                {
                    model = result.Data.Select(Data => new TreeModelODataEntity { Id = Data.ChickenFarmID, cName = Data.ChickenFarmName }).ToList();
                    return model;
                }
                return new List<TreeModelODataEntity>();
            }
            catch (Exception ex)
            {
                return new List<TreeModelODataEntity>();
            }
        }

        /// <summary>
        /// 出入库 摘要
        /// </summary>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetInOutAbstract()
        {
            DataDictRequest SalesAbstractTyperequestIn = new DataDictRequest();
            //入库pid
            SalesAbstractTyperequestIn.PID = _inWarehousePid;
            List<TreeModelODataEntity> SalesAbstract1 = getDictService(SalesAbstractTyperequestIn);
            DataDictRequest SalesAbstractTyperequestOut = new DataDictRequest();
            //物资出库pid 
            //SalesAbstractTyperequestOut.DictID = 201707140104402209;
            SalesAbstractTyperequestOut.PID = _outWarehousePid;
            List<TreeModelODataEntity> SalesAbstract2 = getDictService(SalesAbstractTyperequestOut);
            var SalesAbstract = SalesAbstract1.Concat(SalesAbstract2).ToList();

            SalesAbstractTyperequestOut.PID = -1;
            SalesAbstractTyperequestOut.DictID = _transformInWarehouseId;
            List<TreeModelODataEntity> SalesAbstract3 = getDictService(SalesAbstractTyperequestOut);
            SalesAbstract = SalesAbstract.Concat(SalesAbstract3).ToList();

            SalesAbstractTyperequestOut.DictID = _transformOutWarehouseId;
            List<TreeModelODataEntity> SalesAbstract4 = getDictService(SalesAbstractTyperequestOut);
            SalesAbstract = SalesAbstract.Concat(SalesAbstract4).ToList();
            return SalesAbstract;
        }
        /// <summary>
        /// 获取税费抵扣查询 发票类型
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetInvoiceType()
        {
            List<TreeModelODataEntity> list = new List<TreeModelODataEntity>()
            {
                 new TreeModelODataEntity(){ cName="火车票", Id="1"},
                 new TreeModelODataEntity(){ cName="机票行程单", Id="2"},
                 new TreeModelODataEntity(){ cName="运输服务电子普通发票",Id="3"},
                 new TreeModelODataEntity(){ cName="公路客运票", Id="4"},
            };
            return list;
        }
        /// <summary>
        /// 运费摘要
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetCarriageAbstract(string enteId)
        {
            try
            {
                List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
                var result = _httpClientUtil1.PostJsonAsync<ResultModel<CarriageAbstract>>($"{_hostCongfiguration.ReportService}/api/RptConditionRelated/GetCarriageAbstractList", new { enteId = enteId }).Result;
                if (result.ResultState)
                {
                    model = result.Data.Select(Data => new TreeModelODataEntity { Id = Data.FieldValue, cName = Data.FieldName }).ToList();
                    return model;
                }
                return new List<TreeModelODataEntity>();
            }
            catch (Exception ex)
            {
                return new List<TreeModelODataEntity>();
            }
        }
        /// <summary>
        /// 获取字典表数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<TreeModelODataEntity> getDictService(DataDictRequest param)
        {
            try
            {
                List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
                var result = _httpClientUtil1.PostJsonAsync<ResultModel<DataDictDataResult>>($"{_hostCongfiguration.QlwServiceHost}/api/DataDict/getDataDict", param).Result;
                if (result.ResultState == true)
                {
                    model = result.Data.Select(Data => new TreeModelODataEntity { Id = Data.DictID.ToString(), cName = Data.cDictName }).ToList();
                    return model;
                }
                return new List<TreeModelODataEntity>();
            }
            catch (Exception ex)
            {
                return new List<TreeModelODataEntity>();
            }
        }

        /// <summary>
        /// 根据单位ID获取应用ID列表信息
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public Task<ResultModel> GetmenuListByEnterpriseId(string enterpriseId, string menulist)
        {
            //熊丽方法
            //var resultModel = _httpClientUtil1.GetJsonAsync<ResultModel>($"{_hostCongfiguration.NxinGatewayUrl}api/nxin.qlw.sc.isenteropenapp/1.0?open_req_src=nxin_shuju&enter_id={searchModel.EnterpriseID}&type=201710240104402022&id=1712111732400000110");
            var resultModel = _httpClientUtil1.GetJsonAsync<ResultModel>($"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.qlwbase.orderauthority.menu.list/1.0?open_req_src=qlw&enterpriseid={enterpriseId}&menulist={menulist}");

            //房忠欢方法
            return resultModel;
        }

        /// <summary>
        /// 获取当前单位的项目名称信息
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetProjectNameSettings(DropSelectSearch searchModel)
        {
            try
            {
                List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
                var param = new
                {
                    EnterpriseID = searchModel.EnterpriseID
                };
                var resultModel = _httpClientUtil1.PostJsonAsync<ResultModel<BIZProjectNameSetting>>($"{_hostCongfiguration.QlwServiceHost}/api/BIZProjectNameSetting/GetAllNotesForFinance", param).Result;
                if (resultModel.Code == 0)
                {
                    model = (from s in resultModel.Data
                             select new TreeModelODataEntity()
                             {
                                 Id = s.ProjectID,
                                 cName = s.ProjectName,
                                 Pid = s.PID.ToString()
                             }).ToList();
                }
                return model;
            }
            catch (Exception ex)
            {
                return new List<TreeModelODataEntity>();
            }
        }
        /// <summary>
        /// 获取当前单位的猪场
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetPigFarm(DropSelectSearch searchModel)
        {
            try
            {
                List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
                var resultModel = _httpClientUtil1.GetJsonAsync<List<PigFarmModel>>($"{_hostCongfiguration._wgUrl}/z/dbn/basic/UnionBizPigFarmNoIdentity/ByEn?en=" + searchModel.EnterpriseID).Result;
                if (resultModel.Count > 0)
                {
                    model = (from s in resultModel
                             select new TreeModelODataEntity()
                             {
                                 Id = s.pigFarmId,
                                 cName = s.pigFarmFullName,
                             }).ToList();
                }
                return model;
            }
            catch (Exception ex)
            {
                return new List<TreeModelODataEntity>();
            }
        }
        /// <summary>
        /// 获取猪只类型
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetPigTypes(string enterpriseid)
        {
            try
            {
                List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
                var resultModel = _httpClientUtil1.GetJsonAsync<List<PigTypes>>($"{_hostCongfiguration._wgUrl}/cost/report/finance/pigtypes?EnterpriseID={enterpriseid}").Result;
                if (resultModel.Count > 0)
                {
                    model = (from s in resultModel
                             select new TreeModelODataEntity()
                             {
                                 Id = s.dictId,
                                 cName = s.dictName,
                                 Pid = ""
                             }).ToList();
                }
                return model;
            }
            catch (Exception ex)
            {
                return new List<TreeModelODataEntity>();
            }
        }

        public async Task<List<BIZ_AccoSubject>> GetSubjectListExtend(long id, long enteID = 0, string EnterDate = "")
        {
            List<BIZ_AccoSubject> totalSubject = new List<BIZ_AccoSubject>();
            ResultModel<BIZ_AccoSubject> resultModel = new ResultModel<BIZ_AccoSubject>();
            var requestResult = string.Empty;

            resultModel = await _httpClientUtil1.PostJsonAsync<ResultModel<BIZ_AccoSubject>>($"{_hostCongfiguration.QlwServiceHost}/api/BIZAccoSubject/getAccoSubject", new { EnterpriseID = enteID, AccoSubjectID = id, EnterDate = string.IsNullOrEmpty(EnterDate) ? DateTime.Now.ToString("yyyy-MM-dd") : EnterDate });
            if (resultModel.ResultState)
            {
                totalSubject = resultModel.Data;// JsonConvert.DeserializeObject<IEnumerable<BIZ_AccoSubject>>(resultModel.Data.ToString()).ToList();
            }
            return totalSubject;
        }

        /// <summary>
        /// 获取当前单位的计量单位
        /// </summary>
        /// <returns></returns>
        public List<UnitMeasurement> GetUnitMeasurement(string EnterpriseID)
        {
            try
            {

                List<UnitMeasurement> model = new List<UnitMeasurement>();
                var param = new
                {
                    EnterpriseID = EnterpriseID
                };
                var resultModel = _httpClientUtil1.PostJsonAsync<List<UnitMeasurement>>($"{_hostCongfiguration.QlwServiceHost}/api/FD_SuppliesSetting/getUnitMeasurementList", param).Result;
                return resultModel;
            }
            catch (Exception ex)
            {
                return new List<UnitMeasurement>();
            }
        }
        /// <summary>
        /// 获取资金账户设置结算方式
        /// </summary>
        /// <returns></returns>
        public AccountInfo GetAccountRelatePayType(string AccountID, string EnterpriseID, string EnterData)
        {
            try
            {
                var acc = new AccountInfo();
                if (string.IsNullOrEmpty(AccountID))
                {
                    return acc;
                }
                var resultModel = _httpClientUtil1.PostJsonAsync<AccResultModel>($"{_hostCongfiguration.QlwServiceHost}/api/FD_AccountInventory/GetAccountRelatedSearch", new { AccountID, EnterpriseID, EnterData }).Result;
                acc.AccountData = resultModel;
                var accPayType = _httpClientUtil1.PostJsonAsync<FD_Account>($"{_hostCongfiguration.QlwServiceHost}/api/FMCommon/GetAccountPayType", new { AccountID }).Result;
                acc.PaymentTypeID = accPayType.PaymentTypeID;
                acc.PaymentTypeName = accPayType.PaymentTypeName;
                return acc;
            }
            catch (Exception ex)
            {
                return new AccountInfo() { AccountData = new AccResultModel() { Data = ex, Msg = "异常" } };
            }
        }
        /// <summary>
        /// 获取银行账户信息
        /// </summary>
        /// <returns></returns>
        public List<Bank_Account> GetRecesAccountInfo(string BusinessType, string PayeeID, string PayerID)
        {
            List<Bank_Account> resultList = new List<Bank_Account>();
            try
            {
                //部门员工 都调人员档案
                if (BusinessType == "201611160104402103" || BusinessType == "201611160104402102")
                {

                    var personAccountList = _httpClientUtil1.PostJsonAsync<List<PersonAccountInfo>>($"{_hostCongfiguration.QlwServiceHost}/api/FMCommon/GetPersonsAccountInfo", new { CustomerID = PayeeID, EnterpriseID = PayerID }).Result;
                    if (personAccountList != null && personAccountList.Count > 0)
                    {
                        foreach (var item in personAccountList)
                        {
                            var resultModel = new Bank_Account();
                            var accountData = item;
                            resultModel.PayerID = accountData.PersonID;
                            resultModel.DepositBank = accountData.DepositBank;
                            resultModel.AccountNumber = accountData.AccountNumber;
                            resultModel.BankID = accountData.BankID;
                            resultModel.AccountName = accountData.AccountName;
                            resultModel.UserCenterPayee = accountData.BO_ID;
                            resultModel.BankCode = accountData.BankAbbr;
                            resultModel.PayeeID = resultModel.UserCenterPayee;
                            resultList.Add(resultModel);
                        }

                    }
                }
                else
                {

                    var accountList = _httpClientUtil1.PostJsonAsync<List<SA_CustomerAccount>>($"{_hostCongfiguration.QlwServiceHost}/api/FMCommon/GetAccountInfo", new { CustomerID = PayeeID, EnterpriseID = PayerID }).Result;
                    if (accountList != null && accountList.Count > 0)
                    {
                        foreach (var item in accountList)
                        {
                            var resultModel = new Bank_Account();
                            var accountData = item;
                            resultModel.PayerID = accountData.CustomerID;
                            resultModel.DepositBank = accountData.DepositBank;
                            resultModel.AccountNumber = accountData.AccountNumber;
                            resultModel.BankID = accountData.BankID;
                            resultModel.AccountName = accountData.AccountName;
                            resultModel.BankCode = accountData.BankAbbr;
                            resultModel.AccountNature = accountData.AccountNature;

                            var payer = TransformationCustomer(accountData.CustomerID);
                            if (payer != null)
                            {
                                resultModel.UserCenterPayee = payer.CustomerID;
                                resultModel.PayeeID = resultModel.UserCenterPayee;
                            }

                            resultList.Add(resultModel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new List<Bank_Account>() { new Bank_Account() { UserCenterPayee = ex.ToString() } };
            }
            return resultList;
        }
        /// <summary>
        /// 批量获取银行账户信息
        /// </summary>
        /// <returns></returns>
        public List<Bank_Account> GetAccountInfoList(AccountInfoReq req)
        {
            List<Bank_Account> resultList = new List<Bank_Account>();
            try
            {
                if (req == null)
                {
                    return resultList;
                }
                if (req.CustomerIdList == null || req.CustomerIdList.Count == 0)
                {
                    return resultList;
                }

                var customerids = string.Join(',', req.CustomerIdList);
                //部门员工 都调人员档案
                if (req.BusinessType == "201611160104402103")
                {

                    var personAccountList = _httpClientUtil1.PostJsonAsync<List<PersonAccountInfo>>($"{_hostCongfiguration.QlwServiceHost}/api/FMCommon/GetPersonsAccountInfo", new { CustomerID = customerids, EnterpriseID = req.EnterpriseID }).Result;
                    if (personAccountList != null && personAccountList.Count > 0)
                    {
                        foreach (var model in personAccountList.GroupBy(p => p.PersonID))
                        {
                            var resultModel = new Bank_Account();
                            var accountData = model.FirstOrDefault();
                            resultModel.PayerID = accountData.PersonID;
                            resultModel.DepositBank = accountData.DepositBank;
                            resultModel.AccountNumber = accountData.AccountNumber;
                            resultModel.BankID = accountData.BankID;
                            resultModel.AccountName = accountData.AccountName;
                            resultModel.UserCenterPayee = accountData.BO_ID;
                            resultModel.BankCode = accountData.BankAbbr;
                            resultModel.PayeeID = resultModel.UserCenterPayee;
                            resultList.Add(resultModel);
                        }

                    }
                }
                else
                {
                    var accountList = _httpClientUtil1.PostJsonAsync<List<SA_CustomerAccount>>($"{_hostCongfiguration.QlwServiceHost}/api/FMCommon/GetCustomersAccountInfo", new { CustomerID = customerids, EnterpriseID = req.EnterpriseID }).Result;
                    if (accountList != null && accountList.Count > 0)
                    {
                        foreach (var model in accountList.GroupBy(p => p.CustomerID))
                        {
                            var resultModel = new Bank_Account();
                            var accountData = model.FirstOrDefault();
                            resultModel.PayerID = accountData.CustomerID;
                            resultModel.DepositBank = accountData.DepositBank;
                            resultModel.AccountNumber = accountData.AccountNumber;
                            resultModel.BankID = accountData.BankID;
                            resultModel.AccountName = accountData.AccountName;
                            resultModel.BankCode = accountData.BankAbbr;
                            resultModel.AccountNature = accountData.AccountNature;

                            var payer = TransformationCustomer(accountData.CustomerID);
                            if (payer != null)
                            {
                                resultModel.UserCenterPayee = payer.CustomerID;
                                resultModel.PayeeID = resultModel.UserCenterPayee;
                            }

                            resultList.Add(resultModel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new List<Bank_Account>() { new Bank_Account() { UserCenterPayee = ex.ToString() } };
            }
            return resultList;
        }

        /// <summary>
        /// 转换供应商
        /// </summary>
        /// <param name="custId"></param>
        /// <returns></returns>
        private FM_Customer TransformationCustomer(string custId, string EnterpriseID = "0")
        {
            try
            {
                var model = new { CustomerID = custId, EnterpriseID = EnterpriseID };
                var dic = _httpClientUtil1.PostJsonAsync<ResultModel>($"{_hostCongfiguration.QlwServiceHost}/api/CustomerBaseInfo/TransformationCustomer", new { CustomerID = custId, EnterpriseID = EnterpriseID }).Result;
                if (dic != null && dic.ResultState && dic.Data != null)
                {
                    var customerList = JsonConvert.DeserializeObject<List<FM_Customer>>(dic.Data.ToString());
                    if (customerList == null || !customerList.Any()) return new FM_Customer { CustomerID = "0" };
                    return customerList.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return new FM_Customer();
            }
            return new FM_Customer { CustomerID = "0" };
        }
        /// <summary>
        /// 获取当前单位的所属项目
        /// </summary>
        /// <returns></returns>
        public List<FMProject> GetBelongProject(string EnterpriseID)
        {
            try
            {

                List<FMProject> model = new List<FMProject>();
                var param = new
                {
                    EnterpriseID = EnterpriseID
                };
                var resultModel = _httpClientUtil1.PostJsonAsync<ResultModel<FMProject>>($"{_hostCongfiguration.QlwServiceHost}/api/PPMProject/getProject", param).Result;
                if (resultModel.ResultState)
                {
                    model = resultModel.Data;
                }
                return model;
            }
            catch (Exception ex)
            {
                return new List<FMProject>();
            }
        }
        /// <summary>
        /// 获取付款往来类型
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        public List<DataDictModel> GetDataDact(string PID)
        {
            var resultModel = _httpClientUtil1.PostJsonAsync<ResultModel<DataDictModel>>($"{_hostCongfiguration.QlwServiceHost}/api/DataDict/getDataDict", new { PID = PID, EnterpriseID = 0 }).Result;
            if (resultModel != null && resultModel.ResultState)
            {
                return resultModel.Data;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 按结算类型获取摘要
        /// </summary>
        /// <param name="type">0：收款；1：付款；2：转账</param>
        /// <returns></returns>
        public List<Summary> GetSummaryByType(string relationId, DropSelectSearch searchModel)
        {
            try
            {
                DateTime beforDT = System.DateTime.Now;
                int type = 0;
                string ReceiptType = "-1";
                long EnterpriseIDs = -1;
                if (!String.IsNullOrEmpty(relationId))
                {
                    string[] contentID = relationId.ToString().Split('*');
                    //如果支出类别根据公司加载
                    if (contentID.Length >= 3)
                    {
                        if (!String.IsNullOrEmpty(contentID[0]))
                            type = int.Parse(contentID[0]);
                        if (!String.IsNullOrEmpty(contentID[1]))
                            ReceiptType = contentID[1];
                        if (!String.IsNullOrEmpty(contentID[2]))
                            EnterpriseIDs = Convert.ToInt64(contentID[2]);
                    }
                    //支出类别不需要公司加载
                    else if (contentID.Length >= 2 && contentID.Length < 3)
                    {
                        if (!String.IsNullOrEmpty(contentID[0]))
                            type = int.Parse(contentID[0]);
                        if (!String.IsNullOrEmpty(contentID[1]))
                            ReceiptType = contentID[1];
                    }
                    //不需要单据id
                    else
                    {
                        type = int.Parse(relationId.ToString());
                    }
                }
                List<Summary> list = GetSummaryList(type, searchModel, ReceiptType, EnterpriseIDs);
                DateTime afterDT1 = System.DateTime.Now;
                TimeSpan ts1 = afterDT1.Subtract(beforDT);
                //LogHelper.Info("FMBaseController/GetSummaryByType" + ts1.TotalMilliseconds.ToString());
                return list;
            }
            catch (Exception e)
            {
                return new List<Summary>();
            }
        }
        private List<Summary> GetSummaryList(int id, DropSelectSearch searchModel, string ReceiptType = "-1", long EnterpriseIDs = -1)
        {
            string SettleSummaryCode = "0";
            switch (id)
            {
                case 0:
                    SettleSummaryCode = "01";
                    break;
                case 1:
                    SettleSummaryCode = "02";
                    break;
                case 2:
                    SettleSummaryCode = "03";
                    break;
                case 3:
                    SettleSummaryCode = String.Empty;
                    break;
            }
            var resultModel = _httpClientUtil1.PostJsonAsync<ResultModel<Summary>>($"{_hostCongfiguration.QlwServiceHost}/api/BIZSettleSummary/getSettleSummary", new { EnterpriseID = EnterpriseIDs == -1 ? Convert.ToInt64(searchModel.EnterpriseID) : EnterpriseIDs, SettleSummaryCode = SettleSummaryCode, ReceiptType = ReceiptType, EnterDate = searchModel.CurrentEnterDate }).Result;
            if (resultModel.ResultState)
            {
                return resultModel.Data;
            }
            else
            {
                return new List<Summary>();
            }
        }

        /// <summary>
        /// 获取商品数据 调用接口
        /// </summary>
        /// <RETURNS></RETURNS>
        public List<ResponseProductInfoListForFDCostCoefficient> GetProductListForCostCoefficientTwo(string enterpriseid)
        {
            List<ResponseProductInfoListForFDCostCoefficient> list = new List<ResponseProductInfoListForFDCostCoefficient>();
            var resultModel = _httpClientUtil1.PostJsonAsync<ResultModel<ResponseProductInfoListForFDCostCoefficient>>($"{_hostCongfiguration.QlwServiceHost}/api/BIZProduct/getProduct", new { EnterpriseID = enterpriseid, bInside = true }).Result;
            if (resultModel.ResultState)
            {
                list = resultModel.Data.Select(o => new ResponseProductInfoListForFDCostCoefficient
                {
                    ProductID = o.ProductID,
                    ProductName = o.ProductName,
                    ClassificationID = o.ClassificationID,
                    ClassificationName = o.ClassificationName,
                    UnitID = o.MeasureUnitID,
                    UnitName = o.MeasureUnit

                }).ToList();
            }
            return list;
        }

        /// <summary>
        /// 获取审批状态
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        //public IQueryable<FD_ExpenseODataEntity> GetAuditState(IQueryable<FD_ExpenseODataEntity> list)
        //{
        //    try
        //    {
        //        if (list == null || list.Count() == 0)
        //        {
        //            return list;
        //        }
        //        //Task<RestFulResult<List<FD_ExpenseODataEntity>>> resultModel = _httpClientUtil1.PostJsonAsync<RestFulResult<List<FD_ExpenseODataEntity>>>($"{_hostCongfiguration.QlwServiceHost}/api/FDExpenseShouldPay/GetAuditResultList", list);
        //        Task<RestFulResult<List<FD_ExpenseODataEntity>>> resultModel = _httpClientUtil1.PostJsonAsync<RestFulResult<List<FD_ExpenseODataEntity>>>($"http://qlw.data.test.com/api/FDExpenseShouldPay/GetAuditResultList", list);

        //        if (resultModel != null && resultModel.Result.code == 0 && resultModel.Result.data != null)
        //        {
        //            return JsonConvert.DeserializeObject<IQueryable<FD_ExpenseODataEntity>>(resultModel.Result.data.ToString());
        //        }
        //        return list;
        //    }
        //    catch (Exception ex)
        //    {
        //        return list;
        //    }
        //}


        /// <summary>
        /// 获取当前单位的客户信息
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public List<FM_Customer> GetBizCustomerList(string EnterpriseID)
        {
            try
            {
                List<FM_Customer> model = new List<FM_Customer>();
                var param = new
                {
                    EnterpriseID = EnterpriseID,
                };
                var resultModel = _httpClientUtil1.PostJsonAsync<ResultModel<FM_Customer>>($"{_hostCongfiguration.QlwServiceHost}/api/BIZCustomer/getCustomer", param).Result;
                if (resultModel.ResultState)
                {
                    model = resultModel.Data;
                }
                return model;
            }
            catch (Exception ex)
            {
                return new List<FM_Customer>();
            }
        }
        /// <summary>
        /// 获取当前单位的末级部门
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetMarketByRank(DropSelectSearch searchModel)
        {
            try
            {
                List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();

                var param = new
                {
                    EnterpriseID = searchModel.EnterpriseID,
                    Type = 2,
                    Rank = searchModel.Rank,
                    EnterDate = searchModel.CurrentEnterDate
                };
                var resultModel = _httpClientUtil1.PostJsonAsync<ResultModel<MM_Markets>>($"{_hostCongfiguration.QlwServiceHost}/api/BIZMarket/getMarket", param).Result;
                if (resultModel.ResultState)
                {
                    model = (from s in resultModel.Data
                             select new TreeModelODataEntity()
                             {
                                 Id = s.MarketID,
                                 cName = s.MarketFullName,
                                 Pid = s.PID.ToString()
                             }).ToList();
                }
                return model;
            }
            catch (Exception ex)
            {
                return new List<TreeModelODataEntity>();
            }
        }

        public List<TreeModelODataEntity> GetArea(DropSelectSearch searchModel)
        {
            try
            {
                List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
                var param = new
                {

                    Rank = searchModel.Rank,
                    PID = searchModel.relationId
                };
                var resultModel = _httpClientUtil1.PostJsonAsync<ResultModel<BIZ_Area>>($"{_hostCongfiguration.QlwServiceHost}/api/BIZArea/getArea", param).Result;
                if (resultModel.ResultState)
                {
                    model = (from s in resultModel.Data
                             select new TreeModelODataEntity()
                             {
                                 Id = s.AreaID,
                                 cName = s.AreaName,
                                 Pid = s.PID,
                                 ExtendId = s.Rank.ToString()
                             }).ToList();
                }
                return model;
            }
            catch (Exception ex)
            {
                return new List<TreeModelODataEntity>();
            }
        }

        /// <summary>
        /// 获取字典
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        public List<DataDictModel> GetDataDict(DataDictRequest searchModel)
        {
            try
            {
                var resultModel = _httpClientUtil1.PostJsonAsync<ResultModel<DataDictModel>>($"{_hostCongfiguration.QlwServiceHost}/api/DataDict/getDataDict", searchModel).Result;
                if (resultModel != null && resultModel.ResultState)
                {
                    return resultModel.Data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return new List<DataDictModel>();
            }
        }
        /// <summary>
        /// 费用科目
        /// </summary>
        /// <returns></returns>
        public List<Subject> GetCostSubjectList(DropSelectSearch searchModel)
        {
            try
            {
                List<Subject> list = new List<Subject>();
                var resultModel = _httpClientUtil1.PostJsonAsync<ResultModel<Subject>>($"{_hostCongfiguration.QlwServiceHost}/api/FDAccoSubject/GetCostAccoSubject", new { searchModel.EnterpriseID, searchModel.GroupID, EnterDate = searchModel.CurrentEnterDate }).Result;
                if (resultModel != null && resultModel.ResultState)
                {
                    return resultModel.Data;
                }
                return list;
            }
            catch (Exception ex)
            {
                return new List<Subject>();
            }
        }
        #region 资金账户是否已用
        public bool IsAccountUse(string AccountID)
        {
            try
            {
                var result = _httpClientUtil1.PostJsonAsync<bool>($"{_hostCongfiguration.QlwServiceHost}/api/FDAccountList/IsAccountUse", AccountID);
                if (result == null) { return true; }
                return result.Result;
            }
            catch (Exception e)
            {
                return true;
            }
        }
        #endregion
        public List<SalarySetItem> GetSalarySetItemList(FM_CarryForwardVoucherSearchCommand request, AuthenticationHeaderValue token)
        {
            try
            {
                string url = $"{_hostCongfiguration.DBN_HrServiceHost}/api/HrSalaryInfo/getSalarySetItem";
                var res = _httpClientUtil1.PostJsonAsync<ResultModel<SalarySetItem>>(url, new { EnterpriseId = request.EnterpriseID }, (a) => { a.Authorization = token; }).Result;
                if (res.Code == 0)
                {
                    res.Data.ForEach(s =>
                    {
                        s.SetItemId = s.SetItemId;
                    });
                    return res.Data;
                }
                else
                {
                    return new List<SalarySetItem>();
                }
            }
            catch (Exception ex)
            {
                return new List<SalarySetItem>();
            }
        }
        public List<ShareCostItem> GetShareCostItemList(FM_CarryForwardVoucherSearchCommand request)
        {
            try
            {
                string url = $"{_hostCongfiguration._wgUrl}/cost/management/api/FM_CostProject/GetCostProjectByEnterpriseID?enterpriseId=" + request.EnterpriseID;
                _logger.LogInformation("费用项目接口" + url);
                var result = _httpClientUtil1.GetJsonAsync<List<ShareCostItem>>(url).Result;
                return result;
            }
            catch (Exception ex)
            {
                return new List<ShareCostItem>();
            }
        }
        /// <summary>
        /// 中介调用外部接口
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public object IntermediaryRequest(string type, string url, object param, string cookie)
        {
            try
            {
                if (string.IsNullOrEmpty(type))
                {
                    type = "post";
                }
                if (type.ToLower() == "post" && string.IsNullOrEmpty(cookie))
                {
                    var a = _httpClientUtil1.PostJsonAsync<object>(url, param).Result;
                    return a;
                }
                else if (type.ToLower() == "post" && !string.IsNullOrEmpty(cookie))
                {
                    var action = new Action<HttpRequestHeaders>(headers =>
                    {
                        headers.Add("Cookie", cookie);
                    });
                    var a = _httpClientUtil1.PostJsonAsync<object>(url, param, action).Result;
                    return a;
                }
                else
                {
                    if (!string.IsNullOrEmpty(cookie))
                    {
                        var action = new Action<HttpRequestHeaders>(headers =>
                        {
                            headers.Add("Cookie", cookie);
                        });
                        return _httpClientUtil1.GetJsonAsync<object, object>(url, null, action).Result;
                    }
                    return _httpClientUtil1.GetStringAsync(url).Result;
                }

            }
            catch (Exception ex)
            {
                return new ResultModel() { Msg = "接口请求失败", Code = 500, ResultState = false };
            }
        }
        /// <summary>
        /// 获取单据号（收付款、凭证）
        /// </summary>
        public ResultModel GetNumber(NumberSearchModel model)
        {
            var res = _httpClientUtil1.PostJsonAsync<ResultModel>($"{_hostCongfiguration.QlwServiceHost}/api/FDPaymentReceivables/GetNumber", model)?.Result;
            if (res == null)
            {
                //res.Msg = "未获取到单据号";
                return res;
            }
            if (!res.ResultState && !string.IsNullOrEmpty(res.Msg))
            {
                return res;
            }
            return res;
        }

        #region
        //客户 供应商
        public Task<List<Biz_CustomerODataEntity>> GetCurrentUnitList(CustomerSearch model)
        {
            var customerWhere = string.Empty;
            var SupplierWhere = string.Empty;
            if (!string.IsNullOrEmpty(model.CustomerID))
            {
                customerWhere += string.Format(" and CustomerID in ({0}) ", model.CustomerID);
                SupplierWhere += string.Format(" and SupplierID in ({0}) ", model.CustomerID);
            }
            if (!string.IsNullOrEmpty(model.CustomerName))
            {
                customerWhere += string.Format(" and CustomerName like '%{0}%'", model.CustomerName);
                SupplierWhere += string.Format(" and SupplierName like '%{0}%'", model.CustomerName);
            }
            var sql = string.Format(@" 
                                    SELECT CONVERT(CustomerID USING utf8mb4) CustomerID,CustomerName,IsUse from nxin_qlw_business.sa_customer where EnterpriseID in({0}) {1}
                                    union 
                                    SELECT CONVERT(SupplierID USING utf8mb4) CustomerID, SupplierName CustomerName,IsUse from nxin_qlw_business.pm_supplier where EnterpriseID in({0}) {2}", model.EnterpriseID, customerWhere, SupplierWhere);//and isuse=1
            return _context.Biz_CustomerDataSet.FromSqlRaw(sql).ToListAsync();
        }
        #endregion

        #region 系统选项
        public string GetBizOptionConfig(string optionId, string enterid, int scopeCode, string defaultValue = "0")
        {
            string url = "";
            try
            {
                string result = defaultValue;
                StringBuilder param = new StringBuilder();
                EncrypDecryp_MD5 omd5 = new EncrypDecryp_MD5 { esEncoding = EncrypDecryp_MD5.eEncoding.UTF8 };
                string tokenstr = _hostCongfiguration.EnterpriseUnionKey;
                if (string.IsNullOrEmpty(enterid) || enterid == "0")
                {
                    return result;
                }
                param.AppendFormat(@"?OptionID={0}&scopeCode={2}&EnterID={1}", optionId, enterid, scopeCode);
                string timeStampstr = GetTimeStamp();
                //增加时间戳 和加密
                param.Append("&timeStamp=" + timeStampstr + "");
                param.Append("&accessToken=" + omd5.EncryptString(tokenstr + timeStampstr) + "");
                url = _hostCongfiguration.QlwBase + "/webapi/ApiOption/GetOption" + param.ToString();
                var response = _httpClientUtil1.GetJsonAsync<ResultModel<BIZ_OptionConfig>>(url).Result;
                if (response != null && response.ResultState && response.Data != null && response.Data.Count > 0)
                {
                    result = response.Data[0].OptionValue;
                }

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static string GetTimeStamp()
        {
            long timeStamp = (long)(System.DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            return timeStamp.ToString();
        }
        #endregion
        #region 会计期间接口   仅限于猪小智使用
        public Biz_EnterprisePeriodInfo CreatePeriodByYear(Biz_EnterprisePeriodInfo model)
        {
            ///{"RegisterYear":2022,"EnterID":"634086739144001721"}
            Biz_EnterpriseperiodEX entity = new Biz_EnterpriseperiodEX();
            entity.Year = model.RegisterYear;

            entity.EnableYear = model.RegisterYear;
            entity.EnableMonth = 1;

            entity.Details = new List<Biz_Enterpriseperiod>();

            for (int i = 0; i < 12; i++)
            {
                Biz_Enterpriseperiod obj = new Biz_Enterpriseperiod();
                obj.Year = model.RegisterYear;
                obj.Month = i + 1;
                obj.StartDate = new DateTime(obj.Year, obj.Month, 1).ToString("yyyy-MM-dd");
                obj.EndDate = new DateTime(obj.Year, obj.Month, 1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                obj.EnterpriseID = model.EnterpriseID;
                obj.CreatedDate = DateTime.Now;
                obj.OwnerID = model.BO_ID;
                entity.Details.Add(obj);
            }
            List<Biz_EnterpriseperiodEX> dtm1 = new List<Biz_EnterpriseperiodEX>();
            dtm1.Add(entity);

            Biz_EnterprisePeriodInfo btm2 = new Biz_EnterprisePeriodInfo();
            btm2.EnableYear = model.RegisterYear;
            btm2.RegisterYear = model.RegisterYear;
            btm2.EnableMonth = 1;
            btm2.EnterpriseID = model.EnterpriseID;
            btm2.LstPeriod = dtm1;

            return btm2;
        }
        /// <summary>
        /// 修改建账日期时，判断猪场设置的建账日期
        /// </summary>
        /// <param name="JsonData"></param>
        /// <returns></returns>
        public FMResultModel ValidatePigFarmCreatedDate(Biz_EnterprisePeriodInfo JsonData)
        {
            try
            {
                var result = _httpClientUtil1.PostJsonAsync<FMResultModel>($"{_hostCongfiguration.QlwServiceHost}/api/Biz_Enterpriseperiod/ValidatePigFarmCreatedDate", JsonData).Result;
                return result;
            }
            catch (Exception e)
            {
                return new FMResultModel() { ResultState = false };
            }
        }
        public int Post(Biz_EnterprisePeriodInfo JsonData)
        {
            var result = _httpClientUtil1.PostJsonAsync<int>($"{_hostCongfiguration.QlwServiceHost}/api/Biz_Enterpriseperiod/SaveModel", JsonData).Result;
            return result;
        }
        #endregion

        /// <summary>
        /// 商城客户转换成企联网客户
        /// </summary>
        public List<Biz_CustomerDrop> GetQlwCustomerBySc(Biz_CustomerDrop model)
        {
            var res = _httpClientUtil1.PostJsonAsync<ResultModel>($"{_hostCongfiguration._rdUrl}/api/FMCommon/GetCustomerByRelated", model)?.Result;
            if (res.ResultState && res.Data != null)
            {
                return JsonConvert.DeserializeObject<List<Biz_CustomerDrop>>(res.Data.ToString());
            }
            return new List<Biz_CustomerDrop>();
        }
        /// <summary>
        /// 获取商城订单是否已生成收付款单
        /// </summary>
        public List<BIZ_Related> GetScHasRelated(BIZ_Related model)
        {
            return _httpClientUtil1.PostJsonAsync<List<BIZ_Related>>($"{_hostCongfiguration._rdUrl}/api/FDRelated/GetBIZ_RelatedBySearchConditions", model)?.Result;
        }
        /// <summary>
        /// 获取组织单位
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<RestfulResult> GetOrgEntityList(OrgEnteRequest req)
        {
            var result = new RestfulResult();
            try
            {
                var url = $"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.permission.org.enter.power.tree.list/1.0?open_req_src=nxin_shuju&enterpriseid={req.enterpriseId}&checklist={req.checklist}&istree={req.istree}&&pid={req.pid}&name={req.name}&boid={req.boid}&ismaster={req.ismaster}&ispowerall={req.ispowerall}&isthree={req.isthree}&isuse={req.isuse}";
                result = await _httpClientUtil1.GetJsonAsync<RestfulResult>(url);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"FMBaseCommon/GetOrgEntityList:异常={ex.ToString()};参数={JsonConvert.SerializeObject(req)}");
                result.msg = "组织单位查询异常";
            }
            return result;
        }
        /// <summary>
        /// 获取科目
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public List<Biz_Subject> GetSubjectTreeList(SubjectRequest req)
        {
            var list = new List<Biz_Subject>();
            var isgroup = "";
            if (req.IsGroup.HasValue) { isgroup = $"&IsGroup={req.IsGroup}"; }
            var url = $"{_hostCongfiguration._wgUrl}/base/finance/oq/AccosubjectAnyOData?dDate={req.DataDate}&EnterpriseId={req.EnterpriseID}{isgroup}&$orderby=AccoSubjectCode";
            var result = _httpClientUtil1.GetJsonAsync<string, ResultModel<Biz_Subject>>(url, null)?.Result;
            list = result.value;
            return list;
        }
        /// <summary>
        /// 获取科目
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public List<Biz_Settlesummary> GetSettlesummaryList(string EnterpriseID, string DataDate = "")
        {
            var url = $"{_hostCongfiguration.QlwBase}/WEBAPI/ApiSettlesummary/GetSettlesummary?EnterpriseID={EnterpriseID}&LastDate={(string.IsNullOrEmpty(DataDate) ? DateTime.Now.ToString("yyyy-MM") : DataDate)}";
            var res = _httpClientUtil1.PostJsonAsync<ResultModel<Biz_Settlesummary>>(url, null)?.Result;
            if (res?.Code == 0)
            {
                return res.Data;
            }
            return new List<Biz_Settlesummary>();
        }
        /// <summary>
        /// 获取部门
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public List<MarketResult> GetMarketList(string EnterpriseID)
        {
            var url = $"{_hostCongfiguration.NxinGatewayUrl}api/nxin.qlwbase.getmarket/1.0";
            var param = new { EnterId = EnterpriseID, HasChild = 1, IsUse = 2 };
            var res = _httpClientUtil1.PostJsonAsync<RestfulResult>(url, param)?.Result;
            if (res?.code == 0 && res?.data != null)
            {
                return JsonConvert.DeserializeObject<List<MarketResult>>(res.data.ToString());
            }
            return new List<MarketResult>();
        }
        /// <summary>
        /// 获取单据字
        /// </summary>
        public List<FMTicketedPoint> GetTicketedPointList(string EnterpriseID)
        {
            var res = _httpClientUtil1.PostJsonAsync<ResultModel<FMTicketedPoint>>($"{_hostCongfiguration.QlwServiceHost}/api/BIZTicketedPoint/getTicketedPoint", new { EnterpriseID = EnterpriseID })?.Result;
            if (res?.Code == 0)
            {
                return res.Data;
            }
            return new List<FMTicketedPoint>();
        }
        #region
        /// <summary>
        /// 集团商品代号信息
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public List<dynamic> GetGroupProductList(string groupid, string productID = "")
        {
            var sqlWhere = string.Empty;
            if (!string.IsNullOrEmpty(productID))
            {
                sqlWhere += string.Format(" and ProductID = {0} ", productID);

            }
            var sql = string.Format(@" 
                                   SELECT ProductID,ProductName,ProductCode,Specification,MeasureUnit,StandardPack,
                                           U.UnitName AS MeasureUnitName FROM `qlw_nxin_com`.`biz_product` pro
                                           LEFT JOIN  qlw_nxin_com.UnitMeasurement AS U ON pro.MeasureUnit = U.UnitID
                                            WHERE `EnterpriseId`={0} {1}   ", groupid, sqlWhere);
            return _context.DynamicSqlQuery(sql).ToList();
        }
        #endregion
        //银行类型转换
        public string ConvertBankType(string bankID)
        {
            var bankType = "";
            if (bankID == ((long)FMSettleReceipType.CEBBankDicID).ToString())
            {
                bankType = "CEB";
            }
            else if (bankID == ((long)FMSettleReceipType.ABCBankDicID).ToString())
            {
                bankType = "ABC";
            }
            else if (bankID == ((long)FMSettleReceipType.MinShengBankDicID).ToString())
            {
                bankType = "CMBC";
            }
            else if (bankID == ((long)FMSettleReceipType.ICBCBankDicID).ToString())
            {
                bankType = "ICBC";
            }
            else if (bankID == ((long)FMSettleReceipType.PSBCBankDicID).ToString())
            {
                bankType = "PSBC";
            }
            else if (bankID == ((long)FMSettleReceipType.ADBCBankDicID).ToString())//农业发展
            {
                bankType = "ADBC";
            }
            else if (bankID == ((long)FMSettleReceipType.BCMBankDicID).ToString())
            {
                bankType = "BCM";
            }
            return bankType;
        }
        public dynamic GetAccoSubjectGroup(dynamic model)
        {
            string sql = $@"SELECT A.* from qlw_nxin_com.biz_accosubject A
            INNER JOIN qlw_nxin_com.`biz_versionsetting` B ON A.`VersionID`=B.`VersionID`
            INNER JOIN qlw_nxin_com.`biz_enterprise` C ON A.`EnterpriseID`=C.`EnterpriseID`
            WHERE C.`PID`={model.GroupId} AND  B.`iVersionType`=1712221411430000101 AND B.`EnterpriseID`={model.GroupId} AND NOW() BETWEEN B.`dBegin` AND B.`dEnd` AND A.IsUse = true";
            return _context.DynamicSqlQuery(sql);
        }
        /// <summary>
        /// 费用分析表 汇总方式
        /// </summary>
        /// <returns></returns>
        public dynamic SumamryGroupList(dynamic model)
        {
            List<dynamic> list = new List<dynamic>();
            //获取部门最大级次
            string MaxRankMarket = $@"SELECT MAX(Rank) Rank from qlw_nxin_com.biz_market where EnterpriseID IN ({model.EnterpriseIds})";
            var MarketRank = _context.DynamicSqlQuery(MaxRankMarket).FirstOrDefault().Rank;
            //获取科目最大级次
            string MaxRankAccSubject = $@"SELECT MAX(Rank) Rank from qlw_nxin_com.biz_accosubject where EnterpriseID IN ({model.EnterpriseIds})";
            var AccSubjectRank = _context.DynamicSqlQuery(MaxRankAccSubject).FirstOrDefault().Rank;
            list.Add(new { Name = "单位部门",Pid = 0,Id = 3 });
            List<dynamic> market = new List<dynamic>();
            for (int i = 0; i < MarketRank; i++)
            {
                market.Add(new
                {
                    Name = $"部门{i+1}级",
                    Property = $"Market_{i + 1}ID",
                    Pid = 3,
                });
            }
            //追加部门数据
            list.AddRange(market);
            List<dynamic> acc = new List<dynamic>();
            list.Add(new { Name = "科目级次", Pid = 0,Id = 4 });
            for (int i = 0; i < AccSubjectRank; i++)
            {
                acc.Add(new
                {
                    Name = $"科目{i+1}级",
                    Property = $"AccSubject_{i+1}ID",
                    Pid = 4,
                });
            }
            list.AddRange(acc); 
            list.Add(new { Name = "其他", Pid = 0, Id = 999 });
            list.Add(new { Name = "员工", Property = "PersonName", Pid = 999, Id = 5 });
            list.Add(new { Name = "月份", Property = "Month",  Pid = 999, Id = 6 });

            //https://confluence.nxin.com/pages/viewpage.action?pageId=73531399
            var url = $"{_hostCongfiguration.NxinGatewayUrl}api/nxin.qlwbase.organization.rank/1.0";
            //636181287875447931 = 部门
            var param = new { enterpriseid = model.GroupId, inheritanceid = 636181287875447931, ismaster = 1, isuse = 1 };
            var resMarket = _httpClientUtil1.PostFormAsync<dynamic>(url, param)?.Result;
            //组织类型单位：636346361736274263
            param = new { enterpriseid = model.GroupId, inheritanceid = 636346361736274263, ismaster = 1, isuse = 1 };
            var resEnter = _httpClientUtil1.PostFormAsync<dynamic>(url, param)?.Result;
            list.Add(new { Name = "单位组织", Pid = 0, Id = 1 });
            list.Add(new { Name = "部门组织", Pid = 0, Id = 2 });
            //单位组织 赋值
            if (resEnter.data != null)
            {
                for (int i = 0; i < resEnter.data.rankName.Count; i++)
                {
                    var temp = resEnter.data.rankName[i];
                    list.Add(new
                    {
                        Name = temp.name,
                        Property = $"org_en{temp.rank}",
                        Pid = 1,
                    });
                }
            }
            list.Add(new { Name = "单位", Pid = 1, Property = "EnterpriseName", Id = 7 });
            //部门组织 赋值
            if (resMarket.data != null)
            {
                for (int i = 0; i < resMarket.data.rankName.Count; i++)
                {
                    var temp = resMarket.data.rankName[i];
                    list.Add(new
                    {
                        Name = temp.name,
                        Property = $"org_market{temp.rank}",
                        Pid = 2,
                    });
                }
            }
            return list;
        }
        /// <summary>
        /// 营销汇总毛利表 汇总方式
        /// </summary>
        /// <returns></returns>
        public dynamic SalesSumamryGroupList(dynamic model)
        {
            List<dynamic> list = new List<dynamic>();
            //获取部门最大级次
            string MaxRankMarket = $@"SELECT MAX(Rank) Rank from qlw_nxin_com.biz_market where EnterpriseID IN ({model.EnterpriseIds})";
            var MarketRank = _context.DynamicSqlQuery(MaxRankMarket).FirstOrDefault().Rank;
            list.Add(new { Name = "单位部门", Pid = 0, Id = 3 });
            List<dynamic> market = new List<dynamic>();
            for (int i = 0; i < 3; i++)
            {
                market.Add(new
                {
                    Name = $"部门{i + 1}级",
                    Property = $"Market_{i + 1}",
                    Pid = 3,
                });
            }
            //追加部门数据
            list.AddRange(market);
            #region 商品
            list.Add(new { Name = "商品", Pid = 0, Id = 5 });
            list.Add(new { Name = "商品分类 1级", Property = "pro_ifi1", Pid = 5 });
            list.Add(new { Name = "商品分类 2级", Property = "pro_ifi2", Pid = 5 });
            list.Add(new { Name = "商品分类 3级", Property = "pro_ifi3", Pid = 5 });
            list.Add(new { Name = "商品代号", Property = "ProductName", Pid = 5 });
            list.Add(new { Name = "商品名称", Property = "ProductGroupName", Pid = 5 });
            #endregion
            #region 客户区域
            list.Add(new { Name = "客户区域", Pid = 0, Id = 6 });
            list.Add(new { Name = "省", Property = "area_name1", Pid = 6 });
            list.Add(new { Name = "市", Property = "area_name2", Pid = 6 });
            list.Add(new { Name = "县", Property = "area_name3", Pid = 6 });
            list.Add(new { Name = "客户", Property = "CustomerName", Pid = 6 });
            #endregion
            #region 日期
            list.Add(new { Name = "日期", Pid = 0, Id = 7 });
            list.Add(new { Name = "月份", Property = "DataMonth", Pid = 7 });
            list.Add(new { Name = "日期", Property = "DataDate", Pid = 7 });
            #endregion
            #region 其他
            list.Add(new { Name = "其他", Pid = 0, Id = 8 });
            list.Add(new { Name = "业务员", Property = "Name", Pid = 8 });
            list.Add(new { Name = "销售摘要", Property = "SalesAbstract", Pid = 8 });
            #endregion
            //https://confluence.nxin.com/pages/viewpage.action?pageId=73531399
            var url = $"{_hostCongfiguration.NxinGatewayUrl}api/nxin.qlwbase.organization.rank/1.0";
            //636181287875447931 = 部门
            var param = new { enterpriseid = model.GroupId, inheritanceid = 636181287875447931, ismaster = 1, isuse = 1 };
            var resMarket = _httpClientUtil1.PostFormAsync<dynamic>(url, param)?.Result;
            //组织类型单位：636346361736274263
            param = new { enterpriseid = model.GroupId, inheritanceid = 636346361736274263, ismaster = 1, isuse = 1 };
            var resEnter = _httpClientUtil1.PostFormAsync<dynamic>(url, param)?.Result;
            list.Add(new { Name = "单位", Pid = 1, Property = "EnterpriseName" });
            list.Add(new { Name = "单位组织", Pid = 0, Id = 1 });
            list.Add(new { Name = "部门组织", Pid = 0, Id = 2 });
            //单位组织 赋值
            if (resEnter.data != null)
            {
                for (int i = 0; i < resEnter.data.rankName.Count; i++)
                {
                    var temp = resEnter.data.rankName[i];
                    list.Add(new
                    {
                        Name = temp.name,
                        Property = $"org_en{temp.rank}",
                        Pid = 1,
                    });
                }
            }
            //部门组织 赋值
            if (resMarket.data != null)
            {
                for (int i = 0; i < resMarket.data.rankName.Count; i++)
                {
                    var temp = resMarket.data.rankName[i];
                    list.Add(new
                    {
                        Name = temp.name,
                        Property = $"bs_en{(int)temp.rank + 1}",
                        Pid = 2,
                    });
                }
            }
            return list;
        }
    }
}
