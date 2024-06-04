using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_IndividualIdentification;
using FinanceManagement.Common;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Architecture.Seedwork.Security;
using Microsoft.AspNet.OData.Query;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Serilog.Core;
using Microsoft.Extensions.Logging;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Newtonsoft.Json;
using Aspose.Cells;
using System.Data;
using System.IO;
using System.Collections.Concurrent;
using Org.BouncyCastle.Ocsp;

namespace FinanceManagement.ApiHost.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_IndividualIdentificationController : ControllerBase
    {
        IMediator _mediator;
        FD_IndividualIdentificationODataProvider _provider;
        private readonly ILogger<FD_IndividualIdentificationController> _logger;
        private IIdentityService _identityService;
        AgingDataUtil _agingDataUtil;
        IFD_IndividualIdentificationExtRepository _extRepository;
        RptAgingReclassificationUtil _agingReclassificationUtil;
        private IFD_IndividualIdentificationRepository _indiRepository;
        IFD_BadDebtProvisionRepository _iProvisionRepository;
        private readonly string _intervalType = "1803300944570000101";//账龄区间类型-年
        private FMBaseCommon _baseUnit;
        FD_BaddebtGroupSettingODataProvider _baddebtGroupSettingODataProvider;

        public FD_IndividualIdentificationController(IMediator mediator, FD_IndividualIdentificationODataProvider provider, ILogger<FD_IndividualIdentificationController> logger, IIdentityService identityService,
            AgingDataUtil agingDataUtil,
            IFD_BadDebtProvisionDetailRepository iProvisionDetailRepository,
            RptAgingReclassificationUtil agingReclassificationUtil,
            IFD_IndividualIdentificationExtRepository extRepository,
            FD_BaddebtGroupSettingODataProvider baddebtGroupSettingODataProvider,
            IFD_IndividualIdentificationRepository indiRepository,
            FMBaseCommon baseUnit
            )
        {
            _mediator = mediator;
            _provider = provider;
            _logger = logger;
            _baseUnit = baseUnit;
            _identityService = identityService;
            _agingDataUtil = agingDataUtil;
            _agingReclassificationUtil = agingReclassificationUtil;
            _extRepository = extRepository;
            _indiRepository= indiRepository;
            _baddebtGroupSettingODataProvider = baddebtGroupSettingODataProvider;
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
            data?.Lines?.ForEach(o =>
            {
                o.AgingList = _provider.GetExtData(o.NumericalOrderDetail)?.Result;
            });
            result.code = ErrorCode.Success.GetIntValue();
            result.data = data;
            return result;
        }

        //增加
        [HttpPost]
        [AllowAnonymous]
        [PermissionAuthorize(Permission.Create)]
        public async Task<Result> Add([FromBody] FD_IndividualIdentificationAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        [PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FD_IndividualIdentificationDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        [PermissionAuthorize(Permission.Update)]
        public async Task<Result> Modify([FromBody] FD_IndividualIdentificationModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        /// <summary>
        /// 获取账龄设置
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="intervalType"></param>
        /// <returns></returns>
        [HttpGet("GetAgingintervalList")]
        public Result GetAgingintervalList(string groupId, string intervalType)
        {
            var result = new Result();
            if (string.IsNullOrEmpty(groupId))
            {
                groupId = _identityService.GroupId;
            }
            if (string.IsNullOrEmpty(intervalType))
            {
                intervalType = "1803300944570000101";//默认按年
            }
            var data = _provider.GetAgingintervals(groupId, intervalType);
            result.data = data;
            result.code = ErrorCode.Success.GetIntValue();
            return result;
        }
        #region 重分类数据
        /// <summary>
        /// 获取重分类数据
        /// </summary>
        /// <param name="model">BusiType 业务类型（0：应收账款 1：其他应收款）</param>
        /// 获取认定类型科目对应设置；过滤非空科目，循环查询账龄重分类；过滤余额为0的；
        /// <returns></returns>
        [HttpPost("GetReclassificationData")]
        public Result GetReclassificationData(AgingReclass model)
        {
            var result = new Result();
            var resultList = new List<FD_IndividualIdentificationDetailODataEntity>();
            try
            {
                if (string.IsNullOrEmpty(model.DataDate))
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "DataDate不能为空";
                    return result;
                }
                
                if (string.IsNullOrEmpty(model.EnterpriseID)) { model.EnterpriseID = _identityService.EnterpriseId; }
                //获取认定类型科目对应设置 过滤非空科目
                var typeSubjectList =_baddebtGroupSettingODataProvider.GetIdentificationTypeSubjectList(new FD_BaddebtGroupSettingODataEntity() {DataDate=model.DataDate,EnterpriseID= _identityService.GroupId, IsFiltersubject=true },0,model.BusiType,model.EnterpriseID)?.Result;
                //_logger.LogInformation(" FD_IndividualIdentification/typeSubjectList：" + JsonConvert.SerializeObject(_identityService));
                if (typeSubjectList==null|| typeSubjectList.Count == 0)
                {
                    result.msg = "未获取到认定类型科目对应信息";
                    result.code=ErrorCode.NoContent.GetIntValue();
                    return result;
                }
                //查询账龄重分类
               
                var list = GetReclassificationRequest(model, typeSubjectList,"1");
                if (list?.Count > 0)
                {
                    resultList.AddRange(list);
                }
                if (model.BusiType == 1)
                {
                    var list2 = GetReclassificationRequest(model, typeSubjectList, "2");
                    if (list2?.Count > 0)
                    {
                        resultList.AddRange(list2);
                    }
                }
                result.data = resultList;
                result.code = ErrorCode.Success.GetIntValue();
            }
            catch (Exception ex)
            {
                _logger.LogError(" FD_IndividualIdentification/GetReclassificationData:参数：" + JsonConvert.SerializeObject(model) + "\n 异常" + ex.ToString());
                result.msg = "获取重分类数据异常";
                result.code = ErrorCode.ServerBusy.GetIntValue();
            }
            return result;
        }
        private List<FD_IndividualIdentificationDetailODataEntity> GetReclassificationRequest(AgingReclass model, List<FD_IdentificationTypeSubjectODataEntity> subjectList, string CustomerType="1")
        {
            try
            {
                var subject = subjectList[0];
                if (subject.Rank == null) return new List<FD_IndividualIdentificationDetailODataEntity>();
                //var isPerson = subjectList.Where(p => p.IsPerson)?.Count()>0;
                var request = new RptAgingReclassificationRequest();
                //request.AccountingSubjectsID = subject.AccoSubjectCode;
                //request.FilterSubject = true;
                //request.Boid = _identityService.UserId;
                request.CustomerType = CustomerType;
                request.Enddate = model.DataDate;               
                request.SubjectLevel = int.Parse(subject.Rank.ToString());               
                request.EnteID = model.EnterpriseID;                
                request.GroupID = _identityService.GroupId;
                request.EnterpriseList = $"{ request.EnteID}";
                request.MenuParttern = "0";//单位
                request.IntervalType = model.IntervalType;
                request.OwnEntes = new List<string>() { request.EnteID };
                request.CanWatchEntes = new List<string>() { request.EnteID };
                var response = _agingReclassificationUtil.GetData(request)?.Result;
                var retdata = response?.Where(o => o.AdjustAmount > 0)?.ToList();
                var list = new List<FD_IndividualIdentificationDetailODataEntity>();
                if (retdata==null||retdata.Count == 0) { return list; }
                foreach(var item in subjectList)
                {
                    var data =retdata?.Where(p => p.AccoSubjectCode == item.AccoSubjectCode);
                    var itemList = data?.Select(o => new FD_IndividualIdentificationDetailODataEntity()
                    {
                        BusinessType = request.CustomerType == "1" ? "201611160104402101" : "201611160104402103",//客户:员工
                        BusinessTypeName = request.CustomerType == "1" ? "客户" : "员工",
                        Amount = o.AdjustAmount,//重分类后金额
                        AgingList = o.AgingintervalDatas?.OrderBy(p => p.Serial)?.Select(a => new FD_IndividualIdentificationExtODataEntity { Amount = a.Amount, Name = a.Name, BusiType = 0 })?.ToList(),
                        CurrentUnit = o.SummaryType1,
                        CurrentUnitName = o.SummaryType1Name,
                        BusiType = model.BusiType,
                        IdentificationType = item.TypeID,
                        IdentificationTypeName = item.TypeName
                    }).ToList();
                    list.AddRange(itemList);
                }
                
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(" FD_IndividualIdentification/GetReclassificationRequest:参数：" + JsonConvert.SerializeObject(model) + "\n 异常" + ex.ToString());
                return new List<FD_IndividualIdentificationDetailODataEntity>();
            }
        }
      
        #region 根据 往来类型+往来单位+一级科目取账龄重分类数据
        [HttpPost("GetReclassByCon")]
        public Result GetReclassByCon(AgingReclass model)
        {
            var result = new Result();
            var resultList = new List<FD_IndividualIdentificationDetailODataEntity>();
            try
            {
                if (string.IsNullOrEmpty(model.DataDate))
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "DataDate不能为空";
                    return result;
                }
                if (string.IsNullOrEmpty(model.BusinessType))
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "BusinessType不能为空";
                    return result;
                }
                if (string.IsNullOrEmpty(model.CurrentUnit))
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "CurrentUnit不能为空";
                    return result;
                }
                var CustomerType = "1";
                var subjectList = new List<FD_IdentificationTypeSubjectODataEntity>();
                var subject =new FD_IdentificationTypeSubjectODataEntity() { Rank = 1 } ;
                CustomerType = model.BusinessType == "201611160104402103" ? "2" : "1";
                if (model.BusiType == 0)
                {
                    subject.AccoSubjectCode = "1122";//应收账款                   
                    
                }
                else if (model.BusiType == 1)
                {
                    subject.AccoSubjectCode = "1221";//其他应收款
                }
                else
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "BusiType不支持";
                    return result;
                }
                subjectList.Add(subject);
                var list = GetReclassificationRequest(model, subjectList, CustomerType);
                if (list?.Count > 0)
                {
                    list = list?.Where(p => p.CurrentUnit == model.CurrentUnit)?.ToList();
                    if (list?.Count > 0)resultList.AddRange(list);
                }
                result.data = resultList;
                result.code = ErrorCode.Success.GetIntValue();
            }
            catch (Exception ex)
            {
                _logger.LogError(" FD_IndividualIdentification/GetReclassByCon:参数：" + JsonConvert.SerializeObject(model) + "\n 异常" + ex.ToString());
                result.msg = "页面获取重分类数据异常";
                result.code = ErrorCode.ServerBusy.GetIntValue();
            }
            return result;
        }

        [HttpPost("GetReclassByConList")]
        public Result GetReclassByConList(List<AgingReclass> reqlist)
        {
            var result = new Result();
            var resultList = new List<FD_IndividualIdentificationDetailODataEntity>();
            try
            {
                if(reqlist == null|| reqlist.Count == 0)
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "参数不能为空";
                    return result;
                }
                var busiTypeList = reqlist.Where(x => x.BusiType!=0&&x.BusiType!=1).ToList();
                if (busiTypeList?.Count > 0)
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "BusiType不支持";
                    return result;
                }
                var firstmodel = reqlist.FirstOrDefault();
                if (string.IsNullOrEmpty(firstmodel.DataDate))
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "DataDate不能为空";
                    return result;
                }
                var bList=reqlist.Where(x =>string.IsNullOrEmpty(x.BusinessType)).ToList();
                if(bList?.Count > 0)
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "BusinessType不能为空";
                    return result;
                }
                var cList = reqlist.Where(x => string.IsNullOrEmpty(x.CurrentUnit)).ToList();
                if (cList?.Count > 0)
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "CurrentUnit不能为空";
                    return result;
                }
                
                Parallel.ForEach(reqlist, new ParallelOptions { MaxDegreeOfParallelism = 8 }, a =>
                {
                    var itemResult = GetReclassData(a);
                    if (itemResult != null)
                    {
                        resultList.Add(itemResult);
                    }
                });
                result.data = resultList;
                result.code = ErrorCode.Success.GetIntValue();
            }
            catch (Exception ex)
            {
                _logger.LogError(" FD_IndividualIdentification/GetReclassByConList:参数：" + JsonConvert.SerializeObject(reqlist) + "\n 异常" + ex.ToString());
                result.msg = "页面获取重分类数据异常";
                result.code = ErrorCode.ServerBusy.GetIntValue();
            }
            return result;
        }

        private FD_IndividualIdentificationDetailODataEntity GetReclassData(AgingReclass model)
        {
            var CustomerType = "1";
            var subjectList = new List<FD_IdentificationTypeSubjectODataEntity>();
            var subject = new FD_IdentificationTypeSubjectODataEntity() { Rank = 1 };
            CustomerType = model.BusinessType == "201611160104402103" ? "2" : "1";
            if (model.BusiType == 0)
            {
                subject.AccoSubjectCode = "1122";//应收账款                   

            }
            else if (model.BusiType == 1)
            {
                subject.AccoSubjectCode = "1221";//其他应收款
            }
            subjectList.Add(subject);
            var list = GetReclassificationRequest(model, subjectList, CustomerType);
            if (list?.Count > 0)
            {
                var filter = list?.Where(p => p.CurrentUnit == model.CurrentUnit)?.FirstOrDefault();//?.ToList();
                return filter;
            }
            return null;
        }
        #endregion
        #endregion
        /// <summary>
        /// 判断增加/删除的日期是否有效
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("ValidDate")]
        public Result ValidDate([FromBody] FD_IndividualIdentificationAddCommand request)
        {
            var result = new Result();
            var provision = _iProvisionRepository.GetLastest(_identityService.EnterpriseId)?.Result;
            if (provision != null)
            {
                if (DateTime.Compare(Convert.ToDateTime(request.DataDate), provision.DataDate) < 0)
                {
                    result.data = false;
                    return result;
                }
            }
            result.data = true;
            return result;
        }

        #region excel文件上传导入
        ///// <summary>
        ///// 事件上传
        ///// </summary>
        ///// <param name="file">文件信息</param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("Upload")]
        //[DisableRequestSizeLimit]
        //public async Task<IActionResult> Upload(IFormFile file)
        //{
        //    try
        //    {
        //        if (file == null)
        //        {
        //            return Ok(new { code = "201", msg = "请上传文件" });
        //        }
        //        var fileExtension = Path.GetExtension(file.FileName);
        //        if (fileExtension == null)
        //        {
        //            return Ok(new { code = "201", msg = "文件无后缀信息" });
        //        }
        //        long length = file.Length;
        //        if (length > 1024 * 1024 * 300)
        //        {
        //            return Ok(new { code = "201", msg = "上传文件不能超过300M" });
        //        }

        //        using (Stream fs = file.OpenReadStream())
        //        {
        //            Workbook workbook = new Workbook(fs);
        //            if(workbook==null||workbook.Worksheets == null|| workbook.Worksheets.Count < 2)
        //            {
        //                return Ok(new { code = "201", msg = "请检查模板是否正确" });
        //            }
        //            var groupId = _identityService.GroupId;

        //            //获取账龄区间设置
        //            var agIntervalsList = _provider.GetAgingintervals(groupId, _intervalType)?.ToList();
        //            if (agIntervalsList == null || agIntervalsList.Count == 0)
        //            {
        //                return Ok(new { code = "201", msg = "未查询到账龄区间设置" });
        //            }
        //            #region 表头
        //            Worksheet firstSheet = workbook.Worksheets[0];
        //            Cells firstSheetCells = firstSheet.Cells;
        //            DataTable dtHead = firstSheetCells.ExportDataTable(0, 0, 1, 6, true);//第一行前三列

        //            var date = dtHead.Columns[1].ColumnName;
        //            DateTime date1 = new DateTime();
        //            bool convertResult1 = DateTime.TryParse(date, out date1);
        //            if (!convertResult1)
        //            {
        //                return Ok(new { code = "201", msg = "日期格式不对,正确格式：年-月-日" }); 
        //            }
        //            var isAdd = false;
        //            var NumericalOrder = dtHead.Columns[5].ColumnName;
        //            FD_IndividualIdentificationCommand head=new FD_IndividualIdentificationCommand();
        //            if (string.IsNullOrEmpty(NumericalOrder.Trim()))
        //            {
        //                isAdd = true;
        //                var ticketedPoint = dtHead.Columns[3].ColumnName;
        //                if (string.IsNullOrEmpty(ticketedPoint))
        //                {
        //                    return Ok(new { code = "201", msg = "新增单据字不能为空" });
        //                }
        //                var ticketedPointResult = await _baseUnit.GetSymbol(_identityService.EnterpriseId);
        //                if (ticketedPointResult == null || !ticketedPointResult.ResultState)
        //                {
        //                    return Ok(new { code = "201", msg = "未获取到单据字信息" });
        //                }
        //                var ticketedPointList = ticketedPointResult.Data;
        //                var ticketedPointFilterList = ticketedPointList.Where(_ => _.TicketedPointName == ticketedPoint);
        //                if (!ticketedPointFilterList.Any())
        //                {
        //                    return Ok(new { code = "201", msg = "单据字不存在" });
        //                }
        //                head.TicketedPointID= ticketedPointFilterList.FirstOrDefault().TicketedPointID;
        //                head.DataDate = date1;
        //            }
        //            else
        //            {
        //                long tnumericalOrder = 0;
        //                var numFlag =long.TryParse(NumericalOrder,out tnumericalOrder);
        //                if(!numFlag )
        //                {
        //                    return Ok(new { code = "201", msg = "流水号为纯数字" });
        //                }
        //                //查询详情
        //                var headODataEntity=await _provider.GetDataAsync(tnumericalOrder);
        //                //head =await _indiRepository.GetHeadAsync(NumericalOrder);
        //                if (headODataEntity == null)
        //                {
        //                    return Ok(new { code = "201", msg = "未查询到该流水号数据，请确认流水号是否正确" });
        //                }
        //                head.NumericalOrder = headODataEntity.NumericalOrder;
        //                head.DataDate =DateTime.Parse( headODataEntity.DataDate);
        //                head.TicketedPointID = headODataEntity.TicketedPointID;
        //                head.Remarks = headODataEntity.Remarks;
        //            }
        //            #endregion

        //            //Dictionary<string, string> columHeadDic = new Dictionary<string, string>()
        //            //{
        //            //    {"日期","DataDate"},
        //            //    {"单据字","TicketedPointID"},
        //            //    {"流水号","NumericalOrder"}
        //            //};
        //            for (int t = 0; t < 2; t++)
        //            {
        //                Worksheet sheet = workbook.Worksheets[t];
        //                Cells cells = sheet.Cells;
        //                //DataTable dtHead = cells.ExportDataTable(0, 0, 1, 6, true);//第一行前三列
        //                DataTable dt = cells.ExportDataTable(1, 0, cells.MaxDataRow+1, cells.MaxDataColumn + 1, true);

        //                //序号/ 计提类型/往来类型/往来单位/期末余额/账龄区间/坏账准备
        //                Dictionary<string, string> columDic = new Dictionary<string, string>()
        //                {
        //                    {"序号","Number"},
        //                    {"计提类型","IdentificationType"},
        //                    {"往来类型","BusinessType"},
        //                    {"往来单位","CurrentUnit"},
        //                    {"期末余额","Amount"},
        //                    //{"账龄区间","AgingList"},
        //                    //{"坏账准备","BadAgingList"}
        //                };
        //                foreach(var item in agIntervalsList)
        //                {
        //                    columDic.Add(item.Name, item.Name);
        //                }
        //                foreach (var item in agIntervalsList)
        //                {
        //                    columDic.Add(item.Name, "Bad"+item.Name);
        //                }
        //                var mergeList = cells.MergedCells;
        //                if (mergeList == null || mergeList.Count == 0)
        //                {
        //                    return Ok(new { code = "201", msg = "请检查模板是否正确！" });
        //                }

        //                if (dt.Columns != null && dt.Columns.Count == columDic.Count)//(columDic.Count+ agIntervalsList.Count*2-2))
        //                {
        //                    string msg = "";
        //                    List<string> ct = new List<string>(columDic.Keys);
        //                    for (int i = 0; i < columDic.Count; i++)
        //                    {
        //                        if (columDic.ContainsKey(dt.Columns[i].ColumnName))
        //                        {
        //                            dt.Columns[i].ColumnName = columDic[dt.Columns[i].ColumnName];
        //                        }
        //                        else
        //                        {
        //                            msg += "模板中列名[" + dt.Columns[i].ColumnName + "]与单据中的列名[" + ct[i] + "]不一致,请修改后再导入";
        //                        }
        //                    }
        //                    if (!string.IsNullOrEmpty(msg))
        //                    {
        //                        return Ok(new { code = "201", status = false, msg = msg });
        //                    }
        //                }
        //                else
        //                {
        //                    return Ok(new { code = "201", status = false, msg = "文件列数不匹配" });
        //                }
        //            }
        //            return Ok(new { code = ErrorCode.Success.GetIntValue(), status = true, msg = $@"导入成功。" });
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        return Ok(new { code = ErrorCode.RequestArgumentError.GetIntValue(), status = false, msg = "导入失败", erro = e });
        //    }
        //}
        #endregion

        #region 最新个别认定
        //[HttpGet("GetDataByEnterId")]
        //public FD_IndividualIdentificationODataEntity GetDataByEnterId(long enterpriseId, string date, string customerId)
        //{
        //    return _provider.GetDataByEnterId(enterpriseId, date, customerId);
        //}
        #endregion
    }
}
