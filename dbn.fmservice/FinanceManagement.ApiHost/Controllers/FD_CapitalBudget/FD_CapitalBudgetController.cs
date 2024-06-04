using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_AccountTransfer;
using FinanceManagement.ApiHost.Controllers.FD_CapitalBudget;
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

namespace FinanceManagement.ApiHost.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_CapitalBudgetController : ControllerBase
    {
        IMediator _mediator;
        FD_CapitalBudgetODataProvider _provider;
        AuditUtil _auditUtil;
        private readonly ILogger<FD_CapitalBudgetController> _logger;
        private IIdentityService _identityService;

        public FD_CapitalBudgetController(IMediator mediator, FD_CapitalBudgetODataProvider provider, AuditUtil auditUtil, ILogger<FD_CapitalBudgetController> logger, IIdentityService identityService)
        {
            _mediator = mediator;
            _provider = provider;
            _auditUtil = auditUtil;
            _logger = logger;
            _identityService = identityService;
        }

        [HttpPost]
        [Route("GetDuration")]
        public Result<List<DurationInfo>> GetDuration()
        {
            var result = new Result<List<DurationInfo>>();
            var curWeek = Utility.WeekOfYear(DateTime.Now.AddDays(7));
            var duration = new List<DurationInfo>();
            DateTime firstDay = Utility.CalculateFirstDateOfWeek(DateTime.Now.Date.AddDays(7));
            DateTime lastDay = Utility.CalculateLastDateOfWeek(DateTime.Now.Date.AddDays(7));

            for (int i = 1; i <= curWeek; i++)
            {
                string startDate = firstDay.AddDays((i - curWeek) * 7).ToString("yyyy-MM-dd");
                string endDate = lastDay.AddDays((i - curWeek) * 7).ToString("yyyy-MM-dd");
                duration.Add(new DurationInfo
                {
                    Description = string.Format("从{0}到{1}[第{2}周]", startDate, endDate, i),
                    Sort = i,
                    StartDate = startDate,
                    EndDate = endDate,
                });
            }

            result.data = duration;
            return result;
        }

        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("{key}")]
        public async Task<Result> GetDetail(long key)
        {
            var result = new Result();
            var data = await _provider.GetSingleDataAsync(key);
            if (DateTime.TryParse(data.EndDate, out DateTime et))
            {
                data.Duration = "第" + Utility.WeekOfYear(et) + "周";
            }
            _logger.LogInformation("资金周计划");
            result.data = data;
            return result;
        }

        [HttpGet]
        [Route("GetEmptyModel")]
        public Result GetEmptyModel()
        {
            var result = new Result();
            var model = new Domain.FD_CapitalBudget();
            model.EnterpriseID = _identityService.EnterpriseId;
            result.data = new Domain.FD_CapitalBudget();
            return result;
        }

        //增加
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FD_CapitalBudgetAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        //[PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FD_CapitalBudgetDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        public async Task<Result> Modify([FromBody] FD_CapitalBudgetModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
    }
}
