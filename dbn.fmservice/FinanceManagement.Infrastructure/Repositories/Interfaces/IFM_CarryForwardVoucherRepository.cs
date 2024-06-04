using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFM_CarryForwardVoucherRepository : IRepository<FM_CarryForwardVoucher, string>
    {
    }

    public interface IFM_CarryForwardVoucherDetailRepository : IRepository<FM_CarryForwardVoucherDetail, string>
    {
    }
    public interface IFM_CarryForwardVoucherExtendRepository : IRepository<FM_CarryForwardVoucherExtend, string>
    {
        Task<List<FM_CarryForwardVoucherExtend>> GetExtends(string id, CancellationToken cancellationToken = default);
    }
    public interface IFM_CarryForwardVoucherFormulaRepository : IRepository<FM_CarryForwardVoucherFormula, string>
    {
        Task<List<FM_CarryForwardVoucherFormula>> GetExtends(string id, CancellationToken cancellationToken = default);
    }
}
