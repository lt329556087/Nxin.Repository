using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFA_UseStateTransferRepository : IRepository<FA_UseStateTransfer, string>
    {
    }


    public interface IFA_UseStateTransferDetailRepository : IRepository<FA_UseStateTransferDetail, string>
    {
    }
}
