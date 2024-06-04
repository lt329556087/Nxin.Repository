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
    public class fd_scheduleplanAddHandler : IRequestHandler<fd_scheduleplanSaveCommand, Result>
    {
        IIdentityService _identityService;
        Ifd_scheduleplanRepository _repository;
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

        public fd_scheduleplanAddHandler(IFD_AccountTransferDetailRepository accountTransferDetailRepository,IbsfileRepository ibsfileRepository,IIdentityService identityService, IFD_ExpenseRepository expenseRepository, IFD_PaymentExtendRepository paymentExtendRepository, IFD_SettleReceiptRepository settleReceiptRepository, IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, Ifd_scheduleplanRepository repository, IBiz_Related biz_RelatedRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
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
        }

        public async Task<Result> Handle(fd_scheduleplanSaveCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();

            try
            {
                var list = _repository.GetList(_identityService.GroupId);
                foreach (var item in request)
                {
                    item.NumericalOrder = await _numericalOrderCreator.CreateAsync();
                    item.GroupId = _identityService.GroupId;
                    item.ModifiedDate = DateTime.Now;
                    item.CreatedDate = DateTime.Now;
                    item.OwnerId = Convert.ToInt64(_identityService.UserId);
                    _repository.Add(item);
                    //筛选出来当前申请已存在的 排程 全部变为历史 用于追溯
                    var temp = list.Where(m => m.ScheduleStatus == 1 && m.ApplyNumericalOrder == item.ApplyNumericalOrder).ToList();
                    if (temp != null)
                    {
                        foreach (var x in temp)
                        {
                            x.ScheduleStatus = -1;
                            _repository.Update(x);
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
    }
    public class fd_scheduleplanCancelHandler : IRequestHandler<fd_scheduleplanCancelCommand, Result>
    {
        IIdentityService _identityService;
        Ifd_scheduleplanRepository _repository;
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

        public fd_scheduleplanCancelHandler(IFD_AccountTransferDetailRepository accountTransferDetailRepository, IbsfileRepository ibsfileRepository, IIdentityService identityService, IFD_ExpenseRepository expenseRepository, IFD_PaymentExtendRepository paymentExtendRepository, IFD_SettleReceiptRepository settleReceiptRepository, IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, Ifd_scheduleplanRepository repository, IBiz_Related biz_RelatedRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
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
        }

        public async Task<Result> Handle(fd_scheduleplanCancelCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();

            try
            {
                //取消直接变历史
                foreach (var item in request)
                {
                    _repository.Update(item);
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
}
