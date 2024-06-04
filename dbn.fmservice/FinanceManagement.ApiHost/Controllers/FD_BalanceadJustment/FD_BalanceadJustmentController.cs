using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_BalanceadJustment;
using FinanceManagement.Common;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_BalanceadJustment
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_BalanceadJustmentController : ControllerBase
    {
        BankAccountBalanceUtil _comUtil;
        IMediator _mediator;
        FD_BalanceadJustmentODataProvider _provider;
        IIdentityService _identityService;

        public FD_BalanceadJustmentController(IMediator mediator, FD_BalanceadJustmentODataProvider provider, BankAccountBalanceUtil comUtil, IIdentityService identityService)
        {
            _mediator = mediator;
            _provider = provider;
            _comUtil = comUtil;
            _identityService = identityService;
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
            result.data = data;
            return result;
        }

        //增加
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FD_BalanceadJustmentAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        //[PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FD_BalanceadJustmentDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        public async Task<Result> Modify([FromBody] FD_BalanceadJustmentModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        [HttpPost]
        [Route("GetAccountData")]
        public async Task<FD_BalanceadJustmentModel> GetAccountData([FromBody] FD_BalanceadJustmentSearchCommand request)
        {
            try
            {
                FD_BalanceadJustmentModel fD_BalanceadJustment = new FD_BalanceadJustmentModel();
                fD_BalanceadJustment.AccountID = request.AccountID;
                #region 资金日记账报表参数
                request.Account_id = request.AccountID;
                request.Begindate = request.DataDate;
                request.Boid = _identityService.UserId;
                request.CanWatchEntes = new List<string>() { _identityService.EnterpriseId };
                request.Enddate = request.DataDate;
                request.EnteID = Convert.ToInt64(_identityService.EnterpriseId);
                request.EnteId = _identityService.EnterpriseId;
                request.MenuParttern = "0";
                request.OwnEntes = new List<string>() { _identityService.EnterpriseId };
                request.Reviewed = "2";
                #endregion
                var result = await _comUtil.GetCapitalJournalReport(request);
                if (result != null && result.Data?.Count >= 2)
                {
                    fD_BalanceadJustment.Balance = result.Data[result.Data.Count - 2].Balance;
                }
                var accountFirstData = await _comUtil.GetFMAccountData(new FMAccount() { AccountID = request.AccountID });
                bool OpenBankEnterConnect = Convert.ToBoolean(accountFirstData.Data?.FirstOrDefault().OpenBankEnterConnect);
                if (OpenBankEnterConnect)
                {
                    var result1 = _comUtil.GetYqtAccountBalInfo(accountFirstData.Data?.FirstOrDefault());
                    if (result1 != null)
                    {
                        fD_BalanceadJustment.AcctBal = Convert.ToDecimal(result1.acctBal);
                    }
                    fD_BalanceadJustment.IsOpenBankEnterConnect = true;
                }
                else
                {
                    fD_BalanceadJustment.IsOpenBankEnterConnect = false;
                    fD_BalanceadJustment.AcctBal = 0;
                }

                return fD_BalanceadJustment;
            }
            catch (Exception ex)
            {
                return new FD_BalanceadJustmentModel();
            }

        }

        [HttpGet]
        [Route("GetEmptyModel")]
        public List<FD_BalanceadJustmentDetailCommand> GetEmptyModel()
        {
            return _provider.GetEmptyModel();
        }
        /// <summary>
        /// 打印查询
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PrintData")]
        public async Task< Result> PrintData(List<string> keys)
        {
            var result = new Result();
            List<FD_BalanceadJustmentODataEntity> list = new List<FD_BalanceadJustmentODataEntity>();
            foreach (var key in keys)
            {
                FD_BalanceadJustmentODataEntity data =await  _provider.GetSingleDataAsync(Convert.ToInt64(key));
                list.Add(data);
            }
            result.data = list;
            return result;
        }
    }
}
