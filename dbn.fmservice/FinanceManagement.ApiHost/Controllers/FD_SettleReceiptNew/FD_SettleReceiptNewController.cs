using Architecture.Common.HttpClientUtil;
using Architecture.Common.NumericalOrderCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using Aspose.Cells;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_SettleReceipt;
using FinanceManagement.ApiHost.Controllers.FD_SettleReceiptNew;
using FinanceManagement.Common;
using FinanceManagement.Common.MakeVoucherCommon;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_SettleReceiptInterface
{
    [Route("api/[controller]")]
    [ApiController]
    public class FD_SettleReceiptNewController : ControllerBase
    {
        IMediator _mediator;
        FD_SettleReceiptNewODataProvider _provider;
        IIdentityService _identityService;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;
        NumericalOrderCreator _numericalOrderCreator;
        VoucherAmortizationUtil _comUtil;
        FMBaseCommon _fmBaseCommon;
        ILogger<FD_SettleReceiptNewController> _logger;
        public FD_SettleReceiptNewController(ILogger<FD_SettleReceiptNewController> logger, FMBaseCommon fmBaseCommon,VoucherAmortizationUtil comUtil, NumericalOrderCreator numericalOrderCreator, IMediator mediator, FD_SettleReceiptNewODataProvider provider,  IIdentityService identityService, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _mediator = mediator;
            _provider = provider;
            _identityService = identityService;
            _httpClientUtil1 = httpClientUtil;
            _numericalOrderCreator = numericalOrderCreator;
            _hostCongfiguration = hostCongfiguration;
            _comUtil = comUtil;
            _fmBaseCommon = fmBaseCommon;
            _logger = logger;
        }
        /// <summary>
        /// 获取资金账户期初数据
        /// </summary>
        /// <param name="begindate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetFundList")]

        public dynamic GetFundList(string begindate,string enddate)
        {
            List<dynamic> list = _provider.GetFundList(begindate, enddate);
            if (list.Count == 0)
            {
                return new List<dynamic>().AsQueryable();
            }
            else
            {
                return list.AsQueryable();
            }
        }
        //增加
        [HttpPost]
        [Route("AddFund")]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> AddFund([FromBody] FD_SettleReceiptNewFundAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //批量审核
        [HttpPost]
        [Route("BatchReview")]
        public async Task<Result> BatchReview([FromBody] FD_SettleReceiptBatchReviewCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //批量取消审核
        [HttpPost]
        [Route("BatchCancelReview")]
        public async Task<Result> BatchCancelReview([FromBody] FD_SettleReceiptBatchCancelReviewCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //凭证整理
        [HttpPost]
        [Route("RemakeNumber")]
        public async Task<Result> RemakeNumber([FromBody] FD_SettleReceiptRemakeCommand request)
        {
            var param = new VerificationCheckoutModel { EnterpriseID = Convert.ToInt64(request.EnterpriseID), OwnerID = Convert.ToInt64(_identityService.UserId), DataDate = Convert.ToDateTime(request.endDate).ToString("yyyy-MM-dd"), AppID = 2108021645470000109 };
            string requrl = $"{this._hostCongfiguration.QlwServiceHost}/api/{request.EnterpriseID}/{_identityService.UserId}/FM_AccoCheck/IsLockForm";
            var res = _httpClientUtil1.PostJsonAsync<ResultModel>(requrl, param).Result;
            if ((bool)res?.ResultState)
            {
                return new Result()
                {
                    msg = "已结账会计期间不允许整理",
                    code = ErrorCode.RequestArgumentError.GetIntValue(),
                };
            }
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //增加
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FD_SettleReceiptNewAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //删除
        [HttpDelete]
        public async Task<Result> Delete([FromBody] FD_SettleReceiptNewDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        public async Task<Result> Modify([FromBody] FD_SettleReceiptNewModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        /// <summary>
        /// 会计凭证号验重
        /// </summary>
        /// <param name="SettleReceipType">凭证类型</param>
        /// <param name="EnterpriseID">单位</param>
        /// <param name="Number">单据号</param>
        /// <param name="BeginDate">会计期间开始日期</param>
        /// <param name="EndDate">结束日期</param>
        /// <param name="NumericalOrder">流水号 用于修改操作排除当前流水号的单据号</param>
        /// <param name="TicketedPoint"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("IsExitNumber")]
        public bool IsExitNumber(string SettleReceipType, string EnterpriseID, string Number, string BeginDate, string EndDate, string NumericalOrder = "", string TicketedPoint = "")
        {
            var number = _provider.GetIsExitNumber(SettleReceipType,EnterpriseID,Number,BeginDate,EndDate,NumericalOrder,TicketedPoint).MaxNumber;
            if (number == "0")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 会计凭证号获取最大号
        /// </summary>
        /// <param name="SettleReceipType">凭证类型</param>
        /// <param name="EnterpriseID">单位</param>
        /// <param name="Number">单据号</param>
        /// <param name="BeginDate">会计期间开始日期</param>
        /// <param name="EndDate">结束日期</param>
        /// <param name="TicketedPointID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetMaxNumberByDate")]
        public string GetMaxNumberByDate(string SettleReceipType, string EnterpriseID, string Number, string BeginDate, string EndDate,string TicketedPointID)
        {
            var number = _provider.GetMaxNumberByDate(SettleReceipType, EnterpriseID, BeginDate, EndDate, TicketedPointID).MaxNumber;
            return number;
        }
        /// <summary>
        /// 会计凭证断号列表集合
        /// </summary>
        /// <param name="settleReceipType"></param>
        /// <param name="enterpriseID"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="TicketedPointID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetBrokenNumber")]
        public List<string> GetBrokenNumber(string settleReceipType, string enterpriseID,string beginDate,string endDate, string TicketedPointID)
        {
            var maxNumber = Convert.ToInt64(_provider.GetMaxNumberByDate(settleReceipType, enterpriseID,beginDate,endDate,TicketedPointID).MaxNumber);
            var data = _provider.GetDataList(beginDate, endDate, enterpriseID).Where(m=>m.SettleReceipType == settleReceipType).OrderBy(m=>m.Number).ToList();
            //断号存储
            List<string> brokenList = new List<string>();
            //是否连续
            bool isContinuity = false;
            //连续临时存储
            string continuity = "";
            long count = 0;
            for (long i = 1; i < maxNumber; i++)
            {
                if (isContinuity)
                {
                    if (!data.Any(m => m.Number.ToString() == i.ToString()))
                    {
                        //碰到 断号范围较大的 直接从 目前截取的断号到最大号（maxNumber）
                        count++;
                        if (count > 10000)
                        {
                            if (continuity != (maxNumber).ToString())
                            {
                                continuity += "~" + (maxNumber);
                            }
                            brokenList.Add(continuity);
                            break;
                        }
                        continue;
                    }
                    else
                    {
                        if (continuity != (i - 1).ToString())
                        {
                            continuity += "~" + (i - 1);
                        }
                        brokenList.Add(continuity);
                        isContinuity = false;
                        continuity = "";
                        continue;
                    }
                }
                if (!data.Any(m => m.Number.ToString() == i.ToString()))
                {
                    isContinuity = true;
                    continuity = i.ToString();
                }
                else
                {
                    isContinuity = false;
                    continue;
                }
            }
            return brokenList;
        }
        /// <summary>
        /// 是否序时
        /// 选项ID：20180330173530 开启后，付款单、付款汇总单、收款单、收款汇总单、会计凭证 需要增加序时控制
        /// 单据或凭证日期不得早于当前会计期间内已存在单据或凭证的最晚日期，允许等于或晚于
        /// 在选择单据或凭证日期时，触发校验，提示：请遵循序时原则
        /// 关闭，不校验序时控制
        /// </summary>
        /// <param name="DataDate">凭证日期</param>
        /// <param name="SettleReceipType">凭证类别</param>
        /// <param name="EnterpriseID">单位</param>
        /// <returns></returns>
        [HttpGet]
        [Route("SequentialTime")]
        public bool SequentialTime(string DataDate, string EnterpriseID, string SettleReceipType)
        {
            return _provider.SequentialTime(DataDate, EnterpriseID, SettleReceipType);
        }
        /// <summary>
        /// 事件上传
        /// </summary>
        /// <param name="file">文件信息</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Upload")]
        [DisableRequestSizeLimit]
        public IActionResult Upload(IFormFile file)
        {
            try
            {
                _logger.LogInformation("凭证导入Token："+JsonConvert.SerializeObject(_identityService));   
                if (file == null)
                {
                    return Ok(new { code = "201", msg = "请上传文件" });
                }
                var fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension == null)
                {
                    return Ok(new { code = "201", msg = "文件无后缀信息" });
                }
                long length = file.Length;
                if (length > 1024 * 1024 * 300) //200M
                {
                    return Ok(new { code = "201", msg = "上传文件不能超过300M" });
                }

                using (Stream fs = file.OpenReadStream())
                {
                    //if (file.FileName.IndexOf(".xls") > -1)
                    //{
                    //    HSSFWorkbook workbook = new HSSFWorkbook(fs);
                    //}
                    XSSFWorkbook workbook = new XSSFWorkbook(fs);
                    ISheet sheet = workbook.GetSheetAt(0);
                    // 创建 DataTable 对象
                    DataTable dt = new DataTable();
                    // 获取列数
                    int columnCount = sheet.GetRow(0).LastCellNum;
                    // 遍历每一行并添加到 DataTable
                    int rowCount = sheet.LastRowNum + 1;
                    //启用跨单位绩效归属
                    //开启后收付款单、会计凭证业务单元（原所属单位）取组织类型为部门的组织信息
                    var optionId = "20190319190646";
                    var option = _provider.GetOptionInfos(optionId, _identityService.GroupId);
                    if (option == null || option.Count == 0)
                    {
                        option = new List<OptionInfo>();
                        option.Add(new OptionInfo() { EnterpriseId = _identityService.GroupId, OptionSwitch = false });
                    }
                    Dictionary<string, string> columDic = new Dictionary<string, string>()
                    {
                        {"*序号","Number"},
                        {"*账务单位","EnterpriseName"},
                        {"*单据字","TicketedPointName"},
                        {"*日期","DataDate"},
                        {"*凭证类别","SettleReceipType"},
                        {"*摘要","ReceiptAbstractName"},
                        {"*内容","Content"},
                        {"*会计科目","AccoSubjectName"},
                        { "员工","PersonName"},
                        {"部门","MarketName"},
                        {"客/商","CustomerName"},
                        { "项目","ProjectName"},
                        {"商品","ProductName"},
                        {"商品名称","ProductGroupName"},
                        {"商品分类","ClassificationName"},
                        {"借方金额","Debit"},
                        {"贷方金额","Credit"},
                        {"资金账户","AccountName"},
                        {"结算方式","PaymentTypeName"},
                        {Convert.ToBoolean((option.FirstOrDefault().OptionSwitch)) ? "业务单元" : "所属单位",(bool)(option.FirstOrDefault().OptionSwitch) ? "OrganizationSortID" : "EnterpriseID"},
                        {"附单据张数","AttachmentNum"}
                    };
                    // 添加列到 DataTable
                    // 读取第一行的数据并插入到 DataRow
                    var firstRow = sheet.GetRow(0);
                    for (int i = 0; i < columnCount; i++)
                    {
                        var cell = firstRow.GetCell(i);
                        dt.Columns.Add(cell?.ToString() ?? string.Empty);

                    }
                    for (int i = 1; i < rowCount; i++)
                    {
                        var row = sheet.GetRow(i);
                        DataRow dataRow = dt.NewRow();

                        for (int j = 0; j < columnCount; j++)
                        {
                            var cell = row.GetCell(j);
                            dataRow[j] = cell?.ToString() ?? string.Empty;
                        }

                        dt.Rows.Add(dataRow);
                    }

                    if (dt.Columns != null && dt.Columns.Count == columDic.Count)
                    {
                        string msg = "";
                        List<string> ct = new List<string>(columDic.Keys);
                        for (int i = 0; i < columDic.Count; i++)
                        {
                            if (columDic.ContainsKey(dt.Columns[i].ColumnName))
                            {
                                dt.Columns[i].ColumnName = columDic[dt.Columns[i].ColumnName];
                            }
                            else
                            {
                                msg += "模板中列名[" + dt.Columns[i].ColumnName + "]与单据中的列名[" + ct[i] + "]不一致,请修改后再导入";
                            }
                        }
                        if (!string.IsNullOrEmpty(msg))
                        {
                            return Ok(new { code = "201", status = false, msg = msg });
                        }
                    }
                    else
                    {
                        return Ok(new { code = "201", status = false, msg = "文件列不匹配" });
                    }



                    var id = _numericalOrderCreator.CreateAsync().Result;
                    var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    List<Tuple<string, object>> trans = new List<Tuple<string, object>>();

                    if (dt == null || dt.Rows.Count <= 1)
                    {
                        return Ok(new { status = false, code = 6, msg = "请导入最少两行数据！" });
                    }
                    var period = _comUtil.getEnterprisePeriod(new Common.MakeVoucherCommon.EnterprisePeriodSearch() { EnterpriseID = _identityService.EnterpriseId, Year = Convert.ToDateTime(dt.Rows[0]["DataDate"]).Year, Month = Convert.ToDateTime(dt.Rows[0]["DataDate"]).Month }).Result?.FirstOrDefault();
                    var BeginDate = period?.StartDate.ToString("yyyy-MM-dd");
                    var EndDate = period?.EndDate.ToString("yyyy-MM-dd");
                    #region 验证年度是否有会计结账 + 名称属性转换为ID 
                    //获取当前集团下单位信息
                    var enters = _provider.GetEnterpriseInfosByName();
                    var tickets = _provider.GetTicketPointInfos();
                    var settles = _provider.GetSettleSummaryInfos();
                    var projects = _provider.GetProjectInfos();
                    var markets = _provider.GetMarketInfos(dt.Rows[0]["DataDate"].ToString());
                    var cuosups = _provider.GetCusSupInfos();
                    //科目验证  根据日期去选择对应的科目
                    _logger.LogInformation("会计凭证导入科目数据入参："+ JsonConvert.SerializeObject(dt.Rows[0]["DataDate"].ToString()) + "\n 单位信息："+JsonConvert.SerializeObject(enters.Where(m => m.EnterpriseName == dt.Rows[0]["EnterpriseName"].ToString()).FirstOrDefault()));
                    var accos = _provider.GetAccoSubjectInfos(dt.Rows[0]["DataDate"].ToString(), enters.Where(m => m.EnterpriseName == dt.Rows[0]["EnterpriseName"].ToString()).FirstOrDefault());
                    var products = _provider.GetProductInfos();
                    var productgroups = _provider.GetProductGroupInfos();
                    var classification = _provider.GetClassificationInfos();
                    var accounts = _provider.GetAccountInfos();
                    var paytypes = _provider.GetPayTypeInfos();
                    List<string> dataList = new List<string>();
                    List<SettleExcelData> excelDatas = new List<SettleExcelData>();
                    List<Inheritance> inheritances = new List<Inheritance>();
                    var persons =  new List<PersonInfo>();
                    //统计全部日期   序号，单位，日期
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //单位替换
                        var Etemp = enters.Where(m => m.EnterpriseName == dt.Rows[i]["EnterpriseName"].ToString()).FirstOrDefault();
                        if (Etemp != null && !string.IsNullOrEmpty(dt.Rows[i]["EnterpriseName"].ToString()))
                        {
                            dt.Rows[i]["EnterpriseName"] = Etemp.EnterpriseId;
                            //如果没有开启了系统选项，所属单位替换
                            if (!option.FirstOrDefault().OptionSwitch)
                            {
                                if (!string.IsNullOrEmpty(dt.Rows[i]["EnterpriseID"].ToString()))
                                {
                                    var Detemp = enters.Where(m => m.EnterpriseName == dt.Rows[i]["EnterpriseID"].ToString()).FirstOrDefault();
                                    if (Detemp != null && !string.IsNullOrEmpty(dt.Rows[i]["EnterpriseID"].ToString()))
                                    {
                                        dt.Rows[i]["EnterpriseID"] = Detemp.EnterpriseId;
                                    }
                                    else
                                    {
                                        dt.Rows[i]["EnterpriseID"] = "无数据";
                                    }
                                }
                            }
                        }
                        else
                        {
                            return Ok(new { code = ErrorCode.RequestArgumentError.GetIntValue(), status = false, msg = $"单位名称【{dt.Rows[i]["EnterpriseName"].ToString()}】未匹配" });
                        }
                        //同一家单位的数据不循环调用

                        if (i == 0 || Etemp.EnterpriseId != dt.Rows[i - 1]["EnterpriseName"].ToString())
                        {
                            var personResult = _httpClientUtil1.GetJsonAsync<ResultModel<PersonInfo>>($"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.qlw.person.get/1.0?group_id={_identityService.GroupId}&open_req_src=dbn.fmservice&type=4&enter_id={Etemp.EnterpriseId}&isonjob=null").Result;
                            persons = personResult?.Data ?? new List<PersonInfo>();
                            if (persons.Count > 0)
                            {
                                persons.FirstOrDefault().EnterpriseId = Etemp.EnterpriseId;
                                //过滤部门 只要人名字
                                persons.ForEach(m => 
                                {
                                    m.PersonName = m.PersonName.IndexOf("-") > 0 ? m.PersonName.Split('-')[0] : m.PersonName;
                                });
                            }
                            var inheritancesResult = _httpClientUtil1.GetJsonAsync<ResultModel<Inheritance>>($"{_hostCongfiguration.NxinGatewayUrl}api/nxin.qlwbase.bsorg.QueryByInhIdAndEnterId/1.0?InheritanceID=636181287875447931&EnterpriseID={Etemp.EnterpriseId}&isused=1").Result;
                            inheritances = inheritancesResult?.Data ?? new List<Inheritance>();
                            if (persons.Count > 0)
                            {
                                persons.FirstOrDefault().EnterpriseId = Etemp.EnterpriseId;
                            }
                        }
                        //如果开了系统选项业务单元替换
                        if (option.FirstOrDefault().OptionSwitch)
                        {
                            if (!string.IsNullOrEmpty(dt.Rows[i]["OrganizationSortID"].ToString()))
                            {
                                var Itemp = inheritances.Where(m => m.cFullName == dt.Rows[i]["OrganizationSortID"].ToString()).FirstOrDefault();
                                if (Itemp != null && !string.IsNullOrEmpty(dt.Rows[i]["OrganizationSortID"].ToString()))
                                {
                                    dt.Rows[i]["OrganizationSortID"] = Itemp.SortId;
                                }
                                else
                                {
                                    dt.Rows[i]["OrganizationSortID"] = "无数据";
                                }
                            }
                        }
                        //单据字替换
                        var Ttemp = tickets.Where(m => m.EnterpriseId == Etemp.EnterpriseId && m.TicketedPointName == dt.Rows[i]["TicketedPointName"].ToString()).FirstOrDefault();
                        if (Ttemp != null && !string.IsNullOrEmpty(dt.Rows[i]["TicketedPointName"].ToString()))
                        {
                            dt.Rows[i]["TicketedPointName"] = Ttemp.TicketedPointId;
                        }
                        else
                        {
                            dt.Rows[i]["TicketedPointName"] = "无数据";
                        }
                        //201610220104402200  记账凭证
                        //201610220104402201  收款凭证
                        //201610220104402202  付款凭证
                        //201610220104402203  转账凭证
                        if (dt.Rows[i]["SettleReceipType"].ToString() == "收款凭证")
                        {
                            dt.Rows[i]["SettleReceipType"] = "201610220104402201";
                        }
                        else if (dt.Rows[i]["SettleReceipType"].ToString() == "付款凭证")
                        {
                            dt.Rows[i]["SettleReceipType"] = "201610220104402202";
                        }
                        else if (dt.Rows[i]["SettleReceipType"].ToString() == "记账凭证")
                        {
                            dt.Rows[i]["SettleReceipType"] = "201610220104402200";
                        }
                        else
                        {
                            dt.Rows[i]["SettleReceipType"] = "201610220104402203";
                        }
                        //摘要验证
                        //记账凭证 不做限制
                        var Stemp = settles.Where(m => m.EnterpriseId == Etemp.EnterpriseId && m.SettleSummaryName == dt.Rows[i]["ReceiptAbstractName"].ToString() && m.SettleSummaryType == dt.Rows[i]["SettleReceipType"].ToString()).FirstOrDefault();
                        if (dt.Rows[i]["SettleReceipType"].ToString() == "201610220104402200")
                        {
                            Stemp = settles.Where(m => m.EnterpriseId == Etemp.EnterpriseId && m.SettleSummaryName == dt.Rows[i]["ReceiptAbstractName"].ToString()).FirstOrDefault();
                        }
                        if (Stemp != null && !string.IsNullOrEmpty(dt.Rows[i]["ReceiptAbstractName"].ToString()))
                        {
                            dt.Rows[i]["ReceiptAbstractName"] = Stemp.SettleSummaryId;
                        }
                        else if (Stemp == null && !string.IsNullOrEmpty(dt.Rows[i]["ReceiptAbstractName"].ToString()))
                        {
                            dt.Rows[i]["ReceiptAbstractName"] = "无数据";
                        }
                        //科目验证  根据日期去选择对应的科目
                        var Atemp = accos.Where(m => m.AccoSubjectName == dt.Rows[i]["AccoSubjectName"].ToString()).FirstOrDefault();
                        if (Atemp != null && !string.IsNullOrEmpty(dt.Rows[i]["AccoSubjectName"].ToString()))
                        {
                            dt.Rows[i]["AccoSubjectName"] = Atemp.AccoSubjectId;
                        }
                        else if (Atemp == null && !string.IsNullOrEmpty(dt.Rows[i]["AccoSubjectName"].ToString()))
                        {
                            dt.Rows[i]["AccoSubjectName"] = "无数据";
                            Atemp = new AccoSubjectInfo();
                        }
                        //项目验证
                        var Ptemp = projects.Where(m => m.ProjectName == dt.Rows[i]["ProjectName"].ToString() && m.EnterpriseID == Etemp.EnterpriseId).FirstOrDefault();
                        if (Ptemp != null && !string.IsNullOrEmpty(dt.Rows[i]["ProjectName"].ToString()))
                        {
                            dt.Rows[i]["ProjectName"] = Ptemp.ProjectID;
                        }
                        else if (Ptemp == null && !string.IsNullOrEmpty(dt.Rows[i]["ProjectName"].ToString()))
                        {
                            dt.Rows[i]["ProjectName"] = "无数据";
                        }
                        //部门验证
                        var Mtemp = markets.Where(m => m.EnterpriseId == Etemp.EnterpriseId && m.MarketName == dt.Rows[i]["MarketName"].ToString()).FirstOrDefault();
                        if (Mtemp != null && !string.IsNullOrEmpty(dt.Rows[i]["MarketName"].ToString()))
                        {
                            dt.Rows[i]["MarketName"] = Mtemp.MarketId;
                        }
                        else if (Mtemp == null && !string.IsNullOrEmpty(dt.Rows[i]["MarketName"].ToString()))
                        {
                            dt.Rows[i]["MarketName"] = "无数据";
                        }
                        //员工
                        var Rtemp = persons.Where(m => m.PersonName.Equals(dt.Rows[i]["PersonName"].ToString())).FirstOrDefault();
                        if (Rtemp != null && !string.IsNullOrEmpty(dt.Rows[i]["PersonName"].ToString()))
                        {
                            dt.Rows[i]["PersonName"] = Rtemp.PersonId;
                        }
                        else if (Rtemp == null && !string.IsNullOrEmpty(dt.Rows[i]["PersonName"].ToString()))
                        {
                            dt.Rows[i]["PersonName"] = "无数据";
                        }
                        //客户/供应商
                        var CStemp = cuosups.Where(m => m.Name == dt.Rows[i]["CustomerName"].ToString()).FirstOrDefault();
                        if (CStemp != null && !string.IsNullOrEmpty(dt.Rows[i]["CustomerName"].ToString()))
                        {
                            dt.Rows[i]["CustomerName"] = CStemp.Id;
                        }
                        else if (CStemp == null && !string.IsNullOrEmpty(dt.Rows[i]["CustomerName"].ToString()))
                        {
                            dt.Rows[i]["CustomerName"] = "无数据";
                        }
                        //商品代号
                        var Protemp = products.Where(m => m.EnterpriseId == Etemp.EnterpriseId && m.ProductName == dt.Rows[i]["ProductName"].ToString()).FirstOrDefault();
                        if (Protemp != null && !string.IsNullOrEmpty(dt.Rows[i]["ProductName"].ToString()))
                        {
                            dt.Rows[i]["ProductName"] = Protemp.ProductId;
                        }
                        else if (Protemp == null && !string.IsNullOrEmpty(dt.Rows[i]["ProductName"].ToString()))
                        {
                            dt.Rows[i]["ProductName"] = "无数据";
                        }
                        //商品名称
                        var PGrotemp = productgroups.Where(m => m.ProductName == dt.Rows[i]["ProductGroupName"].ToString()).FirstOrDefault();
                        if (PGrotemp != null && !string.IsNullOrEmpty(dt.Rows[i]["ProductGroupName"].ToString()))
                        {
                            dt.Rows[i]["ProductGroupName"] = PGrotemp.ProductId;
                        }
                        else if (PGrotemp == null && !string.IsNullOrEmpty(dt.Rows[i]["ProductGroupName"].ToString()))
                        {
                            dt.Rows[i]["ProductGroupName"] = "无数据";
                        }
                        //商品分类
                        var Ctemp = classification.Where(m => m.ClassificationName == dt.Rows[i]["ClassificationName"].ToString()).FirstOrDefault();
                        if (Ctemp != null && !string.IsNullOrEmpty(dt.Rows[i]["ClassificationName"].ToString()))
                        {
                            dt.Rows[i]["ClassificationName"] = Ctemp.ClassificationID;
                        }
                        else if (Ctemp == null && !string.IsNullOrEmpty(dt.Rows[i]["ClassificationName"].ToString()))
                        {
                            dt.Rows[i]["ClassificationName"] = "无数据";
                        }
                        //资金账户
                        var ACtemp = accounts.Where(m => m.EnterpriseId == Etemp.EnterpriseId && m.AccountName == dt.Rows[i]["AccountName"].ToString()).FirstOrDefault();
                        if (ACtemp != null && !string.IsNullOrEmpty(dt.Rows[i]["AccountName"].ToString()))
                        {
                            dt.Rows[i]["AccountName"] = ACtemp.AccountId;
                        }
                        else if (ACtemp == null && !string.IsNullOrEmpty(dt.Rows[i]["AccountName"].ToString()))
                        {
                            dt.Rows[i]["AccountName"] = "无数据";
                        }
                        //结算方式
                        var PTtemp = paytypes.Where(m => m.PayTypeName == dt.Rows[i]["PaymentTypeName"].ToString()).FirstOrDefault();
                        if (PTtemp != null && !string.IsNullOrEmpty(dt.Rows[i]["PaymentTypeName"].ToString()))
                        {
                            dt.Rows[i]["PaymentTypeName"] = PTtemp.PayTypeId;
                        }
                        else if (PTtemp == null && !string.IsNullOrEmpty(dt.Rows[i]["PaymentTypeName"].ToString()))
                        {
                            dt.Rows[i]["PaymentTypeName"] = "无数据";
                        }
                        var excelObj = new SettleExcelData()
                        {
                            AccoSubjectName = dt.Rows[i]["AccoSubjectName"].ToString(),
                            EnterpriseName = dt.Rows[i]["EnterpriseName"].ToString(),
                            AccountName = dt.Rows[i]["AccountName"].ToString(),
                            AttachmentNum = dt.Rows[i]["AttachmentNum"].ToString(),
                            ClassificationName = dt.Rows[i]["ClassificationName"].ToString(),
                            Content = dt.Rows[i]["Content"].ToString(),
                            Credit = string.IsNullOrEmpty(dt.Rows[i]["Credit"].ToString()) ? "0.00" : dt.Rows[i]["Credit"].ToString(),
                            CustomerName = dt.Rows[i]["CustomerName"].ToString(),
                            DataDate = dt.Rows[i]["DataDate"].ToString(),
                            Debit = string.IsNullOrEmpty(dt.Rows[i]["Debit"].ToString()) ? "0.00" : dt.Rows[i]["Debit"].ToString(),
                            MarketName = dt.Rows[i]["MarketName"].ToString(),
                            Number = dt.Rows[i]["Number"].ToString(),
                            PaymentTypeName = dt.Rows[i]["PaymentTypeName"].ToString(),
                            PersonName = dt.Rows[i]["PersonName"].ToString(),
                            ProductGroupName = dt.Rows[i]["ProductGroupName"].ToString(),
                            ProductName = dt.Rows[i]["ProductName"].ToString(),
                            ProjectName = dt.Rows[i]["ProjectName"].ToString(),
                            ReceiptAbstractName = dt.Rows[i]["ReceiptAbstractName"].ToString(),
                            SettleReceipType = dt.Rows[i]["SettleReceipType"].ToString(),
                            TicketedPointName = dt.Rows[i]["TicketedPointName"].ToString(),
                            IsBank = Atemp.IsBank,
                            IsCash = Atemp.IsCash,
                            IsCus = Atemp.IsCus,
                            IsDept = Atemp.IsDept,
                            IsItem = Atemp.IsItem,
                            IsLorR = Atemp.IsLorR,
                            IsPerson = Atemp.IsPerson,
                            IsProject = Atemp.IsProject,
                            IsSup = Atemp.IsSup,
                            IsTorF = Atemp.IsTorF,
                        };
                        if (option.FirstOrDefault().OptionSwitch)
                        {
                            excelObj.OrganizationSortID = dt.Rows[i]["OrganizationSortID"].ToString();
                        }
                        else
                        {
                            excelObj.EnterpriseID = dt.Rows[i]["EnterpriseID"]?.ToString();
                        }
                        excelDatas.Add(excelObj);
                        dataList.Add(dt.Rows[i]["Number"].ToString() + "," + dt.Rows[i]["EnterpriseName"] + "," + dt.Rows[i]["DataDate"].ToString());
                    }
                    //根据日期验证是否结账
                    foreach (var item in dataList.GroupBy(m => m))
                    {
                        var date = item.Key.Split(',')[2];
                        DateTime date1 = new DateTime();
                        bool convertResult1 = DateTime.TryParse(date, out date1);
                        if (!convertResult1)
                        {
                            return Ok(new { status = false, code = 6, msg = date + "日期格式不对,正确格式：年-月-日" });
                        }
                        else
                        {
                            var param = new VerificationCheckoutModel { EnterpriseID = Convert.ToInt64(item.Key.Split(',')[1]), OwnerID = Convert.ToInt64(_identityService.UserId), DataDate = Convert.ToDateTime(date).ToString("yyyy-MM-dd"), AppID = 2108021645470000109 };
                            string requrl = $"{this._hostCongfiguration.QlwServiceHost}/api/{item.Key.Split(',')[1]}/{_identityService.UserId}/FM_AccoCheck/IsLockForm";
                            var res = _httpClientUtil1.PostJsonAsync<ResultModel>(requrl, param).Result;
                            if ((bool)res?.ResultState)
                            {
                                return Ok(new { status = false, code = 6, msg = date1.Year.ToString() + "年度已经有会计结账，请检查模板日期是否正确或先到月末结账取消会计结账再上传！" });
                            }
                        }
                    }
                    //数据验证
                    var Vmsg = "";
                    var count = 0;
                    foreach (var item in excelDatas.GroupBy(m => m.Number).ToList())
                    {
                        count++;
                        int counts = 0;
                        foreach (var items in item)
                        {
                            counts++;
                            if (items.EnterpriseName != item.FirstOrDefault().EnterpriseName)
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,请和该序号第一行数据（账务单位）保持一致\n";
                            }
                            if (items.DataDate != item.FirstOrDefault().DataDate)
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,请和该序号第一行数据（日期）保持一致\n";
                            }
                            if (items.TicketedPointName != item.FirstOrDefault().TicketedPointName)
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,请和该序号第一行数据（单据字）保持一致\n";
                            }
                            if (items.SettleReceipType != item.FirstOrDefault().SettleReceipType)
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,请和该序号第一行数据（凭证类别）保持一致\n";
                            }
                            if (string.IsNullOrEmpty(items.Number))
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,请填入未填写字段（序号）\n";
                            }
                            if (string.IsNullOrEmpty(items.EnterpriseName))
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,请填入未填写字段（账务单位）\n";
                            }
                            if (string.IsNullOrEmpty(items.TicketedPointName))
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,请填入未填写字段（单据字）\n";
                            }
                            if (string.IsNullOrEmpty(items.DataDate))
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,请填入未填写字段（日期）\n";
                            }
                            if (string.IsNullOrEmpty(items.SettleReceipType))
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,请填入未填写字段（凭证类别）\n";
                            }
                            if (string.IsNullOrEmpty(items.ReceiptAbstractName))
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,请填入未填写字段（摘要）\n";
                            }
                            else if (items.ReceiptAbstractName == "无数据")
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,摘要与凭证类别不符/摘要不存在\n";
                            }
                            if (string.IsNullOrEmpty(items.Content))
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,请填入未填写字段（内容）\n";
                            }
                            if (string.IsNullOrEmpty(items.AccoSubjectName))
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,请填入未填写字段（会计科目）\n";
                            }
                            //如会计科目（根据单位会计科目判断）有对应辅助核算或所属项目，辅助核算或所属项目不能为空，（若会计科目没有设定辅助项目/所属项目，则允许填写辅助项目/所属项目）
                            //若未填写，导入时提示：第几行请填入科目对应的辅助核算项目
                            //资金类科目（根据单位会计科目判断）对应的结算方式和资金账户不能为空，非资金类科目不允许填写结算方式和资金账户
                            //应该写未写，提示：第几行请填入结算方式与资金账户
                            //不应该写已写，第几行为非资金类科目，请删除结算方式与资金账户
                            else if (items.IsProject && items.ProjectName == "无数据")
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,科目有对应辅助核算,所属项目不存在\n";
                            }
                            else if (items.IsProject && string.IsNullOrEmpty(items.ProjectName))
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,科目有对应辅助核算,所属项目不能为空\n";
                            }
                            else if (items.IsPerson && items.PersonName == "无数据")
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,科目有对应辅助核算,所属员工不存在\n";
                            }
                            else if (items.IsPerson && string.IsNullOrEmpty(items.PersonName))
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,科目有对应辅助核算,所属员工不能为空\n";
                            }
                            else if ((items.IsCus || items.IsSup) && items.CustomerName == "无数据")
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,科目有对应辅助核算,所属客户/供应商不存在\n";
                            }
                            else if ((items.IsCus || items.IsSup) && string.IsNullOrEmpty(items.CustomerName))
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,科目有对应辅助核算,所属客户/供应商不能为空\n";
                            }
                            else if (items.IsDept && items.MarketName == "无数据")
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,科目有对应辅助核算,所属部门不存在\n";
                            }
                            else if (items.IsDept && string.IsNullOrEmpty(items.MarketName))
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,科目有对应辅助核算,所属部门不能为空\n";
                            }
                            //资金类科目判断
                            else if (items.IsTorF)
                            {
                                if (items.PaymentTypeName == "无数据" || string.IsNullOrEmpty(items.PaymentTypeName))
                                {
                                    Vmsg += @$"序号【{items.Number}】,第{counts}行,科目对应的结算方式不存在\n";
                                }
                                else if (items.AccountName == "无数据" || string.IsNullOrEmpty(items.AccountName))
                                {
                                    Vmsg += @$"序号【{items.Number}】,第{counts}行,科目对应的资金账户不存在\n";
                                }
                            }
                            else if (!items.IsTorF)
                            {
                                if (!string.IsNullOrEmpty(items.PaymentTypeName))
                                {
                                    Vmsg += @$"序号【{items.Number}】,第{counts}行,非资金类科目,不允许填写结算方式和资金账户\n";
                                }
                                else if (!string.IsNullOrEmpty(items.AccountName))
                                {
                                    Vmsg += @$"序号【{items.Number}】,第{counts}行,非资金类科目,不允许填写资金账户和结算方式\n";
                                }
                            }
                            if (items.EnterpriseID == "无数据")
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,所属单位有误\n";
                            }
                            if (items.OrganizationSortID == "无数据")
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,业务单元有误\n";
                            }
                            if (items.PersonName == "无数据")
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,员工不存在\n";
                            }
                            if (items.PersonName == "无数据")
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,员工不存在\n";
                            }
                            if (items.ReceiptAbstractName == "无数据")
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,摘要不存在\n";
                            }
                            if (items.AccoSubjectName == "无数据")
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,科目不存在\n";
                            }
                            if (items.ProjectName == "无数据")
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,项目不存在\n";
                            }
                            if (items.MarketName == "无数据")
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,部门不存在\n";
                            }
                            if (items.CustomerName == "无数据")
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,客户/供应商 不存在\n";
                            }
                            if (items.AccountName == "无数据")
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,资金账户 不存在\n";
                            }
                            if (items.PaymentTypeName == "无数据")
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,结算方式 不存在\n";
                            }
                            if (items.ClassificationName == "无数据")
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,商品分类不存在\n";
                            }
                            if (items.ProductName == "无数据")
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,商品代号不存在\n";
                            }
                            if (items.ProductGroupName == "无数据")
                            {
                                Vmsg += @$"序号【{items.Number}】,第{counts}行,商品名称不存在\n";
                            }
                            if (items.Debit != "0.00" && items.Credit != "0.00")
                            {
                                Vmsg += $@"序号【{items.Number}】,第{counts}行,同一分录行同时存在借贷金额\n";
                            }
                            else if (items.Debit == "0.00" && items.Credit == "0.00")
                            {
                                Vmsg += $@"序号【{items.Number}】,第{counts}行,同一分录行均未填写借/贷 金额\n";
                            }
                        }
                        if (item.Sum(m =>Math.Round(Convert.ToDecimal(m.Credit),2)) != Math.Round(item.Sum(m => Convert.ToDecimal(m.Debit)),2))
                        {
                            Vmsg += $@"序号{item.Key},借贷金额不等\n";
                        }
                    }

                    if (string.IsNullOrEmpty(Vmsg))
                    {
                        foreach (var item in excelDatas.GroupBy(m => m.Number).ToList())
                        {
                            FD_SettleReceiptNewAddCommand data = new FD_SettleReceiptNewAddCommand()
                            {
                                EnterpriseID = item.FirstOrDefault().EnterpriseName,
                                AttachmentNum = item.FirstOrDefault().AttachmentNum,
                                CreatedDate = DateTime.Now,
                                DataDate = item.FirstOrDefault().DataDate,
                                BeginDate = BeginDate,
                                EndDate = EndDate,
                                ModifiedDate = DateTime.Now,
                                SettleReceipType = item.FirstOrDefault().SettleReceipType,
                                TicketedPointID = item.FirstOrDefault().TicketedPointName,
                                Lines = new List<FD_SettleReceiptNew.FD_SettleReceiptDetailInterfacesCommand>(),
                                Remarks = "会计凭证导入数据",
                            };
                            foreach (var items in item)
                            {
                                data.Lines.Add(new FD_SettleReceiptNew.FD_SettleReceiptDetailInterfacesCommand()
                                {
                                    AccoSubjectID = items.AccoSubjectName,
                                    AccountID = items.AccountName,
                                    ClassificationID = items.ClassificationName,
                                    Content = items.Content,
                                    Credit = Convert.ToDecimal(items.Credit),
                                    Debit = Convert.ToDecimal(items.Debit),
                                    CustomerID = items.CustomerName,
                                    MarketID = items.MarketName,
                                    PaymentTypeID = items.PaymentTypeName,
                                    PersonID = items.PersonName,
                                    ProductGroupID = items.ProductGroupName,
                                    ProductID = items.ProductName,
                                    ProjectID = items.ProjectName,
                                    RowNum = data.Lines.Count + 1,
                                    ReceiptAbstractID = items.ReceiptAbstractName,
                                    LorR = Convert.ToDecimal(items.Credit) != 0 ? false : true,
                                    EnterpriseID = items.EnterpriseID,
                                    OrganizationSortID = items.OrganizationSortID
                                });
                            }
                            var result = _mediator.Send(data, HttpContext.RequestAborted).Result;
                            if (result.code != ErrorCode.Success.GetIntValue())
                            {
                                Vmsg += @$"序号【{item.FirstOrDefault().Number}】,导入失败。\n";
                            }
                        }
                        if (!string.IsNullOrEmpty(Vmsg))
                        {
                            return Ok(new { code = ErrorCode.RequestArgumentError.GetIntValue(), status = false, msg = $@"部分导入失败:{Vmsg}" });
                        }
                    }
                    else
                    {
                        return Ok(new { code = ErrorCode.ServerBusy.GetIntValue(), status = false, msg = "校验未通过：" + Vmsg.Replace("\\n", "\n") });
                    }
                    #endregion
                    return Ok(new { code = ErrorCode.Success.GetIntValue(), status = true, msg = $@"导入成功，共生成{count}张会计凭证。" });
                }

            }
            catch (Exception e)
            {
                return Ok(new { code = ErrorCode.RequestArgumentError.GetIntValue(), status = false, msg = "导入失败", erro = e });
            }
        }

        [HttpPost]
        [Route("VoucherExport")]
        public async Task<ActionResult> VoucherExport(VoucherParam model)
        {
            //本地调试用
            if (model == null)
            {
                //model = new VoucherParam() { BeginDate = "2009-05-17",EndDate = "2023-05-23",EnterpriseIds = "634086739144001721,1251802" };
                model = new VoucherParam();
            }
            //获取凭证数据
            var Voucher = _provider.GetVouchersExport(model);
            if (Voucher == null)
            {
                return new EmptyResult();
            }
            if (Voucher.Count > 0)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                // 创建 Excel 文件
                using (var package = new ExcelPackage())
                {
                    foreach (var item in Voucher.GroupBy(m => m.EnterpriseID))
                    {
                        //单位命名 sheet
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(item.FirstOrDefault().EnterpriseName);
                        worksheet.DefaultColWidth = 15;
                        worksheet.Cells[1, 1, 1, 22].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[1, 1, 1, 22].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255,217,217,217));
                        #region 标头
                        worksheet.Cells[1, 1].Value = "日期";
                        worksheet.Cells[1, 2].Value = "凭证类别";
                        worksheet.Cells[1, 3].Value = "单据字号";
                        worksheet.Cells[1, 4].Value = "摘要";
                        worksheet.Cells[1, 5].Value = "会计科目";
                        worksheet.Cells[1, 6].Value = "员工";
                        worksheet.Cells[1, 7].Value = "部门";
                        worksheet.Cells[1, 8].Value = "客/商";
                        worksheet.Cells[1, 9].Value = "项目";
                        worksheet.Cells[1, 10].Value = "商品";
                        worksheet.Cells[1, 11].Value = "商品名称";
                        worksheet.Cells[1, 12].Value = "商品分类";
                        worksheet.Cells[1, 13].Value = "借方金额";
                        worksheet.Cells[1, 14].Value = "贷方金额";
                        worksheet.Cells[1, 15].Value = "资金账户";
                        worksheet.Cells[1, 16].Value = "结算方式";
                        worksheet.Cells[1, 17].Value = "所属单位/业务单元";
                        worksheet.Cells[1, 18].Value = "现流项目";
                        worksheet.Cells[1, 19].Value = "附单据张数";
                        worksheet.Cells[1, 20].Value = "备注";
                        worksheet.Cells[1, 21].Value = "制单人";
                        worksheet.Cells[1, 22].Value = "审核人";
                        #endregion
                        int row = 2;
                        foreach (var items in item)
                        {
                            //i=2 不需要前两列的数据
                            for (int i = 3; i < items.GetType().GetProperties().Count(); i++)
                            {
                                worksheet.Cells[row, i-2].Value = items.GetType().GetProperty(items.GetType().GetProperties()[i].Name).GetValue(items);
                            }
                            row++;
                        }
                    }
                    // 生成 Excel 文件流
                    var stream = new MemoryStream(package.GetAsByteArray());

                    // 设置响应头信息，告诉浏览器下载文件
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //命名excel名称  防止ASCII 报错
                    var contentDisposition = new ContentDisposition
                    {
                        FileName = $"会计凭证{model.BeginDate}-{model.EndDate}.xlsx",
                        Inline = false
                    };
                    Response.Headers.Add("content-disposition", contentDisposition.ToString());

                    // 将文件流输出到 Response 中，供前端下载
                    await stream.CopyToAsync(Response.Body);
                    stream.Close();
                }
            }
            // 返回空内容，避免触发视图渲染
            return new EmptyResult();
        }
        [NonAction]
        public List<dynamic> GetMyData1()
        {
            List<dynamic> dataList1 = new List<dynamic>();
            // 假设MyData类有Name和Age属性
            dataList1.Add(new { Name = "John", Age = 25 });
            dataList1.Add(new { Name = "Alice", Age = 30 });
            dataList1.Add(new { Name = "Bob", Age = 35 });
            // 添加更多数据...

            return dataList1;
        }

        [NonAction]
        public List<dynamic> GetMyData2()
        {
            List<dynamic> dataList2 = new List<dynamic>();
            // 假设MyData类有Name和Age属性
            dataList2.Add(new { Name = "Sarah", Age = 28 });
            dataList2.Add(new { Name = "Michael", Age = 32 });
            dataList2.Add(new { Name = "Emily", Age = 27 });
            // 添加更多数据...

            return dataList2;
        }
        [HttpPost]
        [Route("GetKMData")]
        public Result GetKMData([FromBody] SearchKM search)
        {
            var data = new Result();
            try
            {
                var value = _provider.GetKMAmount(search);
                data.code = ErrorCode.Success.GetIntValue();
                data.msg = "成功";
                data.data = value;
            }
            catch (Exception e)
            {
                data.msg = "失败";
                data.code = ErrorCode.RequestArgumentError.GetIntValue();
                data.errors = new List<ErrorRow>();
                data.errors.Add(new ErrorRow() { columns = new List<ErrorColumn>() });
                data.errors[0].columns.Add(new ErrorColumn() { value = e });
            }
            return data;
        }
        [HttpPost]
        [Route("GetOptionInfos")]
        public Result GetOptionInfos(string optionId,string groupId)
        {
            var data = new Result();
            try
            {
                var value = _provider.GetOptionInfos(optionId,groupId);
                data.code = ErrorCode.Success.GetIntValue();
                data.msg = "成功";
                data.data = value;
            }
            catch (Exception e)
            {
                data.msg = "失败";
                data.code = ErrorCode.RequestArgumentError.GetIntValue();
                data.errors = new List<ErrorRow>();
                data.errors.Add(new ErrorRow() { columns = new List<ErrorColumn>() });
                data.errors[0].columns.Add(new ErrorColumn() { value = e });
            }
            return data;
        }
        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("{key}")]
        [PermissionAuthorize(Permission.Retrieve)]
        public async Task<Result> GetDetail(long key)
        {
            var result = new Result();
            var data = await _provider.GetSingleDataAsync(key);            
            result.data = data;
            result.code = ErrorCode.Success.GetIntValue();
            return result;
        }

        /// <summary>
        /// 查询 按流水号查询，nums.NumericalOrder = '123,3456,2345'
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetDataList")]
        [PermissionAuthorize(Permission.Retrieve)]
        public Result GetDataList(FD_SettleReceipt.FD_SettleReceiptCommand nums)
        {
            var result = new Result();
            var data = _provider.GetDataList("","","", nums.NumericalOrder).ToList();
            if (data?.Count > 0)
            {
                foreach(var item in data)
                {
                    var details = _provider.GetDetaiDatasAsync(long.Parse( item.NumericalOrder)).Result;
                    item.Lines = details;
                }
            }
            result.data = data;
            result.code = ErrorCode.Success.GetIntValue();
            return result;
        }
        [HttpGet]
        [Route("GetVoucherHandleList")]
        [EnableQuery(MaxNodeCount = 10000)]
        [PermissionAuthorize(Permission.Retrieve)]
        public IQueryable<VoucherHandleInfoEntity> GetVoucherHandleList(ODataQueryOptions<RefundList> odataqueryoptions, Uri uri)
        {
            var entes = _fmBaseCommon.GetAuthorEnterpise(_identityService.EnterpriseId,_identityService.UserId);
            return _provider.GetVoucherHandleInfoEntities(entes).ToList().AsQueryable();
        }
        /// <summary>
        /// 凭证处理，生成凭证
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GenerateVoucher")]
        public Result GenerateVoucher(VoucherCommand request)
        {
            return _mediator.Send(request, HttpContext.RequestAborted).Result;
        }
    }
}
