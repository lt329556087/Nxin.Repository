using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories
{
    public class fd_scheduleplanRepository : Repository<fd_scheduleplan, string, Nxin_Qlw_BusinessContext>, Ifd_scheduleplanRepository
    {
        public fd_scheduleplanRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public override Task<fd_scheduleplan> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<fd_scheduleplan>().FirstOrDefaultAsync(o => o.NumericalOrder.ToString() == id);
        }

        public List<fd_scheduleplan> GetList(string GroupId)
        {
            return DbContext.Set<fd_scheduleplan>().FromSqlRaw($@"SELECT 
                                                                  `NumericalOrder`,
                                                                  `GroupId`,
                                                                  `ApplyData`,
                                                                  `ApplyEnterpriseId`,
                                                                  `ApplyMenuId`,
                                                                  `ApplyNumericalOrder`,
                                                                  `ApplyPayContent`,
                                                                   ApplyContactType,
                                                                  `ApplyContactEnterpriseId`,
                                                                  `ApplyEmergency`,
                                                                  `ApplyDeadLine`,
                                                                  `ApplyAmount`,
                                                                  PayNumericalOrder,
                                                                  `ApplySurplusAmount`,
                                                                  `PayAmount`,
                                                                  `DeadLine`,
                                                                  `Level`,
                                                                  `SettlementMethod`,
                                                                  `ScheduleStatus`,
                                                                  `OwnerId`,
                                                                  `CreatedDate`,
                                                                  `ModifiedDate` 
                                                                FROM
                                                                  `nxin_qlw_business`.`fd_scheduleplan` 
                                                                    WHERE GroupId = {GroupId} ").ToList();
        }
    }
}
