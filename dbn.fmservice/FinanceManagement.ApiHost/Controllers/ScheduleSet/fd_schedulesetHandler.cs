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
    public class fd_schedulesetAddHandler : IRequestHandler<fd_schedulesetSaveCommand, Result>
    {
        IIdentityService _identityService;
        Ifd_schedulesetRepository _repository;
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

        public fd_schedulesetAddHandler(Ifd_scheduleplanRepository scheduleplanRepository,IFD_AccountTransferDetailRepository accountTransferDetailRepository,IbsfileRepository ibsfileRepository,IIdentityService identityService, IFD_ExpenseRepository expenseRepository, IFD_PaymentExtendRepository paymentExtendRepository, IFD_SettleReceiptRepository settleReceiptRepository, IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, Ifd_schedulesetRepository repository, IBiz_Related biz_RelatedRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
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

        public async Task<Result> Handle(fd_schedulesetSaveCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();

            try
            {
                //var plan = _ScheduleplanRepository.GetList(_identityService.GroupId).GroupBy(m=>m.Level).ToList();
                //string msg = "";

                //foreach (var item in plan)
                //{
                //    if (request.Where(m=>m.Level == item.Key).Count() == 0)
                //    {
                //        if (string.IsNullOrEmpty(msg))
                //        {
                //            msg = item.Key.ToString();
                //        }
                //        else
                //        {
                //            msg += "," + item.Key.ToString();
                //        }
                //    }
                //}
                //if (!string.IsNullOrEmpty(msg))
                //{
                //    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                //    result.msg = "当前等级" + msg + "已被排程引用，不可删除";
                //    return result;
                //}
                //request.SettleReceipType = "201611180104402202";
                foreach (var item in request)
                {
                    item.GroupId = _identityService.GroupId;
                    item.ModifiedDate = DateTime.Now;
                    item.CreatedDate = DateTime.Now;
                    item.OwnerId = Convert.ToInt64(_identityService.UserId);
                    _repository.Add(item);
                }
                var list = _repository.GetList(_identityService.GroupId);
                _repository.RemoveRange(list);
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
