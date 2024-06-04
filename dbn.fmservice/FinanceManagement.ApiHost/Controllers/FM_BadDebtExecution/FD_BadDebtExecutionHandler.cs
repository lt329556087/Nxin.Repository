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

namespace FinanceManagement.ApiHost.Controllers.FD_BadDebtExecution
{
    public class FD_BadDebtExecutionAddHandler : IRequestHandler<FD_BadDebtExecutionAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BadDebtExecutionRepository _repository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FD_BadDebtExecutionAddHandler(IIdentityService identityService, IFD_BadDebtExecutionRepository repository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FD_BadDebtExecutionAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                //long number = _numberCreator.Create<Domain.FD_BadDebtExecution>(o => o.DataDate, o => o.Number, Convert.ToDateTime(request.DataDate), o => o.EnterpriseID == _identityService.EnterpriseId);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FD_BadDebtExecution()
                {
                    //TicketedPointID = request.TicketedPointID,
                    //Number = number.ToString(),
                    //ResponsiblePerson = request.ResponsiblePerson,
                    //Guid = Guid.NewGuid(),
                    //DataDate = Convert.ToDateTime(request.DataDate),
                    //CreatedDate = DateTime.Now,
                    //ModifiedDate = DateTime.Now,
                    //NumericalOrder = numericalOrder,
                    //Remarks = request.Remarks,
                    //OwnerID = _identityService.UserId,
                    //EnterpriseID = _identityService.EnterpriseId
                };

                await _repository.AddAsync(domain);
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

    public class FD_BadDebtExecutionDeleteHandler : IRequestHandler<FD_BadDebtExecutionDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BadDebtExecutionRepository _repository;

        public FD_BadDebtExecutionDeleteHandler(IIdentityService identityService, IFD_BadDebtExecutionRepository repository)
        {
            _identityService = identityService;
            _repository = repository;
        }

        public async Task<Result> Handle(FD_BadDebtExecutionDeleteCommand request, CancellationToken cancellationToken)
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
}
