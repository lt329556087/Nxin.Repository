using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_AccountInventoryRepository : IRepository<FD_AccountInventory, string>
    {
    }

    public interface IFD_AccountInventoryDetailRepository : IRepository<FD_AccountInventoryDetail, string>
    {
    }
}
