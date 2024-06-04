using Architecture.Common.HttpClientUtil;
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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FA_AssetsApply
{
    public class FA_AssetsApplyAddHandler : IRequestHandler<FA_AssetsApplyAddCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_AssetsApplyRepository _repository;
        IFA_AssetsApplyDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FA_AssetsApplyAddHandler(IIdentityService identityService,
                                            IFA_AssetsApplyRepository repository,
                                            IFA_AssetsApplyDetailRepository detailRepository,
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
        public async Task<Result> Handle(FA_AssetsApplyAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                //long number = _numberCreator.Create<Domain.FA_AssetsApply>(o => o.CreatedDate, o => o.Number, DateTime.Now, o => o.EnterpriseID == _identityService.EnterpriseId);
                long number = _numberCreator.Create<Domain.FA_AssetsApply>(o => o.DataDate, o => o.Number, Convert.ToDateTime(request.DataDate), o => o.EnterpriseID == _identityService.EnterpriseId);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FA_AssetsApply()
                {
                    NumericalOrder = numericalOrder,
                    OwnerID = this._identityService.UserId,
                    EnterpriseID = request.EnterpriseID,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    MarketID = request.MarketID,
                    DataDate = request.DataDate,
                    Remarks = request.Remarks,
                    UpDataInfo = request.UpDataInfo,
                    Number = number.ToString(),
                    TicketedPointID = request.TicketedPointID
                };
                request.Lines?.ForEach(o =>
                {
                    var numericalOrderDetail = _numericalOrderCreator.CreateAsync();
                    domain.AddDetail(new FA_AssetsApplyDetail()
                    {
                        NumericalOrder = numericalOrder,
                        NumericalOrderDetail = numericalOrderDetail.Result,
                        AssetsName = o.AssetsName,
                        AssetsTypeID = o.AssetsTypeID,
                        Specification = o.Specification,
                        Brand = o.Brand,
                        MeasureUnit = o.MeasureUnit,
                        Quantity = o.Quantity,
                        TaxRate = o.TaxRate,
                        UnitPrice = o.UnitPrice,
                        Amount = o.Amount,
                        SupplierID =!string.IsNullOrEmpty(o.SupplierID) ? o.SupplierID:"0",
                        ProjectID = !string.IsNullOrEmpty(o.ProjectID) ? o.ProjectID : "0",
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

    public class FA_AssetsApplyDeleteHandler : IRequestHandler<FA_AssetsApplyDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_AssetsApplyRepository _repository;
        IFA_AssetsApplyDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        HttpClientUtil _httpClient;
        HostConfiguration _hostCongfiguration;

        public FA_AssetsApplyDeleteHandler(IIdentityService identityService,
                                            IFA_AssetsApplyRepository repository,
                                            IFA_AssetsApplyDetailRepository detailRepository,
                                            NumericalOrderCreator numericalOrderCreator,
                                            NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                            IBiz_ReviewRepository biz_ReviewRepository,
                                            HttpClientUtil httpClient,
                                            HostConfiguration hostCongfiguration,
                                            IBiz_Related biz_RelatedRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
            _httpClient = httpClient;
            _hostCongfiguration = hostCongfiguration;
        }

        public async Task<Result> Handle(FA_AssetsApplyDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(request.NumericalOrder))
                {
                    var nums = request.NumericalOrder.Split(',').ToList();
                    List<string> newNums = nums.Where((x, i) => nums.FindIndex(z => z == x) == i).ToList();
                    foreach (var item in newNums)
                    {
                        await _repository.RemoveRangeAsync(o => o.NumericalOrder == item);
                        await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == item);
                    }
                    await _repository.UnitOfWork.SaveChangesAsync();
                    await _detailRepository.UnitOfWork.SaveChangesAsync();
                    result.code = ErrorCode.Success.GetIntValue();
                    result.msg = "删除成功!";
                    _httpClient.PostJsonAsync($"{_hostCongfiguration._rdUrl}/api/FAAuditRecord/DeleteAuditRecord", new { request.NumericalOrder });
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

    public class FA_AssetsApplyModifyHandler : IRequestHandler<FA_AssetsApplyModifyCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_AssetsApplyRepository _repository;
        IFA_AssetsApplyDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FA_AssetsApplyModifyHandler(IIdentityService identityService,
                                            IFA_AssetsApplyRepository repository,
                                            IFA_AssetsApplyDetailRepository detailRepository,
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

        public async Task<Result> Handle(FA_AssetsApplyModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.NumericalOrder);
                domain.Update(request.EnterpriseID, request.DataDate, request.MarketID, request.Remarks,request.UpDataInfo,request.TicketedPointID);
                //1增 2改 3删
                foreach (var item in request.Lines)
                {
                    if (item.IsCreate)
                    {
                        var numericalOrderDetail = await this._numericalOrderCreator.CreateAsync();
                        domain.AddDetail(new FA_AssetsApplyDetail()
                        {
                            NumericalOrder = domain.NumericalOrder,
                            NumericalOrderDetail = numericalOrderDetail,
                            AssetsName = item.AssetsName,
                            AssetsTypeID = item.AssetsTypeID,
                            Specification = item.Specification,
                            Brand = item.Brand,
                            MeasureUnit = item.MeasureUnit,
                            Quantity = item.Quantity,
                            TaxRate = item.TaxRate,
                            UnitPrice = item.UnitPrice,
                            Amount = item.Amount,
                            SupplierID = !string.IsNullOrEmpty(item.SupplierID) ? item.SupplierID : "0" ,
                            ProjectID = !string.IsNullOrEmpty(item.ProjectID) ? item.ProjectID : "0",
                            Remarks = item.Remarks,
                        });
                        continue;
                    }
                    if (item.IsUpdate)
                    {
                        var obj = domain.Details?.Find(o => o.NumericalOrderDetail == item.NumericalOrderDetail);
                        obj?.Update(item.AssetsName, item.AssetsTypeID, item.Specification, item.Brand, item.MeasureUnit, item.Quantity, item.TaxRate, item.UnitPrice, item.Amount, item.SupplierID, item.ProjectID, item.Remarks);
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

    public class FA_AssetsApplyCopyHandler : IRequestHandler<FA_AssetsApplyCopyCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_AssetsApplyRepository _repository;
        IFA_AssetsApplyDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FA_AssetsApplyCopyHandler(IIdentityService identityService,
                                            IFA_AssetsApplyRepository repository,
                                            IFA_AssetsApplyDetailRepository detailRepository,
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
        public async Task<Result> Handle(FA_AssetsApplyCopyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = _repository.GetAsync(request.NumericalOrder).Result;
                //long number = _numberCreator.Create<Domain.FA_AssetsApply>(o => o.CreatedDate, o => o.Number, DateTime.Now, o => o.EnterpriseID == _identityService.EnterpriseId);
                long number = _numberCreator.Create<Domain.FA_AssetsApply>(o => o.DataDate, o => o.Number, Convert.ToDateTime(domain.DataDate), o => o.EnterpriseID == _identityService.EnterpriseId);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var copy = new Domain.FA_AssetsApply()
                {
                    NumericalOrder = numericalOrder,
                    OwnerID = this._identityService.UserId,
                    EnterpriseID = domain.EnterpriseID,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    MarketID = domain.MarketID,
                    DataDate = domain.DataDate,
                    Remarks = domain.Remarks,
                    UpDataInfo=domain.UpDataInfo,
                    Number = number.ToString(),
                    TicketedPointID = domain.TicketedPointID
                };
                domain.Details?.ForEach(o =>
                {
                    var numericalOrderDetail = _numericalOrderCreator.CreateAsync();
                    copy.AddDetail(new FA_AssetsApplyDetail()
                    {
                        NumericalOrder = numericalOrder,
                        NumericalOrderDetail = numericalOrderDetail.Result,
                        AssetsName = o.AssetsName,
                        AssetsTypeID = o.AssetsTypeID,
                        Specification = o.Specification,
                        Brand = o.Brand,
                        MeasureUnit = o.MeasureUnit,
                        Quantity = o.Quantity,
                        TaxRate = o.TaxRate,
                        UnitPrice = o.UnitPrice,
                        Amount = o.Amount,
                        SupplierID = !string.IsNullOrEmpty(o.SupplierID) ? o.SupplierID : "0" ,
                        ProjectID = !string.IsNullOrEmpty(o.ProjectID) ? o.ProjectID : "0",
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
