using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using Aspose.Cells;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.Common;
using FinanceManagement.Common.NewMakeVoucherCommon;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors("allowSpecificOrigins")]
    public class MS_FormulaController : ControllerBase
    {
        VoucherAmortizationUtil _comUtil;
        IMediator _mediator;
        MS_FormulaODataProvider _provider;
        IIdentityService _identityService;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;
        FMBaseCommon _baseUnit;
        ExportCommon _exportCommon;

        public MS_FormulaController(IMediator mediator, MS_FormulaODataProvider provider, VoucherAmortizationUtil comUtil, IIdentityService identityService, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration, FMBaseCommon baseUnit, ExportCommon exportCommon)
        {
            _mediator = mediator;
            _provider = provider;
            _comUtil = comUtil;
            _identityService = identityService;
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            _baseUnit = baseUnit;
            _exportCommon = exportCommon;
        }
        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("{key}")]
        public Result GetDetail(long key)
        {
            try
            {
                var result = new Result();
                var data = _provider.GetSingleData(key);
                result.data = data;
                return result;
            }
            catch (Exception ex)
            {
                return new Result() { code = -1, msg = "查询失败,请联系管理员！" };
            }
        }

        //增加
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] MS_FormulaAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        //[PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] MS_FormulaDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        public async Task<Result> Modify([FromBody] MS_FormulaModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        [HttpPost]
        [Route("GetFormulaByDate")]
        public List<dynamic> GetFormulaByDate(MS_FormulaSearch model)
        {
            return _provider.GetFormulaByDate(model);
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
                AuthenticationHeaderValue authentication = null;
                bool verification = AuthenticationHeaderValue.TryParse(HttpContext.Request.GetAuthToken(), out authentication);
                if (!verification)
                {
                    return Ok(new { code = "201", msg = "当前未检测到登录状态" });
                }
                List<MS_FormulaImport> importList = new List<MS_FormulaImport>();
                using (Stream fs = file.OpenReadStream())
                {
                    Workbook workbook = new Workbook(fs);
                    Worksheet sheet = workbook.Worksheets[0];
                    Cells cells = sheet.Cells;
                    DataTable dt = cells.ExportDataTable(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);
                    Dictionary<string, string> columDic = new Dictionary<string, string>()
                    {
                        {"*序号","RowNum"},
                        {"*配方名称","FormulaName"},
                        {"*日期","DataDate"},
                        {"*应用单位","UseEnterprise"},
                        {"*适用产品","UseProduct"},
                        {"*配方基数","BaseQuantity"},
                        {"*有效期间","EffectiveDate"},
                        {"原料信息","ProductName"},
                        {"Column9","ProportionQuantity"},
                        {"Column10","UnitCost"},
                        {"包装信息","ProductExtendName"},
                        {"Column12","PackingName"},
                        {"Column13","Quantity"},
                        {"Column14","IsUse"}
                    };
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

                    #region 基础信息
                    //权限单位
                    var enterList = _baseUnit.GetAuthorEnterpiseToModel(_identityService.EnterpriseId, _identityService.UserId);
                    var productList = _baseUnit.GetGroupProductList(_identityService.GroupId);
                    #endregion
                    if (dt.Rows.Count >= 1)
                    {
                        dt.Rows.RemoveAt(0);
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int number = i + 1;
                        MS_FormulaImport import = new MS_FormulaImport();
                        //序号
                        import.RowNum = dt.Rows[i]["RowNum"].ToString();
                        if (string.IsNullOrEmpty(import.RowNum))
                        {
                            return Ok(new { code = "201", status = false, msg = $"第{number}行序号不能为空" });
                        }
                        //配方名称
                        import.FormulaName = dt.Rows[i]["FormulaName"].ToString();
                        if (string.IsNullOrEmpty(import.FormulaName))
                        {
                            return Ok(new { code = "201", status = false, msg = $"第{number}行配方名称不能为空" });
                        }
                        //日期
                        DateTime DataDateResult = DateTime.Now;
                        string DataDate = dt.Rows[i]["DataDate"].ToString();
                        bool isDataDate = DateTime.TryParse(DataDate, out DataDateResult);
                        if (string.IsNullOrEmpty(DataDate))
                        {
                            return Ok(new { code = "201", status = false, msg = $"第{number}行日期不能为空" });
                        }
                        if (!isDataDate)
                        {
                            return Ok(new { code = "201", status = false, msg = $"第{number}行日期格式不符" });
                        }
                        import.DataDate = DataDateResult;
                        //应用单位
                        string UseEnterprise = dt.Rows[i]["UseEnterprise"].ToString();
                        if (string.IsNullOrEmpty(UseEnterprise))
                        {
                            return Ok(new { code = "201", status = false, msg = $"第{number}行应用单位不能为空" });
                        }
                        UseEnterprise = UseEnterprise.Replace('，', ',');
                        List<string> UseEnterpriseStrList = UseEnterprise.Split(',').ToList();
                        List<DictionaryData> UseEnterpriseList = new List<DictionaryData>();
                        foreach (var item in UseEnterpriseStrList)
                        {
                            var result1 = enterList.Where(s => s.EnterpriseName == item).FirstOrDefault();
                            if (result1 != null)
                            {
                                UseEnterpriseList.Add(new DictionaryData() { Id = result1.EnterpriseID, Name = result1.EnterpriseName });
                            }
                            else
                            {
                                return Ok(new { code = "201", status = false, msg = $"第{number}行{item}无应用单位权限，请先分配单位权限" });
                            }
                        }
                        import.UseEnterpriseList = UseEnterpriseList;
                        //适用商品
                        string UseProduct = dt.Rows[i]["UseProduct"].ToString();
                        if (string.IsNullOrEmpty(UseProduct))
                        {
                            return Ok(new { code = "201", status = false, msg = $"第{number}行适用商品不能为空" });
                        }
                        UseProduct = UseProduct.Replace('，', ',');
                        List<string> UseProductStrList = UseProduct.Split(',').ToList();
                        List<DictionaryData> UseProductList = new List<DictionaryData>();
                        foreach (var item in UseProductStrList)
                        {
                            var result2 = productList.Where(s => s.ProductName == item).FirstOrDefault();
                            if (result2 != null)
                            {
                                UseProductList.Add(new DictionaryData() { Id = result2.ProductID, Name = result2.ProductName });
                            }
                            else
                            {
                                return Ok(new { code = "201", status = false, msg = $"第{number}行适用产品不存在" });
                            }
                        }
                        import.UseProductList = UseProductList;
                        //配方基数
                        decimal BaseQuantityResult = 0M;
                        string BaseQuantity = dt.Rows[i]["BaseQuantity"].ToString();
                        bool isBaseQuantity = decimal.TryParse(BaseQuantity, out BaseQuantityResult);
                        if (string.IsNullOrEmpty(BaseQuantity))
                        {
                            return Ok(new { code = "201", status = false, msg = $"第{number}行配方基数不能为空" });
                        }
                        if (!isBaseQuantity)
                        {
                            return Ok(new { code = "201", status = false, msg = $"第{number}行配方基数请录入阿拉伯数字" });
                        }
                        import.BaseQuantity = BaseQuantityResult;
                        //有效期间
                        string EffectiveDate = dt.Rows[i]["EffectiveDate"].ToString();
                        List<string> EffectiveDateList = EffectiveDate.Split('-').ToList();
                        if (string.IsNullOrEmpty(EffectiveDate))
                        {
                            return Ok(new { code = "201", status = false, msg = $"第{number}行有效期间不能为空" });
                        }
                        DateTime EffectiveBeginDate = DateTime.Now;
                        bool isEffectiveBeginDate = DateTime.TryParse(EffectiveDateList[0], out EffectiveBeginDate);
                        if (!isEffectiveBeginDate)
                        {
                            return Ok(new { code = "201", status = false, msg = $"第{number}行有效期间格式有误" });
                        }
                        import.EffectiveBeginDate = EffectiveBeginDate;
                        if (EffectiveDateList.Count > 1)
                        {
                            if (!string.IsNullOrEmpty(EffectiveDateList[1]))
                            {
                                DateTime EffectiveEndDate = DateTime.Now;
                                bool isEffectiveEndDate = DateTime.TryParse(EffectiveDateList[1], out EffectiveEndDate);
                                if (!isEffectiveEndDate)
                                {
                                    return Ok(new { code = "201", status = false, msg = $"第{number}行有效期间格式有误" });
                                }
                                import.EffectiveEndDate = EffectiveEndDate;
                            }
                        }
                        //商品代号
                        string ProductName = dt.Rows[i]["ProductName"].ToString();
                        string ProportionQuantity = dt.Rows[i]["ProportionQuantity"].ToString();
                        if (!string.IsNullOrEmpty(ProductName) || !string.IsNullOrEmpty(ProportionQuantity))
                        {
                            if (string.IsNullOrEmpty(ProductName))
                            {
                                return Ok(new { code = "201", status = false, msg = $"第{number}行商品代号不能为空" });
                            }
                            var prodyctResult1 = productList.Where(m => m.ProductName == ProductName).FirstOrDefault();
                            if (prodyctResult1 != null)
                            {
                                import.ProductID = prodyctResult1.ProductID;
                                import.ProductName = prodyctResult1.ProductName;
                            }
                            else
                            {
                                return Ok(new { code = "201", status = false, msg = $"第{number}行商品代号不存在" });
                            }
                            //理论耗用量
                            decimal ProportionQuantityResult = 0M;
                            bool isProportionQuantity = decimal.TryParse(ProportionQuantity, out ProportionQuantityResult);
                            if (!isProportionQuantity)
                            {
                                return Ok(new { code = "201", status = false, msg = $"第{number}行理论耗用量格式有误，录入阿拉伯数字" });
                            }
                            import.ProportionQuantity = ProportionQuantityResult;

                            //单位成本
                            decimal UnitCostResult = 0M;
                            string UnitCost = dt.Rows[i]["UnitCost"].ToString();
                            if (!string.IsNullOrEmpty(UnitCost))
                            {
                                bool isUnitCost = decimal.TryParse(UnitCost, out UnitCostResult);
                                if (!isUnitCost)
                                {
                                    return Ok(new { code = "201", status = false, msg = $"第{number}行单位成本格式有误，请录入阿拉伯数字" });
                                }
                            }
                            import.UnitCost = UnitCostResult;
                        }

                        string ProductExtendName = dt.Rows[i]["ProductExtendName"].ToString();
                        string IsUse = dt.Rows[i]["IsUse"].ToString();
                        if (!string.IsNullOrEmpty(ProductExtendName) || !string.IsNullOrEmpty(IsUse))
                        {
                            //产品名称
                            if (string.IsNullOrEmpty(ProductExtendName))
                            {
                                return Ok(new { code = "201", status = false, msg = $"第{number}行产品名称不能为空" });
                            }
                            var prodyctResult2 = productList.Where(m => m.ProductName == ProductExtendName).FirstOrDefault();
                            if (prodyctResult2 != null)
                            {
                                import.ProductExtendID = prodyctResult2.ProductID;
                                import.ProductExtendName = prodyctResult2.ProductName;
                            }
                            else
                            {
                                return Ok(new { code = "201", status = false, msg = $"第{number}行产品名称不存在" });
                            }
                            //包装物
                            string PackingName = dt.Rows[i]["PackingName"].ToString();
                            var prodyctResult3 = productList.Where(m => m.ProductName == PackingName).FirstOrDefault();
                            if (prodyctResult3 != null)
                            {
                                import.PackingID = prodyctResult3.ProductID;
                                import.PackingName = prodyctResult3.ProductName;
                            }
                            //理论数量
                            decimal QuantityResult = 0M;
                            string Quantity = dt.Rows[i]["Quantity"].ToString();
                            if (!string.IsNullOrEmpty(Quantity))
                            {
                                bool isQuantity = decimal.TryParse(Quantity, out QuantityResult);
                                if (!isQuantity)
                                {
                                    return Ok(new { code = "201", status = false, msg = $"第{number}行理论数量格式有误，请录入阿拉伯数字" });
                                }
                            }
                            import.Quantity = QuantityResult;

                            if (string.IsNullOrEmpty(IsUse))
                            {
                                return Ok(new { code = "201", status = false, msg = $"第{number}行是否启用不能为空" });
                            }
                            if (IsUse != "是" && IsUse != "否")
                            {
                                return Ok(new { code = "201", status = false, msg = $"第{number}行是否启用请按是或否录入" });
                            }
                            import.IsUse = IsUse == "是" ? true : false;
                        }
                        importList.Add(import);
                    }
                    var groupList = importList.GroupBy(s => s.RowNum);
                    Result result = new Result();
                    List<Result> resultList = new List<Result>();
                    foreach (var item in groupList)
                    {
                        var model = importList.Where(s => s.RowNum == item.Key).FirstOrDefault();
                        var domain = new MS_FormulaAddCommand()
                        {
                            DataDate = model.DataDate,
                            FormulaName = model.FormulaName,
                            BaseQuantity = model.BaseQuantity,
                            EffectiveBeginDate = model.EffectiveBeginDate,
                            EffectiveEndDate = model.EffectiveEndDate,
                            UseEnterpriseList = model.UseEnterpriseList,
                            UseProductList = model.UseProductList
                        };
                        List<MS_FormulaDetailCommand> Lines = new List<MS_FormulaDetailCommand>();
                        List<MS_FormulaExtendCommand> Extends = new List<MS_FormulaExtendCommand>();
                        foreach (var line in item)
                        {
                            if (!string.IsNullOrEmpty(line.ProductID))
                            {
                                Lines.Add(new MS_FormulaDetailCommand()
                                {
                                    ProductID = line.ProductID,
                                    ProportionQuantity = line.ProportionQuantity,
                                    Quantity = line.ProportionQuantity / domain.BaseQuantity,
                                    Cost = line.UnitCost * line.ProportionQuantity,
                                    UnitCost = line.UnitCost
                                });
                            }
                            if (!string.IsNullOrEmpty(line.ProductExtendID))
                            {
                                Extends.Add(new MS_FormulaExtendCommand()
                                {
                                    ProductID = line.ProductExtendID,
                                    PackingID = line.PackingID,
                                    Quantity = line.Quantity,
                                    IsUse = line.IsUse
                                });
                            }

                        }
                        domain.Remarks = Lines.Count() > 0 ? DateTime.Now + "导入原料信息" : "";
                        domain.PackageRemarks = Extends.Count() > 0 ? DateTime.Now + "导入包装信息" : "";
                        domain.Lines = Lines;
                        domain.Extends = Extends;
                        result = _mediator.Send(domain, HttpContext.RequestAborted).Result;
                        if (result.code != 0)
                        {
                            return Ok(new { code = result.code, status = false, msg = result.msg + "参数" + JsonConvert.SerializeObject(model), });
                        }
                        else
                        {
                            resultList.Add(result);
                        }
                    }
                    if (resultList.Count == groupList.Count())
                    {
                        return Ok(new { code = "1", status = true, msg = "导入成功", });
                    }
                    else
                    {
                        return Ok(new { code = "201", status = false, msg = "导入失败", });
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(new { code = ErrorCode.RequestArgumentError.GetIntValue(), status = false, msg = "导入失败", erro = e });
            }
        }
        [HttpPost]
        [Route("Export")]
        public IActionResult Export(MS_FormulaSearch model)
        {
            model.GroupID = _identityService.GroupId;
            #region 基础配置
            //获取凭证数据
            var Propertys = new string[] { "Number", "FormulaName", "DataDate", "UseEnterpriseNames", "UseProductNames", "BaseQuantity", "EffectiveDate", "ProductName", "Specification1" ,
                                           "MeasureUnitName", "ProportionQuantity", "Quantity", "UnitCost", "Cost", "Remarks", "ProductExtendName", "Specification2", "StandardPack" ,
                                           "PackingName", "QuantityExtend", "PackageRemarks" };
            var auditColumns = new List<AuditColumn>()
                         {
                            new AuditColumn(){ Caption= "单据号",  DataField="",Columns=new List<AuditColumn>(){ }, Region =new int[]{ 0,1,0,0},ColumnIndex=0, IsEnd=false },
                            new AuditColumn(){ Caption="配方名称", DataField="",Columns=new List<AuditColumn>(){ }, Region =new int[]{ 0,1,1,1},ColumnIndex=1 , IsEnd=false  },
                            new AuditColumn(){ Caption="日期", DataField="",Columns=new List<AuditColumn>(){ }, Region =new int[]{ 0,1,2,2},ColumnIndex=2, IsEnd=false},
                            new AuditColumn(){ Caption="应用单位", DataField="",Columns=new List<AuditColumn>(){ }, Region =new int[]{ 0,1,3,3},ColumnIndex=3, IsEnd=false},
                            new AuditColumn(){ Caption="适用产品", DataField="",Columns=new List<AuditColumn>(){ }, Region =new int[]{ 0,1,4,4},ColumnIndex=4, IsEnd=false},
                            new AuditColumn(){ Caption="配方基数", DataField="",Columns=new List<AuditColumn>(){ }, Region =new int[]{ 0,1,5,5},ColumnIndex=5, IsEnd=false},
                            new AuditColumn(){ Caption="有效期间", DataField="",Columns=new List<AuditColumn>(){ }, Region =new int[]{ 0,1,6,6},ColumnIndex=6, IsEnd=false},
                            new AuditColumn(){ Caption="原料信息", DataField="",Columns=new List<AuditColumn>(){ }, Region =new int[]{ 0,0,7,14},ColumnIndex=7, IsEnd=false},
                            new AuditColumn(){ Caption="包装信息", DataField="",Columns=new List<AuditColumn>(){
                              new AuditColumn(){Caption="商品代号",DataField="",Columns=new List<AuditColumn>(){ },ColumnIndex=7, IsEnd=true },
                              new AuditColumn(){Caption="规格",DataField="",Columns=new List<AuditColumn>(){ },ColumnIndex=8, IsEnd=true },
                              new AuditColumn(){Caption="计量单位",DataField="",Columns=new List<AuditColumn>(){ },ColumnIndex=9, IsEnd=true },
                              new AuditColumn(){Caption="理论耗用量",DataField="",Columns=new List<AuditColumn>(){ },ColumnIndex=10, IsEnd=true },
                              new AuditColumn(){Caption="耗用比率",DataField="",Columns=new List<AuditColumn>(){ },ColumnIndex=11, IsEnd=true },
                              new AuditColumn(){Caption="单位成本",DataField="",Columns=new List<AuditColumn>(){ },ColumnIndex=12, IsEnd=true },
                              new AuditColumn(){Caption="成本",DataField="",Columns=new List<AuditColumn>(){ },ColumnIndex=13, IsEnd=true },
                              new AuditColumn(){Caption="备注",DataField="",Columns=new List<AuditColumn>(){ },ColumnIndex=14, IsEnd=true },
                              new AuditColumn(){Caption="产品名称",DataField="",Columns=new List<AuditColumn>(){ },ColumnIndex=15, IsEnd=true },
                              new AuditColumn(){Caption="规格",DataField="",Columns=new List<AuditColumn>(){ },ColumnIndex=16, IsEnd=true },
                              new AuditColumn(){Caption="标包",DataField="",Columns=new List<AuditColumn>(){ },ColumnIndex=17, IsEnd=true },
                              new AuditColumn(){Caption="包装物",DataField="",Columns=new List<AuditColumn>(){ },ColumnIndex=18, IsEnd=true },
                              new AuditColumn(){Caption="理论数量",DataField="",Columns=new List<AuditColumn>(){ },ColumnIndex=19, IsEnd=true },
                              new AuditColumn(){Caption="是否启用",DataField="",Columns=new List<AuditColumn>(){ },ColumnIndex=20, IsEnd=true },
                              new AuditColumn(){Caption="备注",DataField="",Columns=new List<AuditColumn>(){ },ColumnIndex=21, IsEnd=true },
                            }, Region =new int[]{ 0,0,15,21},ColumnIndex=15 , IsEnd=false}
                         };
            #endregion
            var exportDataList =  _provider.ExportDataList(model);
            if (exportDataList!=null)
            {
                IWorkbook workbook = new XSSFWorkbook();
                XSSFFont font = (XSSFFont)workbook.CreateFont();
                XSSFCellStyle style1 = (XSSFCellStyle)workbook.CreateCellStyle();
                //font.FontHeightInPoints = 14;
                style1.Alignment = HorizontalAlignment.CenterSelection;
                font.FontName = "宋体";
                //style1.VerticalAlignment = VerticalAlignment.Justify;//垂直居中 方法1 
                //font.Boldweight = (short)FontBoldWeight.Bold;//加粗
                style1.SetFont(font);
                ISheet sheet = workbook.CreateSheet("标准配方单");
                _exportCommon.SetCell(sheet, auditColumns, -1, style1);
                int rounum = 2;
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
                return File(buffer, "application/ms-excel", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "标准配方单.xlsx");
            }
            return  File(new byte[1024 * 50], "application/ms-excel", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "标准配方单.xlsx");
        }

       


    }
    
}
