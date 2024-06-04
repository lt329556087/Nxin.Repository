using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_VoucherAmortizationRepository : IRepository<FD_VoucherAmortization, string>
    {
    }

    public interface IFD_VoucherAmortizationDetailRepository : IRepository<FD_VoucherAmortizationDetail, string>
    {
    }
    public interface IFD_VoucherAmortizationPeriodDetailRepository : IRepository<FD_VoucherAmortizationPeriodDetail, string>
    {
        Task<List<FD_VoucherAmortizationPeriodDetail>> GetPeriods(string id, CancellationToken cancellationToken = default);
    }
}
