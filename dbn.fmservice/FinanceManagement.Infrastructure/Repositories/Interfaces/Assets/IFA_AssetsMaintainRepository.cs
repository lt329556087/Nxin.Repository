using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFA_AssetsMaintainRepository : IRepository<FA_AssetsMaintain, string>
    {
    }


    public interface IFA_AssetsMaintainDetailRepository : IRepository<FA_AssetsMaintainDetail, string>
    {
    }
}
