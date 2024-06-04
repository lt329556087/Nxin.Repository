using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFA_InventoryRepository : IRepository<FA_Inventory, string>
    {
    }

    public interface IFA_InventoryDetailRepository : IRepository<FA_InventoryDetail, string>
    {
    }

}
