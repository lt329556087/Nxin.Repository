using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFA_AssetsInspectRepository : IRepository<FA_AssetsInspect, string>
    {
    }


    public interface IFA_AssetsInspectDetailRepository : IRepository<FA_AssetsInspectDetail, string>
    {
    }
}
