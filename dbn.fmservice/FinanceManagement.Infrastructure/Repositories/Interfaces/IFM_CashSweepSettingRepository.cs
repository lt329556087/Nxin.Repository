using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFM_CashSweepSettingRepository : IRepository<FM_CashSweepSetting, string>
    {
    }


    public interface IFM_CashSweepSettingDetailRepository : IRepository<FM_CashSweepSettingDetail, string>
    {
    }
    public interface IFM_CashSweepSettingExtRepository : IRepository<FM_CashSweepSettingExt, string>
    {
    }
}
