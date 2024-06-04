using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
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

namespace FinanceManagement.ApiHost.Controllers.FA_AssetsContract
{
    public class FA_AssetsContractAddHandler : IRequestHandler<FA_AssetsContractAddCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_AssetsContractRepository _repository;
        IFA_AssetsContractDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FA_AssetsContractAddHandler(IIdentityService identityService,
                                            IFA_AssetsContractRepository repository,
                                            IFA_AssetsContractDetailRepository detailRepository,
                                            NumericalOrderCreator numericalOrderCreator,
                                            NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                            IBiz_ReviewRepository biz_ReviewRepository,
                                            IBiz_Related biz_RelatedRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
        }
        public async Task<Result> Handle(FA_AssetsContractAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                long number = _numberCreator.Create<Domain.FA_AssetsContract>(o => o.DataDate, o => o.Number, Convert.ToDateTime(request.DataDate), o => o.EnterpriseID == _identityService.EnterpriseId);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FA_AssetsContract()
                {
                    NumericalOrder = numericalOrder,
                    DataDate = request.DataDate,
                    ContractName = request.ContractName,
                    ContractNumber = request.ContractNumber,
                    EnterpriseID = request.EnterpriseID,
                    MarketID = request.MarketID ?? "0",
                    SupplierID = request.SupplierID ?? "0",
                    Number = number.ToString(),
                    ContractTemplate = request.ContractTemplate??"0",
                    OwnerID = this._identityService.UserId,
                    Remarks = request.Remarks,
                    UpDataInfo = request.UpDataInfo,
                    ContractClause = request.ContractClause,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    TicketedPointID=request.TicketedPointID,
                };
                request.Lines?.ForEach(o =>
                {
                    var numericalOrderDetail = _numericalOrderCreator.CreateAsync();
                    domain.AddDetail(new FA_AssetsContractDetail()
                    {
                        NumericalOrder = numericalOrder,
                        NumericalOrderDetail = numericalOrderDetail.Result,
                        NumericalOrderInput = o.NumericalOrderInput,
                        AssetsName = o.AssetsName,
                        AssetsTypeID = o.AssetsTypeID,
                        Specification = o.Specification ?? "0",
                        Brand = o.Brand,
                        MeasureUnit = o.MeasureUnit,
                        Quantity = o.Quantity,
                        UnitPrice = o.UnitPrice,
                        Amount = o.Amount,
                        Remarks = o.Remarks,
                    });
                });
                await _repository.AddAsync(domain);
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
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
                result.msg = "保存失败,请联系管理员！";
            }
            return result;
        }
    }

    public class FA_AssetsContractDeleteHandler : IRequestHandler<FA_AssetsContractDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_AssetsContractRepository _repository;
        IFA_AssetsContractDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FA_AssetsContractDeleteHandler(IIdentityService identityService,
                                            IFA_AssetsContractRepository repository,
                                            IFA_AssetsContractDetailRepository detailRepository,
                                            NumericalOrderCreator numericalOrderCreator,
                                            NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                            IBiz_ReviewRepository biz_ReviewRepository,
                                            IBiz_Related biz_RelatedRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
        }

        public async Task<Result> Handle(FA_AssetsContractDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(request.NumericalOrder))
                {
                    var nums = request.NumericalOrder.Split(',').ToList();
                    List<string> newNums = nums.Where((x, i) => nums.FindIndex(z => z == x) == i).ToList();//去重
                    await _repository.RemoveRangeAsync(o => newNums.Contains(o.NumericalOrder));
                    await _detailRepository.RemoveRangeAsync(o => newNums.Contains(o.NumericalOrder));
                    await _repository.UnitOfWork.SaveChangesAsync();
                    await _detailRepository.UnitOfWork.SaveChangesAsync();
                    result.code = ErrorCode.Success.GetIntValue();
                    result.msg = "删除成功!";
                }
                else
                {
                    result.code = ErrorCode.Delete.GetIntValue();
                    result.msg = "流水号不能为空!";
                }
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Delete.GetIntValue();
                result.msg = "保存失败,请联系管理员！";
            }

            return result;
        }
    }

    public class FA_AssetsContractModifyHandler : IRequestHandler<FA_AssetsContractModifyCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_AssetsContractRepository _repository;
        IFA_AssetsContractDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FA_AssetsContractModifyHandler(IIdentityService identityService,
                                            IFA_AssetsContractRepository repository,
                                            IFA_AssetsContractDetailRepository detailRepository,
                                            NumericalOrderCreator numericalOrderCreator,
                                            NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                            IBiz_ReviewRepository biz_ReviewRepository,
                                            IBiz_Related biz_RelatedRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
        }

        public async Task<Result> Handle(FA_AssetsContractModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.NumericalOrder);
                domain.Update(request.DataDate, request.ContractName, request.ContractNumber, request.EnterpriseID, request.MarketID, request.SupplierID,
            string.IsNullOrEmpty(request.ContractTemplate) ? "0" : request.ContractTemplate, request.Remarks, request.UpDataInfo, request.ContractClause,request.TicketedPointID);
                //1增 2改 3删
                foreach (var item in request.Lines)
                {
                    if (item.IsCreate)
                    {
                        var numericalOrderDetail = await this._numericalOrderCreator.CreateAsync();
                        domain.AddDetail(new FA_AssetsContractDetail()
                        {
                            NumericalOrder = domain.NumericalOrder,
                            NumericalOrderDetail = numericalOrderDetail,
                            NumericalOrderInput = item.NumericalOrderInput,
                            AssetsName = item.AssetsName,
                            AssetsTypeID = item.AssetsTypeID,
                            Specification = item.Specification,
                            Brand = item.Brand,
                            MeasureUnit = item.MeasureUnit,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            Amount = item.Amount,
                            Remarks = item.Remarks,
                        });
                        continue;
                    }
                    if (item.IsUpdate)
                    {
                        var obj = domain.Details?.Find(o => o.NumericalOrderDetail == item.NumericalOrderDetail);
                        obj?.Update(item.AssetsName, item.AssetsTypeID, item.Specification, item.Brand, item.MeasureUnit, item.Quantity, item.UnitPrice, item.Amount, item.Remarks);
                        continue;
                    }
                    if (item.IsDelete)
                    {
                        await _detailRepository.RemoveRangeAsync(o => o.NumericalOrderDetail == item.NumericalOrderDetail);
                        continue;
                    }
                }
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _repository.UnitOfWork.SaveChangesAsync();
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
                result.msg = "保存失败,请联系管理员！";
            }
            return result;
        }

    }

    public class FA_AssetsContractCopyHandler : IRequestHandler<FA_AssetsContractCopyCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_AssetsContractRepository _repository;
        IFA_AssetsContractDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FA_AssetsContractCopyHandler(IIdentityService identityService,
                                            IFA_AssetsContractRepository repository,
                                            IFA_AssetsContractDetailRepository detailRepository,
                                            NumericalOrderCreator numericalOrderCreator,
                                            NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                            IBiz_ReviewRepository biz_ReviewRepository,
                                            IBiz_Related biz_RelatedRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
        }
        public async Task<Result> Handle(FA_AssetsContractCopyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = _repository.GetAsync(request.NumericalOrder).Result;
                long number = _numberCreator.Create<Domain.FA_AssetsContract>(o => o.DataDate, o => o.Number, Convert.ToDateTime(domain.DataDate), o => o.EnterpriseID == _identityService.EnterpriseId);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var copy = new Domain.FA_AssetsContract()
                {
                    NumericalOrder = numericalOrder,
                    DataDate = domain.DataDate,
                    ContractName = domain.ContractName,
                    ContractNumber = domain.ContractNumber,
                    EnterpriseID = domain.EnterpriseID,
                    MarketID = domain.MarketID,
                    SupplierID = domain.SupplierID,
                    Number = number.ToString(),
                    ContractTemplate = domain.ContractTemplate??"0",
                    OwnerID = this._identityService.UserId,
                    Remarks = domain.Remarks,
                    UpDataInfo = domain.UpDataInfo,
                    ContractClause = domain.ContractClause,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    TicketedPointID = domain.TicketedPointID,
                };
                domain.Details?.ForEach(o =>
                {
                    var numericalOrderDetail = _numericalOrderCreator.CreateAsync();
                    copy.AddDetail(new FA_AssetsContractDetail()
                    {
                        NumericalOrder = numericalOrder,
                        NumericalOrderDetail = numericalOrderDetail.Result,
                        NumericalOrderInput = o.NumericalOrderInput,
                        AssetsName = o.AssetsName,
                        AssetsTypeID = o.AssetsTypeID,
                        Specification = o.Specification,
                        Brand = o.Brand,
                        MeasureUnit = o.MeasureUnit,
                        Quantity = o.Quantity,
                        UnitPrice = o.UnitPrice,
                        Amount = o.Amount,
                        Remarks = o.Remarks,
                    });
                });
                await _repository.AddAsync(copy);
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
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
                result.msg = "复制失败,请联系管理员！";
            }
            return result;
        }
    }

}
