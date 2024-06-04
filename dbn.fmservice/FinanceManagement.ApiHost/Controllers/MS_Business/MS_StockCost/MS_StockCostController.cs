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
    public class MS_StockCostController : ControllerBase
    {
        VoucherAmortizationUtil _comUtil;
        IMediator _mediator;
        MS_StockCostODataProvider _provider;
        IIdentityService _identityService;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;
        FMBaseCommon _baseUnit;

        public MS_StockCostController(IMediator mediator, MS_StockCostODataProvider provider, VoucherAmortizationUtil comUtil, IIdentityService identityService, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration, FMBaseCommon baseUnit)
        {
            _mediator = mediator;
            _provider = provider;
            _comUtil = comUtil;
            _identityService = identityService;
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            _baseUnit = baseUnit;
        }
        [HttpPost]
        [Route("GetReportData")]
        public List<MS_StockCostResultModel> GetReportData(MS_StockCostSearch model)
        {
           return  _provider.GetReportData(model);
        }
        [HttpGet]
        [Route("GetReportSummary")]
        public List<ReportSummary> GetReportSummary()
        {
            AuthenticationHeaderValue authentication = null;
            bool verification = AuthenticationHeaderValue.TryParse(HttpContext.Request.GetAuthToken(), out authentication);
            return _provider.GetReportSummary(authentication);
        }

    }
   
}
