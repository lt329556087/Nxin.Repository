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

namespace FinanceManagement.ApiHost.Controllers.FA_AssetsMaintain
{
    public class FA_AssetsMaintainAddHandler : IRequestHandler<FA_AssetsMaintainAddCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_AssetsMaintainRepository _repository;
        IFA_AssetsMaintainDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FA_AssetsMaintainAddHandler(IIdentityService identityService,
                                            IFA_AssetsMaintainRepository repository,
                                            IFA_AssetsMaintainDetailRepository detailRepository,
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
        public async Task<Result> Handle(FA_AssetsMaintainAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                long number = _numberCreator.Create<Domain.FA_AssetsMaintain>(o => o.DataDate, o => o.Number, Convert.ToDateTime(request.DataDate), o => o.EnterpriseID == _identityService.EnterpriseId);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FA_AssetsMaintain()
                {
                    NumericalOrder = numericalOrder,
                    OwnerID = this._identityService.UserId,
                    EnterpriseID = request.EnterpriseID,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    DataDate = request.DataDate,
                    Remarks = request.Remarks,
                    Number = number.ToString()
                };
                request.Lines?.ForEach(o =>
                {
                    if (o.FileModels == null)
                    {
                        o.FileModels = new List<FileModel>();
                    }
                    var numericalOrderDetail = _numericalOrderCreator.CreateAsync();
                    domain.AddDetail(new FA_AssetsMaintainDetail()
                    {
                        NumericalOrder = numericalOrder,
                        NumericalOrderDetail = numericalOrderDetail.Result,
                        CardID =o.CardID,
                        AssetsName = o.AssetsName,
                        AssetsCode = o.AssetsCode,
                        MaintainID = o.MaintainID,
                        MaintainDate = o.MaintainDate,
                        Content = o.Content,
                        Amount = o.Amount,
                        DepositID = o.DepositID,
                        FileName = string.Join(',', o.FileModels?.Select(s => s.FileName)),
                        FilePath = string.Join(',', o.FileModels?.Select(s => s.FilePath)),
                        PersonID = o.PersonID,
                        Remarks = o.Remarks,
                        ModifiedDate = DateTime.Now,
                        CreatedDate = DateTime.Now,
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

    public class FA_AssetsMaintainDeleteHandler : IRequestHandler<FA_AssetsMaintainDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_AssetsMaintainRepository _repository;
        IFA_AssetsMaintainDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FA_AssetsMaintainDeleteHandler(IIdentityService identityService,
                                            IFA_AssetsMaintainRepository repository,
                                            IFA_AssetsMaintainDetailRepository detailRepository,
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

        public async Task<Result> Handle(FA_AssetsMaintainDeleteCommand request, CancellationToken cancellationToken)
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

    public class FA_AssetsMaintainModifyHandler : IRequestHandler<FA_AssetsMaintainModifyCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_AssetsMaintainRepository _repository;
        IFA_AssetsMaintainDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FA_AssetsMaintainModifyHandler(IIdentityService identityService,
                                            IFA_AssetsMaintainRepository repository,
                                            IFA_AssetsMaintainDetailRepository detailRepository,
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

        public async Task<Result> Handle(FA_AssetsMaintainModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.NumericalOrder);
                domain.Update(request.EnterpriseID, request.DataDate, request.Remarks);
                //1增 2改 3删
                foreach (var item in request.Lines)
                {
                    if (item.FileModels==null)
                    {
                        item.FileModels = new List<FileModel>();
                    }
                    if (item.IsCreate)
                    {
                        var numericalOrderDetail = await this._numericalOrderCreator.CreateAsync();
                        var assetsTypeID = await this._numericalOrderCreator.CreateAsync();
                        domain.AddDetail(new FA_AssetsMaintainDetail()
                        {
                            NumericalOrder = domain.NumericalOrder,
                            NumericalOrderDetail = numericalOrderDetail,
                            AssetsName = item.AssetsName,
                            CardID = item.CardID,
                            AssetsCode = item.AssetsCode,
                            MaintainID = item.MaintainID,
                            MaintainDate = item.MaintainDate,
                            Content = item.Content,
                            Amount = item.Amount,
                            DepositID = item.DepositID,
                            FileName = string.Join(',', item.FileModels?.Select(s => s.FileName)),
                            FilePath = string.Join(',', item.FileModels?.Select(s => s.FilePath)),
                            PersonID = item.PersonID,
                            Remarks = item.Remarks,
                            ModifiedDate = DateTime.Now,
                            CreatedDate = DateTime.Now,
                        });
                        continue;
                    }
                    if (item.IsUpdate)
                    {
                        var obj = domain.Details?.Find(o => o.NumericalOrderDetail == item.NumericalOrderDetail);
                        obj?.Update(item.AssetsName, item.AssetsCode,item.CardID, item.MaintainID, item.MaintainDate, item.Content, item.Amount, item.DepositID, string.Join(',', item.FileModels?.Select(s => s.FileName)), string.Join(',', item.FileModels?.Select(s => s.FilePath)), item.PersonID, item.Remarks);
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
}
