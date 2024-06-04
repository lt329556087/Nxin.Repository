using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFA_MarketSubjectRepository : IRepository<FA_MarketSubject, string>
    {
    }


    public interface IFA_MarketSubjectDetailRepository : IRepository<FA_MarketSubjectDetail, string>
    {
    }
}
