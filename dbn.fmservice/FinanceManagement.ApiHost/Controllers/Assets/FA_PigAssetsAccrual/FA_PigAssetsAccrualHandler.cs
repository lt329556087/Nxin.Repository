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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FA_PigAssetsAccrual
{
    public class FA_PigAssetsAccrualAddHandler : IRequestHandler<FA_PigAssetsAccrualAddCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_PigAssetsAccrualRepository _repository;
        IFA_PigAssetsAccrualDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IFA_AssetsApplyDetailRepository _fA_AssetsApplyDetailRepository;
        IFA_AssetsContractDetailRepository _fA_AssetsContractDetailRepository;
        FA_PigAssetsAccrualODataProvider _provider;
        public FA_PigAssetsAccrualAddHandler(IIdentityService identityService,
                                            IFA_PigAssetsAccrualRepository repository,
                                            IFA_PigAssetsAccrualDetailRepository detailRepository,
                                            NumericalOrderCreator numericalOrderCreator,
                                            NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                            IBiz_ReviewRepository biz_ReviewRepository,
                                            IBiz_Related biz_RelatedRepository,
                                            IFA_AssetsApplyDetailRepository fA_AssetsApplyDetailRepository,
                                            IFA_AssetsContractDetailRepository fA_AssetsContractDetailRepository,
                                            FA_PigAssetsAccrualODataProvider provider)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
            _fA_AssetsApplyDetailRepository = fA_AssetsApplyDetailRepository;
            _fA_AssetsContractDetailRepository = fA_AssetsContractDetailRepository;
            _provider = provider;
        }
        public async Task<Result> Handle(FA_PigAssetsAccrualAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (request==null)
                {
                    result.data = null;
                    result.code = ErrorCode.Create.GetIntValue();
                    result.msg = "参数不能为空";
                    return result;
                }
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FA_PigAssetsAccrual()
                {
                    NumericalOrder = numericalOrder,
                    OwnerID = this._identityService.UserId,
                    EnterpriseID = _identityService.EnterpriseId,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    DataDate = request.DataDate,
                    Remarks = request.Remarks,
                };
                var mains= await _provider.GetMains();//获取该单位所有计提数据
                if (mains.Count>0)
                {
                   var updata=   mains.Where(s => s.DataDate == request.DataDate.AddMonths(-1).ToString("yyyy-MM-dd")).FirstOrDefault();
                    if (updata==null)
                    {
                        result.data = null;
                        result.code = ErrorCode.Create.GetIntValue();
                        result.msg = "上月未计提，请先计提上月数据";
                        return result;
                    }
                    else
                    {
                        #region 处理正常需要计提的数据
                        var updetails =  await _provider.GetDetaiDatasAsync(Convert.ToInt64(updata.NumericalOrder));
                        foreach (var item in updetails)
                        {
                            //已折旧完不再进行计提
                            if (item.UseMonth!=item.AlreadyAccruedMonth&& item.NetValue!=0)
                            {
                                var newAlreadyAccruedMonth = item.AlreadyAccruedMonth + 1;
                                var numericalOrderDetail = _numericalOrderCreator.CreateAsync();
                                domain.AddDetail(new FA_PigAssetsAccrualDetail()
                                {
                                    NumericalOrder = numericalOrder,
                                    NumericalOrderDetail = numericalOrderDetail.Result,
                                    NumericalOrderInput = item.NumericalOrderInput,//资产流水号
                                    AssetsCode = item.AssetsCode,
                                    CardCode = item.CardCode,
                                    AssetsName = item.AssetsName,
                                    PigfarmNatureID = item.PigfarmNatureID,
                                    PigfarmID = item.PigfarmID ?? "0",
                                    MarketID = item.MarketID,
                                    AssetsTypeID = item.AssetsTypeID ?? "0",
                                    OriginalValue = item.OriginalValue,
                                    DepreciationMonthAmount = item.DepreciationMonthAmount,
                                    DepreciationMonthRate = item.DepreciationMonthRate,
                                    DepreciationAccumulated = item.UseMonth==newAlreadyAccruedMonth?item.OriginalValue:item.DepreciationAccumulated+ item.DepreciationMonthAmount,
                                    NetValue = item.UseMonth == newAlreadyAccruedMonth ?0:item.NetValue- item.DepreciationMonthAmount,
                                    UseMonth = item.UseMonth,
                                    AlreadyAccruedMonth = newAlreadyAccruedMonth,
                                    CreatedDate = DateTime.Now,
                                    ModifiedDate = DateTime.Now
                                });
                            }
                        }
                        #endregion
                    }
                }
                //如果本月计提过就删掉。重新计提
                if (!string.IsNullOrEmpty(request.NumericalOrder))
                {
                    await _repository.RemoveRangeAsync(o => o.NumericalOrder == request.NumericalOrder);
                    await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == request.NumericalOrder);
                }
                #region 处理本月新重置资产进行计提折旧
                //获取本月新重置资产进行计提折旧
                var pigAsstes = _provider.GetPigAssetsResetByAccrual(request.DataDate.ToString("yyyy-MM-dd"), Convert.ToDateTime(request.DataDate).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd")).ToList();

                    foreach (var item in pigAsstes)
                    {
                        if (item.ResetOriginalValue!=0)
                        {
                            var numericalOrderDetail = _numericalOrderCreator.CreateAsync();
                            //计算月折旧额
                            decimal DepreciationMonthAmount = item.ResetUseYear == 0 ? item.ResetOriginalValue : item.ResetOriginalValue / item.ResetUseYear;
                            FA_PigAssetsAccrualDetail detail = new FA_PigAssetsAccrualDetail();
                            detail.NumericalOrder = numericalOrder;
                            detail.NumericalOrderDetail = numericalOrderDetail.Result;
                            detail.NumericalOrderInput = item.NumericalOrderDetail;//资产流水号
                            detail.AssetsCode = item.AssetsCode;
                            detail.CardCode = item.CardCode;
                            detail.AssetsName = item.AssetsName;
                            detail.PigfarmNatureID = item.PigfarmNatureID;
                            detail.PigfarmID = item.PigfarmID;
                            detail.MarketID = item.MarketID;
                            detail.AssetsTypeID = item.AssetsTypeID;
                            detail.OriginalValue = item.ResetOriginalValue;
                            detail.DepreciationMonthAmount = DepreciationMonthAmount;
                            detail.DepreciationMonthRate = item.ResetUseYear == 0 ? 1 : (decimal)1/item.ResetUseYear;
                            detail.DepreciationAccumulated = DepreciationMonthAmount;
                            detail.NetValue = item.ResetOriginalValue - DepreciationMonthAmount;
                            detail.UseMonth = item.ResetUseYear;
                            detail.AlreadyAccruedMonth = 1;
                            detail.CreatedDate = DateTime.Now;
                            detail.ModifiedDate = DateTime.Now;
                            domain.AddDetail(detail);
                        }
                    }
                if (domain.Details.Count==0)
                {
                    result.data = null;
                    result.code = ErrorCode.Create.GetIntValue();
                    result.msg = "暂无计提数据";
                    return result;
                }
                #endregion
                
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
    public class FA_PigAssetsAccrualDeleteHandler : IRequestHandler<FA_PigAssetsAccrualDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_PigAssetsAccrualRepository _repository;
        IFA_PigAssetsAccrualDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FA_PigAssetsAccrualDeleteHandler(IIdentityService identityService,
                                            IFA_PigAssetsAccrualRepository repository,
                                            IFA_PigAssetsAccrualDetailRepository detailRepository,
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

        public async Task<Result> Handle(FA_PigAssetsAccrualDeleteCommand request, CancellationToken cancellationToken)
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

}
