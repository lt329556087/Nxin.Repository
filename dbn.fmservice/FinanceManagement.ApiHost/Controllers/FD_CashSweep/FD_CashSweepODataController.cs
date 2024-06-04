using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.Common;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Architecture.Common.Application.Query;

namespace FinanceManagement.ApiHost.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_CashSweepODataController : ODataController
    {
        FD_CashSweepODataProvider _provider;
        AuditUtil _auditUtil;

        public FD_CashSweepODataController(FD_CashSweepODataProvider provider, AuditUtil auditUtil)
        {
            _provider = provider;
            _auditUtil = auditUtil;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="odataqueryoptions"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        [EnableQuery]
        public IEnumerable<FD_CashSweepODataEntity> Get(ODataQueryOptions<FD_CashSweepODataEntity> odataqueryoptions, Uri uri)
        {
            var data = _provider.GetData(odataqueryoptions, uri).Result;
            return data;
        }
    }
}
