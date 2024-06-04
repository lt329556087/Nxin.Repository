using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_BadDebtOccur;
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

namespace FinanceManagement.ApiHost.Controllers.FD_BadDebtOccur
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_BadDebtOccurController : ControllerBase
    {
        IMediator _mediator;
        FD_BadDebtOccurODataProvider _provider;
        FD_BaddebtSettingODataProvider _settingProvider;
        IIdentityService _identityService;
        AgingDataUtil _agingDataUtil;
        SettleReceiptUtil _settleReceiptUtil;
        FMBaseCommon _baseUtil;
        ReceiptExecutionUtil _exeUtil;
        IFD_BadDebtExecutionRepository _exeRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        RptAgingReclassificationUtil _agingReclassificationUtil;

        public FD_BadDebtOccurController(IMediator mediator, FD_BadDebtOccurODataProvider provider, FD_BaddebtSettingODataProvider settingProvider, IIdentityService identityService,
            AgingDataUtil agingDataUtil,
            SettleReceiptUtil settleReceiptUtil,
            FMBaseCommon baseUtil,
            IFD_BadDebtExecutionRepository exeRepository,
            IBiz_ReviewRepository biz_ReviewRepository,
            ReceiptExecutionUtil exeUtil,
            RptAgingReclassificationUtil agingReclassificationUtil
            )
        {
            _mediator = mediator;
            _provider = provider;
            _settingProvider = settingProvider;
            _identityService = identityService;
            _agingDataUtil = agingDataUtil;
            _settleReceiptUtil = settleReceiptUtil;
            _baseUtil = baseUtil;
            _exeRepository = exeRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
            _exeUtil = exeUtil;
            _agingReclassificationUtil = agingReclassificationUtil;

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
        public Result GetEmptyModel([FromBody] FD_BadDebtOccurCommand request)
        {
            var result = new Result();
            var occurModel = new FD_BadDebtOccurODataEntity();
            var enterpriseId = Convert.ToInt64(_identityService.EnterpriseId);

            var subjectCodeList = _baseUtil.GetEnterSubjectList(0, enterpriseId, occurModel.DataDate).Result;
            //应收账款
            occurModel.AccoSubjectID1 = subjectCodeList.FirstOrDefault(o => o.cAccoSubjectCode == "1122")?.AccoSubjectID;
            occurModel.AccoSubjectName1 = subjectCodeList.FirstOrDefault(o => o.cAccoSubjectCode == "1122")?.cAccoSubjectFullName;
            //其他应收款
            occurModel.AccoSubjectID2 = subjectCodeList.FirstOrDefault(o => o.cAccoSubjectCode == "1221")?.AccoSubjectID;
            occurModel.AccoSubjectName2 = subjectCodeList.FirstOrDefault(o => o.cAccoSubjectCode == "1221")?.cAccoSubjectFullName;
            var dataDate = request.DataDate;//单据日期

            decimal amount1 = 0, amount2 = 0;
            if (!string.IsNullOrEmpty(request.DataDate) && (!string.IsNullOrEmpty(request.CustomerID) || !string.IsNullOrEmpty(request.PersonID)))
            {
                //应收账款
                var request1 = new RptAgingReclassificationRequest();
                request1.CustomerType = request.BusinessType == "201611160104402101" ? "1" : "2";
                request1.CustomerID = request.BusinessType == "201611160104402101" ? request.CustomerID : null;
                request1.PersonID = request.BusinessType == "201611160104402103" ? request.PersonID : null;
                request1.Enddate = request.DataDate;
                request1.AccountingSubjectsID = "1122";
                amount1 = GetReclassification(request1);
                //其他应收款
                var request2 = new RptAgingReclassificationRequest();
                request2.CustomerType = request.BusinessType == "201611160104402101" ? "1" : "2";
                request2.CustomerID = request.BusinessType == "201611160104402101" ? request.CustomerID : null;
                request2.PersonID = request.BusinessType == "201611160104402103" ? request.PersonID : null;
                request2.Enddate = request.DataDate;
                request2.AccountingSubjectsID = "1221";
                amount2 = GetReclassification(request2);
            }
            occurModel.Lines1.Add(new FD_BadDebtOccurDetailODataEntity() { Amount = amount1, CurrentOccurAmount = amount1, AccoSubjectID = occurModel.AccoSubjectID1 });//科目1
            occurModel.Lines2.Add(new FD_BadDebtOccurDetailODataEntity() { Amount = amount2, CurrentOccurAmount = amount2, AccoSubjectID = occurModel.AccoSubjectID2 });//科目2

            occurModel.DataDate = request.DataDate;
            occurModel.CustomerID = request.CustomerID;
            occurModel.PersonID = request.PersonID;
            occurModel.BusinessType = request.BusinessType;
            result.data = occurModel;
            return result;
        }


        /// <summary>
        /// 获取账龄重分类
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private decimal GetReclassification(RptAgingReclassificationRequest param)
        {
            //param.Enddate = request.DataDate;
            param.Boid = _identityService.UserId;
            param.EnteID = _identityService.EnterpriseId;
            param.GroupID = _identityService.GroupId;
            param.EnterpriseList = $"{_identityService.EnterpriseId}";

            param.MenuParttern = "0";//单位
            if (string.IsNullOrEmpty(param.IntervalType) || param.IntervalType == "0")
            {
                param.IntervalType = "1803300944570000101";//默认按年
            }
            param.OwnEntes = new List<string>() { _identityService.EnterpriseId };
            param.CanWatchEntes = new List<string>() { _identityService.EnterpriseId };
            var data = _agingReclassificationUtil.GetData(param).Result;//账龄报表默认只有一条
            if (data?.Count > 0)
            {
                return data.Sum(o => o.AdjustAmount);
            }

            return 0;
        }

        private DealingOccurDataResult GetAgingData(string dataDate, string accoSubjectId, string customerId, string personId, string businessType)
        {
            var param = new DealingOccurRequest()
            {
                Boid = _identityService.UserId,//    "1799425",
                Enddate = dataDate,
                EnteID = _identityService.EnterpriseId,//634086739144001721,
                GroupID = _identityService.GroupId,//957025251000000,
                CustomerID = customerId,
                PersonID = personId,
                IntervalType = "1803300944570000101",
                EnterpriseList = $"|{_identityService.EnterpriseId}",
                OwnEntes = new List<string>() { _identityService.EnterpriseId },
                CustomerType = businessType == ((long)BusinessTypeEnum.Customer).ToString() ? "1" : "2",
                MenuParttern = "1",
                AccountingSubjectsID = accoSubjectId, //应收科目一   1702241401310000101
            };
            var result = _agingDataUtil.GetAgingData(param).Result?.FirstOrDefault();//账龄报表默认只有一条
            return result;
        }

        //增加
        [HttpPost, Route("CreateSettleReceipt")]
        public async Task<RpcResult<ReceiptResult>> CreateSettleReceipt([FromBody] FD_BadDebtOccurAddCommand request)
        {
            try
            {
                var result = new Result();
                var enterpriseId = Convert.ToInt64(_identityService.EnterpriseId);
                var occurModel = _provider.GetSingleDataAsync(Convert.ToInt64(request.NumericalOrder)).Result;
                //var settingModel = _settingProvider.GetDataByEnterId(enterpriseId, occurModel.DataDate);
                var settingModel = new FD_BaddebtSettingODataEntity();// await _settingProvider.GetSingleDataAsync(Convert.ToInt64(occurModel.NumericalOrderSetting));

                var receiptResult = new RpcResult<ReceiptResult>();
                //凭证1
                var amount1 = occurModel.Lines.Where(o => o.AccoSubjectID == occurModel.AccoSubjectID1).Sum(o => o.CurrentOccurAmount);
                if (amount1 > 0)
                {
                    await SaveReceipt(occurModel, settingModel, settingModel.BadAccoSubjectOne, settingModel.DebtReceAccoSubjectOne, amount1);
                }
                //凭证2
                var amount2 = occurModel.Lines.Where(o => o.AccoSubjectID == occurModel.AccoSubjectID2).Sum(o => o.CurrentOccurAmount);
                if (amount2 > 0)
                {
                    receiptResult = await SaveReceipt(occurModel, settingModel, settingModel.BadAccoSubjectTwo, settingModel.DebtReceAccoSubjectTwo, amount2);
                }
                return receiptResult;
            }
            catch (Exception ex)
            {
                return new RpcResult<ReceiptResult>() { code = 0 };
            }
        }

        private async Task<RpcResult<ReceiptResult>> SaveReceipt(FD_BadDebtOccurODataEntity occurModel, FD_BaddebtSettingODataEntity settingModel, string debitSubjectId, string creditSubjectId, decimal Amount)
        {
            var recetiptData = new ReceiptData();
            recetiptData.DataA.DataDate = occurModel.DataDate;
            recetiptData.DataA.TicketedPointID = occurModel.TicketedPointID;
            recetiptData.DataA.SettleReceipType = ((long)SettleReceiptType.转账凭证).ToString();
            recetiptData.DataA.EnterpriseID = occurModel.EnterpriseID;
            recetiptData.DataA.Remarks = "坏账发生";

            var receiptDebit = new SettleReceiptDetail();
            receiptDebit.LorR = 0;//借方
            receiptDebit.ReceiptAbstractID = settingModel.OccurReceiptAbstractID;
            receiptDebit.Debit = Amount;
            receiptDebit.AccoSubjectID = debitSubjectId;//坏账科目一
            var subjectCodeDebit = _baseUtil.GetEnterSubjectList(Convert.ToInt64(debitSubjectId), Convert.ToInt64(_identityService.EnterpriseId), occurModel.DataDate).Result;
            if (subjectCodeDebit != null)
            {
                var code = subjectCodeDebit.FirstOrDefault(o => o.AccoSubjectID == receiptDebit.AccoSubjectID);
                receiptDebit.AccoSubjectCode = code?.cAccoSubjectCode;
                if (occurModel.BusinessType == ((long)BusinessTypeEnum.Customer).ToString())
                {
                    receiptDebit.CustomerID = occurModel.CustomerID;
                }
                if (occurModel.BusinessType == ((long)BusinessTypeEnum.Person).ToString())
                {
                    receiptDebit.PersonID = occurModel.PersonID;
                }
            }
            var receiptCredit = new SettleReceiptDetail();
            receiptCredit.LorR = 1;//贷方
            receiptCredit.ReceiptAbstractID = settingModel.OccurReceiptAbstractID;
            receiptCredit.Credit = Amount;
            receiptCredit.AccoSubjectID = creditSubjectId;// settinModel.DebtReceAccoSubjectOne;//应收科目一

            var subjectCodeCredit = _baseUtil.GetEnterSubjectList(Convert.ToInt64(creditSubjectId), Convert.ToInt64(_identityService.EnterpriseId), occurModel.DataDate).Result;
            if (subjectCodeCredit != null)
            {
                var code = subjectCodeCredit.FirstOrDefault(o => o.AccoSubjectID == receiptCredit.AccoSubjectID);
                receiptCredit.AccoSubjectCode = code?.cAccoSubjectCode;
                if (occurModel.BusinessType == ((long)BusinessTypeEnum.Customer).ToString())
                {
                    receiptCredit.CustomerID = occurModel.CustomerID;
                }
                if (occurModel.BusinessType == ((long)BusinessTypeEnum.Person).ToString())
                {
                    receiptCredit.PersonID = occurModel.PersonID;
                }
            }
            recetiptData.DataB.Add(receiptDebit);
            recetiptData.DataB.Add(receiptCredit);

            var resultReceipt = await _exeUtil.AfterSaveReceipt(recetiptData, occurModel.NumericalOrder, _identityService.AppId, _identityService.EnterpriseId, _identityService.UserId);
            return resultReceipt;

        }
        [HttpPost]
        public async Task<RpcResult<ReceiptResult>> AfterSaveReceipt(ReceiptData recetiptData, string numericalOrder, string appId, string enterpriseId)
        {
            var resultReceipt = _settleReceiptUtil.Save(recetiptData);

            if (resultReceipt.code == 1)
            {
                //生成制单
                Biz_Review review = new Biz_Review(resultReceipt.data.NumericalOrder, "1611091727140000101", _identityService.UserId).SetMaking();
                await _biz_ReviewRepository.AddAsync(review);
                //添加执行情况:成功
                await _exeRepository.AddAsync(new Domain.FD_BadDebtExecution()
                {
                    AppID = appId,
                    EnterpriseID = enterpriseId,
                    NumericalOrder = numericalOrder,
                    NumericalOrderReceipt = resultReceipt.data.NumericalOrder,
                    State = true,//生成成功
                    CreateDate = DateTime.Now,
                    Remarks = "凭证生成成功"
                }).ConfigureAwait(false);
                await _exeRepository.UnitOfWork.SaveChangesAsync();
            }
            else
            {
                var domain = new Domain.FD_BadDebtExecution()
                {
                    AppID = _identityService.AppId,
                    EnterpriseID = _identityService.EnterpriseId,
                    NumericalOrder = numericalOrder,
                    NumericalOrderReceipt = "0",
                    //NumericalReceipt = result.data.NumericalOrder,
                    State = false,//生成失败
                    CreateDate = DateTime.Now,
                    Remarks = resultReceipt.msg ?? "凭证生成异常"
                };
                //添加执行情况：失败
                await _exeRepository.AddAsync(domain);
                await _exeRepository.UnitOfWork.SaveChangesAsync();
            }

            return resultReceipt;
        }


        //增加
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FD_BadDebtOccurAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        //[PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FD_BadDebtOccurDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        public async Task<Result> Modify([FromBody] FD_BadDebtOccurModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
    }
}
