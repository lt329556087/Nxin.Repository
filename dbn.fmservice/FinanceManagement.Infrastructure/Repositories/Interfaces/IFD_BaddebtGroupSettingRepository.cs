using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_BaddebtGroupSettingRepository : IRepository<FD_BaddebtGroupSetting, string>
    {
    }


    public interface IFD_BaddebtGroupSettingDetailRepository : IRepository<FD_BaddebtGroupSettingDetail, string>
    {
    }
    public interface IFD_BaddebtGroupSettingExtendRepository : IRepository<FD_BaddebtGroupSettingExtend, string>
    {
    }
    public interface IFD_IdentificationTypeRepository : IRepository<FD_IdentificationType, string> 
    {
        List<FD_IdentificationType> GetTypeList(string groupId);
        List<FD_IdentificationType> GetTypeByID(string num);
    }
    public interface IFD_IdentificationTypeSubjectRepository : IRepository<FD_IdentificationTypeSubject, string> 
    {
        List<FD_IdentificationTypeSubject> GetTypeSubjectByID(string num);
    }
}
