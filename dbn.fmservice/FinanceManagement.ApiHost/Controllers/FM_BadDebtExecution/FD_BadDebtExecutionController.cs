using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_BadDebtExecution;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_BadDebtExecution
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_BadDebtExecutionController : ControllerBase
    {
        IMediator _mediator;
        FD_BadDebtExecutionODataProvider _provider;
        IFD_BadDebtExecutionRepository _exeRepository;

        public FD_BadDebtExecutionController(IMediator mediator, FD_BadDebtExecutionODataProvider provider, IFD_BadDebtExecutionRepository exeRepository)
        {
            _mediator = mediator;
            _provider = provider;
            _exeRepository = exeRepository;
        }

        //增加
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FD_BadDebtExecutionAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FD_BadDebtExecutionDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //是否生成过凭证
        [HttpGet("{key}")]
        public Result IsExist(string key)
        {
            var result = new Result();
            result.code = _exeRepository.IsExist(key) ? 1 : 0;
            return result;
        }

    }
}
