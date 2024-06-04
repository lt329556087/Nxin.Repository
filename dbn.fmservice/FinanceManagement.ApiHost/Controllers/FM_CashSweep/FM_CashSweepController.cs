using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FM_CashSweep;
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
namespace FinanceManagement.ApiHost.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FM_CashSweepController : ControllerBase
    {
        IMediator _mediator;
        FM_CashSweepODataProvider _provider;
        private readonly ILogger<FM_CashSweepController> _logger;
        private IIdentityService _identityService;
        private IFM_CashSweepRepository _repository;
        private FinanceTradeUtil _financeTradeUtil;
        private FundSummaryUtil _fundSummaryUtil;
        HostConfiguration _hostCongfiguration;
        public FM_CashSweepController(IMediator mediator, FM_CashSweepODataProvider provider, ILogger<FM_CashSweepController> logger, IIdentityService identityService,
            HostConfiguration hostCongfiguration,
            IFM_CashSweepRepository repository,
            FinanceTradeUtil financeTradeUtil,
            FundSummaryUtil fundSummaryUtil
            )
        {
            _mediator = mediator;
            _provider = provider;
            _logger = logger;
            _identityService = identityService;
            _repository = repository;
            _financeTradeUtil = financeTradeUtil;
            _fundSummaryUtil = fundSummaryUtil;
            _hostCongfiguration = hostCongfiguration;
        }


        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("{key}")]
        [PermissionAuthorize(Permission.Retrieve)]
        public async Task<Result> GetDetail(long key)
        {
            var result = new Result();
            var data = await _provider.GetSingleDataAsync(key);
            if (data?.Lines?.Count > 0)
            {
                // 批量获取单据状态
                var workflowStates = await _provider.GetWorkflowStatesAsync(String.Join(',', data.Lines.Select(p => p.NumericalOrder)));
                data = _provider.GetAuditResult(data,workflowStates);
                if (!data.IsNew)
                {
                    //转归集状态 新旧菜单金融接口返回状态值不同，需要转换
                    if (data.TradeResult == "归集成功")
                    {
                        data.TradeResult = "归集失败";
                    }
                    else if (data.TradeResult == "归集失败")
                    {
                        data.TradeResult = "归集成功";
                    }
                }
                //自动归集 归集金额为公式
                if (data.SweepType == "1811191754180000202")
                {
                    data.Lines?.ForEach(p => p.AutoSweepBalance_Show = data.SchemeFormula);
                }
                EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
                foreach (var item in data.Lines)
                {
                    var resultNum = encryptAccount.AccountNumberDecrypt(item.AccountNumber);
                    item.AccountNumber = resultNum?.Item1 == true ? resultNum.Item2 : item.AccountNumber;
                }
            }
            result.data = data;
            result.code = ErrorCode.Success.GetIntValue();
            return result;
        }

        //增加
        [HttpPost]
        [AllowAnonymous]
        [PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FM_CashSweepAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        [PermissionAuthorize(Permission.Making)]
        public async Task<Result> Delete([FromBody] FM_CashSweepDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        [PermissionAuthorize(Permission.Making)]
        public async Task<Result> Modify([FromBody] FM_CashSweepModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //实时余额
        [HttpPost]
        [Route("GetYqtBal")]
        public Result GetYqtBal(string acc_no, string bank_type, string client_id)
        {
            var result = new Result();
            var balResult = _financeTradeUtil.GetYqtBal(acc_no, bank_type, client_id);
            if (balResult == null)
            {
                result.msg = "获取余额返回空";
            }
            else
            {
                if (string.IsNullOrEmpty(balResult.Item1))
                {
                    result.code = ErrorCode.Success.GetIntValue();
                }
                else
                {
                    result.msg = balResult.Item1;
                }
                result.data = balResult.Item2;
            }
            return result;
        }
        //备用金设置
        [HttpPost]
        [Route("GetRevolvingFundData")]
        public async Task<RevolvingFundDetailODataEntity> GetRevolvingFundData([FromBody] RevolvingFundDetailRequest request)
        {
            var result = new RevolvingFundDetailODataEntity();
            var list = await _provider.GetRevolvingFundData(request);
            result = list?.FirstOrDefault();
            return result;
        }
        //资金汇总表
        [HttpPost]
        [Route("GetFundsSummaryReport")]
        public async Task<Result> GetFundsSummaryReport([FromBody] FundSummaryRequest request)
        {
            var result = new Result();
            if (string.IsNullOrEmpty(request.EnterpriseId))
            {
                request.EnterpriseId = _identityService.EnterpriseId;
            }
            if (string.IsNullOrEmpty(request.GroupId))
            {
                request.GroupId = _identityService.GroupId;
            }
            if (string.IsNullOrEmpty(request.BeginDate))
            {
                result.msg = "BeginDate不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(request.EndDate))
            {
                result.msg = "EndDate不能为空";
                return result;
            }
            request.FilterAccount = true;
            request.OpenBankEnterConnect = false;
            var data = await _fundSummaryUtil.GetData(request);
            decimal amount = 0;
            if (data?.Count > 0)
            {
                amount = data.Sum(p => p.EnddingAmount);
            }
            result.data = amount;
            result.code = ErrorCode.Success.GetIntValue();
            return result;
        }
    }
}
