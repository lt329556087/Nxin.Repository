using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Infrastructure.Repositories;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Architecture.Common.Application.Commands;

namespace FinanceManagement.ApiHost.Controllers.FD_SettleReceiptInterface
{
    public class FD_SettleReceiptInterfaceAddHandler : IRequestHandler<FD_SettleReceiptInterfaceAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_SettleReceiptRepository _repository;
        IFD_SettleReceiptDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        private TreeModelODataProvider _treeModel;
        Common.FMBaseCommon _baseCommon;

        public FD_SettleReceiptInterfaceAddHandler(IIdentityService identityService, TreeModelODataProvider treeModel, IFD_SettleReceiptRepository repository, IFD_SettleReceiptDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository,Common.FMBaseCommon baseCommon)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _treeModel = treeModel;
            _baseCommon = baseCommon;
        }
        public async Task<Result> Handle(FD_SettleReceiptInterfaceAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                result= ValidParam(request);
                if (!string.IsNullOrEmpty(result.msg))
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    return result;
                }
                long number = _numberCreator.Create<Domain.FD_SettleReceipt>(o => o.DataDate, o => o.Number, DateTime.Now, o => o.EnterpriseID == _identityService.EnterpriseId);
                //var numberResult = _baseCommon.GetNumber(new NumberSearchModel() {IsComplex="1",DataDate=request.DataDate,ChildSettleReceipType=request.SettleReceipType,TicketedPointID=request.TicketedPointID,EnterpriseID=request.EnterpriseID }); 
               
                //if (!numberResult.ResultState&& !string.IsNullOrEmpty(numberResult.Msg))
                //{
                //    return result;
                //}
                //var number = numberResult.Data;
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FD_SettleReceipt()
                {
                    NumericalOrder = numericalOrder,
                    Guid = Guid.NewGuid(),
                    SettleReceipType = request.SettleReceipType,
                    DataDate = Convert.ToDateTime(request.DataDate),
                    TicketedPointID = string.IsNullOrEmpty(request.TicketedPointID) ? "0" : request.TicketedPointID,
                    Number = number.ToString(),
                    AccountNo = request.AccountNo,
                    AttachmentNum = request.AttachmentNum,
                    Remarks = request.Remarks,
                    EnterpriseID =request.EnterpriseID,
                    OwnerID = request.OwnerID,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    details = new List<FD_SettleReceiptDetail>()
                };
                int i = 1;
                request.Lines?.ForEach(o =>
                {
                    domain.details.Add(new FD_SettleReceiptDetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = Guid.NewGuid(),
                        EnterpriseID =string.IsNullOrEmpty(o.EnterpriseID)?request.EnterpriseID:o.EnterpriseID,
                        ReceiptAbstractID = string.IsNullOrEmpty(o.ReceiptAbstractID) ? "0" : o.ReceiptAbstractID,
                        AccoSubjectID = string.IsNullOrEmpty(o.AccoSubjectID) ? "0" : o.AccoSubjectID,
                        AccoSubjectCode = o.AccoSubjectCode,
                        CustomerID = string.IsNullOrEmpty(o.CustomerID) ? "0" : o.CustomerID,
                        PersonID = string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID,
                        MarketID = string.IsNullOrEmpty(o.MarketID) ? "0" : o.MarketID,
                        ProjectID = string.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,
                        ProductID = string.IsNullOrEmpty(o.ProductID) ? "0" : o.ProductID,
                        PaymentTypeID = o.PaymentTypeID,
                        AccountID = o.AccountID,
                        LorR = o.LorR,
                        Credit = o.Credit,
                        Debit = o.Debit,
                        Content = o.Content,
                        RowNum = i++,
                        OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                        IsCharges = false
                    });
                });
                await _repository.AddAsync(domain);
                await _detailRepository.AddRangeAsync(domain.details);
                Biz_Review review = new Biz_Review(numericalOrder, "1611091727140000101",request.OwnerID).SetMaking();
                await _biz_ReviewRepository.AddAsync(review);
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                result.msg = "保存成功";
                result.data = new { NumericalOrder = numericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.msg = "保存失败,请联系管理员！";
            }
            return result;
        }
        private Result ValidParam(FD_SettleReceiptInterfaceAddCommand model)
        {
            Result result = new Result();
            if (model == null)
            {
                result.msg = "参数有误";
                return result;
            }
            if (string.IsNullOrEmpty(model.EnterpriseID))
            {
                result.msg = "EnterpriseID空";
                return result;
            }
            if (string.IsNullOrEmpty(model.SettleReceipType))
            {
                result.msg = "SettleReceipType空";
                return result;
            }
            if (string.IsNullOrEmpty(model.OwnerID))
            {
                result.msg = "OwnerID空";
                return result;
            }
            return result;
        }
    }

    /// <summary>
    /// 删除
    /// </summary>
    public class FD_SettleReceiptInterfaceDeleteHandler : IRequestHandler<FD_SettleReceiptInterfaceDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFD_SettleReceiptRepository _repository;
        IFD_SettleReceiptDetailRepository _detailRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FD_SettleReceiptInterfaceDeleteHandler(IIdentityService identityService, IFD_SettleReceiptRepository repository, IFD_SettleReceiptDetailRepository detailRepository, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FD_SettleReceiptInterfaceDeleteCommand request, CancellationToken cancellationToken)
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
                    await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _detailRepository.UnitOfWork.SaveChangesAsync();
                    await _repository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _repository.UnitOfWork.SaveChangesAsync();
                    await _biz_ReviewRepository.RemoveRangeAsync(o => o.NumericalOrder == num);  
                    await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
                }               
                result.data = new { NumericalOrder = request.NumericalOrder };
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

    public class FD_SettleReceiptInterfaceModifyHandler : IRequestHandler<FD_SettleReceiptInterfaceModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFD_SettleReceiptRepository _repository;
        IFD_SettleReceiptDetailRepository _detailRepository;
        public FD_SettleReceiptInterfaceModifyHandler(IIdentityService identityService, IFD_SettleReceiptRepository repository, IFD_SettleReceiptDetailRepository detailRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
        }

        public async Task<Result> Handle(FD_SettleReceiptInterfaceModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetDataAsync(request.NumericalOrder);
                if (domain == null) { result.code = ErrorCode.RequestArgumentError.GetIntValue(); result.msg = "NumericalOrder查询空"; return result; }
                domain.DataDate = Convert.ToDateTime(request.DataDate);
                domain.TicketedPointID = string.IsNullOrEmpty(request.TicketedPointID) ? "0" : request.TicketedPointID;
                domain.AccountNo = request.AccountNo;
                domain.AttachmentNum = request.AttachmentNum;
                domain.Remarks = request.Remarks;
                _repository.Update(domain);
                int i = 1;
                foreach (var item in request.Lines)
                {
                    if (item.Target == TargetType.Create || item.IsCreate)
                    {
                        var detial = new FD_SettleReceiptDetail()
                        {
                            NumericalOrder = request.NumericalOrder,
                            Guid = Guid.NewGuid(),
                            EnterpriseID = string.IsNullOrEmpty(item.EnterpriseID) ? request.EnterpriseID : item.EnterpriseID,
                            ReceiptAbstractID = string.IsNullOrEmpty(item.ReceiptAbstractID) ? "0" : item.ReceiptAbstractID,
                            AccoSubjectID = string.IsNullOrEmpty(item.AccoSubjectID) ? "0" : item.AccoSubjectID,
                            AccoSubjectCode = item.AccoSubjectCode,
                            CustomerID = string.IsNullOrEmpty(item.CustomerID) ? "0" : item.CustomerID,
                            PersonID = string.IsNullOrEmpty(item.PersonID) ? "0" : item.PersonID,
                            MarketID = string.IsNullOrEmpty(item.MarketID) ? "0" : item.MarketID,
                            ProjectID = string.IsNullOrEmpty(item.ProjectID) ? "0" : item.ProjectID,
                            ProductID = string.IsNullOrEmpty(item.ProductID) ? "0" : item.ProductID,
                            PaymentTypeID = item.PaymentTypeID,
                            AccountID = item.AccountID,
                            LorR = item.LorR,
                            Credit = item.Credit,
                            Debit = item.Debit,
                            Content = item.Content,
                            //RowNum = i++,
                            OrganizationSortID = string.IsNullOrEmpty(item.OrganizationSortID) ? "0" : item.OrganizationSortID,
                            IsCharges = false
                        };
                        _detailRepository.Add(detial);
                        continue;
                    }
                    if (item.Target == TargetType.Delete|| item.IsDelete)
                    {
                        await _detailRepository.RemoveRangeAsync(o => o.RecordID == item.RecordID);
                        continue;
                    }
                    if (item.Target == TargetType.Update || item.IsUpdate)
                    {
                        var obj = await _detailRepository.GetDetailByIdAsync(item.RecordID);
                        if (obj != null && obj.RecordID > 0)
                        {
                            obj.EnterpriseID = string.IsNullOrEmpty(item.EnterpriseID) ? request.EnterpriseID : item.EnterpriseID;
                            obj.ReceiptAbstractID = string.IsNullOrEmpty(item.ReceiptAbstractID) ? "0" : item.ReceiptAbstractID;
                            obj.AccoSubjectID = string.IsNullOrEmpty(item.AccoSubjectID) ? "0" : item.AccoSubjectID;
                            obj.AccoSubjectCode = item.AccoSubjectCode;
                            obj.CustomerID = string.IsNullOrEmpty(item.CustomerID) ? "0" : item.CustomerID;
                            obj.PersonID = string.IsNullOrEmpty(item.PersonID) ? "0" : item.PersonID;
                            obj.MarketID = string.IsNullOrEmpty(item.MarketID) ? "0" : item.MarketID;
                            obj.ProjectID = string.IsNullOrEmpty(item.ProjectID) ? "0" : item.ProjectID;
                            obj.ProductID = string.IsNullOrEmpty(item.ProductID) ? "0" : item.ProductID;
                            obj.PaymentTypeID = item.PaymentTypeID;
                            obj.AccountID = item.AccountID;
                            obj.LorR = item.LorR;
                            obj.Credit = item.Credit;
                            obj.Debit = item.Debit;
                            obj.Content = item.Content;
                            //obj.RowNum = i++,
                            obj.OrganizationSortID = string.IsNullOrEmpty(item.OrganizationSortID) ? "0" : item.OrganizationSortID;
                            _detailRepository.Update(obj);
                        }
                       
                    }
                }

                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                result.data = new { NumericalOrder = request.NumericalOrder };
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
