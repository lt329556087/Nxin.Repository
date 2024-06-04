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

namespace FinanceManagement.ApiHost.Controllers.FM_PigOriginalAssets
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FM_PigOriginalAssetsController : ControllerBase
    {
        IMediator _mediator;
        FM_PigOriginalAssetsODataProvider _provider;
        FundSummaryUtil _fundSummaryUtil;
        IIdentityService _identityService;

        public FM_PigOriginalAssetsController(IMediator mediator, FM_PigOriginalAssetsODataProvider provider, FundSummaryUtil fundSummaryUtil, IIdentityService identityService)
        {
            _mediator = mediator;
            _provider = provider;
            _fundSummaryUtil = fundSummaryUtil;
            _identityService = identityService;
        }

        [HttpGet]
        [Route("Query")]
        public async Task<Result> Query([FromQuery] FM_PigOriginalAssetsCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        [HttpGet("{key}")]
        public FM_PigOriginalAssetsEntity Get(string key)
        {
            var data = _provider.GetSingleData(key);
            return data;
        }

        [HttpPost]
        [Route("Submit")]
        public async Task<Result> Submit([FromBody] FM_PigOriginalAssetsAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        [HttpPost]
        [Route("Update")]
        public async Task<Result> Update([FromBody] FM_PigOriginalAssetsModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<Result> Delete([FromBody] FM_PigOriginalAssetsDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        [HttpDelete]
        [Route("BatchDelete")]
        public async Task<Result> BatchDelete([FromBody] FM_PigOriginalAssetsBatchDelCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
    }
}
