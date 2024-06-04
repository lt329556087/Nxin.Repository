using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_Payment
{
    public class fd_schedulesetODataProvider : OneWithManyQueryEntity<fd_scheduleset>
    {

        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public fd_schedulesetODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<fd_scheduleset> GetList()
        {
            var datas = GetData().AsEnumerable();
            return datas;
        }

        public IQueryable<fd_scheduleset> GetData()
        {
            return GetDatas();
        }

        /// <summary>
        /// 列表`
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<fd_scheduleset> GetDatas(NoneQuery query = null)
        {
            return _context.fd_scheduleset.FromSqlRaw($@"SELECT 
                                                                      `RecordId`,
                                                                      CONCAT(`GroupId`) GroupId,
                                                                      `Level`,
                                                                      `StayDay`,
                                                                      `OwnerId`,
                                                                      `CreatedDate`,
                                                                      `ModifiedDate` 
                                                                    FROM
                                                                      `nxin_qlw_business`.`fd_scheduleset`
                                                                    WHERE GroupId = {_identityservice.GroupId} ").AsQueryable();
        }
    }
}
