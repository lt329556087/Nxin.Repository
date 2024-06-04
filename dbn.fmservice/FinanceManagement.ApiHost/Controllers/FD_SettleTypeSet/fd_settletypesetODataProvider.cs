using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_Payment
{
    public class fd_settletypesetODataProvider : OneWithManyQueryEntity<fd_settletypeset>
    {

        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        Ifd_settletypesetRepository _repository;

        public fd_settletypesetODataProvider(IIdentityService identityservice, QlwCrossDbContext context, Ifd_settletypesetRepository repository)
        {
            _identityservice = identityservice;
            _context = context;
            _repository = repository;
        }
        public fd_settletypeset GetData(string GroupId)
        {
            if (string.IsNullOrEmpty(GroupId))
            {
                GroupId = _identityservice.GroupId;
            }
            return _repository.GetAsync(GroupId).Result;
        }
        public dynamic GetSettleType()
        {
            return _context.DynamicSqlQuery(@$"
            SELECT * from nxin_qlw_business.biz_datadict where PID = 201610220104402002  and DataDictID NOT in (201610220104402204,201610220104402205,201610220104402206)
            union
            SELECT * from nxin_qlw_business.biz_datadict where PID = 201610220104402002 and EnterpriseID = {_identityservice.EnterpriseId} 
            ");
        }
    }
}
