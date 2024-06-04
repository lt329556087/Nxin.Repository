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

namespace FinanceManagement.ApiHost.Controllers.FM_CostProject
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FM_CostProjectController : ControllerBase
    {
        IMediator _mediator;
        FM_CostProjectODataProvider _provider;
        FundSummaryUtil _fundSummaryUtil;
        IIdentityService _identityService;

        public FM_CostProjectController(IMediator mediator, FM_CostProjectODataProvider provider, FundSummaryUtil fundSummaryUtil, IIdentityService identityService)
        {
            _mediator = mediator;
            _provider = provider;
            _fundSummaryUtil = fundSummaryUtil;
            _identityService = identityService;
        }

        [HttpGet]
        [Route("Query")]
        public async Task<Result> Query([FromQuery] FM_CostProjectCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        [HttpGet("{key}")]
        public FM_CostProjectEntity Get(string key)
        {
            var data = _provider.GetSingleData(key);
            return data;
        }

        [HttpPost]
        [Route("Submit")]
        public async Task<Result> Submit([FromBody] FM_CostProjectAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        [HttpPost]
        [Route("Update")]
        public async Task<Result> Update([FromBody] FM_CostProjectModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<Result> Delete([FromBody] FM_CostProjectDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        [HttpDelete]
        [Route("BatchDelete")]
        public async Task<Result> BatchDelete([FromBody] FM_CostProjectBatchDelCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        [HttpPost]
        [Route("BatchAdd")]
        public async Task<Result> BatchAdd([FromBody] FM_CostProjectBatchAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
    }
}
