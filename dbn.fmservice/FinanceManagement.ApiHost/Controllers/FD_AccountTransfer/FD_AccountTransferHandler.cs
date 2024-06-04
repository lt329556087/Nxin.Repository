using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.Common;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.Repositories;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_AccountTransfer
{
    /// <summary>
    /// 增加
    /// </summary>
    public class FD_AccountTransferAddHandler : IRequestHandler<FD_AccountTransferAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_AccountTransferRepository _repository;
        IFD_AccountTransferDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        ApplySubjectUtil _applySubjectUtil;

        public FD_AccountTransferAddHandler(IIdentityService identityService, IFD_AccountTransferRepository repository, IFD_AccountTransferDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, ApplySubjectUtil applySubjectUtil)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _applySubjectUtil = applySubjectUtil;
        }

        public async Task<Result> Handle(FD_AccountTransferAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (string.IsNullOrEmpty(request.OwnerID) || request.OwnerID == "0")
                {
                    request.OwnerID = _identityService.UserId;
                }
                if (string.IsNullOrEmpty(request.EnterpriseID) || request.EnterpriseID == "0")
                {
                    request.EnterpriseID = _identityService.EnterpriseId;
                }
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FD_AccountTransfer()
                {
                    AccountTransferAbstract = request.AccountTransferAbstract,
                    AccountTransferType = request.AccountTransferType,
                    Guid = Guid.NewGuid(),
                    DataDate = Convert.ToDateTime(request.DataDate),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    NumericalOrder = numericalOrder,
                    Remarks = request.Remarks,
                    OwnerID = request.OwnerID,
                    EnterpriseID = request.EnterpriseID ,
                    UploadUrl = request.UploadUrl ?? ""
                };

                request.Lines?.ForEach(o =>
                {
                    domain.AddDetail(new FD_AccountTransferDetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = Guid.NewGuid(),
                        EnterpriseID =(string.IsNullOrEmpty(o.EnterpriseID)||o.EnterpriseID=="0")? request.EnterpriseID : o.EnterpriseID,
                        PaymentTypeID = o.PaymentTypeID ?? "0",
                        AccountID = o.AccountID ?? "0",
                        Amount = o.Amount,
                        IsIn = o.IsIn,
                        DataDateTime = string.IsNullOrEmpty(o.DataDateTime) ? DateTime.Now : Convert.ToDateTime(o.DataDateTime),
                        ModifiedDate = DateTime.Now,
                        Remarks = o.Remarks
                    });
                });

                await _repository.AddAsync(domain);
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                //添加申请主题
                var subject = new SubjectInfo()
                {
                    NumericalOrder = numericalOrder,
                    Subject = "资金调拨",
                    SubjectExtendInfo = new SubjectExtendInfo()
                    {
                        Amount = request.Lines.Find(o => o.IsIn).Amount,
                        DateInfo = new NameValue { Name = "申请日期", Value = request.DataDate }
                    }
                };
                _applySubjectUtil.AddSubject(subject);
                result.data = new { NumericalOrder = numericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Create.GetIntValue();
                result.msg = "保存异常";
            }

            return result;
        }
    }

    /// <summary>
    /// 删除
    /// </summary>
    public class FD_AccountTransferDeleteHandler : IRequestHandler<FD_AccountTransferDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFD_AccountTransferRepository _repository;
        IFD_AccountTransferDetailRepository _detailRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FD_AccountTransferDeleteHandler(IIdentityService identityService, IFD_AccountTransferRepository repository, IFD_AccountTransferDetailRepository detailRepository, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FD_AccountTransferDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(request.NumericalOrder))
                {
                    list = request.NumericalOrder.Split(',').ToList();
                }

                foreach (var num in list)
                {
                    await _repository.RemoveRangeAsync(o => o.EnterpriseID == _identityService.EnterpriseId && o.NumericalOrder == num);
                    await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                }

                await _repository.UnitOfWork.SaveChangesAsync();
                result.code = ErrorCode.Success.GetIntValue();
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Delete.GetIntValue();
                result.msg = "保存异常";
            }

            return result;
        }
    }

    public class FD_AccountTransferModifyHandler : IRequestHandler<FD_AccountTransferModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFD_AccountTransferRepository _repository;
        IFD_AccountTransferDetailRepository _detailRepository;
        ApplySubjectUtil _applySubjectUtil;


        public FD_AccountTransferModifyHandler(IIdentityService identityService, IFD_AccountTransferRepository repository, IFD_AccountTransferDetailRepository detailRepository, ApplySubjectUtil applySubjectUtil)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _applySubjectUtil = applySubjectUtil;
        }

        public async Task<Result> Handle(FD_AccountTransferModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.NumericalOrder);
                domain.Update(request.AccountTransferType, request.AccountTransferAbstract, request.DataDate, request.DataDate, request.OwnerID, request.EnterpriseID);

                foreach (var item in request.Lines)
                {
                    item.EnterpriseID = string.IsNullOrEmpty(item.EnterpriseID) ? "0" : item.EnterpriseID;
                    var obj = domain.Details.Find(o => o.RecordID == item.RecordID);
                    if (string.IsNullOrEmpty(item.EnterpriseID)||item.EnterpriseID=="0")
                    {
                        item.EnterpriseID = request.EnterpriseID;
                    }
                    obj.Update(item.EnterpriseID, item.PaymentTypeID, item.AccountID, item.Amount, item.IsIn, item.DataDateTime, item.Remarks);
                }
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                //添加申请主题
                var subject = new SubjectInfo()
                {
                    NumericalOrder = request.NumericalOrder,
                    Subject = "资金调拨",
                    SubjectExtendInfo = new SubjectExtendInfo()
                    {
                        Amount = request.Lines.Find(o => o.IsIn).Amount,
                        DateInfo = new NameValue { Name = "申请日期", Value = request.DataDate }
                    }
                };
                _applySubjectUtil.AddSubject(subject);
                result.code = ErrorCode.Success.GetIntValue();
                return result;
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Delete.GetIntValue();
                result.msg = "保存异常";
            }
            return result;
        }
    }
}
