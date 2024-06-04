using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_AccountInventory;
using FinanceManagement.Common;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FM_Expensereport
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FM_ExpensereportController : ControllerBase
    {
        IMediator _mediator;
        FM_ExpensereportODataProvider _provider;
        FundSummaryUtil _fundSummaryUtil;
        IIdentityService _identityService;

        public FM_ExpensereportController(IMediator mediator, FM_ExpensereportODataProvider provider, FundSummaryUtil fundSummaryUtil, IIdentityService identityService)
        {
            _mediator = mediator;
            _provider = provider;
            _fundSummaryUtil = fundSummaryUtil;
            _identityService = identityService;
        }

        [HttpGet]
        [Route("Query")]
        public async Task<Result> Query([FromQuery] FM_ExpensereportCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        [HttpGet("{key}")]
        public FM_ExpensereportEntity Get(string key)
        {
            var data = _provider.GetSingleData(key);
            return data;
        }
        [HttpGet("{key}/{reportPeriod}")]
        public FM_ExpensereportEntity Get(string key,string reportPeriod)
        {
            var data = _provider.GetSingleData(key,reportPeriod);
            return data;
        }

        [HttpPost]
        [Route("Submit")]
        public async Task<Result> Submit([FromBody] FM_ExpensereportAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        [HttpPost]
        [Route("Update")]
        public async Task<Result> Update([FromBody] FM_ExpensereportModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<Result> Delete([FromBody] FM_ExpensereportDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        [HttpDelete]
        [Route("BatchDelete")]
        public async Task<Result> BatchDelete([FromBody] FM_ExpensereportBatchDelCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        [HttpGet]
        [Route("QuerySummaryReport")]
        public async Task<Result> QuerySummaryReport([FromQuery] FM_ExpensereportSummaryCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        [HttpGet]
        [Route("QueryReportLogs")]
        public async Task<Result> QueryReportLogs([FromQuery] FM_ExpensereportLogsCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        /// <summary>
        /// 手动录入归集
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("FillData")]
        public async Task<Result> FillData([FromBody] FM_ExpensereportFillCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        /// <summary>
        /// 问题追溯
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("QueryReportDetailLogs")]
        public async Task<Result> QueryReportDetailLogs([FromQuery] FM_ExpensereportDetailLogsCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
    }
}
