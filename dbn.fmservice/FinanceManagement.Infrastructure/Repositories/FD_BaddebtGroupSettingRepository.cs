using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace FinanceManagement.Infrastructure.Repositories
{
    public class FD_BaddebtGroupSettingRepository : Repository<FD_BaddebtGroupSetting, string, Nxin_Qlw_BusinessContext>, IFD_BaddebtGroupSettingRepository
    {

        public FD_BaddebtGroupSettingRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FD_BaddebtGroupSetting> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_BaddebtGroupSetting>().Include(o => o.Details).Include(o => o.Extends).FirstOrDefaultAsync(o => o.NumericalOrder == id); //.Include(o => o.Types).Include(o => o.TypeSubjects)
        }
    }

    public class FD_BaddebtGroupSettingDetailRepository : Repository<FD_BaddebtGroupSettingDetail, string, Nxin_Qlw_BusinessContext>, IFD_BaddebtGroupSettingDetailRepository
    {
        public FD_BaddebtGroupSettingDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }

    public class FD_BaddebtGroupSettingExtendRepository : Repository<FD_BaddebtGroupSettingExtend, string, Nxin_Qlw_BusinessContext>, IFD_BaddebtGroupSettingExtendRepository
    {
        public FD_BaddebtGroupSettingExtendRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
    public class FD_IdentificationTypeRepository : Repository<FD_IdentificationType, string, Nxin_Qlw_BusinessContext>, IFD_IdentificationTypeRepository
    {
        public FD_IdentificationTypeRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public List<FD_IdentificationType> GetTypeList(string groupId)
        {
            return DbContext.Set<FD_IdentificationType>().Where(o => o.EnterpriseID == groupId).ToList();
        }
        public List<FD_IdentificationType> GetTypeByID(string num)
        {
            return DbContext.Set<FD_IdentificationType>().Where(o => o.NumericalOrder == num).ToList();
        }
    }
    public class FD_IdentificationTypeSubjectRepository : Repository<FD_IdentificationTypeSubject, string, Nxin_Qlw_BusinessContext>, IFD_IdentificationTypeSubjectRepository
    {
        public FD_IdentificationTypeSubjectRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public List<FD_IdentificationTypeSubject> GetTypeSubjectByID(string num)
        {
            return DbContext.Set<FD_IdentificationTypeSubject>().Where(o => o.NumericalOrder == num).ToList();
        }
    }
}
