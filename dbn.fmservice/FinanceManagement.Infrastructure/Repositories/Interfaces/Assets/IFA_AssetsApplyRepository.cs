using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFA_AssetsApplyRepository : IRepository<FA_AssetsApply, string>
    {
    }


    public interface IFA_AssetsApplyDetailRepository : IRepository<FA_AssetsApplyDetail, string>
    {
    }
}
