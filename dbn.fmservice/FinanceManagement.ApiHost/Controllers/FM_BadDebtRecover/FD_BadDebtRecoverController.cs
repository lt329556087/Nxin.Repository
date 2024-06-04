using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_BadDebtRecover;
using FinanceManagement.Common;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
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

namespace FinanceManagement.ApiHost.Controllers.FD_BadDebtRecover
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_BadDebtRecoverController : ControllerBase
    {
        IMediator _mediator;
        FD_BadDebtRecoverODataProvider _provider;
        FD_BaddebtSettingODataProvider _settingProvider;
        SettleReceiptUtil _settleReceiptUtil;
        IIdentityService _identityService;
        FMBaseCommon _baseUtil;
        IFD_BadDebtExecutionRepository _exeRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        ReceiptExecutionUtil _exeUtil;

        public FD_BadDebtRecoverController(IMediator mediator, FD_BadDebtRecoverODataProvider provider, FD_BaddebtSettingODataProvider settingProvider, SettleReceiptUtil settleReceiptUtil,
            IIdentityService identityService,
            FMBaseCommon baseUtil,
            IFD_BadDebtExecutionRepository exeRepository,
            IBiz_ReviewRepository biz_ReviewRepository,
                    ReceiptExecutionUtil exeUtil
            )
        {
            _mediator = mediator;
            _provider = provider;
            _settingProvider = settingProvider;
            _settleReceiptUtil = settleReceiptUtil;
            _baseUtil = baseUtil;
            _exeRepository = exeRepository;
            _identityService = identityService;
            _biz_ReviewRepository = biz_ReviewRepository;
            _exeUtil = exeUtil;
        }
        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("{key}")]
        public Result GetDetail(long key)
        {
            var result = new Result();
            var data = _provider.GetSingleDataAsync(key).Result;
            data.Lines1 = data.Lines.Where(o => data.AccoSubjectID1 == o.AccoSubjectID).ToList();
            data.Lines2 = data.Lines.Where(o => data.AccoSubjectID2 == o.AccoSubjectID).ToList();
            result.data = data;
            return result;
        }

        //增加
        [HttpPost, Route("GetEmptyModel")]
        public Result GetEmptyModel(FD_BadDebtRecoverAddCommand request)
        {
            var result = new Result();
            var recoverModel = new FD_BadDebtRecoverODataEntity();
            var enterpriseId = Convert.ToInt64(_identityService.EnterpriseId);
            var settinModel = _settingProvider.GetDataByEnterId(enterpriseId, request.DataDate);
            if (settinModel != null)
            {
                recoverModel.AccoSubjectID1 = settinModel.BadAccoSubjectOne;
                recoverModel.AccoSubjectName1 = "坏账收回/应收账款坏账收回";
                recoverModel.AccoSubjectID2 = settinModel.BadAccoSubjectTwo;
                recoverModel.AccoSubjectName2 = "坏账收回/其他应收账款坏账收回";
                recoverModel.NumericalOrderSetting = settinModel.NumericalOrder;
            }

            recoverModel.Lines1.Add(new FD_BadDebtRecoverDetailODataEntity() { AccoSubjectID = recoverModel.AccoSubjectID1 });
            recoverModel.Lines2.Add(new FD_BadDebtRecoverDetailODataEntity() { AccoSubjectID = recoverModel.AccoSubjectID2 });
            recoverModel.DataDate = request.DataDate;
            recoverModel.CustomerID = request.CustomerID;
            recoverModel.PersonID = request.PersonID;
            recoverModel.BusinessType = request.BusinessType;

            result.data = recoverModel;
            return result;
        }


        //增加
        [HttpPost, Route("CreateSettleReceipt")]
        public async Task<RpcResult<ReceiptResult>> CreateSettleReceipt([FromBody] FD_BadDebtRecoverAddCommand request)
        {
            try
            {
                var enterpriseId = Convert.ToInt64(_identityService.EnterpriseId);
                var recoverModel = _provider.GetSingleDataAsync(Convert.ToInt64(request.NumericalOrder)).Result;
                //var settingModel = _settingProvider.GetDataByEnterId(enterpriseId, recoverModel.DataDate);
                var settingModel = new FD_BaddebtSettingODataEntity();// await _settingProvider.GetSingleDataAsync(Convert.ToInt64(recoverModel.NumericalOrderSetting));
                //var result = await SaveReceipt(recoverModel, settingModel, settingModel.BadAccoSubjectOne, recoverModel.CAccoSubjectID);

                var receiptResult = new RpcResult<ReceiptResult>();
                var amount1 = recoverModel.Lines.Where(o => o.AccoSubjectID == recoverModel.AccoSubjectID1).Sum(o => o.CurrentRecoverAmount);
                if (amount1 > 0)
                {
                    ////第一步：冲抵坏账准备-- - 金额为负值
                    //借：信用减值损失 / 应收账款减值(取值为坏账参数中信用减值损失科目)
                    //贷：坏账准备 / 应收账款坏账准备(取值为坏账参数中坏账准备科目)
                    await SaveReceipt(recoverModel, settingModel, settingModel.OtherAccoSubjectOne, settingModel.BadAccoSubjectOne, -Math.Abs(amount1));

                    //第二步：冲抵坏账发生（取值为坏账参数中坏账冲销摘要）
                    //借：应收账款(取值为坏账参数中应收账款科目)
                    //贷：坏账准备 / 应收账款坏账准备(取值为坏账参数中坏账准备科目)
                    await SaveReceipt(recoverModel, settingModel, settingModel.DebtReceAccoSubjectOne, settingModel.BadAccoSubjectOne, amount1);

                    //第三步：收回账款（取值为坏账参数中坏账收回摘要）
                    //借：银行存款(取值为坏账参数中收款科目)
                    //贷：应收账款(取值为坏账参数中应收账款科目)
                    receiptResult = await SaveReceipt(recoverModel, settingModel, settingModel.ReceAccoSubjectOne, settingModel.DebtReceAccoSubjectOne, amount1);
                }

                var amount2 = recoverModel.Lines.Where(o => o.AccoSubjectID == recoverModel.AccoSubjectID2).Sum(o => o.CurrentRecoverAmount);
                if (amount2 > 0)
                {
                    ////第一步：冲抵坏账准备-- - 金额为负值
                    //借：信用减值损失 / 应收账款减值(取值为坏账参数中信用减值损失科目)
                    //贷：坏账准备 / 应收账款坏账准备(取值为坏账参数中坏账准备科目)
                    await SaveReceipt(recoverModel, settingModel, settingModel.OtherAccoSubjectTwo, settingModel.BadAccoSubjectTwo, -Math.Abs(amount2));

                    //第二步：冲抵坏账发生（取值为坏账参数中坏账冲销摘要）
                    //借：应收账款(取值为坏账参数中应收账款科目)
                    //贷：坏账准备 / 应收账款坏账准备(取值为坏账参数中坏账准备科目)
                    await SaveReceipt(recoverModel, settingModel, settingModel.DebtReceAccoSubjectTwo, settingModel.BadAccoSubjectTwo, amount2);

                    //第三步：收回账款（取值为坏账参数中坏账收回摘要）
                    //借：银行存款(取值为坏账参数中收款科目)
                    //贷：应收账款(取值为坏账参数中应收账款科目)
                    receiptResult = await SaveReceipt(recoverModel, settingModel, settingModel.ReceAccoSubjectTwo, settingModel.DebtReceAccoSubjectTwo, amount2);
                }
                return receiptResult;
            }
            catch (Exception ex)
            {
                return new RpcResult<ReceiptResult>() { code = 0 };
            }
        }

        private async Task<RpcResult<ReceiptResult>> SaveReceipt(FD_BadDebtRecoverODataEntity recoverModel, FD_BaddebtSettingODataEntity settingModel, string debitAccoSubjectID, string creditAccoSubjectID, decimal amount)
        {
            var recetiptData = new ReceiptData();
            recetiptData.DataA.TicketedPointID = recoverModel.TicketedPointID;
            recetiptData.DataA.SettleReceipType = ((long)SettleReceiptType.转账凭证).ToString();
            recetiptData.DataA.EnterpriseID = recoverModel.EnterpriseID;
            recetiptData.DataA.DataDate = recoverModel.DataDate;

            var receiptDebit = new SettleReceiptDetail();
            receiptDebit.LorR = 0;//借方
            receiptDebit.ReceiptAbstractID = settingModel.RecoverReceiptAbstractID;
            receiptDebit.Debit = amount;
            receiptDebit.AccoSubjectID = debitAccoSubjectID;
            var subjectCodeDebit = _baseUtil.GetEnterSubjectList(Convert.ToInt64(receiptDebit.AccoSubjectID), Convert.ToInt64(_identityService.EnterpriseId), recoverModel.DataDate).Result;
            if (subjectCodeDebit != null)
            {
                var code = subjectCodeDebit.FirstOrDefault(o => o.AccoSubjectID == receiptDebit.AccoSubjectID);
                receiptDebit.AccoSubjectCode = code?.cAccoSubjectCode;
                if (!string.IsNullOrEmpty(recoverModel.CustomerID))
                {
                    receiptDebit.CustomerID = recoverModel.CustomerID;
                }
            }

            var receiptCredit = new SettleReceiptDetail();
            receiptCredit.LorR = 1;//贷方
            receiptCredit.ReceiptAbstractID = settingModel.RecoverReceiptAbstractID;
            receiptCredit.Credit = amount;
            receiptCredit.AccoSubjectID = creditAccoSubjectID;
            recetiptData.DataA.Remarks = "坏账收回";
            var subjectCodeCredit = _baseUtil.GetEnterSubjectList(Convert.ToInt64(receiptCredit.AccoSubjectID), Convert.ToInt64(_identityService.EnterpriseId), recoverModel.DataDate).Result;
            if (subjectCodeCredit != null)
            {
                var code = subjectCodeCredit.FirstOrDefault(o => o.AccoSubjectID == receiptCredit.AccoSubjectID);
                receiptCredit.AccoSubjectCode = code.cAccoSubjectCode;

                if (!string.IsNullOrEmpty(recoverModel.CustomerID))
                {
                    receiptCredit.CustomerID = recoverModel.CustomerID;
                }
            }
            recetiptData.DataB.Add(receiptDebit);
            recetiptData.DataB.Add(receiptCredit);

            var resultReceipt = await _exeUtil.AfterSaveReceipt(recetiptData, recoverModel.NumericalOrder, _identityService.AppId, _identityService.EnterpriseId, _identityService.UserId);
            return resultReceipt;

        }

        //增加
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FD_BadDebtRecoverAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        //[PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FD_BadDebtRecoverDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        public async Task<Result> Modify([FromBody] FD_BadDebtRecoverModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
    }
}
