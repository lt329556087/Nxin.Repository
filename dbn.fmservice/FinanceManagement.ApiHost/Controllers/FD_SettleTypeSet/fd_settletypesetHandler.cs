using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.Repositories;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers
{
    public class fd_settletypeAddHandler : IRequestHandler<fd_settletypeSaveCommand, Result>
    {
        IIdentityService _identityService;
        Ifd_settletypeRepository _repository;
        Ifd_scheduleplanRepository _ScheduleplanRepository;
        IFD_SettleReceiptRepository _settleReceiptRepository;
        IFD_SettleReceiptDetailRepository _settleReceiptDetailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;
        IFD_PaymentExtendRepository _paymentExtendRepository;
        IFD_ExpenseRepository _expenseRepository;
        IbsfileRepository _ibsfileRepository;
        IFD_AccountTransferDetailRepository _AccountTransferDetailRepository;

        public fd_settletypeAddHandler(Ifd_scheduleplanRepository scheduleplanRepository, IFD_AccountTransferDetailRepository accountTransferDetailRepository, IbsfileRepository ibsfileRepository, IIdentityService identityService, IFD_ExpenseRepository expenseRepository, IFD_PaymentExtendRepository paymentExtendRepository, IFD_SettleReceiptRepository settleReceiptRepository, IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, Ifd_settletypeRepository repository, IBiz_Related biz_RelatedRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _AccountTransferDetailRepository = accountTransferDetailRepository;
            _identityService = identityService;
            _repository = repository;
            _settleReceiptRepository = settleReceiptRepository;
            _settleReceiptDetailRepository = settleReceiptDetailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
            _paymentExtendRepository = paymentExtendRepository;
            _expenseRepository = expenseRepository;
            _ibsfileRepository = ibsfileRepository;
            _ScheduleplanRepository = scheduleplanRepository;
        }
        /// <summary>
        /// 增 改 一体
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Result> Handle(fd_settletypeSaveCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();

            try
            {
                //删
                if (request.IsDelete)
                {
                    var list = _repository.GetAsync(request.DataDictID).Result;
                    if (list != null)
                    {
                        _repository.Remove(list);
                    }
                } 
                else
                {
                    if (string.IsNullOrEmpty(request.DataDictID))
                    {
                        //增加
                        AddData(request);
                    }
                    else
                    {
                        var list = _repository.GetAsync(request.DataDictID).Result;
                        if (list != null)
                        {   
                            //修改
                            UpdataData(request, list);
                        }
                    }
                }
                
                await _repository.UnitOfWork.SaveChangesAsync();
                result.code = ErrorCode.Success.GetIntValue();
                result.msg = "保存成功";
                return result;
            }
            catch (Exception e)
            {
                result.data = e;
                result.code = ErrorCode.RequestArgumentError.GetIntValue();
                result.msg = "保存异常";
                return result;
            }
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="request"></param>
        /// <param name="list"></param>
        private void UpdataData(fd_settletypeSaveCommand request, biz_datadict list)
        {
            _repository.Remove(list);
            SetValue(request);
            _repository.Add(request);
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="request"></param>
        private void AddData(fd_settletypeSaveCommand request)
        {
            request.DataDictID = _numericalOrderCreator.CreateAsync().Result;
            SetValue(request);
            _repository.Add(request);
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="request"></param>
        private void SetValue(fd_settletypeSaveCommand request)
        {
            request.PID = "201610220104402002";
            request.ModifiedDate = DateTime.Now;
            request.CreatedDate = DateTime.Now;
            request.EnterpriseID = _identityService.EnterpriseId;
            request.DataDictType = request.DataDictID;
        }
    }
    public class fd_settletypesetAddHandler : IRequestHandler<fd_settletypesetSaveCommand, Result>
    {
        IIdentityService _identityService;
        Ifd_settletypesetRepository _repository;
        Ifd_scheduleplanRepository _ScheduleplanRepository;
        IFD_SettleReceiptRepository _settleReceiptRepository;
        IFD_SettleReceiptDetailRepository _settleReceiptDetailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;
        IFD_PaymentExtendRepository _paymentExtendRepository;
        IFD_ExpenseRepository _expenseRepository;
        IbsfileRepository _ibsfileRepository;
        IFD_AccountTransferDetailRepository _AccountTransferDetailRepository;

        public fd_settletypesetAddHandler(Ifd_scheduleplanRepository scheduleplanRepository,IFD_AccountTransferDetailRepository accountTransferDetailRepository,IbsfileRepository ibsfileRepository,IIdentityService identityService, IFD_ExpenseRepository expenseRepository, IFD_PaymentExtendRepository paymentExtendRepository, IFD_SettleReceiptRepository settleReceiptRepository, IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, Ifd_settletypesetRepository repository, IBiz_Related biz_RelatedRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _AccountTransferDetailRepository = accountTransferDetailRepository;
            _identityService = identityService;
            _repository = repository;
            _settleReceiptRepository = settleReceiptRepository;
            _settleReceiptDetailRepository = settleReceiptDetailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
            _paymentExtendRepository = paymentExtendRepository;
            _expenseRepository = expenseRepository;
            _ibsfileRepository = ibsfileRepository;
            _ScheduleplanRepository = scheduleplanRepository;
        }

        public async Task<Result> Handle(fd_settletypesetSaveCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();

            try
            {
                request.NumericalOrder = _numericalOrderCreator.CreateAsync().Result;
                request.GroupId = _identityService.GroupId;
                request.ModifiedDate = DateTime.Now;
                request.CreatedDate = DateTime.Now;
                request.OwnerId = _identityService.UserId;
                _repository.Add(request);
                
                var list = _repository.GetAsync(_identityService.GroupId).Result;
                if (list != null)
                {
                    _repository.Remove(list);
                }
                await _repository.UnitOfWork.SaveChangesAsync();
                result.code = ErrorCode.Success.GetIntValue();
                result.msg = "保存成功";
                return result;
            }
            catch (Exception e)
            {
                result.data = e;
                result.code = ErrorCode.RequestArgumentError.GetIntValue();
                result.msg = "保存异常";
                return result;
            }
        }
    }
    public class fd_settletypesetRemoveHandler : IRequestHandler<fd_settletypesetRemoveCommand, Result>
    {
        IIdentityService _identityService;
        Ifd_settletypesetRepository _repository;
        Ifd_scheduleplanRepository _ScheduleplanRepository;
        IFD_SettleReceiptRepository _settleReceiptRepository;
        IFD_SettleReceiptDetailRepository _settleReceiptDetailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;
        IFD_PaymentExtendRepository _paymentExtendRepository;
        IFD_ExpenseRepository _expenseRepository;
        IbsfileRepository _ibsfileRepository;
        IFD_AccountTransferDetailRepository _AccountTransferDetailRepository;

        public fd_settletypesetRemoveHandler(Ifd_scheduleplanRepository scheduleplanRepository, IFD_AccountTransferDetailRepository accountTransferDetailRepository, IbsfileRepository ibsfileRepository, IIdentityService identityService, IFD_ExpenseRepository expenseRepository, IFD_PaymentExtendRepository paymentExtendRepository, IFD_SettleReceiptRepository settleReceiptRepository, IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, Ifd_settletypesetRepository repository, IBiz_Related biz_RelatedRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _AccountTransferDetailRepository = accountTransferDetailRepository;
            _identityService = identityService;
            _repository = repository;
            _settleReceiptRepository = settleReceiptRepository;
            _settleReceiptDetailRepository = settleReceiptDetailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
            _paymentExtendRepository = paymentExtendRepository;
            _expenseRepository = expenseRepository;
            _ibsfileRepository = ibsfileRepository;
            _ScheduleplanRepository = scheduleplanRepository;
        }

        public async Task<Result> Handle(fd_settletypesetRemoveCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();

            try
            {
                var list = _repository.GetAsync(_identityService.GroupId).Result;
                _repository.Remove(list);
                await _repository.UnitOfWork.SaveChangesAsync();
                result.code = ErrorCode.Success.GetIntValue();
                result.msg = "保存成功";
                return result;
            }
            catch (Exception e)
            {
                result.data = e;
                result.code = ErrorCode.RequestArgumentError.GetIntValue();
                result.msg = "保存异常";
                return result;
            }
        }
    }
}
