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
    public class fd_settletypesetRepository : Repository<fd_settletypeset, string, Nxin_Qlw_BusinessContext>, Ifd_settletypesetRepository
    {
        public fd_settletypesetRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public override Task<fd_settletypeset> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<fd_settletypeset>().Where(o => o.GroupId == id).FirstOrDefaultAsync();
        }
    }
    public class fd_settletypeRepository : Repository<biz_datadict, string, Nxin_Qlw_BusinessContext>, Ifd_settletypeRepository
    {
        public fd_settletypeRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public override Task<biz_datadict> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<biz_datadict>().Where(o => o.DataDictID == id).FirstOrDefaultAsync();
        }
    }
}
