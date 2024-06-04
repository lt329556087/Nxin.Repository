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

namespace FinanceManagement.ApiHost.Controllers.PerformanceIncome
{
    public class FM_PerformanceIncomeAddHandler : IRequestHandler<FM_PerformanceIncomeAddCommand, Result>
    {
        IIdentityService _identityService;
        IFM_PerformanceIncomeRepository _repository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FM_PerformanceIncomeAddHandler(IIdentityService identityService, IFM_PerformanceIncomeRepository repository,  NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
        }
        public async Task<Result> Handle(FM_PerformanceIncomeAddCommand  request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                List<FM_PerformanceIncome> list = new List<FM_PerformanceIncome>();
                foreach (var item in request.List)
                {
                    FM_PerformanceIncome model = new FM_PerformanceIncome()
                    {
                        ProductGroupID = item.ProductGroupID,
                        ProductGroupName = item.ProductGroupName,
                        ProductGroupTypeName = item.ProductGroupTypeName,
                        IncomeTypeName = item.IncomeTypeName,
                        ParentTypeName = item.ParentTypeName,
                        PropertyName = item.PropertyName,
                        EnterpriseID = _identityService.GroupId.ToString(),
                        CreatedDate=DateTime.Now,
                        ModifiedDate=DateTime.Now
                    };
                    list.Add(model);
                }
                await _repository.AddRangeAsync(list);
                await _repository.UnitOfWork.SaveChangesAsync();
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

}
