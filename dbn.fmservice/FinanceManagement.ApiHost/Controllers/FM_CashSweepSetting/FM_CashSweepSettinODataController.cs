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
using FinanceManagement.Util;
using FinanceManagement.Infrastructure.Repositories.Interfaces;

namespace FinanceManagement.ApiHost.Controllers.OData
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FM_CashSweepSettingODataController : ODataController
    {
        FM_CashSweepSettingODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        IFD_BadDebtProvisionRepository _iProvisionRepository;
        public FM_CashSweepSettingODataController(FM_CashSweepSettingODataProvider prodiver, IIdentityService identityService)
        {
            _prodiver = prodiver;
            _identityService = identityService;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        [PermissionAuthorize(Permission.Retrieve)]
        public IEnumerable<FM_CashSweepSettingODataEntity> Get(ODataQueryOptions<FM_CashSweepSettingODataEntity> odataqueryoptions, Uri uri)
        {
            var list = _prodiver.GetList(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value)).ToList();
            return list.AsQueryable();
        }

        #endregion 



    }
}
