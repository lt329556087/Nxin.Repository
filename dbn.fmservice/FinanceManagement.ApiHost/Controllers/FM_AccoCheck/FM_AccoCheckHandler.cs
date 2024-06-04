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

namespace FinanceManagement.ApiHost.Controllers.FM_AccoCheck
{
    public class FM_AccoCheckAddHandler : IRequestHandler<FM_AccoCheckAddCommand, Result>
    {
        IIdentityService _identityService;
        IFM_AccoCheckRepository _repository;
        IFM_AccoCheckDetailRepository _detailRepository;
        IFM_AccoCheckExtendRepository _extend;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        public FM_AccoCheckAddHandler(IIdentityService identityService, IFM_AccoCheckRepository repository, IFM_AccoCheckDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository, IFM_AccoCheckExtendRepository extend)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _extend = extend;
        }

        public async Task<Result> Handle(FM_AccoCheckAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                FinanceManagement.Domain.FM_AccoCheck domain = null;
                List<FM_AccoCheckExtend> extends = new List<FM_AccoCheckExtend>();
                string numericalOrder = string.Empty;
                List<FinanceManagement.Domain.FM_AccoCheck> checkList = await _repository.GetListByYear(Convert.ToDateTime(request.DataDate).Year, _identityService.EnterpriseId) ?? new List<Domain.FM_AccoCheck>();
                FinanceManagement.Domain.FM_AccoCheck curCheck = checkList.Where(s => s.DataDate.Year == request.DataDate.Year && s.DataDate.Month == request.DataDate.Month && s.DataDate.Day == request.DataDate.Day).FirstOrDefault();
                if (curCheck != null)
                {
                    numericalOrder = curCheck.NumericalOrder;
                }
                else
                {
                    numericalOrder = await _numericalOrderCreator.CreateAsync();
                    domain = new FinanceManagement.Domain.FM_AccoCheck()
                    {
                        NumericalOrder = numericalOrder,
                        DataDate = request.DataDate,
                        StartDate = request.StartDate,
                        EndDate = request.EndDate,
                        CheckMark=request.CheckMark,
                        Remarks = request.Remarks ?? "",
                        OwnerID = request.OwnerID,
                        EnterpriseID = request.EnterpriseID,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                    };
                }
                List<FM_AccoCheckDetail> details = new List<FM_AccoCheckDetail>();
                request.Lines?.ForEach(o =>
                {
                    var numericalOrderDetail = _numericalOrderCreator.CreateAsync();
                    details.Add(new FM_AccoCheckDetail()
                    {
                        NumericalOrder = numericalOrder,
                        NumericalOrderDetail = numericalOrderDetail.Result,
                        AccoCheckType = o.AccoCheckType,
                        CheckMark = o.CheckMark,
                        IsNew = true,
                        OwnerID = request.OwnerID,
                        ModifiedDate = DateTime.Now
                    }) ;
                    o.Extends?.ForEach(s =>
                    {
                        extends.Add(new FM_AccoCheckExtend()
                        {
                            NumericalOrder = numericalOrder,
                            NumericalOrderDetail = numericalOrderDetail.Result,
                            MenuID = Convert.ToInt32(s.MenuID),
                            CheckMark = s.CheckMark,
                            ModifiedDate = DateTime.Now
                        });
                    });
                });
                if (domain != null)
                {
                    await _repository.AddAsync(domain);
                }
                if (details.Count > 0)
                {
                    await _detailRepository.AddRangeAsync(details);
                }
                if (extends.Count > 0)
                {
                    await _extend.AddRangeAsync(extends);
                }
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _extend.UnitOfWork.SaveChangesAsync();
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

    public class FM_AccoCheckDeleteHandler : IRequestHandler<FM_AccoCheckDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFM_AccoCheckRepository _repository;
        IFM_AccoCheckDetailRepository _detailRepository;
        IFM_AccoCheckExtendRepository _extend;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FM_AccoCheckDeleteHandler(IIdentityService identityService, IBiz_ReviewRepository biz_ReviewRepository, IFM_AccoCheckRepository repository, IFM_AccoCheckDetailRepository detailRepository, IFM_AccoCheckExtendRepository extend)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _extend = extend;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FM_AccoCheckDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (string.IsNullOrEmpty(request.NumericalOrder))
                {
                    result.code = ErrorCode.Delete.GetIntValue();
                    result.msg = "参数有误";
                    return result;
                }
                if (request.IsSinge)
                {
                    await _detailRepository.RemoveRangeAsync(o => o.NumericalOrderDetail == request.Lines.FirstOrDefault().NumericalOrderDetail);
                    await _extend.RemoveRangeAsync(o => o.NumericalOrderDetail == request.Lines.FirstOrDefault().NumericalOrderDetail);
                }
                else
                {
                    await _repository.RemoveRangeAsync(o => o.EnterpriseID == _identityService.EnterpriseId && o.NumericalOrder == request.NumericalOrder);
                    await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == request.NumericalOrder);
                    await _extend.RemoveRangeAsync(o => o.NumericalOrder == request.NumericalOrder);
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

    public class FM_AccoCheckModifyHandler : IRequestHandler<FM_AccoCheckModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFM_AccoCheckRepository _repository;
        NumericalOrderCreator _numericalOrderCreator;
        public FM_AccoCheckModifyHandler(IIdentityService identityService, IFM_AccoCheckRepository repository, NumericalOrderCreator numericalOrderCreator)
        {
            _identityService = identityService;
            _repository = repository;
            _numericalOrderCreator = numericalOrderCreator;
        }

        public async Task<Result> Handle(FM_AccoCheckModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.NumericalOrder.ToString());
                //修改本月结账状态
                domain.CheckMark = request.CheckMark;
                await _repository.UnitOfWork.SaveChangesAsync();
                result.data = new { RecordID = request.NumericalOrder };
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
                result.msg = ex.ToString();
            }
            return result;
        }
    }


}
