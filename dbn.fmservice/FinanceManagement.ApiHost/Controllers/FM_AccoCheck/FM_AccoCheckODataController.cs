using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FM_AccoCheck
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FM_AccoCheckODataController : ControllerBase
    {
        FM_AccoCheckODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;

        public FM_AccoCheckODataController(FM_AccoCheckODataProvider prodiver)
        {
            _prodiver = prodiver;
        }

        #region 列表查询
       
        #endregion 

    }
}
