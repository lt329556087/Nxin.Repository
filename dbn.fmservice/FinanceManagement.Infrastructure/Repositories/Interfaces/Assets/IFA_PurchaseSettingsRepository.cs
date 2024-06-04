using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFA_PurchaseSettingsRepository : IRepository<FA_PurchaseSettings, string>
    {
    }


    public interface IFA_PurchaseSettingsDetailRepository : IRepository<FA_PurchaseSettingsDetail, string>
    {
    }
}
