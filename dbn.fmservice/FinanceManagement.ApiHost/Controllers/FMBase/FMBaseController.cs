using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FMBase;
using FinanceManagement.Common;
using FinanceManagement.Common.NewMakeVoucherCommon;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Infrastructure.Repositories;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FMBase
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FMBaseController : ControllerBase
    {
        BankAccountBalanceUtil _comUtil;
        FMBaseCommon _baseUnit;
        IMediator _mediator;
        IIdentityService _identityService;
        BIZ_DataDictODataProvider _dictProvider;
        TreeModelODataProvider _treeModelProvider;
        EnterprisePeriodUtil _enterpriseperiodUtil;
        IbsfileRepository _bsfileRepository;
        HttpClientUtil _httpClientUtil;
        BusinessOutputProvider _businessOutputProvider;
        HostConfiguration _hostCongfiguration;
        ILogger<FMBaseController> _logger;
        private readonly string inventorypid = "2016030703636788604";

        public FMBaseController(ILogger<FMBaseController> logger, HostConfiguration hostConfiguration, HttpClientUtil httpClientUtil, IbsfileRepository bsfileRepository, IMediator mediator, BankAccountBalanceUtil comUtil, FMBaseCommon baseUnit, IIdentityService identityService, BIZ_DataDictODataProvider dictProvider, TreeModelODataProvider treeModelProvider, EnterprisePeriodUtil enterpriseperiodUtil, BusinessOutputProvider businessOutputProvider)
        {
            _mediator = mediator;
            _baseUnit = baseUnit;
            _comUtil = comUtil;
            _dictProvider = dictProvider;
            _identityService = identityService;
            _treeModelProvider = treeModelProvider;
            _enterpriseperiodUtil = enterpriseperiodUtil;
            _bsfileRepository = bsfileRepository;
            _httpClientUtil = httpClientUtil;
            _hostCongfiguration = hostConfiguration;
            _logger = logger;
            _businessOutputProvider = businessOutputProvider;
        }
        [Authorize]
        [HttpGet]
        [Route("GetBsfiles")]
        public dynamic GetBsfiles(string nums)
        {
            var data = _bsfileRepository.GetBsfiles(nums);
            if (data?.Count == 0)
            {
                return new List<bsfile>();
            }
            var param = new
            {
                EnterpriseID = data.FirstOrDefault().EnterId,
                Type = 4,
            };
            var resultModel = _httpClientUtil.PostJsonAsync<ResultModel<PM_Person>>($"{_hostCongfiguration.QlwServiceHost}/api/BIZPerson/getPerson", param).Result;
            foreach (var item in data)
            {
                item.OwnerName = resultModel.Data.Where(m => m.UserID.ToString() == item.OwnerID).FirstOrDefault().PersonName;
            }
            return data;
        }
        /// <summary>
        /// 获取单据字
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSymbol")]
        public async Task<List<FM_TicketedPoint>> GetSymbol()
        {
            var result = await _baseUnit.GetSymbol(_identityService.EnterpriseId);
            if (result.ResultState)
            {
                return result.Data;
            }
            return new List<FM_TicketedPoint>();
        }
        /// <summary>
        /// 专用调用外部接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("IntermediaryRequest")]
        public dynamic IntermediaryRequest(IntermediaryRequest model)
        {
            _logger.LogInformation("专用调用外部接口 参数：" + JsonConvert.SerializeObject(model));
            object result = _baseUnit.IntermediaryRequest(model.type, model.url, model.param, model.cookie);
            _logger.LogInformation("专用调用外部接口 结果：" + JsonConvert.SerializeObject(result));
            return result;
        }
        /// <summary>
        /// 根据单位ID 获取 单位信息
        /// </summary>
        /// <param name="EnterpriseId"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        [Route("GetEnterpriseInfo")]
        [AllowAnonymous]
        public dynamic GetEnterpriseInfo(string EnterpriseId)
        {
            return _businessOutputProvider.GetEnterprise(EnterpriseId);
        }
        /// <summary>
        /// 获取单据字
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSymbolById")]
        public async Task<List<FM_TicketedPoint>> GetSymbolById(string EnterpriseId = "")
        {
            if (string.IsNullOrEmpty(EnterpriseId))
            {
                var result = await _baseUnit.GetSymbol(_identityService.EnterpriseId);
                if (result.ResultState)
                {
                    return result.Data;
                }
            }
            else
            {
                var result = new ResultModel<FM_TicketedPoint>() { Data = new List<FM_TicketedPoint>() };
                foreach (var item in EnterpriseId.Split(','))
                {
                    var tr = await _baseUnit.GetSymbol(item);
                    if (tr.ResultState)
                    {
                        result.Data.AddRange(tr.Data);
                    }
                }
                return result.Data;
            }

            return new List<FM_TicketedPoint>();
        }
        [HttpGet]
        [Route("GetBalanceSheetReviwe")]
        public Result GetBalanceSheetReviwe(string enterpriseId, string year, string month)
        {
            try
            {
                if (string.IsNullOrEmpty(enterpriseId))
                {
                    return new Result() { code = ErrorCode.NoContent.GetIntValue(), msg = "enterpriseId 不能为空" };
                }
                if (string.IsNullOrEmpty(year))
                {
                    return new Result() { code = ErrorCode.NoContent.GetIntValue(), msg = "year 不能为空" };
                }
                if (string.IsNullOrEmpty(month))
                {
                    return new Result() { code = ErrorCode.NoContent.GetIntValue(), msg = "month 不能为空" };
                }
                return new Result() { code = ErrorCode.Success.GetIntValue(), msg = "成功", data = _dictProvider.GetBalanceSheetReviwe(enterpriseId, year, month) };
            }
            catch (Exception e)
            {
                _logger.LogError("获取资产负债表填报审核状态失败：" + JsonConvert.SerializeObject(e));
                return new Result() { code = ErrorCode.RequestArgumentError.GetIntValue(), msg = "获取失败", errors = new List<ErrorRow>() { new ErrorRow() { columns = new List<ErrorColumn>() { new ErrorColumn() { value = e } } } } };
            }
        }
        /// <summary>
        /// 获取系统选项
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("OptionConfigValue")]
        public bool OptionConfigValue(string optionId, string EnterpriseId = "")
        {
            if (string.IsNullOrEmpty(EnterpriseId))
            {
                EnterpriseId = _identityService.EnterpriseId;
            }
            var result = _baseUnit.OptionConfigValue(optionId, EnterpriseId);
            if (result != "0" && !string.IsNullOrEmpty(result))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 新：获取系统选项
        /// 2023-06-20 11:29:20
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("OptionConfigValueNew")]
        public bool OptionConfigValueNew(string optionId, string EnterpriseId = "")
        {
            if (string.IsNullOrEmpty(EnterpriseId))
            {
                EnterpriseId = _identityService.EnterpriseId;
            }
            var result = _baseUnit.OptionConfigValueNew(optionId, EnterpriseId);
            if (result != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获取字典
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetDataDictAsync")]
        public async Task<List<BIZ_DataDictODataEntity>> GetDataDictAsync(BIZ_DataDictODataEntity model)
        {
            if (model.PID != "1711231120560000101" && model.PID != "202205111355001101")
            {
                var result = await _dictProvider.GetDataDictAsync(model.PID);//取业务库字典
                if (model.PID == "201610140104402301")//采购结转=》采购摘要
                {
                    result.RemoveAll(s => s.DataDictID == "201610140104402305" || s.DataDictID == "1712301642260000101");
                }
                if (model.PID == "1211081429200000101")//老版结转类别PID
                {
                    var isHave = await _baseUnit.GetCheckenMealJurisdiction(new DropSelectSearch() { EnterpriseID = _identityService.EnterpriseId });
                    if (!Convert.ToBoolean(isHave.Data))//没有禽成本套餐的话就移除禽成本结转
                    {
                        result.RemoveAll(s => s.DataDictID == "1911081429200000104" || s.DataDictID == "1911081429200099105");
                    }
                }
                if (model.PID == "1911081429200099103")//福利计提
                {
                    var option = OptionConfigValue("20221024095722");
                    if (option)
                    {
                        List<string> removeList = new List<string>() { "2111261612570000102", "2111261612570000104", "2111261612570000106", "2111261612570000110", "2111261612570000112", "2111261612570000114", "2111261612570000116", "2111261612570000118", "2111261612570000120", "2111261612570000121", "2111261612570000122" };
                        result.RemoveAll(s => removeList.Contains(s.DataDictID));
                    }
                }
                //【83606】禽成本结转，增加蛋成本表及费用项目，分录生成条件增加商品分类
                //来源数据=种禽成本流转表、孵化成本流转表，借贷方公式配置数据源增加自定义费用项目.
                if (model.PID == "1911081429200099106")
                {
                    AuthenticationHeaderValue authentication = null;
                    AuthenticationHeaderValue.TryParse(HttpContext.Request.GetAuthToken(), out authentication);
                    var resultModel = _httpClientUtil.PostJsonAsync<List<BIZ_DataDictODataEntity>>($"{_hostCongfiguration._wgUrl}/q/reportbreed/PoultryCostProject",null, 
                        (a) => { a.Authorization = authentication; }).Result;
                    if (resultModel?.Count > 0)
                    {
                        return resultModel;
                    }
                }
                return result.Distinct().ToList();
            }
            else
            {
                var result = await _dictProvider.GetDataDictAsyncExtend(model.PID);//取基础库字典
                return result;
            }

        }
        /// <summary>
        /// 获取字典根据PID
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetDataDictByPid")]
        public async Task<List<BIZ_DataDictODataEntity>> GetDataDictByPid(BIZ_DataDictODataEntity model)
        {
            var result = await _dictProvider.GetDataDictAsyncExtend(model.PID);//取基础库字典
            return result;
        }
        /// <summary>
        /// 按结算类型获取摘要
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetReceiptAbstractList")]
        public async Task<List<ReceiptAbstract>> GetReceiptAbstractList(string EnterDate)
        {
            var result = await _baseUnit.GetReceiptAbstractList(_identityService.EnterpriseId, "2", EnterDate);
            return result;
        }
        /// <summary>
        /// 获取科目
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSubjectList")]
        public async Task<List<Subject>> GetSubjectList(string EnterDate, string EnterpriseId)
        {
            var result = await _baseUnit.GetSubjectList(0, Convert.ToInt64(string.IsNullOrEmpty(EnterpriseId) ? _identityService.EnterpriseId : EnterpriseId), EnterDate);
            return result;
        }
        [HttpPost]
        [Route("GetTreeModelAsync")]
        public List<TreeModelODataEntity> GetTreeModelAsync(DropSelectSearch searchModel)
        {
            List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
            searchModel.EnterpriseID = _identityService.EnterpriseId;
            searchModel.Boid = _identityService.UserId;
            searchModel.GroupID = _identityService.GroupId;
            if (string.IsNullOrEmpty(searchModel.InEnterpriseID))
            {
                searchModel.InEnterpriseID = _identityService.EnterpriseId;
            }
            var list = searchModel.InEnterpriseID.Split(',');
            switch (searchModel.SortID)
            {
                case 1001://商品分类
                    model = _treeModelProvider.GetProductGroupClassAsync(searchModel.GroupID);
                    break;
                case 1002://供应商
                    model = _treeModelProvider.GetSupplierAsync(searchModel.InEnterpriseID);
                    break;
                case 1003://物品分类
                    model = _treeModelProvider.GetSuppliesAsync(_identityService.EnterpriseId);
                    break;
                case 1004://部门信息
                    foreach (var item in list)
                    {
                        searchModel.EnterpriseID = item;
                        model.AddRange(_baseUnit.GetMarket(searchModel));
                    }
                    break;
                case 1005://人员信息
                    foreach (var item in list)
                    {
                        searchModel.EnterpriseID = item;
                        model.AddRange(_baseUnit.GetPerson(searchModel));
                    }
                    break;
                case 1006://客户信息
                    foreach (var item in list)
                    {
                        searchModel.EnterpriseID = item;
                        model.AddRange(_baseUnit.GetCustomer(searchModel));
                    }
                    break;
                case 1007://鸡场信息
                    model = _baseUnit.GetChickenFarm(searchModel);
                    break;
                case 1008://品种信息
                    model = _baseUnit.GetBreeding(searchModel);
                    break;
                case 1009://批次信息
                    model = _baseUnit.GetBatching(searchModel);
                    break;
                case 1010://厂区信息
                    model = _baseUnit.GetJurisdictionList(searchModel, "2");
                    break;
                case 1011://鸡舍信息
                    model = _baseUnit.GetJurisdictionList(searchModel, "3");
                    break;
                case 1012://项目名称设置信息
                    model = _baseUnit.GetProjectNameSettings(searchModel);
                    break;
                case 1014://猪场信息
                    model = _baseUnit.GetPigFarm(searchModel);
                    break;
                case 1015://商品代号信息
                    model = _baseUnit.getProductData(searchModel);
                    break;
                case 1017://费用性质
                    var data = GetDataDictAsync(new BIZ_DataDictODataEntity() { PID = "202205111355001101" }).Result;
                    data?.ForEach(s =>
                    {
                        model.Add(new TreeModelODataEntity()
                        {
                            Id = s.DataDictID,
                            cName = s.DataDictName
                        });
                    });
                    break;
                case 1013://销售摘要信息
                case 1018://采购摘要信息
                    var data1 = GetDataDictAsync(new BIZ_DataDictODataEntity() { PID = searchModel.PID }).Result;
                    data1?.ForEach(s =>
                    {
                        model.Add(new TreeModelODataEntity()
                        {
                            Id = s.DataDictID,
                            cName = s.DataDictName
                        });
                    });
                    break;
                case 1019://猪只类型
                    model = _baseUnit.GetPigTypes(searchModel.EnterpriseID);
                    break;
                case 1020://资产类别
                    model = _baseUnit.getFA_AssetsClassificationData(searchModel.GroupID);
                    break;
                case 1021://禽养殖场
                    model = _baseUnit.getChickenFarmList(searchModel.EnterpriseID);
                    break;
                case 1022://出入库方式
                    model = _baseUnit.GetInOutAbstract();
                    break;
                case 1023://发票类型
                    model = _baseUnit.GetInvoiceType();
                    break;
                case 1024://运费摘要
                    model = _baseUnit.GetCarriageAbstract(searchModel.EnterpriseID);
                    break;
                case 1025://存货分类
                    model = _dictProvider.GetDataDictConvertDrop(inventorypid);//取基础库字典;
                    break;
                default:
                    break;
            }
            model = model.GroupBy(p => p).Select(p => p.Key).ToList();//去重
            return model;
        }
        [HttpPost]
        [Route("GetSortTypeList")]
        public List<SortType> GetSortTypeList(FM_CarryForwardVoucherODataEntity model)
        {
            return _baseUnit.GetSortTypeList(model.TransferAccountsType);
        }
        [HttpGet]
        [Route("GetDataSource")]
        public FM_CarryForwardVoucherODataEntity GetDataSource(string type)
        {
            return _baseUnit.GetDataSource(type);
        }
        /// <summary>
        /// 获取集团下所有单位
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetEnterpriseList")]
        public List<Biz_Enterprise> GetEnterpriseList()
        {
            var result = _baseUnit.GetGroupPermissonEnterData(_identityService.EnterpriseId, _identityService.UserId);
            return result;
        }
        /// <summary>
        /// 获取单位所有科目
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetEnterSubjectList")]
        public async Task<List<Subject>> GetEnterSubjectList(string EnterDate)
        {
            var result = await _baseUnit.GetEnterSubjectList(0, Convert.ToInt64(_identityService.EnterpriseId), EnterDate);
            return result;
        }
        /// <summary>
        /// 获取单位客户
        /// </summary>
        /// <param name="enteID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetCustomerList")]
        public List<FM_Customer> GetCustomerList(string EnterpriseID)
        {
            if (string.IsNullOrEmpty(EnterpriseID) || EnterpriseID == "0") { EnterpriseID = _identityService.EnterpriseId; }
            return _baseUnit.GetCustomerList(EnterpriseID);
        }

        /// <summary>
        /// 获取基础库字典
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetDataDictAsyncExtend")]
        public async Task<List<BIZ_DataDictODataEntity>> GetDataDictAsyncExtend(BIZ_DataDictODataEntity model)
        {
            var result = await _dictProvider.GetDataDictAsyncExtend(model.PID);
            if (model.PID == "201904121023082101")
            { //费用项目类型字典
                //猪场设置：2005201609450000101
                //生产入库单：1611121402400000101
                //代宰结算单：1901091342520000100
                var resultData = await _baseUnit.GetmenuListByEnterpriseId(_identityService.EnterpriseId, "2005201609450000101,1611121402400000101,1901091342520000100");
                if (resultData != null && resultData.Data != null)
                {
                    List<string> menulist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(resultData.Data.ToString());
                    if (!menulist.Contains("2005201609450000101"))
                    {
                        result.RemoveAll(_ => _.DataDictID == "201904121023082104");
                    }
                    if (!menulist.Contains("1611121402400000101"))
                    {
                        result.RemoveAll(_ => _.DataDictID == "201904121023082103");
                    }
                    if (!menulist.Contains("1901091342520000100"))
                    {
                        result.RemoveAll(_ => _.DataDictID == "201904121023082102");
                    }
                }
            }
            if (model.PID == "2022021710000")
            { //费用项目设置功能(费用预置项目DropList)，排除使用过的
                result = await _dictProvider.GetDataDictAsyncForPreSetItem(model.PID, model.EnterpriseID);
            }
            return result;
        }

        /// <summary>
        /// 获取单位会计期间-成本计算设置专用
        /// </summary>
        /// <param name="enteID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetEnterperisePeriodList")]
        public List<EnterperisePeriod> GetEnterperisePeriodList(EnterperisePeriod model)
        {
            if (string.IsNullOrEmpty(model.EnterpriseID) || model.EnterpriseID == "0") { model.EnterpriseID = _identityService.EnterpriseId; }

            var result = _enterpriseperiodUtil.GetEnterperisePeriodList(model.EnterpriseID, model.Year, model.Month);
            for (int i = 1; i < 3; i++)
            {
                var result1 = _enterpriseperiodUtil.GetEnterperisePeriodList(model.EnterpriseID, model.Year + i, model.Month);
                result1.ForEach(_ =>
                {
                    if (!result.Any(a => a.Year == _.Year && a.Month == _.Month))
                    {
                        result.Add(_);
                    }
                });
            }

            return result;
        }

        /// <summary>
        /// 获取科目类型是损益、成本类型的科目集合
        /// </summary>
        /// <param name="EnterDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSubjectListForTree")]
        public async Task<List<Subject>> GetSubjectListForTree(string EnterDate)
        {
            var result = await _baseUnit.GetSubjectListExtend(0, Convert.ToInt64(_identityService.EnterpriseId), EnterDate);

            var result1 = result.Select(_ => new Subject()
            {
                AccoSubjectID = _.AccoSubjectID,
                cAccoSubjectName = _.cAccoSubjectName,
                cAccoSubjectFullName = _.cAccoSubjectFullName,
                PID = _.Rank == 1 ? _.AccoSubjectType.ToString() : _.PID.ToString(),
                //isParent = result.Where(a => a.PID.ToString() == _.AccoSubjectID).Any(),
                cAccoSubjectCode = _.cAccoSubjectCode,
                AccoSubjectType = _.AccoSubjectType.ToString(),
                Rank = _.Rank,
                bTorF = _.bTorF,
                bLorR = _.bLorR,
                bEnd = _.bEnd,
                bProject = _.bProject,
                bCus = _.bCus,
                bPerson = _.bPerson,
                bSup = _.bSup,
                bDept = _.bDept,
                bItem = _.bItem,
                bCash = _.bCash,
                bBank = _.bBank,
                cAxis = _.cAxis
            }).ToList();

            var accTypes = await _dictProvider.GetDataDictAsyncExtend("201612180104402001");//科目类型 

            //损益、成本
            var dicAccTypes = accTypes.Where(_ => _.DataDictID == "201612180104402105" || _.DataDictID == "201612180104402106");

            var filterResult = result1.Where(_ => dicAccTypes.Any(a => a.cPrivCode == _.AccoSubjectType)).OrderBy(_ => _.cAccoSubjectCode).ToList();

            filterResult.AddRange(dicAccTypes.Select(n => new Subject() { AccoSubjectID = n.cPrivCode.ToString(), cAccoSubjectName = n.DataDictName, PID = "0" }).ToList());

            return filterResult;
        }

        [HttpGet]
        [Route("GetUnitMeasurement")]
        public List<UnitMeasurement> GetUnitMeasurement()
        {
            return _baseUnit.GetUnitMeasurement(_identityService.EnterpriseId);
        }
        [HttpPost]
        [Route("GetRecesAccountInfo")]
        public List<Bank_Account> GetRecesAccountInfo(string BusinessType, string PayeeID, string PayerID)
        {
            return _baseUnit.GetRecesAccountInfo(BusinessType, PayeeID, PayerID);
        }
        [HttpPost]
        [Route("GetAccountInfoList")]
        public List<Bank_Account> GetAccountInfoList(AccountInfoReq req)
        {
            return _baseUnit.GetAccountInfoList(req);
        }
        [HttpPost]
        [Route("GetAccountRelatePayType")]
        public AccountInfo GetAccountRelatePayType(string AccountID, string EnterpriseID, string EnterData)
        {
            return _baseUnit.GetAccountRelatePayType(AccountID, EnterpriseID, EnterData);
        }
        [HttpGet]
        [Route("GetBelongProject")]
        public List<FMProject> GetBelongProject(string EnterpriseId = "")
        {
            return _baseUnit.GetBelongProject(string.IsNullOrEmpty(EnterpriseId) ? _identityService.EnterpriseId : EnterpriseId);
        }
        [HttpGet]
        [Route("GetDataDact")]
        public List<DataDictModel> GetDataDact()
        {
            return _baseUnit.GetDataDact("201611160104402001");
        }
        [HttpPost]
        [Route("GetSummaryByType")]
        public List<Summary> GetSummaryByType(DropSelectSearch searchModel)
        {
            searchModel.EnterpriseID = string.IsNullOrEmpty(searchModel.InEnterpriseID) ? _identityService.EnterpriseId : searchModel.InEnterpriseID;
            List<Summary> list = new List<Summary>();
            foreach (var item in searchModel.EnterpriseID.Split(','))
            {
                searchModel.EnterpriseID = item;
                list.AddRange(_baseUnit.GetSummaryByType(searchModel.relationId, searchModel));
            }
            return list;
        }
        [HttpGet]
        [Route("GetProductList")]
        public List<ResponseProductInfoListForFDCostCoefficient> GetProductList(string EnterpriseId = "")
        {
            return _baseUnit.GetProductListForCostCoefficientTwo(string.IsNullOrEmpty(EnterpriseId) ? _identityService.EnterpriseId : EnterpriseId);
        }
        [HttpGet]
        [Route("CheckFMAccount")]
        public ResultModel CheckFMAccount(string ownerID, string dataDate, string appId, string enterpriseID = "")
        {
            if (string.IsNullOrEmpty(ownerID))
            {
                ownerID = _identityService.UserId;
            }
            if (string.IsNullOrEmpty(enterpriseID))
            {
                enterpriseID = _identityService.EnterpriseId;
            }
            return _baseUnit.CheckFMAccount(ownerID, dataDate, appId, enterpriseID);
        }
        [HttpGet]
        [Route("GetPigFarmAsync")]
        public List<TreeModelODataEntity> GetPigFarmAsync()
        {
            return _treeModelProvider.GetPigFarmAsync();
        }
        [HttpGet]
        [Route("GetYqtAccountBalInfo")]
        public async Task<BankAccountBalanceInnerResult> GetYqtAccountBalInfo(string AccountID)
        {
            var accountFirstData = await _comUtil.GetFMAccountData(new FMAccount() { AccountID = AccountID });
            bool OpenBankEnterConnect = Convert.ToBoolean(accountFirstData.Data?.FirstOrDefault().OpenBankEnterConnect);
            if (OpenBankEnterConnect)
            {
                BankAccountBalanceInnerResult result1 = _comUtil.GetYqtAccountBalInfo(accountFirstData.Data?.FirstOrDefault());
                if (result1 != null)
                {
                    return result1;
                }
                else
                {
                    return new BankAccountBalanceInnerResult();
                }
            }
            else
            {
                return new BankAccountBalanceInnerResult();
            }
        }

        /// <summary>
        /// 按级次获取部门
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetMarketByRank")]
        public List<TreeModelODataEntity> GetMarketByRank(DropSelectSearch model)
        {
            if (string.IsNullOrEmpty(model.EnterpriseID) || model.EnterpriseID == "0") { model.EnterpriseID = _identityService.EnterpriseId; }
            return _baseUnit.GetMarketByRank(model);
        }
        /// <summary>
        /// 按级次获取区域
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetArea")]
        public List<TreeModelODataEntity> GetArea(DropSelectSearch searchModel)
        {
            return _baseUnit.GetArea(searchModel);
        }
        /// <summary>
        /// 获取字典
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetDataDict")]
        public List<DataDictModel> GetDataDict(DataDictRequest searchModel)
        {
            return _baseUnit.GetDataDict(searchModel);
        }


        #region 科目
        /// <summary>
        /// 资金科目 1001,1002,1012的所有科目
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetFundSubjectList")]
        public async Task<List<BIZ_AccoSubject>> GetFundSubjectList(DropSelectSearch param)
        {
            if (string.IsNullOrEmpty(param.CurrentEnterDate))
            {
                param.CurrentEnterDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            var resultData = new List<BIZ_AccoSubject>();
            var data = await _baseUnit.GetSubjectListExtend(0, long.Parse(param.EnterpriseID), param.CurrentEnterDate);
            foreach (var item in data)
            {
                var code = item.cAccoSubjectCode?.Substring(0, 4);
                if (code == "1001" || code == "1002" || code == "1012")
                {
                    if (item.Rank == 1) item.PID = "0";
                    resultData.Add(item);
                }
            }
            resultData = resultData.OrderBy(o => o.cAccoSubjectCode).ToList();
            return resultData;
        }
        /// <summary>
        /// 费用科目 6601,6602,6603,5301 
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetCostSubjectList")]
        public async Task<List<BIZ_AccoSubject>> GetCostSubjectList(DropSelectSearch searchModel)
        {
            if (string.IsNullOrEmpty(searchModel.EnterpriseID) || searchModel.EnterpriseID == "0") { searchModel.EnterpriseID = _identityService.EnterpriseId; }
            if (string.IsNullOrEmpty(searchModel.CurrentEnterDate))
            {
                searchModel.CurrentEnterDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            var resultData = new List<BIZ_AccoSubject>();
            var data = await _baseUnit.GetSubjectListExtend(0, long.Parse(searchModel.EnterpriseID), searchModel.CurrentEnterDate);
            foreach (var item in data)
            {
                var code = item.cAccoSubjectCode?.Substring(0, 4);
                if (code == "6601" || code == "6602" || code == "6603" || code == "5301")
                {
                    if (item.Rank == 1) item.PID = "0";
                    resultData.Add(item);
                }
            }
            resultData = resultData.OrderBy(o => o.cAccoSubjectCode).ToList();
            return resultData;
        }
        #endregion
        /// <summary>
        /// 获取薪资项信息
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSalarySetItemList")]
        public List<SalarySetItem> GetSalarySetItemList()
        {
            AuthenticationHeaderValue authentication = null;
            bool verification = AuthenticationHeaderValue.TryParse(HttpContext.Request.GetAuthToken(), out authentication);
            if (!verification)
            {
                return null;
            }
            List<SalarySetItem> result = new List<SalarySetItem>();
            var option = OptionConfigValue("20221024095722");
            if (option)
            {
                result = new List<SalarySetItem>() { new SalarySetItem()
                {
                    SetItemId = "2111261612570000166",
                    SetItemName = "月收入"
                },new SalarySetItem()
                {
                    SetItemId = "2111261612570000188",
                    SetItemName = "绩效工资"
                }};
            }
            else
            {
                result = _baseUnit.GetSalarySetItemList(new Common.NewMakeVoucherCommon.FM_CarryForwardVoucherSearchCommand() { EnterpriseID = _identityService.EnterpriseId }, authentication);
            }
            result.Insert(0, new SalarySetItem() { SetItemId = "1612131431310000101", SetItemName = "薪资汇总表" });
            return result;
        }
        /// <summary>
        /// 费用分摊结转公式
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetShareCostItemList")]
        public List<ShareCostItem> GetShareCostItemList(int tableType)
        {
            List<ShareCostItem> result = new List<ShareCostItem>();
            result = _baseUnit.GetShareCostItemList(new Common.NewMakeVoucherCommon.FM_CarryForwardVoucherSearchCommand() { EnterpriseID = _identityService.EnterpriseId });
            result.ForEach(s =>
            {
                s.CostProjectName = s.CostProjectCode + s.CostProjectName;
            });
            if (tableType == 1)
            {
                result.Insert(0, new ShareCostItem() { CostProjectId = "2201101409320000151", CostProjectName = "取值明细(费用分摊明细表)" });
            }
            else
            {
                result.Insert(0, new ShareCostItem() { CostProjectId = "2201101409320000152", CostProjectName = "分摊明细(费用分摊明细表)" });
            }
            return result;
        }
        /// <summary>
        /// 获取单位信息
        /// </summary>
        /// <param name="isgroup"></param>
        /// <returns></returns>
        [HttpGet("GetEnterpriseList/{isgroup}")]
        public async Task<List<Biz_EnterpirseEntityODataEntity>> GetEnterpriseList(bool isgroup)
        {
            return await _dictProvider.GetEnterpriseListAsync(isgroup);
        }
        [HttpPost]
        [Route("GetCurrentUnitList")]
        public async Task<List<Biz_CustomerODataEntity>> GetCurrentUnitList(CustomerSearch model)
        {
            if (string.IsNullOrEmpty(model.EnterpriseID)) return new List<Biz_CustomerODataEntity>(); //{ model.EnterpriseID = _identityService.EnterpriseId; }
            return await _baseUnit.GetCurrentUnitList(model);
        }
        /// <summary>
        /// 获取汇总方式-费用分析表专用
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetCostSummary")]
        public dynamic GetCostSummary()
        {
            string entes = _baseUnit.GetAuthorEnterpise(_identityService.EnterpriseId, _identityService.UserId);
            dynamic data = new ExpandoObject();
            data.GroupId = _identityService.GroupId;
            data.EnterpriseIds = entes;
            if (string.IsNullOrEmpty(entes))
            {
                return new List<dynamic>();
            }
            return _baseUnit.SumamryGroupList(data);
        }
        /// <summary>
        /// 获取汇总方式-营销汇总毛利表专用
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetSalesSummary")]
        public dynamic GetSalesSummary()
        {
            string entes = _baseUnit.GetAuthorEnterpise(_identityService.EnterpriseId, _identityService.UserId);
            dynamic data = new ExpandoObject();
            data.GroupId = _identityService.GroupId;
            data.EnterpriseIds = entes;
            if (string.IsNullOrEmpty(entes))
            {
                return new List<dynamic>();
            }
            return _baseUnit.SalesSumamryGroupList(data);
        }
        /// <summary>
        /// 获取集团会计科目-费用分析专用
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetAccoSubjectGroup")]
        public dynamic GetAccoSubjectGroup()
        {
            dynamic data = new ExpandoObject();
            data.GroupId = _identityService.GroupId;
            return _baseUnit.GetAccoSubjectGroup(data);
        }
        [HttpGet("GetGroupProductList/{productID}")]
        public List<dynamic> GetGroupProductList(string productID)
        {
            return _baseUnit.GetGroupProductList(_identityService.GroupId, productID);
        }
        /// <summary>
        /// 获取物资本月出库金额
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetSuppliesOutAmountByMonth")]
        public List<dynamic> GetSuppliesOutAmountByMonth(DropSelectSearch model)
        {
            return _businessOutputProvider.GetSuppliesOutAmountByMonth(model);
        }
    }
}
