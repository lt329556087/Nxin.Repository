using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using Aspose.Cells.Drawing;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_SettleReceiptInterface;
using FinanceManagement.ApiHost.Controllers.SettleReceiptBalance;
using FinanceManagement.Common;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Drawing;
using System.Net.Mime;
using OfficeOpenXml.Style;

namespace FinanceManagement.ApiHost.Controllers
{
    /// <summary>
    /// 会计辅助账
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RptAssistRecordController : ControllerBase
    {
        RptAssistRecordODataProvider _prodiver;
        private IIdentityService _identityService;
        private FinanceTradeUtil _financeTradeUtil;
        private readonly string AppID = "1612121635080000101";//老会计辅助账appid
        private FMBaseCommon _fmbaseCommon;
        private readonly ILogger<RptAssistRecordController> _logger;
        private HostConfiguration _hostCongfiguration;

        public RptAssistRecordController(RptAssistRecordODataProvider prodiver, FinanceTradeUtil financeTradeUtil, IIdentityService identityService,FMBaseCommon fmbaseCommon, ILogger<RptAssistRecordController> logger, HostConfiguration hostCongfiguration)
        {
            _prodiver = prodiver;
            _logger = logger;
            _financeTradeUtil = financeTradeUtil;
            _identityService = identityService;
            _fmbaseCommon = fmbaseCommon;
            _hostCongfiguration = hostCongfiguration;
            GetRedisInstance();
        }

        #region 列表查询
        private bool isReturn=false;
        [HttpPost]
        [Route("GetDataReport")]
        public async Task<GatewayResultModel> GetDataReport(RptAssistRecordRequest req)
        {
            var result = new GatewayResultModel();
            isReturn = false;
            #region 校验参数信息
            result =await ValidData(req);
            if (!string.IsNullOrEmpty(result.msg))
            {
                return result;
            }
            if(isReturn) { return result; }

            #endregion            
            if (req.ReportType == 1)
            {
                string rediskey = $"KJFZZ:-GroupId:{_identityService.GroupId}-Userid={_identityService.UserId}";
                RedisHelper.Set(rediskey,req.SummaryTypeList);
                return _prodiver.GetSummaryData(req);
            }
            else
            {
                return _prodiver.GetDetailData(req);
            }
        }
        public async Task<GatewayResultModel> ValidData(RptAssistRecordRequest req)
        {
            var result = new GatewayResultModel();
            #region 校验参数信息
            var validationMsg = req.CheckModel();
            if (validationMsg.IsNotNullOrEmpty())
            {
                isReturn = true;
                result.msg = validationMsg;
                return result;
            }
            //权限单位
            var perParam = new PermissionEnter { Bo_ID = _identityService.UserId, EnterpriseID = _identityService.EnterpriseId, MenuID = AppID };
            var perList = await _financeTradeUtil.GetPermissionMenuEnter(perParam);
            if (perList == null || perList.Count == 0)
            {
                isReturn = true;
                return result;
            }
            var permidlist = perList.Select(p => p.EnterpriseID).ToList();
            req.EnableEnterList = permidlist;
            if (req.EnterpriseList?.Count > 0)
            {
                req.EnableEnterList = req.EnterpriseList.Intersect(permidlist).ToList();
            }
            if (req.EnableEnterList.Count == 0)
            {
                isReturn = true;
                return result;
            }
            if (string.IsNullOrEmpty(req.GroupID))
            {
                req.GroupID = _identityService.GroupId;
            }
            #endregion
            return result;
        }
        #endregion

        #region 导出
        [HttpPost]
        [Route("Export")]
        public async Task<ActionResult> Export(RptAssistRecordRequest req)
        {
            var result = await ValidData(req);
            if (!string.IsNullOrEmpty(result.msg))
            {
                return new EmptyResult();
            }
            //获取数据
            if (!isReturn) 
            {

                if (req.ReportType == 1)
                {
                    var list= _prodiver.RequestSummaryList(req);
                    if (list.Count > 0)
                    {
                        return await ExportSummary(list,req);
                    }
                }
                else
                {
                    var list= _prodiver.RequestDetailList(req);
                    if (list.Count > 0)
                    {
                        //增加合计行
                        var sumDebit=list.Sum(p=>p.Debit);
                        var sumCredit= list.Sum(p => p.Credit);
                        list.Add(new AssistRecordResultEntity { EnterpriseName = "合计", Debit = sumDebit, Credit = sumCredit });
                        return await ExportDetail(list);
                    }
                }
            }            
            
            // 返回空内容，避免触发视图渲染
            return new EmptyResult();
        }
        private async Task<EmptyResult> ExportSummary(List<AssistRecordSummaryResultEntity> list, RptAssistRecordRequest req)
        {
           
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // 创建 Excel 文件
            using (var package = new ExcelPackage())
            {
                //单位命名 sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet1");

                var columnBindings = new Dictionary<string, string>();
                SetExcelWorksheetColumnsSummary(worksheet, req);
                int row = 2;
                var length = 4;//总共三列数据 不需要前两列
                var modeltype = list.FirstOrDefault().GetType();
                var modelProperties = modeltype.GetProperties();
                
                foreach (var item in list)
                {                    
                    try
                    {
                        //i=1 不需要第一列的数据
                        for (int i = 1; i < length; i++)
                        {                            
                            var summaryCount = item.SummaryTypeNameList.Count;
                            int col = summaryCount;
                            if (i==3)//汇总方式
                            {                                
                                for (var j=0;j< summaryCount; j++)
                                {
                                    worksheet.Cells[row, j+1].Value = item.SummaryTypeNameList[j];
                                }
                            }
                            else
                            {
                                worksheet.Cells[row, col+i].Value = modeltype.GetProperty(modelProperties[i].Name).GetValue(item);
                            }                            
                        }
                    }
                    catch
                    {
                    }

                    row++;
                }
                // 生成 Excel 文件流
                var stream = new MemoryStream(package.GetAsByteArray());

                // 设置响应头信息，告诉浏览器下载文件
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //命名excel名称  防止ASCII 报错

                var contentDisposition = new ContentDisposition
                {
                    FileName = $"会计辅助账汇总.xlsx",
                    Inline = false
                };
                Response.Headers.Add("content-disposition", contentDisposition.ToString());

                // 将文件流输出到 Response 中，供前端下载
                await stream.CopyToAsync(Response.Body);
                stream.Close();
            }
            return new EmptyResult();
        }
        private async Task<EmptyResult> ExportDetail(List<AssistRecordResultEntity> list)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //系统选项
            var optionValue = "0";
            // 创建 Excel 文件
            using (var package = new ExcelPackage())
            {
                //单位命名 sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet1");

                var columnBindings = new Dictionary<string, string>();
                SetExcelWorksheetColumnsDetail(worksheet, optionValue);
                int row = 3;
                bool isenter = true;
                var length = 18;//多两列单据字号和业务单元// list.FirstOrDefault().GetType().GetProperties().Count();
                foreach (var items in list)
                {
                    var modeltype = items.GetType();
                    var modelProperties = modeltype.GetProperties();
                    int col = 1;
                    try
                    {
                        //i=1 不需要第一列的数据
                        for (int i = 0; i < length; i++)
                        {
                            if (i == 4) //单据号
                            {
                                continue;
                            }
                            else if (i == 13 && !isenter) //所属单位
                            {
                                continue;
                            }
                            else if (i == 14 && isenter) //业务单元
                            {
                                continue;
                            }
                            else if (i == 3)//单据字
                            {
                                worksheet.Cells[row, col].Value = $"{modeltype.GetProperty(modelProperties[i].Name).GetValue(items)}-{modeltype.GetProperty(modelProperties[i + 1].Name).GetValue(items)}";
                            }
                            else
                            {
                                worksheet.Cells[row, col].Value = modeltype.GetProperty(modelProperties[i].Name).GetValue(items);
                            }
                            col++;
                        }
                    }
                    catch
                    {
                    }

                    row++;
                }
                // 生成 Excel 文件流
                var stream = new MemoryStream(package.GetAsByteArray());

                // 设置响应头信息，告诉浏览器下载文件
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //命名excel名称  防止ASCII 报错

                var contentDisposition = new ContentDisposition
                {
                    FileName = $"会计辅助账明细.xlsx",
                    Inline = false
                };
                Response.Headers.Add("content-disposition", contentDisposition.ToString());

                // 将文件流输出到 Response 中，供前端下载
                await stream.CopyToAsync(Response.Body);
                stream.Close();
            }
            return new EmptyResult();
        }
        #region 设置导出列
        /// <summary>
        /// 明细导出列
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="optionValue"></param>
        /// <returns></returns>
        private ExcelWorksheet SetExcelWorksheetColumnsDetail(ExcelWorksheet worksheet,string optionValue="0")
        {
            worksheet.DefaultColWidth = 18;
            worksheet.Cells[1, 1, 2, 18].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            worksheet.Cells[1, 1, 2, 18].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 217, 217, 217));
            worksheet.Cells[1, 1, 2, 18].Style.HorizontalAlignment =ExcelHorizontalAlignment.Center;//水平居中
            //账务单位 基本信息   辅助项目 发生额     备注
            //日期  凭证类别 凭证字号    记账号 摘要  内容 会计科目    客商 部门  员工 项目  归属单位 借方金额    贷方金额
            worksheet.Cells["A1:A2"].Merge = true; // 合并单元格
            worksheet.Cells["A1:A2"].Value = "财务单位";
            worksheet.Cells["B1:H1"].Merge = true; 
            worksheet.Cells["B1:H1"].Value = "基本信息";
            worksheet.Cells["I1:M1"].Merge = true;
            worksheet.Cells["I1:M1"].Value = "辅助项目";
            worksheet.Cells["N1:O1"].Merge = true;
            worksheet.Cells["N1:O1"].Value = "发生额";
            worksheet.Cells["P1:P2"].Merge = true;
            worksheet.Cells["P1:P2"].Value = "备注";


            worksheet.Cells["B2"].Value = "日期";
            worksheet.Cells["C2"].Value = "凭证类别";
            worksheet.Cells["D2"].Value = "凭证字号";
            worksheet.Cells["E2"].Value = "记账号";
            worksheet.Cells["F2"].Value = "摘要";
            worksheet.Cells["G2"].Value = "内容";
            worksheet.Cells["H2"].Value = "会计科目";
            worksheet.Cells["I2"].Value = "客商";
            worksheet.Cells["J2"].Value = "部门";
            worksheet.Cells["K2"].Value = "员工";
            worksheet.Cells["L2"].Value = "项目";
            if (optionValue == "1")
            {
                worksheet.Cells["M2"].Value = "业务单元";
            }
            else
            {
                worksheet.Cells["M2"].Value = "归属单位";
            }
            worksheet.Cells["N2"].Value = "借方金额";
            worksheet.Cells["O2"].Value = "贷方金额";
           
            
            return worksheet;
        }
        /// <summary>
        /// 汇总导出列
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        private ExcelWorksheet SetExcelWorksheetColumnsSummary(ExcelWorksheet worksheet, RptAssistRecordRequest req)
        {
            var reqsummaryList = req.SummaryTypeList;
            var summaryNum = reqsummaryList.Count;
            var colNum= summaryNum + 3;
            worksheet.DefaultColWidth = colNum;
            worksheet.Cells[1, 1, 1, colNum].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            worksheet.Cells[1, 1, 1, colNum].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 217, 217, 217));
            worksheet.Cells[1, 1, 1, colNum].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
            //汇总方式
            var summaryList= _prodiver.GetSummaryTypeList(new SummaryTypeRequest { GroupID=req.GroupID,DataDate=req.EndDate,EnterpriseList=req.EnterpriseList });
            //汇总方式 借方金额    贷方金额

           for(var i = 0; i <summaryNum; i++)
           {
                var id = reqsummaryList[i];
                var name = "汇总方式" + i;
                var filterList = summaryList.Where(p => p.SV == id);
                if (filterList.Any())
                {
                    name = filterList.FirstOrDefault().SN;
                }
                worksheet.Cells[1, i+1].Value = name;
            }
            
            worksheet.Cells[1, summaryNum+1].Value = "借方金额";
            worksheet.Cells[1, summaryNum+2].Value = "贷方金额";


            return worksheet;
        }
       
        #endregion
        #endregion

        #region 下拉
        /// <summary>
        /// 汇总方式
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetSummaryTypeList")]
        public Result GetSummaryTypeList(SummaryTypeRequest req)
        {
            var result = new Result(); 
            if(req == null)
            {
                result.msg = "参数不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(req.DataDate))
            {
                result.msg = "DataDate不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(req.GroupID))
            {
                req.GroupID = _identityService.GroupId;
            }
            result.data= _prodiver.GetSummaryTypeList(req);
            result.code = 0;
            return result;
        }
        /// <summary>
        /// 财务单位
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetOrgEntityList")]
        public async Task<RestfulResult> GetOrgEntityList(Domain.OrgEnteRequest req)
        {
            var result = new RestfulResult() { code = -1 };
            if (req == null)
            {
                result.msg = "参数不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(req.enterpriseId))
            {
               req.enterpriseId = _identityService.GroupId;
            }
            if (string.IsNullOrEmpty(req.boid))
            {
                req.boid = _identityService.UserId;
            }
            result= await _fmbaseCommon.GetOrgEntityList(req);
            var list = new List<OrgEnterResult>();
            if (result?.code == 0 && result?.data != null)
            {
                list = JsonConvert.DeserializeObject<List<OrgEnterResult>>(result.data.ToString());
                list = list?.Where(p => p.IsEnter != 2)?.ToList();
            }
            result.code = 0;
            result.data = list;
            return result;
        }
        /// <summary>
        /// 科目：账务单位全不选，则会计科目展示集团下科目；选择账务单位，则展示此账务单位下会计科目，会计科目级次一级是单位，二级是科目类别，三级依次是各级次科目
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetSubjectTreeList")]
        public async Task<RestfulResult> GetSubjectTreeList(AssistDorpRequest req)
        {
            var result = new RestfulResult() { code = -1 };
            if (req == null)
            {
                result.msg = "参数不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(req.DataDate))
            {
                result.msg = "DataDate不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(req.GroupID))
            {
                req.GroupID = _identityService.GroupId;
            }
            if (string.IsNullOrEmpty(req.EnterpriseID))
            {
                req.EnterpriseID = _identityService.EnterpriseId;
            }
            
            var list = new List<Biz_Subject>();
            var param = new SubjectRequest() { DataDate=req.DataDate};
            try
            {
                if (req.EnterpriseList?.Count > 0)
                {
                    var enterids = string.Join(",", req.EnterpriseList);
                    var enterlist =_prodiver.GetEnterpriseList(req.GroupID,enterids);
                    param.IsGroup = null;
                    var num = req.EnterpriseList.Count;
                    Parallel.For(0, num, n =>
                    {
                        var enterid = req.EnterpriseList[n];
                        var newparam=new SubjectRequest() { DataDate=param.DataDate,IsGroup=param.IsGroup};
                        newparam.EnterpriseID = enterid;
                        var tmp = _fmbaseCommon.GetSubjectTreeList(newparam);
                        if (tmp?.Count > 0)
                        {
                            var firstRankList = tmp.Where(p => (string.IsNullOrEmpty(p.Pid) || p.Pid == "0") && p.Rank == 0).ToList();
                            firstRankList.ForEach(p => { p.Pid = enterid; });
                            tmp.ForEach(p => { p.IdKey = p.AccoSubjectId + "," + enterid; p.PidKey = p.Pid + "," + enterid; });
                        }
                        var entername= enterlist?.Where(p=>p.EnterpriseId== enterid)?.FirstOrDefault()?.EnterpriseName;  
                        tmp.Insert(0, new Biz_Subject() { AccoSubjectId = enterid, AccoSubjectName = entername, Rank = -1, Pid = "0", IdKey = enterid + "," + enterid, PidKey = "0" });
                        list.AddRange(tmp);
                    });
                }
                else//集团科目
                {
                    param.EnterpriseID = req.EnterpriseID;
                    param.IsGroup = true;                    
                    list = _fmbaseCommon.GetSubjectTreeList(param);
                    list.ForEach(p => { p.IdKey = p.AccoSubjectId ; p.PidKey = p.Pid; });
                }
                result.code = 0;
                result.data = list;
            }
            catch (Exception ex)
            {
                _logger.LogError($@"RptAssistRecord/GetSubjectTreeList:异常={ex.ToString()};参数={JsonConvert.SerializeObject(req)}");
                result.msg = "会计科目查询异常";
            }            
            return result;
        }
        /// <summary>
        /// 摘要：账务单位全不选，则结算摘要展示集团下摘要；选择账务单位，则展示此账务单位下结算摘要，结算摘要级次一级是单位，二级依次是各级次摘要
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetSettlesummaryList")]
        public RestfulResult GetSettlesummaryList(AssistDorpRequest req)
        {
            var result = new RestfulResult() { code = -1 };
            if (req == null)
            {
                result.msg = "参数不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(req.DataDate))
            {
                result.msg = "DataDate不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(req.GroupID))
            {
                req.GroupID = _identityService.GroupId;
            }
            if (string.IsNullOrEmpty(req.EnterpriseID))
            {
                req.EnterpriseID = _identityService.EnterpriseId;
            }

            var list = new List<Biz_Settlesummary>();
            try
            {
                if (req.EnterpriseList?.Count > 0)
                {
                    var enterids = string.Join(",", req.EnterpriseList);
                    var enterlist = _prodiver.GetEnterpriseList(req.GroupID, enterids);
                    var num = req.EnterpriseList.Count;
                    Parallel.For(0, num, n =>
                    {
                        var enterid = req.EnterpriseList[n];
                        var tmp = _fmbaseCommon.GetSettlesummaryList(enterid,req.DataDate);
                        if (tmp?.Count > 0)
                        {
                            var firstRankList = tmp.Where(p => (string.IsNullOrEmpty(p.pid) || p.pid == "0") && p.Rank == 1).ToList();
                            firstRankList.ForEach(p => { p.pid = enterid; });
                            tmp.ForEach(p => { p.IdKey = p.id + "," + enterid; p.PidKey = p.pid + "," + enterid; });
                        }
                        var entername = enterlist?.Where(p => p.EnterpriseId == enterid)?.FirstOrDefault()?.EnterpriseName;
                        tmp.Insert(0, new Biz_Settlesummary() { id = enterid, name = entername, Rank = -1, IdKey = enterid + "," + enterid });
                        list.AddRange(tmp);
                    });
                }
                else//集团摘要
                {
                    var enterid = req.EnterpriseID;
                    list = _fmbaseCommon.GetSettlesummaryList(req.EnterpriseID,req.DataDate);
                    if(list?.Count > 0)
                    {
                        list=list.Where(p=>p.IsGroup==1)?.ToList();
                        list.ForEach(p => { p.IdKey = p.id; p.PidKey = p.pid; });
                    }
                }
                result.code = 0;
                result.data = list;
            }
            catch (Exception ex)
            {
                _logger.LogError($@"RptAssistRecord/GetSubjectTreeList:异常={ex.ToString()};参数={JsonConvert.SerializeObject(req)}");
                result.msg = "结算摘要查询异常";
            }
            return result;
        }
        /// <summary>
        /// 部门 单位树
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetMarketList")]
        public RestfulResult GetMarketList(AssistDorpRequest req)
        {
            var result = new RestfulResult() { code = -1 };
            if (req == null)
            {
                result.msg = "参数不能为空";
                return result;
            }
            //if (string.IsNullOrEmpty(req.DataDate))
            //{
            //    result.msg = "DataDate不能为空";
            //    return result;
            //}
            if (req.EnterpriseList==null||req.EnterpriseList.Count==0)
            {
                result.msg = "EnterpriseList不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(req.GroupID))
            {
                req.GroupID = _identityService.GroupId;
            }

            var list = new List<MarketResult>();
            try
            {
                var enterids = string.Join(",", req.EnterpriseList);
                var enterlist = _prodiver.GetEnterpriseList(req.GroupID, enterids);
                var num = req.EnterpriseList.Count;
                Parallel.For(0, num, n =>
                {
                    var enterid = req.EnterpriseList[n];
                    var tmp = _fmbaseCommon.GetMarketList(enterid);
                    if (tmp?.Count > 0)
                    {
                        var firstRankList = tmp.Where(p => (string.IsNullOrEmpty(p.Pid) || p.Pid == "0") && p.Rank == 1).ToList();
                        firstRankList.ForEach(p => { p.Pid = enterid; });
                    }
                    var entermodel = enterlist?.Where(p => p.EnterpriseId == enterid)?.FirstOrDefault();
                    var entername = entermodel?.EnterpriseName;
                    tmp.Insert(0, new MarketResult() { MarketId = enterid, MarketName = entername, Rank = 0 });
                    list.AddRange(tmp);
                });
                result.code = 0;
                result.data = list;
            }
            catch (Exception ex)
            {
                _logger.LogError($@"RptAssistRecord/GetMarketList:异常={ex.ToString()};参数={JsonConvert.SerializeObject(req)}");
                result.msg = "部门查询异常";
            }
            return result;
        }
        /// <summary>
        /// 项目 单位树
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetProjectList")]
        public RestfulResult GetProjectList(AssistDorpRequest req)
        {
            var result = new RestfulResult() { code = -1 };
            if (req == null)
            {
                result.msg = "参数不能为空";
                return result;
            }
            if (req.EnterpriseList == null || req.EnterpriseList.Count == 0)
            {
                result.msg = "EnterpriseList不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(req.GroupID))
            {
                req.GroupID = _identityService.GroupId;
            }

            var list = new List<FMProject>();
            try
            {
                var enterids = string.Join(",", req.EnterpriseList);
                var enterlist = _prodiver.GetEnterpriseList(req.GroupID, enterids);
                var num = req.EnterpriseList.Count;
                Parallel.For(0, num, n =>
                {
                    var enterid = req.EnterpriseList[n];
                    var tmp = _fmbaseCommon.GetBelongProject(enterid);
                    if (tmp?.Count > 0)
                    {
                        var firstRankList = tmp.Where(p => (string.IsNullOrEmpty(p.PID) || p.PID == "0") && p.Rank == 1).ToList();
                        firstRankList.ForEach(p => { p.PID = enterid; });
                    }
                    var entermodel = enterlist?.Where(p => p.EnterpriseId == enterid)?.FirstOrDefault();
                    var entername = entermodel?.EnterpriseName;
                    tmp.Insert(0, new FMProject() { ProjectID = enterid, ProjectName = entername, Rank = 0 });
                    list.AddRange(tmp);
                });
                result.code = 0;
                result.data = list;
            }
            catch (Exception ex)
            {
                _logger.LogError($@"RptAssistRecord/GetMarketList:异常={ex.ToString()};参数={JsonConvert.SerializeObject(req)}");
                result.msg = "项目查询异常";
            }
            return result;
        }
        /// <summary>
        /// 单据字 单位树
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetTicketedPointList")]
        public RestfulResult GetTicketedPointList(AssistDorpRequest req)
        {
            var result = new RestfulResult() { code = -1 };
            if (req == null)
            {
                result.msg = "参数不能为空";
                return result;
            }
            if (req.EnterpriseList == null || req.EnterpriseList.Count == 0)
            {
                result.msg = "EnterpriseList不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(req.GroupID))
            {
                req.GroupID = _identityService.GroupId;
            }

            var list = new List<FMTicketedPoint>();
            try
            {
                var enterids = string.Join(",", req.EnterpriseList);
                var enterlist = _prodiver.GetEnterpriseList(req.GroupID, enterids);
                var num = req.EnterpriseList.Count;
                Parallel.For(0, num, n =>
                {
                    var enterid = req.EnterpriseList[n];
                    var tmp = _fmbaseCommon.GetTicketedPointList(enterid);
                    if (tmp?.Count > 0)
                    {
                        tmp.ForEach(p => { p.PID = enterid; });
                    }
                    tmp.ForEach(p => { p.IdKey = p.TicketedPointID + "," + enterid; p.PidKey = p.PID + "," + enterid; });
                    var entermodel = enterlist?.Where(p => p.EnterpriseId == enterid)?.FirstOrDefault();
                    var entername = entermodel?.EnterpriseName;
                    tmp.Insert(0, new FMTicketedPoint() { TicketedPointID = enterid, TicketedPointName = entername,IdKey= enterid + "," + enterid });
                    
                    list.AddRange(tmp);
                });
                result.code = 0;
                result.data = list;
            }
            catch (Exception ex)
            {
                _logger.LogError($@"RptAssistRecord/GetMarketList:异常={ex.ToString()};参数={JsonConvert.SerializeObject(req)}");
                result.msg = "单据字查询异常";
            }
            return result;
        }
        /// <summary>
        /// 获取系统选项
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetOptionValue")]
        public RestfulResult GetOptionValue(OptionRequest req)
        {
            var result = new RestfulResult() { code = -1 };
            if (req == null)
            {
                result.msg = "参数不能为空";
                return result;
            }
            
            if (string.IsNullOrEmpty(req.EnterpriseID))
            {
                req.EnterpriseID = _identityService.EnterpriseId;
            }
            var optionvalue = _fmbaseCommon.OptionConfigValueNew(req.OptionId,req.EnterpriseID,req.ScopeCode);
            result.code = 0;
            result.data = optionvalue;
            return result;
        }
        /// <summary>
        /// 缓存汇总方式
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetCachSummaryList")]
        public RestfulResult GetCachSummaryList()//AssistDorpRequest req)
        {
            var result = new RestfulResult() { code = 0 };
            var list = new List<string>();
            try
            {
                //if (req == null)
                //{
                //    result.msg = "参数不能为空";
                //    return result;
                //}

                //if (string.IsNullOrEmpty(req.GroupID))
                //{
                //    req.EnterpriseID = _identityService.GroupId;
                //}
                string rediskey = $"KJFZZ:-GroupId:{_identityService.GroupId}-Userid={_identityService.UserId}";
                if (RedisHelper.Exists(rediskey))
                {
                    list= RedisHelper.Get<List<string>>(rediskey);
                }
            }
            catch { }
            result.data = list;
            return result;
        }

        private void GetRedisInstance()
        {
            var csredis = new CSRedis.CSRedisClient(_hostCongfiguration.RedisServer);
            //初始化 RedisHelper
            RedisHelper.Initialization(csredis);
        }
        #endregion

    }
}
