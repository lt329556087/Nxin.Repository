using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.MS_FormulaProductPrice;
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
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;
using System.Net.Mime;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.ComponentModel.DataAnnotations;
using NPOI.SS.Util;
using MP.MiddlePlatform.Integration.Integaration;
using FinanceManagement.ApiHost.Extension;
using Org.BouncyCastle.Ocsp;
using Architecture.Common.Util;
using Microsoft.AspNetCore.Cors;
//using MimeKit;

namespace FinanceManagement.ApiHost.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors("allowSpecificOrigins")]
    public class MS_FormulaProductPriceController : ControllerBase
    {
        IMediator _mediator;
        MS_FormulaProductPriceODataProvider _provider;
        private readonly ILogger<MS_FormulaProductPriceController> _logger;
        private IIdentityService _identityService;
        private MS_FormulaODataProvider _formulaProvider;
        private HandleData _handleData = new HandleData();
        private FMBaseCommon _fmBaseCommom;
        ExportCommon _exportCommon;
        public MS_FormulaProductPriceController(IMediator mediator, MS_FormulaProductPriceODataProvider provider, ILogger<MS_FormulaProductPriceController> logger, IIdentityService identityService,
           MS_FormulaODataProvider formulaProvider, FMBaseCommon fmBaseCommom
, ExportCommon exportCommon)
        {
            _mediator = mediator;
            _provider = provider;
            _logger = logger;
            _identityService = identityService;
            _formulaProvider = formulaProvider;
            _fmBaseCommom = fmBaseCommom;
            _exportCommon = exportCommon;
        }


        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("{key}")]
        //[PermissionAuthorize(Permission.Retrieve)]
        public async Task<Result> GetDetail(long key)
        {
            var result = new Result();
            var data = await _provider.GetSingleDataAsync(key);
            if (data != null)
            {
                var ext = _provider.GetExtData(key.ToString());
                if (ext?.Count > 0)
                {
                    data.EnterpriseList = ext.Select(p => p.EnterpriseID).ToList();
                    data.EnterpriseNameList = ext.Select(p => p.EnterpriseName).ToList();
                }
            }
            result.code = ErrorCode.Success.GetIntValue();
            result.data = data;
            return result;
        }

        //增加
        [HttpPost]
        [AllowAnonymous]
        //[PermissionAuthorize(Permission.Create)]
        public async Task<Result> Add([FromBody] MS_FormulaProductPriceAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        //[PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] MS_FormulaProductPriceDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        //[PermissionAuthorize(Permission.Update)]
        public async Task<Result> Modify([FromBody] MS_FormulaProductPriceModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        #region 接口
        //[HttpPost]
        //[Route("GetFormulaByDate")]
        //public List<dynamic> GetFormulaByDate(MS_FormulaSearch model)
        //{
        //    var list= _formulaProvider.GetFormulaByDate(model);
        //    if (list?.Count > 0)
        //    {
        //        foreach (var item in list.GroupBy(p=>p.) { }
        //    }
        //}
        /// <summary>
        /// 重复商品
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetIsExistsProduct")]
        public RestfulResult GetIsExistsProduct(MS_FormulaProductPriceBaseCommand req)
        {
            var result = new RestfulResult() { code = -1 };
            var validResult = _handleData.ValidData(req, true);
            if (validResult.code != 0)
            {
                result.msg = validResult.msg;
                return result;
            }
            if (string.IsNullOrEmpty(req.GroupID))
            {
                req.GroupID = _identityService.GroupId;
            }
            var productList = req.Lines.Select(p => p.ProductID).ToList();
            var list = IsExistsProduct(req, productList);
            result.data = list;
            result.code = 0;
            return result;
        }
        private List<MS_FormulaProductPriceValidODataEntity> IsExistsProduct(MS_FormulaProductPriceBaseCommand req, List<string> productList)
        {
            var resultList = new List<MS_FormulaProductPriceValidODataEntity>();
            var productids = string.Join(",", productList);
            var list = _provider.GetExistProductByCon(req, productids, out productids);
            if (list?.Count > 0)
            {
                //var numericalOrderList = list.Select(p => p.NumericalOrder).ToList();
                //var numericalOrders=string.Join(",", numericalOrderList);
                var enterpriseIDs = string.Join(',', req.EnterpriseList);
                foreach (var item in list.GroupBy(p => p.NumericalOrder))
                {
                    var numericalOrder = item.Key;
                    List<MS_FormulaProductPriceExtODataEntity> extList = _provider.GetExtDataByCon(numericalOrder, enterpriseIDs);
                    if (extList.Any())
                    {
                        var filterextNameList = extList.Select(p => p.EnterpriseName).ToList();
                        foreach (var detail in item)
                        {
                            if (filterextNameList.Any())
                            {
                                detail.EnterpriseName = string.Join(',', filterextNameList);
                            }
                            resultList.Add(detail);
                        }
                    }
                }
            }
            return resultList;
        }
        #endregion

        #region 导出
        [HttpPost]
        [Route("Export")]
        public async Task<ActionResult> Export(MS_FormulaProductPriceExportRequest req)
        {
            //MS_FormulaProductPriceExportRequest req = new MS_FormulaProductPriceExportRequest() { GroupID = "957025251000000" };
            //var result = new RestfulResult() { code = -1,data= new EmptyResult() };
            //if (req == null)
            //{
            //    result.msg = "参数为空";
            //    return result;
            //}
            if (string.IsNullOrEmpty(req.GroupID))
            {
                req.GroupID = _identityService.GroupId;
            }
            #region 基础配置
            //获取基础配置
            var Propertys = new string[] { "Number", "EnterpriseName", "DataDate", "ProductName", "Specification", "StandardPack", "MeasureUnitName", "MarketPrice", "Remarks" };
            var auditColumns = new List<AuditColumn>()
                         {
                            new AuditColumn(){ Caption= "单据号",  DataField="",Columns=new List<AuditColumn>(){ }, Region =new int[]{ 0,0,0,0},ColumnIndex=0, IsEnd=true },
                            new AuditColumn(){ Caption="账务单位", DataField="",Columns=new List<AuditColumn>(){ }, Region =new int[]{ 0,0,1,1},ColumnIndex=1 , IsEnd=true  },
                            new AuditColumn(){ Caption="日期", DataField="",Columns=new List<AuditColumn>(){ }, Region =new int[]{ 0,0,2,2},ColumnIndex=2, IsEnd=true},
                            new AuditColumn(){ Caption="商品代号", DataField="",Columns=new List<AuditColumn>(){ }, Region =new int[]{ 0,0,3,3},ColumnIndex=3, IsEnd=true},
                            new AuditColumn(){ Caption="规格", DataField="",Columns=new List<AuditColumn>(){ }, Region =new int[]{ 0,0,4,4},ColumnIndex=4, IsEnd=true},
                            new AuditColumn(){ Caption="标包", DataField="",Columns=new List<AuditColumn>(){ }, Region =new int[]{ 0,0,5,5},ColumnIndex=5, IsEnd=true},
                            new AuditColumn(){ Caption="计量单位", DataField="",Columns=new List<AuditColumn>(){ }, Region =new int[]{ 0,0,6,6},ColumnIndex=6, IsEnd=true},
                            new AuditColumn(){ Caption="市场单价", DataField="",Columns=new List<AuditColumn>(){ }, Region =new int[]{ 0,0,7,7},ColumnIndex=7, IsEnd=true},
                            new AuditColumn(){ Caption="备注", DataField="",Columns=new List<AuditColumn>(){ }, Region =new int[]{ 0,0,8,8},ColumnIndex=8, IsEnd=true},
                         };
            #endregion
            //获取数据
            var exportDataList = await _provider.GetExportDatalist(req);
            if (exportDataList != null)
            {
                foreach (var item in exportDataList)
                {
                    //获取账务单位
                    var extList = _provider.GetExtData(item.NumericalOrder);
                    if (extList?.Count > 0)
                    {
                        var EnterpriseNameList = extList.Select(p => p.EnterpriseName).ToList();
                        item.EnterpriseName = string.Join(",", EnterpriseNameList);
                    }
                }
                IWorkbook workbook = new XSSFWorkbook();
                XSSFFont font = (XSSFFont)workbook.CreateFont();
                XSSFCellStyle style1 = (XSSFCellStyle)workbook.CreateCellStyle();
                //font.FontHeightInPoints = 14;
                style1.Alignment = HorizontalAlignment.CenterSelection;
                font.FontName = "宋体";
                //style1.VerticalAlignment = VerticalAlignment.Justify;//垂直居中 方法1 
                //font.Boldweight = (short)FontBoldWeight.Bold;//加粗
                style1.SetFont(font);
                ISheet sheet = workbook.CreateSheet("配方商品价格设定");
                _exportCommon.SetCell(sheet, auditColumns, -1, style1);
                int rounum = 1;
                foreach (var item in exportDataList)
                {
                    Type type = item.GetType();
                    IRow drow = sheet.CreateRow(rounum);
                    for (int c = 0; c < Propertys.Length; c++)
                    {
                        ICell cell = drow.CreateCell(c, NPOI.SS.UserModel.CellType.String);
                        var value = type.GetProperty(Propertys[c]).GetValue(item) + "";
                        cell.SetCellValue(value);
                    }
                    rounum++;
                }
                byte[] buffer = new byte[1024 * 50];
                using (MemoryStream ms = new MemoryStream())
                {
                    workbook.Write(ms, true);
                    buffer = ms.ToArray();
                    ms.Close();
                }
                return File(buffer, "application/ms-excel", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "配方商品价格设定.xlsx");
            }
            return File(new byte[1024 * 50], "application/ms-excel", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "配方商品价格设定.xlsx");
        }

        #endregion

        #region 导入
        /// <summary>
        /// 导入 需要token 从token取当前单位、集团和登录人信息
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Import")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Import(IFormFile file)
        {
            try
            {
                var validmsg = ValidParam(file);
                if (!string.IsNullOrEmpty(validmsg))
                {
                    return Ok(new { code = "201", msg = validmsg });
                }
                var result = new RestfulResult { code = 201 };
                var excelList = new List<MS_FormulaProductPriceExcelModel>();
                using (Stream fs = file.OpenReadStream())
                {
                    Workbook workbook = new Workbook(fs);
                    Worksheet sheet = workbook.Worksheets[0];
                    Cells cells = sheet.Cells;
                    DataTable dt = cells.ExportDataTable(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);
                    result = ValidData(dt, excelList).Result;
                }
                result.code = ErrorCode.ServerBusy.GetIntValue();
                if (string.IsNullOrEmpty(result.msg))
                {
                    if (excelList.Count > 0)
                    {
                        var domainList = GetDomainList(excelList);
                        var request = new MS_FormulaProductPriceListCommand();
                        request.List = domainList;
                        var tempResult = await _mediator.Send(request);
                        if (tempResult.code != ErrorCode.Success.GetIntValue())
                        {
                            return Ok(new { result.code, status = false, msg = $"导入失败:{tempResult.msg}" });
                        }
                        else
                        {
                            return Ok(new { result.code, status = true, msg = $@"导入成功，共生成{domainList.Count}张单据。" });
                        }
                    }
                    else
                    {
                        return Ok(new { result.code, status = false, msg = "无有效数据" });
                    }
                }
                else
                {
                    return Ok(new { result.code, status = false, msg = $"校验未通过:{result.msg}" });
                }

            }
            catch (Exception e)
            {
                return Ok(new { code = ErrorCode.RequestArgumentError.GetIntValue(), status = false, msg = "导入失败", erro = e });
            }
        }
        private string ValidParam(IFormFile file)
        {
            var msg = "";
            if (file == null)
            {
                return "请上传文件;";
            }
            var fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension == null)
            {
                return "文件无后缀信息";
            }
            else if (fileExtension != ".xlsx" && fileExtension != ".xls")
            {
                return "只能上传 .xlsx 或 .xls 后缀的文件;";
            }
            long length = file.Length;
            var maxSize = 200;//200M
            if (length > 1024 * 1024 * maxSize)
            {
                return $"上传文件不能超过{maxSize}M;";
            }
            return msg;
        }
        private async Task<RestfulResult> ValidData(DataTable dt, List<MS_FormulaProductPriceExcelModel> excelList)
        {
            var result = new RestfulResult { code = 201 };
            Dictionary<string, string> columDic = GetColumnName();
            string msg = "";
            if (dt.Columns != null && dt.Columns.Count == columDic.Count)
            {
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
                    result.msg = msg;
                    return result;
                }
            }
            else
            {
                result.msg = "文件列不匹配;";
                return result;
            }


            result.code = ErrorCode.RequestArgumentError.GetIntValue();
            #region 数据验证
            var groupId = _identityService.GroupId;
            var userId = _identityService.UserId;
            //权限单位
            var enterList = _fmBaseCommom.GetAuthorEnterpiseToModel(_identityService.EnterpriseId, userId);
            //商品
            var productList = _fmBaseCommom.GetGroupProductList(groupId);
            //var productId = "0";
            //*序号	*账务单位	*日期	*商品代号	市场单价	备注
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                var rowindex = i + 1;
                var number = row["Number"].ToString();
                if (string.IsNullOrEmpty(number))
                {
                    msg += $@"第{rowindex}行序号不能为空;";
                }
                var productName = row["PrductName"].ToString();
                //msg += ValidFieldEmpty(row["Number"].ToString(), "序号", rowindex);
                msg += ValidFieldEmpty(productName, "商品代号", rowindex);
                var filterproductList = productList.Where(p => p.ProductName == productName);
                dynamic product = null;
                if (filterproductList?.Count() > 0)
                {
                    product = filterproductList.FirstOrDefault();
                }
                else
                {
                    msg += $@"第{rowindex}行商品代号不存在;";
                }
                //市场价格
                decimal marketPrice = 0;
                var pricemsg = ValidFieldNumber(row["MarketPrice"].ToString(), "市场价格", rowindex, ref marketPrice);
                if (!string.IsNullOrEmpty(pricemsg))
                {
                    msg += pricemsg;
                }
                var excelObj = new MS_FormulaProductPriceExcelModel()
                {
                    Number = row["Number"].ToString(),
                    EnterpriseName = row["EnterpriseName"].ToString(),
                    DataDate = row["DataDate"].ToString(),
                    ProductName = row["PrductName"].ToString(),
                    MarketPrice = marketPrice,
                    Remarks = row["Remarks"].ToString(),
                    ProductID = product == null ? "0" : product.ProductID,
                    Specification = product == null ? "" : product.Specification,
                    StandardPack = product == null ? null : Convert.ToDecimal(product.StandardPack),
                    MeasureUnit = product == null ? "" : product.MeasureUnit
                };
                excelList.Add(excelObj);
            }
            if (excelList.Count == 0)
            {
                msg += "无数据;";
            }
            //数据验证
            var rowIndex = 1;
            var sameprductflag = false;
            foreach (var gropItem in excelList)
            {
                var firstModel = gropItem;
                //单位
                var enterpriseName = firstModel.EnterpriseName.ToString();
                msg += ValidFieldEmpty(enterpriseName, "账务单位", rowIndex);
                var enternameList = enterpriseName.Split(',').ToList();
                var enteridList = new List<string>();
                var entermsg = "";
                foreach (var entername in enternameList)
                {
                    var enter = enterList.Where(m => m.EnterpriseName == entername);
                    if (enter?.Count() > 0)
                    {
                        enteridList.Add(enter.FirstOrDefault().EnterpriseID);
                    }
                    else
                    {
                        entermsg += entername + ",";
                    }
                }
                if (!string.IsNullOrEmpty(entermsg))
                {
                    msg += $"当前账号无第{rowIndex}行账务单位{entermsg.TrimEnd(',')}权限，请先分配单位权限;";
                }
                else
                {
                    firstModel.EnterpriseList = enteridList;
                }
                //日期
                DateTime dateTime = DateTime.MinValue;
                var dataDate = firstModel.DataDate.ToString();
                msg += ValidFieldDate(dataDate, "日期", rowIndex, ref dateTime);
                //同张单据是否有重复商品
                var productIdList = excelList.Where(s => s.Number == firstModel.Number && s.ProductID != "0").Select(s => s.ProductID).ToList();// gropItem.Select(c => c.ProductID).Distinct()?.ToList();
                if (productIdList.Count>0)
                {
                    if (productIdList.Count != productIdList.Distinct().Count())
                    {
                        sameprductflag = true;
                        msg += $"序号为{firstModel.Number}存在相同商品;";
                    }
                    else
                    {//同一天，同账务单位是否已存在相同商品
                        var req = new MS_FormulaProductPriceBaseCommand() { GroupID = groupId, DataDate = firstModel.Date, EnterpriseList = enteridList };
                        var existProductList = IsExistsProduct(req, productIdList);
                        if (existProductList?.Count > 0)
                        {
                            sameprductflag = true;
                            foreach (var product in existProductList)
                            {
                                msg += $"序号[{firstModel.Number}]商品代号{product.ProductName}已存在;";
                            }
                        }
                    }
                }
                rowIndex++;
            }

            #region 不同单据中是否存在相同商品
            if (!sameprductflag)
            {
                //不同单据中是否存在相同商品
                foreach (var gropItem in excelList.GroupBy(m => new { m.ProductID, m.DataDate }))
                {
                    var firstModel = gropItem.FirstOrDefault();
                    var groupList = gropItem.ToList();
                    if (groupList.Count() > 1)//不同序号
                    {
                        var sameProductList = new List<string>();
                        //账务单位
                        for (var i = 0; i < groupList.Count; i++)
                        {
                            for (var j = 0; j < groupList.Count; j++)
                            {
                                if (i == j) continue;//排除本身
                                var intersectList = groupList[i].EnterpriseList.Intersect(groupList[j].EnterpriseList);
                                if (intersectList.Count() > 0)
                                {
                                    var isExists = sameProductList.Where(p => p == (i + "," + j) || p == (j + "," + i)).ToList();
                                    if (isExists.Count == 0)
                                    {
                                        msg += $@"序号{groupList[i].Number}与{groupList[j].Number}的{groupList[i].ProductName}重复;";
                                    }
                                }
                            }
                        }
                    }

                    var tmpList = gropItem.Select(p => p.EnterpriseList);

                }
            }
            #endregion

            result.msg = msg;
            return result;
            #endregion
        }
        private List<Domain.MS_FormulaProductPrice> GetDomainList(List<MS_FormulaProductPriceExcelModel> list)
        {
            var domainList = new List<Domain.MS_FormulaProductPrice>();
            var groupId = _identityService.GroupId;
            var userId = _identityService.UserId;
            foreach (var gropItem in list.GroupBy(m => m.Number))
            {
                var firstModel = gropItem.FirstOrDefault();
                var model = new Domain.MS_FormulaProductPrice
                {
                    DataDate = Convert.ToDateTime(firstModel.DataDate),
                    OwnerID = userId,
                    GroupID = groupId
                };
                var detailList = new List<MS_FormulaProductPriceDetail>();
                foreach (var item in gropItem)
                {
                    var detail = new MS_FormulaProductPriceDetail
                    {
                        ProductID = item.ProductID,
                        Specification = item.Specification,
                        StandardPack = item.StandardPack,
                        MeasureUnit = item.MeasureUnit,
                        MarketPrice = item.MarketPrice,
                        Remarks = item.Remarks,
                        ModifiedDate = DateTime.Now
                    };
                    detailList.Add(detail);
                }
                var extList = new List<MS_FormulaProductPriceExt>();
                foreach (var enter in firstModel.EnterpriseList)
                {
                    var ext = new MS_FormulaProductPriceExt()
                    {
                        EnterpriseID = enter,
                        ModifiedDate = DateTime.Now
                    };
                    extList.Add(ext);
                }
                model.Lines = detailList;
                model.ExtList = extList;
                domainList.Add(model);
            }
            return domainList;
        }
        private Dictionary<string, string> GetColumnName()
        {
            //*序号	*账务单位	*日期	*商品代号	市场单价	备注
            return new Dictionary<string, string>()
                    {
                        {"*序号","Number"},
                        {"*账务单位","EnterpriseName"},
                        {"*日期","DataDate"},
                        {"*商品代号","PrductName"},
                        {"市场单价","MarketPrice"},
                        {"备注","Remarks"}
                    };
        }
        private string ValidFieldEmpty(string value, string FieldName, int rowindex)
        {
            if (string.IsNullOrEmpty(value))
                return $"第{rowindex}行{FieldName}不允许为空;";
            return "";
        }
        private string ValidFieldDate(string value, string FieldName, int rowindex, ref DateTime dateTime)
        {
            var msg = ValidFieldEmpty(value, FieldName, rowindex);
            if (!string.IsNullOrEmpty(msg))
            {
                return msg;
            }
            //DateTime dateTime = new DateTime();
            bool convertResult = DateTime.TryParse(value, out dateTime);
            if (!convertResult)
            {
                return $"第{rowindex}行{FieldName}格式不符;";
            }
            return "";
        }
        private string ValidFieldNumber(string value, string FieldName, int rowindex, ref decimal result)
        {
            if (string.IsNullOrEmpty(value))
            {
                result = 0;
                return "";
            }
            bool convertResult = decimal.TryParse(value, out result);
            if (!convertResult)
            {
                return $"第{rowindex}行{FieldName}请保留两位小数;";
            }
            return "";
        }
        #endregion
    }
}
