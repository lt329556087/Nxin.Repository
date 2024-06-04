using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_BalanceadJustmentRepository : IRepository<FD_BalanceadJustment, string>
    {
    }

    public interface IFD_BalanceadJustmentDetailRepository : IRepository<FD_BalanceadJustmentDetail, string>
    {
    }
}
