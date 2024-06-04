using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_BaddebtSettingRepository : IRepository<FD_BaddebtSetting, string>
    {
    }


    public interface IFD_BaddebtSettingDetailRepository : IRepository<FD_BaddebtSettingDetail, string>
    {
    }
}
