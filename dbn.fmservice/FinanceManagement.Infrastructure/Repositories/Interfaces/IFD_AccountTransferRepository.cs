using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_AccountTransferRepository : IRepository<FD_AccountTransfer, string>
    {
    }


    public interface IFD_AccountTransferDetailRepository : IRepository<FD_AccountTransferDetail, string>
    {
        FD_AccountTransferDetail GetSingleData(string id);
        FD_AccountTransferDetail GetSingleDataIsIn(string id);
    }
}
