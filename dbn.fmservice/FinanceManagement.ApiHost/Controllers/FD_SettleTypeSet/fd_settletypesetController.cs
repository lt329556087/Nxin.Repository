using Architecture.Common.HttpClientUtil;
using Architecture.Common.NumericalOrderCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_Payment;
using FinanceManagement.Common;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
//凭证设置 + 会计凭证 类别自定义
namespace FinanceManagement.ApiHost.Controllers.FD_Payment
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class fd_settletypesetController : ControllerBase
    {
        IMediator _mediator;
        //fd_schedulesetTODataProvider _provider;
        FundSummaryUtil _fundSummaryUtil;
        IIdentityService _identityService;
        fd_settletypesetODataProvider _prodiver;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;
        private readonly ILogger<fd_settletypesetController> _logger;
        IFD_PaymentExtendRepository _paymentExtendRepository;
        NumericalOrderCreator _numericalOrderCreator;
        public fd_settletypesetController(IMediator mediator, HttpClientUtil httpClientUtil,
            //fd_schedulesetTODataProvider provider, 
            FundSummaryUtil fundSummaryUtil, IFD_PaymentExtendRepository paymentExtendRepository, NumericalOrderCreator numericalOrderCreator, IIdentityService identityService, fd_settletypesetODataProvider prodiver, ILogger<fd_settletypesetController> logger, HostConfiguration hostCongfiguration)
        {
            _mediator = mediator;
            //_provider = provider;
            _fundSummaryUtil = fundSummaryUtil;
            _identityService = identityService;
            _prodiver = prodiver;
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            _logger = logger;
            _paymentExtendRepository = paymentExtendRepository;
            _numericalOrderCreator = numericalOrderCreator;
        }

        //通用保存接口（全部删除 重加）
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] fd_settletypesetSaveCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //通用删除接口
        [HttpDelete]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Delete([FromBody] fd_settletypesetRemoveCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        [HttpPost]
        [Route("GetData")]
        public fd_settletypeset GetData(string GroupId)
        {
            return _prodiver.GetData(GroupId);
        }
        [HttpPost]
        [Route("GetSettleType")]
        public dynamic GetSettleType()
        {
            return _prodiver.GetSettleType();
        }
        //通用保存接口（全部删除 重加）
        [HttpPost]
        [Route("AddSettleType")]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> AddSettleType([FromBody] fd_settletypeSaveCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
    }
}
