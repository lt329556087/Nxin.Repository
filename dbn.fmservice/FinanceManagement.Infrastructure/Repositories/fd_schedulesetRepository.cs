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
    public class fd_schedulesetRepository : Repository<fd_scheduleset, string, Nxin_Qlw_BusinessContext>, Ifd_schedulesetRepository
    {
        public fd_schedulesetRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public override Task<fd_scheduleset> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<fd_scheduleset>().FirstOrDefaultAsync(o => o.RecordId.ToString() == id);
        }

        public List<fd_scheduleset> GetList(string GroupId)
        {
            return DbContext.Set<fd_scheduleset>().FromSqlRaw($@"SELECT 
                                                                      `RecordId`,
                                                                      (`GroupId`) GroupId,
                                                                      `Level`,
                                                                      `StayDay`,
                                                                      `OwnerId`,
                                                                      `CreatedDate`,
                                                                      `ModifiedDate` 
                                                                    FROM
                                                                      `nxin_qlw_business`.`fd_scheduleset`
                                                                    WHERE GroupId = {GroupId} ").ToList();
        }
    }
}
