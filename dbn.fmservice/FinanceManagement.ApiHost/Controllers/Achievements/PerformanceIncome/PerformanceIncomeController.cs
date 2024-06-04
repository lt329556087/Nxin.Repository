using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using Aspose.Cells;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.Common;
using FinanceManagement.Common.MonthEndCheckout;
using FinanceManagement.Common.NewMakeVoucherCommon;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.PerformanceIncome
{

    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceIncomeController : ControllerBase
    {
        VoucherAmortizationUtil _comUtil;
        IMediator _mediator;
        PerformanceIncomeODataProvider _provider;
        IIdentityService _identityService;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;
        FMAPIService FMAPIService;
        FMBaseCommon _baseUnit;

        public PerformanceIncomeController(IMediator mediator, FMAPIService FMAPIService, FMBaseCommon baseUnit, PerformanceIncomeODataProvider provider, VoucherAmortizationUtil comUtil, IIdentityService identityService, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _baseUnit = baseUnit;
            _mediator = mediator;
            _provider = provider;
            _comUtil = comUtil;
            _identityService = identityService;
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            this.FMAPIService = FMAPIService;
        }
        [HttpPost]
        [Route("GetList")]
        public Result<List<PerformanceIncomeEntity>> GetList(PerformanceIncomeSearch model)
        {
            Result<List<PerformanceIncomeEntity>> result = new Result<List<PerformanceIncomeEntity>>();
            try
            {
                result.msg = "成功";
                result.data = _provider.GetList(model);
                return result;
            }
            catch (Exception ex)
            {
                result.msg = "保存异常";
                result.data = new List<PerformanceIncomeEntity>();
                return result;
            }
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
                    return null;
                }
                using (Stream fs = file.OpenReadStream())
                {
                    Workbook workbook = new Workbook(fs);
                    Worksheet sheet = workbook.Worksheets[0];
                    Cells cells = sheet.Cells;
                    DataTable dt = cells.ExportDataTable(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);
                    Dictionary<string, string> columDic = new Dictionary<string, string>()
                    {
                        {"商品名称ID","ProductGroupID"},
                        {"商品名称","ProductGroupName"},
                        {"商品分类","ProductGroupTypeName"},
                        {"收入分类","IncomeTypeName"},
                        {"三号文分类","ParentTypeName"},
                        {"匹配属性","PropertyName"},
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

                    FM_PerformanceIncomeAddCommand import = new FM_PerformanceIncomeAddCommand();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        FM_PerformanceIncomeAddCommand model = new FM_PerformanceIncomeAddCommand()
                        {
                            ProductGroupID = dt.Rows[i]["ProductGroupID"].ToString(),
                            ProductGroupName = dt.Rows[i]["ProductGroupName"].ToString(),
                            ProductGroupTypeName = dt.Rows[i]["ProductGroupTypeName"].ToString(),
                            IncomeTypeName = dt.Rows[i]["IncomeTypeName"].ToString(),
                            ParentTypeName = dt.Rows[i]["ParentTypeName"].ToString(),
                            PropertyName = dt.Rows[i]["PropertyName"].ToString(),
                            EnterpriseID = _identityService.GroupId.ToString(),
                        };
                        import.List.Add(model);
                    }
                    var result = _mediator.Send(import, HttpContext.RequestAborted).Result;
                }
                return Ok(new { code = ErrorCode.RequestArgumentError.GetIntValue(), status = false, msg = "导入成功" });
            }
            catch (Exception e)
            {
                return Ok(new { code = ErrorCode.RequestArgumentError.GetIntValue(), status = false, msg = "导入失败", erro = e });
            }
        }

        [HttpPost]
        [Route("GetSaleAmount")]
        public List<dynamic> GetSaleAmount(BusinessSearch model)
        {
           
            try
            {
               return _provider.GetSaleAmount(model);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpPost]
        [Route("GetCostAmount")]
        public List<dynamic> GetCostAmount(BusinessSearch model)
        {

            try
            {
                return _provider.GetCostAmount(model);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpPost]
        [Route("GetBoidBySort")]
        public List<dynamic> GetBoidBySort(BusinessSearch model)
        {

            try
            {
                return _provider.GetBoidBySort(model);
            }
            catch (Exception ex)
            {
                return null;
            }
        }



    }
}
